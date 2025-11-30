using Microsoft.Iris.Debug.Data;
using System;

namespace Microsoft.Iris.Debug;

public interface IDebuggerState
{
    void Start();

    InterpreterCommand DebuggerCommand { get; set; }

    //IObservableCollection<Breakpoint> Breakpoints { get; }
}

public interface IRemoteDebuggerState : IDebuggerState
{
    /// <summary>
    /// The URI the client is connected to.
    /// </summary>
    public string ConnectionString { get; }

    /// <summary>
    /// Fired when a connection is established.
    /// </summary>
    public event Action<IDebuggerState, object> Connected;
}
