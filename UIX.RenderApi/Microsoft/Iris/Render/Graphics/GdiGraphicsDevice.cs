// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Graphics.GdiGraphicsDevice
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Internal;
using Microsoft.Iris.Render.Protocol;
using Microsoft.Iris.Render.Protocols.Splash.Rendering;
using Microsoft.Iris.Render.Protocols.Splash.Rendering.Nt;

namespace Microsoft.Iris.Render.Graphics
{
    internal sealed class GdiGraphicsDevice : GraphicsDevice, IRenderHandleOwner
    {
        private RemoteGdiDevice m_remoteDevice;
        private bool m_fAllowAlpha;
        private bool m_fAllowAnimations;
        private bool m_fEnableBackBuffer;
        private bool m_fSystemMemoryBitmaps;
        private ColorF m_clrBackground;
        private bool m_fEnableAlphaTracking;
        private VideoZoomMode m_vzmZoomMode;
        private bool m_fSecureDesktopMode;

        internal GdiGraphicsDevice(RenderSession session)
          : base(session, "Gdi")
        {
            this.m_remoteDevice = session.BuildRemoteGdiDevice(this);
            this.m_fAllowAlpha = true;
            this.m_fAllowAnimations = true;
            this.m_fEnableBackBuffer = true;
            this.m_fSystemMemoryBitmaps = false;
            this.m_clrBackground = new ColorF(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
            this.m_deviceType = GraphicsDeviceType.Gdi;
            this.m_graphicsCaps = session.GetGraphicsCaps(this.m_deviceType);
            this.m_renderingQuality = GraphicsRenderingQuality.MinQuality;
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

        public override bool CanPlayAnimations => this.AllowAnimations;

        public bool AllowAlpha
        {
            get => this.m_fAllowAlpha;
            set
            {
                if (this.m_fAllowAlpha == value)
                    return;
                this.m_remoteDevice.SendSetAllowAlpha(value);
                this.m_fAllowAlpha = value;
            }
        }

        public bool AllowAnimations
        {
            get => this.m_fAllowAnimations;
            set
            {
                if (this.m_fAllowAnimations == value)
                    return;
                this.m_remoteDevice.SendSetAllowAnimations(value);
                this.m_fAllowAnimations = value;
            }
        }

        public bool EnableBackBuffer
        {
            get => this.m_fEnableBackBuffer;
            set
            {
                if (value == this.m_fEnableBackBuffer)
                    return;
                this.m_remoteDevice.SendSetEnableBackBuffer(value);
                this.m_fEnableBackBuffer = value;
            }
        }

        public bool EnableAlphaTracking
        {
            get => this.m_fEnableAlphaTracking;
            set
            {
                if (value == this.m_fEnableAlphaTracking)
                    return;
                this.m_remoteDevice.SendSetEnableAlphaTracking(value);
                this.m_fEnableAlphaTracking = value;
            }
        }

        public VideoZoomMode ZoomMode
        {
            get => this.m_vzmZoomMode;
            set
            {
                if (value == this.m_vzmZoomMode)
                    return;
                this.m_remoteDevice.SendSetZoomMode(value);
                this.m_vzmZoomMode = value;
            }
        }

        public bool SystemMemoryBitmaps
        {
            get => this.m_fSystemMemoryBitmaps;
            set
            {
                if (value == this.m_fSystemMemoryBitmaps)
                    return;
                this.m_remoteDevice.SendSetUseSystemMemoryBitmaps(value);
                this.m_fSystemMemoryBitmaps = value;
            }
        }

        public ColorF BackgroundColor
        {
            get => this.m_clrBackground;
            set
            {
                if (!(value != this.m_clrBackground))
                    return;
                this.m_remoteDevice.SendSetBackgroundColor(value);
                this.m_clrBackground = value;
            }
        }

        internal bool SecureDesktopMode
        {
            get => this.m_fSecureDesktopMode;
            set
            {
                if (value == this.m_fSecureDesktopMode)
                    return;
                this.m_remoteDevice.SendSecureDesktopMode(value);
                this.m_fSecureDesktopMode = value;
            }
        }

        internal override bool IsSurfaceFormatSupported(SurfaceFormat nFormat)
        {
            switch (nFormat)
            {
                case SurfaceFormat.RGB24:
                case SurfaceFormat.RGB32:
                case SurfaceFormat.ARGB32:
                    return true;
                case SurfaceFormat.External:
                    return false;
                default:
                    return false;
            }
        }

        protected override void CreateSurfacePoolWorker(
          object oSurfacePool,
          object oHandle,
          SurfaceFormat nFormat)
        {
            this.m_remoteDevice.SendCreateSurfacePool((RENDERHANDLE)oHandle, nFormat);
        }

        protected override void RestartRendering(uint nRenderGeneration) => this.m_remoteDevice.SendRestart(nRenderGeneration);

        public override bool CanPlayAnimationType(AnimationInputType type) => false;

        internal override EffectTemplate CreateEffectTemplate(string stName) => new GdiEffectTemplate(this.m_session, this, stName);
    }
}
