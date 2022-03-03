// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Layout.LayoutNodeEnumerator
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

namespace Microsoft.Iris.Layout
{
    internal struct LayoutNodeEnumerator
    {
        private ILayoutNode _start;
        private ILayoutNode _current;
        private bool _haventStartedYet;

        public LayoutNodeEnumerator GetEnumerator() => this;

        public LayoutNodeEnumerator(ILayoutNode start)
        {
            _start = start;
            _current = null;
            _haventStartedYet = true;
        }

        public bool MoveNext()
        {
            if (_current != null)
                _current = _current.NextVisibleSibling;
            else if (_haventStartedYet)
            {
                _current = _start;
                _haventStartedYet = false;
            }
            return _current != null;
        }

        public ILayoutNode Current => _current;

        public void Reset()
        {
            _current = null;
            _haventStartedYet = true;
        }
    }
}
