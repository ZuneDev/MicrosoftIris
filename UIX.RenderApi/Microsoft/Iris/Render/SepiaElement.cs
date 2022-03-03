// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.SepiaElement
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Graphics;
using System;

namespace Microsoft.Iris.Render
{
    public class SepiaElement : EffectOperation
    {
        internal const string LightColorPropertyName = "LightColor";
        internal const string DarkColorPropertyName = "DarkColor";
        internal const string DesaturatePropertyName = "Desaturate";
        internal const string TonePropertyName = "Tone";
        private ColorF m_lightColor;
        private byte m_nLightColorID;
        private ColorF m_darkColor;
        private byte m_nDarkColorID;
        private float m_flDesaturate;
        private byte m_nDesaturateID;
        private float m_flTone;
        private byte m_nToneID;

        public SepiaElement(
          string stName,
          ColorF darkColor,
          ColorF lightColor,
          float flDesat,
          float flTone)
        {
            Debug2.Validate(!string.IsNullOrEmpty(stName), typeof(ArgumentException), nameof(stName));
            Debug2.Validate(flDesat >= 0.0, typeof(ArgumentOutOfRangeException), "Valid range for Desat is >= 0");
            Debug2.Validate(flTone >= 0.0, typeof(ArgumentOutOfRangeException), "Valid range for Tone is >= 0");
            this.m_typeOperation = EffectOperationType.Sepia;
            this.m_lightColor = lightColor;
            this.m_darkColor = darkColor;
            this.m_flDesaturate = flDesat;
            this.m_flTone = flTone;
            this.m_stName = stName;
        }

        public SepiaElement(string stName)
        {
            this.m_typeOperation = EffectOperationType.Sepia;
            this.m_lightColor = new ColorF(1f, 0.9f, 0.5f);
            this.m_darkColor = new ColorF(0.2f, 0.05f, 0.0f);
            this.m_flDesaturate = 0.5f;
            this.m_flTone = 1f;
            this.m_stName = stName;
        }

        public SepiaElement()
          : this(null)
        {
        }

        public ColorF LightColor
        {
            get => this.m_lightColor;
            set => this.m_lightColor = value;
        }

        internal byte LightColorID => this.m_nLightColorID;

        public ColorF DarkColor
        {
            get => this.m_darkColor;
            set => this.m_darkColor = value;
        }

        internal byte DarkColorID => this.m_nDarkColorID;

        public float Desaturate
        {
            get => this.m_flDesaturate;
            set => this.m_flDesaturate = value;
        }

        internal byte DesaturateID => this.m_nDesaturateID;

        public float Tone
        {
            get => this.m_flTone;
            set => this.m_flTone = value;
        }

        internal byte ToneID => this.m_nToneID;

        internal override int PreProcessProperties(
          Map<string, EffectProperty> dictionary,
          ref byte nNextUniqueID)
        {
            return base.PreProcessProperties(dictionary, ref nNextUniqueID) + this.PreProcessProperty(dictionary, "LightColor", 20, ref this.m_nLightColorID, ref nNextUniqueID) + this.PreProcessProperty(dictionary, "DarkColor", 20, ref this.m_nDarkColorID, ref nNextUniqueID) + this.PreProcessProperty(dictionary, "Desaturate", 8, ref this.m_nDesaturateID, ref nNextUniqueID) + this.PreProcessProperty(dictionary, "Tone", 8, ref this.m_nToneID, ref nNextUniqueID);
        }

        internal override bool Process(Map<string, EffectProperty> dictProperties) => this.GenerateProperty("LightColor", EffectPropertyType.Color, m_lightColor, this.m_nLightColorID, dictProperties) && this.GenerateProperty("DarkColor", EffectPropertyType.Color, m_darkColor, this.m_nDarkColorID, dictProperties) && this.GenerateProperty("Desaturate", EffectPropertyType.Float, m_flDesaturate, this.m_nDesaturateID, dictProperties) && this.GenerateProperty("Tone", EffectPropertyType.Float, m_flTone, this.m_nToneID, dictProperties);

        internal override void AddCacheKey(ByteBuilder cacheKey)
        {
            base.AddCacheKey(cacheKey);
            this.GeneratePropertyCacheKey("LightColor", this.LightColorID, this.LightColor.ToVector4(), cacheKey);
            this.GeneratePropertyCacheKey("DarkColor", this.DarkColorID, this.DarkColor.ToVector4(), cacheKey);
            this.GeneratePropertyCacheKey("Desaturate", this.DesaturateID, this.Desaturate, cacheKey);
            this.GeneratePropertyCacheKey("Tone", this.ToneID, this.Tone, cacheKey);
        }
    }
}
