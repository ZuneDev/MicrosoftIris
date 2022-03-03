// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.EmbossElement
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Graphics;
using System;

namespace Microsoft.Iris.Render
{
    public class EmbossElement : EffectOperation
    {
        internal const string DirectionPropertyName = "Direction";
        internal const string GrayLevelPropertyName = "GrayLevel";
        private EmbossDirection m_direction;
        private byte m_nDirectionID;
        private float m_flGrayLevel;
        private byte m_nGrayLevelID;

        public EmbossElement(string stName, EmbossDirection direction, float flGrayLevel)
          : this()
        {
            Debug2.Validate(!string.IsNullOrEmpty(stName), typeof(ArgumentException), nameof(stName));
            Debug2.Validate(flGrayLevel >= 0.0, typeof(ArgumentOutOfRangeException), "Valid range for GrayLevel is >= 0");
            this.m_stName = stName;
            this.m_direction = direction;
            this.m_flGrayLevel = flGrayLevel;
        }

        public EmbossElement() => this.m_typeOperation = EffectOperationType.Emboss;

        public EmbossDirection Direction
        {
            get => this.m_direction;
            set => this.m_direction = value;
        }

        internal byte DirectionID => this.m_nDirectionID;

        public float GrayLevel
        {
            get => this.m_flGrayLevel;
            set => this.m_flGrayLevel = value;
        }

        internal byte GrayLevelID => this.m_nGrayLevelID;

        internal override int PreProcessProperties(
          Map<string, EffectProperty> dictionary,
          ref byte nNextUniqueID)
        {
            return base.PreProcessProperties(dictionary, ref nNextUniqueID) + this.PreProcessProperty(dictionary, "Direction", 5, ref this.m_nDirectionID, ref nNextUniqueID) + this.PreProcessProperty(dictionary, "GrayLevel", 8, ref this.m_nGrayLevelID, ref nNextUniqueID);
        }

        internal override bool Process(Map<string, EffectProperty> dictProperties) => this.GenerateProperty("Direction", EffectPropertyType.Integer, m_direction, this.m_nDirectionID, dictProperties) && this.GenerateProperty("GrayLevel", EffectPropertyType.Float, m_flGrayLevel, this.m_nGrayLevelID, dictProperties);

        internal override void AddCacheKey(ByteBuilder cacheKey)
        {
            base.AddCacheKey(cacheKey);
            this.GeneratePropertyCacheKey("Direction", this.DirectionID, (byte)this.Direction, cacheKey);
            this.GeneratePropertyCacheKey("GrayLevel", this.GrayLevelID, this.GrayLevel, cacheKey);
        }
    }
}
