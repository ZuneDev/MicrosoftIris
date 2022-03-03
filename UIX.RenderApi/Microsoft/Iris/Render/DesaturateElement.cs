// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.DesaturateElement
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Graphics;
using System;

namespace Microsoft.Iris.Render
{
    public class DesaturateElement : EffectOperation
    {
        internal const string DesaturatePropertyName = "Desaturate";
        private float m_flDesaturate;
        private byte m_nDesaturateID;

        public DesaturateElement(string stName, float flDesaturate)
          : this()
        {
            Debug2.Validate(!string.IsNullOrEmpty(stName), typeof(ArgumentException), nameof(stName));
            Debug2.Validate(flDesaturate >= 0.0 && flDesaturate <= 1.0, typeof(ArgumentOutOfRangeException), "Valid range for desaturate is [0,1]");
            this.m_flDesaturate = flDesaturate;
            this.m_stName = stName;
        }

        public DesaturateElement() => this.m_typeOperation = EffectOperationType.Desaturate;

        public float Desaturate
        {
            get => this.m_flDesaturate;
            set => this.m_flDesaturate = value;
        }

        internal byte DesaturateID => this.m_nDesaturateID;

        internal override int PreProcessProperties(
          Map<string, EffectProperty> dictionary,
          ref byte nNextUniqueID)
        {
            return base.PreProcessProperties(dictionary, ref nNextUniqueID) + this.PreProcessProperty(dictionary, "Desaturate", 8, ref this.m_nDesaturateID, ref nNextUniqueID);
        }

        internal override bool Process(Map<string, EffectProperty> dictProperties) => this.GenerateProperty("Desaturate", EffectPropertyType.Float, m_flDesaturate, this.m_nDesaturateID, dictProperties);

        internal override void AddCacheKey(ByteBuilder cacheKey)
        {
            base.AddCacheKey(cacheKey);
            this.GeneratePropertyCacheKey("Desaturate", this.DesaturateID, this.Desaturate, cacheKey);
        }
    }
}
