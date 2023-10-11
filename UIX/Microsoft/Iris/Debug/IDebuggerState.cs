using Microsoft.Iris.Debug.Data;
using System;

namespace Microsoft.Iris.Debug;

public interface IDebuggerState
{
    /// <summary>
    /// The URI the client is connected to.
    /// </summary>
    public Uri ConnectionUri { get; }

    InterpreterCommand DebuggerCommand { get; set; }

    /// <summary>
    /// Fired when a connection is established.
    /// </summary>
    public event Action<IDebuggerState, object> Connected;
}
