using Microsoft.Iris.Markup;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.Iris.Debug.Data;

[Serializable]
public class InterpreterEntry
{
    public InterpreterEntry(OpCode opCode, uint offset, string loadUri, params OpCodeArgument[] args)
    {
        OpCode = opCode;
        Offset = offset;
        LoadUri = loadUri;

        if (args != null && args.Length > 0)
            Arguments = args;
        else
            Arguments = new List<OpCodeArgument>();
    }

    public OpCode OpCode { get; }

    public uint Offset { get; }

    public string LoadUri { get; }

    public IList<OpCodeArgument> Arguments { get; }

    public IList<object> ReturnValues { get; } = new List<object>();

    public override string ToString()
    {
        StringBuilder sb = new($"[{LoadUri} @ 0x{Offset:X}] {OpCode}({string.Join(", ", Arguments)})");

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

[Serializable]
public class OpCodeArgument : ISerializable
{
    public string Name { get; set; }
    public Type Type { get; set; }
    public object Value { get; set; }

    protected bool CanSerializeValue => Value is ISerializable;

    public OpCodeArgument(string name, Type type, object value)
    {
        Name = name;
        Type = type;
        Value = value;
    }

    protected OpCodeArgument(SerializationInfo info, StreamingContext context)
    {
        Name = info.GetString(nameof(Name));
        Type = Type.GetType(info.GetString(nameof(Type)), false);

        bool canSerializeValue = info.GetBoolean(nameof(CanSerializeValue));
        Type valueSerializeType = canSerializeValue ? Type : typeof(string);
        Value = info.GetValue(nameof(Value), valueSerializeType);
    }

    public override string ToString() => $"{Type} {Value}";

    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue(nameof(Name), Name);
        info.AddValue(nameof(Type), Type.FullName);
        info.AddValue(nameof(CanSerializeValue), CanSerializeValue);
        info.AddValue(nameof(Value), CanSerializeValue ? Value : Value?.ToString());
    }
}
