using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.Iris.Debug.Symbols;

public class ApplicationDebugSymbols
{
    public List<FileDebugSymbols> Files { get; set; }
}

public class FileDebugSymbols
{
    public string SourceFileName { get; set; }

    public string CompiledFileName { get; set; }

    public Dictionary<uint, ScriptDebugSymbols> ScriptSymbols { get; set; }
}

public class ScriptDebugSymbols
{
    public string SourceCode { get; set; }

    public Dictionary<uint, SourceSpan> SourceMap { get; set; }
}

[DebuggerDisplay("[{Start}..{End}]")]
public class SourceSpan
{
    public SourceSpan(int start, int end)
    {
        if (start > end)
            throw new ArgumentException($"Source start index must be at or after end index.");

        Start = start;
        End = end;
    }

    public int Start { get; set; }

    public int End { get; set; }

    public int GetLength() => End - Start;
}

