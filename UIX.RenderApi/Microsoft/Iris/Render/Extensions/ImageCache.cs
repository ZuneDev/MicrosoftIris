// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Extensions.ImageCache
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using System;
using System.Collections;
using System.Diagnostics;

namespace Microsoft.Iris.Render.Extensions
{
    public class ImageCache : IDisposable
    {
        private static readonly ImageCache.ObjectTest s_testCulling = new ImageCache.ObjectTest(CullingTest);
        private static readonly ImageCache.ObjectTest s_testDisposeAll = new ImageCache.ObjectTest(DisposeAllTest);
        private static readonly ImageCache.ObjectTest s_testObject = new ImageCache.ObjectTest(SpecificObjectTest);
        private static readonly ImageCache.ObjectTest s_testDropImages = new ImageCache.ObjectTest(DropImageTest);
        private Map<object, object> m_objectsTable;
        private Vector<ImageCache.ImageCacheEntry> m_lruList;
        private bool m_fObjectDisposed;
        private string m_stName;
        private bool m_fCleanupPending;
        private bool m_fShuttingDown;
        private int m_numItemsToKeep;
        private TimeSpan m_tsCullingKeepTime;
        private bool m_fUseCutoffTime;
        private DateTime m_dtCullingCutoff;
        private int m_numItemsInUse;
        private int m_numItemsInCache;

        public ImageCache(IRenderSession session, string name)
        {
            this.m_stName = name;
            this.m_numItemsToKeep = 100;
            this.m_tsCullingKeepTime = new TimeSpan(0, 1, 0);
        }

        ~ImageCache()
        {
            if (this.m_fObjectDisposed)
                return;
            this.Dispose(false);
        }

        public void Dispose()
        {
            if (this.m_fObjectDisposed)
                return;
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool fInDispose)
        {
            if (!fInDispose)
                return;
            this.OnDispose();
        }

        protected virtual void OnDispose()
        {
            this.DisposeAllObjects();
            this.m_fObjectDisposed = true;
        }

        public void DisposeAllObjects()
        {
            this.m_numItemsInCache = 0;
            this.RemoveObjects(s_testDisposeAll, this);
        }

        public void PrepareToShutdown()
        {
            this.m_numItemsInCache = 0;
            this.RemoveObjects(s_testDropImages, this);
            this.m_fCleanupPending = false;
            this.m_fShuttingDown = true;
        }

        public bool IsEmpty => this.m_objectsTable.Count == 0;

        public bool CleanupPending => this.m_fCleanupPending;

        protected void ClearCleanupPending() => this.m_fCleanupPending = false;

        public int NumItemsToKeep
        {
            get => this.m_numItemsToKeep;
            set => this.m_numItemsToKeep = value;
        }

        public TimeSpan ItemRetainTime
        {
            get => this.m_tsCullingKeepTime;
            set => this.m_tsCullingKeepTime = value;
        }

        public void Add(ImageCacheKey key, ImageCacheItem item)
        {
            Debug2.Validate(!this.m_fShuttingDown, typeof(InvalidOperationException), "Should not Add items to cache after starting shutdown sequence");
            Debug2.Validate(item.ImageCacheOwner == null, typeof(InvalidOperationException), "The given item is already part of a cache! double-add detected.");
            if (this.m_objectsTable == null)
                this.SetupObjectTable();
            this.m_objectsTable[key] = item;
            this.m_lruList.Insert(0, new ImageCache.ImageCacheEntry(key, item));
            item.ImageCacheOwner = this;
            this.ScheduleScavenge();
        }

        public void RemoveData(ImageCacheKey key) => ((ImageCacheItem)this.Find(key))?.RemoveData();

        public void RemoveItem(ImageCacheItem item) => this.RemoveObjects(s_testObject, item);

        public void RemoveObjects(ImageCache.ObjectTest test, object paramObject)
        {
            if (this.m_objectsTable == null)
                return;
            ArrayList arrayList = null;
            foreach (ImageCache.ImageCacheEntry lru in this.m_lruList)
            {
                ImageCacheItem imageCacheItem = lru.item;
                if (test(imageCacheItem, paramObject))
                {
                    if (arrayList == null)
                        arrayList = new ArrayList();
                    arrayList.Add(lru);
                }
            }
            if (arrayList == null)
                return;
            foreach (ImageCache.ImageCacheEntry imageCacheEntry in arrayList)
            {
                this.m_lruList.Remove(imageCacheEntry);
                this.m_objectsTable.Remove(imageCacheEntry.key);
                imageCacheEntry.item.ImageCacheOwner = null;
                imageCacheEntry.item.Dispose();
            }
        }

        public ImageCacheItem Lookup(ImageCacheKey key)
        {
            ImageCacheItem imageCacheItem = (ImageCacheItem)this.Find(key);
            if (imageCacheItem == null)
                this.ScheduleScavenge();
            return imageCacheItem;
        }

        public void CullObjects()
        {
            this.m_numItemsInUse = 0;
            this.m_numItemsInCache = 0;
            this.m_fUseCutoffTime = !this.m_tsCullingKeepTime.Equals(TimeSpan.Zero);
            this.m_dtCullingCutoff = DateTime.UtcNow - this.m_tsCullingKeepTime;
            this.RemoveObjects(s_testCulling, this);
            this.m_fCleanupPending = false;
        }

        [Conditional("DEBUG")]
        protected void AssertOwningThread()
        {
        }

        protected virtual void SetupObjectTable()
        {
            if (this.m_objectsTable != null)
                return;
            this.m_objectsTable = new Map<object, object>();
            this.m_lruList = new Vector<ImageCache.ImageCacheEntry>(this.m_numItemsToKeep);
        }

        private object Find(object key)
        {
            object obj = null;
            if (this.m_objectsTable != null)
                this.m_objectsTable.TryGetValue(key, out obj);
            return obj;
        }

        private bool ContainsKey(object key)
        {
            bool flag = false;
            if (this.m_objectsTable != null)
                flag = this.m_objectsTable.ContainsKey(key);
            return flag;
        }

        protected virtual void ScheduleScavenge()
        {
            if (this.m_fCleanupPending || this.m_fShuttingDown)
                return;
            this.m_fCleanupPending = true;
        }

        private static bool DisposeAllTest(object subjectObject, object paramObject)
        {
            ImageCache imageCache = paramObject as ImageCache;
            if (!(subjectObject is ImageCacheItem))
                return false;
            ++imageCache.m_numItemsInCache;
            return true;
        }

        private static bool DropImageTest(object subjectObject, object paramObject)
        {
            ImageCache imageCache = paramObject as ImageCache;
            if (subjectObject is ImageCacheItem imageCacheItem)
            {
                imageCacheItem.ReleaseImage();
                ++imageCache.m_numItemsInCache;
            }
            return false;
        }

        private static bool CullingTest(object subjectObject, object paramObject)
        {
            ImageCache imageCache = paramObject as ImageCache;
            if (subjectObject is ImageCacheItem imageCacheItem)
            {
                ++imageCache.m_numItemsInCache;
                if (imageCacheItem.HasLoadsInProgress || imageCacheItem.InUse)
                {
                    ++imageCache.m_numItemsInUse;
                    return false;
                }
                if (imageCache.m_numItemsInCache - imageCache.m_numItemsInUse > imageCache.m_numItemsToKeep || imageCache.m_fUseCutoffTime && imageCacheItem.IsOlder(imageCache.m_dtCullingCutoff))
                    return true;
            }
            return false;
        }

        private static bool SpecificObjectTest(object subjectObject, object paramObject) => subjectObject == paramObject;

        [Conditional("DEBUG")]
        private void IncKeptItems()
        {
        }

        [Conditional("DEBUG")]
        private void IncCulledItems()
        {
        }

        [Conditional("DEBUG")]
        private void ClearActivityCounts()
        {
        }

        internal void UpdateLastUsedItem(ImageCacheItem item)
        {
            ImageCache.ImageCacheEntry inLruList = this.FindInLruList(item);
            if (inLruList == null)
                return;
            this.m_lruList.Remove(inLruList);
            this.m_lruList.Insert(0, inLruList);
        }

        internal ImageCache.ImageCacheEntry FindInLruList(ImageCacheItem item)
        {
            foreach (ImageCache.ImageCacheEntry lru in this.m_lruList)
            {
                if (lru.MatchItem(item))
                    return lru;
            }
            return null;
        }

        public delegate bool ObjectTest(object subjectObject, object paramObject);

        internal class ImageCacheEntry
        {
            public ImageCacheKey key;
            public ImageCacheItem item;

            public ImageCacheEntry(ImageCacheKey keyIn, ImageCacheItem itemIn)
            {
                this.key = keyIn;
                this.item = itemIn;
            }

            public bool MatchItem(ImageCacheItem itemToMatch) => itemToMatch == this.item;
        }
    }
}
