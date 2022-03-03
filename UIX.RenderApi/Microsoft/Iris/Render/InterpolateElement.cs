// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.InterpolateElement
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Graphics;
using System;

namespace Microsoft.Iris.Render
{
    public class InterpolateElement : EffectInput
    {
        internal const string ValuePropertyName = "Value";
        private EffectInput m_effectInput1;
        private EffectInput m_effectInput2;
        private float m_flValue;
        private byte m_nValueID;

        public InterpolateElement(
          string stName,
          EffectInput effectInput1,
          EffectInput effectInput2,
          float flValue)
          : this()
        {
            Debug2.Validate(!string.IsNullOrEmpty(stName), typeof(ArgumentException), nameof(stName));
            Debug2.Validate(effectInput1 != null, typeof(ArgumentNullException), nameof(effectInput1));
            Debug2.Validate(effectInput2 != null, typeof(ArgumentNullException), nameof(effectInput2));
            Debug2.Validate(flValue >= 0.0 && flValue <= 1.0, typeof(ArgumentOutOfRangeException), nameof(flValue));
            this.m_stName = stName;
            this.m_effectInput1 = effectInput1;
            this.m_effectInput2 = effectInput2;
            this.m_flValue = flValue;
        }

        public InterpolateElement() => this.m_typeInput = EffectInputType.Interpolate;

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

        public float Value
        {
            get => this.m_flValue;
            set => this.m_flValue = value;
        }

        internal byte ValueID => this.m_nValueID;

        internal override int PreProcessProperties(
          Map<string, EffectProperty> dictionary,
          ref byte nNextUniqueID)
        {
            return base.PreProcessProperties(dictionary, ref nNextUniqueID) + this.m_effectInput1.PreProcessProperties(dictionary, ref nNextUniqueID) + this.m_effectInput2.PreProcessProperties(dictionary, ref nNextUniqueID) + this.PreProcessProperty(dictionary, "Value", 8, ref this.m_nValueID, ref nNextUniqueID);
        }

        internal override bool Process(Map<string, EffectProperty> dictProperties) => this.m_effectInput1.Process(dictProperties) && this.m_effectInput2.Process(dictProperties) && this.GenerateProperty("Value", EffectPropertyType.Float, m_flValue, this.m_nValueID, dictProperties);

        internal override void AddCacheKey(ByteBuilder cacheKey)
        {
            base.AddCacheKey(cacheKey);
            this.m_effectInput1.AddCacheKey(cacheKey);
            this.m_effectInput2.AddCacheKey(cacheKey);
            this.GeneratePropertyCacheKey("Value", this.ValueID, this.Value, cacheKey);
        }
    }
}
