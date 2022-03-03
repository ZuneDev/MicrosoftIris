// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Drawing.TextMeasureParams
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.RenderAPI.Drawing;
using Microsoft.Iris.Session;
using Microsoft.Iris.ViewItems;
using System;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Drawing
{
    internal struct TextMeasureParams
    {
        public TextMeasureParams.MarshalledData _data;
        public TextStyle _textStyle;
        public TextMeasureParams.FormattedRange[] _formattedRanges;
        public TextStyle.MarshalledData[] _formattedRangeStyles;
        private GCHandle[] _formattedRangeStyleFontFaces;

        public void Initialize()
        {
            if (!UISession.Default.IsRtl)
                return;
            _data._flags |= MeasureFlags.IsRtl;
        }

        public void Dispose()
        {
            if (_formattedRangeStyleFontFaces == null)
                return;
            foreach (GCHandle rangeStyleFontFace in _formattedRangeStyleFontFaces)
            {
                if (rangeStyleFontFace.IsAllocated)
                    rangeStyleFontFace.Free();
            }
            _formattedRangeStyleFontFaces = null;
        }

        public unsafe void SetContent(char* content)
        {
            _data._flags |= MeasureFlags.Content;
            _data._content = content;
        }

        public void SetFormat(LineAlignment lineAlignment, TextStyle style)
        {
            switch (lineAlignment)
            {
                case LineAlignment.Near:
                    _data._alignment = 1;
                    break;
                case LineAlignment.Center:
                    _data._alignment = 3;
                    break;
                case LineAlignment.Far:
                    _data._alignment = 2;
                    break;
            }
            _textStyle = style;
        }

        public void TrimLeftSideBearing() => _data._flags |= MeasureFlags.TrimLeftSideBearing;

        public void SetEditMode(bool inEditMode)
        {
            if (inEditMode)
                _data._flags |= MeasureFlags.FormatOnly;
            else
                _data._flags &= ~MeasureFlags.FormatOnly;
        }

        public void SetScale(float scale) => _data._scale = scale;

        public void SetWordWrap(bool wordWrap)
        {
            _data._flags |= MeasureFlags.WordWrap;
            if (wordWrap)
                _data._flags |= MeasureFlags.WordWrapValue;
            else
                _data._flags &= ~MeasureFlags.WordWrapValue;
        }

        public void SetPasswordChar(char passwordChar)
        {
            _data._flags |= MeasureFlags.PasswordMasked;
            _data._passwordChar = passwordChar;
        }

        public void SetConstraint(SizeF constraint) => _data._constraint = constraint;

        public void AllocateFormattedRanges(int formattedRangeCount, int formattedRangeStylesCount)
        {
            _formattedRanges = new TextMeasureParams.FormattedRange[formattedRangeCount];
            _formattedRangeStyles = new TextStyle.MarshalledData[formattedRangeStylesCount];
            _formattedRangeStyleFontFaces = new GCHandle[formattedRangeStylesCount];
            _data._formattedRangeCount = formattedRangeCount;
            _data._formattedRangeStylesCount = formattedRangeStylesCount;
        }

        public TextMeasureParams.FormattedRange[] FormattedRanges => _formattedRanges;

        public unsafe void SetFormattedRangeStyle(int index, TextStyle style)
        {
            _formattedRangeStyles[index] = new TextStyle.MarshalledData(style);
            string fontFace = style.FontFace;
            if (fontFace == null)
                return;
            GCHandle gcHandle = GCHandle.Alloc(fontFace, GCHandleType.Pinned);
            _formattedRangeStyles[index]._fontFace = (char*)gcHandle.AddrOfPinnedObject().ToPointer();
            _formattedRangeStyleFontFaces[index] = gcHandle;
        }

        internal struct MarshalledData
        {
            public TextMeasureParams.MeasureFlags _flags;
            public byte _alignment;
            [MarshalAs(UnmanagedType.U2)]
            public char _passwordChar;
            public unsafe char* _content;
            public float _scale;
            public SizeF _constraint;
            public unsafe TextStyle.MarshalledData* _pTextStyle;
            public int _formattedRangeCount;
            public unsafe TextMeasureParams.FormattedRange* _pFormattedRanges;
            public int _formattedRangeStylesCount;
            public unsafe TextStyle.MarshalledData* _pFormattedRangeStyles;
        }

        [Flags]
        internal enum MeasureFlags : byte
        {
            None = 0,
            Content = 1,
            IsRtl = 2,
            WordWrap = 4,
            WordWrapValue = 8,
            PasswordMasked = 16, // 0x10
            TrimLeftSideBearing = 32, // 0x20
            FormatOnly = 64, // 0x40
        }

        internal struct FormattedRange
        {
            public int FirstCharacter;
            public int LastCharacter;
            public Color Color;
            public int StyleIndex;
        }
    }
}
