// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Graphics.DynamicRow
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using System;
using System.Collections;
using System.Diagnostics;

namespace Microsoft.Iris.Render.Graphics
{
    internal sealed class DynamicRow : IDynamicBlock, IComparer
    {
        private DynamicBucket m_bktOwner;
        private Size m_sizeRowPxl;
        private int m_nOffsetYPxl;
        private int m_nLargestFreePxl;
        private int m_nTotalFreePxl;
        private ArrayList m_alBlocks;
        private uint m_uLastAvgAge;

        internal DynamicRow(DynamicBucket bktOwner, int nHeightPxl, int nOffsetPxl)
        {
            this.m_bktOwner = bktOwner;
            this.m_sizeRowPxl = new Size(bktOwner.DynamicPool.SurfacePool.StorageSize.Width, nHeightPxl);
            this.m_nOffsetYPxl = nOffsetPxl;
            this.m_alBlocks = new ArrayList();
            this.m_nLargestFreePxl = this.m_sizeRowPxl.Width;
        }

        public void Dispose()
        {
            this.m_bktOwner.AssertOwningThread();
            this.m_bktOwner = null;
            foreach (DynamicBlock alBlock in this.m_alBlocks)
                alBlock.Dispose();
            this.m_alBlocks = null;
        }

        public int NearEdge => this.m_nOffsetYPxl;

        public int FarEdge => this.m_nOffsetYPxl + this.m_sizeRowPxl.Height;

        public int LargestFree => this.m_nLargestFreePxl;

        public int TotalFree => this.m_nTotalFreePxl;

        public DynamicPool DynamicPool => this.m_bktOwner.DynamicPool;

        public DynamicBucket Bucket => this.m_bktOwner;

        public ICollection Blocks => m_alBlocks;

        internal bool IsEmpty => this.m_alBlocks.Count == 0;

        internal Size StorageSize => this.m_sizeRowPxl;

        internal bool LastFullyEmpty => this.m_uLastAvgAge != uint.MaxValue;

        internal uint LastAvgAge => this.m_uLastAvgAge;

        internal Surface CreateSurface(
          ISurfaceContentOwner surfaceOwner,
          Size sizeRequestPxl,
          Size sizeContentPxl,
          Size sizeGutter,
          bool fRotated)
        {
            int idxInsert;
            int nOffsetNewPxl;
            DynamicPool.FindInsertionPoint(this.m_alBlocks, this.m_sizeRowPxl.Width, sizeRequestPxl.Width, out idxInsert, out nOffsetNewPxl);
            DynamicBlock dynamicBlock = new DynamicBlock(this, new Rectangle(new Point(nOffsetNewPxl, this.m_nOffsetYPxl), sizeRequestPxl), sizeContentPxl, sizeGutter, fRotated);
            Surface surface = dynamicBlock.Create(surfaceOwner);
            surface.RegisterUsage(DynamicPool.SurfacePool);
            this.m_alBlocks.Insert(idxInsert, dynamicBlock);
            this.RecomputeAvailable();
            return surface;
        }

        internal void UpdateLivenessAndAge()
        {
            ulong num = 0;
            int count = this.m_alBlocks.Count;
            for (int index = 0; index < count; ++index)
            {
                Surface surface = ((DynamicBlock)this.m_alBlocks[index]).Surface;
                if (surface.InUse)
                {
                    this.m_uLastAvgAge = uint.MaxValue;
                    return;
                }
                num += surface.LastUsage;
            }
            this.m_uLastAvgAge = (uint)(num / (ulong)count);
        }

        internal object SpeculateScavengeBlocks(
          int nRequestedWidth,
          out uint uAvgTimestampsNoCompact,
          out uint uAvgTimestampsWithCompact)
        {
            uAvgTimestampsNoCompact = uint.MaxValue;
            uAvgTimestampsWithCompact = uint.MaxValue;
            if (nRequestedWidth <= this.m_nLargestFreePxl)
            {
                uAvgTimestampsNoCompact = 0U;
                return null;
            }
            int count = this.m_alBlocks.Count;
            int length = 0;
            DynamicBlock[] dynamicBlockArray = new DynamicBlock[count];
            int num1 = 0;
            DynamicBlock dynamicBlock1 = null;
            uint num2 = uint.MaxValue;
            for (int index = 0; index < count; ++index)
            {
                DynamicBlock alBlock = (DynamicBlock)this.m_alBlocks[index];
                Surface surface = alBlock.Surface;
                if (!surface.InUse)
                {
                    dynamicBlockArray[length++] = alBlock;
                    num1 += alBlock.StorageSize.Width;
                    if (surface.LastUsage < num2)
                    {
                        int num3 = 0;
                        if (index > 0)
                            num3 = ((DynamicBlock)this.m_alBlocks[index - 1]).FarEdge;
                        int num4 = this.m_sizeRowPxl.Width;
                        if (index < count - 1)
                            num4 = ((DynamicBlock)this.m_alBlocks[index + 1]).NearEdge;
                        if (num4 - num3 >= nRequestedWidth)
                        {
                            dynamicBlock1 = alBlock;
                            num2 = surface.LastUsage;
                        }
                    }
                }
            }
            if (dynamicBlock1 != null)
            {
                uAvgTimestampsNoCompact = num2;
                return dynamicBlock1;
            }
            if (nRequestedWidth <= this.m_nTotalFreePxl)
            {
                uAvgTimestampsWithCompact = 0U;
                return null;
            }
            if (nRequestedWidth > num1 + this.m_nTotalFreePxl)
                return null;
            Array.Sort(dynamicBlockArray, 0, length, this);
            int nTotalFreePxl = this.m_nTotalFreePxl;
            int num5 = 0;
            ulong num6 = 0;
            while (num5 < length)
            {
                DynamicBlock dynamicBlock2 = dynamicBlockArray[num5++];
                nTotalFreePxl += dynamicBlock2.StorageSize.Width;
                num6 += dynamicBlock2.Surface.LastUsage;
                if (nTotalFreePxl >= nRequestedWidth)
                    break;
            }
            uAvgTimestampsWithCompact = (uint)(num6 / (ulong)num5);
            while (num5 < length)
                dynamicBlockArray[num5++] = null;
            return dynamicBlockArray;
        }

        internal bool FinishScavengeBlocks(
          ArrayList alReloadContent,
          int nRequestedWidth,
          object objResultOfPotentialCalculation)
        {
            if (objResultOfPotentialCalculation != null)
            {
                if (objResultOfPotentialCalculation is DynamicBlock blk1)
                {
                    this.CollectDeadBlockSurface(blk1, blk1.Surface);
                }
                else
                {
                    foreach (DynamicBlock blk2 in objResultOfPotentialCalculation as DynamicBlock[])
                    {
                        if (blk2 != null)
                            this.CollectDeadBlockSurface(blk2, blk2.Surface);
                        if (nRequestedWidth <= this.m_nLargestFreePxl)
                            break;
                    }
                }
            }
            if (nRequestedWidth > this.m_nLargestFreePxl)
                this.CompactBlocks(alReloadContent, false);
            return nRequestedWidth <= this.m_nLargestFreePxl;
        }

        int IComparer.Compare(object oL, object oR)
        {
            DynamicBlock dynamicBlock1 = oL as DynamicBlock;
            DynamicBlock dynamicBlock2 = oR as DynamicBlock;
            Surface surface1 = dynamicBlock1.Surface;
            Surface surface2 = dynamicBlock2.Surface;
            uint lastUsage1 = surface1.LastUsage;
            uint lastUsage2 = surface2.LastUsage;
            if (lastUsage1 < lastUsage2)
                return -1;
            return lastUsage1 > lastUsage2 ? 1 : 0;
        }

        internal void CollectDeadBlocks()
        {
            int index = 0;
            while (index < this.m_alBlocks.Count)
            {
                DynamicBlock alBlock = (DynamicBlock)this.m_alBlocks[index];
                Surface surface = alBlock.Surface;
                if (!surface.InUse)
                    this.CollectDeadBlockSurface(alBlock, surface);
                else
                    ++index;
            }
        }

        private void CollectDeadBlockSurface(DynamicBlock blk, Surface sur)
        {
            sur.UnregisterUsage(DynamicPool.SurfacePool);
            blk.Dispose();
        }

        internal void CompactBlocks(ArrayList alReloadContent, bool fCompactToRight)
        {
            if (this.m_alBlocks.Count <= 0)
                return;
            DynamicBlock dynamicBlock = null;
            try
            {
                if (fCompactToRight)
                {
                    Point ptNewOffsetPxl = new Point(this.m_sizeRowPxl.Width, this.m_nOffsetYPxl);
                    for (int index = this.m_alBlocks.Count - 1; index >= 0; --index)
                    {
                        DynamicBlock alBlock = (DynamicBlock)this.m_alBlocks[index];
                        ptNewOffsetPxl.X -= alBlock.StorageSize.Width;
                        alBlock.NotifyPositionChanged(alReloadContent, ptNewOffsetPxl);
                        dynamicBlock = alBlock;
                    }
                }
                else
                {
                    Point ptNewOffsetPxl = new Point(0, this.m_nOffsetYPxl);
                    for (int index = 0; index < this.m_alBlocks.Count; ++index)
                    {
                        DynamicBlock alBlock = (DynamicBlock)this.m_alBlocks[index];
                        alBlock.NotifyPositionChanged(alReloadContent, ptNewOffsetPxl);
                        ptNewOffsetPxl.X = alBlock.FarEdge;
                        dynamicBlock = alBlock;
                    }
                }
            }
            finally
            {
                if (fCompactToRight)
                {
                    this.m_nLargestFreePxl = this.m_sizeRowPxl.Width;
                    if (dynamicBlock != null)
                        this.m_nLargestFreePxl = dynamicBlock.NearEdge;
                }
                else
                {
                    this.m_nLargestFreePxl = this.m_sizeRowPxl.Width;
                    if (dynamicBlock != null)
                        this.m_nLargestFreePxl -= dynamicBlock.FarEdge;
                }
            }
        }

        internal static bool MoveBlocksToEnd(
          ArrayList alReloadContent,
          DynamicRow rowDest,
          DynamicRow rowSrc,
          bool fCompactToRight)
        {
            bool flag = true;
            while (rowSrc.m_alBlocks.Count > 0)
            {
                int index = fCompactToRight ? rowSrc.m_alBlocks.Count - 1 : 0;
                DynamicBlock alBlock = (DynamicBlock)rowSrc.m_alBlocks[index];
                if (!MoveBlockToEnd(alReloadContent, alBlock, rowDest, rowSrc, fCompactToRight))
                {
                    flag = false;
                    break;
                }
            }
            return flag;
        }

        private static bool MoveBlockToEnd(
          ArrayList alReloadContent,
          DynamicBlock blkMove,
          DynamicRow rowDest,
          DynamicRow rowSrc,
          bool fCompactToRight)
        {
            int num1 = blkMove.FarEdge - blkMove.NearEdge;
            int count = rowDest.m_alBlocks.Count;
            int x = 0;
            int num2;
            if (fCompactToRight)
            {
                if (count > 0)
                    x = ((DynamicBlock)rowDest.m_alBlocks[0]).NearEdge;
                num2 = x;
            }
            else
            {
                if (count > 0)
                    x = ((DynamicBlock)rowDest.m_alBlocks[count - 1]).FarEdge;
                num2 = rowDest.m_sizeRowPxl.Width - x;
            }
            if (num1 > num2)
                return false;
            try
            {
                if (fCompactToRight)
                    rowDest.m_alBlocks.Insert(0, blkMove);
                else
                    rowDest.m_alBlocks.Add(blkMove);
                int index = rowSrc.m_alBlocks.IndexOf(blkMove);
                rowSrc.m_alBlocks.RemoveAt(index);
                if (fCompactToRight)
                    x -= num1;
                Point ptNewOffsetPxl = new Point(x, rowDest.m_nOffsetYPxl);
                blkMove.NotifyRowChanged(alReloadContent, rowDest, rowSrc, ptNewOffsetPxl);
            }
            finally
            {
                rowDest.RecomputeAvailable();
                rowSrc.RecomputeAvailable();
            }
            return true;
        }

        private void RecomputeAvailable()
        {
            this.m_uLastAvgAge = this.IsEmpty ? 0U : uint.MaxValue;
            DynamicPool.RecomputeAvailable(this.m_alBlocks, this.m_sizeRowPxl.Width, out this.m_nLargestFreePxl, out this.m_nTotalFreePxl);
        }

        [Conditional("DEBUG")]
        private void Validate(bool fTightPacking)
        {
        }

        internal int NotifyPositionChanged(ArrayList alReloadContent, int nNewOffsetYPxl)
        {
            if (this.m_nOffsetYPxl != nNewOffsetYPxl)
            {
                this.m_nOffsetYPxl = nNewOffsetYPxl;
                for (int index = 0; index < this.m_alBlocks.Count; ++index)
                {
                    DynamicBlock alBlock = (DynamicBlock)this.m_alBlocks[index];
                    alBlock.NotifyPositionChanged(alReloadContent, new Point(alBlock.NearEdge, this.m_nOffsetYPxl));
                }
            }
            return this.m_sizeRowPxl.Height;
        }

        internal void NotifyBlockDestroyed(DynamicBlock blkRemove)
        {
            if (this.m_bktOwner == null)
                return;
            this.m_alBlocks.RemoveAt(this.m_alBlocks.IndexOf(blkRemove));
            this.RecomputeAvailable();
            if (this.m_nLargestFreePxl != this.m_sizeRowPxl.Width)
                return;
            this.m_bktOwner.NotifyEmpty(this);
        }

        public int ID => 0;
    }
}
