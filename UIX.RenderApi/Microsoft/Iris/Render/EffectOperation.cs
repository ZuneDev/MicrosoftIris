// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.EffectOperation
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Graphics;

namespace Microsoft.Iris.Render
{
    public abstract class EffectOperation : EffectElement
    {
        protected EffectOperationType m_typeOperation;

        internal EffectOperationType Type => this.m_typeOperation;

        internal override void AddCacheKey(ByteBuilder cacheKey) => this.GenerateClassCacheKey(1, (byte)this.Type, cacheKey);

        internal override int PreProcessProperties(
          Map<string, EffectProperty> dictionary,
          ref byte nNextUniqueID)
        {
            return 3 + base.PreProcessProperties(dictionary, ref nNextUniqueID);
        }
    }
}
