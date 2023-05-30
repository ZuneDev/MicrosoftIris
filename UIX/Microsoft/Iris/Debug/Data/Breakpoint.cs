using System;
using System.Text;

namespace Microsoft.Iris.Debug.Data;

public struct Breakpoint : IEquatable<Breakpoint>
{
    public Breakpoint(string uri, int line, int column, bool enabled = true) : this(uri, enabled)
    {
        Line = line;
        Column = column;
    }

    public Breakpoint(string uri, uint offset, bool enabled = true) : this(uri, enabled)
    {
        Offset = offset;
    }

    private Breakpoint(string uri, bool enabled)
    {
        Uri = uri;
        Enabled = enabled;
    }

    public string Uri { get; set; }

    public int Line { get; set; } = -1;

    public int Column { get; set; } = -1;

    public uint Offset { get; set; } = uint.MaxValue;

    public bool Enabled { get; set; }

    public bool Equals(string uri, int line, int column) => uri == Uri && line == Line && column == Column;

    public bool Equals(Breakpoint other) => Equals(other.Uri, other.Line, other.Column);

    public static bool operator ==(Breakpoint left, Breakpoint right) => left.Equals(right);

    public static bool operator !=(Breakpoint left, Breakpoint right) => !(left == right);

    public override bool Equals(object obj) => obj is Breakpoint bp && Equals(bp);

    public override string ToString()
    {
        StringBuilder sb = new();

        sb.Append(Enabled ? '+' : '-');
        sb.Append(' ');
        sb.Append(Uri);
        sb.Append(' ');

        if (Line >= 0 && Column >= 0)
            sb.Append($"({Line}, {Column})");
        else
            sb.Append($"0x{Offset:X}");

        return sb.ToString();
    }

    public override int GetHashCode() => ToString().GetHashCode();
}
