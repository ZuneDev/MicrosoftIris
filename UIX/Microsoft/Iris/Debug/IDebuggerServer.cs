﻿using Microsoft.Iris.Debug.Data;

namespace Microsoft.Iris.Debug;

public interface IDebuggerServer : IDebuggerState
{
    /// <summary>
    /// Sends the requested line number table.
    /// </summary>
    MarkupLineNumberEntry[] OnLineNumberTableRequested(string uri);

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

    /// <summary>
    /// Waits for the debugger to continue execution. Will block the current thread.
    /// </summary>
    void WaitForContinue();
}
