// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Drawing.Font
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

namespace Microsoft.Iris.Drawing
{
    internal sealed class Font
    {
        private string _fontName;
        private float _fontHeight;
        private float _altFontHeight;
        private FontStyles _fontStyle;

        public Font()
          : this("Arial", 12f, 0.0f, FontStyles.None)
        {
        }

        public Font(string fontName)
          : this(fontName, 12f, 0.0f, FontStyles.None)
        {
        }

        public Font(string fontName, float fontHeight)
          : this(fontName, fontHeight, 0.0f, FontStyles.None)
        {
        }

        public Font(string fontName, float fontHeight, float altFontHeight)
          : this(fontName, fontHeight, altFontHeight, FontStyles.None)
        {
        }

        public Font(string fontName, float fontHeight, FontStyles fontStyles)
          : this(fontName, fontHeight, 0.0f, fontStyles)
        {
        }

        public Font(string fontName, float fontHeight, float altFontHeight, FontStyles fontStyles)
        {
            _fontName = fontName;
            _fontHeight = fontHeight;
            _altFontHeight = altFontHeight;
            _fontStyle = fontStyles;
        }

        public float FontSize
        {
            get => _fontHeight;
            set => _fontHeight = value;
        }

        public float AltFontSize
        {
            get => _altFontHeight == 0.0 ? _fontHeight : _altFontHeight;
            set => _altFontHeight = value;
        }

        public FontStyles FontStyle
        {
            get => _fontStyle;
            set => _fontStyle = value;
        }

        public string FontName
        {
            get => _fontName;
            set => _fontName = value;
        }

        public override bool Equals(object obj) => obj is Font font && _fontName == font._fontName && (_fontHeight == (double)font._fontHeight && _altFontHeight == (double)font._altFontHeight) && _fontStyle == font._fontStyle;

        public override int GetHashCode() => _fontName.GetHashCode() ^ _fontHeight.GetHashCode() ^ _altFontHeight.GetHashCode() ^ _fontStyle.GetHashCode();

        public override string ToString() => $"{{Font \"{_fontName}\" {_fontHeight}pt {_fontStyle}}}";
    }
}
