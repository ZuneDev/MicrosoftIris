using Microsoft.Iris;

namespace IrisShell.UI;

public abstract class Frame : ModelItem
{
    private bool _isCurrent;
    private Choice? _experiences;

    public Frame(IModelItemOwner? owner) : base(owner)
    {
    }

    public Choice Experiences
    {
        get
        {
            if (_experiences == null)
            {
                _experiences = new Choice(this)
                {
                    Options = (System.Collections.IList)ExperiencesList,
                };
            }
            return _experiences;
        }
        set
        {
            if (_experiences == value)
                return;
            _experiences = value;
            FirePropertyChanged(nameof(Experiences));
        }
    }

    public abstract IList<Experience> ExperiencesList { get; }

    public bool IsCurrent
    {
        get => _isCurrent;
        set
        {
            if (_isCurrent == value)
                return;
            _isCurrent = value;
            OnIsCurrentChanged();
            FirePropertyChanged(nameof(IsCurrent));
        }
    }

    protected virtual void OnIsCurrentChanged()
    {
    }
}
