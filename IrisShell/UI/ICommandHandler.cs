using System.Collections;

namespace IrisShell.UI;

public interface ICommandHandler
{
    void Execute(string command, IDictionary? commandArgs);
}

public class CurrentPageCommandHandler : ICommandHandler
{
    public void Execute(string command, IDictionary? commandArgs)
    {
        var defaultInstance = IrisShell.DefaultInstance
            ?? throw new InvalidOperationException("No Shell instance has been registered.  Unable to perform navigation.");

        defaultInstance.CurrentPage?.CommandHandler?.Execute(command, commandArgs);
    }
}

public class DictionaryCommandHandler : ICommandHandler
{
    private Dictionary<string, ICommandHandler> _handlers;
    private string _divider;

    public DictionaryCommandHandler()
    {
        _handlers = [];
        _divider = "\\";
    }

    public IDictionary Handlers => _handlers;

    public string Divider
    {
        get => _divider;
        set => _divider = !string.IsNullOrEmpty(value)
            ? value
            : throw new ArgumentException("Must provide a non-empty divider.", nameof(value));
    }

    public void Execute(string command, IDictionary? commandArgs)
    {
        SplitCommand(command, out string prefix, out string suffix);
        ICommandHandler? commandHandler = null;

        if (_handlers.ContainsKey(prefix))
            commandHandler = _handlers[prefix];
        if (commandHandler == null)
            throw new ArgumentException("Unknown prefix: " + prefix, "prefix");

        commandHandler.Execute(suffix, commandArgs);
    }

    private void SplitCommand(string command, out string prefix, out string? suffix)
    {
        int length = !string.IsNullOrEmpty(command) ? command.IndexOf(_divider) : throw new ArgumentException("Must provide a non-empty command", nameof(command));
        if (length < 0)
        {
            prefix = command;
            suffix = null;
        }
        else
        {
            prefix = command.Substring(0, length);
            suffix = command.Substring(length + _divider.Length);
        }
    }
}
