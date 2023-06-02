using Microsoft.Iris.Debug.Data;

namespace Microsoft.Iris.Debug;

internal interface IDebuggerServer
{
    InterpreterCommand DebuggerCommand { get; set; }

    /// <summary>
    /// Logs the context, opcode, and arguments of an instruction
    /// executed by <c>Microsoft.Iris.Markup.Interpreter</c>.
    /// </summary>
    void LogInterpreterOpCode(object context, InterpreterEntry entry);

    /// <summary>
    /// Logs the string representation of a dispatcher step.
    /// </summary>
    /// <param name="message"></param>
    void LogDispatcher(string message);
}
