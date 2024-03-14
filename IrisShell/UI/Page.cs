using Microsoft.Iris;

namespace IrisShell.UI;

public class Page : ModelItem, IPage
{
    private bool _isCurrentPage;

    public bool IsCurrentPage
    {
        get => _isCurrentPage;
        private set
        {
            if (_isCurrentPage == value)
                return;
            _isCurrentPage = value;
            FirePropertyChanged(nameof(IsCurrentPage));
        }
    }

    public void OnNavigatedTo() => OnNavigatedToWorker();

    public void OnNavigatedAway(IPage? destination) => OnNavigatedAwayWorker(destination);

    public event EventHandler? NavigatedTo;

    public event EventHandler? NavigatedAway;

    public virtual IPageState? SaveAndRelease() => new InstancePageState(this);

    public virtual void Release() => Dispose();

    protected virtual void OnNavigatedToWorker()
    {
        IsCurrentPage = true;
        if (NavigatedTo == null)
            return;
        NavigatedTo(this, EventArgs.Empty);
    }

    protected virtual void OnNavigatedAwayWorker(IPage? destination)
    {
        IsCurrentPage = false;
        if (NavigatedAway == null)
            return;
        NavigatedAway(this, EventArgs.Empty);
    }
}
