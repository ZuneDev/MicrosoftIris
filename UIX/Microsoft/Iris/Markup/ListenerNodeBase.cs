// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.ListenerNodeBase
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System.Diagnostics;

namespace Microsoft.Iris.Markup
{
    internal class ListenerNodeBase
    {
        protected ListenerNodeBase _next;
        protected ListenerNodeBase _prev;

        protected ListenerNodeBase()
        {
            _next = null;
            _prev = null;
        }

        public virtual void Dispose()
        {
            if (!IsLinked)
                return;
            Unlink();
        }

        public ListenerNodeBase Next => _next;

        public bool IsLinked => _next != null && _prev != null;

        public void AddPrevious(ListenerNodeBase node)
        {
            if (_prev == null)
            {
                _prev = this;
                _next = this;
            }
            node._prev = _prev;
            node._next = this;
            _prev._next = node;
            _prev = node;
        }

        public void Unlink()
        {
            if (_prev == _next)
            {
                _prev._next = null;
                _prev._prev = null;
            }
            else
            {
                _prev._next = _next;
                _next._prev = _prev;
            }
            _prev = null;
            _next = null;
        }

        [Conditional("DEBUG")]
        private void DEBUG_ValidateList()
        {
        }
    }
}
