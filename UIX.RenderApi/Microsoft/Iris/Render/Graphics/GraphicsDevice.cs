// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Graphics.GraphicsDevice
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Internal;
using Microsoft.Iris.Render.Protocol;
using Microsoft.Iris.Render.Protocols.Splash.Rendering;
using System;

namespace Microsoft.Iris.Render.Graphics
{
    internal abstract class GraphicsDevice : RenderObject, IGraphicsDevice, IDeviceCallback
    {
        protected RenderSession m_session;
        private string m_stDisplayName;
        private SurfaceManager m_manSurfaces;
        private ObjectTracker m_trackerSurfacePools;
        private ObjectTracker m_trackerWindows;
        private uint m_cRenderGeneration;
        private readonly DeferredHandler m_cbRestartRenderer;
        private bool m_fQueuedRestart;
        private bool m_fCreated;
        private bool m_fAllowDynamicPool;
        private bool m_fInVsaBlock;
        protected GraphicsCaps m_graphicsCaps;
        protected GraphicsDeviceType m_deviceType;
        protected GraphicsRenderingQuality m_renderingQuality;
        private readonly DeferredHandler m_cbTrackerUpdate;
        private bool m_fQueuedTrackerUpdate;
        protected Size m_sizeGutterPxl;
        private static readonly TrackerBase.ObjectTest s_testUnused = new TrackerBase.ObjectTest(TestIsUnused);
        private bool m_fBeginCaptureBackBuffer;
        private bool m_fEndCaptureBackBuffer;

        internal GraphicsDevice(RenderSession session, string stDisplayName)
        {
            this.m_session = session;
            this.m_stDisplayName = stDisplayName;
            this.m_trackerSurfacePools = new ObjectTracker(session, this);
            this.m_trackerWindows = new ObjectTracker(session, this);
            this.m_cRenderGeneration = 1U;
            this.m_cbRestartRenderer = new DeferredHandler(this.OnRestartRenderer);
            this.m_cbTrackerUpdate = new DeferredHandler(this.OnTrackerUpdate);
        }

        public virtual void PostCreate() => this.m_manSurfaces = new SurfaceManager(this, this.m_session);

        protected override void Dispose(bool fInDispose)
        {
            try
            {
                if (fInDispose)
                {
                    this.m_session.AssertOwningThread();
                    if (this.m_manSurfaces != null)
                        this.m_manSurfaces.Dispose();
                    if (this.m_trackerSurfacePools != null)
                        this.m_trackerSurfacePools.Dispose();
                    if (this.m_trackerWindows != null)
                        this.m_trackerWindows.Dispose();
                }
                this.m_trackerWindows = null;
                this.m_trackerSurfacePools = null;
                this.m_manSurfaces = null;
                this.m_session = null;
            }
            finally
            {
                base.Dispose(fInDispose);
            }
        }

        GraphicsDeviceType IGraphicsDevice.DeviceType => this.m_deviceType;

        Size IGraphicsDevice.MaximumImageSize => new Size(this.m_graphicsCaps.MaxTextureWidth, this.m_graphicsCaps.MaxTextureHeight);

        internal RenderSession Session => this.m_session;

        public string DisplayName => this.m_stDisplayName;

        internal SurfaceManager SurfaceManager => this.m_manSurfaces;

        public abstract bool IsVideoComposited { get; }

        public GraphicsRenderingQuality RenderingQuality => this.m_renderingQuality;

        public abstract bool CanPlayAnimations { get; }

        public bool InVsaBlock => this.m_fInVsaBlock;

        internal bool AllowDynamicPool => this.m_fAllowDynamicPool;

        internal abstract RemoteDevice RemoteDevice { get; }

        internal GraphicsCaps GraphicsCaps => this.m_graphicsCaps;

        internal abstract bool IsSurfaceFormatSupported(SurfaceFormat nFormat);

        internal void CreateSurfacePool(SurfacePool spNew, RENDERHANDLE handle, SurfaceFormat nFormat)
        {
            this.CreateSurfacePoolWorker(spNew, handle, nFormat);
            this.m_trackerSurfacePools.AddObject(spNew);
        }

        internal void OnSurfacePoolDispose()
        {
            if (this.m_fQueuedTrackerUpdate)
                return;
            this.m_session.DeferredInvoke(m_cbTrackerUpdate, null, DeferredInvokePriority.Idle, new TimeSpan(0, 0, 10));
            this.m_fQueuedTrackerUpdate = true;
        }

        private void OnTrackerUpdate(object args)
        {
            this.m_trackerSurfacePools.RemoveObjects(s_testUnused);
            this.m_fQueuedTrackerUpdate = false;
        }

        private static bool TestIsUnused(object objSubject, object objParam) => ((SurfacePool)objSubject).RemoteStub == null;

        public void AllocateSurfacePool(int nPoolID, SurfaceFormat nFormat, Size sizeStoragePxl) => this.m_manSurfaces.AllocateNamedSurface(nPoolID, nFormat, sizeStoragePxl);

        public void FreeSurfacePool(int nPoolID) => this.m_manSurfaces.FreeNamedSurface(nPoolID);

        protected abstract void CreateSurfacePoolWorker(
          object oSurfacePool,
          object oHandle,
          SurfaceFormat nFormat);

        internal void CreateVideoPool(VideoPool spNew, RENDERHANDLE handle)
        {
            this.CreateVideoPoolWorker(spNew, handle);
            this.m_trackerSurfacePools.AddObject(spNew);
        }

        internal virtual void CreateVideoPoolWorker(object oSurfacePool, object oHandle) => Debug2.Validate(false, null, "GraphicsDevice type does not support Video Pool");

        internal void RegisterWindow(RenderWindow win) => this.m_trackerWindows.AddObject(win);

        public virtual bool CanPlayAnimationType(AnimationInputType type) => false;

        internal SurfaceManager.VideoMemoryBreakdown ComputeBreakdown() => (SurfaceManager.VideoMemoryBreakdown)this.InternalComputeBreakdown();

        internal abstract EffectTemplate CreateEffectTemplate(string stName);

        internal Image CreateImage(string identifier, ContentNotifyHandler handler) => new Image(this.m_session, this.m_manSurfaces, identifier, handler, this.m_sizeGutterPxl);

        protected virtual object InternalComputeBreakdown() => new SurfaceManager.VideoMemoryBreakdown();

        private void RestoreAllSurfaces()
        {
            this.m_session.AssertOwningThread();
            foreach (SurfacePool liveObject in this.m_trackerSurfacePools.LiveObjects)
                liveObject.Restore();
        }

        protected abstract void RestartRendering(uint nRenderGeneration);

        void IDeviceCallback.OnCreated(RENDERHANDLE target, bool fAllowDynamicPool)
        {
            this.m_fCreated = true;
            this.m_fAllowDynamicPool = fAllowDynamicPool;
        }

        void IDeviceCallback.OnLostDevice(
          RENDERHANDLE target,
          uint cRenderGeneration,
          bool fLost)
        {
            if (fLost)
            {
                this.m_cRenderGeneration = cRenderGeneration;
            }
            else
            {
                this.ResetDeviceContent();
                if (this.m_fQueuedRestart)
                    return;
                this.m_session.DeferredInvoke(m_cbRestartRenderer, null, DeferredInvokePriority.Low);
                this.m_fQueuedRestart = true;
            }
        }

        internal virtual void ResetDeviceContent()
        {
            if (this.m_manSurfaces != null)
                this.m_manSurfaces.CollectDeadSurfaces();
            this.RestoreAllSurfaces();
        }

        void IDeviceCallback.OnVsaBlock(RENDERHANDLE target, bool fInBlock) => this.m_fInVsaBlock = fInBlock;

        void IDeviceCallback.OnSurfacePoolAllocation(
          RENDERHANDLE target,
          RENDERHANDLE idSurfacePool,
          SurfacePoolAllocationResult nResult)
        {
            IRenderHandleOwner handleOwner = this.Session.RenderingPort.TryGetHandleOwner(idSurfacePool);
            if (handleOwner == null)
                return;
            if (handleOwner is SurfacePool surfacePool)
                surfacePool.NotifyAllocation(nResult);
            else
                Debug2.Throw(false, "invalid surface pool owner");
        }

        private void OnRestartRenderer(object arg)
        {
            this.RestartRendering(this.m_cRenderGeneration);
            this.m_fQueuedRestart = false;
        }

        public virtual void RenderNowIfPossible()
        {
        }

        public void BeginCaptureBackBuffer(string stFileName)
        {
            Debug2.Validate(!this.m_fBeginCaptureBackBuffer, typeof(InvalidOperationException), "BeginCaptureBackBuffer() called but m_fBeginCaptureBackBuffer = true");
            Debug2.Validate(!this.m_fEndCaptureBackBuffer, typeof(InvalidOperationException), "EndCaptureBackBuffer() called but m_fEndCaptureBackBuffer = true");
            this.m_fBeginCaptureBackBuffer = true;
            this.RemoteDevice.SendBeginCaptureBackBuffer(stFileName);
        }

        public void EndCaptureBackBuffer()
        {
            Debug2.Validate(this.m_fBeginCaptureBackBuffer, typeof(InvalidOperationException), "BeginCaptureBackBuffer() called but m_fBeginCaptureBackBuffer = false");
            Debug2.Validate(!this.m_fEndCaptureBackBuffer, typeof(InvalidOperationException), "EndCaptureBackBuffer() called but m_fEndCaptureBackBuffer = true");
            this.m_session.DeferredInvoke(new DeferredHandler(this.DeferredEndCaptureBackBuffer), null, DeferredInvokePriority.VisualUpdate);
        }

        private void DeferredEndCaptureBackBuffer(object args)
        {
            Debug2.Validate(this.m_fBeginCaptureBackBuffer, typeof(InvalidOperationException), "DeferredEndCaptureBackBuffer() called but m_fBeginCaptureBackBuffer = false");
            Debug2.Validate(!this.m_fEndCaptureBackBuffer, typeof(InvalidOperationException), "DeferredEndCaptureBackBuffer() called but m_fEndCaptureBackBuffer = true");
            this.m_fEndCaptureBackBuffer = true;
            this.RemoteDevice.SendEndCaptureBackBuffer();
        }

        void IDeviceCallback.OnBackBufferCaptured(RENDERHANDLE target)
        {
            Debug2.Validate(this.BackBufferCapturedEvent != null, typeof(InvalidOperationException), "No event handler registered: BackBufferCapturedEvent = null");
            this.m_fBeginCaptureBackBuffer = false;
            this.m_fEndCaptureBackBuffer = false;
            this.BackBufferCapturedEvent();
        }

        public void TriggerDeviceReset() => this.RemoteDevice.SendTriggerDeviceReset();

        public event BackBufferCapturedHandler BackBufferCapturedEvent;
    }
}
