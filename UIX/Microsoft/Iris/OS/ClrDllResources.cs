using Microsoft.Iris.Data;
using Microsoft.Iris.Library;
using Microsoft.Iris.Markup;
using Microsoft.Iris.Session;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.OS
{
    internal class ClrDllResources : IResourceProvider
    {
        public const string Scheme = "clr-res";

        private static ClrDllResources s_instance = new ClrDllResources();
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

            DllResources.ParseResource(hierarchicalPart, out string host, out string identifier);

            if (host != null)
            {
                Assembly assembly = GetAssembly(host);
                if (assembly != null)
                    resource = new ClrDllResource(uri, assembly, identifier);
            }
            
            return resource is not null;
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
    }
}
