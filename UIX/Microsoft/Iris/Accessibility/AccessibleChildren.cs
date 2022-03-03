// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Accessibility.AccessibleChildren
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Session;
using Microsoft.Iris.UI;
using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Microsoft.Iris.Accessibility
{
    [ComVisible(true)]
    public class AccessibleChildren : IEnumVARIANT
    {
        private AccessibleProxy _proxy;
        private int _current;

        internal AccessibleChildren(AccessibleProxy proxy)
          : this(proxy, -1)
        {
        }

        internal AccessibleChildren(AccessibleProxy proxy, int position)
        {
            _proxy = proxy;
            _current = position;
        }

        internal IEnumVARIANT Clone() => new AccessibleChildren(_proxy, _current);

        internal int Next(int count, object[] children)
        {
            int index = 0;
            do
            {
                ++_current;
                if (_current >= _proxy.UI.Children.Count)
                {
                    _current = _proxy.UI.Children.Count;
                    break;
                }
                UIClass child = (UIClass)_proxy.UI.Children[_current];
                children[index] = child.AccessibleProxy;
                ++index;
            }
            while (index < count);
            return index;
        }

        internal void Reset() => _current = -1;

        internal bool Skip(int count)
        {
            _current += count;
            if (_current < _proxy.UI.Children.Count)
                return true;
            _current = _proxy.UI.Children.Count;
            return false;
        }

        IEnumVARIANT IEnumVARIANT.Clone()
        {
            VerifyEnumeratorAccess();
            return Clone();
        }

        unsafe int IEnumVARIANT.Next(int celt, object[] rgVar, IntPtr pceltFetched)
        {
            VerifyEnumeratorAccess();
            int num = Next(celt, rgVar);
            *(int*)(void*)pceltFetched = num;
            return num != celt ? 1 : 0;
        }

        int IEnumVARIANT.Reset()
        {
            VerifyEnumeratorAccess();
            Reset();
            return 0;
        }

        int IEnumVARIANT.Skip(int celt)
        {
            VerifyEnumeratorAccess();
            return !Skip(celt) ? 1 : 0;
        }

        private void VerifyEnumeratorAccess()
        {
            if (_proxy.UI != null && UIDispatcher.IsUIThread)
                return;
            Marshal.ThrowExceptionForHR(-2147467259);
        }
    }
}
