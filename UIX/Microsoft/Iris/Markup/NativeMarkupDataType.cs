// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.NativeMarkupDataType
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.CodeModel.Cpp;
using Microsoft.Iris.Library;
using Microsoft.Iris.OS;
using Microsoft.Iris.Session;
using System;

namespace Microsoft.Iris.Markup
{
    internal class NativeMarkupDataType : MarkupDataType
    {
        private IntPtr _externalObject;
        private ulong _handleToMe;
        private ulong _typeHandle;
        private static object s_finalizeLock;
        private static bool s_pendingAppThreadRelease;
        private static Vector<NativeMarkupDataType.AppThreadReleaseEntry> s_pendingReleases;
        private static SimpleCallback s_releaseOnAppThread;
        private static MarkupDataTypeHandleTable s_handleTable;

        public static void InitializeStatics()
        {
            s_handleTable = new MarkupDataTypeHandleTable();
            s_finalizeLock = new object();
            s_pendingAppThreadRelease = false;
            s_releaseOnAppThread = new SimpleCallback(ReleaseFinalizedObjects);
        }

        private NativeMarkupDataType(MarkupDataTypeSchema type, IntPtr externalObject)
          : base(type)
        {
            _externalObject = externalObject;
            _handleToMe = s_handleTable.RegisterProxy(this);
            _typeHandle = type.UniqueId;
            NativeApi.SpAddRefExternalObject(_externalObject);
            int num = (int)NativeApi.SpDataBaseObjectSetInternalHandle(_externalObject, _handleToMe);
            TypeSchema.Owner.RegisterProxyUsage();
        }

        protected override void OnDispose()
        {
            ReleaseNativeObject(_externalObject, _handleToMe, _typeHandle);
            GC.SuppressFinalize(this);
            base.OnDispose();
        }

        ~NativeMarkupDataType()
        {
            lock (s_finalizeLock)
            {
                if (s_pendingReleases == null)
                    s_pendingReleases = new Vector<NativeMarkupDataType.AppThreadReleaseEntry>();
                s_pendingReleases.Add(new NativeMarkupDataType.AppThreadReleaseEntry(_externalObject, _handleToMe, _typeHandle));
                if (s_pendingAppThreadRelease)
                    return;
                s_pendingAppThreadRelease = true;
                DeferredCall.Post(DispatchPriority.Idle, s_releaseOnAppThread);
            }
        }

        protected override bool ExternalObjectGetProperty(string propertyName, out object value)
        {
            UIXVariant propertyValue;
            int property = (int)NativeApi.SpDataBaseObjectGetProperty(_externalObject, propertyName, out propertyValue);
            TypeSchema.FindPropertyDeep(propertyName);
            value = UIXVariant.GetValue(propertyValue, TypeSchema.Owner);
            return true;
        }

        protected override unsafe bool ExternalObjectSetProperty(string propertyName, object value)
        {
            // ISSUE: untyped stack allocation
            UIXVariant* uixVariantPtr = stackalloc UIXVariant[sizeof(UIXVariant)];
            UIXVariant.MarshalObject(value, uixVariantPtr);
            int num = (int)NativeApi.SpDataBaseObjectSetProperty(_externalObject, propertyName, uixVariantPtr);
            return true;
        }

        protected override IDataProviderBaseObject ExternalAssemblyObject => (IDataProviderBaseObject)null;

        public override IntPtr ExternalNativeObject => _externalObject;

        public static NativeMarkupDataType LookupByHandle(ulong handle)
        {
            MarkupDataType markupDataType;
            s_handleTable.LookupByHandle(handle, out markupDataType);
            return (NativeMarkupDataType)markupDataType;
        }

        public static NativeMarkupDataType Create(
          ulong typeHandle,
          IntPtr nativeObject)
        {
            return new NativeMarkupDataType((MarkupDataTypeSchema)TypeSchema.LookupById(typeHandle), nativeObject);
        }

        public static void ReleaseOutstandingProxies()
        {
            s_pendingAppThreadRelease = true;
            GC.Collect();
            GC.WaitForPendingFinalizers();
            foreach (IDisposableObject disposableObject in s_handleTable)
                disposableObject.Dispose(disposableObject);
            ReleaseFinalizedObjects();
        }

        private static void ReleaseFinalizedObjects()
        {
            Vector<NativeMarkupDataType.AppThreadReleaseEntry> pendingReleases;
            lock (s_finalizeLock)
            {
                pendingReleases = s_pendingReleases;
                s_pendingReleases = null;
                s_pendingAppThreadRelease = false;
            }
            if (pendingReleases == null || pendingReleases.Count == 0)
                return;
            foreach (NativeMarkupDataType.AppThreadReleaseEntry threadReleaseEntry in pendingReleases)
                ReleaseNativeObject(threadReleaseEntry._nativeObject, threadReleaseEntry._handle, threadReleaseEntry._typeHandle);
            lock (s_finalizeLock)
            {
                if (s_pendingAppThreadRelease)
                    return;
                pendingReleases.Clear();
                s_pendingReleases = pendingReleases;
            }
        }

        private static void ReleaseNativeObject(
          IntPtr nativeObject,
          ulong proxyHandle,
          ulong typeHandle)
        {
            s_handleTable.ReleaseProxy(proxyHandle);
            ulong frameworkQuery;
            int internalHandle = (int)NativeApi.SpDataBaseObjectGetInternalHandle(nativeObject, out frameworkQuery);
            if ((long)proxyHandle == (long)frameworkQuery)
            {
                int num = (int)NativeApi.SpDataBaseObjectSetInternalHandle(nativeObject, 0UL);
            }
            NativeApi.SpReleaseExternalObject(nativeObject);
            TypeSchema.LookupById(typeHandle).Owner.UnregisterProxyUsage();
        }

        internal struct AppThreadReleaseEntry
        {
            public IntPtr _nativeObject;
            public ulong _handle;
            public ulong _typeHandle;

            public AppThreadReleaseEntry(IntPtr nativeObject, ulong handle, ulong typeHandle)
            {
                _nativeObject = nativeObject;
                _handle = handle;
                _typeHandle = typeHandle;
            }
        }
    }
}
