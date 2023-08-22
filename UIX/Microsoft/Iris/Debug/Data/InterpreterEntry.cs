using Microsoft.Iris.Markup;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Iris.Debug.Data;

[Serializable]
public class InterpreterEntry : IComparable<InterpreterEntry>
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

    public string InstructionString => ToInstructionString();

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

    public string ToInstructionString()
    {
        StringBuilder sb = new();
        sb.AppendFormat("0x{0:X4}", Offset);
        sb.Append('\t');
        sb.Append(OpCode);
        sb.Append(' ', 2);

        for (int p = 0; p < Parameters.Count; ++p)
        {
            var param = Parameters[p];

            if (p != 0)
                sb.Append(", ");

            if (param.Source == InstructionObjectSource.Inline)
                sb.Append(param.Value ?? "NULL");
            else
                sb.Append(param.Source);

            if (param.TableIndex != -1)
            {
                sb.Append('[');
                sb.Append(param.TableIndex);
                sb.Append(']');
            }
        }

        return sb.ToString();
    }

    public int CompareTo(InterpreterEntry other)
    {
        int stringCmp = LoadUri.CompareTo(other.LoadUri);
        return stringCmp != 0
            ? stringCmp
            : Offset.CompareTo(other.Offset);
    }
}
