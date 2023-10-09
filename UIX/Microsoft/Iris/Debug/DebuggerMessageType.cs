namespace Microsoft.Iris.Debug;

public enum DebuggerMessageType : int
{
    Null = 0,

    InterpreterDecode,
    InterpreterExecute,
    DispatcherStep,

    UpdateBreakpoint,
    InterpreterCommand,
}
