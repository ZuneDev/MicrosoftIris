using Microsoft.Iris.Markup;
using System;
using System.Collections.Generic;

namespace Microsoft.Iris.Debug.Data;

[Serializable]
public class InterpreterInstruction : IComparable<InterpreterInstruction>
{
    public InterpreterInstruction() { }

    public InterpreterInstruction(OpCode opCode, uint offset, string loadUri)
    {
        OpCode = opCode;
        Offset = offset;
        LoadUri = loadUri;
    }

    public OpCode OpCode { get; set; }

    public uint Offset { get; set; }

    public string LoadUri { get; set; }

    public List<object> Operands { get; } = new();

    public int CompareTo(InterpreterInstruction other)
    {
        var stringCmp = string.Compare(LoadUri, other.LoadUri, StringComparison.Ordinal);
        return stringCmp != 0
            ? stringCmp
            : Offset.CompareTo(other.Offset);
    }

    public override string ToString() => $"[0x{Offset:X}] {OpCode} {string.Join(", ", Operands)}";
}
