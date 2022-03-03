// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Navigation.NavigationItem
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Debug;
using Microsoft.Iris.Render;
using Microsoft.Iris.RenderAPI.Drawing;
using Microsoft.Iris.Session;
using System;
using System.Collections;
using System.Diagnostics;

namespace Microsoft.Iris.Navigation
{
    internal abstract class NavigationItem
    {
        private NavigationItem.FocusEntry[] s_emptyFocusRankList = new NavigationItem.FocusEntry[0];
        private INavigationSite _subjectSite;
        private Direction _searchDirection;
        private NavigationItem _parentItem;
        private RectangleF _locationRectangleF;
        private NavigationItem.FocusEntry[] _focusRankList;
        private int _rawChildOrderValue;

        protected NavigationItem(INavigationSite subjectSite, Direction searchDirection)
        {
            _subjectSite = subjectSite;
            _searchDirection = searchDirection;
            Vector3 positionPxlVector;
            Vector3 sizePxlVector;
            _subjectSite.ComputeBounds(out positionPxlVector, out sizePxlVector);
            _locationRectangleF = new RectangleF(positionPxlVector.X, positionPxlVector.Y, sizePxlVector.X, sizePxlVector.Y);
        }

        public IList GetChildrenToSearch(RectangleF startRectangleF, bool enteringFlag)
        {
            ArrayList childrenList = new ArrayList();
            FindNavigableChildren(Subject, childrenList);
            IList list = null;
            if (childrenList.Count > 0)
            {
                list = ComputeSearchOrder(childrenList, startRectangleF, enteringFlag);
                if (list != null && list.Count == 0)
                    list = null;
            }
            return list;
        }

        protected abstract IList ComputeSearchOrder(
          IList allChildrenList,
          RectangleF startRectangleF,
          bool enteringFlag);

        private void FindNavigableChildren(INavigationSite parentSite, ArrayList childrenList)
        {
            foreach (INavigationSite child in parentSite.Children)
            {
                if (child.Visible)
                {
                    Direction searchDirection = SearchDirection;
                    if (IsPreferFocusOrderContainer(child))
                        searchDirection = Direction.Next;
                    NavigationItem itemForSite = CreateItemForSite(child, searchDirection, false);
                    if (itemForSite != null)
                    {
                        int num = childrenList.Add(itemForSite);
                        itemForSite.RawChildOrder = num;
                    }
                    else
                        FindNavigableChildren(child, childrenList);
                }
            }
        }

        public NavigationItem Parent
        {
            get
            {
                if (_parentItem == null && !IsBoundingSite(_subjectSite, _searchDirection))
                {
                    INavigationSite parent = _subjectSite.Parent;
                    if (parent != null)
                        _parentItem = CreateAreaForSite(parent, _searchDirection, true, false);
                }
                return _parentItem;
            }
        }

        public INavigationSite Subject => _subjectSite;

        public Direction SearchDirection => _searchDirection;

        public RectangleF Location => _locationRectangleF;

        public PointF Position => _locationRectangleF.Center;

        public NavigationClass Navigability => _subjectSite.Navigability;

        internal int RawChildOrder
        {
            get => _rawChildOrderValue;
            set => _rawChildOrderValue = value;
        }

        public override int GetHashCode() => Subject.GetHashCode();

        public override bool Equals(object rhs)
        {
            NavigationItem navigationItem = rhs as NavigationItem;
            return !(navigationItem == null) && Subject.Equals(navigationItem.Subject);
        }

        public static bool operator ==(NavigationItem lhs, NavigationItem rhs)
        {
            bool flag1 = (object)lhs == null;
            bool flag2 = (object)rhs == null;
            if (flag1 && flag2)
                return true;
            return !flag1 && !flag2 && lhs.Subject.Equals(rhs.Subject);
        }

        public static bool operator !=(NavigationItem lhs, NavigationItem rhs) => !(lhs == rhs);

        public override string ToString() => GetType().Name + "[" + _subjectSite + "]";

        protected static int CompareFocusOrder(NavigationItem niA, NavigationItem niB)
        {
            if (niA == null || niB == null)
                return 0;
            int num = CompareFocusRanks(niA.FocusRank, niB.FocusRank);
            if (num != 0)
                return num;
            int rawChildOrder1 = niA.RawChildOrder;
            int rawChildOrder2 = niB.RawChildOrder;
            if (rawChildOrder1 < rawChildOrder2)
                return -1;
            return rawChildOrder1 > rawChildOrder2 ? 1 : 0;
        }

        [Conditional("DEBUG")]
        internal void DebugTraceFocusRank()
        {
            if (!Debug.Trace.IsCategoryEnabled(TraceCategory.Focus))
                return;
            int num = 0;
            while (num < FocusRank.Length)
                ++num;
        }

        internal NavigationItem SearchUpTree(
          RectangleF startRectangleF,
          INavigationSite excludeStickyContainerSite,
          INavigationSite excludeStickyDestinationSite)
        {
            NavigationItem navigationItem1 = this;
            while (true)
            {
                NavigationItem parent = navigationItem1.Parent;
                if (!(parent == null))
                {
                    IList childrenToSearch = parent.GetChildrenToSearch(startRectangleF, false);
                    if (childrenToSearch != null)
                    {
                        for (int index = childrenToSearch.IndexOf(navigationItem1) + 1; index < childrenToSearch.Count; ++index)
                        {
                            NavigationItem navigationItem2 = (childrenToSearch[index] as NavigationItem).SearchDownTree(startRectangleF, true, excludeStickyContainerSite, excludeStickyDestinationSite);
                            if (navigationItem2 != null)
                                return navigationItem2;
                        }
                    }
                    navigationItem1 = parent;
                }
                else
                    break;
            }
            return null;
        }

        internal NavigationItem SearchDownTree(
          RectangleF startRectangleF,
          bool depthFirst,
          INavigationSite excludeStickyContainerSite,
          INavigationSite exclueStickyDestinationSite)
        {
            if (Subject != excludeStickyContainerSite)
            {
                object groupFocusId = GetGroupFocusId(Subject);
                if (groupFocusId != null)
                {
                    INavigationSite site = Subject.LookupChildById(groupFocusId);
                    if (site != null && site != exclueStickyDestinationSite)
                    {
                        NavigationItem itemForSite = CreateItemForSite(site, SearchDirection, false);
                        if (itemForSite != null && itemForSite.CheckDestination(startRectangleF))
                            return itemForSite;
                        SetGroupFocusId(Subject, null);
                    }
                }
            }
            if (!depthFirst || IsPreferContainerFocus(Subject))
            {
                depthFirst = false;
                if (CheckDestination(startRectangleF))
                    return this;
            }
            IList childrenToSearch = GetChildrenToSearch(startRectangleF, true);
            if (childrenToSearch != null)
            {
                int num = 0;
                foreach (NavigationItem navigationItem1 in childrenToSearch)
                {
                    NavigationItem navigationItem2 = navigationItem1.SearchDownTree(startRectangleF, depthFirst, excludeStickyContainerSite, exclueStickyDestinationSite);
                    if (navigationItem2 != null)
                        return navigationItem2;
                    ++num;
                }
            }
            return depthFirst && CheckDestination(startRectangleF) ? this : null;
        }

        internal static void RememberFocus(INavigationSite focusSite)
        {
            object uniqueId = focusSite.UniqueId;
            for (INavigationSite groupSite = focusSite; groupSite != null; groupSite = groupSite.Parent)
                SetGroupFocusId(groupSite, uniqueId);
        }

        internal static void ClearFocus(INavigationSite startSite)
        {
            for (INavigationSite groupSite = startSite; groupSite != null; groupSite = groupSite.Parent)
                SetGroupFocusId(groupSite, null);
        }

        internal static NavigationItem CreateItemForSite(
          INavigationSite site,
          Direction searchDirection,
          bool mustUseThisSiteFlag)
        {
            return CreateItemForSiteWorker(site, searchDirection, mustUseThisSiteFlag);
        }

        internal static NavigationItem CreateAreaForSite(
          INavigationSite targetSite,
          Direction searchDirection,
          bool searchAncestorsFlag,
          bool mustUseThisSiteFlag)
        {
            return CreateAreaForSiteWorker(targetSite, searchDirection, searchAncestorsFlag, mustUseThisSiteFlag);
        }

        private static NavigationItem CreateItemForSiteWorker(
          INavigationSite site,
          Direction searchDirection,
          bool mustUseThisSiteFlag)
        {
            if (site == null)
                return null;
            if (!site.Visible)
                return null;
            if (site.Navigability != NavigationClass.None)
                mustUseThisSiteFlag = true;
            return CreateAreaForSiteWorker(site, searchDirection, false, mustUseThisSiteFlag);
        }

        private static NavigationItem CreateAreaForSiteWorker(
          INavigationSite targetSite,
          Direction searchDirection,
          bool searchAncestorsFlag,
          bool mustUseThisSiteFlag)
        {
            if (targetSite == null)
                return null;
            if (!targetSite.Visible)
                return null;
            INavigationSite governSite;
            NavigationOrientation containerOrientation = ComputeGoverningContainerOrientation(targetSite, searchDirection, searchAncestorsFlag || mustUseThisSiteFlag, out governSite);
            if (governSite != targetSite && !mustUseThisSiteFlag)
            {
                if (!searchAncestorsFlag)
                    return null;
                targetSite = governSite;
            }
            switch (searchDirection)
            {
                case Direction.North:
                case Direction.South:
                case Direction.East:
                case Direction.West:
                    switch (containerOrientation)
                    {
                        case NavigationOrientation.Horizontal:
                        case NavigationOrientation.Vertical:
                            return new NavigationStrip(targetSite, searchDirection, containerOrientation);
                        case NavigationOrientation.FlowHorizontal:
                        case NavigationOrientation.FlowVertical:
                            return new NavigationFlow(targetSite, searchDirection, containerOrientation);
                        case NavigationOrientation.Free:
                            return new NavigationSpace(targetSite, searchDirection);
                    }
                    break;
                case Direction.Previous:
                case Direction.Next:
                    switch (containerOrientation)
                    {
                        case NavigationOrientation.Horizontal:
                        case NavigationOrientation.FlowHorizontal:
                        case NavigationOrientation.Vertical:
                        case NavigationOrientation.FlowVertical:
                        case NavigationOrientation.Free:
                            return new NavigationOrder(targetSite, searchDirection);
                    }
                    break;
            }
            return null;
        }

        private static NavigationOrientation ComputeGoverningContainerOrientation(
          INavigationSite targetSite,
          Direction searchDirection,
          bool searchAncestorsFlag,
          out INavigationSite governSite)
        {
            NavigationOrientation orientation = GetAreaDisposition(targetSite, searchDirection);
            if (!IsNeutralGoverningOrientation(orientation, searchAncestorsFlag))
            {
                governSite = targetSite;
            }
            else
            {
                governSite = targetSite;
                for (INavigationSite parent = targetSite.Parent; parent != null; parent = parent.Parent)
                {
                    NavigationOrientation areaDisposition = GetAreaDisposition(parent, searchDirection);
                    if (areaDisposition != NavigationOrientation.None)
                    {
                        switch (areaDisposition)
                        {
                            case NavigationOrientation.Inherit:
                                orientation = !searchAncestorsFlag ? NavigationOrientation.None : NavigationOrientation.Inherit;
                                break;
                            case NavigationOrientation.Free:
                                if (searchAncestorsFlag)
                                {
                                    governSite = parent;
                                    orientation = NavigationOrientation.Free;
                                    break;
                                }
                                orientation = NavigationOrientation.None;
                                break;
                            default:
                                if (searchAncestorsFlag)
                                {
                                    governSite = parent;
                                    orientation = areaDisposition;
                                    break;
                                }
                                orientation = NavigationOrientation.Free;
                                break;
                        }
                    }
                    if (!IsNeutralGoverningOrientation(orientation, searchAncestorsFlag))
                        goto label_16;
                }
                orientation = NavigationOrientation.None;
            }
        label_16:
            return orientation;
        }

        private static bool IsNeutralGoverningOrientation(
          NavigationOrientation orientation,
          bool searchingAncestorsFlag)
        {
            switch (orientation)
            {
                case NavigationOrientation.None:
                    return searchingAncestorsFlag;
                case NavigationOrientation.Inherit:
                    return true;
                default:
                    return false;
            }
        }

        private static NavigationOrientation GetAreaDisposition(
          INavigationSite targetSite,
          Direction searchDirection)
        {
            NavigationOrientation explicitOrientation = GetExplicitOrientation(targetSite);
            if (explicitOrientation != NavigationOrientation.Inherit)
                return explicitOrientation;
            NavigationOrientation flowOrientation = GetFlowOrientation(targetSite, searchDirection);
            if (flowOrientation != NavigationOrientation.Inherit)
                return flowOrientation;
            if (targetSite.Parent == null || targetSite.Navigability == NavigationClass.Direct || (IsBoundingSite(targetSite, searchDirection) || IsPreferFocusOrderContainer(targetSite)) || (IsTabGroup(targetSite) || IsRememberFocus(targetSite) || IsPreferContainerFocus(targetSite)))
                return NavigationOrientation.Free;
            return targetSite.IsLogicalJunction ? NavigationOrientation.Inherit : NavigationOrientation.None;
        }

        private static NavigationOrientation GetExplicitOrientation(
          INavigationSite targetSite)
        {
            NavigationPolicies mode = targetSite.Mode;
            switch (mode & (NavigationPolicies.Row | NavigationPolicies.Column))
            {
                case NavigationPolicies.Row:
                    return NavigationOrientation.Horizontal;
                case NavigationPolicies.Column:
                    return NavigationOrientation.Vertical;
                case NavigationPolicies.Row | NavigationPolicies.Column:
                    return NavigationOrientation.Free;
                default:
                    return (mode & NavigationPolicies.Group) == NavigationPolicies.Group ? NavigationOrientation.Free : NavigationOrientation.Inherit;
            }
        }

        internal static NavigationOrientation GetFlowOrientation(
          INavigationSite targetSite,
          Direction searchDirection)
        {
            NavigationPolicies mode = targetSite.Mode;
            switch (searchDirection)
            {
                case Direction.North:
                case Direction.South:
                case Direction.East:
                case Direction.West:
                    if ((mode & NavigationPolicies.FlowHorizontal) != NavigationPolicies.None)
                        return NavigationOrientation.FlowHorizontal;
                    if ((mode & NavigationPolicies.FlowVertical) != NavigationPolicies.None)
                        return NavigationOrientation.FlowVertical;
                    break;
            }
            return NavigationOrientation.Inherit;
        }

        internal static bool IsBoundingSite(INavigationSite targetSite, Direction searchDirection)
        {
            NavigationPolicies mode = targetSite.Mode;
            switch (searchDirection)
            {
                case Direction.North:
                case Direction.South:
                    if ((mode & (NavigationPolicies.ContainVertical | NavigationPolicies.WrapVertical)) != NavigationPolicies.None)
                        return true;
                    break;
                case Direction.East:
                case Direction.West:
                    if ((mode & (NavigationPolicies.ContainHorizontal | NavigationPolicies.WrapHorizontal)) != NavigationPolicies.None)
                        return true;
                    break;
                case Direction.Previous:
                case Direction.Next:
                    if ((mode & NavigationPolicies.ContainTabOrder) == NavigationPolicies.ContainTabOrder || (mode & NavigationPolicies.WrapTabOrder) == NavigationPolicies.WrapTabOrder)
                        return true;
                    break;
            }
            return false;
        }

        internal static bool IsWrappingSite(INavigationSite targetSite, Direction searchDirection)
        {
            NavigationPolicies mode = targetSite.Mode;
            switch (searchDirection)
            {
                case Direction.North:
                case Direction.South:
                    if ((mode & NavigationPolicies.WrapVertical) != NavigationPolicies.None)
                        return true;
                    break;
                case Direction.East:
                case Direction.West:
                    if ((mode & NavigationPolicies.WrapHorizontal) != NavigationPolicies.None)
                        return true;
                    break;
                case Direction.Previous:
                case Direction.Next:
                    if ((mode & NavigationPolicies.WrapTabOrder) == NavigationPolicies.WrapTabOrder)
                        return true;
                    break;
            }
            return false;
        }

        internal static bool IsPreferFocusOrderContainer(INavigationSite targetSite) => (targetSite.Mode & NavigationPolicies.PreferFocusOrder) == NavigationPolicies.PreferFocusOrder;

        internal static bool IsPreferContainerFocus(INavigationSite targetSite) => (targetSite.Mode & NavigationPolicies.PreferContainerFocus) == NavigationPolicies.PreferContainerFocus;

        internal static bool IsTabGroup(INavigationSite targetSite) => (targetSite.Mode & NavigationPolicies.TabGroup) == NavigationPolicies.TabGroup;

        private static bool IsRememberFocus(INavigationSite targetSite) => (targetSite.Mode & NavigationPolicies.RememberFocus) == NavigationPolicies.RememberFocus;

        private bool CheckDestination(RectangleF startRectangleF)
        {
            bool flag = false;
            switch (Navigability)
            {
                case NavigationClass.None:
                    flag = false;
                    break;
                case NavigationClass.Direct:
                    flag = true;
                    break;
            }
            return flag;
        }

        private static void SetGroupFocusId(INavigationSite groupSite, object uniqueIdObject)
        {
            if (!IsRememberFocus(groupSite))
                return;
            groupSite.StateCache = uniqueIdObject;
        }

        private static object GetGroupFocusId(INavigationSite groupSite)
        {
            object obj = null;
            if (IsRememberFocus(groupSite))
                obj = groupSite.StateCache;
            return obj;
        }

        private static int CompareFocusRanks(
          NavigationItem.FocusEntry[] arA,
          NavigationItem.FocusEntry[] arB)
        {
            int num = Math.Min(arA.Length, arB.Length);
            for (int index = 0; index < num && arA[index].groupSite == arB[index].groupSite; ++index)
            {
                int orderValue1 = arA[index].orderValue;
                int orderValue2 = arB[index].orderValue;
                if (orderValue1 != orderValue2)
                {
                    if (orderValue1 < orderValue2)
                        return -1;
                    if (orderValue1 > orderValue2)
                        return 1;
                }
            }
            return 0;
        }

        private NavigationItem.FocusEntry[] FocusRank
        {
            get
            {
                if (_focusRankList == null)
                    _focusRankList = ComputeFocusRank();
                return _focusRankList;
            }
        }

        private NavigationItem.FocusEntry[] ComputeFocusRank()
        {
            switch (_searchDirection)
            {
                case Direction.Previous:
                case Direction.Next:
                    NavigationItem parent = Parent;
                    if (parent == null)
                        return s_emptyFocusRankList;
                    int num1 = 0;
                    INavigationSite navigationSite1 = Subject;
                    INavigationSite subject = parent.Subject;
                    if (navigationSite1 == subject)
                        return s_emptyFocusRankList;
                    NavigationItem.FocusEntry[] focusEntryArray;
                    int num2;
                    int index;
                    INavigationSite navigationSite2;
                    while (true)
                    {
                        do
                        {
                            navigationSite1 = navigationSite1.Parent;
                            if (navigationSite1 == subject)
                            {
                                int length = num1 + 1;
                                focusEntryArray = new NavigationItem.FocusEntry[length];
                                num2 = int.MaxValue;
                                index = length - 1;
                                navigationSite2 = Subject;
                                goto label_10;
                            }
                        }
                        while (!navigationSite1.IsLogicalJunction);
                        ++num1;
                    }
                label_10:
                    while (true)
                    {
                        do
                        {
                            if (num2 == int.MaxValue)
                                num2 = navigationSite2.FocusOrder;
                            navigationSite2 = navigationSite2.Parent;
                            if (navigationSite2 == subject)
                            {
                                focusEntryArray[index].groupSite = navigationSite2;
                                focusEntryArray[index].orderValue = num2;
                                return focusEntryArray;
                            }
                        }
                        while (!navigationSite2.IsLogicalJunction);
                        focusEntryArray[index].groupSite = navigationSite2;
                        focusEntryArray[index].orderValue = num2;
                        num2 = int.MaxValue;
                        --index;
                    }
                default:
                    return s_emptyFocusRankList;
            }
        }

        private struct FocusEntry
        {
            public INavigationSite groupSite;
            public int orderValue;
        }
    }
}
