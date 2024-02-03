// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Data.ResourceManager
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Session;
using System;

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

        public void RegisterSource(string scheme, IResourceProvider source) => _sourcesTable[scheme] = source;

        public void UnregisterSource(string scheme) => _sourcesTable.Remove(scheme);

        public bool IsRegisteredSource(string scheme) => _sourcesTable.ContainsKey(scheme);

        public Resource GetResource(string uri) => GetResource(uri, false);

        public Resource GetResource(string uri, bool forceSynchronous)
        {
            Resource resource = null;
            if (_redirects != null)
            {
                foreach (ResourceManager.UriRedirect redirect in _redirects)
                {
                    if (uri.StartsWith(redirect.fromPrefix, StringComparison.OrdinalIgnoreCase))
                    {
                        if (redirect.toPrefix.Equals("{ERROR}", StringComparison.OrdinalIgnoreCase))
                        {
                            ErrorManager.ReportError("Resource {0} not found, but should have been located by a markup redirect", uri);
                            return null;
                        }
                        resource = GetResourceWorker(redirect.toPrefix + uri.Substring(redirect.fromPrefix.Length), true);
                        if (resource != null)
                        {
                            resource.Acquire();
                            bool flag = resource.Status == ResourceStatus.Available;
                            resource.Free();
                            if (!flag)
                                resource = null;
                        }
                    }
                    if (resource != null)
                        break;
                }
            }
            if (resource == null)
                resource = GetResourceWorker(uri, forceSynchronous);
            return resource;
        }

        private Resource GetResourceWorker(string uri, bool forceSynchronous)
        {
            Resource resource = null;
            string scheme;
            string hierarchicalPart;
            ParseUri(uri, out scheme, out hierarchicalPart);
            if (string.IsNullOrEmpty(scheme) || string.IsNullOrEmpty(hierarchicalPart))
            {
                ErrorManager.ReportWarning("Invalid resource uri: '{0}'", uri);
                return null;
            }
            IResourceProvider resourceProvider;
            if (_sourcesTable.TryGetValue(scheme, out resourceProvider))
                resource = resourceProvider.GetResource(hierarchicalPart, uri, forceSynchronous);
            else
                ErrorManager.ReportWarning("Invalid resource protocol: '{0}'", scheme);
            return resource;
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

        public void AddUriRedirect(string fromPrefix, string toPrefix)
        {
            ResourceManager.UriRedirect uriRedirect = new ResourceManager.UriRedirect();
            uriRedirect.fromPrefix = fromPrefix;
            uriRedirect.toPrefix = toPrefix;
            if (_redirects == null)
                _redirects = new Vector<ResourceManager.UriRedirect>();
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
        }
    }
}
