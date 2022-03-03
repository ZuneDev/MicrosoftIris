// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Layouts.ScrollingLayout
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Layout;
using Microsoft.Iris.Library;
using Microsoft.Iris.Render;
using Microsoft.Iris.ViewItems;
using System;

namespace Microsoft.Iris.Layouts
{
    internal class ScrollingLayout : ILayout
    {
        private Orientation _orientation;
        private int _prefetch;
        private static bool s_allowScrollFocusIntoView;

        public ScrollingLayout(Orientation orientation, int prefetch)
        {
            _orientation = orientation;
            _prefetch = prefetch;
        }

        public Orientation Orientation
        {
            get => _orientation;
            set
            {
                if (_orientation == value)
                    return;
                _orientation = value;
            }
        }

        public int Prefetch
        {
            get => _prefetch;
            set
            {
                if (_prefetch == value)
                    return;
                _prefetch = value;
            }
        }

        public ItemAlignment DefaultChildAlignment => ItemAlignment.Default;

        Size ILayout.Measure(ILayoutNode layoutNode, Size constraint)
        {
            Size size;
            if (!(layoutNode.GetLayoutInput(ScrollingLayoutInput.Data) is ScrollingLayoutInput layoutInput) || layoutInput.Enabled)
            {
                Size infiniteConstraint = GetInfiniteConstraint(constraint);
                size = DefaultLayout.Measure(layoutNode, infiniteConstraint);
                layoutNode.MeasureData = size;
            }
            else
                size = DefaultLayout.Measure(layoutNode, constraint);
            return size;
        }

        void ILayout.Arrange(ILayoutNode layoutNode, LayoutSlot slot)
        {
            if (!(layoutNode.GetLayoutInput(ScrollingLayoutInput.Data) is ScrollingLayoutInput sli))
                sli = new ScrollingLayoutInput();
            int scrollAmount = sli.ScrollAmount;
            ScrollingLayoutOutput scrollingLayoutOutput = null;
            if (layoutNode.LayoutChildrenCount > 0)
            {
                if (sli.Enabled)
                {
                    scrollingLayoutOutput = Arrange(layoutNode, slot, sli, ref scrollAmount);
                }
                else
                {
                    DefaultLayout.Arrange(layoutNode, slot);
                    scrollAmount = 0;
                    scrollingLayoutOutput = new ScrollingLayoutOutput();
                }
            }
            sli.SetScrollAmount(scrollAmount);
            if (scrollingLayoutOutput == null)
                return;
            layoutNode.SetExtendedLayoutOutput(scrollingLayoutOutput);
        }

        private ScrollingLayoutOutput Arrange(
          ILayoutNode layoutNode,
          LayoutSlot slot,
          ScrollingLayoutInput sli,
          ref int scrollAmount)
        {
            LayoutSlot childSlot;
            Rectangle viewBounds;
            GetChildSlot(slot, scrollAmount, out childSlot, out viewBounds);
            int num = scrollAmount;
            ScrollingLayoutOutput scrollingLayoutOutput = ArrangeChildren(layoutNode, slot, childSlot, viewBounds, sli, true, ref scrollAmount);
            if (num != scrollAmount)
            {
                GetChildSlot(slot, scrollAmount, out childSlot, out viewBounds);
                scrollingLayoutOutput = ArrangeChildren(layoutNode, slot, childSlot, viewBounds, sli, false, ref scrollAmount);
            }
            return scrollingLayoutOutput;
        }

        private void GetChildSlot(
          LayoutSlot slot,
          int scrollAmount,
          out LayoutSlot childSlot,
          out Rectangle viewBounds)
        {
            Size infiniteConstraint = GetInfiniteConstraint(slot.Bounds);
            Size bounds = slot.Bounds;
            viewBounds = new Rectangle(Point.Zero, bounds);
            Point offset = ScrollAmountToOffset(_prefetch);
            Rectangle viewPeripheralBounds = viewBounds;
            viewPeripheralBounds.Inflate(offset.X, offset.Y);
            childSlot = new LayoutSlot(infiniteConstraint, slot.Offset, viewBounds, viewPeripheralBounds);
        }

        private ScrollingLayoutOutput ArrangeChildren(
          ILayoutNode layoutNode,
          LayoutSlot slot,
          LayoutSlot childSlot,
          Rectangle viewBounds,
          ScrollingLayoutInput sli,
          bool applyPendingScrollData,
          ref int scrollAmount)
        {
            Rectangle areaOfInterestBounds = Rectangle.Zero;
            bool scrollAreaOfInterestIntoView = sli.ScrollIntoViewDisposition.Enabled;
            bool flag1 = false;
            VisibleIndexRangeLayoutOutput rangeLayoutOutput = null;
            Rectangle rectangle1 = Rectangle.Zero;
            Rectangle bounds = new Rectangle(new MajorMinor(-scrollAmount, 0).ToPoint(Orientation), Size.Max(slot.Bounds, (Size)layoutNode.MeasureData));
            foreach (ILayoutNode layoutChild in layoutNode.LayoutChildren)
            {
                layoutChild.Arrange(childSlot, bounds);
                rangeLayoutOutput = layoutChild.GetExtendedLayoutOutput(VisibleIndexRangeLayoutOutput.DataCookie) as VisibleIndexRangeLayoutOutput;
                bool flag2 = true;
                AreaOfInterest area1;
                if (!layoutChild.TryGetAreaOfInterest(AreaOfInterestID.ScrollIntoViewRequest, out area1) && !layoutChild.TryGetAreaOfInterest(AreaOfInterestID.PendingFocus, out area1))
                    flag2 = layoutChild.TryGetAreaOfInterest(AreaOfInterestID.Focus, out area1);
                if (flag2)
                    areaOfInterestBounds = area1.Rectangle;
                if (IsFocusAreaOfInterest(area1.Id) && !sli.ScrollIntoViewDisposition.Enabled)
                    DisallowScrollFocusIntoView();
                scrollAreaOfInterestIntoView &= ScrollFocusIntoView;
                if (area1.Id == AreaOfInterestID.ScrollIntoViewRequest && !areaOfInterestBounds.IsZero)
                {
                    scrollAreaOfInterestIntoView = true;
                    flag1 = true;
                }
                Rectangle rectangle2 = new Rectangle(Point.Zero, layoutChild.DesiredSize);
                AreaOfInterest area2;
                if (layoutChild.TryGetAreaOfInterest(AreaOfInterestID.ScrollableRange, out area2))
                    rectangle2 = Rectangle.Union(rectangle2, area2.Rectangle);
                rectangle1 = Rectangle.Union(rectangle1, rectangle2);
            }
            ScrollingLayoutOutput scrollAmount1 = CalculateScrollAmount(layoutNode, viewBounds, rectangle1, sli, applyPendingScrollData, scrollAreaOfInterestIntoView, areaOfInterestBounds, ref scrollAmount);
            scrollAmount1.VisibleIndices = rangeLayoutOutput;
            scrollAmount1.ProcessedExplicitScrollIntoViewRequest = flag1;
            scrollAmount1.ScrollFocusIntoView = ScrollFocusIntoView;
            return scrollAmount1;
        }

        private bool IsFocusAreaOfInterest(AreaOfInterestID areaOfInterestID) => areaOfInterestID == AreaOfInterestID.PendingFocus || areaOfInterestID == AreaOfInterestID.Focus;

        private ScrollingLayoutOutput CalculateScrollAmount(
          ILayoutNode layoutNode,
          Rectangle viewBounds,
          Rectangle scrollableBounds,
          ScrollingLayoutInput sli,
          bool applyPendingScrollData,
          bool scrollAreaOfInterestIntoView,
          Rectangle areaOfInterestBounds,
          ref int scrollAmount)
        {
            int space1 = ItemToSpace(viewBounds.Size);
            int space2 = ItemToSpace(scrollableBounds.Size);
            int num1 = -(space1 - space2);
            int num2 = (int)Math.Round(space1 * (double)sli.PageStep);
            int num3 = ItemToSpace(scrollableBounds.Location.ToSize());
            int num4 = num3 + num1;
            if (num1 < 0)
            {
                num3 = 0;
                num4 = 0;
            }
            if (!areaOfInterestBounds.IsZero && scrollAreaOfInterestIntoView)
            {
                scrollAmount = ScrollIntoView(layoutNode, areaOfInterestBounds, scrollAmount, space1, space2, num3, num4, sli.ScrollIntoViewDisposition);
                if (sli.SecondaryScrollIntoViewDisposition != null && sli.SecondaryScrollIntoViewDisposition.Enabled)
                    scrollAmount = ScrollIntoView(layoutNode, areaOfInterestBounds, scrollAmount, space1, space2, num3, num4, sli.SecondaryScrollIntoViewDisposition);
                scrollAmount = Math.Max(scrollAmount, num3);
                scrollAmount = Math.Min(scrollAmount, num4);
            }
            else if (num1 > 0)
            {
                if (applyPendingScrollData)
                {
                    float position = 0.0f;
                    if (sli.GetPendingScrollPosition(out position))
                    {
                        int num5 = (int)Math2.Blend(num3, num4, position, false);
                        scrollAmount = num5;
                    }
                    int amount = 0;
                    if (sli.GetPendingPageRequests(out amount))
                    {
                        int num5 = amount * num2;
                        scrollAmount += num5;
                    }
                }
                scrollAmount = Math.Max(scrollAmount, num3);
                scrollAmount = Math.Min(scrollAmount, num4);
            }
            else
                scrollAmount = 0;
            int space3 = ItemToSpace(scrollableBounds.Location.ToSize());
            bool flag1 = -scrollAmount < space3;
            bool flag2 = -scrollAmount + space2 > space1;
            float num6 = 1f;
            float val2 = 1f;
            float num7 = 0.0f;
            float num8 = 1f;
            float num9 = num1;
            float num10 = scrollAmount;
            if (num2 > 0)
            {
                val2 = (float)(num9 / (double)num2 + 1.0);
                num6 = Math.Max(Math.Min((float)(num10 / (double)num2 + 1.0), val2), 0.0f);
            }
            if (num1 > 0)
            {
                num7 = num10 / space2;
                num8 = (num10 + space1) / space2;
            }
            return new ScrollingLayoutOutput()
            {
                CanScrollNegative = flag1,
                CanScrollPositive = flag2,
                CurrentPage = num6,
                TotalPages = val2,
                ViewNear = num7,
                ViewFar = num8
            };
        }

        private int ScrollIntoView(
          ILayoutNode layoutNode,
          Rectangle areaOfInterestBounds,
          int scrollAmount,
          int viewMajorSpace,
          int naturalMajorSpace,
          int naturalMinScroll,
          int naturalMaxScroll,
          ScrollIntoViewDisposition scrollIntoView)
        {
            Rectangle rectangle = areaOfInterestBounds;
            int num1 = ItemToSpace(rectangle.Location.ToSize());
            int space = ItemToSpace(rectangle.Size);
            int num2 = num1 + space;
            int num3 = scrollAmount;
            int num4 = viewMajorSpace;
            int num5 = num3 + num4;
            int num6 = num3;
            int num7 = num5;
            int beginPadding = scrollIntoView.BeginPadding;
            switch (scrollIntoView.BeginPaddingRelativeTo)
            {
                case RelativeEdge.Near:
                    num6 = num3 + beginPadding;
                    break;
                case RelativeEdge.Far:
                    num6 = num5 - beginPadding;
                    break;
            }
            int endPadding = scrollIntoView.EndPadding;
            switch (scrollIntoView.EndPaddingRelativeTo)
            {
                case RelativeEdge.Near:
                    num7 = num3 + endPadding;
                    break;
                case RelativeEdge.Far:
                    num7 = num5 - endPadding;
                    break;
            }
            int num8 = num6 - num3;
            int num9 = num7 - num5;
            if (scrollIntoView.Locked)
            {
                float num10 = num7 - num6;
                float num11 = num10 * scrollIntoView.LockedPosition;
                float num12 = num10 * (1f - scrollIntoView.LockedPosition);
                int num13 = num1 + (int)(space * (double)scrollIntoView.LockedAlignment);
                num1 = num13 - (int)num11;
                num2 = num13 + (int)num12;
            }
            int num14 = scrollAmount;
            if (num2 > num7)
            {
                scrollAmount = num2 - num4 - num9;
                int num10 = scrollAmount - num14;
                num6 += num10;
                int num11 = num7 + num10;
            }
            if (num1 < num6)
                scrollAmount = num1 - num8;
            if (scrollIntoView.ContentPositioningBehavior == ContentPositioningPolicy.ShowMaximalContent)
                scrollAmount = Math2.Clamp(scrollAmount, naturalMinScroll, naturalMaxScroll);
            return scrollAmount;
        }

        private int ItemToSpace(Size size) => new MajorMinor(size, _orientation).Major;

        protected Point ScrollAmountToOffset(int scrollAmount) => new MajorMinor(scrollAmount, 0).ToPoint(_orientation);

        protected Size GetInfiniteConstraint(Size maxExtent) => new MajorMinor(maxExtent, _orientation)
        {
            Major = 16777215
        }.ToSize(_orientation);

        public static void ResetScrollFocusIntoView() => s_allowScrollFocusIntoView = true;

        public static bool ScrollFocusIntoView => s_allowScrollFocusIntoView;

        public static void DisallowScrollFocusIntoView() => s_allowScrollFocusIntoView = false;
    }
}
