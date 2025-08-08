// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.MarkupSystem
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.CodeModel.Cpp;
using Microsoft.Iris.Data;
using Microsoft.Iris.Debug;
using Microsoft.Iris.Markup.Validation;
using Microsoft.Iris.OS;
using Microsoft.Iris.Session;
using Microsoft.Iris.UI;
using System;

namespace Microsoft.Iris.Markup
{
    public static class MarkupSystem
    {
        public const string RootTag = "UIX";
        public const string SchemaVersion = "http://schemas.microsoft.com/2007/uix";
        public const string DataSchemaRootTag = "DataSchema";
        public const string DataSchemaVersion = "http://schemas.microsoft.com/2007/uixdata";
        public static UIXLoadResult UIXGlobal;
        internal static RootLoadResult RootGlobal;
        public static bool CompileMode;
        public static bool TrackAdditionalMetadata;
        public static bool MarkupSystemActive;
        private static Vector s_factoriesByProtocol;
        private static Vector s_factoriesByExtension;
        private static uint s_rootIslandId;
        private static uint s_activeIslands;

        public static event EventHandler<LoadResult> NewMarkupLoaded;

        public static void Startup(bool compileMode)
        {
            MarkupSystemActive = true;
            CompileMode = compileMode;
            s_factoriesByProtocol = new Vector();
            s_factoriesByExtension = new Vector();
            s_rootIslandId = AllocateIslandId();
            UIXGlobal = new UIXLoadResult("http://schemas.microsoft.com/2007/uix");
            UIXGlobal.RegisterUsage(typeof(MarkupSystem));
            UIXLoadResult.InitializeStatics();
            ValidateContext.InitializeStatics();
            ValidateUI.InitializeStatics();
            ValidateClass.InitializeStatics();
            ValidateStatementForEach.InitializeStatics();
            ValidateParameter.InitializeStatics();
            TypeRestriction.InitializeStatics();
            NativeMarkupDataQuery.InitializeStatics();
            NativeMarkupDataType.InitializeStatics();
            RootGlobal = new RootLoadResult("Root");
            RootGlobal.RegisterUsage(typeof(MarkupSystem));
            AssemblyLoadResult.Startup();
            DllLoadResult.Startup();
            ResourceManager.Instance.RegisterSource("res", DllResources.Instance);
            ResourceManager.Instance.RegisterSource(ClrDllResources.Scheme, ClrDllResources.Instance);
            HttpResources.Startup();
            ResourceManager.Instance.RegisterSource("file", FileResources.Instance);
        }

        public static void Shutdown()
        {
            UnloadAll();
            AssemblyLoadResult.Shutdown();
            DllLoadResult.Shutdown();
            RootGlobal.UnregisterUsage(typeof(MarkupSystem));
            RootGlobal = null;
            UIXGlobal.UnregisterUsage(typeof(MarkupSystem));
            UIXGlobal = null;
            HttpResources.Shutdown();
            if (s_factoriesByProtocol != null)
                s_factoriesByProtocol.Clear();
            if (s_factoriesByExtension != null)
                s_factoriesByExtension.Clear();
            MarkupSystemActive = false;
        }

        public static void EnableMetadataTracking() => TrackAdditionalMetadata = true;

        public static LoadResult Load(string uri, uint islandId)
        {
            ErrorManager.EnterContext(uri);
            LoadResult loadResult = ResolveLoadResult(uri, islandId);
            if (loadResult != null)
            {
                loadResult.Load(LoadPass.DeclareTypes);
                loadResult.Load(LoadPass.PopulatePublicModel);
                loadResult.Load(LoadPass.Full);
                loadResult.Load(LoadPass.Done);

                NewMarkupLoaded?.Invoke(null, loadResult);
            }
            ErrorManager.ExitContext();
            return loadResult;
        }

        public static LoadResult ResolveLoadResult(string uri, uint islandId)
        {
            ErrorManager.EnterContext(uri);
            var loadResult = LoadResultCache.Read(uri);
            
            if (loadResult == null)
            {
                var handled = false;
                var cacheResult = true;
                
                foreach (Factory factory in s_factoriesByProtocol)
                {
                    if (uri.StartsWith(factory.key, StringComparison.Ordinal))
                    {
                        loadResult = factory.handler(uri);
                        handled = true;
                        break;
                    }
                }

                if (!handled)
                {
                    foreach (Factory factory in s_factoriesByExtension)
                    {
                        if (uri.EndsWith(factory.key, StringComparison.Ordinal))
                        {
                            loadResult = factory.handler(uri);
                            handled = true;
                            break;
                        }
                    }
                }
                
                if (!handled)
                    loadResult = CreateMarkupLoadResult(uri, ref cacheResult);
                
                loadResult ??= new ErrorLoadResult(uri);
                
                if (cacheResult && loadResult.Cachable)
                {
                    LoadResultCache.Write(uri, loadResult);
                    if (loadResult.UnderlyingUri != null)
                        LoadResultCache.Write(loadResult.UnderlyingUri, loadResult);
                }
            }

            loadResult?.AddReference(islandId);
            ErrorManager.ExitContext();

            return loadResult;
        }

        private static LoadResult CreateMarkupLoadResult(string uri, ref bool cacheResult)
        {
            Resource resource = ResourceManager.AcquireResource(uri);
            if (resource == null)
                return null;
            ErrorManager.EnterContext(resource.Uri);
            LoadResult loadResult = LoadResultCache.Read(resource.Uri);
            if (loadResult != null)
            {
                cacheResult = false;
                LoadResultCache.Write(uri, loadResult);
            }
            else
            {
                loadResult = MarkupLoadResult.Create(uri, resource);
                if (loadResult != null)
                    resource = null;
            }
            resource?.Free();
            ErrorManager.ExitContext();
            return loadResult;
        }

        public static uint RootIslandId => s_rootIslandId;

        public static uint AllocateIslandId()
        {
            int num1 = ~(int)s_activeIslands;
            uint num2 = (uint)(num1 & -num1);
            s_activeIslands |= num2;
            return num2;
        }

        public static void FreeIslandId(uint islandId) => s_activeIslands &= ~islandId;

        public static void UnloadIsland(uint islandId) => LoadResultCache.Remove(islandId);

        public static void UnloadAll() => LoadResultCache.Clear();

        public static bool RegisterFactoryByProtocol(string protocol, CreateLoadResultHandler handler)
        {
            if (protocol.EndsWith("://", StringComparison.Ordinal))
                protocol = protocol.Substring(0, protocol.Length - 3);
            bool flag = !ResourceManager.Instance.IsRegisteredSource(protocol);
            protocol += "://";
            if (flag)
            {
                foreach (MarkupSystem.Factory factory in s_factoriesByProtocol)
                {
                    if (factory.key == protocol)
                        flag = false;
                }
            }
            if (flag)
                s_factoriesByProtocol.Add(new MarkupSystem.Factory()
                {
                    key = protocol,
                    handler = handler
                });
            return flag;
        }

        public static bool RegisterFactoryByExtension(string extension, CreateLoadResultHandler handler)
        {
            bool flag = true;
            if (!extension.StartsWith(".", StringComparison.Ordinal))
                extension = "." + extension;
            foreach (MarkupSystem.Factory factory in s_factoriesByExtension)
            {
                if (factory.key == extension)
                    flag = false;
            }
            if (flag)
                s_factoriesByExtension.Add(new MarkupSystem.Factory()
                {
                    handler = handler,
                    key = extension
                });
            return flag;
        }

        [Obsolete]
        public static void AddImportRedirect(string fromPrefix, string toPrefix) { }

        [Obsolete]
        private static string ApplyImportRedirects(string uri) => uri;

        public static bool IsDebuggingEnabled(byte level) => !CompileMode && Trace.IsCategoryEnabled(TraceCategory.MarkupDebug, level);

        internal class Factory
        {
            public CreateLoadResultHandler handler;
            public string key;
        }
    }
}
