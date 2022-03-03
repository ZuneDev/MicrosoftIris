// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Graphics.DynamicBlock
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Protocols.Splash.Rendering;
using System;
using System.Collections;
using System.Diagnostics;

namespace Microsoft.Iris.Render.Graphics
{
    internal sealed class DynamicBlock : IDynamicBlock
    {
        private DynamicRow m_rowOwner;
        private Rectangle m_rcLocationPxl;
        private Size m_sizeContentPxl;
        private Surface m_surface;
        private bool m_fRotated;
        private Size m_sizeGutter;

        internal DynamicBlock(
          DynamicRow rowOwner,
          Rectangle rcLocationPxl,
          Size sizeContentPxl,
          Size sizeGutter,
          bool fRotated)
        {
            this.m_rowOwner = rowOwner;
            this.m_fRotated = fRotated;
            this.m_rcLocationPxl = rcLocationPxl;
            this.m_sizeContentPxl = sizeContentPxl;
            this.m_sizeGutter = sizeGutter;
        }

        internal Surface Create(ISurfaceContentOwner surfaceOwner)
        {
            Surface surface = new Surface(this.m_rowOwner.DynamicPool.SurfacePool, surfaceOwner, this.m_sizeContentPxl, this.m_sizeGutter);
            this.m_surface = surface;
            RemoteSurface remoteStub = surface.RemoteStub;
            remoteStub.SendRemapLocation(this.m_rcLocationPxl);
            remoteStub.SendSetRotation(this.m_fRotated);
            remoteStub.SendSetGutter(this.m_sizeGutter);
            return surface;
        }

        ~DynamicBlock() => this.Dispose(false);

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            this.Dispose(true);
        }

        private void Dispose(bool fInDispose)
        {
            this.m_surface = null;
            if (!fInDispose)
                return;
            this.m_rowOwner.NotifyBlockDestroyed(this);
        }

        public int NearEdge => this.m_rcLocationPxl.Left;

        public int FarEdge => this.m_rcLocationPxl.Right;

        internal Point StoragePosition
        {
            get => this.m_rcLocationPxl.Location;
            set
            {
                if (!(this.m_rcLocationPxl.Location != value))
                    return;
                this.m_rcLocationPxl.Location = value;
            }
        }

        internal Size StorageSize
        {
            get => this.m_rcLocationPxl.Size;
            set
            {
                if (!(this.m_rcLocationPxl.Size != value))
                    return;
                this.m_rcLocationPxl.Size = value;
            }
        }

        public Surface Surface => this.m_surface;

        public DynamicRow Row => this.m_rowOwner;

        internal void NotifyPositionChanged(ArrayList alReloadContent, Point ptNewOffsetPxl)
        {
            if (!(ptNewOffsetPxl != this.m_rcLocationPxl.Location))
                return;
            this.m_rcLocationPxl.Location = ptNewOffsetPxl;
            Surface surface = this.Surface;
            if (surface == null)
                return;
            RemoteSurface remoteStub = surface.RemoteStub;
            switch (surface.SurfacePool.StorageState)
            {
                case SurfacePoolAllocationResult.NotAllocated:
                case SurfacePoolAllocationResult.Requested:
                case SurfacePoolAllocationResult.Immoveable:
                    remoteStub.SendRemapLocation(this.m_rcLocationPxl);
                    alReloadContent.Add(surface);
                    break;
            }
        }

        internal void NotifyRowChanged(
          ArrayList alReloadContent,
          DynamicRow rowNewOwner,
          DynamicRow rowOldOwner,
          Point ptNewOffsetPxl)
        {
            this.m_rowOwner = rowNewOwner;
            this.NotifyPositionChanged(alReloadContent, ptNewOffsetPxl);
        }

        [Conditional("DEBUG")]
        private void OnPositionChanged()
        {
        }
    }
}
