using Microsoft.Iris.Debug.Data;

namespace Microsoft.Iris.Debug;

internal interface IDebuggerServer
{
    InterpreterCommand DebuggerCommand { get; set; }

    /// <summary>
    /// Logs the context, opcode, and operands of an instruction
    /// decoded by <c>Microsoft.Iris.Markup.Interpreter</c>.
    /// </summary>
    void LogInterpreterDecode(object context, InterpreterInstruction instruction);

    /// <summary>
    /// Logs the context, opcode, arguments, and results of an instruction
    /// executed by <c>Microsoft.Iris.Markup.Interpreter</c>.
    /// </summary>
    void LogInterpreterExecute(object context, InterpreterEntry entry);

    /// <summary>
    /// Logs the string representation of a dispatcher step.
    /// </summary>
    /// <param name="message"></param>
    void LogDispatcher(string message);
}
