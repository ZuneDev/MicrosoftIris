using System;
using System.Runtime.InteropServices;
using Microsoft.Iris.Render.Interop.Drawing;
using Microsoft.Iris.Render.Interop.Extensions;
using StbImageSharp;

namespace Microsoft.Iris.Render.Subsystems.Assets;

// Backing store for the SpBitmap* family (UIX.RenderApi/.../Extensions/ExtensionsApi.cs).
//
// Decode is real and cross-platform: StbImageSharp is pure managed (PNG/JPEG/BMP/TGA/GIF/
// PSD), so this works identically on Windows and Linux and stays NativeAOT-safe, unlike
// System.Drawing/GDI+. See logs/UIXrender/FullSurface.md, decision 3.
//
// Pixels always land as ARGB32 in a single unmanaged allocation the caller can read
// through ImageInformation.Data.rgData until SpBitmapDelete. Stb decodes to RGBA byte
// order; the Iris SurfaceFormat.ARGB32 the managed side expects is BGRA in memory on a
// little-endian machine (0xAARRGGBB as a uint), so the channel swap below is required,
// not incidental.
internal sealed class LoadedBitmap : IDisposable
{
    private IntPtr _pixels;

    private LoadedBitmap(IntPtr pixels, ImageHeader header)
    {
        _pixels = pixels;
        Header = header;
    }

    public ImageHeader Header { get; }

    public ImageInformation ToInformation() => new()
    {
        header = Header,
        data = new ImageData { rgData = _pixels },
    };

    public static LoadedBitmap FromDecoded(ImageResult image, in ImageRequirements requirements)
    {
        int width = image.Width;
        int height = image.Height;
        int stride = width * 4;

        IntPtr buffer = Marshal.AllocHGlobal(stride * height);
        unsafe
        {
            var dest = (byte*)buffer;
            byte[] src = image.Data;
            for (int i = 0; i < width * height; i++)
            {
                // RGBA (stb) -> BGRA in memory == 0xAARRGGBB as a little-endian uint.
                dest[i * 4 + 0] = src[i * 4 + 2];
                dest[i * 4 + 1] = src[i * 4 + 1];
                dest[i * 4 + 2] = src[i * 4 + 0];
                dest[i * 4 + 3] = src[i * 4 + 3];
            }
        }

        var header = new ImageHeader
        {
            sizeActualPxl = new Size(width, height),
            sizeOriginalPxl = new Size(width, height),
            stride = stride,
            format = SurfaceFormat.ARGB32,
        };

        return new LoadedBitmap(buffer, header);
    }

    public static unsafe LoadedBitmap FromRaw(Size sizeActualPxl, int stride, SurfaceFormat format, IntPtr source)
    {
        int byteCount = stride * sizeActualPxl.height;
        IntPtr buffer = Marshal.AllocHGlobal(byteCount);
        Buffer.MemoryCopy((void*)source, (void*)buffer, byteCount, byteCount);

        var header = new ImageHeader
        {
            sizeActualPxl = sizeActualPxl,
            sizeOriginalPxl = sizeActualPxl,
            stride = stride,
            format = format,
        };

        return new LoadedBitmap(buffer, header);
    }

    public void Dispose()
    {
        if (_pixels != IntPtr.Zero)
        {
            Marshal.FreeHGlobal(_pixels);
            _pixels = IntPtr.Zero;
        }
    }
}
