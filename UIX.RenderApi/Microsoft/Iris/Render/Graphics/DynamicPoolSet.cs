// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Graphics.DynamicPoolSet
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using System;
using System.Collections;

namespace Microsoft.Iris.Render.Graphics
{
    internal class DynamicPoolSet : RenderObject
    {
        private GraphicsDevice m_device;
        private ArrayList m_alPools;
        private SurfaceFormat m_nFormat;
        private int m_idxLastAlloc;

        internal DynamicPoolSet(GraphicsDevice device, SurfaceFormat nFormat)
        {
            this.m_device = device;
            this.m_alPools = new ArrayList();
            this.m_nFormat = nFormat;
            this.m_idxLastAlloc = 0;
        }

        protected override void Dispose(bool fInDispose)
        {
            try
            {
                if (fInDispose)
                {
                    foreach (DynamicPool alPool in this.m_alPools)
                        alPool.Dispose();
                }
                this.m_alPools = null;
                this.m_device = null;
            }
            finally
            {
                base.Dispose(fInDispose);
            }
        }

        internal SurfaceFormat Format => this.m_nFormat;

        internal ArrayList Pools => this.m_alPools;

        internal DynamicPool AllocateDynamicPool(
          Size sizeDynamicPoolPxl,
          int nMinHeightPxl,
          int nMaxHeightPxl)
        {
            SurfacePool poolSurface = new SurfacePool(this.m_device, sizeDynamicPoolPxl, this.m_nFormat);
            poolSurface.DestroyWhenEmpty = false;
            poolSurface.RemoteStub.SendSetPriority(-10);
            DynamicPool dynamicPool = new DynamicPool(poolSurface, nMinHeightPxl, nMaxHeightPxl);
            this.m_alPools.Add(dynamicPool);
            return dynamicPool;
        }

        internal Surface RequestSurface(
          ISurfaceContentOwner surfaceOwner,
          Size sizeRequestPxl,
          Size sizeContentPxl,
          Size sizeGutter,
          bool fRotated,
          PoolAllocMethod minAllocMethod,
          PoolAllocMethod maxAllocMethod)
        {
            Debug2.Validate(surfaceOwner != null, typeof(ArgumentNullException), "Must provide valid surface owner");
            Debug2.Validate(sizeContentPxl.Width > 0 && sizeContentPxl.Height > 0, typeof(ArgumentOutOfRangeException), "Must provide positive area for surface");
            Surface surface = null;
            for (int index1 = 0; index1 < this.m_alPools.Count; ++index1)
            {
                for (int index2 = (int)minAllocMethod; (PoolAllocMethod)index2 <= maxAllocMethod; ++index2)
                {
                    int index3 = (this.m_idxLastAlloc + index1) % this.m_alPools.Count;
                    surface = ((DynamicPool)this.m_alPools[index3]).CreateSurface(surfaceOwner, sizeRequestPxl, sizeContentPxl, sizeGutter, fRotated, (PoolAllocMethod)index2);
                    if (surface != null)
                    {
                        this.m_idxLastAlloc = index3;
                        break;
                    }
                }
                if (surface != null)
                    break;
            }
            return surface;
        }

        protected override void Invariant()
        {
        }
    }
}
