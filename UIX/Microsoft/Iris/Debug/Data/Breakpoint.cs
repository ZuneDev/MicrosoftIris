using Microsoft.Iris.Markup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Iris.Debug.Data;

[Serializable]
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

    public bool Equals(LoadResult loadResult, int line, int column, uint offset = uint.MaxValue)
    {
        return Equals([loadResult.Uri, loadResult.UnderlyingUri], line, column, offset);
    }

    public bool Equals(IEnumerable<string> uris, int line, int column, uint offset = uint.MaxValue)
    {
        var thisUri = Uri;
        if (uris.Any(uri => !thisUri.Equals(uri, StringComparison.OrdinalIgnoreCase)))
            return false;

        if (Offset != uint.MaxValue && offset != uint.MaxValue)
            return Offset == offset;
        else
            return line == Line && column == Column;
    }

    public bool Equals(Breakpoint other) => Equals([other.Uri], other.Line, other.Column);

    public static bool operator ==(Breakpoint left, Breakpoint right) => left.Equals(right);

    public static bool operator !=(Breakpoint left, Breakpoint right) => !(left == right);

    public override bool Equals(object obj) => obj is Breakpoint bp && Equals(bp);

    public override string ToString() => ToString(true);

    public string ToString(bool includeEnabled)
    {
        StringBuilder sb = new();

        if (includeEnabled)
        {
            sb.Append(Enabled ? '+' : '-');
            sb.Append(' ');
        }

        sb.Append(Uri);
        sb.Append(' ');

        if (Line >= 0 && Column >= 0)
            sb.Append($"({Line}, {Column})");
        else
            sb.Append($"0x{Offset:X}");

        return sb.ToString();
    }

    public override int GetHashCode() => ToString(false).GetHashCode();
}
