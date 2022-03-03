// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.BrightnessElement
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Graphics;
using System;

namespace Microsoft.Iris.Render
{
    public class BrightnessElement : EffectOperation
    {
        internal const string BrightnessPropertyName = "Brightness";
        private float m_flBrightness;
        private byte m_nBrightnessID;

        public BrightnessElement(string stName, float flBrightness)
          : this()
        {
            Debug2.Validate(!string.IsNullOrEmpty(stName), typeof(ArgumentException), nameof(stName));
            Debug2.Validate(flBrightness >= -1.0 && flBrightness <= 1.0, typeof(ArgumentOutOfRangeException), "Valid range for Brightness is [-1..1]");
            this.m_flBrightness = flBrightness;
            this.m_stName = stName;
        }

        public BrightnessElement() => this.m_typeOperation = EffectOperationType.Brightness;

        public float Brightness
        {
            get => this.m_flBrightness;
            set => this.m_flBrightness = value;
        }

        internal byte BrightnessID => this.m_nBrightnessID;

        internal override int PreProcessProperties(
          Map<string, EffectProperty> dictionary,
          ref byte nNextUniqueID)
        {
            return base.PreProcessProperties(dictionary, ref nNextUniqueID) + this.PreProcessProperty(dictionary, "Brightness", 8, ref this.m_nBrightnessID, ref nNextUniqueID);
        }

        internal override bool Process(Map<string, EffectProperty> dictProperties) => this.GenerateProperty("Brightness", EffectPropertyType.Float, m_flBrightness, this.m_nBrightnessID, dictProperties);

        internal override void AddCacheKey(ByteBuilder cacheKey)
        {
            base.AddCacheKey(cacheKey);
            this.GeneratePropertyCacheKey("Brightness", this.BrightnessID, this.Brightness, cacheKey);
        }
    }
}
