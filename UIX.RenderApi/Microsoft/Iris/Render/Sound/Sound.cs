// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Sound.Sound
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Animation;
using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Protocol;
using Microsoft.Iris.Render.Protocols.Splash.Rendering;
using System;

namespace Microsoft.Iris.Render.Sound
{
    internal class Sound :
      SharedRenderObject,
      ISound,
      ISharedRenderObject,
      IRenderHandleOwner,
      IActivatableObject,
      IActivatable
    {
        public static readonly string PlayMethod = "Play";
        public static readonly string StopMethod = "Stop";
        private RemoteSound m_remoteSound;
        private SoundBuffer m_bufferOwner;

        internal Sound(SoundBuffer buffer)
        {
            this.m_remoteSound = this.CreateRemoteObject(buffer);
            this.m_bufferOwner = buffer;
        }

        protected override void Dispose(bool inDispose)
        {
            try
            {
                if (this.m_remoteSound != null)
                {
                    this.m_remoteSound.Dispose();
                    this.m_remoteSound = null;
                }
                this.m_bufferOwner = null;
            }
            finally
            {
                base.Dispose(inDispose);
            }
        }

        void ISound.Play()
        {
            if (!this.m_bufferOwner.IsContentLoaded)
                this.m_bufferOwner.LoadContent();
            if (!this.m_bufferOwner.IsContentLoaded || !this.m_remoteSound.IsValid)
                return;
            this.m_remoteSound.SendPlay();
        }

        void ISound.Stop()
        {
            if (!this.m_bufferOwner.IsContentLoaded || !this.m_remoteSound.IsValid)
                return;
            this.m_remoteSound.SendStop();
        }

        private RemoteSound CreateRemoteObject(SoundBuffer buffer)
        {
            RemoteSound remoteSound = null;
            RENDERHANDLE handle = buffer.Device.Session.AllocateRenderHandle(this);
            try
            {
                buffer.Device.CreateRemoteSound(handle, buffer);
                remoteSound = buffer.Device.Session.BuildSoundFromHandle(handle);
                return remoteSound;
            }
            finally
            {
                if (!remoteSound.IsValid)
                    buffer.Device.Session.FreeRenderHandle(handle);
            }
        }

        RENDERHANDLE IRenderHandleOwner.RenderHandle => this.m_remoteSound.RenderHandle;

        void IRenderHandleOwner.OnDisconnect() => this.m_remoteSound = null;

        RENDERHANDLE IActivatableObject.GetObjectId() => this.m_remoteSound.RenderHandle;

        uint IActivatableObject.GetMethodId(string methodName)
        {
            if (methodName == PlayMethod)
                return 1;
            if (methodName == StopMethod)
                return 2;
            Debug2.Validate(false, typeof(ArgumentException), "Unsupported method: {0}", methodName);
            return 0;
        }

        protected enum ActivatableMethods : uint
        {
            Play = 1,
            Stop = 2,
        }
    }
}
