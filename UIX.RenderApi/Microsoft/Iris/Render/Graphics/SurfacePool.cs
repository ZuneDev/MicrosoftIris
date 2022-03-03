// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Graphics.SurfacePool
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Internal;
using Microsoft.Iris.Render.Protocol;
using Microsoft.Iris.Render.Protocols.Splash.Rendering;
using System;
using System.Collections;

namespace Microsoft.Iris.Render.Graphics
{
    internal class SurfacePool : RenderObject, IRenderHandleOwner
    {
        private GraphicsDevice m_device;
        private RenderSession m_session;
        private ObjectTracker m_trackerSurfaces;
        protected RemoteSurfacePool m_remotePool;
        private Size m_sizeStoragePxl;
        private bool m_fDestroyWhenEmpty;
        private SurfaceFormat m_nFormat;
        private SurfacePoolAllocationResult m_nStorage;
        private bool m_fPrimaryOwner;
        private Effect m_effect;

        internal SurfacePool(GraphicsDevice device, SurfaceFormat nFormat, RENDERHANDLE rhExternal) => this.CommonCreate(device, nFormat, rhExternal);

        internal SurfacePool(GraphicsDevice device, Size sizeStoragePxl, SurfaceFormat nFormat)
        {
            Debug2.Validate(sizeStoragePxl.Width > 0 && sizeStoragePxl.Height > 0, typeof(ArgumentException), "Must have positive area");
            this.CommonCreate(device, nFormat, RENDERHANDLE.NULL);
            this.m_sizeStoragePxl = sizeStoragePxl;
            this.m_nStorage = SurfacePoolAllocationResult.NotAllocated;
            this.Allocate();
        }

        private void CommonCreate(
          GraphicsDevice device,
          SurfaceFormat nFormat,
          RENDERHANDLE rhExternal)
        {
            Debug2.Validate(nFormat != SurfaceFormat.None, typeof(ArgumentException), nameof(nFormat));
            Debug2.Validate(device != null, typeof(ArgumentNullException), "Creating a SurfacePool requires a GraphicsDevice");
            this.m_device = device;
            this.m_session = device.Session;
            Debug2.Validate(this.m_session != null, typeof(InvalidOperationException), "Creating a SurfacePool requires a RenderSession");
            this.m_session.AssertOwningThread();
            this.m_remotePool = null;
            this.m_sizeStoragePxl = new Size();
            this.m_nFormat = nFormat;
            RENDERHANDLE handle = rhExternal;
            if (handle == RENDERHANDLE.NULL)
            {
                handle = this.m_session.RenderingPort.AllocHandle(this);
                this.m_fPrimaryOwner = true;
            }
            try
            {
                this.CreateRemoteSurfacePool(handle, this.m_fPrimaryOwner);
            }
            finally
            {
                if (this.m_remotePool == null)
                {
                    GC.SuppressFinalize(this);
                    if (this.m_fPrimaryOwner)
                        this.m_session.RenderingPort.FreeHandle(handle);
                }
            }
            this.m_trackerSurfaces = new ObjectTracker(this.m_session, this);
            if (this.m_fPrimaryOwner)
                return;
            this.m_nStorage = SurfacePoolAllocationResult.Immoveable;
        }

        protected virtual void CreateRemoteSurfacePool(RENDERHANDLE handle, bool fPrimaryOwner)
        {
            if (fPrimaryOwner)
            {
                this.m_device.CreateSurfacePool(this, handle, this.Format);
                this.m_remotePool = RemoteSurfacePool.CreateFromHandle(this.m_session.RenderingPort, handle);
            }
            else
                this.m_remotePool = RemoteSurfacePool.CreateFromExternalHandle(this.m_session.RenderingPort, handle, this);
        }

        protected override void Dispose(bool fInDispose)
        {
            try
            {
                if (fInDispose)
                {
                    if (this.m_effect != null)
                        this.m_effect.UnregisterUsage(this);
                    this.DisposeAllSurfaces();
                    this.m_trackerSurfaces.Dispose();
                    this.m_remotePool.Dispose();
                    this.m_device.OnSurfacePoolDispose();
                }
                this.m_effect = null;
                this.m_remotePool = null;
                this.m_trackerSurfaces = null;
                this.m_device = null;
            }
            finally
            {
                base.Dispose(fInDispose);
            }
        }

        RENDERHANDLE IRenderHandleOwner.RenderHandle => this.m_remotePool.RenderHandle;

        void IRenderHandleOwner.OnDisconnect() => this.m_remotePool = null;

        public Size StorageSize => this.m_sizeStoragePxl;

        public SurfaceFormat Format => this.m_nFormat;

        internal RemoteSurfacePool RemoteStub => this.m_remotePool;

        protected RenderSession Session => this.m_session;

        internal GraphicsDevice Device => this.m_device;

        internal bool DestroyWhenEmpty
        {
            get => this.m_fDestroyWhenEmpty;
            set => this.m_fDestroyWhenEmpty = value;
        }

        internal IList AllSurfaces
        {
            get
            {
                IList list = null;
                if (this.m_trackerSurfaces != null)
                    list = m_trackerSurfaces.LiveObjects;
                return list;
            }
        }

        internal SurfacePoolAllocationResult StorageState => this.m_nStorage;

        public Effect Effect
        {
            get => this.m_effect;
            set
            {
                Effect effect = value;
                if (this.m_effect == effect)
                    return;
                if (this.m_effect != null)
                    this.m_effect.UnregisterUsage(this);
                this.m_effect = effect;
                if (this.m_effect != null)
                    this.m_effect.RegisterUsage(this);
                this.m_remotePool.SendSetEffect(this.m_effect == null ? null : this.m_effect.RemoteStub);
            }
        }

        internal int CreateSurface(Surface spNew, RENDERHANDLE handle)
        {
            this.m_remotePool.SendCreateSurface(handle);
            return this.AttachSurface(spNew);
        }

        internal int AttachSurface(Surface spNew) => this.m_trackerSurfaces.AddObject(spNew);

        internal void DisposeAllSurfaces()
        {
            if (this.m_trackerSurfaces == null)
                return;
            ArrayList liveObjects = this.m_trackerSurfaces.LiveObjects;
            for (int index = 0; index < liveObjects.Count; ++index)
                ((SharedRenderObject)liveObjects[index]).UnregisterUsage(this);
        }

        internal void NotifyDisposeSurface(int nID)
        {
            if (this.m_trackerSurfaces == null)
                return;
            this.m_trackerSurfaces.RemoveObject(nID);
            if (!this.m_fDestroyWhenEmpty || !this.m_trackerSurfaces.IsEmpty)
                return;
            this.Dispose();
        }

        internal void Restore()
        {
            if (this.m_remotePool == null || !this.m_remotePool.IsValid)
                return;
            if (!this.m_sizeStoragePxl.IsZero)
                this.Allocate();
            ArrayList liveObjects = this.m_trackerSurfaces.LiveObjects;
            for (int index = 0; index < liveObjects.Count; ++index)
                ((Surface)liveObjects[index]).RestoreContent();
        }

        internal static void RemapSurface(Surface surCur, SurfacePool poolNew)
        {
            SurfacePool surfacePool = surCur.SurfacePool;
            if (poolNew == surfacePool)
                return;
            int surfacePoolInstanceId = surCur.SurfacePoolInstanceID;
            surfacePool.NotifyDisposeSurface(surfacePoolInstanceId);
            int nNewInstanceID = poolNew.m_trackerSurfaces.AddObject(surCur);
            surCur.NotifyNewPool(poolNew, nNewInstanceID);
        }

        internal void NotifyAllocation(SurfacePoolAllocationResult nResult) => this.m_nStorage = nResult;

        private void Allocate()
        {
            this.m_remotePool.SendAllocate(this.m_sizeStoragePxl);
            this.m_nStorage = SurfacePoolAllocationResult.Requested;
        }
    }
}
