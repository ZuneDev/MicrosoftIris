// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Graphics.DynamicBucket
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using System.Collections;

namespace Microsoft.Iris.Render.Graphics
{
    internal sealed class DynamicBucket
    {
        private DynamicPool m_poolOwner;
        private int m_nHeightPxl;
        private ArrayList m_alRows;
        private bool m_fCompacting;

        public DynamicBucket(DynamicPool poolOwner, int nHeightPxl)
        {
            this.m_poolOwner = poolOwner;
            this.m_nHeightPxl = nHeightPxl;
            this.m_alRows = new ArrayList();
        }

        public int RowHeight => this.m_nHeightPxl;

        internal DynamicPool DynamicPool => this.m_poolOwner;

        public Surface CreateSurface(
          ISurfaceContentOwner surfaceOwner,
          Size sizeRequestPxl,
          Size sizeContentPxl,
          Size sizeGutter,
          bool fRotated,
          PoolAllocMethod allocMethod)
        {
            ArrayList alReloadContent = null;
            DynamicRow dynamicRow = null;
            switch (allocMethod)
            {
                case PoolAllocMethod.Available:
                    dynamicRow = this.FindTrivialFreeSpaceRow(sizeRequestPxl.Width);
                    break;
                case PoolAllocMethod.New:
                case PoolAllocMethod.CompactDead:
                case PoolAllocMethod.CompactAll:
                    alReloadContent = new ArrayList();
                    dynamicRow = this.m_poolOwner.CreateRow(alReloadContent, this, surfaceOwner, sizeRequestPxl, allocMethod);
                    if (dynamicRow != null)
                    {
                        this.m_alRows.Add(dynamicRow);
                        break;
                    }
                    break;
                case PoolAllocMethod.ScavengeDead:
                    alReloadContent = new ArrayList();
                    dynamicRow = this.ScavengeForAvailableSpace(alReloadContent, sizeRequestPxl);
                    break;
            }
            if (alReloadContent != null)
                this.m_poolOwner.ProcessDeferredContent(alReloadContent);
            return dynamicRow?.CreateSurface(surfaceOwner, sizeRequestPxl, sizeContentPxl, sizeGutter, fRotated);
        }

        public void NotifyEmpty(DynamicRow rowEmpty)
        {
            if (this.m_fCompacting)
                return;
            this.NotifyEmptyWorker(this.m_alRows.IndexOf(rowEmpty));
        }

        private void NotifyEmptyWorker(int idxRow)
        {
            DynamicRow alRow = (DynamicRow)this.m_alRows[idxRow];
            this.m_alRows.RemoveAt(idxRow);
            this.m_poolOwner.NotifyEmpty(alRow);
            if (this.m_alRows.Count != 0)
                return;
            this.m_poolOwner.NotifyEmpty(this);
        }

        private DynamicRow FindTrivialFreeSpaceRow(int nRequestedWidth)
        {
            for (int index = 0; index < this.m_alRows.Count; ++index)
            {
                DynamicRow alRow = (DynamicRow)this.m_alRows[index];
                if (nRequestedWidth <= alRow.LargestFree)
                    return alRow;
            }
            return null;
        }

        private DynamicRow ScavengeForAvailableSpace(
          ArrayList alReloadContent,
          Size sizeRequested)
        {
            uint uAvgTimestampsNoCompact;
            uint uAvgTimestampsWithCompact;
            object objResultOfSpeculation = this.m_poolOwner.SpeculateScavengeRows(this.RowHeight, this, out uAvgTimestampsNoCompact, out uAvgTimestampsWithCompact);
            DynamicRow dynamicRow = null;
            if (uAvgTimestampsNoCompact == uint.MaxValue)
                dynamicRow = this.ScavengeLocalRows(alReloadContent, sizeRequested.Width);
            if (uAvgTimestampsNoCompact != uint.MaxValue || uAvgTimestampsWithCompact != uint.MaxValue)
            {
                int nRequestedHeight = 0;
                if (dynamicRow == null)
                    nRequestedHeight = this.RowHeight;
                this.m_poolOwner.FinishScavengeRows(alReloadContent, nRequestedHeight, objResultOfSpeculation);
            }
            return dynamicRow;
        }

        private DynamicRow ScavengeLocalRows(ArrayList alReloadContent, int nRequestedWidth)
        {
            DynamicRow dynamicRow1 = null;
            object obj1 = null;
            uint num1 = uint.MaxValue;
            DynamicRow dynamicRow2 = null;
            object obj2 = null;
            uint num2 = uint.MaxValue;
            for (int index = 0; index < this.m_alRows.Count; ++index)
            {
                DynamicRow alRow = (DynamicRow)this.m_alRows[index];
                uint uAvgTimestampsNoCompact;
                uint uAvgTimestampsWithCompact;
                object obj3 = alRow.SpeculateScavengeBlocks(nRequestedWidth, out uAvgTimestampsNoCompact, out uAvgTimestampsWithCompact);
                if (num1 > uAvgTimestampsNoCompact)
                {
                    num1 = uAvgTimestampsNoCompact;
                    obj1 = obj3;
                    dynamicRow1 = alRow;
                }
                if (num2 > uAvgTimestampsWithCompact)
                {
                    num2 = uAvgTimestampsWithCompact;
                    obj2 = obj3;
                    dynamicRow2 = alRow;
                }
            }
            DynamicRow dynamicRow3 = dynamicRow1;
            object objResultOfPotentialCalculation = obj1;
            if (dynamicRow3 == null)
            {
                dynamicRow3 = dynamicRow2;
                objResultOfPotentialCalculation = obj2;
            }
            if (dynamicRow3 != null && !dynamicRow3.FinishScavengeBlocks(alReloadContent, nRequestedWidth, objResultOfPotentialCalculation))
                dynamicRow3 = null;
            if (dynamicRow3 != null && !this.m_alRows.Contains(dynamicRow3))
                dynamicRow3 = null;
            return dynamicRow3;
        }

        internal void CollectDeadBlocks()
        {
            if (this.m_alRows.Count <= 0)
                return;
            this.m_fCompacting = true;
            try
            {
                for (int index = 0; index < this.m_alRows.Count; ++index)
                    ((DynamicRow)this.m_alRows[index]).CollectDeadBlocks();
                this.RemoveEmptyRows();
            }
            finally
            {
                this.m_fCompacting = false;
            }
        }

        internal void CompactBlocks(ArrayList alReloadContent, bool fCompactToRight)
        {
            if (this.m_alRows.Count <= 0)
                return;
            this.m_fCompacting = true;
            try
            {
                int index = 0;
                for (; index < this.m_alRows.Count; ++index)
                {
                    DynamicRow alRow1 = (DynamicRow)this.m_alRows[index];
                    alRow1.CompactBlocks(alReloadContent, fCompactToRight);
                    int num = index + 1;
                    while (num < this.m_alRows.Count)
                    {
                        DynamicRow alRow2 = (DynamicRow)this.m_alRows[num++];
                        if (!DynamicRow.MoveBlocksToEnd(alReloadContent, alRow1, alRow2, fCompactToRight))
                            break;
                    }
                }
                this.RemoveEmptyRows();
            }
            finally
            {
                this.m_fCompacting = false;
            }
        }

        internal void RemoveEmptyRows()
        {
            int num = 0;
            while (num < this.m_alRows.Count)
            {
                if (((DynamicRow)this.m_alRows[num]).IsEmpty)
                    this.NotifyEmptyWorker(num);
                else
                    ++num;
            }
        }

        public int ID => 0;

        internal void AssertOwningThread() => this.m_poolOwner.AssertOwningThread();
    }
}
