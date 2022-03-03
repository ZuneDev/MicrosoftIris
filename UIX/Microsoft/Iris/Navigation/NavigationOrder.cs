// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Navigation.NavigationOrder
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.RenderAPI.Drawing;
using Microsoft.Iris.Session;
using System;
using System.Collections;

namespace Microsoft.Iris.Navigation
{
    internal class NavigationOrder : NavigationItem, IComparer
    {
        private int _orderModifierValue;

        internal NavigationOrder(INavigationSite subjectSite, Direction searchDirection)
          : base(subjectSite, searchDirection)
        {
        }

        protected override IList ComputeSearchOrder(
          IList allChildrenList,
          RectangleF startRectangleF,
          bool enteringFlag)
        {
            _orderModifierValue = 1;
            if (SearchDirection == Direction.Previous && (!enteringFlag || !IsTabGroup(Subject)))
                _orderModifierValue = -1;
            NavigationItem[] navigationItemArray = new NavigationItem[allChildrenList.Count];
            int num = 0;
            foreach (NavigationItem allChildren in allChildrenList)
                navigationItemArray[num++] = allChildren;
            Array.Sort(navigationItemArray, this);
            return navigationItemArray;
        }

        int IComparer.Compare(object a, object b) => _orderModifierValue * CompareFocusOrder((NavigationItem)a, (NavigationItem)b);
    }
}
