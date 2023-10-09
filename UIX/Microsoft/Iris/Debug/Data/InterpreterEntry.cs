using Microsoft.Iris.Markup;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Iris.Debug.Data;

[Serializable]
public class InterpreterEntry : IComparable<InterpreterEntry>
{
    public InterpreterEntry() { }

    public InterpreterEntry(InterpreterInstruction instruction)
    {
        Instruction = instruction;
    }

    public InterpreterInstruction Instruction { get; }

    public List<InterpreterObject> Parameters { get; } = new();

    public List<InterpreterObject> ReturnValues { get; } = new();

    public string InstructionString => ToInstructionString();

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
        sb.AppendFormat("0x{0:X4}", Instruction.Offset);
        sb.Append('\t');
        sb.Append(Instruction.OpCode);
        sb.Append(' ', 2);

        for (int p = 0; p < Parameters.Count; ++p)
        {
            var param = Parameters[p];

            if (p != 0)
                sb.Append(", ");

            param.ToString(sb, false);
        }

        return sb.ToString();
    }

    public int CompareTo(InterpreterEntry other) => Instruction.CompareTo(other.Instruction);
}
