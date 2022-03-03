// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.EdgeDetectionElement
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Graphics;
using System;

namespace Microsoft.Iris.Render
{
    public class EdgeDetectionElement : EffectOperation
    {
        internal const string EdgeLimitPropertyName = "EdgeLimit";
        private float m_flEdgeLimit;
        private byte m_nEdgeLimitID;

        public EdgeDetectionElement(string stName, float flEdgeLimit)
          : this()
        {
            Debug2.Validate(!string.IsNullOrEmpty(stName), typeof(ArgumentException), nameof(stName));
            this.m_stName = stName;
            this.m_flEdgeLimit = flEdgeLimit;
        }

        public EdgeDetectionElement() => this.m_typeOperation = EffectOperationType.EdgeDetection;

        public float EdgeLimit
        {
            get => this.m_flEdgeLimit;
            set => this.m_flEdgeLimit = value;
        }

        internal byte EdgeLimitID => this.m_nEdgeLimitID;

        internal override int PreProcessProperties(
          Map<string, EffectProperty> dictionary,
          ref byte nNextUniqueID)
        {
            return base.PreProcessProperties(dictionary, ref nNextUniqueID) + this.PreProcessProperty(dictionary, "EdgeLimit", 8, ref this.m_nEdgeLimitID, ref nNextUniqueID);
        }

        internal override bool Process(Map<string, EffectProperty> dictProperties) => this.GenerateProperty("EdgeLimit", EffectPropertyType.Float, m_flEdgeLimit, this.m_nEdgeLimitID, dictProperties);

        internal override void AddCacheKey(ByteBuilder cacheKey)
        {
            base.AddCacheKey(cacheKey);
            this.GeneratePropertyCacheKey("EdgeLimit", this.EdgeLimitID, this.EdgeLimit, cacheKey);
        }
    }
}
