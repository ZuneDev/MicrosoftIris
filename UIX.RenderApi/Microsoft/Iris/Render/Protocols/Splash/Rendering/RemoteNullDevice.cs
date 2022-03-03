// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocols.Splash.Rendering.RemoteNullDevice
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Protocol;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Protocols.Splash.Rendering
{
    internal class RemoteNullDevice : RemoteDevice
    {
        private static ushort[] s_priv_ByteOrder_Msg8_Create;

        protected RemoteNullDevice()
        {
        }

        public static unsafe RemoteNullDevice Create(
          ProtocolSplashRendering _priv_protocolInstance,
          IRenderHandleOwner _priv_owner,
          LocalDeviceCallback cb)
        {
            RenderPort port = _priv_protocolInstance.Port;
            RENDERHANDLE deviceClassHandle = _priv_protocolInstance.NullDevice_ClassHandle;
            RemoteNullDevice remoteNullDevice = new RemoteNullDevice(port, _priv_owner);
            uint num = (uint)sizeof(RemoteNullDevice.Msg8_Create);
            // ISSUE: untyped stack allocation
            byte* pMem = stackalloc byte[(int)num];
            RemoteNullDevice.Msg8_Create* msg8CreatePtr = (RemoteNullDevice.Msg8_Create*)pMem;
            msg8CreatePtr->_priv_size = num;
            msg8CreatePtr->_priv_msgid = 8U;
            msg8CreatePtr->_priv_objcb = cb.RenderHandle;
            msg8CreatePtr->_priv_ctxcb = port.Session.LocalContext;
            msg8CreatePtr->_priv_idObjectSubject = remoteNullDevice.m_renderHandle;
            if (port.ForeignByteOrder)
                MarshalHelper.SwapByteOrder(pMem, ref s_priv_ByteOrder_Msg8_Create, typeof(RemoteNullDevice.Msg8_Create), 0, 0);
            port.CreateRemoteObject(deviceClassHandle, remoteNullDevice.m_renderHandle, (Message*)msg8CreatePtr);
            return remoteNullDevice;
        }

        public static RemoteNullDevice CreateFromHandle(
          RenderPort port,
          RENDERHANDLE handle)
        {
            return new RemoteNullDevice(port, handle, true);
        }

        public static RemoteNullDevice CreateFromExternalHandle(
          RenderPort port,
          RENDERHANDLE handle,
          IRenderHandleOwner owner)
        {
            port.RegisterKnownHandle(owner, handle);
            return new RemoteNullDevice(port, handle, false);
        }

        protected RemoteNullDevice(RenderPort port, RENDERHANDLE handle, bool fFreeOnDispose)
          : base(port, handle, fFreeOnDispose)
        {
        }

        protected RemoteNullDevice(RenderPort port, IRenderHandleOwner owner)
          : base(port, owner)
        {
        }

        public override bool Equals(object other) => other is RemoteNullDevice && this.m_renderHandle == ((RemoteObject)other).m_renderHandle;

        public override int GetHashCode() => base.GetHashCode();

        [ComVisible(false)]
        private struct Msg8_Create
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public RENDERHANDLE _priv_objcb;
            public ContextID _priv_ctxcb;
        }
    }
}
