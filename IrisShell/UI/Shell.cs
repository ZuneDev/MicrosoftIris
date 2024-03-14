// Decompiled with JetBrains decompiler
// Type: ZuneUI.Shell
// Assembly: ZuneShell, Version=4.7.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: FC8028F3-A47B-4FB4-B35B-11D1752D8264
// Assembly location: C:\Program Files\Zune\ZuneShell.dll

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Iris;
using Microsoft.Iris.Render;
using System.Globalization;
using System.Text;

namespace IrisShell.UI;

public class Shell : IrisShell
{
    private const int c_minimumWindowWidth = 734;
    private const int c_minimumWindowHeight = 500;
    private Node _currentNode;
    private bool _haveDoneInitialNavigation;
    private bool _pivotMismatch;
    private string _backgroundImage;
    private Point _normalWindowPosition;
    private Size _normalWindowSize;
    private bool _applicationInitializationIsComplete;
    private static string _sessionStartupPath;

    public Shell()
    {
    }

    public Frame? CurrentFrame => CurrentExperience?.Frame;

    public Experience? CurrentExperience => _currentNode?.Experience;

    public bool PivotMismatch
    {
        get => _pivotMismatch;
        set
        {
            if (_pivotMismatch == value)
                return;
            _pivotMismatch = value;
            FirePropertyChanged(nameof(PivotMismatch));
        }
    }

    public Node CurrentNode
    {
        get => _currentNode;
        set
        {
            if (_currentNode == value)
                return;
            Frame currentFrame1 = CurrentFrame;
            Experience currentExperience1 = CurrentExperience;
            Node currentNode = _currentNode;
            _currentNode = value;
            if (currentNode != null)
                currentNode.IsCurrent = false;
            if (_currentNode != null)
            {
                _currentNode.IsCurrent = true;
                Experience currentExperience2 = CurrentExperience;
                Choice experiences = CurrentFrame.Experiences;
                int num1 = experiences.Options.IndexOf(currentExperience2);
                if (num1 != -1)
                {
                    PivotMismatch = false;
                    experiences.ChosenIndex = num1;
                    Choice nodes = currentExperience2.Nodes;
                    int num2 = nodes.Options.IndexOf(_currentNode);
                    if (num2 != -1)
                        nodes.ChosenIndex = num2;
                }
                else
                    PivotMismatch = true;
            }
            Frame currentFrame2 = CurrentFrame;
            if (currentFrame1 != currentFrame2)
            {
                if (currentFrame1 != null)
                    currentFrame1.IsCurrent = false;
                if (currentFrame2 != null)
                    currentFrame2.IsCurrent = true;
                FirePropertyChanged("CurrentFrame");
            }
            Experience currentExperience3 = CurrentExperience;
            if (currentExperience1 != currentExperience3)
            {
                if (currentExperience1 != null)
                    currentExperience1.IsCurrent = false;
                if (currentExperience3 != null)
                    currentExperience3.IsCurrent = true;
                FirePropertyChanged("CurrentExperience");
            }
            FirePropertyChanged(nameof(CurrentNode));
        }
    }

    public static bool PreRelease => false;

    public static string SessionStartupPath
    {
        get => _sessionStartupPath;
        private set => _sessionStartupPath = value;
    }

    public static void InitializeInstance()
    {
        Shell shell = new Shell();
    }

    public static void NavigateToHomePage()
    {
        try
        {
            DefaultInstance!.Execute(SessionStartupPath, null);
        }
        catch (ArgumentException ex)
        {
            throw;
        }
    }

    protected override void OnPropertyChanged(string property)
    {
        if (property == "CurrentPage")
        {
            UpdatePivots();
        }
        else if (property == "CommandHandler" && !_haveDoneInitialNavigation)
        {
            if (Application.RenderingType != RenderingType.GDI)
            {
                Application.DeferredInvoke(delegate
                {
                    GdiToD3DRenderPrompt();
                }, new TimeSpan(0, 0, 3));
            }

            _haveDoneInitialNavigation = true;
        }
        base.OnPropertyChanged(property);
    }

    private void GdiToD3DRenderPrompt() => Win32MessageBox.Show(LoadString("IDS_RENDER_PROMPT_AFTER_D3D_SWITCH"), LoadString("IDS_RENDER_PROMPT_CAPTION"), Win32MessageBoxType.MB_YESNO | Win32MessageBoxType.MB_ICONQUESTION, args =>
    {
        var returnValue = (Win32MessageBoxReturnValue)args;
        if (returnValue != Win32MessageBoxReturnValue.NO)
            return;
        
        Win32MessageBox.Show(LoadString("IDS_RENDER_PROMPT_RESTART"), LoadString("IDS_RENDER_PROMPT_CAPTION"), Win32MessageBoxType.MB_ICONASTERISK, delegate
        {
            Application.Window.Close();
        });
    });

    private void UpdatePivots()
    {
        if (CurrentPage is null)
            return;

        IrisPage currentPage = CurrentPage;
        if (currentPage.PivotPreference != null)
            CurrentNode = currentPage.PivotPreference;
        else
            currentPage.PivotPreference = CurrentNode;
    }

    public string BackgroundImage
    {
        get => _backgroundImage;
        internal set
        {
            if (!(_backgroundImage != value))
                return;
            _backgroundImage = value;
            FirePropertyChanged(nameof(BackgroundImage));
        }
    }

    public bool AmbientAnimations => Application.RenderingQuality == RenderingQuality.MaxQuality;

    public Point NormalWindowPosition
    {
        get => _normalWindowPosition;
        set
        {
            if (_normalWindowPosition == value)
                return;
            _normalWindowPosition = value;
            FirePropertyChanged(nameof(NormalWindowPosition));
        }
    }

    public Size NormalWindowSize
    {
        get => _normalWindowSize;
        set
        {
            if (_normalWindowSize == value)
                return;
            _normalWindowSize = value;
            FirePropertyChanged(nameof(NormalWindowSize));
        }
    }

    public bool ApplicationInitializationIsComplete
    {
        get => _applicationInitializationIsComplete;
        set
        {
            if (!value || _applicationInitializationIsComplete)
                return;
            _applicationInitializationIsComplete = value;
            FirePropertyChanged(nameof(ApplicationInitializationIsComplete));
        }
    }

    public static void ShowErrorDialog(int hr, string title) => ErrorDialogInfo.Show(hr, title);

    public static void ShowErrorDialogWithLocalization(int hr, string stringId) => ShowErrorDialog(hr, LoadString(stringId));

    public static void ShowErrorDialogWithLocalization(int hr, string titleId, string descriptionId) => ErrorDialogInfo.Show(hr, LoadString(titleId), LoadString(descriptionId));

    public static string LoadString(string resourceName)
    {
        var localizer = IrisAppBase.Current!.ServiceProvider!.GetService<IStringLocalizer>()
            ?? throw new Exception("Localization service was misconfigured.");

        return localizer.GetString(resourceName);
    }

    public static string LoadString(object resourceName) => LoadString(resourceName.ToString() ?? throw new ArgumentNullException(nameof(resourceName)));

    public static string TimeSpanToString(TimeSpan time, bool prefixWithNegative)
    {
        string str = !prefixWithNegative ? "" : CultureInfo.CurrentCulture.NumberFormat.NegativeSign;
        return time.Hours != 0
            ? $"{str}{time.Hours}{CultureInfo.CurrentCulture.DateTimeFormat.TimeSeparator}{time.Minutes:00}{CultureInfo.CurrentCulture.DateTimeFormat.TimeSeparator}{time.Seconds:00}"
            : $"{str}{time.Minutes:0}{CultureInfo.CurrentCulture.DateTimeFormat.TimeSeparator}{time.Seconds:00}";
    }

    public static string TimeSpanToString(TimeSpan time) => TimeSpanToString(time, false);

    //public static void OpenFolderAndSelectItems(string filePath) => ShellInterop.OpenFolderAndSelectItem(filePath);
}