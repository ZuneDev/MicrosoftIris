using Microsoft.Iris;
using System;

namespace SimpleIrisApp;

internal class Program
{
    static void Main(string[] args)
    {
#if DEBUG
        Console.WriteLine("Starting debugger server...");

        Application.DebuggerServerReady += (_, __) =>
        {
            var textColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Debugger server ready at {Application.DebugSettings.DebugConnectionUri}");
            Console.ForegroundColor = textColor;
        };

        Application.DebugSettings.DebugConnectionUri = args.Length >= 2
            ? args[1] : "tcp://127.0.0.1:5556";
#endif

        Console.WriteLine("Initializing Iris...");
        Application.Initialize();

        Console.WriteLine("Loading content...");
        Application.Window.Caption = "Iris app";
        Application.Window.RequestLoad("clr-res://SimpleIrisApp!MainPage.uix#Frame");

        Application.Run(OnInitialLoadComplete);
    }

    static void OnInitialLoadComplete(object arg)
    {
        Console.WriteLine("Initial load complete");

        var rootUi = Application.Window.Form.Zone.RootUI;
        var rootView = Application.Window.Form.Zone.RootViewItem;

        rootView.DeepLayoutChange += RootView_DeepParentChange;

        Console.WriteLine("Visual tree ready");
    }

    private static void RootView_DeepParentChange(object? sender, EventArgs e)
    {

    }
}