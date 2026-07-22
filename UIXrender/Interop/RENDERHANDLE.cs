using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Interop;

// Bit-for-bit mirror of Microsoft.Iris.Render.Protocol.RENDERHANDLE
// (UIX.RenderApi/Microsoft/Iris/Render/Protocol/RENDERHANDLE.cs).
[StructLayout(LayoutKind.Sequential)]
public struct RENDERHANDLE
{
    public static readonly RENDERHANDLE NULL = new(0);

    public uint value;

    public RENDERHANDLE(uint value) => this.value = value;

    public static bool operator ==(RENDERHANDLE a, RENDERHANDLE b) => a.value == b.value;
    public static bool operator !=(RENDERHANDLE a, RENDERHANDLE b) => a.value != b.value;

    public override bool Equals(object obj) => obj is RENDERHANDLE other && value == other.value;
    public override int GetHashCode() => (int)value;
}
