using System;
using System.Text;
using Microsoft.Iris.OS;
using Microsoft.Iris.Render;
using Microsoft.Iris.RenderAPI;
using Microsoft.Iris.Render.Text;
using Microsoft.Iris.Session;
using Microsoft.Iris.ViewItems;
using Size = Microsoft.Iris.Render.Size;
using Color = Microsoft.Iris.Drawing.Color;
using HRESULT = Microsoft.Iris.Render.Internal.HRESULT;

namespace Microsoft.Iris.Drawing;

// Windows backend for Microsoft.Iris.Render.Text.TextDocument - see the
// remark on SpFontResource for why this lives in UIX rather than
// UIX.RenderApi. Registered with TextDocumentFactory by TextBackendRegistration.
//
// Wraps one of the two native text object families NativeApi exposes:
//  - "standalone" wraps its own SpSimpleTextBuildObject handle (STO), exactly
//    what Microsoft.Iris.Drawing.SimpleText used before this refactor. Only
//    Measure/MeasurePossible/Rasterize are meaningful for an STO.
//  - "bound" is constructed with an existing RichText RTO handle so
//    SetContent/GetSimpleContent/GetNaturalBounds run against RichText's own
//    session. Measure/MeasurePossible are NOT implemented in bound mode:
//    RichText.Measure needs TextMeasureParams' multi-range formatting, which
//    this simplified single-style abstraction can't safely replicate without
//    regressing existing Windows behavior - RichText keeps calling
//    NativeApi.SpRichTextMeasure directly for that. See logs/ for the
//    rationale.
internal sealed class SpTextDocument : TextDocument
{
    private Win32Api.HANDLE _handle;
    private readonly bool _bound;

    public SpTextDocument()
    {
        var sizeMaximumSurface = Size.Zero;
        if (UISession.Default != null)
            sizeMaximumSurface = UIImage.MaximumSurfaceSize(UISession.Default);
        RendererApi.IFC(NativeApi.SpSimpleTextBuildObject(sizeMaximumSurface, out _handle));
        _bound = false;
    }

    public SpTextDocument(Win32Api.HANDLE boundRtoHandle)
    {
        _handle = boundRtoHandle;
        _bound = true;
    }

    public override HRESULT SetContent(string content)
    {
        RequireBound();
        return NativeApi.SpRichTextSetContent(_handle, content).Int;
    }

    public override HRESULT GetSimpleContent(out string content)
    {
        RequireBound();

        var hresult = NativeApi.SpRichTextGetSimpleContentLength(_handle, out var textLength);
        if (!hresult.IsSuccess() || textLength == 0)
        {
            content = null;
            return hresult.Int;
        }

        var textBuffer = new StringBuilder(textLength);
        hresult = NativeApi.SpRichTextGetSimpleContent(_handle, textBuffer, textBuffer.Capacity);
        content = hresult.IsSuccess() ? textBuffer.ToString() : null;
        return hresult.Int;
    }

    public override HRESULT GetNaturalBounds(out Size bounds)
    {
        RequireBound();

        var hresult = NativeApi.SpRichTextGetNaturalBounds(_handle, out var cWidth, out var cHeight);
        bounds = hresult.IsSuccess() ? new Size(cWidth, cHeight) : Size.Zero;
        return hresult.Int;
    }

    public override unsafe HRESULT MeasurePossible(string content, TextStyleInfo style, out bool possible)
    {
        RequireStandalone();

        fixed (char* facePtr = TruncateFontFace(style?.FontFace))
        {
            var marshalled = ToMarshalledData(style, facePtr);
            return NativeApi.SpSimpleTextMeasurePossible(_handle, content, &marshalled, out possible).Int;
        }
    }

    public override unsafe HRESULT Measure(string content, TextAlignment alignment, TextStyleInfo style, Size constraint, out GlyphRunInfo glyphRun)
    {
        RequireStandalone();

        content ??= string.Empty;
        short wAlignment = alignment switch
        {
            TextAlignment.Near => 1,
            TextAlignment.Center => 3,
            TextAlignment.Far => 2,
            _ => 0,
        };

        IntPtr hGlyphRunInfo;
        NativeApi.RasterizeRunPacket rasterizeRunPacket;
        Microsoft.Iris.RenderAPI.HRESULT hresult;
        fixed (char* facePtr = TruncateFontFace(style?.FontFace))
        {
            var marshalled = ToMarshalledData(style, facePtr);
            hresult = NativeApi.SpSimpleTextMeasure(_handle, content, wAlignment, &marshalled, constraint, out hGlyphRunInfo, &rasterizeRunPacket);
        }

        if (!hresult.IsSuccess())
        {
            glyphRun = null;
            return hresult.Int;
        }

        glyphRun = FromRasterizeRunPacket(hGlyphRunInfo, ref rasterizeRunPacket, content);
        return HRESULT.S_OK;
    }

    public override unsafe HRESULT Rasterize(GlyphRunInfo glyphRun, ColorF textColor, bool outline, bool shadow, out RasterizedGlyphBitmap bitmap)
    {
        if (glyphRun?.BackendHandle is not IntPtr hGlyphRunInfo)
        {
            bitmap = null;
            return HRESULT.E_FAIL;
        }

        var textColorArgb = new Color(textColor.A, textColor.R, textColor.G, textColor.B);
        var hresult = NativeApi.SpRichTextRasterize(hGlyphRunInfo, outline ? 1 : 0, textColorArgb, shadow ? 1 : 0,
            out var phTextBitmap, out var ppvBits, out var psizeBitmap);

        if (!hresult.IsSuccess())
        {
            bitmap = null;
            return hresult.Int;
        }

        bitmap = new RasterizedGlyphBitmap(() => NativeApi.SpFreeDib(phTextBitmap))
        {
            Size = psizeBitmap,
            Bits = ppvBits,
            NativeHandle = phTextBitmap,
        };
        return HRESULT.S_OK;
    }

    public override void Dispose()
    {
        if (_bound)
            return;

        if (_handle == Win32Api.HANDLE.NULL)
            return;

        NativeApi.SpSimpleTextDestroyObject(_handle);
        _handle = Win32Api.HANDLE.NULL;
    }

    private void RequireBound()
    {
        if (!_bound)
            throw new NotSupportedException("Standalone (SimpleText) text documents have no persistent content buffer.");
    }

    private void RequireStandalone()
    {
        if (_bound)
            throw new NotSupportedException("Bound (RichText) text documents don't support single-style Measure - RichText.Measure uses NativeApi.SpRichTextMeasure directly to keep multi-range formatting.");
    }

    private static GlyphRunInfo FromRasterizeRunPacket(IntPtr hGlyphRunInfo, ref NativeApi.RasterizeRunPacket run, string content)
    {
        return new GlyphRunInfo(handle => NativeApi.SpRichTextDestroyGlyphRunInfo((IntPtr)handle))
        {
            Content = content,
            LayoutBounds = run.rcLayoutBounds,
            RenderBoundsX = run.rcfRenderBounds.X,
            RenderBoundsY = run.rcfRenderBounds.Y,
            RenderBoundsWidth = run.rcfRenderBounds.Width,
            RenderBoundsHeight = run.rcfRenderBounds.Height,
            NaturalExtent = run.sizeNatural,
            NaturalX = run.naturalX,
            NaturalY = run.naturalY,
            RasterizeX = run.rasterizeX,
            RasterizeY = run.rasterizeY,
            RasterizerConfig = run.AAConfig,
            RunColor = run.clrText.RenderConvert(),
            HighlightColor = run.clrBackground.RenderConvert(),
            FontFaceUniqueId = run.fontFaceUniqueId,
            FontHeight = run.lf.lfHeight,
            FontWeight = run.lf.lfWeight,
            Italic = run.lf.lfItalic != 0,
            Underline = run.lf.lfUnderline != 0,
            Link = (run.dwEffects & 32) != 0,
            UnderlineStyle = (Microsoft.Iris.Render.Text.UnderlineStyle)run.usUnderlineStyle,
            UnderlineBounds = run.rcUnderlineBounds,
            Line = run.nLineNumber,
            AscenderInset = run.ascenderInset,
            BaselineInset = run.baselineInset,
            BackendHandle = hGlyphRunInfo,
        };
    }

    private static string TruncateFontFace(string fontFace)
    {
        fontFace ??= string.Empty;
        return fontFace.Length < 32 ? fontFace : fontFace.Substring(0, 31);
    }

    private static unsafe TextStyle.MarshalledData ToMarshalledData(TextStyleInfo style, char* fontFacePtr)
    {
        style ??= new TextStyleInfo();

        var flags = TextStyle.SetFlags.FontFace | TextStyle.SetFlags.FontHeight | TextStyle.SetFlags.Bold |
                     TextStyle.SetFlags.Italic | TextStyle.SetFlags.Underline | TextStyle.SetFlags.LineSpacing |
                     TextStyle.SetFlags.EnableKerning | TextStyle.SetFlags.CharacterSpacing;
        if (style.HasColor)
            flags |= TextStyle.SetFlags.TextColor;
        if (style.AltFontSize != 0)
            flags |= TextStyle.SetFlags.AltFontHeight;
        if (style.Bold)
            flags |= TextStyle.SetFlags.BoldValue;
        if (style.Italic)
            flags |= TextStyle.SetFlags.ItalicValue;
        if (style.Underline)
            flags |= TextStyle.SetFlags.UnderlineValue;
        if (style.EnableKerning)
            flags |= TextStyle.SetFlags.EnableKerningValue;

        return new TextStyle.MarshalledData
        {
            _flags = (int)flags,
            _fontFace = fontFacePtr,
            _fontHeightPts = style.FontSize,
            _altFontHeightPts = style.AltFontSize,
            _lineSpacing = style.LineSpacing,
            _characterSpacing = style.CharacterSpacing,
            _textColor = new Color(style.Color.A, style.Color.R, style.Color.G, style.Color.B),
        };
    }
}
