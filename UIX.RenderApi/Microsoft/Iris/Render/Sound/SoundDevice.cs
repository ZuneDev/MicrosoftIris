// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Sound.SoundDevice
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Internal;
using Microsoft.Iris.Render.Protocol;
using Microsoft.Iris.Render.Protocols.Splash.Rendering;
using System;

namespace Microsoft.Iris.Render.Sound
{
    internal abstract class SoundDevice : RenderObject, ISoundDevice
    {
        private RenderSession m_ownerSession;
        private bool m_isValid;

        internal SoundDevice(RenderSession ownerSession)
        {
            this.m_ownerSession = ownerSession;
            this.m_isValid = false;
        }

        protected void PostCreate()
        {
            this.CreateRemoteSoundDevice();
            this.m_isValid = true;
        }

        protected override void Dispose(bool inDispose)
        {
            try
            {
                this.m_ownerSession = null;
            }
            finally
            {
                base.Dispose(inDispose);
            }
        }

        SoundDeviceType ISoundDevice.DeviceType => this.DeviceType;

        bool ISoundDevice.Mute
        {
            get
            {
                Debug2.Validate(this.DeviceType == SoundDeviceType.XAudio, typeof(InvalidOperationException), "Only supported on XAudio");
                return false;
            }
            set => Debug2.Validate(this.DeviceType == SoundDeviceType.XAudio, typeof(InvalidOperationException), "Only supported on XAudio");
        }

        float ISoundDevice.Volume
        {
            get
            {
                Debug2.Validate(this.DeviceType == SoundDeviceType.XAudio, typeof(InvalidOperationException), "Only supported on XAudio");
                return 0.0f;
            }
            set => Debug2.Validate(this.DeviceType == SoundDeviceType.XAudio, typeof(InvalidOperationException), "Only supported on XAudio");
        }

        internal RenderSession Session => this.m_ownerSession;

        internal bool IsValid => this.m_isValid;

        ISoundBuffer ISoundDevice.CreateSoundBuffer(
          object objUser,
          ISoundData soundData)
        {
            Debug2.Validate(objUser != null, typeof(ArgumentNullException), nameof(objUser));
            Debug2.Validate(soundData != null, typeof(ArgumentNullException), nameof(soundData));
            SoundBuffer soundBuffer = new SoundBuffer(this, soundData);
            soundBuffer.RegisterUsage(objUser);
            return soundBuffer;
        }

        internal void Rebuild()
        {
            if (this.m_isValid)
                this.EvictExternalResources();
            if (this.m_isValid)
                return;
            this.CreateExternalResources();
        }

        private void EvictExternalResources()
        {
            this.m_isValid = false;
            this.EvictExternalResourcesImpl();
        }

        private void CreateExternalResources()
        {
            this.CreateExternalResourcesImpl();
            this.m_isValid = true;
        }

        private void CreateRemoteSoundDevice() => this.CreateRemoteSoundDeviceImpl();

        internal void CreateRemoteSoundBuffer(
          RENDERHANDLE handle,
          SoundHeader info,
          LocalSoundBufferCallback callback)
        {
            this.CreateRemoteSoundBufferImpl(handle, info, callback);
        }

        internal void CreateRemoteSound(RENDERHANDLE handle, SoundBuffer soundBuffer) => this.CreateRemoteSoundImpl(handle, soundBuffer);

        protected abstract SoundDeviceType DeviceType { get; }

        protected abstract void CreateRemoteSoundDeviceImpl();

        protected abstract void CreateExternalResourcesImpl();

        protected abstract void EvictExternalResourcesImpl();

        protected abstract void CreateRemoteSoundBufferImpl(
          RENDERHANDLE handle,
          SoundHeader info,
          LocalSoundBufferCallback callback);

        protected abstract void CreateRemoteSoundImpl(RENDERHANDLE handle, SoundBuffer soundBuffer);
    }
}
