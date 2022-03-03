// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.SpotLight2DElement
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Graphics;
using System;

namespace Microsoft.Iris.Render
{
    public class SpotLight2DElement : EffectInput
    {
        internal const string PositionPropertyName = "Position";
        internal const string DirectionAnglePropertyName = "DirectionAngle";
        internal const string LightColorPropertyName = "LightColor";
        internal const string AmbientColorPropertyName = "AmbientColor";
        internal const string InnerConeAnglePropertyName = "InnerConeAngle";
        internal const string OuterConeAnglePropertyName = "OuterConeAngle";
        internal const string IntensityPropertyName = "Intensity";
        internal const string AttenuationPropertyName = "Attenuation";
        private Vector3 m_vPosition;
        private byte m_nPositionID;
        private float m_flDirectionAngle;
        private byte m_nDirectionAngleID;
        private ColorF m_clrLight;
        private byte m_nLightColorID;
        private ColorF m_clrAmbient;
        private byte m_nAmbientColorID;
        private float m_flInnerConeAngle;
        private byte m_nInnerConeAngleID;
        private float m_flOuterConeAngle;
        private byte m_nOuterConeAngleID;
        private float m_flIntensity;
        private byte m_nIntensityID;
        private Vector3 m_vAttenuation;
        private byte m_nAttenuationID;

        public SpotLight2DElement(
          string stName,
          Vector3 vPosition,
          float flDirectionAngle,
          ColorF clrLight,
          ColorF clrAmbient,
          float flInnerConeAngle,
          float flOuterConeAngle,
          float flIntensity,
          Vector3 vAttenuation)
          : this()
        {
            Debug2.Validate(!string.IsNullOrEmpty(stName), typeof(ArgumentException), nameof(stName));
            Debug2.Validate(flIntensity >= 0.0, typeof(ArgumentOutOfRangeException), "Valid range for flIntensity is [0..)");
            Debug2.Validate(flInnerConeAngle >= 0.0, typeof(ArgumentOutOfRangeException), "Valid range for flInnerConeAngle is [0..)");
            Debug2.Validate(flOuterConeAngle >= 0.0, typeof(ArgumentOutOfRangeException), "Valid range for flOuterConeAngle is [0..)");
            this.m_stName = stName;
            this.m_vPosition = vPosition;
            this.m_flDirectionAngle = flDirectionAngle;
            this.m_clrLight = clrLight;
            this.m_clrAmbient = clrAmbient;
            this.m_flInnerConeAngle = flInnerConeAngle;
            this.m_flOuterConeAngle = flOuterConeAngle;
            this.m_flIntensity = flIntensity;
            this.m_vAttenuation = vAttenuation;
        }

        public SpotLight2DElement()
        {
            this.m_typeInput = EffectInputType.Spotlight2D;
            this.m_vAttenuation = new Vector3(0.0f, 1f, 0.0f);
            this.m_clrAmbient = new ColorF(0, 0, 0, 0);
        }

        public Vector3 Position
        {
            get => this.m_vPosition;
            set => this.m_vPosition = value;
        }

        internal byte PositionID => this.m_nPositionID;

        public float DirectionAngle
        {
            get => this.m_flDirectionAngle;
            set => this.m_flDirectionAngle = value;
        }

        internal byte DirectionAngleID => this.m_nDirectionAngleID;

        public ColorF LightColor
        {
            get => this.m_clrLight;
            set => this.m_clrLight = value;
        }

        internal byte LightColorID => this.m_nLightColorID;

        public ColorF AmbientColor
        {
            get => this.m_clrAmbient;
            set => this.m_clrAmbient = value;
        }

        internal byte AmbientColorID => this.m_nAmbientColorID;

        public float InnerConeAngle
        {
            get => this.m_flInnerConeAngle;
            set => this.m_flInnerConeAngle = value;
        }

        internal byte InnerConeAngleID => this.m_nInnerConeAngleID;

        public float OuterConeAngle
        {
            get => this.m_flOuterConeAngle;
            set => this.m_flOuterConeAngle = value;
        }

        internal byte OuterConeAngleID => this.m_nOuterConeAngleID;

        public float Intensity
        {
            get => this.m_flIntensity;
            set => this.m_flIntensity = value;
        }

        internal byte IntensityID => this.m_nIntensityID;

        public Vector3 Attenuation
        {
            get => this.m_vAttenuation;
            set => this.m_vAttenuation = value;
        }

        internal byte AttenuationID => this.m_nAttenuationID;

        internal override int PreProcessProperties(
          Map<string, EffectProperty> dictionary,
          ref byte nNextUniqueID)
        {
            return base.PreProcessProperties(dictionary, ref nNextUniqueID) + this.PreProcessProperty(dictionary, "Position", 16, ref this.m_nPositionID, ref nNextUniqueID) + this.PreProcessProperty(dictionary, "DirectionAngle", 8, ref this.m_nDirectionAngleID, ref nNextUniqueID) + this.PreProcessProperty(dictionary, "LightColor", 20, ref this.m_nLightColorID, ref nNextUniqueID) + this.PreProcessProperty(dictionary, "AmbientColor", 20, ref this.m_nAmbientColorID, ref nNextUniqueID) + this.PreProcessProperty(dictionary, "InnerConeAngle", 8, ref this.m_nInnerConeAngleID, ref nNextUniqueID) + this.PreProcessProperty(dictionary, "OuterConeAngle", 8, ref this.m_nOuterConeAngleID, ref nNextUniqueID) + this.PreProcessProperty(dictionary, "Intensity", 8, ref this.m_nIntensityID, ref nNextUniqueID) + this.PreProcessProperty(dictionary, "Attenuation", 16, ref this.m_nAttenuationID, ref nNextUniqueID);
        }

        internal override bool Process(Map<string, EffectProperty> dictProperties) => this.GenerateProperty("Position", EffectPropertyType.Vector3, m_vPosition, this.m_nPositionID, dictProperties) && this.GenerateProperty("DirectionAngle", EffectPropertyType.Float, m_flDirectionAngle, this.m_nDirectionAngleID, dictProperties) && (this.GenerateProperty("LightColor", EffectPropertyType.Color, m_clrLight, this.m_nLightColorID, dictProperties) && this.GenerateProperty("AmbientColor", EffectPropertyType.Color, m_clrAmbient, this.m_nAmbientColorID, dictProperties)) && (this.GenerateProperty("InnerConeAngle", EffectPropertyType.Float, m_flInnerConeAngle, this.m_nInnerConeAngleID, dictProperties) && this.GenerateProperty("OuterConeAngle", EffectPropertyType.Float, m_flOuterConeAngle, this.m_nOuterConeAngleID, dictProperties) && (this.GenerateProperty("Intensity", EffectPropertyType.Float, m_flIntensity, this.m_nIntensityID, dictProperties) && this.GenerateProperty("Attenuation", EffectPropertyType.Vector3, m_vAttenuation, this.m_nAttenuationID, dictProperties)));

        internal override void AddCacheKey(ByteBuilder cacheKey)
        {
            base.AddCacheKey(cacheKey);
            this.GeneratePropertyCacheKey("Position", this.PositionID, this.Position, cacheKey);
            this.GeneratePropertyCacheKey("DirectionAngle", this.DirectionAngleID, this.DirectionAngle, cacheKey);
            this.GeneratePropertyCacheKey("LightColor", this.LightColorID, this.LightColor.ToVector4(), cacheKey);
            this.GeneratePropertyCacheKey("AmbientColor", this.AmbientColorID, this.AmbientColor.ToVector4(), cacheKey);
            this.GeneratePropertyCacheKey("InnerConeAngle", this.InnerConeAngleID, this.InnerConeAngle, cacheKey);
            this.GeneratePropertyCacheKey("OuterConeAngle", this.OuterConeAngleID, this.OuterConeAngle, cacheKey);
            this.GeneratePropertyCacheKey("Intensity", this.IntensityID, this.Intensity, cacheKey);
            this.GeneratePropertyCacheKey("Attenuation", this.AttenuationID, this.Attenuation, cacheKey);
        }
    }
}
