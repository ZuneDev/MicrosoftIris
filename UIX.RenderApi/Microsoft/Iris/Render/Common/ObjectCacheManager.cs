// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Common.ObjectCacheManager
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Internal;
using System;

namespace Microsoft.Iris.Render.Common
{
    internal class ObjectCacheManager
    {
        private RenderSession m_sesion;
        private Vector<ObjectCache> m_listCaches;
        private bool m_fPendingUpdateCache;
        private readonly DeferredHandler m_cbUpdateCache;

        internal ObjectCacheManager(RenderSession session, int nInitialSize)
        {
            this.m_sesion = session;
            this.m_listCaches = new Vector<ObjectCache>(nInitialSize);
            this.m_cbUpdateCache = new DeferredHandler(this.UpdateCache);
        }

        internal void RegisterCache(ObjectCache cache)
        {
            this.m_listCaches.Add(cache);
            this.ScheduleUpdateCache();
        }

        internal void UnregisterCache(ObjectCache cache) => this.m_listCaches.Remove(cache);

        private void UpdateCache(object arg)
        {
            this.m_fPendingUpdateCache = false;
            for (int index = this.m_listCaches.Count - 1; index >= 0; --index)
                this.m_listCaches[index].Update();
            this.ScheduleUpdateCache();
        }

        private void ScheduleUpdateCache()
        {
            if (this.m_fPendingUpdateCache || this.m_listCaches.Count <= 0)
                return;
            this.m_sesion.DeferredInvoke(m_cbUpdateCache, null, DeferredInvokePriority.Idle, new TimeSpan(0, 0, 5));
            this.m_fPendingUpdateCache = true;
        }
    }
}
