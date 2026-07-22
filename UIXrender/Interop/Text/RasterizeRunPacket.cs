using System.Runtime.InteropServices;
using Microsoft.Iris.Render.Interop.Drawing;
using Microsoft.Iris.Render.Interop.Win32;

namespace Microsoft.Iris.Render.Interop.Text;

// Bit-for-bit mirror of NativeApi.RasterizeRunPacket/UnderlineStyle
// (UIX/Microsoft/Iris/OS/NativeApi.cs).
public enum UnderlineStyle
{
    None,
    Solid,
    Thick,
    Dotted,
    Dash,
    DashDot,
    DashDotDot,
}

[StructLayout(LayoutKind.Sequential)]
public struct RasterizeRunPacket
{
    public Rectangle rcLayoutBounds;
    public RectangleF rcfRenderBounds;
    public int naturalX;
    public int naturalY;
    public int rasterizeX;
    public int rasterizeY;
    public byte aaConfig;
    public Color clrText;
    public Color clrBackground;
    public int fontFaceUniqueId;
    public LOGFONTW lf;
    public Size sizeRasterizeRun;
    public Size sizeNatural;
    public int ascenderInset;
    public int baselineInset;
    public int lineNumber;
    public int effects;
    public UnderlineStyle underlineStyle;
    public Rectangle rcUnderlineBounds;
}
