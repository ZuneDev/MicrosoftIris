using IrisShell;
using Microsoft.Iris;
using System;

namespace SimpleIrisApp;

internal sealed class Program : IrisAppBase
{
    public override string Title => "Iris app";

    public override string WindowSource => "clr-res://SimpleIrisApp!MainPage.uix#Frame";

    public override bool EnableDebugging => false;

    [STAThread]
    static void Main(string[] args)
    {
        Program app = new();
        app.Run(args);
    }

    protected override void OnInitialLoadComplete(object arg)
    {
        base.OnInitialLoadComplete(arg);
    
        Console.WriteLine("Initial load complete");

        var rootUi = Application.Window.Form.Zone.RootUI;
        var rootView = Application.Window.Form.Zone.RootViewItem;

        rootView.DeepLayoutChange += RootView_DeepParentChange;

        Console.WriteLine("Visual tree ready");
    }

    protected override void DebuggerSetup(string[] args)
    {
        base.DebuggerSetup(args);

        Application.DebugSettings.Breakpoints.Add(new("clr-res://SimpleIrisApp!MainPage.uix", 3, 22));
    }

    private static void RootView_DeepParentChange(object? sender, EventArgs e)
    {
    }
}