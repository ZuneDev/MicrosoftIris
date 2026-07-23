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

    // Measures the run with the resolved font (real glyph advances + kerning, or the ratio
    // fallback if no font is available) and produces a GlyphRun behind hGlyphRunInfo that
    // SpRichTextRasterize can later draw. Fills the RasterizeRunPacket with the resulting
    // geometry.
    [UnmanagedCallersOnly(EntryPoint = "SpSimpleTextMeasure")]
    public static HRESULT SpSimpleTextMeasure(HANDLE hSto, char* pszRef, short wAlignment, TextStyleData* textStyle, Size sizeConstraint, IntPtr* hGlyphRunInfo, RasterizeRunPacket* pRun)
    {
        if (hGlyphRunInfo == null || !HandleTable.TryGet(hSto.h, out SimpleTextObject _))
            return HRESULT.E_INVALIDARG;

        *hGlyphRunInfo = IntPtr.Zero;

        string text = NativeString.UniToString(pszRef) ?? string.Empty;
        string face = textStyle != null ? NativeString.UniToString(textStyle->fontFace) : null;
        float fontHeight = textStyle != null && textStyle->fontHeightPts > 0 ? textStyle->fontHeightPts : 12f;
        Color color = textStyle != null ? textStyle->textColor : default;

        LoadedFont font = FontStore.Resolve(face);
        bool wrap = sizeConstraint.width > 0;
        Size measured = TextLayout.Measure(font, text, fontHeight, wrap, sizeConstraint.width);

        var run = new GlyphRun(font, text, fontHeight, color, measured);
        *hGlyphRunInfo = HandleTable.Alloc(run);

        if (pRun != null)
        {
            int ascent = TextLayout.Ascent(font, fontHeight);
            pRun->rcLayoutBounds = new Rectangle { x = 0, y = 0, width = measured.width, height = measured.height };
            pRun->rcfRenderBounds = new RectangleF { x = 0, y = 0, width = measured.width, height = measured.height };
            pRun->sizeRasterizeRun = measured;
            pRun->sizeNatural = measured;
            pRun->ascenderInset = ascent;
            pRun->baselineInset = ascent;
            pRun->lineNumber = 0;
            pRun->clrText = color;
        }

        return HRESULT.S_OK;
    }

    // "Is this string measurable with this style" -- true whenever a font resolves (real
    // or fallback), which is the condition under which SpSimpleTextMeasure will succeed.
    [UnmanagedCallersOnly(EntryPoint = "SpSimpleTextMeasurePossible")]
    public static HRESULT SpSimpleTextMeasurePossible(HANDLE hSto, char* pszRef, TextStyleData* textStyle, int* fPossible)
    {
        if (fPossible == null)
            return HRESULT.E_INVALIDARG;

        string face = textStyle != null ? NativeString.UniToString(textStyle->fontFace) : null;
        *fPossible = FontStore.Resolve(face) != null || textStyle != null ? 1 : 0;
        return HRESULT.S_OK;
    }
}
