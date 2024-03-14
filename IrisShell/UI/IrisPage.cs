// Decompiled with JetBrains decompiler
// Type: ZuneUI.ZunePage
// Assembly: ZuneShell, Version=4.7.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: FC8028F3-A47B-4FB4-B35B-11D1752D8264
// Assembly location: C:\Program Files\Zune\ZuneShell.dll

using Microsoft.Iris;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;

namespace IrisShell.UI;

public class IrisPage : Page, IZunePage, IPage, INotifyPropertyChanged
{
    private string _pageUI;
    private string _pageUIPath;
    private string _backgroundUI;
    private string _bottomBarUI;
    private string _overlayUI;
    private Hashtable _overlayState;
    private IDictionary _navigationArguments;
    private string _navigationCommand;
    private ICommandHandler _commandHandler;
    private Node _pivotPreference;
    private Command _releaseCommand;
    private Command _navigateAwayCommand;
    private Command _navigateToCommand;
    private object _temporaryPageState;
    private BitVector32 _bits;

    public IrisPage()
    {
        SetBit(Bits.ShowDeviceIcon, true);
        SetBit(Bits.ShowPlaylistIcon, true);
        SetBit(Bits.ShowCDIcon, true);
        SetBit(Bits.ShowBackArrow, true);
        SetBit(Bits.NotificationAreaVisible, true);
        SetBit(Bits.TransportControlsVisible, true);
        SetBit(Bits.ShowLogo, true);
        SetBit(Bits.ShowPivots, true);
        SetBit(Bits.ShowSearch, true);
        SetBit(Bits.ShowSettings, true);
        SetBit(Bits.ShowAppBackground, true);
        SetBit(Bits.TakeFocusOnNavigate, true);
        SetBit(Bits.ShowNowPlayingBackgroundOnIdle, true);
        SetBit(Bits.CanEnterCompactMode, true);
        SetBit(Bits.NoStackPage, false);
    }

    public string UI
    {
        get => _pageUI;
        set
        {
            if (!(_pageUI != value))
                return;
            _pageUI = value;
            FirePropertyChanged(nameof(UI));
        }
    }

    public string UIPath
    {
        get => _pageUIPath;
        set
        {
            if (!(_pageUIPath != value))
                return;
            _pageUIPath = value;
            FirePropertyChanged(nameof(UIPath));
        }
    }

    public string BackgroundUI
    {
        get => _backgroundUI;
        set
        {
            if (!(_backgroundUI != value))
                return;
            _backgroundUI = value;
            FirePropertyChanged(nameof(BackgroundUI));
        }
    }

    public string BottomBarUI
    {
        get => _bottomBarUI;
        set
        {
            if (!(_bottomBarUI != value))
                return;
            _bottomBarUI = value;
            FirePropertyChanged(nameof(BottomBarUI));
        }
    }

    public string OverlayUI
    {
        get => _overlayUI;
        set
        {
            if (!(_overlayUI != value))
                return;
            _overlayUI = value;
            FirePropertyChanged(nameof(OverlayUI));
        }
    }

    protected override void OnDispose(bool disposing)
    {
        if (disposing)
        {
            UI = null;
            BackgroundUI = null;
            BottomBarUI = null;
            OverlayUI = null;
        }
        base.OnDispose(disposing);
    }

    public void SetOverlayState(string overlayKey, object state)
    {
        if (_overlayState == null)
            _overlayState = new Hashtable(1);
        _overlayState[overlayKey] = state;
    }

    public object? GetOverlayState(string overlayKey) => _overlayState != null ? _overlayState[overlayKey] : null;

    public IDictionary NavigationArguments
    {
        get => _navigationArguments;
        set => _navigationArguments = value;
    }

    public string NavigationCommand
    {
        get => _navigationCommand;
        set => _navigationCommand = value;
    }

    public ICommandHandler CommandHandler
    {
        get => _commandHandler;
        set => _commandHandler = value;
    }

    public bool ShowBackArrow
    {
        get => GetBit(Bits.ShowBackArrow);
        set
        {
            if (!ChangeBit(Bits.ShowBackArrow, value))
                return;
            FirePropertyChanged(nameof(ShowBackArrow));
        }
    }

    public bool NoStackPage
    {
        get => GetBit(Bits.NoStackPage);
        set
        {
            if (!ChangeBit(Bits.NoStackPage, value))
                return;
            FirePropertyChanged(nameof(NoStackPage));
        }
    }

    public bool AutoHideToolbars
    {
        get => GetBit(Bits.AutoHideToolbars);
        set
        {
            if (!ChangeBit(Bits.AutoHideToolbars, value))
                return;
            FirePropertyChanged(nameof(AutoHideToolbars));
        }
    }

    public bool ShowAppBackground
    {
        get => GetBit(Bits.ShowAppBackground);
        set
        {
            if (!ChangeBit(Bits.ShowAppBackground, value))
                return;
            FirePropertyChanged(nameof(ShowAppBackground));
        }
    }

    public bool ShowingVideoPreview
    {
        get => GetBit(Bits.ShowingVideoPreview);
        set
        {
            if (!ChangeBit(Bits.ShowingVideoPreview, value))
                return;
            FirePropertyChanged(nameof(ShowingVideoPreview));
        }
    }

    public bool ShowLogo
    {
        get => GetBit(Bits.ShowLogo);
        set => SetBit(Bits.ShowLogo, value);
    }

    public bool ShowPivots
    {
        get => GetBit(Bits.ShowPivots);
        set => ChangeBit(Bits.ShowPivots, value);
    }

    public bool ShowSearch
    {
        get => GetBit(Bits.ShowSearch);
        set => SetBit(Bits.ShowSearch, value);
    }

    public bool ShowSettings
    {
        get => GetBit(Bits.ShowSettings);
        set => SetBit(Bits.ShowSettings, value);
    }

    public bool TakeFocusOnNavigate
    {
        get => GetBit(Bits.TakeFocusOnNavigate);
        set => SetBit(Bits.TakeFocusOnNavigate, value);
    }

    public Node PivotPreference
    {
        get => _pivotPreference;
        set => _pivotPreference = value;
    }

    public bool IsRootPage
    {
        get => GetBit(Bits.IsRootPage);
        set => SetBit(Bits.IsRootPage, value);
    }

    public object TemporaryPageState
    {
        get => _temporaryPageState;
        set
        {
            if (_temporaryPageState == value)
                return;
            _temporaryPageState = value;
            FirePropertyChanged(nameof(TemporaryPageState));
        }
    }

    //public virtual void InvokeSettings() => Shell.SettingsFrame.Settings.Invoke();

    public bool ShouldHandleBack
    {
        get => GetBit(Bits.ShouldHandleBack);
        set
        {
            if (!ChangeBit(Bits.ShouldHandleBack, value))
                return;
            FirePropertyChanged(nameof(ShouldHandleBack));
        }
    }

    public bool ShouldHandleEscape
    {
        get => GetBit(Bits.ShouldHandleEscape);
        set
        {
            if (!ChangeBit(Bits.ShouldHandleEscape, value))
                return;
            FirePropertyChanged(nameof(ShouldHandleEscape));
        }
    }

    public event EventHandler BackHandled;

    public event EventHandler EscapeHandled;

    public virtual bool HandleBack()
    {
        if (!ShouldHandleBack)
            return false;
        if (BackHandled != null)
            BackHandled(this, EventArgs.Empty);
        FirePropertyChanged("BackHandled");
        return true;
    }

    public virtual bool HandleEscape()
    {
        if (!ShouldHandleEscape)
            return false;
        if (EscapeHandled != null)
            EscapeHandled(this, EventArgs.Empty);
        FirePropertyChanged("EscapeHandled");
        return true;
    }

    public virtual bool CanNavigateForwardTo(IZunePage destination) => true;

    public event EventHandler Refresh;

    public void RefreshPage()
    {
        if (Refresh != null)
            Refresh(this, EventArgs.Empty);
        FirePropertyChanged("Refresh");
    }

    public override void Release()
    {
        if (_releaseCommand != null)
            _releaseCommand.Invoke();
        base.Release();
    }

    protected override void OnNavigatedToWorker()
    {
        _navigateToCommand?.Invoke();
        base.OnNavigatedToWorker();
    }

    protected override void OnNavigatedAwayWorker(IPage? destination)
    {
        _navigateAwayCommand?.Invoke();
        base.OnNavigatedAwayWorker(destination);
    }

    public override IPageState? SaveAndRelease()
    {
        if (!NoStackPage)
            return base.SaveAndRelease();

        Release();
        return null;
    }

    public Command ReleaseCommand
    {
        get => _releaseCommand;
        set
        {
            if (_releaseCommand == value)
                return;
            _releaseCommand = value;
            FirePropertyChanged(nameof(ReleaseCommand));
        }
    }

    public Command NavigateToCommand
    {
        get => _navigateToCommand;
        set
        {
            if (_navigateToCommand == value)
                return;
            _navigateToCommand = value;
            FirePropertyChanged(nameof(NavigateToCommand));
        }
    }

    public Command NavigateAway
    {
        get => _navigateAwayCommand;
        set
        {
            if (_navigateAwayCommand == value)
                return;
            _navigateAwayCommand = value;
            FirePropertyChanged("NavigateAwayCommand");
        }
    }

    private bool GetBit(Bits lookupBit) => _bits[(int)lookupBit];

    private void SetBit(Bits changeBit, bool value) => _bits[(int)changeBit] = value;

    private bool ChangeBit(Bits bit, bool value)
    {
        if (_bits[(int)bit] == value)
            return false;
        _bits[(int)bit] = value;
        return true;
    }

    private enum Bits : uint
    {
        ShowBackArrow = 1,
        ShowDeviceIcon = 2,
        ShowPlaylistIcon = 4,
        ShowCDIcon = 8,
        ShowNowPlayingX = 16, // 0x00000010
        NotificationAreaVisible = 32, // 0x00000020
        AutoHideToolbars = 64, // 0x00000040
        ShowAppBackground = 128, // 0x00000080
        IsRootPage = 256, // 0x00000100
        ShouldHandleBack = 512, // 0x00000200
        ShowLogo = 1024, // 0x00000400
        ShowPivots = 2048, // 0x00000800
        ShowSearch = 4096, // 0x00001000
        ShowSettings = 8192, // 0x00002000
        TakeFocusOnNavigate = 16384, // 0x00004000
        ShowingVideoPreview = 32768, // 0x00008000
        ShowNowPlayingBackgroundOnIdle = 65536, // 0x00010000
        ShouldHandleEscape = 131072, // 0x00020000
        CanEnterCompactMode = 262144, // 0x00040000
        NoStackPage = 524288, // 0x00080000
        TransportControlsVisible = 1048576, // 0x00100000
    }
}