// Decompiled with JetBrains decompiler
// Type: ZuneUI.Experience
// Assembly: ZuneShell, Version=4.7.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: FC8028F3-A47B-4FB4-B35B-11D1752D8264
// Assembly location: C:\Program Files\Zune\ZuneShell.dll

using Microsoft.Iris;
using System.Collections;

namespace IrisShell.UI;

public abstract class Experience : Command
{
    private bool _isCurrent;
    private Choice? _nodes;

    public Experience(Frame frameOwner)
      : base(frameOwner, "", null)
    { }

    public Experience(Frame frameOwner, string nameId)
      : base(frameOwner, Shell.LoadString(nameId), null)
    { }

    public Choice Nodes
    {
        get
        {
            if (_nodes == null)
            {
                _nodes = new Choice(this);
                _nodes.Options = NodesList;
            }
            return _nodes;
        }
        set
        {
            if (_nodes == value)
                return;
            _nodes = value;
            FirePropertyChanged(nameof(Nodes));
        }
    }

    public abstract List<Node> NodesList { get; }

    public Frame Frame => (Frame)Owner;

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

    public virtual string DefaultUIPath => "";

    protected int GetNodeIndex(Node node)
    {
        for (int index = 0; index < NodesList.Count; ++index)
        {
            if (NodesList[index] == node)
                return index;
        }
        return -1;
    }

    protected virtual void OnIsCurrentChanged()
    {
    }

    protected override void OnInvoked()
    {
        if (IsCurrent)
            return;
        ((Frame)Owner).Experiences.ChosenValue = this;
        Node node = (Node)Nodes.ChosenValue;
        if (node == null && NodesList != null && NodesList.Count > 0)
            node = NodesList[0];
        node?.Invoke(InvokePolicy.Synchronous);
        base.OnInvoked();
    }
}