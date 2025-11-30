using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Microsoft.Iris.Debug.Symbols;

[CLSCompliant(false)]
public class ApplicationDebugSymbols
{
    public List<FileDebugSymbols> Files { get; set; }
}

[CLSCompliant(false)]
public class FileDebugSymbols
{
    private string[] _sourceCodeLines;

    public string OriginalFileName { get; set; }

    public string CompiledFileName { get; set; }

    public string SourceFileName { get; set; }

    public SourceMap SourceMap { get; } = new();

    public string GetSourceSubstring(SourceSpan span)
    {
        if (!HasSourceCode())
            throw new InvalidOperationException($"Must supply source with {nameof(SetSourceCode)}");

        span.AsZeroIndexed(out var startLine, out var startColumn, out var endLine, out var endColumn);

        StringBuilder sb = new();

        for (int currentLineIndex = startLine; currentLineIndex <= endLine; ++currentLineIndex)
        {
            var line = _sourceCodeLines[currentLineIndex];

            Index lineStartIndex = currentLineIndex == startLine
                ? startColumn
                : 0;

            Index lineEndIndex = currentLineIndex == endLine
                ? endColumn
                : ^0;

            sb.AppendLine(line[lineStartIndex..lineEndIndex]);
        }

        return sb.ToString();
    }

    public void SetSourceCode(string source)
    {
        _sourceCodeLines = source.Split('\n')
            .Select(s => s.TrimEnd('\r'))
            .ToArray();
    }

    public bool HasSourceCode() => _sourceCodeLines is not null;
}

[CLSCompliant(false)]
public class SourceMap : Dictionary<uint, SourceSpan>
{
    public Entry GetLocationFromPosition(SourcePosition pos)
    {
        var foundLocation = this
            .Where(kvp =>
            {
                var offset = kvp.Key;
                var span = kvp.Value;
                return span.Contains(pos);
            })
            .OrderBy(kvp => kvp.Value.Size)
            .FirstOrDefault();

        if (foundLocation.Equals(default(KeyValuePair<uint, SourceSpan>)))
            return null;

        return new Entry(foundLocation.Key, foundLocation.Value);
    }

    public Entry GetLocationFromOffset(uint offset) => new(offset, this[offset]);

    public sealed class Entry(uint offset, SourceSpan span)
    {
        public uint Offset { get; } = offset;
        public SourceSpan Span { get; } = span;
    }
}

[DebuggerDisplay("L{Line}C{Column}")]
public class SourcePosition : IComparable<SourcePosition>, IEquatable<SourcePosition>
{
    public int Line { get; }

    public int Column { get; }

    internal ulong Value => ((ulong)Line << (sizeof(uint) * 8)) | (uint)Column;

    public SourcePosition(int line, int column)
    {
        if (line <= 0)
            throw new ArgumentOutOfRangeException(nameof(line));

        if (column <= 0)
            throw new ArgumentOutOfRangeException(nameof(line));

        Line = line;
        Column = column;
    }

    public void AsZeroIndexed(out int line, out int column)
    {
        line = Line - 1;
        column = Column - 1;
    }

    public int CompareTo(SourcePosition other) => Value.CompareTo(other.Value);

    public bool Equals(SourcePosition other) => Value == other.Value;

    public override bool Equals(object obj)
    {
        if (obj is not SourcePosition other)
            return false;
        return Equals(other);
    }

    public override int GetHashCode()
    {
        int hashCode = 533871040;
        hashCode = hashCode * -1521134295 + Line.GetHashCode();
        hashCode = hashCode * -1521134295 + Column.GetHashCode();
        hashCode = hashCode * -1521134295 + Value.GetHashCode();
        return hashCode;
    }

    public static bool operator <(SourcePosition left, SourcePosition right) => left.CompareTo(right) < 0;

    public static bool operator >(SourcePosition left, SourcePosition right) => left.CompareTo(right) > 0;

    public static bool operator <=(SourcePosition left, SourcePosition right) => left.CompareTo(right) <= 0;

    public static bool operator >=(SourcePosition left, SourcePosition right) => left.CompareTo(right) >= 0;

    public static bool operator ==(SourcePosition left, SourcePosition right) => left.Equals(right);

    public static bool operator !=(SourcePosition left, SourcePosition right) => !(left == right);
}

[DebuggerDisplay("[{Start}..{End}]")]
public class SourceSpan
{
    public SourceSpan(SourcePosition start, SourcePosition end)
    {
        if (start > end)
            throw new ArgumentException($"Source start index must be at or after end index.");

        Start = start;
        End = end;
    }

    public SourcePosition Start { get; }

    public SourcePosition End { get; }

    internal ulong Size => End.Value - Start.Value;

    public bool Contains(SourcePosition pos) => Start <= pos && pos < End;

    public void AsZeroIndexed(out int startLine, out int startColumn, out int endLine, out int endColumn)
    {
        Start.AsZeroIndexed(out startLine, out startColumn);
        End.AsZeroIndexed(out endLine, out endColumn);
    }
}

