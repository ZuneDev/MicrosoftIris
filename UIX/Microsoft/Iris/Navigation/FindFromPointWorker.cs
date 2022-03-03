// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Navigation.FindFromPointWorker
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Render;
using Microsoft.Iris.RenderAPI.Drawing;
using Microsoft.Iris.Session;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Iris.Navigation
{
    internal class FindFromPointWorker
    {
        private Direction _biasDirection;
        private INavigationSite _startSite;
        private List<FindFromPointWorker.FindFromPointInfo> _candidatesList;

        public FindFromPointWorker(INavigationSite startSite, Direction biasDirection)
        {
            _startSite = startSite;
            _biasDirection = biasDirection;
        }

        public bool FindFromPoint(PointF pt, out INavigationSite result)
        {
            result = null;
            FindFromPointWorker.FindFromPointInfo itemA = null;
            if (_candidatesList == null)
            {
                _candidatesList = new List<FindFromPointWorker.FindFromPointInfo>();
                CollectChildrenToSearch(_startSite, false);
            }
            if (_candidatesList.Count > 0)
            {
                itemA = _candidatesList[0];
                for (int index = 1; index < _candidatesList.Count; ++index)
                {
                    if (CompareItems(itemA, _candidatesList[index], pt) > 0)
                        itemA = _candidatesList[index];
                }
            }
            if (itemA != null)
                result = itemA.Site;
            return _candidatesList.Count > 0;
        }

        private void CollectChildrenToSearch(INavigationSite originSite, bool preferContainerFocus)
        {
            foreach (INavigationSite child in originSite.Children)
            {
                if (child.Visible)
                {
                    preferContainerFocus |= NavigationItem.IsPreferContainerFocus(child);
                    if (child.Navigability != NavigationClass.None)
                        _candidatesList.Add(new FindFromPointWorker.FindFromPointInfo(child, preferContainerFocus));
                    CollectChildrenToSearch(child, preferContainerFocus);
                }
            }
        }

        private bool IsItemADescendant(INavigationSite potentialChild, INavigationSite potentialParent)
        {
            do
            {
                potentialChild = potentialChild.Parent;
            }
            while (potentialChild != null && potentialChild != potentialParent);
            return potentialChild == potentialParent;
        }

        private RectangleF LocationForSite(INavigationSite site)
        {
            Vector3 positionPxlVector;
            Vector3 sizePxlVector;
            site.ComputeBounds(out positionPxlVector, out sizePxlVector);
            return new RectangleF(positionPxlVector.X, positionPxlVector.Y, sizePxlVector.X, sizePxlVector.Y);
        }

        private int CompareItems(
          FindFromPointWorker.FindFromPointInfo itemA,
          FindFromPointWorker.FindFromPointInfo itemB,
          PointF comparePoint)
        {
            INavigationSite site1 = itemA.Site;
            INavigationSite site2 = itemB.Site;
            if (site1 == site2)
                return 0;
            if (IsItemADescendant(site1, site2))
                return !itemB.PreferContainerFocus ? -1 : 1;
            if (IsItemADescendant(site2, site1))
                return !itemA.PreferContainerFocus ? 1 : -1;
            RectangleF rectangleF1 = LocationForSite(site1);
            RectangleF rectangleF2 = LocationForSite(site2);
            PointF center1 = rectangleF1.Center;
            PointF center2 = rectangleF2.Center;
            if (_biasDirection != Direction.Next)
            {
                float num1 = center1.X - center2.X;
                float num2 = center1.Y - center2.Y;
                float num3 = 0.0f;
                switch (_biasDirection)
                {
                    case Direction.North:
                        num3 = num2;
                        break;
                    case Direction.South:
                        num3 = -num2;
                        break;
                    case Direction.East:
                        num3 = -num1;
                        break;
                    case Direction.West:
                        num3 = num1;
                        break;
                }
                if (num3 < 0.0)
                    return -1;
                if (num3 > 0.0)
                    return 1;
            }
            bool flag1 = rectangleF1.Contains(comparePoint);
            bool flag2 = rectangleF2.Contains(comparePoint);
            if (flag1 ^ flag2)
                return !flag1 ? 1 : -1;
            PointF pointF1 = new PointF(Math.Abs(center1.X - comparePoint.X), Math.Abs(center1.Y - comparePoint.Y));
            PointF pointF2 = new PointF(Math.Abs(center2.X - comparePoint.X), Math.Abs(center2.Y - comparePoint.Y));
            float num4 = pointF1.X + pointF1.Y;
            float num5 = pointF2.X + pointF2.Y;
            if (num4 < (double)num5)
                return -1;
            if (num5 < (double)num4)
                return 1;
            if (center1.X < (double)center2.X)
                return -1;
            if (center2.X < (double)center1.X)
                return 1;
            if (center1.Y < (double)center2.Y)
                return -1;
            return center2.Y < (double)center1.Y ? 1 : 0;
        }

        public class FindFromPointInfo
        {
            public bool PreferContainerFocus;
            public INavigationSite Site;

            public FindFromPointInfo(INavigationSite site, bool preferContainerFocus)
            {
                Site = site;
                PreferContainerFocus = preferContainerFocus;
            }
        }
    }
}
