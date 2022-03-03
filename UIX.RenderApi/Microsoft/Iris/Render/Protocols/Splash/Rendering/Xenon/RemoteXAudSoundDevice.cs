// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocols.Splash.Rendering.Xenon.RemoteXAudSoundDevice
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Protocol;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Protocols.Splash.Rendering.Xenon
{
    internal class RemoteXAudSoundDevice : RemoteSoundDevice
    {
        private static ushort[] s_priv_ByteOrder_Msg6_Create;
        private static ushort[] s_priv_ByteOrder_Msg4_SetMute;
        private static ushort[] s_priv_ByteOrder_Msg5_SetVolume;

        protected RemoteXAudSoundDevice()
        {
        }

        public static unsafe RemoteXAudSoundDevice Create(
          ProtocolSplashRenderingXenon _priv_protocolInstance,
          IRenderHandleOwner _priv_owner)
        {
            RenderPort port = _priv_protocolInstance.Port;
            RENDERHANDLE deviceClassHandle = _priv_protocolInstance.XAudSoundDevice_ClassHandle;
            RemoteXAudSoundDevice remoteXaudSoundDevice = new RemoteXAudSoundDevice(port, _priv_owner);
            uint num = (uint)sizeof(RemoteXAudSoundDevice.Msg6_Create);
            // ISSUE: untyped stack allocation
            byte* pMem = stackalloc byte[(int)num];
            RemoteXAudSoundDevice.Msg6_Create* msg6CreatePtr = (RemoteXAudSoundDevice.Msg6_Create*)pMem;
            msg6CreatePtr->_priv_size = num;
            msg6CreatePtr->_priv_msgid = 6U;
            msg6CreatePtr->_priv_idObjectSubject = remoteXaudSoundDevice.m_renderHandle;
            if (port.ForeignByteOrder)
                MarshalHelper.SwapByteOrder(pMem, ref s_priv_ByteOrder_Msg6_Create, typeof(RemoteXAudSoundDevice.Msg6_Create), 0, 0);
            port.CreateRemoteObject(deviceClassHandle, remoteXaudSoundDevice.m_renderHandle, (Message*)msg6CreatePtr);
            return remoteXaudSoundDevice;
        }

        public unsafe void BuildSetMute(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          bool fMuted)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteXAudSoundDevice.Msg4_SetMute);
            RemoteXAudSoundDevice.Msg4_SetMute* msg4SetMutePtr = (RemoteXAudSoundDevice.Msg4_SetMute*)_priv_portUse.AllocMessageBuffer(size);
            msg4SetMutePtr->_priv_size = size;
            msg4SetMutePtr->_priv_msgid = 4U;
            msg4SetMutePtr->fMuted = fMuted ? uint.MaxValue : 0U;
            msg4SetMutePtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg4SetMutePtr, ref s_priv_ByteOrder_Msg4_SetMute, typeof(RemoteXAudSoundDevice.Msg4_SetMute), 0, 0);
            _priv_pmsgUse = (Message*)msg4SetMutePtr;
        }

        public unsafe void SendSetMute(bool fMuted)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetMute(out _priv_portUse, out _priv_pmsgUse, fMuted);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildSetVolume(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          float flVolume)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteXAudSoundDevice.Msg5_SetVolume);
            RemoteXAudSoundDevice.Msg5_SetVolume* msg5SetVolumePtr = (RemoteXAudSoundDevice.Msg5_SetVolume*)_priv_portUse.AllocMessageBuffer(size);
            msg5SetVolumePtr->_priv_size = size;
            msg5SetVolumePtr->_priv_msgid = 5U;
            msg5SetVolumePtr->flVolume = flVolume;
            msg5SetVolumePtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg5SetVolumePtr, ref s_priv_ByteOrder_Msg5_SetVolume, typeof(RemoteXAudSoundDevice.Msg5_SetVolume), 0, 0);
            _priv_pmsgUse = (Message*)msg5SetVolumePtr;
        }

        public unsafe void SendSetVolume(float flVolume)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildSetVolume(out _priv_portUse, out _priv_pmsgUse, flVolume);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public static RemoteXAudSoundDevice CreateFromHandle(
          RenderPort port,
          RENDERHANDLE handle)
        {
            return new RemoteXAudSoundDevice(port, handle, true);
        }

        public static RemoteXAudSoundDevice CreateFromExternalHandle(
          RenderPort port,
          RENDERHANDLE handle,
          IRenderHandleOwner owner)
        {
            port.RegisterKnownHandle(owner, handle);
            return new RemoteXAudSoundDevice(port, handle, false);
        }

        protected RemoteXAudSoundDevice(RenderPort port, RENDERHANDLE handle, bool fFreeOnDispose)
          : base(port, handle, fFreeOnDispose)
        {
        }

        protected RemoteXAudSoundDevice(RenderPort port, IRenderHandleOwner owner)
          : base(port, owner)
        {
        }

        public override bool Equals(object other) => other is RemoteXAudSoundDevice && this.m_renderHandle == ((RemoteObject)other).m_renderHandle;

        public override int GetHashCode() => base.GetHashCode();

        [ComVisible(false)]
        private struct Msg6_Create
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
        }

        [ComVisible(false)]
        private struct Msg4_SetMute
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public uint fMuted;
        }

        [ComVisible(false)]
        private struct Msg5_SetVolume
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public float flVolume;
        }
    }
}
