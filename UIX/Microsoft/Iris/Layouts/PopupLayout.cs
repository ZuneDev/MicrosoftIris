// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Layouts.PopupLayout
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Data;
using Microsoft.Iris.Layout;
using Microsoft.Iris.Library;
using Microsoft.Iris.OS;
using Microsoft.Iris.Render;
using Microsoft.Iris.RenderAPI.Drawing;
using Microsoft.Iris.Session;
using Microsoft.Iris.UI;
using Microsoft.Iris.ViewItems;
using System;
using System.Collections;

namespace Microsoft.Iris.Layouts
{
    internal class PopupLayout : ILayout
    {
        private const float c_tolerance = 0.01f;
        private static DeferredHandler s_checkForLayoutChanges = new DeferredHandler(CheckForLayoutChanges);
        private Vector<ViewItem> _followMouseSubjects;
        private static readonly DataCookie s_dataProperty = DataCookie.ReserveSlot();

        internal static DataCookie DataCookie => s_dataProperty;

        public ItemAlignment DefaultChildAlignment => ItemAlignment.Default;

        Size ILayout.Measure(ILayoutNode layoutNode, Size constraint)
        {
            foreach (ILayoutNode layoutChild in layoutNode.LayoutChildren)
            {
                if (!(layoutChild.GetLayoutInput(s_dataProperty) is PopupLayoutInput layoutInput) || !layoutInput.ConstrainToTarget)
                    layoutChild.Measure(constraint);
            }
            layoutNode.RequestMoreChildren(int.MaxValue);
            return constraint;
        }

        void ILayout.Arrange(ILayoutNode layoutNode, LayoutSlot slot)
        {
            bool hook = false;
            ViewItem viewItem = (ViewItem)layoutNode;
            if (layoutNode.LayoutChildrenCount > 0)
            {
                RectangleF layoutBounds = new RectangleF(PointF.Zero, new SizeF(slot.Bounds.Width, slot.Bounds.Height));
                foreach (ILayoutNode layoutChild in layoutNode.LayoutChildren)
                {
                    if (!(layoutChild.GetLayoutInput(s_dataProperty) is PopupLayoutInput layoutInput))
                        layoutInput = PopupLayoutInput.Default;
                    if (layoutInput.TargetIsFollowMouse)
                        hook = true;
                    ViewItem child = (ViewItem)layoutChild;
                    Point zero = Point.Zero;
                    Size size = Size.Zero;
                    RectangleF placementTargetRect = GetPlacementTargetRect(child, viewItem, layoutInput, slot);
                    if (layoutInput.ConstrainToTarget)
                    {
                        size = new Size((int)placementTargetRect.Width, (int)placementTargetRect.Height);
                        layoutChild.Measure(size);
                    }
                    else
                        size = layoutChild.DesiredSize;
                    Point childPosition = GetChildPosition(viewItem, layoutBounds, layoutInput, layoutChild, child, slot, placementTargetRect);
                    layoutChild.Arrange(slot, new Rectangle(childPosition, size));
                }
            }
            HookMousePositionChanged(viewItem, hook);
        }

        private Point GetChildPosition(
          ViewItem layoutElement,
          RectangleF layoutBounds,
          PopupLayoutInput layoutInput,
          ILayoutNode childNode,
          ViewItem child,
          LayoutSlot slot,
          RectangleF placementRect)
        {
            PlacementMode placement = layoutInput.Placement;
            if (placement == null || ListUtility.IsNullOrEmpty(placement.PopupPositions))
                return Point.Zero;
            PointF[] interestPoints = InterestPointsFromRect(placementRect);
            PointF[] childInterestPoints = GetChildInterestPoints(childNode);
            GetBounds(interestPoints);
            RectangleF bounds = GetBounds(childInterestPoints);
            double width = bounds.Width;
            double height = bounds.Height;
            bool flag1 = layoutInput.RespectMenuDropAlignment && Win32Api.GetMenuDropAlignment();
            PointF bestPosition = PointF.Zero;
            float num1 = -1f;
            FlipDirection flipDirection = FlipDirection.None;
            int length = placement.PopupPositions.Length;
            for (int index = 0; index < length; ++index)
            {
                PopupPosition popupPosition = placement.PopupPositions[index];
                InterestPoint target = popupPosition.Target;
                InterestPoint popup = popupPosition.Popup;
                SizeF sizeF = interestPoints[(int)target] - childInterestPoints[(int)popup];
                PointF pos = new PointF(sizeF.Width, sizeF.Height);
                RectangleF b = RectangleF.Offset(bounds, pos);
                RectangleF rectangleF = RectangleF.Intersect(layoutBounds, b);
                float num2 = !rectangleF.IsEmpty ? rectangleF.Width * rectangleF.Height : 0.0f;
                bool flag2 = (popupPosition.Flipped & FlipDirection.Horizontal) == FlipDirection.Horizontal;
                bool flag3 = (popupPosition.Flipped & FlipDirection.Vertical) == FlipDirection.Vertical;
                if (flag2 && flag1 || !flag2 && !flag1)
                    num2 += 0.1f;
                if (!flag3)
                    num2 += 0.1f;
                if (num2 - (double)num1 > 0.00999999977648258)
                {
                    bestPosition = pos;
                    num1 = num2;
                    flipDirection = popupPosition.Flipped;
                }
                if (!layoutInput.StayInBounds)
                    break;
            }
            layoutInput.FlippedHorizontally = (flipDirection & FlipDirection.Horizontal) == FlipDirection.Horizontal;
            layoutInput.FlippedVertically = (flipDirection & FlipDirection.Vertical) == FlipDirection.Vertical;
            if (layoutInput.StayInBounds)
                bestPosition = NudgeIntoBounds(layoutBounds, bestPosition, bounds, placement);
            return new Point((int)bestPosition.X, (int)bestPosition.Y);
        }

        private RectangleF GetPlacementTargetRect(
          ViewItem child,
          ViewItem layoutElement,
          PopupLayoutInput layoutInput,
          LayoutSlot slot)
        {
            PlacementMode placement = layoutInput.Placement;
            ViewItem placementTarget = layoutInput.PlacementTarget;
            bool flag = false;
            RectangleF rectangleF;
            if (layoutInput.TargetIsMouse || layoutInput.TargetIsFollowMouse)
            {
                if (layoutInput.MouseRect == RectangleF.Zero || layoutInput.TargetIsFollowMouse)
                    layoutInput.MouseRect = GetMouseRect(placement);
                rectangleF = layoutInput.MouseRect;
                flag = true;
            }
            else if (placementTarget == null || placementTarget.IsDisposed)
            {
                rectangleF = new RectangleF(Point.Zero, slot.Bounds);
            }
            else
            {
                rectangleF = placementTarget.BoundsRelativeToAncestor(null);
                flag = true;
                PopupLayout.PlacementTargetInfo placementTargetInfo = new PopupLayout.PlacementTargetInfo(child, placementTarget, rectangleF);
                DeferredCall.Post(DispatchPriority.LayoutSync, s_checkForLayoutChanges, placementTargetInfo);
            }
            rectangleF.Offset(layoutInput.Offset.X, layoutInput.Offset.Y);
            if (flag)
                rectangleF = layoutElement.TransformFromAncestor(null, rectangleF);
            return rectangleF;
        }

        private static void CheckForLayoutChanges(object args)
        {
            PopupLayout.PlacementTargetInfo placementTargetInfo = (PopupLayout.PlacementTargetInfo)args;
            if (placementTargetInfo.placementTarget.IsDisposed || placementTargetInfo.child.IsDisposed)
                return;
            RectangleF ancestor = placementTargetInfo.placementTarget.BoundsRelativeToAncestor(null);
            if (!(placementTargetInfo.bounds != ancestor))
                return;
            placementTargetInfo.child.MarkLayoutInvalid();
        }

        private PointF[] GetChildInterestPoints(ILayoutNode childNode)
        {
            Size desiredSize = childNode.DesiredSize;
            return InterestPointsFromRect(new RectangleF(0.0f, 0.0f, desiredSize.Width, desiredSize.Height));
        }

        private static PointF[] InterestPointsFromRect(RectangleF rect) => new PointF[5]
        {
      rect.TopLeft,
      rect.TopRight,
      rect.BottomLeft,
      rect.BottomRight,
      rect.Center
        };

        private RectangleF GetBounds(PointF[] interestPoints)
        {
            float num1;
            float x1 = num1 = interestPoints[0].X;
            float num2;
            float y1 = num2 = interestPoints[0].Y;
            for (int index = 1; index < interestPoints.Length; ++index)
            {
                float x2 = interestPoints[index].X;
                float y2 = interestPoints[index].Y;
                if (x2 < (double)x1)
                    x1 = x2;
                if (x2 > (double)num1)
                    num1 = x2;
                if (y2 < (double)y1)
                    y1 = y2;
                if (y2 > (double)num2)
                    num2 = y2;
            }
            return new RectangleF(x1, y1, num1 - x1, num2 - y1);
        }

        private PointF NudgeIntoBounds(
          RectangleF layoutBounds,
          PointF bestPosition,
          RectangleF childBounds,
          PlacementMode placement)
        {
            bestPosition.X = Math2.Clamp(bestPosition.X, layoutBounds.Left, layoutBounds.Right - childBounds.Width);
            bestPosition.Y = Math2.Clamp(bestPosition.Y, layoutBounds.Top, layoutBounds.Bottom - childBounds.Height);
            return bestPosition;
        }

        private RectangleF GetMouseRect(PlacementMode placement)
        {
            Point physicalMousePos = UISession.Default.InputManager.MostRecentPhysicalMousePos;
            if (!placement.UsesTargetSize)
                return new RectangleF(physicalMousePos.X, physicalMousePos.Y, 0.0f, 0.0f);
            int height;
            int hotY;
            NativeApi.SpGetMouseCursorInfo(out height, out hotY);
            return new RectangleF(physicalMousePos.X, physicalMousePos.Y - hotY - 1, 0.0f, height + 2);
        }

        private void HookMousePositionChanged(ViewItem subject, bool hook)
        {
            if ((_followMouseSubjects != null && _followMouseSubjects.Contains(subject)) == hook)
                return;
            if (hook)
            {
                if (_followMouseSubjects == null)
                {
                    _followMouseSubjects = new Vector<ViewItem>();
                    UISession.Default.InputManager.MousePositionChanged += new EventHandler(OnMousePositionChanged);
                }
                _followMouseSubjects.Add(subject);
            }
            else
            {
                _followMouseSubjects.Remove(subject);
                if (_followMouseSubjects.Count != 0)
                    return;
                _followMouseSubjects = null;
                UISession.Default.InputManager.MousePositionChanged -= new EventHandler(OnMousePositionChanged);
            }
        }

        private void OnMousePositionChanged(object sender, EventArgs args)
        {
            for (int index = 0; index < _followMouseSubjects.Count; ++index)
            {
                ViewItem followMouseSubject = _followMouseSubjects[index];
                if (followMouseSubject.Layout == this && !followMouseSubject.IsDisposed)
                {
                    followMouseSubject.MarkLayoutInvalid();
                }
                else
                {
                    HookMousePositionChanged(followMouseSubject, false);
                    if (_followMouseSubjects == null)
                        break;
                    --index;
                }
            }
        }

        public bool Equals(ILayout other)
        {
            if (other is not PopupLayout o) return false;

            return DefaultChildAlignment == o.DefaultChildAlignment;
        }

        private class PlacementTargetInfo
        {
            public ViewItem child;
            public ViewItem placementTarget;
            public RectangleF bounds;

            public PlacementTargetInfo(ViewItem child, ViewItem placementTarget, RectangleF bounds)
            {
                this.child = child;
                this.placementTarget = placementTarget;
                this.bounds = bounds;
            }
        }
    }
}
