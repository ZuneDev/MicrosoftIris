using System;
using System.Runtime.InteropServices;
using Microsoft.Iris.Interop;
using Microsoft.Iris.Render.Engine;
using Microsoft.Iris.Render.Interop;
using Microsoft.Iris.Render.Interop.Drawing;
using Microsoft.Iris.Render.Interop.Text;
using Microsoft.Iris.Render.Interop.Win32;

namespace Microsoft.Iris.Render.Subsystems.Text;

// [UnmanagedCallersOnly] exports for the SpSimpleText* family in
// UIX/Microsoft/Iris/OS/NativeApi.cs -- measurement/rendering only, no editing.
public static unsafe class SimpleTextApi
{
    private sealed class SimpleTextObject(Size maximumSurface)
    {
        public Size MaximumSurface { get; } = maximumSurface;
    }

    // The simple-text path *is* available: it's the measurement fast path, and this
    // implementation provides measurement (approximately -- see TextMetrics). Reporting
    // false here would push every caller onto the rich-text path for no benefit.
    [UnmanagedCallersOnly(EntryPoint = "SpSimpleTextIsAvailable")]
    public static int SpSimpleTextIsAvailable() => 1;

    [UnmanagedCallersOnly(EntryPoint = "SpSimpleTextBuildObject")]
    public static HRESULT SpSimpleTextBuildObject(Size sizeMaximumSurface, HANDLE* hSto)
    {
        if (hSto == null)
            return HRESULT.E_INVALIDARG;

        hSto->h = HandleTable.Alloc(new SimpleTextObject(sizeMaximumSurface));
        return HRESULT.S_OK;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpSimpleTextDestroyObject")]
    public static void SpSimpleTextDestroyObject(HANDLE hSto) => HandleTable.Free(hSto.h);

    // Fills in the caller's RasterizeRunPacket with real layout geometry derived from
    // TextMetrics (bounds, natural size, ascender/baseline insets, colour, line number).
    // No glyph run handle is produced, because there are no glyphs -- hGlyphRunInfo comes
    // back null and the caller's subsequent SpRichTextRasterize would report E_NOTIMPL.
    // TODO: produce a real glyph run once a font backend exists.
    [UnmanagedCallersOnly(EntryPoint = "SpSimpleTextMeasure")]
    public static HRESULT SpSimpleTextMeasure(HANDLE hSto, char* pszRef, short wAlignment, TextStyleData* textStyle, Size sizeConstraint, IntPtr* hGlyphRunInfo, RasterizeRunPacket* pRun)
    {
        if (hGlyphRunInfo == null || !HandleTable.TryGet(hSto.h, out SimpleTextObject _))
            return HRESULT.E_INVALIDARG;

        *hGlyphRunInfo = IntPtr.Zero;

        string text = NativeString.UniToString(pszRef) ?? string.Empty;
        float fontHeight = textStyle != null && textStyle->fontHeightPts > 0 ? textStyle->fontHeightPts : 12f;

        Size measured = TextMetrics.Measure(text, fontHeight, wordWrap: sizeConstraint.width > 0, sizeConstraint.width);

        if (pRun != null)
        {
            pRun->rcLayoutBounds = new Rectangle { x = 0, y = 0, width = measured.width, height = measured.height };
            pRun->rcfRenderBounds = new RectangleF { x = 0, y = 0, width = measured.width, height = measured.height };
            pRun->sizeRasterizeRun = measured;
            pRun->sizeNatural = measured;
            pRun->ascenderInset = TextMetrics.Ascent(fontHeight);
            pRun->baselineInset = TextMetrics.Ascent(fontHeight);
            pRun->lineNumber = 0;
            if (textStyle != null)
                pRun->clrText = textStyle->textColor;
        }

        return HRESULT.S_OK;
    }

    // "Is this string measurable with this style" -- true whenever there's a style to
    // measure against, since TextMetrics has no per-glyph coverage requirement (it is
    // metric-derived, not font-table-derived).
    [UnmanagedCallersOnly(EntryPoint = "SpSimpleTextMeasurePossible")]
    public static HRESULT SpSimpleTextMeasurePossible(HANDLE hSto, char* pszRef, TextStyleData* textStyle, int* fPossible)
    {
        if (fPossible == null)
            return HRESULT.E_INVALIDARG;

        *fPossible = textStyle != null ? 1 : 0;
        return HRESULT.S_OK;
    }
}
