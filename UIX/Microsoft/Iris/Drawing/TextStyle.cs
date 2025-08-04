// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Drawing.TextStyle
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.Iris.Drawing
{
    [Serializable]
    internal class TextStyle : ISerializable
    {
        private BitVector32 _flags;
        private string _fontFace;
        private float _fontHeightPts;
        private float _altFontHeightPts;
        private float _lineSpacing;
        private float _characterSpacing;
        private Color _textColor;
        private bool _fragment;

        public TextStyle() { }

        protected TextStyle(SerializationInfo info, StreamingContext context)
        {
            FontFace = info.GetString(nameof(FontFace));
            FontSize = info.GetSingle(nameof(FontSize));
            LineSpacing = info.GetSingle(nameof(LineSpacing));
            Color = (Color)info.GetValue(nameof(Color), typeof(Color));

            _flags = new(info.GetInt32("flags"));
        }

        public bool IsInitialized() => _flags.Data != 0;

        public string FontFace
        {
            get => _fontFace;
            set
            {
                _flags[1] = !string.IsNullOrEmpty(value);
                _fontFace = value;
            }
        }

        public float FontSize
        {
            get => _fontHeightPts;
            set
            {
                _flags[2] = true;
                _fontHeightPts = value;
            }
        }

        public float AltFontSize
        {
            get => _altFontHeightPts == 0.0 ? _fontHeightPts : _altFontHeightPts;
            set
            {
                _flags[512] = true;
                _altFontHeightPts = value;
            }
        }

        public bool Bold
        {
            get => _flags[65536];
            set
            {
                _flags[4] = true;
                _flags[65536] = value;
            }
        }

        public bool Italic
        {
            get => _flags[131072];
            set
            {
                _flags[8] = true;
                _flags[131072] = value;
            }
        }

        public bool Underline
        {
            get => _flags[262144];
            set
            {
                _flags[16] = true;
                _flags[262144] = value;
            }
        }

        public Color Color
        {
            get => _textColor;
            set
            {
                _flags[64] = true;
                _textColor = value;
            }
        }

        public float LineSpacing
        {
            get => _lineSpacing;
            set
            {
                _flags[32] = true;
                _lineSpacing = value;
            }
        }

        public bool EnableKerning
        {
            get => _flags[524288];
            set
            {
                _flags[128] = true;
                _flags[524288] = value;
            }
        }

        public float CharacterSpacing
        {
            get => _characterSpacing;
            set
            {
                _flags[256] = true;
                _characterSpacing = value;
            }
        }

        public bool Fragment
        {
            get => _fragment;
            set => _fragment = value;
        }

        public void Add(TextStyle additional)
        {
            _fragment = additional._fragment;
            if (additional._flags[1])
                FontFace = additional.FontFace;
            if (additional._flags[2])
                FontSize = additional.FontSize;
            if (additional._flags[512])
                AltFontSize = additional._altFontHeightPts;
            if (additional._flags[4])
                Bold = additional.Bold;
            if (additional._flags[8])
                Italic = additional.Italic;
            if (additional._flags[16])
                Underline = additional.Underline;
            if (additional._flags[32])
                LineSpacing = additional.LineSpacing;
            if (additional._flags[128])
                EnableKerning = additional.EnableKerning;
            if (additional._flags[256])
                CharacterSpacing = additional.CharacterSpacing;
            if (additional._flags[64])
                Color = additional.Color;
        }

        public bool HasColor => _flags[64];

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("{TextStyle");
            if (_flags[1])
            {
                stringBuilder.Append(" Font = \"");
                stringBuilder.Append(FontFace);
                stringBuilder.Append("\"");
            }
            if (_flags[2])
            {
                stringBuilder.Append(" Pt = ");
                stringBuilder.Append(FontSize);
            }
            if (_flags[4])
            {
                stringBuilder.Append(" Bold = ");
                stringBuilder.Append(Bold);
            }
            if (_flags[8])
            {
                stringBuilder.Append(" Italic = ");
                stringBuilder.Append(Italic);
            }
            if (_flags[16])
            {
                stringBuilder.Append(" Underline = ");
                stringBuilder.Append(Underline);
            }
            if (_flags[32])
            {
                stringBuilder.Append(" LineSpacing = ");
                stringBuilder.Append(LineSpacing);
            }
            if (_flags[64])
            {
                stringBuilder.Append(" Color = ");
                stringBuilder.Append(Color);
            }
            stringBuilder.Append(" }");
            return stringBuilder.ToString();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(FontFace), FontFace);
            info.AddValue(nameof(FontSize), FontSize);
            info.AddValue(nameof(LineSpacing), LineSpacing);
            info.AddValue(nameof(Color), Color);

            info.AddValue("flags", _flags.Data);
        }

        internal string TruncatedFontFace => _fontFace.Length < 32 ? _fontFace : _fontFace.Substring(0, 31);

        [Flags]
        internal enum SetFlags
        {
            None = 0,
            FontFace = 1,
            FontHeight = 2,
            Bold = 4,
            Italic = 8,
            Underline = 16, // 0x00000010
            LineSpacing = 32, // 0x00000020
            TextColor = 64, // 0x00000040
            EnableKerning = 128, // 0x00000080
            CharacterSpacing = 256, // 0x00000100
            AltFontHeight = 512, // 0x00000200
            BoldValue = 65536, // 0x00010000
            ItalicValue = 131072, // 0x00020000
            UnderlineValue = 262144, // 0x00040000
            EnableKerningValue = 524288, // 0x00080000
        }

        internal struct MarshalledData
        {
            public int _flags;
            public unsafe char* _fontFace;
            public float _fontHeightPts;
            public float _altFontHeightPts;
            public float _lineSpacing;
            public float _characterSpacing;
            public Color _textColor;

            public unsafe MarshalledData(TextStyle from)
            {
                _flags = from._flags.Data;
                _fontHeightPts = from._fontHeightPts;
                _altFontHeightPts = from._altFontHeightPts;
                _lineSpacing = from._lineSpacing;
                _characterSpacing = from._characterSpacing;
                _textColor = from._textColor;
                _fontFace = null;
            }
        }
    }
}
