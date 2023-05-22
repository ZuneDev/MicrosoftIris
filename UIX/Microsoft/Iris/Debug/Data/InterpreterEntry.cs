using System;
using System.Collections.Generic;

namespace Microsoft.Iris.Debug.Data;

public class InterpreterEntry
{
    public InterpreterEntry(object opCode, params OpCodeArgument[] args)
    {
        OpCode = opCode;

        if (args != null && args.Length > 0)
            Arguments = args;
        else
            Arguments = new List<OpCodeArgument>();
    }

    public object OpCode { get; }
    public IList<OpCodeArgument> Arguments { get; }
    public IList<object> ReturnValues { get; } = new List<object>();

    public override string ToString()
    {
        return $"{OpCode}({string.Join(", ", Arguments)}) -> [{ReturnValues}]";
    }
}

public class OpCodeArgument
{
    public string Name { get; set; }
    public Type Type { get; set; }
    public object Value { get; set; }

    public OpCodeArgument(string name, Type type, object value)
    {
        Name = name;
        Type = type;
        Value = value;
    }

    public override string ToString() => $"{Type} {Value}";
}
