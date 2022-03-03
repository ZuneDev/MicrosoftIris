// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocols.Splash.Rendering.Nt.RemoteDs8SoundDevice
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Protocol;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Protocols.Splash.Rendering.Nt
{
    internal class RemoteDs8SoundDevice : RemoteSoundDevice
    {
        protected RemoteDs8SoundDevice()
        {
        }

        public static unsafe RemoteDs8SoundDevice INPROC_Create(
          ProtocolSplashRenderingNt _priv_protocolInstance,
          IRenderHandleOwner _priv_owner,
          HWND hwnd)
        {
            RenderPort port = _priv_protocolInstance.Port;
            RENDERHANDLE deviceClassHandle = _priv_protocolInstance.Ds8SoundDevice_ClassHandle;
            RemoteDs8SoundDevice remoteDs8SoundDevice = new RemoteDs8SoundDevice(port, _priv_owner);
            uint num = (uint)sizeof(RemoteDs8SoundDevice.Msg4_Create);
            // ISSUE: untyped stack allocation
            byte* bytes = stackalloc byte[(int)num];
            RemoteDs8SoundDevice.Msg4_Create* msg4CreatePtr = (Msg4_Create*)bytes;
            msg4CreatePtr->_priv_size = num;
            msg4CreatePtr->_priv_msgid = 4U;
            msg4CreatePtr->hwnd = hwnd;
            msg4CreatePtr->_priv_idObjectSubject = remoteDs8SoundDevice.m_renderHandle;
            port.CreateRemoteObject(deviceClassHandle, remoteDs8SoundDevice.m_renderHandle, (Message*)msg4CreatePtr);
            return remoteDs8SoundDevice;
        }

        public static RemoteDs8SoundDevice CreateFromHandle(
          RenderPort port,
          RENDERHANDLE handle)
        {
            return new RemoteDs8SoundDevice(port, handle, true);
        }

        public static RemoteDs8SoundDevice CreateFromExternalHandle(
          RenderPort port,
          RENDERHANDLE handle,
          IRenderHandleOwner owner)
        {
            port.RegisterKnownHandle(owner, handle);
            return new RemoteDs8SoundDevice(port, handle, false);
        }

        protected RemoteDs8SoundDevice(RenderPort port, RENDERHANDLE handle, bool fFreeOnDispose)
          : base(port, handle, fFreeOnDispose)
        {
        }

        protected RemoteDs8SoundDevice(RenderPort port, IRenderHandleOwner owner)
          : base(port, owner)
        {
        }

        public override bool Equals(object other) => other is RemoteDs8SoundDevice && this.m_renderHandle == ((RemoteObject)other).m_renderHandle;

        public override int GetHashCode() => base.GetHashCode();

        [ComVisible(false)]
        private struct Msg4_Create
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public HWND hwnd;
        }
    }
}
