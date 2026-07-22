using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Interop.Drawing;

// Bit-for-bit mirror of Microsoft.Iris.Render.Rectangle (UIX.RenderApi/Microsoft/Iris/Render/Rectangle.cs).
[StructLayout(LayoutKind.Sequential)]
public struct Rectangle
{
    public int x;
    public int y;
    public int width;
    public int height;
}

// Bit-for-bit mirror of Microsoft.Iris.RenderAPI.Drawing.RectangleF
// (UIX/Microsoft/Iris/RenderAPI/Drawing/RectangleF.cs).
[StructLayout(LayoutKind.Sequential)]
public struct RectangleF
{
    public float x;
    public float y;
    public float width;
    public float height;
}
