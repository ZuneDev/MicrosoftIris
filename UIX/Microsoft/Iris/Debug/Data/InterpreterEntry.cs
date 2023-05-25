using Microsoft.Iris.Markup;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Iris.Debug.Data;

[Serializable]
public class InterpreterEntry
{
    public InterpreterEntry(OpCode opCode, uint offset, string loadUri, params InterpreterObject[] args)
    {
        OpCode = opCode;
        Offset = offset;
        LoadUri = loadUri;

        if (args != null && args.Length > 0)
            Parameters = args;
        else
            Parameters = new List<InterpreterObject>();
    }

    public OpCode OpCode { get; }

    public uint Offset { get; }

    public string LoadUri { get; }

    public IList<InterpreterObject> Parameters { get; }

    public IList<InterpreterObject> ReturnValues { get; } = new List<InterpreterObject>();

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
