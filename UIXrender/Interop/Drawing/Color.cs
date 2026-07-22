using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Interop.Drawing;

// Bit-for-bit mirror of Microsoft.Iris.Drawing.Color (UIX/Microsoft/Iris/Drawing/Color.cs)
// -- a single packed 0xAARRGGBB uint, not four separate byte fields.
[StructLayout(LayoutKind.Sequential)]
public struct Color
{
    public uint value;

    public Color(uint value) => this.value = value;

    public byte A => (byte)(value >> 24);
    public byte R => (byte)(value >> 16);
    public byte G => (byte)(value >> 8);
    public byte B => (byte)value;

    public static Color FromArgb(byte a, byte r, byte g, byte b) =>
        new((uint)((a << 24) | (r << 16) | (g << 8) | b));
}
