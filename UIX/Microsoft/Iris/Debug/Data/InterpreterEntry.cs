using Microsoft.Iris.Markup;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Iris.Debug.Data;

[Serializable]
public class InterpreterEntry
{
    public InterpreterEntry() { }

    public InterpreterEntry(OpCode opCode, uint offset, string loadUri)
    {
        OpCode = opCode;
        Offset = offset;
        LoadUri = loadUri;
    }

    public OpCode OpCode { get; set; }

    public uint Offset { get; set; }

    public string LoadUri { get; set; }

    public List<InterpreterObject> Parameters { get; } = new();

    public List<InterpreterObject> ReturnValues { get; } = new();

    public override string ToString()
    {
        StringBuilder sb = new($"[{LoadUri} @ 0x{Offset:X}] {OpCode}({string.Join(", ", Parameters)})");

        if (ReturnValues.Count > 0)
        {
            sb.Append(" -> ");

            if (ReturnValues.Count == 1)
                sb.Append(ReturnValues[0]);
            else
                sb.Append($"[{string.Join(", ", ReturnValues)}]");
        }

        return sb.ToString();
    }
}
