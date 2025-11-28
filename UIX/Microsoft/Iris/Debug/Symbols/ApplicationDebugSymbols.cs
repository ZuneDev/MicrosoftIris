using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.Iris.Debug.Symbols;

public class ApplicationDebugSymbols
{
    public List<FileDebugSymbols> Files { get; set; }
}

public class FileDebugSymbols
{
    private string[] _sourceCodeLines;

    public string OriginalFileName { get; set; }

    public string CompiledFileName { get; set; }

    public string SourceFileName { get; set; }

    public Dictionary<uint, SourceSpan> SourceMap { get; set; }

    public uint OffsetByLineAndColumn(int line, int col) => OffsetByLineAndColumn(new(line, col));

    public uint OffsetByLineAndColumn(SourcePosition pos)
    {
        Assert.IsNotNull(_sourceCodeLines, nameof(_sourceCodeLines));

        return SourceMap.First(kvp =>
        {
            var offset = kvp.Key;
            var span = kvp.Value;
            return span.Contains(pos);
        }).Key;
    }

    public void SetSourceCode(string source)
    {
        _sourceCodeLines = source.Split('\n')
            .Select(s => s.TrimEnd('\r'))
            .ToArray();
    }
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

