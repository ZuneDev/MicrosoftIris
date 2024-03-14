namespace IrisShell.UI;

public interface IPage
{
    IPageState? SaveAndRelease();

    void Release();

    void OnNavigatedTo();

    void OnNavigatedAway(IPage? destination);
}

public interface IPageState
{
    IPage RestoreAndRelease();

    void Release();

    bool CanBeTrimmed { get; }
}

public enum NavigationDirection
{
    Forward,
    Back,
}
