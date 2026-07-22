using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Interop.Drawing;

// Bit-for-bit mirror of Microsoft.Iris.Render.Point (UIX.RenderApi/Microsoft/Iris/Render/Point.cs).
[StructLayout(LayoutKind.Sequential)]
public struct Point
{
    public int x;
    public int y;
}
