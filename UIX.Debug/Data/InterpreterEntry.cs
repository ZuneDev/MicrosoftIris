using System;
using System.Linq;

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

        public override string ToString()
        {
            var args =
#if ZUNE5
                (System.Collections.Generic.IEnumerable<OpCodeArgument>)Arguments;
#else
                Arguments.Select(a => a.ToString()).ToArray();
#endif
            return $"{OpCode}({string.Join(", ", args)})";
        }
    }

    public class OpCodeArgument
    {
        public Type Type { get; private set; }
        public object Value { get; private set; }

        public override string ToString() => $"{Type} {Value}";
    }
}
