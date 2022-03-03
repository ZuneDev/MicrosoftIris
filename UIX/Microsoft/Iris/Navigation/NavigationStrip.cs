// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Navigation.NavigationStrip
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.RenderAPI.Drawing;
using Microsoft.Iris.Session;
using System.Collections;

namespace Microsoft.Iris.Navigation
{
    internal class NavigationStrip : NavigationItem
    {
        private static readonly NavigationStrip.CompareFunction s_orderHorizontalMethod = new NavigationStrip.CompareFunction(CompareOrderHorizontal);
        private static readonly NavigationStrip.CompareFunction s_orderVerticalMethod = new NavigationStrip.CompareFunction(CompareOrderVertical);
        private static readonly NavigationStrip.CompareFunction s_distanceHorizontalMethod = new NavigationStrip.CompareFunction(CompareDistanceHorizontal);
        private static readonly NavigationStrip.CompareFunction s_distanceVerticalMethod = new NavigationStrip.CompareFunction(CompareDistanceVertical);
        private NavigationOrientation _orientationValue;

        internal NavigationStrip(
          INavigationSite subjectSite,
          Direction searchDirection,
          NavigationOrientation orientionValue)
          : base(subjectSite, searchDirection)
        {
            _orientationValue = orientionValue;
        }

        protected override IList ComputeSearchOrder(
          IList allChildrenList,
          RectangleF startRectangleF,
          bool enteringFlag)
        {
            NavigationOrientation searchOrientation = SearchOrientation;
            bool flag = searchOrientation == _orientationValue;
            if (!enteringFlag && !flag)
                return null;
            ArrayList arrayList = new ArrayList(allChildrenList.Count);
            PointF center = startRectangleF.Center;
            foreach (NavigationItem allChildren in allChildrenList)
            {
                if (IsCandidateInSearchDirection(allChildren.Position - center))
                    arrayList.Add(allChildren);
            }
            NavigationStrip.CompareFunction compareMethod = null;
            float paramValue = 0.0f;
            if (flag)
            {
                switch (searchOrientation)
                {
                    case NavigationOrientation.Horizontal:
                        compareMethod = s_orderHorizontalMethod;
                        break;
                    case NavigationOrientation.Vertical:
                        compareMethod = s_orderVerticalMethod;
                        break;
                }
                switch (SearchDirection)
                {
                    case Direction.North:
                    case Direction.West:
                    case Direction.Previous:
                        paramValue = -1f;
                        break;
                    case Direction.South:
                    case Direction.East:
                    case Direction.Next:
                        paramValue = 1f;
                        break;
                }
            }
            else
            {
                switch (searchOrientation)
                {
                    case NavigationOrientation.Horizontal:
                        compareMethod = s_distanceVerticalMethod;
                        paramValue = center.Y;
                        break;
                    case NavigationOrientation.Vertical:
                        compareMethod = s_distanceHorizontalMethod;
                        paramValue = center.X;
                        break;
                }
            }
            arrayList.Sort(new NavigationStrip.ItemComparer(compareMethod, paramValue));
            return arrayList;
        }

        private NavigationOrientation SearchOrientation
        {
            get
            {
                switch (SearchDirection)
                {
                    case Direction.North:
                    case Direction.South:
                        return NavigationOrientation.Vertical;
                    case Direction.East:
                    case Direction.West:
                        return NavigationOrientation.Horizontal;
                    default:
                        return _orientationValue;
                }
            }
        }

        private bool IsCandidateInSearchDirection(SizeF deltaExtent)
        {
            switch (SearchDirection)
            {
                case Direction.North:
                    if (deltaExtent.Height >= 0.0)
                        return false;
                    break;
                case Direction.South:
                    if (deltaExtent.Height <= 0.0)
                        return false;
                    break;
                case Direction.East:
                    if (deltaExtent.Width <= 0.0)
                        return false;
                    break;
                case Direction.West:
                    if (deltaExtent.Width >= 0.0)
                        return false;
                    break;
            }
            return true;
        }

        private static int CompareOrderHorizontal(
          NavigationItem niA,
          NavigationItem niB,
          float orderValue)
        {
            float num1 = niA.Position.X * orderValue;
            float num2 = niB.Position.X * orderValue;
            if (num1 < (double)num2)
                return -1;
            return num1 > (double)num2 ? 1 : 0;
        }

        private static int CompareOrderVertical(
          NavigationItem niA,
          NavigationItem niB,
          float orderValue)
        {
            float num1 = niA.Position.Y * orderValue;
            float num2 = niB.Position.Y * orderValue;
            if (num1 < (double)num2)
                return -1;
            return num1 > (double)num2 ? 1 : 0;
        }

        private static int CompareDistanceHorizontal(
          NavigationItem niA,
          NavigationItem niB,
          float originValue)
        {
            float num1 = niA.Position.X - originValue;
            if (num1 < 0.0)
                num1 *= -1f;
            float num2 = niB.Position.X - originValue;
            if (num2 < 0.0)
                num2 *= -1f;
            if (num1 < (double)num2)
                return -1;
            return num1 > (double)num2 ? 1 : 0;
        }

        private static int CompareDistanceVertical(
          NavigationItem niA,
          NavigationItem niB,
          float originValue)
        {
            float num1 = niA.Position.Y - originValue;
            if (num1 < 0.0)
                num1 *= -1f;
            float num2 = niB.Position.Y - originValue;
            if (num2 < 0.0)
                num2 *= -1f;
            if (num1 < (double)num2)
                return -1;
            return num1 > (double)num2 ? 1 : 0;
        }

        private delegate int CompareFunction(NavigationItem a, NavigationItem b, float param);

        private class ItemComparer : IComparer
        {
            private NavigationStrip.CompareFunction _compareMethod;
            private float _paramValue;

            public ItemComparer(NavigationStrip.CompareFunction compareMethod, float paramValue)
            {
                _compareMethod = compareMethod;
                _paramValue = paramValue;
            }

            public int Compare(object a, object b) => _compareMethod((NavigationItem)a, (NavigationItem)b, _paramValue);
        }
    }
}
