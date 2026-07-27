using System;
using System.Collections.Generic;
using Microsoft.Iris.Render.Internal;

namespace Microsoft.Iris.Render.Text;

// Based on Microsoft.Iris.Render.Bitmaps.BitmapInformation
//
// Covers the "given text (+ a style), produce metrics/pixels" surface that is
// safe to swap per-platform: content storage/query, measurement, and glyph
// rasterization. Deliberately does NOT cover interactive text editing (IME,
// keyboard/mouse forwarding, undo, clipboard, scrollbars, timers) - that
// remains native/Windows-only in Microsoft.Iris.Drawing.RichText's hosted
// mode. See the "Cross-platform text/font abstraction" plan and logs/ for the
// rationale.
public abstract class TextDocument : IDisposable
{
    public abstract HRESULT SetContent(string content);

    public abstract HRESULT GetSimpleContent(out string content);

    public abstract HRESULT GetNaturalBounds(out Size bounds);

    public abstract HRESULT MeasurePossible(string content, TextStyleInfo style, out bool possible);

    public abstract HRESULT Measure(string content, TextAlignment alignment, TextStyleInfo style, Size constraint, out GlyphRunInfo glyphRun);

    // Multi-range formatted measurement: mirrors TextMeasureParams' formatted
    // ranges (e.g. differently-colored hyperlink runs within one text block),
    // used by Microsoft.Iris.Drawing.RichText's non-hosted (display-only)
    // mode. Default implementation ignores formattedRanges/wordWrap and
    // falls back to a single whole-content Measure call, so implementations
    // that don't need multi-range formatting (or don't support it - see
    // SpTextDocument's bound mode) don't have to override it.
    public virtual HRESULT Measure(string content, TextAlignment alignment, TextStyleInfo baseStyle, IReadOnlyList<TextStyleRun> formattedRanges, Size constraint, bool wordWrap, out GlyphRunInfo[] glyphRuns)
    {
        var hresult = Measure(content, alignment, baseStyle, constraint, out var glyphRun);
        glyphRuns = hresult.IsSuccess() && glyphRun != null ? new[] { glyphRun } : Array.Empty<GlyphRunInfo>();
        return hresult;
    }

    public abstract HRESULT Rasterize(GlyphRunInfo glyphRun, ColorF textColor, bool outline, bool shadow, out RasterizedGlyphBitmap bitmap);

    public abstract void Dispose();
}
