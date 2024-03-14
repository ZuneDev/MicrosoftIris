// Decompiled with JetBrains decompiler
// Type: ZuneUI.ZuneShell
// Assembly: ZuneShell, Version=4.7.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: FC8028F3-A47B-4FB4-B35B-11D1752D8264
// Assembly location: C:\Program Files\Zune\ZuneShell.dll

using IrisShell.Controls;
using Microsoft.Iris;
using System.Collections;
using System.ComponentModel;

namespace IrisShell.UI;

public class IrisShell : ModelItem
{
    public Node DeferredNavigateNode;
    private static IrisShell? s_defaultInstance;
    private PageStack _pageStack;
    private Command _navigateBackCommand;
    private ICommandHandler _commandHandler;
    private int _navigationsToPagePending;
    private bool _navigationLocked;
    private object thisLock = new();

    public IrisShell()
    {
        _pageStack = new PageStack(this);
        _pageStack.PropertyChanged += OnPageStackPropertyChanged;
        _navigateBackCommand = new Command(this, Shell.LoadString("IDS_NAVIGATE_BACK"), OnClickNavigateBack);
        _pageStack.MaximumStackSize = 1024U;
        _pageStack.NavigateToPage(new StartupPage());
        DefaultInstance = this;
    }

    protected override void OnDispose(bool disposing)
    {
        base.OnDispose(disposing);
        if (!disposing || DefaultInstance != this)
            return;
        DefaultInstance = null;
    }

    public Command NavigateBackCommand => _navigateBackCommand;

    private void OnPageStackPropertyChanged(object? sender, PropertyChangedEventArgs args)
    {
        string propertyName = args.PropertyName ?? throw new ArgumentNullException(nameof(args.PropertyName));
        if (!(propertyName == "CurrentPage") && !(propertyName == "CanNavigateBack") && !(propertyName == "LastNavigationDirection") && !(propertyName == "MaximumStackSize"))
            return;
        FirePropertyChanged(propertyName);
        if (!(propertyName == "CurrentPage") && !(propertyName == "CanNavigateBack"))
            return;
        _navigateBackCommand.Available = CanNavigateBack && (CurrentPage?.ShowBackArrow ?? false);
    }

    private void OnClickNavigateBack(object? sender, EventArgs args) => NavigateBack();

    public int MaximumStackSize
    {
        get => (int)_pageStack.MaximumStackSize;
        set => _pageStack.MaximumStackSize = value >= 0 ? (uint)value : throw new ArgumentOutOfRangeException(nameof(value));
    }

    public IrisPage? CurrentPage => _pageStack.CurrentPage as IrisPage;

    public bool CanNavigateBack => _pageStack.CanNavigateBack;

    public bool NavigationLocked
    {
        get => _navigationLocked;
        set
        {
            if (_navigationLocked == value)
                return;
            _navigationLocked = value;
            FirePropertyChanged(nameof(NavigationLocked));
        }
    }

    public bool BlockedByNavigationLock
    {
        get => true;
        set => FirePropertyChanged(nameof(BlockedByNavigationLock));
    }

    public NavigationDirection LastNavigationDirection => _pageStack.LastNavigationDirection;

    public ICommandHandler CommandHandler
    {
        get => _commandHandler;
        set
        {
            if (_commandHandler == value)
                return;
            _commandHandler = value;
            FirePropertyChanged(nameof(CommandHandler));
        }
    }

    public static IrisShell? DefaultInstance
    {
        get => s_defaultInstance;
        private set
        {
            if (s_defaultInstance is not null && s_defaultInstance != value)
                throw new InvalidOperationException("Should only have one static shell instance.");

            s_defaultInstance = value; 
        }
    }

    public bool NavigationsPending => _navigationsToPagePending > 0;

    public void NavigateToPage(IrisPage page)
    {
        lock (thisLock)
            ++_navigationsToPagePending;
        Application.DeferredInvoke(new DeferredInvokeHandler(DeferredNavigateToPage), page);
    }

    private void DeferredNavigateToPage(object args)
    {
        IrisPage zunePage = (IrisPage)args;
        if (CurrentPage == null || CurrentPage.CanNavigateForwardTo(zunePage))
            _pageStack.NavigateToPage(zunePage);
        else
            zunePage.Release();
        lock (thisLock)
            --_navigationsToPagePending;
    }

    public void NavigateBack() => NavigateBack(false);

    public void NavigateBack(bool bypassPage) => Application.DeferredInvoke(new DeferredInvokeHandler(DeferredNavigateBack), bypassPage);

    private void DeferredNavigateBack(object args)
    {
        if (!(bool)args && CurrentPage != null && CurrentPage.HandleBack())
            return;
        _pageStack.NavigateBack();
    }

    public void Execute(string command, IDictionary? commandArguments)
    {
        if (_commandHandler == null)
            throw new InvalidOperationException("No CommandHandler has been registered.  Unable to resolve shell command: " + command);
        _commandHandler.Execute(command, commandArguments);
    }

    //public void LaunchHelp() => this.Execute(InternetConnection.Instance.IsConnected ? "Web\\" + CultureHelper.GetHelpUrl() : "Help\\" + Shell.LoadString(StringId.IDS_ZUNECLIENT_LOCALE) + "\\help.htm", null);
}
