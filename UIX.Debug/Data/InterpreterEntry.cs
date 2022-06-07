using System;
using System.Collections.Generic;

namespace Microsoft.Iris.Debug.Data
{
    public class InterpreterEntry
    {
        public InterpreterEntry(object opCode, params OpCodeArgument[] args)
        {
            OpCode = opCode;
            Arguments = args;
        }

        public object OpCode { get; private set; }
        public OpCodeArgument[] Arguments { get; private set; }

        public override string ToString() => $"{OpCode}({string.Join(", ", (IEnumerable<OpCodeArgument>)Arguments)})";
    }

    public class OpCodeArgument
    {
        public Type Type { get; private set; }
        public object Value { get; private set; }

        public override string ToString() => $"{Type} {Value}";
    }
}
