using System;

namespace Microsoft.Iris.Render.Text;

// Replaces the phTextBitmap/ppvBits/psizeBitmap triple returned by the native
// SpRichTextRasterize. Bits is always unmanaged memory (owned by whichever
// TextDocument implementation produced it) laid out as top-down A8R8G8B8.
public sealed class RasterizedGlyphBitmap : IDisposable
{
    public Size Size { get; init; }
    public IntPtr Bits { get; init; }

    // Backend-native handle (e.g. the Windows HBITMAP/DIB handle), if any -
    // only meaningful to the backend that produced this instance.
    public IntPtr NativeHandle { get; init; }
    private readonly Action _disposeAction;

    public RasterizedGlyphBitmap(Action disposeAction)
    {
        _disposeAction = disposeAction;
    }

    public void Dispose() => _disposeAction?.Invoke();
}
