using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Iris.Render.Internal;
using Microsoft.Iris.Render.Monitors;
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
        // Line-break detection must use the positioned *advance* rectangle,
        // not the rendered glyph ink Bounds: Bounds is tight to each glyph's
        // visible pixels (e.g. a space has near-zero height, 'l' sits higher
        // than 'o'), so its Y bounces around within a single visual line and
        // would misfire a "new line" on almost every character. Advance.Y is
        // the cell position SixLabors.Fonts itself laid the grapheme into,
        // which is uniform for every grapheme on the same line (confirmed
        // against the shipped XML docs - Bounds is "rendered glyph bounds",
        // Advance is "positioned logical advance rectangle").
        float lineY = metrics[0].Advance.Y;
        int rangeIndex = FindRangeIndex(formattedRanges, metrics[0].StringIndex);

        for (int i = 1; i < metrics.Length; i++)
        {
            var g = metrics[i];
            var gRangeIndex = FindRangeIndex(formattedRanges, g.StringIndex);
            var newLine = g.Advance.Y != lineY;
            if (newLine || gRangeIndex != rangeIndex)
            {
                FlushGlyphRun(result, metrics, content, formattedRanges, baseStyle, groupStart, i, line, rangeIndex);
                groupStart = i;
                rangeIndex = gRangeIndex;
                if (newLine)
                {
                    line++;
                    lineY = g.Advance.Y;
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

    // Backs RichText's hosted (interactive-editing) mode on non-Windows: caret
    // placement after keyboard navigation. Windows never calls this - native
    // RichEdit tracks caret position internally and reports it via
    // IRichTextCallbacks.SetCaretPos - see TextDocument.GetCaretMetrics.
    public override HRESULT GetCaretMetrics(string content, TextStyleInfo style, Size constraint, bool wordWrap, int characterIndex, out Rectangle caretBounds)
    {
        content ??= string.Empty;
        style ??= DefaultStyle;
        if (!TryResolveFont(style, out var font))
        {
            caretBounds = Rectangle.Zero;
            return 0x80070490; // ERROR_NOT_FOUND
        }

        var fallbackLineHeight = (int)MathF.Ceiling(font.FontMetrics.HorizontalMetrics.LineHeight * font.Size / font.FontMetrics.UnitsPerEm);
        characterIndex = Math.Clamp(characterIndex, 0, content.Length);

        if (content.Length == 0)
        {
            caretBounds = new Rectangle(0, 0, 1, Math.Max(1, fallbackLineHeight));
            return HRESULT.S_OK;
        }

        var options = new TextOptions(font);
        if (constraint.Width > 0 && wordWrap)
            options.WrappingLength = constraint.Width;

        var linesSpan = TextMeasurer.GetLineMetrics(content, options).Span;
        var metrics = TextMeasurer.GetGraphemeMetrics(content, options).Span;
        if (linesSpan.Length == 0 || metrics.Length == 0)
        {
            caretBounds = new Rectangle(0, 0, 1, Math.Max(1, fallbackLineHeight));
            return HRESULT.S_OK;
        }

        var line = FindLine(linesSpan, characterIndex, content.Length);
        var lineEndExclusive = LineEndExclusive(linesSpan, line, content.Length);
        var height = Math.Max(1, (int)MathF.Ceiling(line.LineHeight));

        GraphemeMetrics? lastInLine = null;
        for (int i = 0; i < metrics.Length; i++)
        {
            if (metrics[i].StringIndex < line.StringIndex || metrics[i].StringIndex >= lineEndExclusive)
                continue;
            if (metrics[i].StringIndex == characterIndex)
            {
                caretBounds = new Rectangle((int)metrics[i].Advance.X, (int)line.Start.Y, 1, height);
                return HRESULT.S_OK;
            }
            lastInLine = metrics[i];
        }

        // Caret is past the last grapheme on this line (end of content, or
        // end of a wrapped line where trailing whitespace was trimmed) -
        // place it right after the last grapheme this line actually has.
        caretBounds = lastInLine is { } last
            ? new Rectangle((int)MathF.Ceiling(last.Advance.Right), (int)line.Start.Y, 1, height)
            : new Rectangle((int)line.Start.X, (int)line.Start.Y, 1, height);
        return HRESULT.S_OK;
    }

    // Backs RichText's hosted mode on non-Windows: mapping a mouse-click
    // point to a character index for caret placement / click-to-select.
    public override HRESULT HitTest(string content, TextStyleInfo style, Size constraint, bool wordWrap, Point point, out int characterIndex)
    {
        content ??= string.Empty;
        style ??= DefaultStyle;
        if (content.Length == 0 || !TryResolveFont(style, out var font))
        {
            characterIndex = 0;
            return HRESULT.S_OK;
        }

        var options = new TextOptions(font);
        if (constraint.Width > 0 && wordWrap)
            options.WrappingLength = constraint.Width;

        var linesSpan = TextMeasurer.GetLineMetrics(content, options).Span;
        var metrics = TextMeasurer.GetGraphemeMetrics(content, options).Span;
        if (linesSpan.Length == 0 || metrics.Length == 0)
        {
            characterIndex = 0;
            return HRESULT.S_OK;
        }

        // Find the visual line whose box is closest to the point's Y.
        var line = linesSpan[0];
        var bestDy = float.MaxValue;
        foreach (var candidate in linesSpan)
        {
            var dy = MathF.Abs(candidate.Start.Y + candidate.LineHeight / 2f - point.Y);
            if (dy < bestDy)
            {
                bestDy = dy;
                line = candidate;
            }
        }
        var lineEndExclusive = LineEndExclusive(linesSpan, line, content.Length);

        var resultIndex = line.StringIndex;
        for (int i = 0; i < metrics.Length; i++)
        {
            if (metrics[i].StringIndex < line.StringIndex || metrics[i].StringIndex >= lineEndExclusive)
                continue;
            var g = metrics[i];
            if (point.X < g.Advance.X + g.Advance.Width / 2f)
            {
                characterIndex = g.StringIndex;
                return HRESULT.S_OK;
            }
            resultIndex = g.StringIndex + 1;
        }
        characterIndex = Math.Clamp(resultIndex, 0, content.Length);
        return HRESULT.S_OK;
    }

    // Finds the laid-out line containing characterIndex: each line covers
    // [line.StringIndex, nextLine.StringIndex) except the last, which runs
    // to the end of the content (so the caret-at-end-of-text position
    // resolves to the last line).
    private static LineMetrics FindLine(ReadOnlySpan<LineMetrics> lines, int characterIndex, int contentLength)
    {
        for (int i = 0; i < lines.Length; i++)
        {
            var nextStart = i + 1 < lines.Length ? lines[i + 1].StringIndex : contentLength + 1;
            if (characterIndex >= lines[i].StringIndex && characterIndex < nextStart)
                return lines[i];
        }
        return lines[^1];
    }

    private static int LineEndExclusive(ReadOnlySpan<LineMetrics> lines, LineMetrics line, int contentLength)
    {
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].StringIndex != line.StringIndex)
                continue;
            return i + 1 < lines.Length ? lines[i + 1].StringIndex : contentLength + 1;
        }
        return contentLength + 1;
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

        // OpenGL is configured to use BGRA32
        using var image = new Image<Bgra32>(width, height);
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
        if (style is null)
            return false;
        
        if (!SixLaborsFontRegistry.TryGetFamily(style.FontFace, out var family))
            return false;

        var fontStyle = style switch
        {
            { Bold: true, Italic: true } => FontStyle.BoldItalic,
            { Bold: true } => FontStyle.Bold,
            { Italic: true } => FontStyle.Italic,
            _ => FontStyle.Regular,
        };
        
        var size = style.FontSize > 0 ? style.FontSize : 12f;
        font = family.CreateFont(size * 1.33f, fontStyle);
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

        var options = new TextOptions(font)
        {
            Dpi = MonitorSystem.Instance.GetDpi(),
        };

        if (constraint.Width > 0)
            options.WrappingLength = constraint.Width;

        fontRect = TextMeasurer.MeasureBounds(content, options);
        return HRESULT.S_OK;
    }
}
