// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Drawing.RichTextInfoKey
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.OS;
using Microsoft.Iris.Render;
using Microsoft.Iris.Render.Extensions;
using Microsoft.Iris.RenderAPI.Drawing;

namespace Microsoft.Iris.Drawing
{
    internal class RichTextInfoKey : ImageCacheKey
    {
        private const int FLAGS_IS_ITALIC = 1;
        private const int FLAGS_IS_UNDERLINE = 2;
        private const int FLAGS_IS_OUTLINE = 4;
        private const int FLAGS_IS_THICK_UNDERLINE = 8;
        private const int FLAGS_IS_DOTTED_UNDERLINE = 16;
        private string _samplingMode;
        private string _content;
        private SizeF _srcSizeF;
        private Size _naturalSize;
        private Point _rasterizedOffset;
        private int _fontFaceUniqueId;
        private int _fontSize;
        private int _fontWeight;
        private byte _rasterizerConfig;
        private Color _textColor;
        private int _flags;
        private int _hashCode;

        public RichTextInfoKey(TextRun run, string samplingMode, bool outline, Color textColor)
          : base(run.Content)
        {
            _samplingMode = samplingMode;
            _content = run.Content;
            _srcSizeF = run.RenderBounds.Size;
            _naturalSize = run.NaturalExtent;
            _rasterizedOffset = run.RasterizedOffset;
            _fontFaceUniqueId = run.FontFaceUniqueId;
            _fontSize = run.FontSize;
            _fontWeight = run.FontWeight;
            _rasterizerConfig = run.RasterizerConfig;
            _textColor = textColor;
            _flags = 0;
            if (run.Italic)
                _flags |= 1;
            if (outline)
                _flags |= 4;
            switch (run.UnderlineStyle)
            {
                case NativeApi.UnderlineStyle.Solid:
                    _flags |= 2;
                    break;
                case NativeApi.UnderlineStyle.Thick:
                    _flags |= 8;
                    break;
                case NativeApi.UnderlineStyle.Dotted:
                case NativeApi.UnderlineStyle.Dash:
                case NativeApi.UnderlineStyle.DashDot:
                case NativeApi.UnderlineStyle.DashDotDot:
                    _flags |= 16;
                    break;
            }
            _hashCode = _samplingMode.GetHashCode() ^ _content.GetHashCode() ^ _srcSizeF.GetHashCode() ^ _naturalSize.GetHashCode() ^ _rasterizedOffset.GetHashCode() ^ _fontFaceUniqueId.GetHashCode() ^ _fontSize ^ _fontWeight ^ _flags ^ _rasterizerConfig ^ _textColor.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;
            return obj is RichTextInfoKey richTextInfoKey && _hashCode == richTextInfoKey._hashCode && (_fontFaceUniqueId == richTextInfoKey._fontFaceUniqueId && _fontSize == richTextInfoKey._fontSize) && (_fontWeight == richTextInfoKey._fontWeight && _flags == richTextInfoKey._flags && (_rasterizerConfig == richTextInfoKey._rasterizerConfig && _samplingMode.Equals(richTextInfoKey._samplingMode))) && (_srcSizeF.Equals(richTextInfoKey._srcSizeF) && _naturalSize.Equals(richTextInfoKey._naturalSize) && (_rasterizedOffset.Equals(richTextInfoKey._rasterizedOffset) && _textColor.Equals(richTextInfoKey._textColor))) && _content.Equals(richTextInfoKey._content);
        }

        public override int GetHashCode() => _hashCode;
    }
}
