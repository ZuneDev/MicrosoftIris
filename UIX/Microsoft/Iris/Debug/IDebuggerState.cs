using Microsoft.Iris.Debug.Data;
using System;

namespace Microsoft.Iris.Debug;

public interface IDebuggerState
{
    InterpreterCommand DebuggerCommand { get; set; }
}

public interface IRemoteDebuggerState : IDebuggerState
{
    /// <summary>
    /// The URI the client is connected to.
    /// </summary>
    public Uri ConnectionUri { get; }

    /// <summary>
    /// Fired when a connection is established.
    /// </summary>
    public event Action<IDebuggerState, object> Connected;
}
