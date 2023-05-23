using Microsoft.Iris.Debug.Data;
using System;

namespace Microsoft.Iris.Debug;

public interface IDebuggerClient
{
    /// <summary>
    /// Fired when the UIX interpreter steps forward.
    /// </summary>
    event EventHandler<InterpreterEntry> InterpreterStep;

    /// <summary>
    /// Fired when the UIX dispatcher executes another call from the queue.
    /// </summary>
    event Action<string> DispatcherStep;
}
