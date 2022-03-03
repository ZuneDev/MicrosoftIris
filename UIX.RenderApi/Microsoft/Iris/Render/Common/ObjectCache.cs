// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Common.ObjectCache
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using System;

namespace Microsoft.Iris.Render.Common
{
    internal class ObjectCache : RenderObject
    {
        private const int k_nReductionThreshold = 3;
        private Vector<CachedRenderObject> m_listObjects;
        private int m_nMaxCacheSize;
        private int m_nMinCacheSize;
        private int m_nStepSize;
        private int m_cCacheMisses;
        private int m_cCacheHits;
        private bool m_fWasCacheFull;
        private int m_cReductionRequests;

        internal ObjectCache(int nMinSize, int nMaxSize, int nStepSize)
        {
            this.m_listObjects = new Vector<CachedRenderObject>(nMinSize);
            this.m_nMinCacheSize = nMinSize;
            this.m_nMaxCacheSize = nMaxSize;
            this.m_nStepSize = nStepSize;
        }

        protected override void Dispose(bool fInDispose)
        {
            try
            {
                if (fInDispose)
                {
                    foreach (CachedRenderObject listObject in this.m_listObjects)
                    {
                        listObject.Cache = null;
                        listObject.UnregisterUsage(this);
                    }
                }
                this.m_listObjects = null;
            }
            finally
            {
                base.Dispose(fInDispose);
            }
        }

        internal int Size => this.m_listObjects.Count;

        internal bool Add(CachedRenderObject obj)
        {
            if (this.m_listObjects.Count < this.m_listObjects.Capacity)
            {
                obj.RegisterUsage(this);
                this.m_listObjects.Add(obj);
                return true;
            }
            this.m_fWasCacheFull = true;
            return false;
        }

        internal CachedRenderObject Remove(object objUser)
        {
            if (this.m_listObjects.Count > 0)
            {
                ++this.m_cCacheHits;
                int index = this.m_listObjects.Count - 1;
                CachedRenderObject listObject = this.m_listObjects[index];
                this.m_listObjects.RemoveAt(index);
                listObject.RegisterUsage(objUser);
                listObject.UnregisterUsage(this);
                return listObject;
            }
            ++this.m_cCacheMisses;
            return null;
        }

        internal void RemoveAll()
        {
            foreach (CachedRenderObject listObject in this.m_listObjects)
            {
                listObject.Cache = null;
                listObject.UnregisterUsage(this);
            }
            this.m_listObjects.Clear();
        }

        internal void Update()
        {
            if (this.ShouldGrowCache())
                this.m_listObjects.Capacity = Math.Min(this.m_listObjects.Capacity + this.m_nStepSize, this.m_nMaxCacheSize);
            else if (this.ShouldShrinkCache())
            {
                if (this.m_cReductionRequests > 3)
                {
                    int num = Math.Max(this.m_listObjects.Capacity - this.m_nStepSize, this.m_nMinCacheSize);
                    if (this.m_listObjects.Count - num > 0)
                    {
                        for (int index = this.m_listObjects.Count - 1; index >= num; --index)
                        {
                            CachedRenderObject listObject = this.m_listObjects[index];
                            listObject.Cache = null;
                            listObject.UnregisterUsage(this);
                            this.m_listObjects.RemoveAt(index);
                        }
                    }
                    this.m_listObjects.Capacity = num;
                    this.m_cReductionRequests = 0;
                }
                else
                    ++this.m_cReductionRequests;
            }
            else
                this.m_cReductionRequests = 0;
            this.m_cCacheMisses = 0;
            this.m_cCacheHits = 0;
            this.m_fWasCacheFull = false;
        }

        private bool ShouldGrowCache() => this.m_cCacheMisses > 0 && this.m_fWasCacheFull && this.m_listObjects.Capacity < this.m_nMaxCacheSize;

        private bool ShouldShrinkCache() => this.m_cCacheMisses + this.m_cCacheHits == 0 && this.m_listObjects.Capacity > this.m_nMinCacheSize;
    }
}
