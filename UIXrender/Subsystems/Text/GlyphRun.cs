using System;
using System.Buffers;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Iris.Render.Interop.Drawing;

namespace Microsoft.Iris.Render.Subsystems.Text;

// A measured, rasterizable line of text -- the object the `hGlyphRunInfo` handle points at
// between SpSimpleTextMeasure (which builds it) and SpRichTextRasterize (which draws it).
// Backend-agnostic: rasterization composites stb_truetype's CPU coverage bitmaps into a
// straight-alpha ARGB32 buffer (0xAARRGGBB in memory == BGRA little-endian, matching the
// SurfaceFormat.ARGB32 convention BitmapStore already uses).
internal sealed class GlyphRun
{
    public GlyphRun(LoadedFont font, string text, float pixelHeight, Color color, Size size)
    {
        Font = font;
        Text = text ?? string.Empty;
        PixelHeight = pixelHeight;
        Color = color;
        Size = size;
    }

    public LoadedFont Font { get; }
    public string Text { get; }
    public float PixelHeight { get; }
    public Color Color { get; }
    public Size Size { get; }

    // Produces a straight-alpha ARGB32 bitmap of the measured size, filled with `color`
    // (SpRichTextRasterize re-supplies the text colour at draw time). Caller owns the
    // returned buffer (freed via SpFreeDib). Returns IntPtr.Zero for an empty run or when
    // no real font is available (the ratio fallback can measure but cannot rasterize).
    public unsafe IntPtr Rasterize(Color color, out Size size)
    {
        size = Size;
        if (Font == null || Size.width <= 0 || Size.height <= 0 || Text.Length == 0)
            return IntPtr.Zero;

        int width = Size.width;
        int height = Size.height;
        int byteCount = width * height * 4;

        IntPtr buffer = Marshal.AllocHGlobal(byteCount);
        var dst = (byte*)buffer;
        for (int i = 0; i < byteCount; i++)
            dst[i] = 0;

        float scale = Font.ScaleForPixelHeight(PixelHeight);
        int baseline = (int)MathF.Round(Font.AscentPx(scale));
        byte colorR = color.R, colorG = color.G, colorB = color.B, colorA = color.A;

        float penX = 0f;
        int previous = 0;

        foreach (Rune rune in Text.EnumerateRunes())
        {
            int cp = rune.Value;
            if (previous != 0)
                penX += Font.KerningPx(previous, cp, scale);

            Font.GetGlyphBox(cp, scale, out int ix0, out int iy0, out int ix1, out int iy1);
            int gw = ix1 - ix0;
            int gh = iy1 - iy0;

            if (gw > 0 && gh > 0)
            {
                byte[] coverage = ArrayPool<byte>.Shared.Rent(gw * gh);
                try
                {
                    fixed (byte* cov = coverage)
                    {
                        Font.RenderGlyphCoverage(cp, scale, cov, gw, gh, gw);
                        Blit(dst, width, height, cov, gw, gh, (int)MathF.Round(penX) + ix0, baseline + iy0, colorR, colorG, colorB, colorA);
                    }
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(coverage);
                }
            }

            penX += Font.AdvancePx(cp, scale);
            previous = cp;
        }

        return buffer;
    }

    private static unsafe void Blit(byte* dst, int dstW, int dstH, byte* coverage, int gw, int gh, int originX, int originY, byte r, byte g, byte b, byte a)
    {
        for (int gy = 0; gy < gh; gy++)
        {
            int dy = originY + gy;
            if ((uint)dy >= (uint)dstH)
                continue;

            for (int gx = 0; gx < gw; gx++)
            {
                byte cov = coverage[gy * gw + gx];
                if (cov == 0)
                    continue;

                int dx = originX + gx;
                if ((uint)dx >= (uint)dstW)
                    continue;

                byte alpha = (byte)(cov * a / 255);
                byte* p = dst + (dy * dstW + dx) * 4;
                // Keep the strongest coverage where glyphs happen to overlap.
                if (alpha >= p[3])
                {
                    p[0] = b;
                    p[1] = g;
                    p[2] = r;
                    p[3] = alpha;
                }
            }
        }
    }
}

// Owns the unmanaged ARGB buffer handed back as `phTextBitmap`; freed by SpFreeDib.
internal sealed class TextBitmap(IntPtr bits) : IDisposable
{
    private IntPtr _bits = bits;

    public IntPtr Bits => _bits;

    public void Dispose()
    {
        if (_bits != IntPtr.Zero)
        {
            Marshal.FreeHGlobal(_bits);
            _bits = IntPtr.Zero;
        }
    }
}
