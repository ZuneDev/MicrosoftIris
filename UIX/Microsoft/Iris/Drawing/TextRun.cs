// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Drawing.TextRun
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;
using Microsoft.Iris.OS;
using Microsoft.Iris.Render;
using Microsoft.Iris.RenderAPI.Drawing;
using System;

namespace Microsoft.Iris.Drawing
{
    internal sealed class TextRun : SharedDisposableObject
    {
        private const int CFE_LINK = 32;
        private IntPtr _hGlyphRunInfo;
        private IntPtr _hRasterizeRunPacket;
        private Rectangle _layoutBounds;
        private RectangleF _renderBounds;
        private Point _offsetPoint;
        private int _naturalX;
        private int _naturalY;
        private int _rasterizeX;
        private int _rasterizeY;
        private byte _rasterizerConfig;
        private byte _bits;
        private string _content;
        private Color _runColor;
        private Color _overrideColor;
        private Color _highlightColor;
        private int _fontFaceUniqueId;
        public int _lfHeight;
        public int _lfWeight;
        private Size _naturalRunExtent;
        private int _lineNumber;
        private int _ascenderInset;
        private int _baselineInset;
        private Rectangle _underlineBounds;
        public ISprite TextSprite;
        public ISprite HighlightSprite;

        private unsafe TextRun(
          IntPtr hGlyphRunInfo,
          NativeApi.RasterizeRunPacket* runPacketPtr,
          string content)
        {
            _hGlyphRunInfo = hGlyphRunInfo;
            _hRasterizeRunPacket = new IntPtr(runPacketPtr);
            _layoutBounds = runPacketPtr->rcLayoutBounds;
            _renderBounds = runPacketPtr->rcfRenderBounds;
            _naturalX = runPacketPtr->naturalX;
            _naturalY = runPacketPtr->naturalY;
            _rasterizeX = runPacketPtr->rasterizeX;
            _rasterizeY = runPacketPtr->rasterizeY;
            _rasterizerConfig = runPacketPtr->AAConfig;
            _runColor = runPacketPtr->clrText;
            _overrideColor = Color.Transparent;
            _highlightColor = runPacketPtr->clrBackground;
            _fontFaceUniqueId = runPacketPtr->fontFaceUniqueId;
            _lfHeight = runPacketPtr->lf.lfHeight;
            _lfWeight = runPacketPtr->lf.lfWeight;
            SetBit(Bits.Italic, runPacketPtr->lf.lfItalic != 0);
            SetBit(Bits.Underline, runPacketPtr->lf.lfUnderline != 0);
            SetBit(Bits.Link, (runPacketPtr->dwEffects & 32) != 0);
            _underlineBounds = runPacketPtr->rcUnderlineBounds;
            _lineNumber = runPacketPtr->nLineNumber;
            _naturalRunExtent = runPacketPtr->sizeNatural;
            _ascenderInset = runPacketPtr->ascenderInset;
            _baselineInset = runPacketPtr->baselineInset;
            _content = content;
        }

        protected override void OnDispose()
        {
            NativeApi.SpRichTextDestroyGlyphRunInfo(_hGlyphRunInfo);
            base.OnDispose();
        }

        public string Content => _content;

        public RectangleF RenderBounds => _renderBounds;

        public Rectangle LayoutBounds => _layoutBounds;

        public Size NaturalExtent => _naturalRunExtent;

        public Color RunColor => _runColor;

        public Color OverrideColor
        {
            get => _overrideColor;
            set => _overrideColor = value;
        }

        public Color Color => _overrideColor != Color.Transparent ? _overrideColor : _runColor;

        public bool Highlighted => _highlightColor.A != 0;

        public Color HighlightColor => _highlightColor;

        public int FontFaceUniqueId => _fontFaceUniqueId;

        public int FontSize => _lfHeight;

        public int FontWeight => _lfWeight;

        public bool Italic => GetBit(Bits.Italic);

        public bool Underline => GetBit(Bits.Underline);

        public Point RasterizedOffset => new Point(_rasterizeX - _naturalX, _rasterizeY - _naturalY);

        public int AscenderInset => _ascenderInset;

        public int BaselineInset => _baselineInset;

        public int Line => _lineNumber;

        public bool Visible
        {
            get => GetBit(Bits.Visible);
            set => SetBit(Bits.Visible, value);
        }

        public bool IsFragment
        {
            get => GetBit(Bits.Fragment);
            set => SetBit(Bits.Fragment, value);
        }

        public Point Position => new Point(_layoutBounds.X, _layoutBounds.Y);

        public Size Size => new Size(_layoutBounds.Width, _layoutBounds.Height);

        public byte RasterizerConfig => _rasterizerConfig;

        public bool Link => GetBit(Bits.Link);

        public unsafe NativeApi.UnderlineStyle UnderlineStyle
        {
            get => _hRasterizeRunPacket == IntPtr.Zero ? NativeApi.UnderlineStyle.None : ((NativeApi.RasterizeRunPacket*)(void*)_hRasterizeRunPacket)->usUnderlineStyle;
            set
            {
                if (!(_hRasterizeRunPacket != IntPtr.Zero))
                    return;
                ((NativeApi.RasterizeRunPacket*)(void*)_hRasterizeRunPacket)->usUnderlineStyle = value;
            }
        }

        public Rectangle UnderlineBounds => _underlineBounds;

        internal void ApplyOffset(Point offsetPoint)
        {
            if (!(_offsetPoint != offsetPoint))
                return;
            _renderBounds.X -= _offsetPoint.X;
            _renderBounds.Y -= _offsetPoint.Y;
            _layoutBounds.X -= _offsetPoint.X;
            _layoutBounds.Y -= _offsetPoint.Y;
            _renderBounds.X += offsetPoint.X;
            _renderBounds.Y += offsetPoint.Y;
            _layoutBounds.X += offsetPoint.X;
            _layoutBounds.Y += offsetPoint.Y;
            _offsetPoint = offsetPoint;
        }

        internal Dib Rasterize(string samplingMode, Color textColor, bool outlineFlag)
        {
            bool shadowMode = false;
            if (samplingMode == "sdw")
                shadowMode = true;
            return RichText.Rasterize(_hGlyphRunInfo, outlineFlag, textColor, shadowMode);
        }

        internal static unsafe TextRun FromRunPacket(
          IntPtr hGlyphRunInfo,
          NativeApi.RasterizeRunPacket* runPacketPtr,
          string content)
        {
            return new TextRun(hGlyphRunInfo, runPacketPtr, content);
        }

        public void RemoveSprites(IVisualContainer container)
        {
            if (TextSprite != null)
            {
                container.RemoveChild(TextSprite);
                TextSprite = null;
            }
            if (HighlightSprite == null)
                return;
            container.RemoveChild(HighlightSprite);
            HighlightSprite = null;
        }

        private bool GetBit(TextRun.Bits lookupBit) => ((TextRun.Bits)_bits & lookupBit) != 0;

        private void SetBit(TextRun.Bits changeBit, bool value) => _bits = value ? (byte)((TextRun.Bits)_bits | changeBit) : (byte)((TextRun.Bits)_bits & ~changeBit);

        private enum Bits : byte
        {
            Visible = 1,
            Fragment = 2,
            Italic = 4,
            Underline = 8,
            Link = 16, // 0x10
        }
    }
}
