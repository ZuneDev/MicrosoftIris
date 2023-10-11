using Microsoft.Iris.Debug.Data;
using System;

namespace Microsoft.Iris.Debug;

public interface IDebuggerClient : IDebuggerState
{
    event Action<InterpreterCommand> InterpreterStateChanged;

    /// <summary>
    /// Fired when the UIX interpreter decodes an instruction.
    /// </summary>
    event EventHandler<InterpreterInstruction> InterpreterDecode;

    /// <summary>
    /// Fired when the UIX interpreter executes an instruction.
    /// </summary>
    event EventHandler<InterpreterEntry> InterpreterExecute;

    /// <summary>
    /// Fired when the UIX dispatcher executes another call from the queue.
    /// </summary>
    event Action<string> DispatcherStep;

    void UpdateBreakpoint(Breakpoint breakpoint);
}
