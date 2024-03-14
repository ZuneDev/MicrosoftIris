namespace IrisShell.UI;

public class InstancePageState(IPage page) : IPageState
{
    public IPage RestoreAndRelease() => page;

    public void Release() => page.Release();

    public bool CanBeTrimmed => true;
}
