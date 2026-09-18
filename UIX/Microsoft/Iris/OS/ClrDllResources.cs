using System.Collections;
using Microsoft.Iris.Data;
using Microsoft.Iris.Library;
using Microsoft.Iris.Markup;
using Microsoft.Iris.Session;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using ResourceManager = System.Resources.ResourceManager;

namespace Microsoft.Iris.OS
{
    internal class ClrDllResources : IResourceProvider, IEnumerableResourceProvider
    {
        public const string Scheme = "clr-res";

        private static ClrDllResources s_instance = new();
        private Dictionary<string, Assembly> _shortNameToAssembly;

        private ClrDllResources() => _shortNameToAssembly = new Dictionary<string, Assembly>(InvariantString.OrdinalIgnoreCaseComparer);

        public static ClrDllResources Instance => s_instance;

        public Resource GetResource(string hierarchicalPart, string uri, bool forceSynchronous)
        {
            if (!TryGetResource(hierarchicalPart, uri, forceSynchronous, out var resource))
                ErrorManager.ReportError($"Invalid resource uri: '{uri}'");

            return resource;
        }

        public bool TryGetResource(string hierarchicalPart, string uri, bool forceSynchronous, out Resource resource)
        {
            resource = null;

            ParseResource(hierarchicalPart, out var host, out var identifier, out var specifier);

            if (host != null)
            {
                var assembly = GetAssembly(host);
                if (assembly != null)
                {
                    ResourceManager rm = new(specifier, assembly);
                    resource = new ClrDllResource(uri, rm, assembly, identifier, specifier);
                }
            }
            
            return resource is not null;
        }

        public IEnumerable<Resource> EnumerateResources(string baseHierarchicalPart, string baseUri, bool forceSynchronous)
        {
            ParseResource(baseHierarchicalPart, out var host, out _, out var specifier);
            if (host == null)
                return null;
            
            var assembly = GetAssembly(host);
            if (assembly == null)
                return null;
            
            ResourceManager rm = new(specifier, assembly);
            var resourceSet = rm.GetResourceSet(CultureInfo.InvariantCulture, true, true);
            return resourceSet is not null
                ? ConvertResourceSet()
                : null;

            IEnumerable<ClrDllResource> ConvertResourceSet()
            {
                foreach (DictionaryEntry clrResource in resourceSet)
                {
                    var identifier = clrResource.Key.ToString();
                    var uri = Path.Join(baseUri, identifier);
                    yield return new ClrDllResource(uri, rm, assembly, identifier, specifier);
                }
            }
        }

        public Assembly GetAssembly(string moduleName)
        {
            if (!_shortNameToAssembly.TryGetValue(moduleName, out Assembly a))
            {
                AssemblyName name = null;
                try
                {
                    name = new AssemblyName(moduleName);
                }
                catch (COMException)
                {
                }
                catch (IOException)
                {
                }

                if (name != null)
                    a = AssemblyLoadResult.FindAssembly(name, out _);
                _shortNameToAssembly[moduleName] = a;
            }
            return a;
        }

        private static void ParseResource(string hierarchicalPart, out string host, out string identifier,
            out string specifier)
        {
            Resource.ParseResource(hierarchicalPart, out host, out identifier);

            specifier = "RCDATA";
            var specifierIndex = identifier.IndexOf('/');
            if (specifierIndex >= 0)
            {
                specifier = identifier.Substring(0, specifierIndex);
                identifier = identifier.Substring(specifierIndex + 1);
            }
        }
    }
}
