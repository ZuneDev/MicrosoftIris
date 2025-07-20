using Microsoft.Iris.Debug.Data;
using Microsoft.Iris.Markup;
using System;
using System.Diagnostics;

namespace Microsoft.Iris.Debug;

public class InProcDebugger : IDebuggerClient, IDebuggerServer
{
    public InterpreterCommand DebuggerCommand { get; set; } = InterpreterCommand.Continue;

    public event Action<InterpreterCommand> InterpreterStateChanged;
    public event EventHandler<InterpreterInstruction> InterpreterDecode;
    public event EventHandler<InterpreterEntry> InterpreterExecute;
    public event Action<string> DispatcherStep;

    public void RequestLineNumberTable(string uri, Action<MarkupLineNumberEntry[]> callback)
    {
        var lineNumberTable = ((IDebuggerServer)this).OnLineNumberTableRequested(uri);
        callback(lineNumberTable);
    }

    public void UpdateBreakpoint(Breakpoint breakpoint)
    {
        if (breakpoint.Enabled)
            Application.DebugSettings.Breakpoints.Add(breakpoint);
        else
            Application.DebugSettings.Breakpoints.Remove(breakpoint);
    }

    void IDebuggerServer.LogDispatcher(string message) => DispatcherStep?.Invoke(message);

    void IDebuggerServer.LogInterpreterDecode(object context, InterpreterInstruction instruction) => InterpreterDecode?.Invoke(context, instruction);

    void IDebuggerServer.LogInterpreterExecute(object context, InterpreterEntry entry) => InterpreterExecute?.Invoke(context, entry);

    MarkupLineNumberEntry[] IDebuggerServer.OnLineNumberTableRequested(string uri)
    {
        var loadResult = (MarkupLoadResult)LoadResultCache.Read(uri);
        var lineNumberTable = loadResult.LineNumberTable.DumpTable();

        return lineNumberTable;
    }

    void IDebuggerServer.WaitForContinue()
    {
        if (DebuggerCommand is InterpreterCommand.Break)
            Debugger.Break();
    }
}
