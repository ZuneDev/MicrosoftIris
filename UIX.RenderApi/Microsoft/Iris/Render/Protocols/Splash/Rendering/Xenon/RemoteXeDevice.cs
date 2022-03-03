// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocols.Splash.Rendering.Xenon.RemoteXeDevice
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Protocol;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Protocols.Splash.Rendering.Xenon
{
    internal class RemoteXeDevice : RemoteDx9Device
    {
        private static ushort[] s_priv_ByteOrder_Msg11_Create;

        protected RemoteXeDevice()
        {
        }

        public static unsafe RemoteXeDevice Create(
          ProtocolSplashRenderingXenon _priv_protocolInstance,
          IRenderHandleOwner _priv_owner,
          LocalDeviceCallback cb,
          Size sizeScreenPxl)
        {
            RenderPort port = _priv_protocolInstance.Port;
            RENDERHANDLE deviceClassHandle = _priv_protocolInstance.XeDevice_ClassHandle;
            RemoteXeDevice remoteXeDevice = new RemoteXeDevice(port, _priv_owner);
            uint num = (uint)sizeof(RemoteXeDevice.Msg11_Create);
            // ISSUE: untyped stack allocation
            byte* pMem = stackalloc byte[(int)num];
            RemoteXeDevice.Msg11_Create* msg11CreatePtr = (RemoteXeDevice.Msg11_Create*)pMem;
            msg11CreatePtr->_priv_size = num;
            msg11CreatePtr->_priv_msgid = 11U;
            msg11CreatePtr->_priv_objcb = cb.RenderHandle;
            msg11CreatePtr->_priv_ctxcb = port.Session.LocalContext;
            msg11CreatePtr->sizeScreenPxl = sizeScreenPxl;
            msg11CreatePtr->_priv_idObjectSubject = remoteXeDevice.m_renderHandle;
            if (port.ForeignByteOrder)
                MarshalHelper.SwapByteOrder(pMem, ref s_priv_ByteOrder_Msg11_Create, typeof(RemoteXeDevice.Msg11_Create), 0, 0);
            port.CreateRemoteObject(deviceClassHandle, remoteXeDevice.m_renderHandle, (Message*)msg11CreatePtr);
            return remoteXeDevice;
        }

        public static RemoteXeDevice CreateFromHandle(
          RenderPort port,
          RENDERHANDLE handle)
        {
            return new RemoteXeDevice(port, handle, true);
        }

        public static RemoteXeDevice CreateFromExternalHandle(
          RenderPort port,
          RENDERHANDLE handle,
          IRenderHandleOwner owner)
        {
            port.RegisterKnownHandle(owner, handle);
            return new RemoteXeDevice(port, handle, false);
        }

        protected RemoteXeDevice(RenderPort port, RENDERHANDLE handle, bool fFreeOnDispose)
          : base(port, handle, fFreeOnDispose)
        {
        }

        protected RemoteXeDevice(RenderPort port, IRenderHandleOwner owner)
          : base(port, owner)
        {
        }

        public override bool Equals(object other) => other is RemoteXeDevice && this.m_renderHandle == ((RemoteObject)other).m_renderHandle;

        public override int GetHashCode() => base.GetHashCode();

        [ComVisible(false)]
        private struct Msg11_Create
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public RENDERHANDLE _priv_objcb;
            public ContextID _priv_ctxcb;
            public Size sizeScreenPxl;
        }
    }
}
