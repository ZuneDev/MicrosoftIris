// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.OS.DllResources
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

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
    internal class DllResources : IResourceProvider
    {
        private static DllResources s_instance = new DllResources();
        private static bool s_staticDllResourcesOnly = true;
        private Dictionary<string, string> _shortNameToFullPath;

        private DllResources() => _shortNameToFullPath = new Dictionary<string, string>(InvariantString.OrdinalIgnoreCaseComparer);

        public static DllResources Instance => s_instance;

        public static bool StaticDllResourcesOnly
        {
            get => s_staticDllResourcesOnly;
            set => s_staticDllResourcesOnly = value;
        }

        public Resource GetResource(string hierarchicalPart, string uri, bool forceSynchronous)
        {
            Resource resource = null;
            string host;
            string identifier;
            ParseResource(hierarchicalPart, out host, out identifier);
            if (host != null)
            {
                string fullPath = GetFullPath(host);
                if (fullPath != null)
                    resource = new DllResource(uri, fullPath, identifier);
            }
            if (resource == null)
                ErrorManager.ReportError("Invalid resource uri: '{0}'", uri);
            return resource;
        }

        public string GetFullPath(string moduleName)
        {
            string str = null;
            if (!_shortNameToFullPath.TryGetValue(moduleName, out str))
            {
                AssemblyName name = null;
                try
                {
                    name = new AssemblyName(moduleName);
                }
                catch (COMException ex)
                {
                }
                catch (IOException ex)
                {
                }
                Assembly assembly = null;
                if (name != null)
                    assembly = AssemblyLoadResult.FindAssembly(name, out Exception _);
                str = assembly == null ? moduleName : assembly.Location;
                _shortNameToFullPath[moduleName] = str;
            }
            return str;
        }

        internal static void ParseResource(string resource, out string host, out string identifier)
        {
            int length = resource.IndexOf('!');
            if (length == -1)
            {
                host = null;
                identifier = resource;
            }
            else
            {
                host = resource.Substring(0, length);
                identifier = resource.Substring(length + 1);
            }
        }
    }
}
