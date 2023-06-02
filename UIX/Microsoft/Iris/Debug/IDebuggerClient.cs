using Microsoft.Iris.Debug.Data;
using System;

namespace Microsoft.Iris.Debug;

public interface IDebuggerClient
{
    /// <summary>
    /// The URI the client is connected to.
    /// </summary>
    Uri ConnectionUri { get; }

    /// <summary>
    /// Fired when the UIX interpreter steps forward.
    /// </summary>
    event EventHandler<InterpreterEntry> InterpreterStep;

    /// <summary>
    /// Fired when the UIX dispatcher executes another call from the queue.
    /// </summary>
    event Action<string> DispatcherStep;
}
