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
        glyphRuns = hresult.IsSuccess() && glyphRun != null ? [glyphRun] : [];
        return hresult;
    }

    public abstract HRESULT Rasterize(GlyphRunInfo glyphRun, ColorF textColor, bool outline, bool shadow, out RasterizedGlyphBitmap bitmap);

    // Caret/hit-testing support for Microsoft.Iris.Drawing.RichText's hosted
    // (interactive-editing) mode on non-Windows platforms: on Windows the
    // native RichEdit-style control computes caret position and mouse hit
    // testing internally and reports it via IRichTextCallbacks, so RichText
    // never needs to ask _textDocument for it there. Default implementation
    // reports "not implemented" so backends that never host interactive
    // editing (SpTextDocument - hosted mode stays fully native on Windows,
    // see RichText.cs) don't have to override it.
    public virtual HRESULT GetCaretMetrics(string content, TextStyleInfo style, Size constraint, bool wordWrap, int characterIndex, out Rectangle caretBounds)
    {
        caretBounds = Rectangle.Zero;
        return unchecked((int)0x80004001); // E_NOTIMPL
    }

    public virtual HRESULT HitTest(string content, TextStyleInfo style, Size constraint, bool wordWrap, Point point, out int characterIndex)
    {
        characterIndex = 0;
        return unchecked((int)0x80004001); // E_NOTIMPL
    }

    public abstract void Dispose();
}
