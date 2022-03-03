// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.CodeModel.Cpp.DllProxyObject
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;
using Microsoft.Iris.Markup;
using Microsoft.Iris.OS;
using Microsoft.Iris.RenderAPI;
using Microsoft.Iris.Session;
using System;

namespace Microsoft.Iris.CodeModel.Cpp
{
    internal class DllProxyObject : NotifyObjectBase, ISchemaInfo, IDisposableObject
    {
        private IntPtr _nativeObject;
        private ulong _handle;
        private DllTypeSchemaBase _type;
        private static object s_finalizeLock = new object();
        private static bool s_pendingAppThreadRelease = false;
        private static Vector<DllProxyObject.AppThreadReleaseEntry> s_pendingReleases;
        private static SimpleCallback s_releaseOnAppThread = new SimpleCallback(ReleaseFinalizedObjects);
        private static DllProxyObjectHandleTable s_handleTable;

        public static void CreateHandleTable() => s_handleTable = new DllProxyObjectHandleTable();

        public static void ReleaseOutstandingProxies()
        {
            s_pendingAppThreadRelease = true;
            GC.Collect();
            GC.WaitForPendingFinalizers();
            foreach (IDisposableObject disposableObject in s_handleTable)
                disposableObject.Dispose(disposableObject);
            ReleaseFinalizedObjects();
        }

        public static DllProxyObject Wrap(IntPtr nativeObject)
        {
            DllProxyObject dllProxyObject = null;
            uint typeID;
            if (!NativeApi.SUCCEEDED(NativeApi.SpGetTypeID(nativeObject, out typeID)))
                return null;
            TypeSchema type = DllLoadResult.MapType(typeID);
            if (type != null)
            {
                dllProxyObject = GetExistingProxy(nativeObject) ?? WrapNewObject(nativeObject, type);
                NativeApi.SpReleaseExternalObject(nativeObject);
            }
            return dllProxyObject;
        }

        private static DllProxyObject GetExistingProxy(IntPtr nativeObject)
        {
            DllProxyObject dllProxyObject = null;
            ulong state;
            NativeApi.SpGetStateCache(nativeObject, out state);
            if (state != 0UL && !s_handleTable.LookupByHandle(state, out dllProxyObject))
                ErrorManager.ReportError("IUIXObject::GetStateCache retrieved unexpected value");
            return dllProxyObject;
        }

        private static DllProxyObject WrapNewObject(IntPtr nativeObject, TypeSchema type)
        {
            DllProxyObject dllProxyObject = null;
            uint marshalAs;
            IntPtr nativeImpl;
            if (DetermineProxyInterfaceForObject(nativeObject, type, out marshalAs, out nativeImpl))
            {
                switch (marshalAs)
                {
                    case 4294967281:
                    case 4294967286:
                    case 4294967287:
                    case 4294967288:
                    case 4294967289:
                    case 4294967290:
                    case 4294967291:
                        dllProxyObject = new DllProxyObject();
                        break;
                    case 4294967285:
                        dllProxyObject = new DllProxyList();
                        break;
                    case uint.MaxValue:
                        dllProxyObject = new DllProxyObject();
                        break;
                    default:
                        dllProxyObject = new DllProxyObject();
                        break;
                }
                dllProxyObject.Load(nativeObject, nativeImpl, (DllTypeSchemaBase)type);
            }
            return dllProxyObject;
        }

        private static bool DetermineProxyInterfaceForObject(
          IntPtr nativeObject,
          TypeSchema type,
          out uint marshalAs,
          out IntPtr nativeImpl)
        {
            bool flag = false;
            DllTypeSchemaBase dllTypeSchemaBase = (DllTypeSchemaBase)type;
            marshalAs = dllTypeSchemaBase.MarshalAs;
            if (marshalAs == uint.MaxValue)
            {
                flag = true;
                nativeImpl = IntPtr.Zero;
            }
            else if (CheckNativeReturn(NativeApi.SpQueryForMarshalAsInterface(nativeObject, marshalAs, out nativeImpl)) && nativeImpl != IntPtr.Zero)
                flag = true;
            else
                ErrorManager.ReportError("Object didn't implement expected interface '{0}'", marshalAs);
            return flag;
        }

        protected DllProxyObject() => _handle = 0UL;

        ~DllProxyObject() => RegisterAppThreadRelease(new DllProxyObject.AppThreadReleaseEntry(_nativeObject, _handle, OwningLoadResult));

        protected static void RegisterAppThreadRelease(DllProxyObject.AppThreadReleaseEntry entry)
        {
            lock (s_finalizeLock)
            {
                if (s_pendingReleases == null)
                    s_pendingReleases = new Vector<DllProxyObject.AppThreadReleaseEntry>();
                s_pendingReleases.Add(entry);
                if (s_pendingAppThreadRelease)
                    return;
                s_pendingAppThreadRelease = true;
                DeferredCall.Post(DispatchPriority.Idle, s_releaseOnAppThread);
            }
        }

        private void Load(IntPtr nativeObject, IntPtr marshalAs, DllTypeSchemaBase type)
        {
            _type = type;
            _nativeObject = nativeObject;
            OwningLoadResult.RegisterProxyUsage();
            _handle = s_handleTable.RegisterProxy(this);
            NativeApi.SpSetStateCache(_nativeObject, _handle);
            NativeApi.SpAddRefExternalObject(_nativeObject);
            LoadWorker(nativeObject, marshalAs);
        }

        protected virtual void LoadWorker(IntPtr nativeObject, IntPtr nativeMarshalAs)
        {
        }

        public static HRESULT OnChangeNotification(IntPtr nativeObject, uint id)
        {
            HRESULT hresult = new HRESULT(0);
            DllProxyObject existingProxy = GetExistingProxy(nativeObject);
            if (existingProxy != null)
            {
                string id1 = null;
                if (existingProxy._type is DllTypeSchema type)
                    id1 = type.MapChangeID(id);
                if (id1 != null)
                {
                    existingProxy.FireNotification(id1);
                }
                else
                {
                    ErrorManager.ReportError("Invalid UIXID '0x{0:X8}' passed to NotifyChange", id);
                    hresult = new HRESULT(-2147024809);
                }
            }
            return hresult;
        }

        public ulong Handle => _handle;

        public IntPtr NativeObject => _nativeObject;

        public TypeSchema TypeSchema => _type;

        protected DllLoadResult OwningLoadResult => _type.Owner as DllLoadResult;

        private static bool CheckNativeReturn(uint hr) => DllLoadResult.CheckNativeReturn(hr, "IUIXObject");

        bool IDisposableObject.IsDisposed => _nativeObject == IntPtr.Zero;

        void IDisposableObject.DeclareOwner(object owner)
        {
        }

        void IDisposableObject.TransferOwnership(object owner)
        {
        }

        void IDisposableObject.Dispose(object owner)
        {
            OnDispose();
            new DllProxyObject.AppThreadReleaseEntry(_nativeObject, _handle, OwningLoadResult).Release();
            GC.SuppressFinalize(this);
        }

        protected virtual void OnDispose()
        {
        }

        private static void ReleaseFinalizedObjects()
        {
            Vector<DllProxyObject.AppThreadReleaseEntry> pendingReleases;
            lock (s_finalizeLock)
            {
                pendingReleases = s_pendingReleases;
                s_pendingReleases = null;
                s_pendingAppThreadRelease = false;
            }
            if (pendingReleases == null || pendingReleases.Count == 0)
                return;
            foreach (DllProxyObject.AppThreadReleaseEntry threadReleaseEntry in pendingReleases)
                threadReleaseEntry.Release();
            lock (s_finalizeLock)
            {
                if (s_pendingAppThreadRelease)
                    return;
                pendingReleases.Clear();
                s_pendingReleases = pendingReleases;
            }
        }

        private static string DEBUG_FormatIntPtr(IntPtr ptr) => (string)null;

        public string DEBUG_Description() => string.Empty;

        public override string ToString() => _type is DllTypeSchema type ? type.InvokeToString(this) : GetType().Name;

        internal struct AppThreadReleaseEntry
        {
            private bool _releaseHandle;
            public IntPtr _nativeObject;
            public ulong _handle;
            public DllLoadResult _loadResult;

            public AppThreadReleaseEntry(IntPtr nativeObject, ulong handle, DllLoadResult loadResult)
            {
                _nativeObject = nativeObject;
                _handle = handle;
                _loadResult = loadResult;
                _releaseHandle = true;
            }

            public AppThreadReleaseEntry(IntPtr nativeObject)
            {
                _nativeObject = nativeObject;
                _releaseHandle = false;
                _handle = 0UL;
                _loadResult = null;
            }

            public void Release()
            {
                if (_releaseHandle)
                {
                    s_handleTable.ReleaseProxy(_handle);
                    ulong state;
                    NativeApi.SpGetStateCache(_nativeObject, out state);
                    if ((long)_handle == (long)state)
                        NativeApi.SpSetStateCache(_nativeObject, 0UL);
                }
                NativeApi.SpReleaseExternalObject(_nativeObject);
                if (!_releaseHandle)
                    return;
                _loadResult.UnregisterProxyUsage();
            }
        }
    }
}
