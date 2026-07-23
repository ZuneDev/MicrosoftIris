using System;
using System.Runtime.InteropServices;
using StbTrueTypeSharp;

namespace Microsoft.Iris.Render.Subsystems.Text;

// A TrueType face loaded via StbTrueTypeSharp (pure-managed stb_truetype). Backend-
// agnostic: this touches no GPU/window API -- it only produces glyph metrics and 8-bit
// coverage bitmaps in CPU memory, which is exactly what SpSimpleTextMeasure /
// SpRichTextMeasure / SpRichTextRasterize need.
//
// stb_truetype stores the raw font-data *pointer* inside stbtt_fontinfo (it does not copy
// by default), so the backing byte[] must stay fixed for the font's whole lifetime --
// hence the pinned GCHandle held here and freed in Dispose.
internal sealed unsafe class LoadedFont : IDisposable
{
    private readonly byte[] _data;
    private GCHandle _pin;
    private readonly StbTrueType.stbtt_fontinfo _info;

    public string Family { get; }

    // Unscaled (font-design-unit) vertical metrics; multiply by ScaleForPixelHeight(px).
    public int AscentUnscaled { get; }
    public int DescentUnscaled { get; }
    public int LineGapUnscaled { get; }

    private LoadedFont(string family, byte[] data, GCHandle pin, StbTrueType.stbtt_fontinfo info)
    {
        Family = family;
        _data = data;
        _pin = pin;
        _info = info;

        int ascent, descent, lineGap;
        StbTrueType.stbtt_GetFontVMetrics(info, &ascent, &descent, &lineGap);
        AscentUnscaled = ascent;
        DescentUnscaled = descent;
        LineGapUnscaled = lineGap;
    }

    public static LoadedFont TryLoad(string family, byte[] data)
    {
        if (data == null || data.Length == 0)
            return null;

        GCHandle pin = GCHandle.Alloc(data, GCHandleType.Pinned);
        var info = new StbTrueType.stbtt_fontinfo();
        var p = (byte*)pin.AddrOfPinnedObject();

        int offset = StbTrueType.stbtt_GetFontOffsetForIndex(p, 0);
        if (offset < 0 || StbTrueType.stbtt_InitFont(info, p, offset) == 0)
        {
            pin.Free();
            return null;
        }

        return new LoadedFont(family, data, pin, info);
    }

    public float ScaleForPixelHeight(float px) => StbTrueType.stbtt_ScaleForPixelHeight(_info, px);

    public float LineHeightPx(float scale) => (AscentUnscaled - DescentUnscaled + LineGapUnscaled) * scale;

    public float AscentPx(float scale) => AscentUnscaled * scale;

    public float AdvancePx(int codepoint, float scale)
    {
        int advance, leftSideBearing;
        StbTrueType.stbtt_GetCodepointHMetrics(_info, codepoint, &advance, &leftSideBearing);
        return advance * scale;
    }

    public float KerningPx(int codepoint1, int codepoint2, float scale) =>
        StbTrueType.stbtt_GetCodepointKernAdvance(_info, codepoint1, codepoint2) * scale;

    public void GetGlyphBox(int codepoint, float scale, out int ix0, out int iy0, out int ix1, out int iy1)
    {
        int x0, y0, x1, y1;
        StbTrueType.stbtt_GetCodepointBitmapBox(_info, codepoint, scale, scale, &x0, &y0, &x1, &y1);
        ix0 = x0; iy0 = y0; ix1 = x1; iy1 = y1;
    }

    // Renders one glyph's 8-bit coverage into a caller-owned buffer (no allocation, no
    // free -- unlike stbtt_GetCodepointBitmap which mallocs).
    public void RenderGlyphCoverage(int codepoint, float scale, byte* output, int width, int height, int stride) =>
        StbTrueType.stbtt_MakeCodepointBitmap(_info, output, width, height, stride, scale, scale, codepoint);

    public void Dispose()
    {
        if (_pin.IsAllocated)
            _pin.Free();
    }
}
