// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Sound.SoundBuffer
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Protocol;
using Microsoft.Iris.Render.Protocols.Splash.Rendering;
using System;

namespace Microsoft.Iris.Render.Sound
{
    internal class SoundBuffer :
      SharedRenderObject,
      ISoundBuffer,
      ISharedRenderObject,
      ISoundBufferCallback,
      IRenderHandleOwner
    {
        private RemoteSoundBuffer m_remoteBuffer;
        private SoundDevice m_device;
        private ISoundData m_soundData;
        private bool m_contentLoaded;
        private DataBufferTracker m_tracker;
        private SoundBuffer.SoundBufferLoadInfo m_loadInfo;

        internal SoundBuffer(SoundDevice device, ISoundData soundData)
        {
            Debug2.Validate(soundData != null, typeof(ArgumentNullException), nameof(soundData));
            Debug2.Validate(soundData.Format == SoundDataFormat.PCM, typeof(ArgumentException), "Only PCM sound data is currently supported");
            Debug2.Validate(soundData.ChannelCount == 1U || soundData.ChannelCount == 2U, typeof(ArgumentException), "Only mono/stereo sound data is currently supported");
            Debug2.Validate(soundData.SampleSize == 8U || soundData.SampleSize == 16U, typeof(ArgumentException), "Only 8bps/16bps sound data is currently supported");
            Debug2.Validate(soundData.SampleCount > 0U, typeof(ArgumentException), "Insufficient sample data");
            Debug2.Validate(soundData.SampleCount % soundData.ChannelCount == 0U, typeof(ArgumentException), "Insufficient sample data");
            this.m_device = device;
            this.m_soundData = soundData;
            this.m_tracker = MessagingSession.Current.CreateDataBufferTracker(this);
            this.m_remoteBuffer = this.CreateRemoteObject();
            this.LoadContent();
        }

        protected override void Dispose(bool inDispose)
        {
            try
            {
                if (inDispose)
                {
                    if (this.m_tracker != null)
                        MessagingSession.Current.ReturnDataBufferTracker(this.m_tracker);
                    if (this.m_remoteBuffer != null)
                        this.m_remoteBuffer.Dispose();
                }
                this.m_tracker = null;
                this.m_soundData = null;
                this.m_device = null;
                this.m_remoteBuffer = null;
            }
            finally
            {
                base.Dispose(inDispose);
            }
        }

        internal SoundDevice Device => this.m_device;

        internal RemoteSoundBuffer RemoteStub => this.m_remoteBuffer;

        internal bool IsContentLoaded => this.m_contentLoaded;

        RENDERHANDLE IRenderHandleOwner.RenderHandle => this.m_remoteBuffer.RenderHandle;

        void IRenderHandleOwner.OnDisconnect() => this.m_remoteBuffer = null;

        ISound ISoundBuffer.CreateSound(object objUser)
        {
            Debug2.Validate(objUser != null, typeof(ArgumentNullException), nameof(objUser));
            Microsoft.Iris.Render.Sound.Sound sound = new Microsoft.Iris.Render.Sound.Sound(this);
            sound.RegisterUsage(objUser);
            return sound;
        }

        private RemoteSoundBuffer CreateRemoteObject()
        {
            RemoteSoundBuffer remoteSoundBuffer = null;
            SoundHeader info;
            info.wFormatTag = 1;
            info.nChannels = (ushort)this.m_soundData.ChannelCount;
            info.nSamplesPerSec = this.m_soundData.SampleRate;
            info.nAvgBytesPerSec = this.m_soundData.SampleRate * (this.m_soundData.SampleSize / 8U) * this.m_soundData.ChannelCount;
            info.nBlockAlign = (ushort)(this.m_soundData.ChannelCount * (this.m_soundData.SampleSize / 8U));
            info.wBitsPerSample = (ushort)this.m_soundData.SampleSize;
            info.cbExtraData = 0;
            info.cbDataSize = this.m_soundData.SampleCount * (this.m_soundData.SampleSize / 8U);
            RENDERHANDLE handle = this.m_device.Session.AllocateRenderHandle(this);
            try
            {
                this.m_device.CreateRemoteSoundBuffer(handle, info, this.m_device.Session.RenderingProtocol.LocalSoundBufferCallbackHandle);
                remoteSoundBuffer = this.m_device.Session.BuildSoundBufferFromHandle(handle);
                return remoteSoundBuffer;
            }
            finally
            {
                if (!remoteSoundBuffer.IsValid)
                    this.m_device.Session.FreeRenderHandle(handle);
            }
        }

        internal void LoadContent()
        {
            if (this.m_contentLoaded)
                return;
            IntPtr pData = this.m_soundData.AcquireContent();
            DataBuffer dataBuffer = null;
            try
            {
                uint cbSize = this.m_soundData.SampleCount * (this.m_soundData.SampleSize / 8U);
                dataBuffer = this.m_device.Session.BuildDataBuffer(pData, cbSize);
                if (this.m_loadInfo == null)
                {
                    this.m_loadInfo = new SoundBuffer.SoundBufferLoadInfo();
                    this.m_loadInfo.loadCount = 0;
                    this.m_loadInfo.soundData = this.m_soundData;
                }
                ++this.m_loadInfo.loadCount;
                this.m_tracker.Track(dataBuffer, new DataBufferTracker.CleanupEventHandler(OnDataBufferConsumed), m_loadInfo);
                dataBuffer.Commit();
                if (this.m_remoteBuffer.IsValid)
                    this.m_remoteBuffer.SendLoadSoundData(dataBuffer.RemoteStub);
                this.m_contentLoaded = true;
            }
            finally
            {
                if (!this.m_contentLoaded)
                    this.m_tracker.Release(dataBuffer, DataBufferTracker.Users.Valid);
            }
        }

        private static void OnDataBufferConsumed(object sender, DataBufferTracker.CleanupEventArgs args)
        {
            SoundBuffer.SoundBufferLoadInfo contextData = (SoundBuffer.SoundBufferLoadInfo)args.ContextData;
            --contextData.loadCount;
            if (contextData.loadCount != 0)
                return;
            contextData.soundData.ReleaseContent();
            contextData.soundData = null;
        }

        void ISoundBufferCallback.OnSoundBufferLost(RENDERHANDLE target) => this.m_contentLoaded = false;

        void ISoundBufferCallback.OnSoundBufferReady(RENDERHANDLE target)
        {
            if (this.m_contentLoaded)
                return;
            this.LoadContent();
        }

        private class SoundBufferLoadInfo
        {
            public int loadCount;
            public ISoundData soundData;
        }
    }
}
