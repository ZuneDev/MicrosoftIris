// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocols.Splash.Rendering.RemoteSoundDevice
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Protocol;
using Microsoft.Iris.Render.Sound;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Protocols.Splash.Rendering
{
    internal class RemoteSoundDevice : RemoteObject
    {
        private static ushort[] s_priv_ByteOrder_Msg0_CreateSound;
        private static ushort[] s_priv_ByteOrder_Msg1_CreateSoundBuffer;
        private static ushort[] s_priv_ByteOrder_Msg2_EvictExternalResources;
        private static ushort[] s_priv_ByteOrder_Msg3_CreateExternalResources;

        protected RemoteSoundDevice()
        {
        }

        public unsafe void BuildCreateSound(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          RENDERHANDLE idNewSound,
          RemoteSoundBuffer soundBuffer)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            if (soundBuffer != null)
                _priv_portUse.ValidateHandleOrNull(soundBuffer.RenderHandle);
            uint size = (uint)sizeof(RemoteSoundDevice.Msg0_CreateSound);
            RemoteSoundDevice.Msg0_CreateSound* msg0CreateSoundPtr = (RemoteSoundDevice.Msg0_CreateSound*)_priv_portUse.AllocMessageBuffer(size);
            msg0CreateSoundPtr->_priv_size = size;
            msg0CreateSoundPtr->_priv_msgid = 0U;
            msg0CreateSoundPtr->idNewSound = idNewSound;
            msg0CreateSoundPtr->soundBuffer = soundBuffer != null ? soundBuffer.RenderHandle : RENDERHANDLE.NULL;
            msg0CreateSoundPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)msg0CreateSoundPtr, ref s_priv_ByteOrder_Msg0_CreateSound, typeof(RemoteSoundDevice.Msg0_CreateSound), 0, 0);
            _priv_pmsgUse = (Message*)msg0CreateSoundPtr;
        }

        public unsafe void SendCreateSound(RENDERHANDLE idNewSound, RemoteSoundBuffer soundBuffer)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildCreateSound(out _priv_portUse, out _priv_pmsgUse, idNewSound, soundBuffer);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildCreateSoundBuffer(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse,
          RENDERHANDLE idNewBuffer,
          SoundHeader info,
          LocalSoundBufferCallback cb)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteSoundDevice.Msg1_CreateSoundBuffer);
            RemoteSoundDevice.Msg1_CreateSoundBuffer* createSoundBufferPtr = (RemoteSoundDevice.Msg1_CreateSoundBuffer*)_priv_portUse.AllocMessageBuffer(size);
            createSoundBufferPtr->_priv_size = size;
            createSoundBufferPtr->_priv_msgid = 1U;
            createSoundBufferPtr->idNewBuffer = idNewBuffer;
            createSoundBufferPtr->info = info;
            createSoundBufferPtr->_priv_objcb = cb.RenderHandle;
            createSoundBufferPtr->_priv_ctxcb = _priv_portUse.Session.LocalContext;
            createSoundBufferPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)createSoundBufferPtr, ref s_priv_ByteOrder_Msg1_CreateSoundBuffer, typeof(RemoteSoundDevice.Msg1_CreateSoundBuffer), 0, 0);
            _priv_pmsgUse = (Message*)createSoundBufferPtr;
        }

        public unsafe void SendCreateSoundBuffer(
          RENDERHANDLE idNewBuffer,
          SoundHeader info,
          LocalSoundBufferCallback cb)
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildCreateSoundBuffer(out _priv_portUse, out _priv_pmsgUse, idNewBuffer, info, cb);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildEvictExternalResources(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteSoundDevice.Msg2_EvictExternalResources);
            RemoteSoundDevice.Msg2_EvictExternalResources* externalResourcesPtr = (RemoteSoundDevice.Msg2_EvictExternalResources*)_priv_portUse.AllocMessageBuffer(size);
            externalResourcesPtr->_priv_size = size;
            externalResourcesPtr->_priv_msgid = 2U;
            externalResourcesPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)externalResourcesPtr, ref s_priv_ByteOrder_Msg2_EvictExternalResources, typeof(RemoteSoundDevice.Msg2_EvictExternalResources), 0, 0);
            _priv_pmsgUse = (Message*)externalResourcesPtr;
        }

        public unsafe void SendEvictExternalResources()
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildEvictExternalResources(out _priv_portUse, out _priv_pmsgUse);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public unsafe void BuildCreateExternalResources(
          out RenderPort _priv_portUse,
          out Message* _priv_pmsgUse)
        {
            Debug2.Throw(this.IsValid, "Non-static method call requires an instance");
            _priv_portUse = this.m_renderPort;
            _priv_portUse.ValidateHandle(this.m_renderHandle);
            uint size = (uint)sizeof(RemoteSoundDevice.Msg3_CreateExternalResources);
            RemoteSoundDevice.Msg3_CreateExternalResources* externalResourcesPtr = (RemoteSoundDevice.Msg3_CreateExternalResources*)_priv_portUse.AllocMessageBuffer(size);
            externalResourcesPtr->_priv_size = size;
            externalResourcesPtr->_priv_msgid = 3U;
            externalResourcesPtr->_priv_idObjectSubject = this.m_renderHandle;
            if (_priv_portUse.ForeignByteOrder)
                MarshalHelper.SwapByteOrder((byte*)externalResourcesPtr, ref s_priv_ByteOrder_Msg3_CreateExternalResources, typeof(RemoteSoundDevice.Msg3_CreateExternalResources), 0, 0);
            _priv_pmsgUse = (Message*)externalResourcesPtr;
        }

        public unsafe void SendCreateExternalResources()
        {
            RenderPort _priv_portUse;
            Message* _priv_pmsgUse;
            this.BuildCreateExternalResources(out _priv_portUse, out _priv_pmsgUse);
            _priv_portUse.SendRemoteMessage(_priv_pmsgUse);
        }

        public static RemoteSoundDevice CreateFromHandle(
          RenderPort port,
          RENDERHANDLE handle)
        {
            return new RemoteSoundDevice(port, handle, true);
        }

        public static RemoteSoundDevice CreateFromExternalHandle(
          RenderPort port,
          RENDERHANDLE handle,
          IRenderHandleOwner owner)
        {
            port.RegisterKnownHandle(owner, handle);
            return new RemoteSoundDevice(port, handle, false);
        }

        protected RemoteSoundDevice(RenderPort port, RENDERHANDLE handle, bool fFreeOnDispose)
          : base(port, handle, fFreeOnDispose)
        {
        }

        protected RemoteSoundDevice(RenderPort port, IRenderHandleOwner owner)
          : base(port, owner)
        {
        }

        public override bool Equals(object other) => other is RemoteSoundDevice && this.m_renderHandle == ((RemoteObject)other).m_renderHandle;

        public override int GetHashCode() => base.GetHashCode();

        [ComVisible(false)]
        private struct Msg0_CreateSound
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public RENDERHANDLE idNewSound;
            public RENDERHANDLE soundBuffer;
        }

        [ComVisible(false)]
        private struct Msg1_CreateSoundBuffer
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
            public RENDERHANDLE idNewBuffer;
            public SoundHeader info;
            public RENDERHANDLE _priv_objcb;
            public ContextID _priv_ctxcb;
        }

        [ComVisible(false)]
        private struct Msg2_EvictExternalResources
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
        }

        [ComVisible(false)]
        private struct Msg3_CreateExternalResources
        {
            public uint _priv_size;
            public uint _priv_msgid;
            public RENDERHANDLE _priv_idObjectSubject;
        }
    }
}
