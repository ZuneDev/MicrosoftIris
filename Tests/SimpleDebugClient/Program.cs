using Microsoft.Iris.Debug;
using Microsoft.Iris.Debug.Data;
using Microsoft.Iris.Debug.SystemNet;
using System;

namespace SimpleDebugClient;

internal class Program
{
    static IDebuggerClient? Debugger { get; set; }

    static int Main(string[] args)
    {
        var connectionString = args.Length >= 2
            ? new Uri(args[1]) : DebugRemoting.DEFAULT_TCP_URI;

        Console.CancelKeyPress += Console_CancelKeyPress;

        Debugger = new NetDebuggerClient(connectionString);
        Debugger.DispatcherStep += Debugger_DispatcherStep;
        Debugger.InterpreterStep += Debugger_InterpreterStep;

        Console.WriteLine("Listening for debug messages. Press Ctrl-C to exit.");
        Console.ReadLine();

        return 0;
    }

    private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
    {
        if (Debugger is IDisposable debugger)
            debugger.Dispose();
    }

    private static void Debugger_DispatcherStep(string obj)
    {
        Console.WriteLine($"[Dispatcher] {obj}");
    }

    private static void Debugger_InterpreterStep(object? sender, InterpreterEntry e)
    {
        Console.WriteLine($"[Interpreter] {e}");
    }
}