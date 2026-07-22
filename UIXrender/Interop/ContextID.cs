using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Interop;

// Bit-for-bit mirror of Microsoft.Iris.Render.Protocol.ContextID
// (UIX.RenderApi/Microsoft/Iris/Render/Protocol/ContextID.cs).
[StructLayout(LayoutKind.Sequential)]
public struct ContextID
{
    public static readonly ContextID NULL = new(0);
    public static readonly ContextID CURRENT = new(uint.MaxValue);

    public uint value;

    public ContextID(uint value) => this.value = value;

    public static bool operator ==(ContextID a, ContextID b) => a.value == b.value;
    public static bool operator !=(ContextID a, ContextID b) => a.value != b.value;

    public override bool Equals(object obj) => obj is ContextID other && value == other.value;
    public override int GetHashCode() => (int)value;
}
