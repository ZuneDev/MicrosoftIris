using Microsoft.Iris.Markup;
using System;

namespace Microsoft.Iris.Debug.Data;

[Serializable]
public class InterpreterInstruction : IComparable<InterpreterInstruction>
{
    public InterpreterInstruction(OpCode opCode, uint offset, string loadUri)
    {
        OpCode = opCode;
        Offset = offset;
        LoadUri = loadUri;
    }

    public OpCode OpCode { get; set; }

    public uint Offset { get; set; }

    public string LoadUri { get; set; }

    public int CompareTo(InterpreterInstruction other)
    {
        int stringCmp = LoadUri.CompareTo(other.LoadUri);
        return stringCmp != 0
            ? stringCmp
            : Offset.CompareTo(other.Offset);
    }
}
