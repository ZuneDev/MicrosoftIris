using Microsoft.Iris.Debug;
using Microsoft.Iris.Debug.Data;
using Microsoft.Iris.Debug.SystemNet;
using System;
using System.Threading;

namespace SimpleDebugClient;

internal class Program
{
    static IDebuggerClient? Debugger { get; set; }

    static int Main(string[] args)
    {
        var connectionString = args.Length >= 2
            ? new Uri(args[1]) : DebugRemoting.DEFAULT_TCP_URI;

        Debugger = new NetDebuggerClient(connectionString);
        //Debugger.DispatcherStep += Debugger_DispatcherStep;
        Debugger.InterpreterStep += Debugger_InterpreterStep;
        Debugger.InterpreterStateChanged += Debugger_InterpreterStateChanged;

        Console.WriteLine($"Listening for debug messages at '{Debugger.ConnectionUri}'. Press Ctrl-C or 'x' to exit.");
        
        while (true)
        {
            var cmd = Console.ReadLine();
            if (cmd[0] == 'x')
            {
                if (Debugger is IDisposable debugger)
                    debugger.Dispose();
                break;
            }

            switch (cmd[0])
            {
                case 's':
                    Debugger.DebuggerCommand = InterpreterCommand.Step;
                    break;

                case 'c':
                    Debugger.DebuggerCommand = InterpreterCommand.Continue;
                    break;
            }
        }

        return 0;
    }

    private static void Debugger_InterpreterStateChanged(InterpreterCommand state)
    {
        Console.WriteLine($"Interpreter is in {state} mode");
    }

    private static void Debugger_DispatcherStep(string obj)
    {
        Console.WriteLine($"[Dispatcher] {obj}");
    }

    private static void Debugger_InterpreterStep(object? sender, InterpreterEntry e)
    {
        if (e.LoadUri.EndsWith("TopToolbarSignIn.uix"))
            return;
        Console.WriteLine($"[Interpreter] {e}");
    }
}