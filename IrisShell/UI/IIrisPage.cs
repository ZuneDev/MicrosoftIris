using System.Collections;
using System.ComponentModel;

namespace IrisShell.UI;

public interface IZunePage : IPage, INotifyPropertyChanged
{
    string UI { get; set; }

    string BackgroundUI { get; set; }

    string BottomBarUI { get; }

    string OverlayUI { get; set; }

    IDictionary NavigationArguments { get; set; }

    ICommandHandler CommandHandler { get; }

    bool ShowBackArrow { get; }

    bool AutoHideToolbars { get; }

    bool ShowAppBackground { get; }

    bool ShowLogo { get; }

    bool ShowPivots { get; }

    bool ShowSearch { get; }

    bool ShowSettings { get; }

    bool HandleBack();

    bool HandleEscape();

    bool CanNavigateForwardTo(IZunePage destination);
}
