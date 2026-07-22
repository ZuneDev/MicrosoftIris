using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Interop.Drawing;

// Bit-for-bit mirror of Microsoft.Iris.Render.ColorF (UIX.RenderApi/Microsoft/Iris/Render/ColorF.cs).
[StructLayout(LayoutKind.Sequential)]
public struct ColorF
{
    public float a;
    public float r;
    public float g;
    public float b;
}
