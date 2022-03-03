// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Graphics.DynamicPool
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Internal;
using System;
using System.Collections;
using System.Diagnostics;

namespace Microsoft.Iris.Render.Graphics
{
    internal sealed class DynamicPool : IComparer
    {
        private RenderSession m_session;
        private GraphicsDevice m_device;
        private SurfacePool m_poolSurface;
        private ArrayList m_alBuckets;
        private ArrayList m_alRows;
        private Size m_sizeTotalPxl;
        private int m_nLargestFreePxl;
        private int m_nTotalFreePxl;
        private int m_nMinimumHeightPxl;
        private int m_nMaximumHeightPxl;
        private uint m_nCompact;
        private bool m_fCollectDeadSurfacesDuringDebugCompact;

        internal DynamicPool(SurfacePool poolSurface, int nMinimumHeightPxl, int nMaximumHeightPxl)
        {
            this.m_device = poolSurface.Device;
            this.m_session = this.m_device.Session;
            this.m_poolSurface = poolSurface;
            this.m_alBuckets = new ArrayList();
            this.m_alRows = new ArrayList();
            this.m_sizeTotalPxl = poolSurface.StorageSize;
            this.m_nLargestFreePxl = this.m_sizeTotalPxl.Height;
            this.m_nMinimumHeightPxl = nMinimumHeightPxl;
            this.m_nMaximumHeightPxl = nMaximumHeightPxl;
        }

        public void Dispose()
        {
            foreach (DynamicRow alRow in this.m_alRows)
                alRow.Dispose();
            this.m_alBuckets = null;
            this.m_alRows = null;
            this.m_poolSurface.Dispose();
            this.m_poolSurface = null;
        }

        internal SurfacePool SurfacePool => this.m_poolSurface;

        public Size TotalSize => this.m_sizeTotalPxl;

        public SurfaceFormat Format => this.m_poolSurface.Format;

        internal void AssertOwningThread() => this.m_session.AssertOwningThread();

        public uint DebugCompactInterval
        {
            get => 0;
            set
            {
            }
        }

        public bool CollectDeadSurfacesDuringDebugCompact
        {
            get => this.m_fCollectDeadSurfacesDuringDebugCompact;
            set => this.m_fCollectDeadSurfacesDuringDebugCompact = value;
        }

        public Surface CreateSurface(
          ISurfaceContentOwner surfaceOwner,
          Size sizeRequestPxl,
          Size sizeContentPxl,
          Size sizeGutter,
          bool fRotated,
          PoolAllocMethod allocMethod)
        {
            int nHeightPxl = Math2.FindPowerOf2(sizeRequestPxl.Height, this.m_nMinimumHeightPxl);
            if (nHeightPxl > this.m_nMaximumHeightPxl)
            {
                if (sizeRequestPxl.Height <= this.m_nMaximumHeightPxl)
                    nHeightPxl = this.m_nMaximumHeightPxl;
            }
            else if (sizeRequestPxl.Width <= this.m_sizeTotalPxl.Width)
            {
                int height = this.m_sizeTotalPxl.Height;
            }
            DynamicBucket dynamicBucket = null;
            for (int index = 0; index < this.m_alBuckets.Count; ++index)
            {
                DynamicBucket alBucket = (DynamicBucket)this.m_alBuckets[index];
                if (alBucket.RowHeight == nHeightPxl)
                {
                    dynamicBucket = alBucket;
                    break;
                }
            }
            if (dynamicBucket == null)
            {
                dynamicBucket = new DynamicBucket(this, nHeightPxl);
                this.m_alBuckets.Add(dynamicBucket);
            }
            return dynamicBucket.CreateSurface(surfaceOwner, sizeRequestPxl, sizeContentPxl, sizeGutter, fRotated, allocMethod);
        }

        public static void FindInsertionPoint(
          ArrayList alItems,
          int nTotalPxl,
          int nNeededPxl,
          out int idxInsert,
          out int nOffsetNewPxl)
        {
            idxInsert = 0;
            nOffsetNewPxl = 0;
            int num1 = int.MaxValue;
            if (alItems.Count <= 0)
                return;
            idxInsert = -1;
            IDynamicBlock dynamicBlock1 = null;
            int index = 0;
            while (index <= alItems.Count)
            {
                IDynamicBlock dynamicBlock2 = null;
                int num2;
                if (index < alItems.Count)
                {
                    dynamicBlock2 = (IDynamicBlock)alItems[index];
                    num2 = dynamicBlock2.NearEdge;
                }
                else
                    num2 = nTotalPxl;
                if (dynamicBlock1 != null)
                    num2 -= dynamicBlock1.FarEdge;
                int num3 = num2 - nNeededPxl;
                if (num3 >= 0 && num3 < num1)
                {
                    nOffsetNewPxl = dynamicBlock1 != null ? dynamicBlock1.FarEdge : 0;
                    num1 = num3;
                    idxInsert = index;
                }
                ++index;
                dynamicBlock1 = dynamicBlock2;
            }
        }

        public static void RecomputeAvailable(
          ArrayList alItems,
          int nTotalPxl,
          out int nOutLargestFreePxl,
          out int nOutTotalFreePxl)
        {
            int num1 = nTotalPxl;
            int num2 = nTotalPxl;
            if (alItems.Count >= 0)
            {
                num1 = 0;
                num2 = 0;
                IDynamicBlock dynamicBlock1 = null;
                int index = 0;
                while (index <= alItems.Count)
                {
                    IDynamicBlock dynamicBlock2;
                    int num3;
                    if (index < alItems.Count)
                    {
                        dynamicBlock2 = (IDynamicBlock)alItems[index];
                        num3 = dynamicBlock2.NearEdge;
                    }
                    else
                    {
                        dynamicBlock2 = null;
                        num3 = nTotalPxl;
                    }
                    if (dynamicBlock1 != null)
                        num3 -= dynamicBlock1.FarEdge;
                    if (num1 < num3)
                        num1 = num3;
                    num2 += num3;
                    ++index;
                    dynamicBlock1 = dynamicBlock2;
                }
            }
            nOutLargestFreePxl = num1;
            nOutTotalFreePxl = num2;
        }

        [Conditional("DEBUG")]
        public static void ValidateItems(
          ArrayList alItems,
          int nTotalPxl,
          int nCachedLargestAvailable,
          bool fTightPacking)
        {
            if (alItems.Count <= 0)
                return;
            int num1 = 0;
            IDynamicBlock dynamicBlock1 = null;
            int index = 0;
            while (index <= alItems.Count)
            {
                IDynamicBlock dynamicBlock2;
                int num2;
                if (index < alItems.Count)
                {
                    dynamicBlock2 = (IDynamicBlock)alItems[index];
                    num2 = dynamicBlock2.NearEdge;
                }
                else
                {
                    dynamicBlock2 = null;
                    num2 = nTotalPxl;
                }
                if (dynamicBlock1 != null)
                    num2 -= dynamicBlock1.FarEdge;
                if (num1 < num2)
                    num1 = num2;
                if (fTightPacking && index > 0)
                {
                    int count = alItems.Count;
                }
                ++index;
                dynamicBlock1 = dynamicBlock2;
            }
        }

        public DynamicRow CreateRow(
          ArrayList alReloadContent,
          DynamicBucket bktOwner,
          ISurfaceContentOwner surfaceOwner,
          Size sizeRequestPxl,
          PoolAllocMethod allocMethod)
        {
            int rowHeight = bktOwner.RowHeight;
            switch (allocMethod)
            {
                case PoolAllocMethod.CompactDead:
                    this.CollectDeadSurfaces();
                    break;
                case PoolAllocMethod.CompactAll:
                    this.FullCompact(alReloadContent);
                    break;
            }
            DynamicRow dynamicRow = null;
            if (rowHeight <= this.m_nLargestFreePxl)
            {
                int idxInsert;
                int nOffsetNewPxl;
                FindInsertionPoint(this.m_alRows, this.m_sizeTotalPxl.Height, rowHeight, out idxInsert, out nOffsetNewPxl);
                dynamicRow = new DynamicRow(bktOwner, rowHeight, nOffsetNewPxl);
                this.m_alRows.Insert(idxInsert, dynamicRow);
                RecomputeAvailable(this.m_alRows, this.m_sizeTotalPxl.Height, out this.m_nLargestFreePxl, out this.m_nTotalFreePxl);
            }
            return dynamicRow;
        }

        internal object SpeculateScavengeRows(
          int nRequestedHeight,
          DynamicBucket bktIgnore,
          out uint uAvgTimestampsNoCompact,
          out uint uAvgTimestampsWithCompact)
        {
            uAvgTimestampsNoCompact = uint.MaxValue;
            uAvgTimestampsWithCompact = uint.MaxValue;
            if (nRequestedHeight <= this.m_nLargestFreePxl)
            {
                uAvgTimestampsNoCompact = 0U;
                return null;
            }
            int count = this.m_alRows.Count;
            int length = 0;
            DynamicRow[] dynamicRowArray = new DynamicRow[count];
            int num1 = 0;
            DynamicRow dynamicRow1 = null;
            uint num2 = uint.MaxValue;
            for (int index = 0; index < count; ++index)
            {
                DynamicRow alRow = (DynamicRow)this.m_alRows[index];
                if (alRow.Bucket != bktIgnore)
                {
                    alRow.UpdateLivenessAndAge();
                    if (alRow.LastFullyEmpty)
                    {
                        dynamicRowArray[length++] = alRow;
                        num1 += alRow.StorageSize.Height;
                        if (alRow.LastAvgAge < num2)
                        {
                            int num3 = 0;
                            if (index > 0)
                                num3 = ((DynamicRow)this.m_alRows[index - 1]).FarEdge;
                            int num4 = this.m_sizeTotalPxl.Height;
                            if (index < count - 1)
                                num4 = ((DynamicRow)this.m_alRows[index + 1]).NearEdge;
                            if (num4 - num3 >= nRequestedHeight)
                            {
                                dynamicRow1 = alRow;
                                num2 = alRow.LastAvgAge;
                            }
                        }
                    }
                }
            }
            if (dynamicRow1 != null)
            {
                uAvgTimestampsNoCompact = num2;
                return dynamicRow1;
            }
            if (nRequestedHeight <= this.m_nTotalFreePxl)
            {
                uAvgTimestampsWithCompact = 0U;
                return null;
            }
            if (nRequestedHeight > num1 + this.m_nTotalFreePxl)
                return null;
            Array.Sort(dynamicRowArray, 0, length, this);
            int nTotalFreePxl = this.m_nTotalFreePxl;
            int num5 = 0;
            ulong num6 = 0;
            while (num5 < length)
            {
                DynamicRow dynamicRow2 = dynamicRowArray[num5++];
                nTotalFreePxl += dynamicRow2.StorageSize.Height;
                num6 += dynamicRow2.LastAvgAge;
                if (nTotalFreePxl >= nRequestedHeight)
                    break;
            }
            uAvgTimestampsWithCompact = (uint)(num6 / (ulong)num5);
            while (num5 < length)
                dynamicRowArray[num5++] = null;
            return dynamicRowArray;
        }

        internal bool FinishScavengeRows(
          ArrayList alReloadContent,
          int nRequestedHeight,
          object objResultOfSpeculation)
        {
            if (objResultOfSpeculation != null)
            {
                if (objResultOfSpeculation is DynamicRow dynamicRow1)
                {
                    dynamicRow1.CollectDeadBlocks();
                    this.RemoveEmptyRows();
                }
                else
                {
                    foreach (DynamicRow dynamicRow2 in objResultOfSpeculation as DynamicRow[])
                    {
                        if (dynamicRow2 != null)
                        {
                            dynamicRow2.CollectDeadBlocks();
                            this.RemoveEmptyRows();
                            if (nRequestedHeight <= this.m_nLargestFreePxl)
                                break;
                        }
                    }
                }
            }
            if (nRequestedHeight > this.m_nLargestFreePxl)
                this.CompactRows(alReloadContent, false);
            return nRequestedHeight <= this.m_nLargestFreePxl;
        }

        int IComparer.Compare(object oL, object oR)
        {
            DynamicRow dynamicRow1 = oL as DynamicRow;
            DynamicRow dynamicRow2 = oR as DynamicRow;
            uint lastAvgAge1 = dynamicRow1.LastAvgAge;
            uint lastAvgAge2 = dynamicRow2.LastAvgAge;
            if (lastAvgAge1 < lastAvgAge2)
                return -1;
            return lastAvgAge1 > lastAvgAge2 ? 1 : 0;
        }

        public void NotifyEmpty(DynamicRow rowEmpty)
        {
            this.m_alRows.Remove(rowEmpty);
            RecomputeAvailable(this.m_alRows, this.m_sizeTotalPxl.Height, out this.m_nLargestFreePxl, out this.m_nTotalFreePxl);
        }

        public void NotifyEmpty(DynamicBucket bktEmpty)
        {
        }

        public void CollectDeadSurfaces()
        {
            for (int index = 0; index < this.m_alBuckets.Count; ++index)
                ((DynamicBucket)this.m_alBuckets[index]).CollectDeadBlocks();
        }

        internal IList ExtractAllSurfaces()
        {
            if (this.m_poolSurface == null)
                return null;
            IList allSurfaces = this.m_poolSurface.AllSurfaces;
            if (allSurfaces == null)
                return null;
            foreach (Surface surCur in allSurfaces)
                SurfacePool.RemapSurface(surCur, new SurfacePool(this.m_device, surCur.ContentSize, SurfaceFormat.ARGB32)
                {
                    DestroyWhenEmpty = true
                });
            this.m_poolSurface = null;
            this.m_alBuckets = new ArrayList();
            this.m_alRows = new ArrayList();
            return allSurfaces;
        }

        private void RemoveEmptyRows()
        {
            for (int index = 0; index < this.m_alBuckets.Count; ++index)
                ((DynamicBucket)this.m_alBuckets[index]).RemoveEmptyRows();
        }

        private int TestCollect()
        {
            if (this.CollectDeadSurfacesDuringDebugCompact)
                this.CollectDeadSurfaces();
            ArrayList alReloadContent = new ArrayList();
            this.FullCompact(alReloadContent);
            this.ProcessDeferredContent(alReloadContent);
            return alReloadContent.Count;
        }

        private void DebugCompactTimeout(object sender, EventArgs args) => this.TestCollect();

        internal void ProcessDeferredContent(ArrayList alReloadContent)
        {
            if (alReloadContent.Count <= 0)
                return;
            foreach (Surface surface in alReloadContent)
                surface.RestoreContent();
        }

        private void FullCompact(ArrayList alReloadContent)
        {
            ++this.m_nCompact;
            bool fCompactToRight = false;
            bool fCompactDownward = false;
            for (int index = 0; index < this.m_alBuckets.Count; ++index)
                ((DynamicBucket)this.m_alBuckets[index]).CompactBlocks(alReloadContent, fCompactToRight);
            this.CompactRows(alReloadContent, fCompactDownward);
            this.m_nLargestFreePxl = this.m_sizeTotalPxl.Height;
            int count = this.m_alRows.Count;
            if (count <= 0)
                return;
            if (fCompactDownward)
                this.m_nLargestFreePxl = ((DynamicRow)this.m_alRows[0]).NearEdge;
            else
                this.m_nLargestFreePxl = this.m_sizeTotalPxl.Height - ((DynamicRow)this.m_alRows[count - 1]).FarEdge;
        }

        private void CompactRows(ArrayList alReloadContent, bool fCompactDownward)
        {
            if (fCompactDownward)
            {
                int height = this.m_sizeTotalPxl.Height;
                for (int index = this.m_alRows.Count - 1; index >= 0; --index)
                {
                    DynamicRow alRow = (DynamicRow)this.m_alRows[index];
                    height -= alRow.StorageSize.Height;
                    alRow.NotifyPositionChanged(alReloadContent, height);
                }
            }
            else
            {
                int nNewOffsetYPxl = 0;
                for (int index = 0; index < this.m_alRows.Count; ++index)
                {
                    DynamicRow alRow = (DynamicRow)this.m_alRows[index];
                    alRow.NotifyPositionChanged(alReloadContent, nNewOffsetYPxl);
                    nNewOffsetYPxl += alRow.StorageSize.Height;
                }
            }
        }

        internal void Dump()
        {
            foreach (DynamicBucket alBucket in this.m_alBuckets)
                ;
            foreach (DynamicRow alRow in this.m_alRows)
            {
                foreach (DynamicBlock block in alRow.Blocks)
                    ;
            }
        }
    }
}
