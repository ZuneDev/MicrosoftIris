// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.LightShaftElement
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Graphics;
using System;

namespace Microsoft.Iris.Render
{
    public class LightShaftElement : EffectOperation
    {
        internal const string PositionPropertyName = "Position";
        internal const string DecayPropertyName = "Decay";
        internal const string DensityPropertyName = "Density";
        internal const string IntensityPropertyName = "Intensity";
        internal const string FallOffPropertyName = "FallOff";
        internal const string WeightPropertyName = "Weight";
        private Vector3 m_vPosition;
        private byte m_nPositionID;
        private float m_flDecay;
        private byte m_nDecayID;
        private float m_flDensity;
        private byte m_nDensityID;
        private float m_flIntensity;
        private byte m_nIntensityID;
        private float m_flFallOff;
        private byte m_nFallOffID;
        private float m_flWeight;
        private byte m_nWeightID;

        public LightShaftElement(
          string stName,
          Vector3 vPosition,
          float flDecay,
          float flDensity,
          float flFallOff,
          float flIntensity,
          float flWeight)
          : this()
        {
            Debug2.Validate(!string.IsNullOrEmpty(stName), typeof(ArgumentException), nameof(stName));
            Debug2.Validate(flDecay >= 0.0 && flDecay <= 1.0, typeof(ArgumentOutOfRangeException), "Valid range for Decay is [0,1]");
            Debug2.Validate(flDensity > 0.0 && flDensity <= 1.0, typeof(ArgumentOutOfRangeException), "Valid range for Density is (0..1]");
            Debug2.Validate(flFallOff >= 0.0 && flFallOff <= 2.0, typeof(ArgumentOutOfRangeException), "Valid range for FallOff is [0..2])");
            Debug2.Validate(flIntensity >= 0.0, typeof(ArgumentOutOfRangeException), "Valid range for Intensity is [0..)");
            Debug2.Validate(flWeight >= 0.0, typeof(ArgumentOutOfRangeException), "Valid range for Weight is [0..)");
            this.m_stName = stName;
            this.m_vPosition = vPosition;
            this.m_flDecay = flDecay;
            this.m_flDensity = flDensity;
            this.m_flIntensity = flIntensity;
            this.m_flFallOff = flFallOff;
            this.m_flWeight = flWeight;
        }

        public LightShaftElement() => this.m_typeOperation = EffectOperationType.LightShaft;

        public Vector3 Position
        {
            get => this.m_vPosition;
            set => this.m_vPosition = value;
        }

        internal byte PositionID => this.m_nPositionID;

        public float Decay
        {
            get => this.m_flDecay;
            set
            {
                Debug2.Validate(value >= 0.0 && value <= 1.0, typeof(ArgumentOutOfRangeException), "Valid range for Decay is [0,1]");
                this.m_flDecay = value;
            }
        }

        internal byte DecayID => this.m_nDecayID;

        public float Density
        {
            get => this.m_flDensity;
            set
            {
                Debug2.Validate(value > 0.0 && value <= 1.0, typeof(ArgumentOutOfRangeException), "Valid range for Density is (0..1]");
                this.m_flDensity = value;
            }
        }

        internal byte DensityID => this.m_nDensityID;

        public float Intensity
        {
            get => this.m_flIntensity;
            set
            {
                Debug2.Validate(value >= 0.0, typeof(ArgumentOutOfRangeException), "Valid range for Intensity is [0..)");
                this.m_flIntensity = value;
            }
        }

        internal byte IntensityID => this.m_nIntensityID;

        public float FallOff
        {
            get => this.m_flFallOff;
            set
            {
                Debug2.Validate(value >= 0.0 && value <= 2.0, typeof(ArgumentOutOfRangeException), "Valid range for FallOff is [0..2])");
                this.m_flFallOff = value;
            }
        }

        internal byte FallOffID => this.m_nFallOffID;

        public float Weight
        {
            get => this.m_flWeight;
            set
            {
                Debug2.Validate(value >= 0.0 && value <= 1.0, typeof(ArgumentOutOfRangeException), "Valid range for Weight is [0..)");
                this.m_flWeight = value;
            }
        }

        internal byte WeightID => this.m_nWeightID;

        internal override int PreProcessProperties(
          Map<string, EffectProperty> dictionary,
          ref byte nNextUniqueID)
        {
            return base.PreProcessProperties(dictionary, ref nNextUniqueID) + this.PreProcessProperty(dictionary, "Position", 16, ref this.m_nPositionID, ref nNextUniqueID) + this.PreProcessProperty(dictionary, "Decay", 8, ref this.m_nDecayID, ref nNextUniqueID) + this.PreProcessProperty(dictionary, "Density", 8, ref this.m_nDensityID, ref nNextUniqueID) + this.PreProcessProperty(dictionary, "Intensity", 8, ref this.m_nIntensityID, ref nNextUniqueID) + this.PreProcessProperty(dictionary, "FallOff", 8, ref this.m_nFallOffID, ref nNextUniqueID) + this.PreProcessProperty(dictionary, "Weight", 8, ref this.m_nWeightID, ref nNextUniqueID);
        }

        internal override bool Process(Map<string, EffectProperty> dictProperties) => this.GenerateProperty("Position", EffectPropertyType.Vector3, m_vPosition, this.m_nPositionID, dictProperties) && this.GenerateProperty("Decay", EffectPropertyType.Float, m_flDecay, this.m_nDecayID, dictProperties) && (this.GenerateProperty("Density", EffectPropertyType.Float, m_flDensity, this.m_nDensityID, dictProperties) && this.GenerateProperty("Intensity", EffectPropertyType.Float, m_flIntensity, this.m_nIntensityID, dictProperties)) && this.GenerateProperty("FallOff", EffectPropertyType.Float, m_flFallOff, this.m_nFallOffID, dictProperties) && this.GenerateProperty("Weight", EffectPropertyType.Float, m_flWeight, this.m_nWeightID, dictProperties);

        internal override void AddCacheKey(ByteBuilder cacheKey)
        {
            base.AddCacheKey(cacheKey);
            this.GeneratePropertyCacheKey("Position", this.PositionID, this.Position, cacheKey);
            this.GeneratePropertyCacheKey("Decay", this.DecayID, this.Decay, cacheKey);
            this.GeneratePropertyCacheKey("Density", this.DensityID, this.Density, cacheKey);
            this.GeneratePropertyCacheKey("Intensity", this.IntensityID, this.Intensity, cacheKey);
            this.GeneratePropertyCacheKey("FallOff", this.FallOffID, this.FallOff, cacheKey);
            this.GeneratePropertyCacheKey("Weight", this.WeightID, this.Weight, cacheKey);
        }
    }
}
