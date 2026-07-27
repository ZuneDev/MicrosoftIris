using System;

namespace Microsoft.Iris.Render.Text.Editing;

// Start/End (not anchor/caret) to match Microsoft.Iris.Drawing.RichText's
// native SetSelectionRange(start, end) shape, so a future integration can map
// 1:1 without reordering semantics.
public readonly struct TextSelection : IEquatable<TextSelection>
{
    public static readonly TextSelection Empty = new(0, 0);

    public TextSelection(int start, int end)
    {
        Start = Math.Min(start, end);
        End = Math.Max(start, end);
    }

    public int Start { get; }
    public int End { get; }
    public int Length => End - Start;
    public bool IsEmpty => Start == End;

    public bool Equals(TextSelection other) => Start == other.Start && End == other.End;
    public override bool Equals(object obj) => obj is TextSelection other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(Start, End);
    public static bool operator ==(TextSelection left, TextSelection right) => left.Equals(right);
    public static bool operator !=(TextSelection left, TextSelection right) => !left.Equals(right);
}
