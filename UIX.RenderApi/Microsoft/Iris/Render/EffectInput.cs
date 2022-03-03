// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.EffectInput
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;

namespace Microsoft.Iris.Render
{
    public abstract class EffectInput : EffectElement
    {
        internal EffectInputType m_typeInput;

        internal EffectInputType Type => this.m_typeInput;

        internal override void AddCacheKey(ByteBuilder cacheKey) => this.GenerateClassCacheKey(0, (byte)this.m_typeInput, cacheKey);
    }
}
