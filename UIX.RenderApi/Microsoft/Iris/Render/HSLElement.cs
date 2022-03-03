// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.HSLElement
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Graphics;
using System;

namespace Microsoft.Iris.Render
{
    internal class HSLElement : EffectOperation
    {
        internal const string HuePropertyName = "Hue";
        internal const string SaturationPropertyName = "Saturation";
        internal const string LightnessPropertyName = "Lightness";
        private float m_flHue;
        private byte m_nHueID;
        private float m_flSaturation;
        private byte m_nSaturationID;
        private float m_flLightness;
        private byte m_nLightnessID;

        public HSLElement(string stName, float flHue, float flSaturation, float flLightness)
          : this()
        {
            Debug2.Validate(!string.IsNullOrEmpty(stName), typeof(ArgumentException), nameof(stName));
            Debug2.Validate(flHue >= 0.0, typeof(ArgumentOutOfRangeException), "Valid range for Hue is >= 0");
            Debug2.Validate(flSaturation >= 0.0, typeof(ArgumentOutOfRangeException), "Valid range for Saturation is >= 0");
            Debug2.Validate(flLightness >= 0.0, typeof(ArgumentOutOfRangeException), "Valid range for Lightness is >= 0");
            this.m_flHue = flHue;
            this.m_flSaturation = flSaturation;
            this.m_flLightness = flLightness;
            this.m_stName = stName;
        }

        public HSLElement() => this.m_typeOperation = EffectOperationType.HSL;

        public float Hue
        {
            get => this.m_flHue;
            set => this.m_flHue = value;
        }

        internal byte HueID => this.m_nHueID;

        public float Saturation
        {
            get => this.m_flSaturation;
            set => this.m_flSaturation = value;
        }

        internal byte SaturationID => this.m_nSaturationID;

        public float Lightness
        {
            get => this.m_flLightness;
            set => this.m_flLightness = value;
        }

        internal byte LightnessID => this.m_nLightnessID;

        internal override int PreProcessProperties(
          Map<string, EffectProperty> dictionary,
          ref byte nNextUniqueID)
        {
            return base.PreProcessProperties(dictionary, ref nNextUniqueID) + this.PreProcessProperty(dictionary, "Hue", 8, ref this.m_nHueID, ref nNextUniqueID) + this.PreProcessProperty(dictionary, "Saturation", 8, ref this.m_nSaturationID, ref nNextUniqueID) + this.PreProcessProperty(dictionary, "Lightness", 8, ref this.m_nLightnessID, ref nNextUniqueID);
        }

        internal override bool Process(Map<string, EffectProperty> dictProperties) => this.GenerateProperty("Hue", EffectPropertyType.Float, m_flHue, this.m_nHueID, dictProperties) && this.GenerateProperty("Saturation", EffectPropertyType.Float, m_flSaturation, this.m_nSaturationID, dictProperties) && this.GenerateProperty("Lightness", EffectPropertyType.Float, m_flLightness, this.m_nLightnessID, dictProperties);

        internal override void AddCacheKey(ByteBuilder cacheKey)
        {
            base.AddCacheKey(cacheKey);
            this.GeneratePropertyCacheKey("Hue", this.HueID, this.Hue, cacheKey);
            this.GeneratePropertyCacheKey("Saturation", this.SaturationID, this.Saturation, cacheKey);
            this.GeneratePropertyCacheKey("Lightness", this.LightnessID, this.Lightness, cacheKey);
        }
    }
}
