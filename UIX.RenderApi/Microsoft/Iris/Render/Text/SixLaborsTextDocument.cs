using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Iris.Render.Internal;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Color = SixLabors.ImageSharp.Color;

namespace Microsoft.Iris.Render.Text;

public sealed class SixLaborsTextDocument : TextDocument
{
    private string _content = string.Empty;
    private TextStyleInfo _lastStyle;

    public override HRESULT SetContent(string content)
    {
        _content = content ?? string.Empty;
        return HRESULT.S_OK;
    }

    public override HRESULT GetSimpleContent(out string content)
    {
        content = _content;
        return HRESULT.S_OK;
    }

    public override HRESULT GetNaturalBounds(out Size bounds)
    {
        var hresult = MeasureCore(_content, _lastStyle ?? DefaultStyle, Size.Zero, out var fontRect, out _);
        bounds = hresult.IsSuccess() ? new Size((int)MathF.Ceiling(fontRect.Width), (int)MathF.Ceiling(fontRect.Height)) : Size.Zero;
        return hresult;
    }

    public override HRESULT MeasurePossible(string content, TextStyleInfo style, out bool possible)
    {
        possible = TryResolveFont(style, out _);
        return HRESULT.S_OK;
    }

    public override HRESULT Measure(string content, TextAlignment alignment, TextStyleInfo style, Size constraint, out GlyphRunInfo glyphRun)
    {
        _lastStyle = style;
        var hresult = MeasureCore(content, style, constraint, out var fontRect, out var font);
        if (!hresult.IsSuccess())
        {
            glyphRun = null;
            return hresult;
        }

        var layoutBounds = new Rectangle((int)fontRect.X, (int)fontRect.Y, (int)MathF.Ceiling(fontRect.Width), (int)MathF.Ceiling(fontRect.Height));
        glyphRun = new GlyphRunInfo()
        {
            Content = content,
            LayoutBounds = layoutBounds,
            RenderBoundsX = fontRect.X,
            RenderBoundsY = fontRect.Y,
            RenderBoundsWidth = fontRect.Width,
            RenderBoundsHeight = fontRect.Height,
            NaturalExtent = new Size((int)MathF.Ceiling(fontRect.Width), (int)MathF.Ceiling(fontRect.Height)),
            RunColor = style.Color,
            HighlightColor = default,
            FontFaceUniqueId = style.FontFace?.GetHashCode() ?? 0,
            FontHeight = (int)style.FontSize,
            FontWeight = style.Bold ? 700 : 400,
            Italic = style.Italic,
            Underline = style.Underline,
            Link = false,
            UnderlineStyle = style.Underline ? UnderlineStyle.Solid : UnderlineStyle.None,
            UnderlineBounds = Rectangle.Zero,
            Line = 1,
            AscenderInset = 0,
            BaselineInset = 0,
            BackendHandle = font,
        };
        return HRESULT.S_OK;
    }

    // Formatted-range measurement backing RichText's non-hosted (display-only)
    // mode - see TextDocument.Measure's remarks. Resolves a per-range font via
    // SixLabors.Fonts.TextOptions.TextRuns (gaps between ranges automatically
    // fall back to the base font - see TextLayout.BuildTextRuns), then
    // reconstructs per-line, per-range GlyphRunInfo entries from
    // TextMeasurer.GetGraphemeMetrics so multi-line wrapped+formatted text
    // renders correctly (a single run can't represent a styled span that
    // wraps across lines).
    public override HRESULT Measure(string content, TextAlignment alignment, TextStyleInfo baseStyle, IReadOnlyList<TextStyleRun> formattedRanges, Size constraint, bool wordWrap, out GlyphRunInfo[] glyphRuns)
    {
        if (formattedRanges == null || formattedRanges.Count == 0)
        {
            var hresult = Measure(content, alignment, baseStyle, constraint, out var glyphRun);
            glyphRuns = hresult.IsSuccess() && glyphRun != null ? new[] { glyphRun } : Array.Empty<GlyphRunInfo>();
            return hresult;
        }

        content ??= string.Empty;
        baseStyle ??= DefaultStyle;
        _lastStyle = baseStyle;

        if (!TryResolveFont(baseStyle, out var baseFont))
        {
            glyphRuns = Array.Empty<GlyphRunInfo>();
            return 0x80070490; // ERROR_NOT_FOUND
        }

        var options = new TextOptions(baseFont);
        if (constraint.Width > 0 && wordWrap)
            options.WrappingLength = constraint.Width;

        var textRuns = new List<SixLabors.Fonts.TextRun>(formattedRanges.Count);
        foreach (var range in formattedRanges)
        {
            if (range == null || range.LastCharacter < range.FirstCharacter)
                continue;
            var start = Math.Clamp(range.FirstCharacter, 0, content.Length);
            var end = Math.Clamp(range.LastCharacter + 1, start, content.Length);
            if (start == end)
                continue;
            TryResolveFont(range.Style ?? baseStyle, out var runFont);
            textRuns.Add(new SixLabors.Fonts.TextRun { Start = start, End = end, Font = runFont ?? baseFont });
        }
        options.TextRuns = textRuns;

        var metrics = TextMeasurer.GetGraphemeMetrics(content, options).Span;
        glyphRuns = BuildFormattedGlyphRuns(content, metrics, baseStyle, formattedRanges);
        return HRESULT.S_OK;
    }

    private static GlyphRunInfo[] BuildFormattedGlyphRuns(string content, ReadOnlySpan<GraphemeMetrics> metrics, TextStyleInfo baseStyle, IReadOnlyList<TextStyleRun> formattedRanges)
    {
        if (metrics.Length == 0)
            return Array.Empty<GlyphRunInfo>();

        var result = new List<GlyphRunInfo>();
        int groupStart = 0;
        int line = 0;
        float lineY = metrics[0].Bounds.Y;
        int rangeIndex = FindRangeIndex(formattedRanges, metrics[0].StringIndex);

        for (int i = 1; i < metrics.Length; i++)
        {
            var g = metrics[i];
            var gRangeIndex = FindRangeIndex(formattedRanges, g.StringIndex);
            var newLine = g.Bounds.Y != lineY;
            if (newLine || gRangeIndex != rangeIndex)
            {
                FlushGlyphRun(result, metrics, content, formattedRanges, baseStyle, groupStart, i, line, rangeIndex);
                groupStart = i;
                rangeIndex = gRangeIndex;
                if (newLine)
                {
                    line++;
                    lineY = g.Bounds.Y;
                }
            }
        }
        FlushGlyphRun(result, metrics, content, formattedRanges, baseStyle, groupStart, metrics.Length, line, rangeIndex);

        return result.ToArray();
    }

    private static void FlushGlyphRun(List<GlyphRunInfo> result, ReadOnlySpan<GraphemeMetrics> metrics, string content, IReadOnlyList<TextStyleRun> formattedRanges, TextStyleInfo baseStyle, int groupStart, int endExclusive, int lineNumber, int currentRangeIndex)
    {
        if (endExclusive <= groupStart)
            return;

        var first = metrics[groupStart];
        var bounds = first.Bounds;
        for (int i = groupStart + 1; i < endExclusive; i++)
            bounds = FontRectangle.Union(bounds, metrics[i].Bounds);

        var last = metrics[endExclusive - 1];
        var startChar = first.StringIndex;
        var endChar = Math.Clamp(last.StringIndex + 1, startChar, content.Length);
        var runContent = startChar < endChar ? content.Substring(startChar, endChar - startChar) : string.Empty;
        var style = currentRangeIndex >= 0 ? (formattedRanges[currentRangeIndex].Style ?? baseStyle) : baseStyle;

        result.Add(new GlyphRunInfo
        {
            Content = runContent,
            LayoutBounds = new Rectangle((int)bounds.X, (int)bounds.Y, (int)MathF.Ceiling(bounds.Width), (int)MathF.Ceiling(bounds.Height)),
            RenderBoundsX = bounds.X,
            RenderBoundsY = bounds.Y,
            RenderBoundsWidth = bounds.Width,
            RenderBoundsHeight = bounds.Height,
            NaturalExtent = new Size((int)MathF.Ceiling(bounds.Width), (int)MathF.Ceiling(bounds.Height)),
            RunColor = style.Color,
            HighlightColor = default,
            FontFaceUniqueId = style.FontFace?.GetHashCode() ?? 0,
            FontHeight = (int)style.FontSize,
            FontWeight = style.Bold ? 700 : 400,
            Italic = style.Italic,
            Underline = style.Underline,
            Link = false,
            UnderlineStyle = style.Underline ? UnderlineStyle.Solid : UnderlineStyle.None,
            UnderlineBounds = Rectangle.Zero,
            Line = lineNumber,
            AscenderInset = 0,
            BaselineInset = 0,
            BackendHandle = first.Font,
        });
    }

    private static int FindRangeIndex(IReadOnlyList<TextStyleRun> ranges, int stringIndex)
    {
        for (int i = 0; i < ranges.Count; i++)
        {
            if (stringIndex >= ranges[i].FirstCharacter && stringIndex <= ranges[i].LastCharacter)
                return i;
        }
        return -1;
    }

    public override unsafe HRESULT Rasterize(GlyphRunInfo glyphRun, ColorF textColor, bool outline, bool shadow, out RasterizedGlyphBitmap bitmap)
    {
        if (glyphRun.BackendHandle is not Font font)
        {
            bitmap = null;
            return HRESULT.E_FAIL;
        }

        var width = Math.Max(1, (int)MathF.Ceiling(glyphRun.RenderBoundsWidth));
        var height = Math.Max(1, (int)MathF.Ceiling(glyphRun.RenderBoundsHeight));
        var color = Color.FromPixel(new Rgba32(
            (byte)Math.Clamp(textColor.R * 255f, 0, 255),
            (byte)Math.Clamp(textColor.G * 255f, 0, 255),
            (byte)Math.Clamp(textColor.B * 255f, 0, 255),
            (byte)Math.Clamp(textColor.A * 255f, 0, 255)));
        var brush = new SolidBrush(color);
        var richTextOptions = new RichTextOptions(font) { Origin = PointF.Empty };

        using var image = new Image<Rgba32>(width, height);
        image.Mutate(ctx => ctx.Paint(canvas => canvas.DrawText(richTextOptions, glyphRun.Content ?? string.Empty, brush, null)));

        var byteCount = width * height * 4;
        var pBits = Marshal.AllocHGlobal(byteCount);
        image.CopyPixelDataTo(new Span<byte>(pBits.ToPointer(), byteCount));

        var pBitsCaptured = pBits;
        bitmap = new RasterizedGlyphBitmap(() => Marshal.FreeHGlobal(pBitsCaptured))
        {
            Size = new Size(width, height),
            Bits = pBits,
            NativeHandle = IntPtr.Zero,
        };
        return HRESULT.S_OK;
    }

    public override void Dispose()
    {
    }

    private static TextStyleInfo DefaultStyle => new() { FontFace = "Arial", FontSize = 12f };

    private static bool TryResolveFont(TextStyleInfo style, out Font font)
    {
        font = null;
        if (!SixLaborsFontRegistry.TryGetFamily(style?.FontFace, out var family))
            return false;

        var fontStyle = (style is { Bold: true, Italic: true }) ? FontStyle.BoldItalic
            : style?.Bold == true ? FontStyle.Bold
            : style?.Italic == true ? FontStyle.Italic
            : FontStyle.Regular;

        var size = style?.FontSize > 0 ? style.FontSize : 12f;
        font = family.CreateFont(size, fontStyle);
        return true;
    }

    private static HRESULT MeasureCore(string content, TextStyleInfo style, Size constraint, out FontRectangle fontRect, out Font font)
    {
        content ??= string.Empty;
        if (!TryResolveFont(style, out font))
        {
            fontRect = default;
            return 0x80070490; // ERROR_NOT_FOUND
        }

        var options = new TextOptions(font);
        if (constraint.Width > 0)
            options.WrappingLength = constraint.Width;

        fontRect = TextMeasurer.MeasureBounds(content, options);
        return HRESULT.S_OK;
    }
}
