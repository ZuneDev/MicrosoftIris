// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Common.CachedRenderObject
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

namespace Microsoft.Iris.Render.Common
{
    internal abstract class CachedRenderObject : SharedRenderObject
    {
        private ObjectCache m_cache;

        internal ObjectCache Cache
        {
            get => this.m_cache;
            set => this.m_cache = value;
        }

        internal override void UnregisterUsage(object user)
        {
            if (this.UsageCount == 1 && this.m_cache != null && this.m_cache.Add(this))
                this.Reset();
            base.UnregisterUsage(user);
        }

        internal abstract void Reset();
    }
}
