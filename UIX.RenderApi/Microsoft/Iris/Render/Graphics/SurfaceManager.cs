// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Graphics.SurfaceManager
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Internal;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Iris.Render.Graphics
{
    internal sealed class SurfaceManager : RenderObject
    {
        private GraphicsDevice m_device;
        private ArrayList m_alDynamicSets;
        private Map<int, SurfacePool> m_dictNamedPools;
        private Size m_sizeThresholdPxl;
        private LinkedList<Surface> m_listLargeSurfaces;
        private SurfaceManager.VideoMemoryBreakdown m_vmbStartup;
        private bool m_fAllowDynamicPool;

        internal SurfaceManager(GraphicsDevice device, RenderSession session)
        {
            session.AssertOwningThread();
            Debug2.Validate(session != null, typeof(InvalidOperationException), "Creating a SurfaceManager requires a valid RenderSession");
            this.m_device = device;
            this.m_alDynamicSets = new ArrayList();
            this.m_dictNamedPools = new Map<int, SurfacePool>();
            this.m_listLargeSurfaces = new LinkedList<Surface>();
            this.m_vmbStartup = this.m_device.ComputeBreakdown();
            this.m_fAllowDynamicPool = this.m_device.AllowDynamicPool;
            if (!this.m_fAllowDynamicPool || this.m_vmbStartup.sizeDynamicPoolPxl.Height <= 0)
                return;
            this.PreallocateDynamicPools(this.m_vmbStartup);
        }

        protected override void Dispose(bool fInDispose)
        {
            try
            {
                if (fInDispose)
                {
                    foreach (RenderObject alDynamicSet in this.m_alDynamicSets)
                        alDynamicSet.Dispose();
                    this.CollectLargeSurfaces();
                }
                this.m_listLargeSurfaces = null;
                this.m_alDynamicSets = null;
                this.m_device = null;
            }
            finally
            {
                base.Dispose(fInDispose);
            }
        }

        private void PreallocateDynamicPools(SurfaceManager.VideoMemoryBreakdown vmb)
        {
            this.m_alDynamicSets.Add(new DynamicPoolSet(this.m_device, SurfaceFormat.ARGB32));
            int num = 0;
            int index = 0;
            while (num < vmb.cbMinimumPool)
            {
                DynamicPool dynamicPool = ((DynamicPoolSet)this.m_alDynamicSets[index]).AllocateDynamicPool(vmb.sizeDynamicPoolPxl, 8, vmb.ThresholdHeightPxl);
                num += dynamicPool.TotalSize.Width * dynamicPool.TotalSize.Height * SurfaceFormatInfo.GetBitsPerPixel(dynamicPool.Format) / 8;
                index = (index + 1) % this.m_alDynamicSets.Count;
            }
            this.m_sizeThresholdPxl = new Size(vmb.sizeDynamicPoolPxl.Width, vmb.ThresholdHeightPxl);
        }

        public Size EstimatedMaxPerformantResolution => this.m_vmbStartup.sizeLargeBuffersPxl;

        public Size MaximumSmallPoolBlockSize => this.m_sizeThresholdPxl;

        public Surface RequestSurface(
          ISurfaceContentOwner surfaceOwner,
          SurfaceFormat nFormat,
          Size sizeContentPxl,
          Size sizeGutter)
        {
            Debug2.Validate(surfaceOwner != null, typeof(ArgumentNullException), "Must provide valid surface owner");
            Debug2.Validate(this.m_device.IsSurfaceFormatSupported(nFormat), typeof(ArgumentException), "Surface format is not supported");
            Debug2.Validate(sizeContentPxl.Width > 0 && sizeContentPxl.Height > 0, typeof(ArgumentOutOfRangeException), "Must provide positive area for surface");
            bool flag1 = false;
            bool flag2 = false;
            Surface surface = null;
            DynamicPoolSet dynamicPoolSet = null;
            for (int index = 0; index < this.m_alDynamicSets.Count; ++index)
            {
                dynamicPoolSet = (DynamicPoolSet)this.m_alDynamicSets[index];
                if (dynamicPoolSet.Format == nFormat)
                {
                    flag1 = true;
                    break;
                }
            }
            if (flag1)
            {
                Size sizeRequestPxl;
                bool fRotated;
                if (sizeContentPxl.Width >= sizeContentPxl.Height)
                {
                    sizeRequestPxl = sizeContentPxl;
                    fRotated = false;
                }
                else
                {
                    sizeRequestPxl = new Size(sizeContentPxl.Height, sizeContentPxl.Width);
                    fRotated = true;
                    sizeGutter = new Size(sizeGutter.Height, sizeGutter.Width);
                }
                sizeRequestPxl.Width += sizeGutter.Width * 2;
                sizeRequestPxl.Height += sizeGutter.Height * 2;
                if (sizeRequestPxl.Width <= this.m_sizeThresholdPxl.Width && sizeRequestPxl.Height <= this.m_sizeThresholdPxl.Height)
                {
                    surface = dynamicPoolSet.RequestSurface(surfaceOwner, sizeRequestPxl, sizeContentPxl, sizeGutter, fRotated, PoolAllocMethod.Available, PoolAllocMethod.ScavengeDead) ?? dynamicPoolSet.RequestSurface(surfaceOwner, sizeRequestPxl, sizeContentPxl, sizeGutter, fRotated, PoolAllocMethod.CompactDead, PoolAllocMethod.CompactAll);
                    if (surface != null)
                        ;
                }
            }
            else
            {
                int num = this.m_fAllowDynamicPool ? 1 : 0;
            }
            if (surface == null)
            {
                this.CollectLargeSurfaces();
                flag2 = true;
                surface = new Surface(new SurfacePool(this.m_device, sizeContentPxl, nFormat)
                {
                    DestroyWhenEmpty = true
                }, surfaceOwner, sizeContentPxl);
                surface.RegisterUsage(this);
            }
            if (flag2)
                this.m_listLargeSurfaces.AddLast(new LinkedListNode<Surface>(surface));
            return surface;
        }

        public Surface RequestNamedSurface(
          ISurfaceContentOwner surfaceOwner,
          Size sizeContentPxl,
          int nPoolID)
        {
            Debug2.Validate(surfaceOwner != null, typeof(ArgumentNullException), "Must provide valid surface owner");
            Debug2.Validate(sizeContentPxl.Width > 0 && sizeContentPxl.Height > 0, typeof(ArgumentOutOfRangeException), "Must provide positive area for surface");
            Debug2.Validate(nPoolID >= 0, typeof(ArgumentException), "Must have valid ID or 0");
            SurfacePool dictNamedPool = this.m_dictNamedPools[nPoolID];
            Debug2.Validate(dictNamedPool != null, typeof(ArgumentException), "Must have previously allocated pool");
            dictNamedPool.DisposeAllSurfaces();
            return new Surface(dictNamedPool, surfaceOwner, sizeContentPxl);
        }

        public void AllocateNamedSurface(int nPoolID, SurfaceFormat nFormat, Size sizeStoragePxl)
        {
            Debug2.Validate(nPoolID > 0, typeof(ArgumentException), "Must have valid ID");
            Debug2.Validate(this.m_device.IsSurfaceFormatSupported(nFormat), typeof(ArgumentException), "Surface format is not supported");
            Debug2.Validate(sizeStoragePxl.Width > 0 && sizeStoragePxl.Height > 0, typeof(ArgumentException), "Must have valid surface size");
            Debug2.Validate(this.m_dictNamedPools[nPoolID] == null, null, "SurfacePool must not already be allocated");
            SurfacePool surfacePool = new SurfacePool(this.m_device, sizeStoragePxl, nFormat);
            try
            {
                surfacePool.DestroyWhenEmpty = false;
                this.m_dictNamedPools[nPoolID] = surfacePool;
                surfacePool = null;
            }
            finally
            {
                surfacePool?.Dispose();
            }
        }

        public void FreeNamedSurface(int nPoolID)
        {
            Debug2.Validate(nPoolID > 0, null, "Must have valid ID");
            SurfacePool dictNamedPool = this.m_dictNamedPools[nPoolID];
            Debug2.Validate(dictNamedPool != null, null, "Must have previously allocated pool");
            this.m_dictNamedPools.Remove(nPoolID);
            dictNamedPool.Dispose();
        }

        public void CollectDeadSurfaces()
        {
            foreach (DynamicPoolSet alDynamicSet in this.m_alDynamicSets)
            {
                foreach (DynamicPool pool in alDynamicSet.Pools)
                    pool.CollectDeadSurfaces();
            }
            this.CollectLargeSurfaces();
        }

        private void CollectLargeSurfaces()
        {
            LinkedListNode<Surface> next;
            for (LinkedListNode<Surface> node = this.m_listLargeSurfaces.First; node != null; node = next)
            {
                next = node.Next;
                Surface surface = node.Value;
                if (!surface.InUse)
                {
                    surface.UnregisterUsage(this);
                    this.m_listLargeSurfaces.Remove(node);
                }
            }
        }

        public struct VideoMemoryBreakdown
        {
            public Size sizeBuffersPxl;
            public uint cbVideo;
            public Size sizeBackgroundPxl;
            public Size sizeDynamicPoolPxl;
            public Size sizeLargeBuffersPxl;
            public uint cbMinimumPool;

            public int ThresholdHeightPxl => Math.Max(Math.Min(this.sizeDynamicPoolPxl.Height, 350), this.sizeDynamicPoolPxl.Height * 350 / 2048);
        }
    }
}
