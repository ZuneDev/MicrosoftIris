using System;

namespace Microsoft.Iris.Render.Text;

// Based on the fields Microsoft.Iris.Drawing.TextRun copies out of the native
// NativeApi.RasterizeRunPacket on construction.
public sealed class GlyphRunInfo : IDisposable
{
    public string Content { get; init; }
    public Rectangle LayoutBounds { get; init; }
    public float RenderBoundsX { get; init; }
    public float RenderBoundsY { get; init; }
    public float RenderBoundsWidth { get; init; }
    public float RenderBoundsHeight { get; init; }
    public Size NaturalExtent { get; init; }
    public int NaturalX { get; init; }
    public int NaturalY { get; init; }
    public int RasterizeX { get; init; }
    public int RasterizeY { get; init; }
    public byte RasterizerConfig { get; init; }
    public ColorF RunColor { get; init; }
    public ColorF HighlightColor { get; init; }
    public int FontFaceUniqueId { get; init; }
    public int FontHeight { get; init; }
    public int FontWeight { get; init; }
    public bool Italic { get; init; }
    public bool Underline { get; init; }
    public bool Link { get; init; }
    public UnderlineStyle UnderlineStyle { get; init; }
    public Rectangle UnderlineBounds { get; init; }
    public int Line { get; init; }
    public int AscenderInset { get; init; }
    public int BaselineInset { get; init; }

    // Opaque backend-owned state (e.g. the native hGlyphRunInfo handle) needed
    // by TextDocument.Rasterize and released by Dispose. Never touched outside
    // the TextDocument implementation that produced this instance.
    public object BackendHandle { get; init; }
    private readonly Action<object> _disposeBackendHandle;

    public GlyphRunInfo(Action<object> disposeBackendHandle = null)
    {
        _disposeBackendHandle = disposeBackendHandle;
    }

    public void Dispose() => _disposeBackendHandle?.Invoke(BackendHandle);
}
