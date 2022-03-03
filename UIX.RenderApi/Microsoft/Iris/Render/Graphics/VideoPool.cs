// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Graphics.VideoPool
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Internal;
using Microsoft.Iris.Render.Protocol;
using Microsoft.Iris.Render.Protocols.Splash.Rendering;

namespace Microsoft.Iris.Render.Graphics
{
    internal sealed class VideoPool : SurfacePool, IVideoPoolCallback
    {
        private RemoteVideoPool m_remoteVideoPool;
        private IVideoPoolNotification m_imgOwner;
        private float m_flContentOverscan;
        private Size m_sizeTargetPxl;

        internal VideoPool(GraphicsDevice device, IVideoPoolNotification imgOwner)
          : base(device, SurfaceFormat.External, RENDERHANDLE.NULL)
          => this.m_imgOwner = imgOwner;

        protected override void CreateRemoteSurfacePool(RENDERHANDLE rhHandle, bool fPrimaryOwner)
        {
            if (fPrimaryOwner)
            {
                this.Device.CreateVideoPool(this, rhHandle);
                this.m_remoteVideoPool = RemoteVideoPool.CreateFromHandle(this.Session.RenderingPort, rhHandle);
            }
            else
                this.m_remoteVideoPool = RemoteVideoPool.CreateFromExternalHandle(this.Session.RenderingPort, rhHandle, this);
            this.m_remotePool = m_remoteVideoPool;
        }

        internal RemoteVideoPool RemoteVideoStub => this.m_remoteVideoPool;

        internal float ContentOverscan
        {
            get => this.m_flContentOverscan;
            set
            {
                if (Math2.WithinEpsilon(this.m_flContentOverscan, value))
                    return;
                this.m_flContentOverscan = value;
                this.m_remoteVideoPool.SendSetContentOverscan(value);
            }
        }

        internal void NotifyVideoModeChanged(Size sizeTargetPxl)
        {
            if (!(this.m_sizeTargetPxl != sizeTargetPxl))
                return;
            this.m_sizeTargetPxl = sizeTargetPxl;
            this.m_remoteVideoPool.SendNotifyVideoSizeChanged(sizeTargetPxl);
        }

        void IVideoPoolCallback.OnInputChanged(
          RENDERHANDLE target,
          Size sizeTargetPxl,
          Size sizeAspectRatio,
          uint nFrameRateNumerator,
          uint nFrameRateDenominator,
          SurfaceFormat nFormat)
        {
            this.m_imgOwner.NotifySourceChanged(sizeTargetPxl, sizeAspectRatio);
        }

        void IVideoPoolCallback.OnInvalidate(RENDERHANDLE target) => this.m_imgOwner.InvalidateClients();
    }
}
