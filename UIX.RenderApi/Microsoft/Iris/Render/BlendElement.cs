// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.BlendElement
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Graphics;
using System;

namespace Microsoft.Iris.Render
{
    public class BlendElement : EffectInput
    {
        private EffectInput m_effectInput1;
        private EffectInput m_effectInput2;
        private ColorOperation m_colorOperation;
        private AlphaOperation m_alphaOperation;

        public BlendElement(
          EffectInput effectInput1,
          EffectInput effectInput2,
          ColorOperation colorOperation,
          AlphaOperation alphaOperation)
          : this()
        {
            Debug2.Validate(effectInput1 != null, typeof(ArgumentNullException), nameof(effectInput1));
            Debug2.Validate(effectInput2 != null, typeof(ArgumentNullException), nameof(effectInput2));
            this.m_effectInput1 = effectInput1;
            this.m_effectInput2 = effectInput2;
            this.m_colorOperation = colorOperation;
            this.m_alphaOperation = alphaOperation;
        }

        public BlendElement(
          EffectInput effectInput1,
          EffectInput effectInput2,
          ColorOperation colorOperation)
          : this()
        {
            Debug2.Validate(effectInput1 != null, typeof(ArgumentNullException), nameof(effectInput1));
            Debug2.Validate(effectInput2 != null, typeof(ArgumentNullException), nameof(effectInput2));
            this.m_effectInput1 = effectInput1;
            this.m_effectInput2 = effectInput2;
            this.m_colorOperation = colorOperation;
        }

        public BlendElement()
        {
            this.m_stName = "Blend";
            this.m_typeInput = EffectInputType.Blend;
            this.m_alphaOperation = AlphaOperation.Source1;
        }

        public EffectInput Input1
        {
            get => this.m_effectInput1;
            set => this.m_effectInput1 = value;
        }

        public EffectInput Input2
        {
            get => this.m_effectInput2;
            set => this.m_effectInput2 = value;
        }

        public ColorOperation ColorOperation
        {
            get => this.m_colorOperation;
            set => this.m_colorOperation = value;
        }

        public AlphaOperation AlphaOperation
        {
            get => this.m_alphaOperation;
            set => this.m_alphaOperation = value;
        }

        internal override void AddCacheKey(ByteBuilder cacheKey)
        {
            base.AddCacheKey(cacheKey);
            this.GenerateClassCacheKey(2, (byte)this.m_colorOperation, cacheKey);
            this.GenerateClassCacheKey(2, (byte)this.m_alphaOperation, cacheKey);
            this.m_effectInput1.AddCacheKey(cacheKey);
            this.m_effectInput2.AddCacheKey(cacheKey);
        }

        internal override int PreProcessProperties(
          Map<string, EffectProperty> dictionary,
          ref byte nNextUniqueID)
        {
            return base.PreProcessProperties(dictionary, ref nNextUniqueID) + 3 + 3 + this.m_effectInput1.PreProcessProperties(dictionary, ref nNextUniqueID) + this.m_effectInput2.PreProcessProperties(dictionary, ref nNextUniqueID);
        }

        internal override bool Process(Map<string, EffectProperty> dictProperties) => this.m_effectInput1.Process(dictProperties) && this.m_effectInput2.Process(dictProperties);
    }
}
