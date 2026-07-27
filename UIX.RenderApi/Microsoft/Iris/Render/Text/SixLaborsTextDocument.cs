using System;
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
