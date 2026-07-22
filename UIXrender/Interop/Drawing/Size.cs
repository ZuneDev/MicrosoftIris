using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Interop.Drawing;

// Bit-for-bit mirror of Microsoft.Iris.Render.Size (UIX.RenderApi/Microsoft/Iris/Render/Size.cs).
[StructLayout(LayoutKind.Sequential)]
public struct Size
{
    public int width;
    public int height;

    public Size(int width, int height)
    {
        this.width = width;
        this.height = height;
    }
}
