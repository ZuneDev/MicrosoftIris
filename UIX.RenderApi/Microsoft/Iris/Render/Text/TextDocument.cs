using System;
using Microsoft.Iris.Render.Internal;

namespace Microsoft.Iris.Render.Text;

// Based on Microsoft.Iris.Render.Bitmaps.BitmapInformation
//
// Covers the "given text (+ a style), produce metrics/pixels" surface that is
// safe to swap per-platform: content storage/query, measurement, and glyph
// rasterization. Deliberately does NOT cover interactive text editing (IME,
// keyboard/mouse forwarding, undo, clipboard, scrollbars, timers) or
// multi-range rich-text formatting (TextMeasureParams' formatted ranges) -
// those remain native/Windows-only in Microsoft.Iris.Drawing.RichText. See
// the "Cross-platform text/font abstraction" plan and logs/ for the rationale.
public abstract class TextDocument : IDisposable
{
    public abstract HRESULT SetContent(string content);

    public abstract HRESULT GetSimpleContent(out string content);

    public abstract HRESULT GetNaturalBounds(out Size bounds);

    public abstract HRESULT MeasurePossible(string content, TextStyleInfo style, out bool possible);

    public abstract HRESULT Measure(string content, TextAlignment alignment, TextStyleInfo style, Size constraint, out GlyphRunInfo glyphRun);

    public abstract HRESULT Rasterize(GlyphRunInfo glyphRun, ColorF textColor, bool outline, bool shadow, out RasterizedGlyphBitmap bitmap);

    public abstract void Dispose();
}
