// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.CodeModel.Cpp.DllLoadResultFactory
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;
using Microsoft.Iris.Markup;
using Microsoft.Iris.OS;
using Microsoft.Iris.Session;
using System;

namespace Microsoft.Iris.CodeModel.Cpp
{
    internal class DllLoadResultFactory : SharedDisposableObject
    {
        private const string c_DllProtocol = "dll://";
        private string _dllName;
        private IntPtr _module;
        private IntPtr _schemaFactory;
        private static Map<string, DllLoadResult> s_loadResultCache = new Map<string, DllLoadResult>();
        private static Map<uint, DllLoadResult> s_loadResultIDCache = new Map<uint, DllLoadResult>();
        private static Map<string, DllLoadResultFactory> s_dllFactoriesCache = new Map<string, DllLoadResultFactory>();

        public static void Startup() => MarkupSystem.RegisterFactoryByProtocol("dll://", new CreateLoadResultHandler(GetLoadResult));

        public static DllLoadResult GetLoadResultByID(uint id)
        {
            DllLoadResult dllLoadResult;
            s_loadResultIDCache.TryGetValue(id, out dllLoadResult);
            return dllLoadResult;
        }

        private static LoadResult GetLoadResult(string uri)
        {
            DllLoadResult dllLoadResult;
            if (s_loadResultCache.TryGetValue(uri, out dllLoadResult))
                return dllLoadResult;
            int length = uri.IndexOf('!');
            string str;
            string qualifier;
            if (length != -1)
            {
                str = uri.Substring("dll://".Length, length);
                qualifier = uri.Substring(length + 1);
            }
            else
            {
                str = uri.Substring("dll://".Length);
                qualifier = null;
            }
            DllLoadResultFactory loadResultFactory;
            if (!s_dllFactoriesCache.TryGetValue(str, out loadResultFactory))
            {
                loadResultFactory = new DllLoadResultFactory(str);
                s_dllFactoriesCache[str] = loadResultFactory;
            }
            DllLoadResult loadResult = loadResultFactory.GetLoadResult(uri, qualifier);
            if (loadResult != null)
            {
                s_loadResultCache[uri] = loadResult;
                s_loadResultIDCache[loadResult.SchemaComponent] = loadResult;
            }
            return loadResult;
        }

        private DllLoadResultFactory(string dllName)
        {
            _dllName = dllName;
            int num = (int)NativeApi.SpLoadDll(_dllName, out _module);
            if ((int)NativeApi.SpCreateDllLoadResultFactory(_module, out _schemaFactory) >= 0)
                return;
            ErrorManager.ReportError("Unable to create IUIXSchemaFactory from '{0}'", dllName);
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            s_dllFactoriesCache.Remove(_dllName);
            if (_schemaFactory != IntPtr.Zero)
            {
                NativeApi.SpReleaseExternalObject(_schemaFactory);
                _schemaFactory = IntPtr.Zero;
            }
            if (!(_module != IntPtr.Zero))
                return;
            NativeApi.SpSendDllSchemaUnloadNotification(_module);
            NativeApi.SpFreeDll(_module);
            _module = IntPtr.Zero;
        }

        private DllLoadResult GetLoadResult(string fullUri, string qualifier)
        {
            DllLoadResult dllLoadResult = null;
            IntPtr loadResult = IntPtr.Zero;
            if (_schemaFactory != IntPtr.Zero)
            {
                if ((int)NativeApi.SpCreateDllLoadResult(_schemaFactory, qualifier, out loadResult) < 0)
                    ErrorManager.ReportError("Unable to create IUIXSchema from '{0}'", fullUri);
                else if (loadResult != IntPtr.Zero)
                {
                    dllLoadResult = new DllLoadResult(this, loadResult, fullUri);
                    RegisterUsage(dllLoadResult);
                }
                else
                    ErrorManager.ReportError("NULL object returned from {0}", "IUIXSchemaFactory::GetSchema");
            }
            return dllLoadResult;
        }

        public void NotifyLoadResultDisposed(DllLoadResult loadResult)
        {
            s_loadResultCache.Remove(loadResult.Uri);
            s_loadResultIDCache.Remove(loadResult.SchemaComponent);
            UnregisterUsage(loadResult);
        }
    }
}
