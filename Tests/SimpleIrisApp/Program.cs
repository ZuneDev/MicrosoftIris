using Microsoft.Iris;
using System;
using System.IO;

namespace SimpleIrisApp;

internal class Program
{
    static void Main(string[] args)
    {
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
    }
}