// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.PointLight2DElement
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Graphics;
using System;

namespace Microsoft.Iris.Render
{
    public class PointLight2DElement : EffectInput
    {
        internal const string PositionPropertyName = "Position";
        internal const string RadiusPropertyName = "Radius";
        internal const string LightColorPropertyName = "LightColor";
        internal const string AmbientColorPropertyName = "AmbientColor";
        internal const string AttenuationPropertyName = "Attenuation";
        private Vector3 m_vPosition;
        private byte m_nPositionID;
        private float m_flRadius;
        private byte m_nRadiusID;
        private ColorF m_clrLight;
        private byte m_nLightColorID;
        private ColorF m_clrAmbient;
        private byte m_nAmbientColorID;
        private Vector3 m_vAttenuation;
        private byte m_nAttenuationID;

        public PointLight2DElement(
          string stName,
          Vector3 vPosition,
          float flRadius,
          ColorF clrLight,
          ColorF clrAmbient,
          Vector3 vAttenuation)
          : this()
        {
            Debug2.Validate(!string.IsNullOrEmpty(stName), typeof(ArgumentException), nameof(stName));
            Debug2.Validate(flRadius >= 0.0, typeof(ArgumentOutOfRangeException), "Valid range for flRadius is [0..)");
            this.m_stName = stName;
            this.m_vPosition = vPosition;
            this.m_flRadius = flRadius;
            this.m_clrLight = clrLight;
            this.m_clrAmbient = clrAmbient;
            this.m_vAttenuation = vAttenuation;
        }

        public PointLight2DElement()
        {
            this.m_typeInput = EffectInputType.PointLight2D;
            this.m_vAttenuation = new Vector3(0.0f, 1f, 0.0f);
            this.m_clrAmbient = new ColorF(0, 0, 0, 0);
        }

        public Vector3 Position
        {
            get => this.m_vPosition;
            set => this.m_vPosition = value;
        }

        internal byte PositionID => this.m_nPositionID;

        public float Radius
        {
            get => this.m_flRadius;
            set => this.m_flRadius = value;
        }

        internal byte RadiusID => this.m_nRadiusID;

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
            return base.PreProcessProperties(dictionary, ref nNextUniqueID) + this.PreProcessProperty(dictionary, "Position", 16, ref this.m_nPositionID, ref nNextUniqueID) + this.PreProcessProperty(dictionary, "Radius", 8, ref this.m_nRadiusID, ref nNextUniqueID) + this.PreProcessProperty(dictionary, "LightColor", 20, ref this.m_nLightColorID, ref nNextUniqueID) + this.PreProcessProperty(dictionary, "AmbientColor", 20, ref this.m_nAmbientColorID, ref nNextUniqueID) + this.PreProcessProperty(dictionary, "Attenuation", 16, ref this.m_nAttenuationID, ref nNextUniqueID);
        }

        internal override bool Process(Map<string, EffectProperty> dictProperties) => this.GenerateProperty("Position", EffectPropertyType.Vector3, m_vPosition, this.m_nPositionID, dictProperties) && this.GenerateProperty("Radius", EffectPropertyType.Float, m_flRadius, this.m_nRadiusID, dictProperties) && (this.GenerateProperty("LightColor", EffectPropertyType.Color, m_clrLight, this.m_nLightColorID, dictProperties) && this.GenerateProperty("AmbientColor", EffectPropertyType.Color, m_clrAmbient, this.m_nAmbientColorID, dictProperties)) && this.GenerateProperty("Attenuation", EffectPropertyType.Vector3, m_vAttenuation, this.m_nAttenuationID, dictProperties);

        internal override void AddCacheKey(ByteBuilder cacheKey)
        {
            base.AddCacheKey(cacheKey);
            this.GeneratePropertyCacheKey("Position", this.PositionID, this.Position, cacheKey);
            this.GeneratePropertyCacheKey("Radius", this.RadiusID, this.Radius, cacheKey);
            this.GeneratePropertyCacheKey("LightColor", this.LightColorID, this.LightColor.ToVector4(), cacheKey);
            this.GeneratePropertyCacheKey("AmbientColor", this.AmbientColorID, this.AmbientColor.ToVector4(), cacheKey);
            this.GeneratePropertyCacheKey("Attenuation", this.AttenuationID, this.Attenuation, cacheKey);
        }
    }
}
