using Microsoft.Iris;
using System.ComponentModel;

namespace IrisShell.UI;

public class Node : Command
{
    private bool _isCurrent;
    private bool _pendingNavigation;
    private string _command;

    public Node(Experience owner, string command)
      : this(owner, null, command)
    { }

    public Node(Experience owner, string? name, string command)
      : base(owner, name, null)
    {
        _command = command;
    }

    public Experience Experience => (Experience)Owner;

    public bool IsCurrent
    {
        get => _isCurrent;
        set
        {
            if (_isCurrent == value)
                return;
            _isCurrent = value;
            OnIsCurrentChanged();
            FirePropertyChanged(nameof(IsCurrent));
        }
    }

    protected virtual void OnIsCurrentChanged()
    {
    }

    protected override void OnInvoked()
    {
        Shell defaultInstance = (Shell)IrisShell.DefaultInstance!;
        if (defaultInstance.NavigationLocked)
        {
            defaultInstance.BlockedByNavigationLock = true;
            defaultInstance.DeferredNavigateNode = this;
        }
        else
        {
            bool flag = defaultInstance.CurrentNode == this;
            bool isRootPage = defaultInstance.CurrentPage.IsRootPage;
            if (!_pendingNavigation)
            {
                if (!flag || !isRootPage)
                {
                    defaultInstance.PropertyChanged += ShellPropertyChanged;
                    Execute(defaultInstance);
                    _pendingNavigation = true;
                }
                else
                    defaultInstance.CurrentPage.RefreshPage();
            }
            base.OnInvoked();
        }
    }

    protected virtual void Execute(Shell shell) => shell.Execute(_command, null);

    private void ShellPropertyChanged(object? sender, PropertyChangedEventArgs? args)
    {
        if (args?.PropertyName != "CurrentPage")
            return;

        _pendingNavigation = false;
        IrisShell.DefaultInstance!.PropertyChanged -= ShellPropertyChanged;
    }
}
