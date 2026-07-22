using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Interop.Drawing;

// Bit-for-bit mirror of Microsoft.Iris.RenderAPI.Drawing.SizeF (UIX/Microsoft/Iris/RenderAPI/Drawing/SizeF.cs).
[StructLayout(LayoutKind.Sequential)]
public struct SizeF
{
    public float width;
    public float height;
}
