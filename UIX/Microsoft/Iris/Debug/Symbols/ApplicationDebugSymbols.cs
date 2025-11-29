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

    public (uint Offset, SourceSpan Span) GetLocationFromPosition(SourcePosition pos)
    {
        var foundLocation = SourceMap.Xml.FirstOrDefault(kvp =>
        {
            var offset = kvp.Key;
            var span = kvp.Value;
            return span.Contains(pos);
        });

        if (foundLocation.Equals(default(KeyValuePair<uint, SourceSpan>)))
            throw new KeyNotFoundException($"No offset was assigned to line {pos.Line}, column {pos.Column}");

        return (foundLocation.Key, foundLocation.Value);
    }

    public uint OffsetByLineAndColumn(int line, int col) => OffsetByLineAndColumn(new(line, col));

    public SourceSpan GetContainingSpan(SourcePosition pos) => GetLocationFromPosition(pos).Span;

    public uint OffsetByLineAndColumn(SourcePosition pos) => GetLocationFromPosition(pos).Offset;

    public string GetSourceSubstring(SourceSpan span)
    {
        if (!HasSourceCode())
            throw new InvalidOperationException($"Must supply source with {nameof(SetSourceCode)}");

        span.Start.AsZeroIndexed(out var startLine, out var startColumn);
        span.End.AsZeroIndexed(out var endLine, out var endColumn);

        StringBuilder sb = new();

        int currentLineIndex = startLine;
        while (currentLineIndex <= endLine)
        {
            var line = _sourceCodeLines[currentLineIndex];

            Index lineStartIndex = currentLineIndex == startLine
                ? startColumn
                : 0;

            Index lineEndIndex = currentLineIndex == endLine
                ? endColumn
                : ^1;

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
public class SourceMap
{
    public Dictionary<uint, SourceSpan> Xml { get; } = [];
    public Dictionary<uint, Tuple<int, int>> Script { get; } = [];
}

[DebuggerDisplay("L{Line}C{Column}")]
public class SourcePosition : IComparable<SourcePosition>, IEquatable<SourcePosition>
{
    public int Line { get; }

    public int Column { get; }

    private ulong Position => ((ulong)Line << (sizeof(uint) * 8)) | (uint)Column;

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

    public int CompareTo(SourcePosition other) => Position.CompareTo(other.Position);

    public bool Equals(SourcePosition other) => Position == other.Position;

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
        hashCode = hashCode * -1521134295 + Position.GetHashCode();
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

    public bool Contains(SourcePosition pos) => Start >= pos && pos < End;
}

