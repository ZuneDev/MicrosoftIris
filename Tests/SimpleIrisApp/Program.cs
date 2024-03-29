﻿using Microsoft.Iris;
using Microsoft.Iris.Debug;
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
            ? args[1] : DebugRemoting.DEFAULT_TCP_URI.OriginalString;

        Application.DebugSettings.Breakpoints.Add(new("clr-res://SimpleIrisApp!MainPage.uix", 3, 22));
#endif

        Console.WriteLine("Initializing Iris...");
        Application.Initialize();

        Console.WriteLine("Loading content...");
        Application.Window.Caption = "Iris app";
        Application.Window.RequestLoad("clr-res://SimpleIrisApp!MainPage.uix#Frame");

        Application.Run(OnInitialLoadComplete);
        Application.Shutdown();
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