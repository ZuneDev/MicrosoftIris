// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocols.Splash.Rendering.Nt.RemoteWinSoundDevice
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Protocol;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Protocols.Splash.Rendering.Nt
{
    internal class RemoteWinSoundDevice : RemoteSoundDevice
    {
        private static ushort[] s_priv_ByteOrder_Msg4_Create;

        protected RemoteWinSoundDevice()
        {
        }

        public static unsafe RemoteWinSoundDevice Create(
          ProtocolSplashRenderingNt _priv_protocolInstance,
          IRenderHandleOwner _priv_owner)
        {
            RenderPort port = _priv_protocolInstance.Port;
            RENDERHANDLE deviceClassHandle = _priv_protocolInstance.WinSoundDevice_ClassHandle;
            RemoteWinSoundDevice remoteWinSoundDevice = new RemoteWinSoundDevice(port, _priv_owner);
            uint num = (uint)sizeof(RemoteWinSoundDevice.Msg4_Create);
            // ISSUE: untyped stack allocation
            byte* pMem = stackalloc byte[(int)num];
            RemoteWinSoundDevice.Msg4_Create* msg4CreatePtr = (RemoteWinSoundDevice.Msg4_Create*)pMem;
            msg4CreatePtr->_priv_size = num;
            msg4CreatePtr->_priv_msgid = 4U;
            msg4CreatePtr->_priv_idObjectSubject = remoteWinSoundDevice.m_renderHandle;
            if (port.ForeignByteOrder)
                MarshalHelper.SwapByteOrder(pMem, ref s_priv_ByteOrder_Msg4_Create, typeof(RemoteWinSoundDevice.Msg4_Create), 0, 0);
            port.CreateRemoteObject(deviceClassHandle, remoteWinSoundDevice.m_renderHandle, (Message*)msg4CreatePtr);
            return remoteWinSoundDevice;
        }

        public static RemoteWinSoundDevice CreateFromHandle(
          RenderPort port,
          RENDERHANDLE handle)
        {
            return new RemoteWinSoundDevice(port, handle, true);
        }

        public static RemoteWinSoundDevice CreateFromExternalHandle(
          RenderPort port,
          RENDERHANDLE handle,
          IRenderHandleOwner owner)
        {
            port.RegisterKnownHandle(owner, handle);
            return new RemoteWinSoundDevice(port, handle, false);
        }

        protected RemoteWinSoundDevice(RenderPort port, RENDERHANDLE handle, bool fFreeOnDispose)
          : base(port, handle, fFreeOnDispose)
        {
        }

        protected RemoteWinSoundDevice(RenderPort port, IRenderHandleOwner owner)
          : base(port, owner)
        {
        }

        public override bool Equals(object other) => other is RemoteWinSoundDevice && this.m_renderHandle == ((RemoteObject)other).m_renderHandle;

        public override int GetHashCode() => base.GetHashCode();

        [ComVisible(false)]
        private struct Msg4_Create
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
        }
    }
}
