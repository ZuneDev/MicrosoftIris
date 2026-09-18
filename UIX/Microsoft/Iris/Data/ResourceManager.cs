// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Data.ResourceManager
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Session;
using System;
using System.Collections.Generic;

namespace Microsoft.Iris.Data
{
    public sealed class ResourceManager
    {
        internal const string ProtocolSeparator = "://";
        private Vector<ResourceManager.UriRedirect> _redirects;
        private Map<string, IResourceProvider> _sourcesTable;
        private static ResourceManager s_instance = new ResourceManager();

        private ResourceManager() => _sourcesTable = new Map<string, IResourceProvider>();

        public static ResourceManager Instance => s_instance;

        public int Bank { get; set; } = 0;

        public void RegisterSource(string scheme, IResourceProvider source) => _sourcesTable[scheme] = source;

        public void UnregisterSource(string scheme) => _sourcesTable.Remove(scheme);

        public bool IsRegisteredSource(string scheme) => _sourcesTable.ContainsKey(scheme);

        public Resource GetResource(string uri) => GetResource(uri, false);

        public Resource GetResource(string uri, bool forceSynchronous)
        {
            Resource resource = null;

            foreach (var uriCandidate in ApplyRedirects(uri))
            {
                resource = GetResourceWorker(uriCandidate, forceSynchronous);
                if (resource is null)
                    continue;
                
                resource.Acquire();
                var success = resource.Status is ResourceStatus.Available;
                resource.Free();
                
                if (!success)
                    resource = null;
                else
                    break;
            }
            
            return resource;
        }

        public IEnumerable<Resource> EnumerateResources(string baseUri, bool forceSynchronous)
        {
            var resourceProvider = GetProvider(baseUri, out var baseHierarchicalPart);
            if (resourceProvider is null)
                return [];
            
            if (resourceProvider is not IEnumerableResourceProvider enumerableResourceProvider)
            {
                ErrorManager.ReportWarning("Resource provider `{0}` does not support enumeration for '{1}'",
                    resourceProvider.GetType().FullName, baseUri);
                return [];
            }
            
            return enumerableResourceProvider.EnumerateResources(baseHierarchicalPart, baseUri, forceSynchronous);
        }
        
        private IEnumerable<string> ApplyRedirects(string uri)
        {
            if (_redirects == null)
            {
                yield return uri;
                yield break;
            }

            foreach (var redirect in _redirects)
            {
                if (redirect.bank is not -1 && redirect.bank != Bank)
                    continue;

                if (!uri.StartsWith(redirect.fromPrefix, StringComparison.OrdinalIgnoreCase))
                    continue;

                if (redirect.toPrefix.Equals("{ERROR}", StringComparison.OrdinalIgnoreCase))
                {
                    ErrorManager.ReportError(
                        "Resource {0} not found, but should have been located by a markup redirect", uri);
                    continue;
                }

                yield return redirect.toPrefix + uri.Substring(redirect.fromPrefix.Length);
            }

            yield return uri;
        }

        private IResourceProvider GetProvider(string uri, out string hierarchicalPart)
        {
            ParseUri(uri, out var scheme, out hierarchicalPart);

            if (string.IsNullOrEmpty(scheme) || string.IsNullOrEmpty(hierarchicalPart))
            {
                ErrorManager.ReportWarning("Invalid resource uri: '{0}'", uri);
                return null;
            }

            if (!_sourcesTable.TryGetValue(scheme, out var resourceProvider))
            {
                ErrorManager.ReportWarning("Invalid resource protocol: '{0}'", scheme);
                return null;
            }
            
            return resourceProvider;
        }

        private Resource GetResourceWorker(string uri, bool forceSynchronous)
        {
            var resourceProvider = GetProvider(uri, out var hierarchicalPart);
            return resourceProvider.GetResource(hierarchicalPart, uri, forceSynchronous);
        }

        public static void ParseUri(string uri, out string scheme, out string hierarchicalPart)
        {
            int length = uri.IndexOf("://", StringComparison.Ordinal);
            if (length > 0)
            {
                scheme = uri.Substring(0, length);
                hierarchicalPart = uri.Substring(length + "://".Length);
            }
            else
            {
                scheme = null;
                hierarchicalPart = uri;
            }
        }

        public void AddUriRedirect(string fromPrefix, string toPrefix) => AddUriRedirect(fromPrefix, toPrefix, -1);

        public void AddUriRedirect(string fromPrefix, string toPrefix, int bank)
        {
            UriRedirect uriRedirect = new()
            {
                fromPrefix = fromPrefix,
                toPrefix = toPrefix,
                bank = bank
            };

            _redirects ??= new Vector<UriRedirect>();
            _redirects.Add(uriRedirect);
        }

        public static Resource AcquireResource(string uri)
        {
            ErrorWatermark watermark = ErrorManager.Watermark;
            Resource resource = Instance.GetResource(uri, true);
            if (resource == null)
                return null;
            resource.Acquire();
            if (resource.Status == ResourceStatus.Error)
            {
                if (resource.ErrorDetails != null)
                    ErrorManager.ReportError(resource.ErrorDetails);
                else
                    ErrorManager.ReportError("Failed to acquire resource '{0}'", uri);
            }
            else if (resource.Status != ResourceStatus.Available)
                ErrorManager.ReportError("Failed to acquire resource '{0}'.  Resources that cannot be fetched synchronously are not valid in this context", uri);
            if (watermark.ErrorsDetected)
            {
                resource.Free();
                resource = null;
            }
            return resource;
        }

        internal struct UriRedirect
        {
            public string fromPrefix;
            public string toPrefix;
            public int bank;
        }
    }
}
