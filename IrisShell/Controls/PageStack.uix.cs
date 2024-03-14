using IrisShell.UI;
using Microsoft.Iris;

namespace IrisShell.Controls;

public class PageStack : ModelItem
{
    private IPage? _currentPage;
    private List<IPageState> _pageStack;
    private NavigationDirection _navDirection;
    private uint _maxStackSize;

    public PageStack()
      : this(null)
    {
    }

    public PageStack(IModelItemOwner? owner)
      : base(owner)
      => _pageStack = [];

    protected override void OnDispose(bool disposing)
    {
        if (disposing)
        {
            if (_currentPage != null)
            {
                _currentPage.OnNavigatedAway(null);
                _currentPage.Release();
                _currentPage = null;
            }
            if (_pageStack != null)
            {
                foreach (IPageState page in _pageStack)
                    page.Release();
                _pageStack.Clear();
            }
        }
        base.OnDispose(disposing);
    }

    public IPage? CurrentPage => _currentPage;

    public bool CanNavigateBack => _pageStack?.Count > 0;

    public uint MaximumStackSize
    {
        get => _maxStackSize;
        set
        {
            if ((int)_maxStackSize == (int)value)
                return;
            _maxStackSize = value;
            FirePropertyChanged(nameof(MaximumStackSize));
            TrimStackSize();
        }
    }

    public NavigationDirection LastNavigationDirection => _navDirection;

    private void SetLastNavigationDirection(NavigationDirection value)
    {
        if (_navDirection == value)
            return;
        _navDirection = value;
        FirePropertyChanged(nameof(LastNavigationDirection));
    }

    public void NavigateToPage(IPage page)
    {
        if (page == null)
            throw new ArgumentNullException(nameof(page));
        if (_currentPage != null)
        {
            _currentPage.OnNavigatedAway(page);
            PushToStack(_currentPage);
        }
        SetCurrentPage(page);
        SetLastNavigationDirection(NavigationDirection.Forward);
        TrimStackSize();
    }

    public void NavigateBack()
    {
        IPage? page1 = null;
        while (CanNavigateBack && page1 == null)
        {
            int index = _pageStack.Count - 1;
            var page2 = _pageStack[index];
            _pageStack.RemoveAt(index);
            if (page2 != null)
                page1 = page2.RestoreAndRelease();
        }

        if (!CanNavigateBack)
            FirePropertyChanged(nameof(CanNavigateBack));

        if (page1 == null)
            return;

        _currentPage?.OnNavigatedAway(page1);
        _currentPage?.Release();

        SetCurrentPage(page1);
        SetLastNavigationDirection(NavigationDirection.Back);
    }

    public void PushToStack(IPage page)
    {
        IPageState pageState = page != null
            ? page.SaveAndRelease()
            : throw new ArgumentNullException(nameof(page));

        if (pageState is null)
            return;

        _pageStack.Add(pageState);
        if (_pageStack.Count != 1)
            return;
        FirePropertyChanged(nameof(CanNavigateBack));
    }

    private void SetCurrentPage(IPage page)
    {
        _currentPage = page;
        FirePropertyChanged(nameof(CurrentPage));
        _currentPage.OnNavigatedTo();
    }

    private void TrimStackSize()
    {
        if (_maxStackSize == 0U)
            return;
        int num = _pageStack.Count - (int)_maxStackSize;
        if (num <= 0)
            return;
        int index = 0;
        while (num > 0 && index < _pageStack.Count)
        {
            IPageState page = _pageStack[index];
            if (page.CanBeTrimmed)
            {
                _pageStack.RemoveAt(index);
                page.Release();
                --num;
            }
            else
                ++index;
        }
    }
}
