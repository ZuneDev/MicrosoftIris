// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Graphics.NtGraphicsDevice
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Internal;
using Microsoft.Iris.Render.Protocol;
using Microsoft.Iris.Render.Protocols.Splash.Rendering;
using Microsoft.Iris.Render.Protocols.Splash.Rendering.Nt;
using System;

namespace Microsoft.Iris.Render.Graphics
{
    internal sealed class NtGraphicsDevice : Dx9GraphicsDevice, IRenderHandleOwner
    {
        private RemoteNtDevice m_remoteDevice;
        private float m_flDisplayOverscan;
        private float m_flMaxOverscan;

        internal NtGraphicsDevice(RenderSession session, GraphicsRenderingQuality renderingQuality)
          : base(session, "Nt Dx9", GraphicsDeviceType.Direct3D9, renderingQuality)
          => this.m_remoteDevice = session.BuildRemoteNtDevice(this);

        protected override void Dispose(bool fInDispose)
        {
            try
            {
                if (fInDispose && this.m_remoteDevice != null)
                    this.m_remoteDevice.Dispose();
                this.m_remoteDevice = null;
            }
            finally
            {
                base.Dispose(fInDispose);
            }
        }

        RENDERHANDLE IRenderHandleOwner.RenderHandle => this.m_remoteDevice.RenderHandle;

        void IRenderHandleOwner.OnDisconnect() => this.m_remoteDevice = null;

        internal RemoteNtDevice RemoteStub => this.m_remoteDevice;

        internal override RemoteDevice RemoteDevice => m_remoteDevice;

        public Size EstimatedMaxPerformantResolution
        {
            get
            {
                Debug2.Validate(this.SurfaceManager != null, typeof(InvalidOperationException), "Dx9 driver must be fully initialized before querying resolution performance");
                return this.SurfaceManager.EstimatedMaxPerformantResolution;
            }
        }

        public float DisplayOverscan
        {
            get => this.m_flDisplayOverscan;
            set
            {
                if (Math2.WithinEpsilon(this.m_flDisplayOverscan, value))
                    return;
                this.m_flDisplayOverscan = value;
                this.m_remoteDevice.SendSetDisplayOverscan(value);
            }
        }

        public float MaxOverscan
        {
            get => this.m_flMaxOverscan;
            set
            {
                if (Math2.WithinEpsilon(this.m_flMaxOverscan, value))
                    return;
                this.m_flMaxOverscan = value;
                this.m_remoteDevice.SendSetMaxOverscan(value);
            }
        }

        public override void PostCreate()
        {
            base.PostCreate();
            SurfaceFormat nFormat = SurfaceFormat.ARGB32;
            this.m_remoteDevice.SendCreateTransferBuffer(new Size(2048, 512), nFormat);
            for (int index = 0; index < 2; ++index)
                this.m_remoteDevice.SendCreateTransferBuffer(new Size(1024, 256), nFormat);
            for (int index = 0; index < 4; ++index)
                this.m_remoteDevice.SendCreateTransferBuffer(new Size(256, 256), nFormat);
            for (int index = 0; index < 8; ++index)
                this.m_remoteDevice.SendCreateTransferBuffer(new Size(128, 64), nFormat);
        }

        internal override bool IsSurfaceFormatSupported(SurfaceFormat nFormat)
        {
            switch (nFormat)
            {
                case SurfaceFormat.A8:
                case SurfaceFormat.RGB24:
                case SurfaceFormat.RGB32:
                case SurfaceFormat.ARGB32:
                    return true;
                case SurfaceFormat.External:
                    return true;
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

        internal override void CreateVideoPoolWorker(object oSurfacePool, object oHandle) => this.m_remoteDevice.SendCreateVideoPool(this.m_session.RenderingProtocol.LocalVideoPoolCallbackHandle, (RENDERHANDLE)oHandle);

        protected override void RestartRendering(uint nRenderGeneration) => this.m_remoteDevice.SendRestart(nRenderGeneration);

        public override void RenderNowIfPossible() => this.m_remoteDevice.SendRenderNowIfPossible();

        private static uint GetPixelCB(Size sizeSurfacePxl) => (uint)(Math2.FindPowerOf2(sizeSurfacePxl.Width) * Math2.FindPowerOf2(sizeSurfacePxl.Height) * 4);

        protected override object InternalComputeBreakdown()
        {
            SurfaceManager.VideoMemoryBreakdown videoMemoryBreakdown = new SurfaceManager.VideoMemoryBreakdown();
            ulong totalVideoMemory = this.m_graphicsCaps.TotalVideoMemory;
            ulong dedicatedVideoMemory = this.m_graphicsCaps.DedicatedVideoMemory;
            ulong num1 = totalVideoMemory;
            videoMemoryBreakdown.sizeBuffersPxl = new Size(Win32Api.GetSystemMetrics(0), Win32Api.GetSystemMetrics(1));
            Size sizeSurfacePxl = new Size(1280, 1024);
            if (totalVideoMemory > 67108864UL)
                sizeSurfacePxl = totalVideoMemory <= 134217728UL ? new Size(1920, 1280) : new Size(2048, 1600);
            videoMemoryBreakdown.sizeLargeBuffersPxl = sizeSurfacePxl;
            uint num2 = GetPixelCB(sizeSurfacePxl) * 3U;
            ulong num3 = num1 - num2;
            videoMemoryBreakdown.cbVideo = 16777216U;
            ulong num4 = num3 - videoMemoryBreakdown.cbVideo;
            videoMemoryBreakdown.sizeBackgroundPxl = new Size(1024, 768);
            uint pixelCb = GetPixelCB(videoMemoryBreakdown.sizeBackgroundPxl);
            ulong num5 = num4 - pixelCb - totalVideoMemory / 20UL;
            videoMemoryBreakdown.sizeDynamicPoolPxl = dedicatedVideoMemory >= 121634816UL ? new Size(2048, 2048) : new Size(1920, 548);
            uint num6 = (uint)(videoMemoryBreakdown.sizeDynamicPoolPxl.Width * videoMemoryBreakdown.sizeDynamicPoolPxl.Height * 4);
            for (uint index = 33554432; videoMemoryBreakdown.cbMinimumPool < index && num5 > num6; num5 -= num6)
                videoMemoryBreakdown.cbMinimumPool += num6;
            return videoMemoryBreakdown;
        }
    }
}
