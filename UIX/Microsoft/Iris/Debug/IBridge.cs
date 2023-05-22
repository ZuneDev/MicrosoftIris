using System;

namespace Microsoft.Iris.Debug;

/// <summary>
/// The contract between the UIX runtime and debuggers.
/// </summary>
public interface IBridge
{
    /// <summary>
    /// Fired when the UIX interpreter steps forward.
    /// </summary>
    event EventHandler<Data.InterpreterEntry> InterpreterStep;

    /// <summary>
    /// Fired when the UIX dispatcher executes another call from the queue.
    /// </summary>
    event Action<string> DispatcherStep;

    /// <summary>
    /// Logs the context, opcode, and arguments of an instruction
    /// executed by <c>Microsoft.Iris.Markup.Interpreter</c>.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="entry"></param>
    public void LogInterpreterOpCode(object context, Data.InterpreterEntry entry);

    /// <summary>
    /// Logs the string representation of a dispatcher step.
    /// </summary>
    /// <param name="message"></param>
    public void LogDispatcher(string message);
}
