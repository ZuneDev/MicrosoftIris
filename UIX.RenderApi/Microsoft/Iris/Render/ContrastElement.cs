// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.ContrastElement
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Graphics;
using System;

namespace Microsoft.Iris.Render
{
    public class ContrastElement : EffectOperation
    {
        internal const string ContrastPropertyName = "Contrast";
        private float m_flContrast;
        private byte m_nContrastID;

        public ContrastElement(string stName, float flConstrastAdjust)
          : this()
        {
            Debug2.Validate(!string.IsNullOrEmpty(stName), typeof(ArgumentException), nameof(stName));
            Debug2.Validate(flConstrastAdjust >= 0.0, typeof(ArgumentOutOfRangeException), "Valid range for Contrast [0..)");
            this.m_flContrast = flConstrastAdjust;
            this.m_stName = stName;
        }

        public ContrastElement() => this.m_typeOperation = EffectOperationType.Contrast;

        public float Contrast
        {
            get => this.m_flContrast;
            set => this.m_flContrast = value;
        }

        internal byte ContrastID => this.m_nContrastID;

        internal override int PreProcessProperties(
          Map<string, EffectProperty> dictionary,
          ref byte nNextUniqueID)
        {
            return base.PreProcessProperties(dictionary, ref nNextUniqueID) + this.PreProcessProperty(dictionary, "Contrast", 8, ref this.m_nContrastID, ref nNextUniqueID);
        }

        internal override bool Process(Map<string, EffectProperty> dictProperties) => this.GenerateProperty("Contrast", EffectPropertyType.Float, m_flContrast, this.m_nContrastID, dictProperties);

        internal override void AddCacheKey(ByteBuilder cacheKey)
        {
            base.AddCacheKey(cacheKey);
            this.GeneratePropertyCacheKey("Contrast", this.ContrastID, this.Contrast, cacheKey);
        }
    }
}
