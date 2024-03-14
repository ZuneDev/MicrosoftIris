using Microsoft.Iris;
using Microsoft.Iris.Debug;

namespace IrisShell;

public abstract class IrisAppBase
{
    public abstract string Title { get; }

    public abstract string WindowSource { get; }

    public virtual WindowColor WindowColor { get; } = new(255, 255, 255);

    public virtual bool EnableDebugging { get; } =
#if DEBUG
        true;
#else
        false;
#endif

    [STAThread]
    public int Run(string[] args)
    {
        Application.ErrorReport += OnErrorReport;

        if (EnableDebugging)
            DebuggerSetup(args);

        Application.Initialize();

        Application.Window.Caption = Title;
        Application.Window.RequestLoad(WindowSource);

        //Application.Window.InitialClientSize = windowSize;
        Application.Window.ShowWindowFrame = true;
        Application.Window.SetBackgroundColor(WindowColor);
        Application.Run(OnInitialLoadComplete);

        Application.Shutdown();

        return 0;
    }

    protected virtual void OnErrorReport(Error[] errors)
    {
        foreach (Error error in errors)
        {
#if DEBUG
            System.Diagnostics.Debug
#else
            Console
#endif
                .WriteLine(error.ToString());
        }
    }

    protected virtual void OnInitialLoadComplete(object arg)
    {
    }

    protected virtual void DebuggerSetup(string[] args)
    {
        Application.DebuggerServerReady += (_, __) =>
        {
            var textColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Debugger server ready at {Application.DebugSettings.DebugConnectionUri}");
            Console.ForegroundColor = textColor;
        };

        Application.DebugSettings.DebugConnectionUri = args.Length >= 2
            ? args[1] : DebugRemoting.DEFAULT_TCP_URI.OriginalString;
    }
}
