namespace IrisShell.UI;

public class NoStackPage : IrisPage
{
    public override IPageState? SaveAndRelease()
    {
        Release();
        return null;
    }
}

public class StartupPage : NoStackPage
{
}
