using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Iris.Debug.Data
{
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
            var args =
#if OPENZUNE
                Arguments;
#else
                Arguments.Select(a => a.ToString()).ToArray();
#endif
            return $"{OpCode}({string.Join(", ", args)})";
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
}
