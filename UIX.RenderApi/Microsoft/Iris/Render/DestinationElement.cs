// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.DestinationElement
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Graphics;
using System;

namespace Microsoft.Iris.Render
{
    public class DestinationElement : EffectInput
    {
        internal const string DownsamplePropertyName = "Downsample";
        private const string DestinationPropertyName = "Destination";
        internal const string UVOffsetPropertyName = "UVOffset";
        private float m_flDownsample;
        private byte m_nDownsampleID;
        private byte m_nDestinationID;
        private Vector2 m_UVOffset;
        private byte m_nUVOffsetID;

        public DestinationElement()
          : this(1f)
        {
        }

        public DestinationElement(float flDownsample)
        {
            Debug2.Validate(flDownsample >= 0.0 && flDownsample <= 1.0, typeof(ArgumentOutOfRangeException), "Valid range for downsample is [0,1]");
            this.m_stName = "Destination";
            this.m_typeInput = EffectInputType.Destination;
            this.m_flDownsample = flDownsample;
        }

        public float Downsample
        {
            get => this.m_flDownsample;
            set => this.m_flDownsample = value;
        }

        internal byte DownsampleID => this.m_nDownsampleID;

        internal byte DestinationID => this.m_nDestinationID;

        public Vector2 UVOffset
        {
            get => this.m_UVOffset;
            set => this.m_UVOffset = value;
        }

        internal byte UVOffsetID => this.m_nUVOffsetID;

        internal override int PreProcessProperties(
          Map<string, EffectProperty> dictionary,
          ref byte nNextUniqueID)
        {
            int num = base.PreProcessProperties(dictionary, ref nNextUniqueID);
            this.AddEffectProperty(dictionary, "Destination");
            this.m_nDestinationID = nNextUniqueID++;
            this.AddEffectProperty(dictionary, "Downsample");
            this.m_nDownsampleID = nNextUniqueID++;
            return num + this.PreProcessProperty(dictionary, "UVOffset", 12, ref this.m_nUVOffsetID, ref nNextUniqueID);
        }

        internal override bool Process(Map<string, EffectProperty> dictProperties)
        {
            if (!this.GenerateProperty("Destination", EffectPropertyType.Image, null, this.m_nDestinationID, dictProperties))
                return false;
            dictProperties[this.GeneratePropertyPath("Destination")].IsDynamic = true;
            return this.GenerateProperty("Downsample", EffectPropertyType.Float, m_flDownsample, this.m_nDownsampleID, dictProperties) && this.GenerateProperty("UVOffset", EffectPropertyType.Vector2, m_UVOffset, this.m_nUVOffsetID, dictProperties);
        }

        internal override void AddCacheKey(ByteBuilder cacheKey)
        {
            base.AddCacheKey(cacheKey);
            this.GeneratePropertyCacheKey("Downsample", this.DownsampleID, this.Downsample, cacheKey);
            this.GeneratePropertyCacheKey("UVOffset", this.UVOffsetID, this.UVOffset, cacheKey);
        }
    }
}
