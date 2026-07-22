using System;
using System.Runtime.InteropServices;
using Microsoft.Iris.Render.Interop.Drawing;

namespace Microsoft.Iris.Render.Interop.Extensions;

// Bit-for-bit mirrors of the ExtensionsApi.cs (UIX.RenderApi/Microsoft/Iris/Render/Extensions/*.cs)
// asset-loading types: ImageRequirements, ImageHeader, ImageData, ImageInformation,
// HSpBitmap, SurfaceFormat.

public enum SurfaceFormat : uint
{
    None = 0,
    Bpp8 = 0x00080000,
    A8 = 0x00088000,
    Bpp16 = 0x00100000,
    RGB16_555 = 0x00100555,
    RGB16_565 = 0x00100565,
    ARGB16_1555 = 0x00101555,
    Bpp24 = 0x00180000,
    RGB24 = 0x00180888,
    Bpp32 = 0x00200000,
    RGB32 = 0x00200888,
    ARGB32 = 0x00208888,
    YUY2 = 0x21100000,
    External = 0x80000000,
}

[Flags]
public enum ImageRequirementsFields
{
    None = 0,
    MaximumSize = 1,
    Border = 2,
    Flippable = 4,
    AntialiasEdges = 16,
}

[StructLayout(LayoutKind.Sequential)]
public struct ImageRequirements
{
    public ImageRequirementsFields mask;
    public Size maximumSizePxl;
    public int borderPxl;
    public ColorF borderColor;
}

[StructLayout(LayoutKind.Sequential)]
public struct ImageHeader
{
    public Size sizeActualPxl;
    public Size sizeOriginalPxl;
    public int stride;
    public SurfaceFormat format;
}

[StructLayout(LayoutKind.Sequential)]
public struct ImageData
{
    public IntPtr rgData;
}

[StructLayout(LayoutKind.Sequential)]
public struct ImageInformation
{
    public ImageHeader header;
    public ImageData data;
}

[Flags]
public enum BitmapOptions
{
    None = 0,
    Decode = 1,
    Flip = 2,
    Valid = Flip | Decode,
}

[StructLayout(LayoutKind.Sequential)]
public struct HSpBitmap
{
    public IntPtr h;
    public static readonly HSpBitmap NULL = new();
}
