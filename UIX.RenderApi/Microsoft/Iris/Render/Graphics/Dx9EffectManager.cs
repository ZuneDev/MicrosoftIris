// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Graphics.Dx9EffectManager
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Internal;
using System;

namespace Microsoft.Iris.Render.Graphics
{
    internal class Dx9EffectManager : RenderObject
    {
        private const int k_cMaxTargetItems = 50;
        private RenderSession m_session;
        private Dx9GraphicsDevice m_device;
        private Dx9EffectBuilder m_effectBuilder;
        private Map<ByteBuilder, Dx9EffectResource> m_mapEffectCache;
        private bool m_fPendingCacheUpdate;
        private readonly DeferredHandler m_cbUpdateCache;

        internal Dx9EffectManager(RenderSession session, Dx9GraphicsDevice dx9Device)
        {
            this.m_session = session;
            this.m_device = dx9Device;
            this.m_cbUpdateCache = new DeferredHandler(this.UpdateCache);
            this.m_effectBuilder = new Dx9EffectBuilder(this.m_device.GraphicsCaps);
        }

        protected override void Dispose(bool fInDispose)
        {
            try
            {
                if (!fInDispose || this.m_mapEffectCache == null)
                    return;
                Map<ByteBuilder, Dx9EffectResource>.ValueCollection.Enumerator enumerator = this.m_mapEffectCache.Values.GetEnumerator();
                while (enumerator.MoveNext())
                    enumerator.Current?.UnregisterUsage(this);
                this.m_mapEffectCache = null;
            }
            finally
            {
                base.Dispose(fInDispose);
            }
        }

        internal Dx9EffectBuilder EffectBuilder => this.m_effectBuilder;

        internal Dx9GraphicsDevice Device => this.m_device;

        private bool CacheLookup(ByteBuilder cacheKey, out Dx9EffectResource resource)
        {
            if (this.m_mapEffectCache == null)
            {
                resource = null;
                return false;
            }
            bool flag = this.m_mapEffectCache.ContainsKey(cacheKey);
            resource = !flag ? null : this.m_mapEffectCache[cacheKey];
            return flag;
        }

        private void AddCachedEffect(ByteBuilder cacheWriter, Dx9EffectResource effect)
        {
            if (this.m_mapEffectCache == null)
                this.m_mapEffectCache = new Map<ByteBuilder, Dx9EffectResource>(50);
            this.m_mapEffectCache.Add(cacheWriter, effect);
            if (effect == null)
                return;
            this.ScheduleCacheUpdate();
        }

        internal bool CreateEffectResource(
          string stName,
          EffectInput element,
          int nPropCacheSize,
          out Dx9EffectResource dxEffectResource)
        {
            Dx9EffectResource resource = null;
            ByteBuilder byteBuilder = new ByteBuilder(nPropCacheSize);
            element.AddCacheKey(byteBuilder);
            if (!this.CacheLookup(byteBuilder, out resource))
            {
                resource = new Dx9EffectResource(this.m_session, stName, this);
                resource.RegisterUsage(this);
                if (!resource.Create(element))
                {
                    resource.UnregisterUsage(this);
                    dxEffectResource = null;
                    this.AddCachedEffect(byteBuilder, null);
                    return false;
                }
                this.AddCachedEffect(byteBuilder, resource);
            }
            dxEffectResource = resource;
            return dxEffectResource != null;
        }

        private void UpdateCache(object arg)
        {
            if (this.m_mapEffectCache != null)
            {
                Map<ByteBuilder, Dx9EffectResource> map = new Map<ByteBuilder, Dx9EffectResource>(Math.Max(50, this.m_mapEffectCache.Count));
                foreach (KeyValueEntry<ByteBuilder, Dx9EffectResource> keyValueEntry in this.m_mapEffectCache)
                {
                    if (keyValueEntry.Value != null)
                    {
                        if (keyValueEntry.Value.UsageCount == 1)
                            keyValueEntry.Value.UnregisterUsage(this);
                        else
                            map.Add(keyValueEntry.Key, keyValueEntry.Value);
                    }
                }
                if (map.Count != this.m_mapEffectCache.Count)
                    this.m_mapEffectCache = map;
            }
            this.m_fPendingCacheUpdate = false;
        }

        private void ScheduleCacheUpdate()
        {
            if (this.m_fPendingCacheUpdate || this.m_mapEffectCache.Count < 50)
                return;
            this.m_session.DeferredInvoke(m_cbUpdateCache, null, DeferredInvokePriority.Idle, new TimeSpan(0, 0, 5));
            this.m_fPendingCacheUpdate = true;
        }
    }
}
