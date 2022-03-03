// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Graphics.NullGraphicsDevice
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Internal;
using Microsoft.Iris.Render.Protocol;
using Microsoft.Iris.Render.Protocols.Splash.Rendering;

namespace Microsoft.Iris.Render.Graphics
{
    internal sealed class NullGraphicsDevice : GraphicsDevice, IRenderHandleOwner
    {
        private RemoteNullDevice m_remoteDevice;

        internal NullGraphicsDevice(RenderSession session, bool fUseSplitWindowingPort)
          : base(session, "Null")
        {
            session.AssertOwningThread();
            this.m_remoteDevice = session.BuildRemoteNullDevice(this, fUseSplitWindowingPort);
        }

        protected override void Dispose(bool fInDispose)
        {
            if (fInDispose && this.m_remoteDevice != null)
                this.m_remoteDevice.Dispose();
            this.m_remoteDevice = null;
            base.Dispose(fInDispose);
        }

        RENDERHANDLE IRenderHandleOwner.RenderHandle => this.m_remoteDevice.RenderHandle;

        void IRenderHandleOwner.OnDisconnect() => this.m_remoteDevice = null;

        public override bool IsVideoComposited => false;

        internal override RemoteDevice RemoteDevice => m_remoteDevice;

        public override bool CanPlayAnimations => false;

        internal override bool IsSurfaceFormatSupported(SurfaceFormat nFormat) => nFormat != SurfaceFormat.None;

        protected override void CreateSurfacePoolWorker(
          object oSurfacePool,
          object oHandle,
          SurfaceFormat nFormat)
        {
            this.m_remoteDevice.SendCreateSurfacePool((RENDERHANDLE)oHandle, nFormat);
        }

        protected override void RestartRendering(uint nRenderGeneration) => this.m_remoteDevice.SendRestart(nRenderGeneration);

        public override bool CanPlayAnimationType(AnimationInputType type) => false;

        internal override EffectTemplate CreateEffectTemplate(string stName)
        {
            Debug2.Throw(false, "Not yet implemented");
            return null;
        }
    }
}
