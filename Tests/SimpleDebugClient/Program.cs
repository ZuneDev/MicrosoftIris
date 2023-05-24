using Microsoft.Iris.Debug;
using Microsoft.Iris.Debug.Data;
using System;

namespace SimpleDebugClient;

internal class Program
{
    static IDebuggerClient? Debugger { get; set; }

    static void Main(string[] args)
    {
        string connectionString = args.Length >= 2
            ? args[1] : "tcp://127.0.0.1:5556";

        Console.CancelKeyPress += Console_CancelKeyPress;

        Debugger = new ZmqDebuggerClient(connectionString);
        Debugger.DispatcherStep += Debugger_DispatcherStep;
        Debugger.InterpreterStep += Debugger_InterpreterStep;

        Console.WriteLine("Listening for debug messages. Press Ctrl-C to exit.");
        Console.ReadLine();
    }

    private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
    {
        if (Debugger is IDisposable debugger)
            debugger.Dispose();
    }

    private static void Debugger_DispatcherStep(string obj)
    {
        Console.WriteLine(obj);
    }

    private static void Debugger_InterpreterStep(object? sender, InterpreterEntry e)
    {
        Console.WriteLine(e);
    }
}