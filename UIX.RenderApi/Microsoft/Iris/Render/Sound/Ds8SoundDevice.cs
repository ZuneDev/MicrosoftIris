// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Sound.Ds8SoundDevice
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Internal;
using Microsoft.Iris.Render.Protocol;
using Microsoft.Iris.Render.Protocols.Splash.Rendering;
using Microsoft.Iris.Render.Protocols.Splash.Rendering.Nt;
using System;

namespace Microsoft.Iris.Render.Sound
{
    internal class Ds8SoundDevice : SoundDevice, IRenderHandleOwner
    {
        private IRenderWindow m_ownerWindow;
        private RemoteDs8SoundDevice m_remoteDevice;

        internal Ds8SoundDevice(RenderSession ownerSession, IRenderWindow ownerWindow)
          : base(ownerSession)
        {
            Debug2.Validate(ownerWindow != null, typeof(ArgumentNullException), nameof(ownerWindow));
            this.m_ownerWindow = ownerWindow;
            this.PostCreate();
        }

        protected override void Dispose(bool inDispose)
        {
            try
            {
                if (this.m_remoteDevice == null)
                    return;
                this.m_remoteDevice.Dispose();
                this.m_remoteDevice = null;
            }
            finally
            {
                base.Dispose(inDispose);
            }
        }

        RENDERHANDLE IRenderHandleOwner.RenderHandle => this.m_remoteDevice.RenderHandle;

        void IRenderHandleOwner.OnDisconnect() => this.m_remoteDevice = null;

        protected override SoundDeviceType DeviceType => SoundDeviceType.DirectSound8;

        protected override void CreateRemoteSoundDeviceImpl() => this.m_remoteDevice = this.Session.NtRenderingProtocol.INPROC_BuildRemoteDs8SoundDevice(this, this.m_ownerWindow.WindowHandle);

        protected override void CreateExternalResourcesImpl() => this.m_remoteDevice.SendCreateExternalResources();

        protected override void EvictExternalResourcesImpl() => this.m_remoteDevice.SendEvictExternalResources();

        protected override void CreateRemoteSoundBufferImpl(
          RENDERHANDLE handle,
          SoundHeader info,
          LocalSoundBufferCallback callback)
        {
            this.m_remoteDevice.SendCreateSoundBuffer(handle, info, callback);
        }

        protected override void CreateRemoteSoundImpl(RENDERHANDLE handle, SoundBuffer soundBuffer) => this.m_remoteDevice.SendCreateSound(handle, soundBuffer.RemoteStub);
    }
}
