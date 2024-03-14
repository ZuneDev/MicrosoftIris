using UIXControls;

namespace IrisShell.UI;

public class ErrorDialogInfo : DialogHelper
{
    private readonly int _hr;
    private readonly int _hrOriginal;
    private readonly string _title;
    private readonly string _description;
    private readonly string _webHelpUrl;

    internal static void Show(int hr, string title) => Show(hr, title, null);

    internal static void Show(int hr, string title, string? description) => new ErrorDialogInfo(hr, title, description).Show();

    private ErrorDialogInfo(int hr, string title, string? description)
      : base("clr-res://IrisShell!IrisShell.ErrorDialog.uix#ErrorDialogContentUI")
    {
        _title = title;
        _hrOriginal = _hr = hr;
        _description = description ?? "";
        _webHelpUrl = "https://github.com/ZuneDev";
    }

    public int HR => _hr;

    public int OriginalHR => _hrOriginal;

    public object ErrorCondition => "";

    public string Title => _title;

    public new string Description => _description;

    public string WebHelpUrl => _webHelpUrl;
}
