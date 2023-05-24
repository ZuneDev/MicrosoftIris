using Microsoft.Iris.Markup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.Iris.Debug.Data;

[Serializable]
public class InterpreterEntry
{
    public InterpreterEntry(OpCode opCode, params OpCodeArgument[] args)
    {
        OpCode = opCode;

        if (args != null && args.Length > 0)
            Arguments = args;
        else
            Arguments = new List<OpCodeArgument>();
    }

    public OpCode OpCode { get; }
    public IList<OpCodeArgument> Arguments { get; }
    public IList<object> ReturnValues { get; } = new List<object>();

    public override string ToString()
    {
        return $"{OpCode}({string.Join(", ", Arguments)}) -> [{string.Join(", ", ReturnValues)}]";
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
        info.AddValue(nameof(Value), CanSerializeValue ? Value : Value.ToString());
    }
}
