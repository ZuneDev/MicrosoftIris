// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.ColorElement
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Graphics;

namespace Microsoft.Iris.Render
{
    public class ColorElement : EffectInput
    {
        internal const string ColorPropertyName = "Color";
        private ColorF m_clrColor;
        private byte m_nColorID;
        private static ColorF s_defaultColor = new ColorF(0, 0, 0, 0);

        public ColorElement(string stName, ColorF clrSolid)
        {
            this.m_stName = stName;
            this.m_typeInput = EffectInputType.Color;
            this.m_clrColor = clrSolid;
        }

        public ColorElement(string stName)
          : this(stName, DefaultColor)
        {
        }

        public ColorElement()
          : this("", DefaultColor)
        {
        }

        public ColorF Color
        {
            get => this.m_clrColor;
            set => this.m_clrColor = value;
        }

        internal byte ColorID => this.m_nColorID;

        internal static ColorF DefaultColor => s_defaultColor;

        internal override int PreProcessProperties(
          Map<string, EffectProperty> dictionary,
          ref byte nNextUniqueID)
        {
            return base.PreProcessProperties(dictionary, ref nNextUniqueID) + this.PreProcessProperty(dictionary, "Color", 20, ref this.m_nColorID, ref nNextUniqueID);
        }

        internal override bool Process(Map<string, EffectProperty> dictProperties) => this.GenerateProperty("Color", EffectPropertyType.Color, m_clrColor, this.m_nColorID, dictProperties);

        internal override void AddCacheKey(ByteBuilder cacheKey)
        {
            base.AddCacheKey(cacheKey);
            this.GeneratePropertyCacheKey("Color", this.ColorID, this.Color.ToVector4(), cacheKey);
        }
    }
}
