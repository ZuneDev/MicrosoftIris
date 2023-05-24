using System;

namespace Microsoft.Iris.Debug;

public struct Breakpoint : IEquatable<Breakpoint>
{
    public Breakpoint(string uri, int line, int column)
    {
        Uri = uri;
        Line = line;
        Column = column;
    }

    public string Uri { get; set; }

    public int Line { get; set; }

    public int Column { get; set; }

    public bool Equals(string uri, int line, int column) => uri == Uri && line == Line && column == Column;

    public bool Equals(Breakpoint other) => Equals(other.Uri, other.Line, other.Column);

    public static bool operator ==(Breakpoint left, Breakpoint right) => left.Equals(right);

    public static bool operator !=(Breakpoint left, Breakpoint right) => !(left == right);

    public override bool Equals(object obj) => obj is Breakpoint bp && Equals(bp);
}
