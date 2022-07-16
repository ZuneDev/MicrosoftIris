using Microsoft.Iris;
using System;
using System.IO;

namespace SimpleIrisApp;

internal class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Launching Iris app...");

        Application.Initialize();

        Application.Window.Caption = "Iris app";
        Application.Window.RequestLoad("clr-res://SimpleIrisApp!MainPage.uix#Frame");

        Application.Run(OnInitialLoadComplete);
    }

    static void OnInitialLoadComplete(object arg)
    {
        Console.WriteLine("Initial load complete");
    }
}