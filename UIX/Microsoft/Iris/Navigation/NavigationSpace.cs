// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Navigation.NavigationSpace
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.RenderAPI.Drawing;
using Microsoft.Iris.Session;
using System;
using System.Collections;

namespace Microsoft.Iris.Navigation
{
    internal class NavigationSpace : NavigationItem, IComparer
    {
        private const NavigationSpace.Rank c_rankAcceptable = Rank.Fair;

        internal NavigationSpace(INavigationSite subjectSite, Direction searchDirection)
          : base(subjectSite, searchDirection)
        {
        }

        protected override IList ComputeSearchOrder(
          IList allChildrenList,
          RectangleF startRectangleF,
          bool enteringFlag)
        {
            ArrayList arrayList = new ArrayList(allChildrenList.Count);
            foreach (NavigationItem allChildren in allChildrenList)
            {
                NavigationSpace.CandidateInfo candidateInfo = AnalyzeCandidate(allChildren, startRectangleF);
                if (candidateInfo != null)
                    arrayList.Add(candidateInfo);
            }
            arrayList.Sort(this);
            NavigationItem[] navigationItemArray = new NavigationItem[arrayList.Count];
            for (int index = 0; index < arrayList.Count; ++index)
                navigationItemArray[index] = ((NavigationSpace.CandidateInfo)arrayList[index]).item;
            return navigationItemArray;
        }

        private NavigationSpace.CandidateInfo AnalyzeCandidate(
          NavigationItem candidateItem,
          RectangleF originRectangleF)
        {
            RectangleF location = candidateItem.Location;
            float xDeltaValue = 0.0f;
            float yDeltaValue = 0.0f;
            float toleranceValue = 0.0f;
            switch (SearchDirection)
            {
                case Direction.North:
                    yDeltaValue = location.Bottom - originRectangleF.Top;
                    toleranceValue = originRectangleF.Height / 2f;
                    goto case Direction.Previous;
                case Direction.South:
                    yDeltaValue = location.Top - originRectangleF.Bottom;
                    toleranceValue = (float)(originRectangleF.Height / 2.0 * -1.0);
                    goto case Direction.Previous;
                case Direction.East:
                    xDeltaValue = location.Left - originRectangleF.Right;
                    toleranceValue = (float)(originRectangleF.Width / 2.0 * -1.0);
                    goto case Direction.Previous;
                case Direction.West:
                    xDeltaValue = location.Right - originRectangleF.Left;
                    toleranceValue = originRectangleF.Width / 2f;
                    goto case Direction.Previous;
                case Direction.Previous:
                case Direction.Next:
                    float overlapValue = 0.0f;
                    switch (SearchDirection)
                    {
                        case Direction.North:
                        case Direction.South:
                            if (location.Right <= (double)originRectangleF.Left)
                            {
                                xDeltaValue = location.Right - originRectangleF.Left;
                                break;
                            }
                            if (location.Left >= (double)originRectangleF.Right)
                            {
                                xDeltaValue = location.Left - originRectangleF.Right;
                                break;
                            }
                            overlapValue = Math.Min(originRectangleF.Right, location.Right) - Math.Max(originRectangleF.Left, location.Left);
                            break;
                        case Direction.East:
                        case Direction.West:
                            if (location.Bottom <= (double)originRectangleF.Top)
                            {
                                yDeltaValue = location.Bottom - originRectangleF.Top;
                                break;
                            }
                            if (location.Top >= (double)originRectangleF.Bottom)
                            {
                                yDeltaValue = location.Top - originRectangleF.Bottom;
                                break;
                            }
                            overlapValue = Math.Min(originRectangleF.Bottom, location.Bottom) - Math.Max(originRectangleF.Top, location.Top);
                            break;
                    }
                    NavigationSpace.Rank rank = ComputeRank(xDeltaValue, yDeltaValue, overlapValue, toleranceValue);
                    if (rank > Rank.Fair)
                        return null;
                    switch (SearchDirection)
                    {
                        case Direction.North:
                        case Direction.South:
                            yDeltaValue -= toleranceValue;
                            break;
                        case Direction.East:
                        case Direction.West:
                            xDeltaValue -= toleranceValue;
                            break;
                    }
                    float weightedFacingDistance = (float)(xDeltaValue * (double)xDeltaValue + yDeltaValue * (double)yDeltaValue) - overlapValue;
                    float centerDistance = 0.0f;
                    float positionOrder = 0.0f;
                    switch (SearchDirection)
                    {
                        case Direction.North:
                        case Direction.South:
                            positionOrder = location.Left + location.Width / 2f;
                            centerDistance = positionOrder - (originRectangleF.Left + originRectangleF.Width / 2f);
                            break;
                        case Direction.East:
                        case Direction.West:
                            positionOrder = location.Top + location.Height / 2f;
                            break;
                        case Direction.Previous:
                        case Direction.Next:
                            positionOrder = (float)(location.Left + location.Width / 2.0 + (location.Top + location.Height / 2.0));
                            break;
                    }
                    return new NavigationSpace.CandidateInfo(candidateItem, rank, weightedFacingDistance, centerDistance, positionOrder);
                default:
                    return null;
            }
        }

        private NavigationSpace.Rank ComputeRank(
          float xDeltaValue,
          float yDeltaValue,
          float overlapValue,
          float toleranceValue)
        {
            switch (SearchDirection)
            {
                case Direction.North:
                    if (yDeltaValue > (double)toleranceValue)
                        return Rank.Poor;
                    break;
                case Direction.South:
                    if (yDeltaValue < (double)toleranceValue)
                        return Rank.Poor;
                    break;
                case Direction.East:
                    if (xDeltaValue < (double)toleranceValue)
                        return Rank.Poor;
                    break;
                case Direction.West:
                    if (xDeltaValue > (double)toleranceValue)
                        return Rank.Poor;
                    break;
            }
            if (overlapValue > 0.0)
                return Rank.Ideal;
            switch (SearchDirection)
            {
                case Direction.North:
                    if (yDeltaValue > 0.0)
                        return Rank.Fair;
                    break;
                case Direction.South:
                    if (yDeltaValue < 0.0)
                        return Rank.Fair;
                    break;
                case Direction.East:
                    if (xDeltaValue < 0.0)
                        return Rank.Fair;
                    break;
                case Direction.West:
                    if (xDeltaValue > 0.0)
                        return Rank.Fair;
                    break;
            }
            xDeltaValue = Math.Abs(xDeltaValue);
            yDeltaValue = Math.Abs(yDeltaValue);
            switch (SearchDirection)
            {
                case Direction.North:
                case Direction.South:
                    if (xDeltaValue >= (double)yDeltaValue)
                        return Rank.Fair;
                    break;
                case Direction.East:
                case Direction.West:
                    if (yDeltaValue >= (double)xDeltaValue)
                        return Rank.Fair;
                    break;
            }
            return Rank.Good;
        }

        int IComparer.Compare(object a, object b)
        {
            NavigationSpace.CandidateInfo candidateInfo1 = (NavigationSpace.CandidateInfo)a;
            NavigationSpace.CandidateInfo candidateInfo2 = (NavigationSpace.CandidateInfo)b;
            int num1 = candidateInfo1.rank - candidateInfo2.rank;
            if (num1 != 0)
                return num1;
            float num2 = candidateInfo1.weightedFacingDistance - candidateInfo2.weightedFacingDistance;
            if (num2 < 0.0)
                return -1;
            if (num2 > 0.0)
                return 1;
            float num3 = candidateInfo1.centerDistance - candidateInfo2.centerDistance;
            if (num3 < 0.0)
                return -1;
            if (num3 > 0.0)
                return 1;
            float num4 = candidateInfo1.positionOrder - candidateInfo2.positionOrder;
            if (num4 < 0.0)
                return -1;
            return num4 > 0.0 ? 1 : 0;
        }

        private class CandidateInfo
        {
            public NavigationItem item;
            public NavigationSpace.Rank rank;
            public float weightedFacingDistance;
            public float centerDistance;
            public float positionOrder;

            public CandidateInfo(
              NavigationItem item,
              NavigationSpace.Rank rank,
              float weightedFacingDistance,
              float centerDistance,
              float positionOrder)
            {
                this.item = item;
                this.rank = rank;
                this.weightedFacingDistance = weightedFacingDistance;
                this.centerDistance = centerDistance;
                this.positionOrder = positionOrder;
            }
        }

        private enum Rank
        {
            Ideal,
            Good,
            Fair,
            Poor,
        }
    }
}
