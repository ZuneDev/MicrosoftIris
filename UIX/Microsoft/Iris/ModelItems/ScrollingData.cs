// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.ModelItems.ScrollingData
// Assembly: UIX, Version=2.1.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: 57C02C21-1B9A-4AE2-836C-8816A259A92A
// Assembly location: D:\Downloads\Zune Software v2.5\packages\Zune-x86\UIX.dll

using Microsoft.Iris.Input;
using Microsoft.Iris.Layout;
using Microsoft.Iris.Layouts;
using Microsoft.Iris.Library;
using Microsoft.Iris.Markup;
using Microsoft.Iris.Navigation;
using Microsoft.Iris.Render;
using Microsoft.Iris.RenderAPI.Drawing;
using Microsoft.Iris.Session;
using Microsoft.Iris.UI;
using System.ComponentModel;

#nullable disable
namespace Microsoft.Iris.ModelItems
{
    internal class ScrollingData : NotifyObjectBase
    {
        private ScrollingLayoutInput _input;
        private ScrollingLayoutOutput _output;
        private ScrollIntoViewDisposition _userDisposition;
        private bool _useUserDisposition;
        private ViewItem _targetItem;
        private ViewItem _lastFocusedItem;
        private UIClass _uiWithAreaOfInterestToClear;
        private bool _isCamping;
        private bool _canDoMoveUp;
        private bool _canDoMoveDown;
        private bool _pageSizedScrollStepFlag;
        private int _scrollStepValue;
        private Orientation _scrollOrientation;
        private ScrollingData.PostLayoutAction _postLayoutAction;
        private ScrollingData.NavigateAction _navigateAction;
        private ScrollingData.AssignFocusAction _assignFocusAction;

        public ScrollingData()
        {
            this._input = new ScrollingLayoutInput();
            this._output = new ScrollingLayoutOutput();
            this.ScrollStep = 50;
            this.PageStep = 0.8f;
            this._userDisposition = new ScrollIntoViewDisposition(0);
            this._userDisposition.Enabled = true;
            this._useUserDisposition = true;
            this.ActualScrollIntoViewDisposition = new ScrollIntoViewDisposition(0);
            this.ActualScrollIntoViewDisposition.Enabled = true;
            this._canDoMoveUp = true;
            this._canDoMoveDown = true;
            this._scrollOrientation = Orientation.Horizontal;
        }

        public void AttachToViewItem(ViewItem vi)
        {
            vi.LayoutInput = (ILayoutInput)this._input;
            vi.LayoutComplete += OnLayoutComplete;
            this._targetItem = vi;
        }

        public void DetachFromViewItem(ViewItem vi)
        {
            this._targetItem.LayoutComplete -= new LayoutCompleteEventHandler(this.OnLayoutComplete);
            this._targetItem.SetLayoutInput(ScrollingLayoutInput.Data, (ILayoutInput)null);
            this._targetItem = (ViewItem)null;
        }

        public ViewItem TargetViewItem
        {
            get => this._targetItem;
            set
            {
                if (this._targetItem == value)
                    return;
                this.AttachToViewItem(value);
            }
        }

        internal Orientation ScrollOrientation
        {
            get => this._scrollOrientation;
            set
            {
                if (this._scrollOrientation == value)
                    return;
                this._scrollOrientation = value;
            }
        }

        public bool Enabled
        {
            get => this._input.Enabled;
            set
            {
                if (this._input.Enabled == value)
                    return;
                this._input.Enabled = value;
                this.OnLayoutInputChanged();
                this.FireNotification(NotificationID.Enabled);
            }
        }

        public int ScrollStep
        {
            get => this._scrollStepValue;
            set
            {
                if (this._scrollStepValue == value)
                    return;
                this._scrollStepValue = value;
                this.FireNotification(NotificationID.ScrollStep);
            }
        }

        public float PageStep
        {
            get => this._input.PageStep;
            set
            {
                if ((double)this._input.PageStep == (double)value)
                    return;
                this._input.PageStep = value;
                this.OnLayoutInputChanged();
                this.FireNotification(NotificationID.PageStep);
            }
        }

        public bool PageSizedScrollStep
        {
            get => this._pageSizedScrollStepFlag;
            set
            {
                if (this._pageSizedScrollStepFlag == value)
                    return;
                this._pageSizedScrollStepFlag = value;
                this.FireNotification(NotificationID.PageSizedScrollStep);
            }
        }

        public void Scroll(int amount)
        {
            this.DisableScrollIntoView();
            this._input.Scroll(amount);
            this.OnLayoutInputChanged();
        }

        public void ScrollUp() => this.ScrollUp(1);

        public void ScrollUp(bool attemptFocusBehavior)
        {
            if (attemptFocusBehavior)
                this.MoveDirection(true);
            else
                this.ScrollUp();
        }

        public void ScrollUp(int numTimes)
        {
            if (this._pageSizedScrollStepFlag)
                this.PageUp(numTimes);
            else
                this.Scroll(numTimes * -this._scrollStepValue);
        }

        public void ScrollDown() => this.ScrollDown(1);

        public void ScrollDown(bool attemptFocusBehavior)
        {
            if (attemptFocusBehavior)
                this.MoveDirection(false);
            else
                this.ScrollDown();
        }

        public void ScrollDown(int numTimes)
        {
            if (this._pageSizedScrollStepFlag)
                this.PageDown(numTimes);
            else
                this.Scroll(numTimes * this._scrollStepValue);
        }

        public void PageUp() => this.PageUp(1);

        public void PageUp(bool attemptFocusBehavior)
        {
            if (attemptFocusBehavior)
                this.MovePage(true);
            else
                this.PageUp();
        }

        public void PageUp(int numTimes)
        {
            this.DisableScrollIntoView();
            for (; numTimes > 0; --numTimes)
                this._input.PageUp();
            this.OnLayoutInputChanged();
        }

        public void PageDown() => this.PageDown(1);

        public void PageDown(bool attemptFocusBehavior)
        {
            if (attemptFocusBehavior)
                this.MovePage(false);
            else
                this.PageDown();
        }

        public void PageDown(int numTimes)
        {
            this.DisableScrollIntoView();
            for (; numTimes > 0; --numTimes)
                this._input.PageDown();
            this.OnLayoutInputChanged();
        }

        public void Home()
        {
            this.DisableScrollIntoView();
            this._input.Home();
            this.OnLayoutInputChanged();
        }

        public void Home(bool attemptFocusBehavior)
        {
            if (attemptFocusBehavior)
                this.MoveToEndPoint(true);
            else
                this.Home();
        }

        public void End()
        {
            this.DisableScrollIntoView();
            this._input.End();
            this.OnLayoutInputChanged();
        }

        public void End(bool attemptFocusBehavior)
        {
            if (attemptFocusBehavior)
                this.MoveToEndPoint(false);
            else
                this.End();
        }

        public void ScrollToPosition(float position)
        {
            if ((double)position < 0.0)
                position = 0.0f;
            else if ((double)position > 1.0)
                position = 1f;
            this.DisableScrollIntoView();
            this._input.ScrollToPosition(position);
            this.OnLayoutInputChanged();
        }

        public void ScrollFocusIntoView() => this.EnableScrollIntoView();

        internal ScrollIntoViewDisposition ScrollIntoViewDisposition
        {
            get => this._userDisposition;
            set
            {
                if (this._userDisposition.Equals((object)value))
                    return;
                this._userDisposition = value;
                this.EnableScrollIntoView();
            }
        }

        private ScrollIntoViewDisposition ActualScrollIntoViewDisposition
        {
            get => this._input.ScrollIntoViewDisposition;
            set => this._input.ScrollIntoViewDisposition = value;
        }

        [DefaultValue(0)]
        public int BeginPadding
        {
            get => this.ScrollIntoViewDisposition.BeginPadding;
            set
            {
                if (this.ScrollIntoViewDisposition.BeginPadding == value)
                    return;
                this.ScrollIntoViewDisposition.BeginPadding = value;
                this.EnableScrollIntoView();
                this.FireNotification(NotificationID.BeginPadding);
            }
        }

        [DefaultValue(0)]
        public int EndPadding
        {
            get => this.ScrollIntoViewDisposition.EndPadding;
            set
            {
                if (this.ScrollIntoViewDisposition.EndPadding == value)
                    return;
                this.ScrollIntoViewDisposition.EndPadding = value;
                this.EnableScrollIntoView();
                this.FireNotification(NotificationID.EndPadding);
            }
        }

        [DefaultValue(RelativeEdge.Near)]
        public RelativeEdge BeginPaddingRelativeTo
        {
            get => this.ScrollIntoViewDisposition.BeginPaddingRelativeTo;
            set
            {
                if (this.ScrollIntoViewDisposition.BeginPaddingRelativeTo == value)
                    return;
                this.ScrollIntoViewDisposition.BeginPaddingRelativeTo = value;
                this.EnableScrollIntoView();
                this.FireNotification(NotificationID.BeginPaddingRelativeTo);
            }
        }

        [DefaultValue(RelativeEdge.Far)]
        public RelativeEdge EndPaddingRelativeTo
        {
            get => this.ScrollIntoViewDisposition.EndPaddingRelativeTo;
            set
            {
                if (this.ScrollIntoViewDisposition.EndPaddingRelativeTo == value)
                    return;
                this.ScrollIntoViewDisposition.EndPaddingRelativeTo = value;
                this.EnableScrollIntoView();
                this.FireNotification(NotificationID.EndPaddingRelativeTo);
            }
        }

        [DefaultValue(-1f)]
        public float LockedPosition
        {
            get => this.ScrollIntoViewDisposition.LockedPosition;
            set
            {
                if ((double)this.ScrollIntoViewDisposition.LockedPosition == (double)value)
                    return;
                this.ScrollIntoViewDisposition.LockedPosition = value;
                this.EnableScrollIntoView();
                this.FireNotification(NotificationID.LockedPosition);
            }
        }

        public bool Locked
        {
            get => this.ScrollIntoViewDisposition.Locked;
            set
            {
                if (this.ScrollIntoViewDisposition.Locked == value)
                    return;
                this.ScrollIntoViewDisposition.Locked = value;
                this.EnableScrollIntoView();
                this.FireNotification(NotificationID.Locked);
            }
        }

        public ContentPositioningPolicy ContentPositioningBehavior
        {
            get => this.ScrollIntoViewDisposition.ContentPositioningBehavior;
            set
            {
                if (this.ScrollIntoViewDisposition.ContentPositioningBehavior == value)
                    return;
                this.ScrollIntoViewDisposition.ContentPositioningBehavior = value;
                this.EnableScrollIntoView();
                this.FireNotification(NotificationID.ContentPositioningBehavior);
            }
        }

        [DefaultValue(0.5f)]
        public float LockedAlignment
        {
            get => this.ScrollIntoViewDisposition.LockedAlignment;
            set
            {
                if ((double)this.ScrollIntoViewDisposition.LockedAlignment == (double)value)
                    return;
                this.ScrollIntoViewDisposition.LockedAlignment = value;
                this.EnableScrollIntoView();
                this.FireNotification(NotificationID.LockedAlignment);
            }
        }

        public bool CanScrollUp => this.Enabled && this._output.CanScrollNegative;

        public bool CanScrollDown => this.Enabled && this._output.CanScrollPositive;

        public float CurrentPage => this._output.CurrentPage;

        public float TotalPages => this._output.TotalPages;

        private void MovePage(bool nearDirection)
        {
            if (this.HadFocus())
            {
                if (!this.CanMoveDirection(nearDirection))
                    return;
                bool flag = true;
                switch (this.GetLastFocusLocation())
                {
                    case ScrollingData.ItemLocation.OffscreenInNearDirection:
                        flag = nearDirection;
                        break;
                    case ScrollingData.ItemLocation.Onscreen:
                        flag = !this.PotentialNavigationTargetIsOnscreen(this.NearFarToDirection(nearDirection), out UIClass _);
                        if (!flag && this._useUserDisposition && this.NonDefaultUserDisposition())
                        {
                            flag = true;
                            break;
                        }
                        break;
                    case ScrollingData.ItemLocation.OffscreenInFarDirection:
                        flag = !nearDirection;
                        break;
                }
                ScrollingData.AssignFocusAction instance = ScrollingData.AssignFocusAction.GetInstance(this, this.GetAssignFocusPoint(nearDirection), nearDirection, false);
                if (!flag)
                {
                    instance.Go();
                }
                else
                {
                    float num = !nearDirection ? 0.0f : 1f;
                    this.ActualScrollIntoViewDisposition.Reset();
                    this.ActualScrollIntoViewDisposition.Enabled = true;
                    this.ActualScrollIntoViewDisposition.LockedPosition = num;
                    this.ActualScrollIntoViewDisposition.LockedAlignment = num;
                    this.SetPendingFocusAreaOfInterest(this._lastFocusedItem.UI);
                    this.OnLayoutInputChanged();
                    this.SetPostLayoutAction((ScrollingData.PostLayoutAction)instance);
                }
            }
            else if (nearDirection)
            {
                if (!this.CanScrollUp)
                    return;
                this.PageUp();
            }
            else
            {
                if (!this.CanScrollDown)
                    return;
                this.PageDown();
            }
        }

        public void NotifyFocusChange(UIClass newUI)
        {
            if (newUI == null || newUI == this._uiWithAreaOfInterestToClear || this._lastFocusedItem != null && this._lastFocusedItem.UI == newUI)
                return;
            this.EnableScrollIntoView();
        }

        private void SetPostLayoutAction(ScrollingData.PostLayoutAction action)
        {
            this._postLayoutAction = action;
        }

        private void DeliverPostLayoutAction()
        {
            if (this._postLayoutAction == null)
                return;
            this._postLayoutAction.Go();
            this._postLayoutAction = (ScrollingData.PostLayoutAction)null;
        }

        private Direction NearFarToDirection(bool near)
        {
            if (this.ScrollOrientation == Orientation.Vertical)
                return !near ? Direction.South : Direction.North;
            near ^= this._targetItem.Zone.Session.IsRtl;
            return !near ? Direction.East : Direction.West;
        }

        private bool LastFocusIsOnscreen()
        {
            return this.GetItemLocation(this._lastFocusedItem) == ScrollingData.ItemLocation.Onscreen;
        }

        private ScrollingData.ItemLocation GetLastFocusLocation()
        {
            return this.GetItemLocation(this._lastFocusedItem);
        }

        private bool ItemIsOnscreen(ViewItem item)
        {
            return this.GetItemLocation(item) == ScrollingData.ItemLocation.Onscreen;
        }

        private ScrollingData.ItemLocation GetItemLocation(ViewItem item)
        {
            RectangleF scrollerRect = this.GetScrollerRect(false);
            RectangleF viewItemRect = this.GetViewItemRect(item, false);
            return !(RectangleF.Intersect(scrollerRect, viewItemRect) == viewItemRect) ? (this.ScrollOrientation != Orientation.Horizontal ? ((double)viewItemRect.Top >= (double)scrollerRect.Top ? ScrollingData.ItemLocation.OffscreenInFarDirection : ScrollingData.ItemLocation.OffscreenInNearDirection) : ((double)viewItemRect.Left < (double)scrollerRect.Left || this._targetItem.Zone.Session.IsRtl && (double)viewItemRect.Right > (double)scrollerRect.Right ? ScrollingData.ItemLocation.OffscreenInNearDirection : ScrollingData.ItemLocation.OffscreenInFarDirection)) : ScrollingData.ItemLocation.Onscreen;
        }

        private bool PotentialNavigationTargetIsOnscreen(
            Direction dir,
            out UIClass navigationResult)
        {
            return this.PotentialNavigationTargetIsOnscreen(this._lastFocusedItem.UI, dir, out navigationResult);
        }

        private bool PotentialNavigationTargetIsOnscreen(
            UIClass ui,
            Direction dir,
            out UIClass navigationResult)
        {
            bool flag = false;
            if (this.PotentialNavigationTargetIsDescendant(ui, dir, out navigationResult))
                flag = this.ItemIsOnscreen(navigationResult.RootItem);
            return flag;
        }

        private bool PotentialNavigationTargetIsDescendant(
            Direction dir,
            out UIClass navigationResult)
        {
            return this.PotentialNavigationTargetIsDescendant(this._lastFocusedItem.UI, dir, out navigationResult);
        }

        private bool PotentialNavigationTargetIsDescendant(
            UIClass ui,
            Direction dir,
            out UIClass navigationResult)
        {
            bool flag = false;
            if (ui.FindNextFocusablePeer(dir, RectangleF.Zero, out navigationResult) && navigationResult != null && this._targetItem.HasDescendant((TreeNode)navigationResult.RootItem))
                flag = true;
            return flag;
        }

        private PointF GetMoveToEndpointPoint(bool near)
        {
            PointF zero = PointF.Zero;
            RectangleF scrollerRect = this.GetScrollerRect(true);
            bool isRtl = this._targetItem.Zone.Session.IsRtl;
            if (near)
            {
                if (isRtl)
                    zero.X = scrollerRect.Right;
            }
            else
            {
                zero.X = isRtl ? scrollerRect.Left : scrollerRect.Right;
                zero.Y = scrollerRect.Bottom;
            }
            return zero;
        }

        private PointF GetAssignFocusPoint(bool near)
        {
            RectangleF lastFocusRect = this.GetLastFocusRect(true);
            RectangleF scrollerRect = this.GetScrollerRect(true);
            PointF center = lastFocusRect.Center;
            float x;
            float y;
            if (this.ScrollOrientation == Orientation.Vertical)
            {
                x = center.X;
                float num = lastFocusRect.Height / 2f;
                y = !near ? scrollerRect.Bottom - num : scrollerRect.Top + num;
            }
            else
            {
                y = center.Y;
                float num = lastFocusRect.Width / 2f;
                x = !(near ^ this._targetItem.Zone.Session.IsRtl) ? scrollerRect.Right - num : scrollerRect.Left + num;
            }
            return new PointF(x, y);
        }

        private void MoveToEndPoint(bool home)
        {
            bool flag1 = this.HadFocus();
            bool flag2 = false;
            bool flag3 = false;
            if (flag1)
                flag3 = this.CanMoveDirection(home);
            if (flag3 || !flag1)
                flag2 = !home ? this.CanScrollDown : this.CanScrollUp;
            if (flag2)
            {
                if (home)
                    this.Home();
                else
                    this.End();
            }
            if (!flag3)
                return;
            ScrollingData.AssignFocusAction instance = ScrollingData.AssignFocusAction.GetInstance(this, PointF.Zero, home, true);
            if (!flag2)
                instance.Go();
            else
                this.SetPostLayoutAction((ScrollingData.PostLayoutAction)instance);
        }

        private void MoveDirection(bool nearDirection)
        {
            if (this.HadFocus())
            {
                if (!this.CanMoveDirection(nearDirection))
                    return;
                Direction direction = this.NearFarToDirection(nearDirection);
                UIClass ui = this._lastFocusedItem.UI;
                bool flag = true;
                UIClass navigationResult;
                if (this.LastFocusIsOnscreen() && this.PotentialNavigationTargetIsOnscreen(direction, out navigationResult))
                {
                    this.NavigateToUI(navigationResult);
                    this.EnableScrollIntoView();
                    flag = false;
                }
                if (!flag)
                    return;
                this.ActualScrollIntoViewDisposition.Reset();
                this.ActualScrollIntoViewDisposition.Enabled = true;
                this.SetPendingFocusAreaOfInterest(this._lastFocusedItem.UI);
                this.SetPostLayoutAction((ScrollingData.PostLayoutAction)ScrollingData.NavigateAction.GetInstance(this, nearDirection, direction));
                this.OnLayoutInputChanged();
            }
            else if (nearDirection)
            {
                if (!this.CanScrollUp)
                    return;
                this.ScrollUp();
            }
            else
            {
                if (!this.CanScrollDown)
                    return;
                this.ScrollDown();
            }
        }

        private void DisableScrollIntoView()
        {
            this.ActualScrollIntoViewDisposition.Enabled = false;
            this._useUserDisposition = false;
        }

        private void EnableScrollIntoView()
        {
            this.ActualScrollIntoViewDisposition.CopyFrom(this._userDisposition);
            this._useUserDisposition = true;
            this.OnLayoutInputChanged();
        }

        private void OnLayoutInputChanged()
        {
            if (this._targetItem == null)
                return;
            this._targetItem.MarkLayoutInvalid();
        }

        private void OnLayoutOutputChanged()
        {
            ScrollingLayoutOutput output = this._output;
            if (!(this._targetItem.GetExtendedLayoutOutput(ScrollingLayoutOutput.DataCookie) is ScrollingLayoutOutput scrollingLayoutOutput))
                scrollingLayoutOutput = new ScrollingLayoutOutput();
            this._output = scrollingLayoutOutput;
            if (this._output.ProcessedExplicitScrollIntoViewRequest || !this._output.ScrollFocusIntoView)
                this.DisableScrollIntoView();
            if (this._output.CanScrollNegative != output.CanScrollNegative)
                this.FireNotification(NotificationID.CanScrollUp);
            if (this._output.CanScrollPositive != output.CanScrollPositive)
                this.FireNotification(NotificationID.CanScrollDown);
            if ((double)this._output.CurrentPage != (double)output.CurrentPage)
                this.FireNotification(NotificationID.CurrentPage);
            if ((double)this._output.TotalPages == (double)output.TotalPages)
                return;
            this.FireNotification(NotificationID.TotalPages);
        }

        private void OnLayoutComplete(object sender)
        {
            this.OnLayoutOutputChanged();
            UIClass keyFocusDescendant = this._targetItem.UI.KeyFocusDescendant;
            if (keyFocusDescendant != null && keyFocusDescendant != this._targetItem.UI && this._targetItem.HasDescendant((TreeNode)keyFocusDescendant.RootItem))
                this._lastFocusedItem = keyFocusDescendant.RootItem;
            this.ClearPendingFocusAreaOfInterest();
            if (!this._useUserDisposition)
            {
                this.ActualScrollIntoViewDisposition.Reset();
                this.ActualScrollIntoViewDisposition.Enabled = false;
            }
            else
                this.ActualScrollIntoViewDisposition.CopyFrom(this._userDisposition);
            this._input.SecondaryScrollIntoViewDisposition = (ScrollIntoViewDisposition)null;
            this.DeliverPostLayoutAction();
        }

        private void ClearPendingFocusAreaOfInterest()
        {
            if (this._uiWithAreaOfInterestToClear == null)
                return;
            if (!this._uiWithAreaOfInterestToClear.IsDisposed)
                this._uiWithAreaOfInterestToClear.ClearAreaOfInterest(AreaOfInterestID.PendingFocus);
            this._uiWithAreaOfInterestToClear = (UIClass)null;
        }

        private void SetPendingFocusAreaOfInterest(UIClass ui)
        {
            this.ClearPendingFocusAreaOfInterest();
            if (ui.KeyFocus)
                return;
            ui.SetAreaOfInterest(AreaOfInterestID.PendingFocus);
            this._uiWithAreaOfInterestToClear = ui;
        }

        private bool HadFocus()
        {
            if (this._lastFocusedItem != null && this._lastFocusedItem.IsDisposed)
                this._lastFocusedItem = (ViewItem)null;
            return this._lastFocusedItem != null;
        }

        private bool NonDefaultUserDisposition() => !this._userDisposition.IsDefault;

        private RectangleF GetLastFocusRect(bool forNavigation)
        {
            return this.GetViewItemRect(this._lastFocusedItem, forNavigation);
        }

        private RectangleF GetScrollerRect(bool forNavigation)
        {
            return this.GetViewItemRect(this._targetItem, forNavigation);
        }

        private RectangleF GetViewItemRect(ViewItem item, bool forNavigation)
        {
            if (!forNavigation)
            {
                var rect = ((ITrackableUIElement)item).EstimatePosition(_targetItem);
                return RectangleF.FromRectangle(rect);
            }

            ((INavigationSite)item).ComputeBounds(out Vector3 positionPxlVector, out Vector3 sizePxlVector);
            return new RectangleF(positionPxlVector.X, positionPxlVector.Y, sizePxlVector.X, sizePxlVector.Y);
        }

        private void NavigateToUI(UIClass ui)
        {
            ui.NotifyNavigationDestination(KeyFocusReason.Directional);
            this.SetPendingFocusAreaOfInterest(ui);
        }

        public void BeginCamp() => this._isCamping = true;

        public void EndCamp()
        {
            this._isCamping = false;
            this._canDoMoveUp = true;
            this._canDoMoveDown = true;
        }

        private bool CanMoveDirection(bool nearDirection)
        {
            return nearDirection ? this._canDoMoveUp : this._canDoMoveDown;
        }

        private void NoteFutileMoveAttempt(bool near)
        {
            if (!this._isCamping)
                return;
            if (near)
                this._canDoMoveUp = false;
            else
                this._canDoMoveDown = false;
        }

        private enum ItemLocation
        {
            OffscreenInNearDirection,
            Onscreen,
            OffscreenInFarDirection,
        }

        private abstract class PostLayoutAction
        {
            private ScrollingData _data;
            private bool _nearDirection;

            protected PostLayoutAction(ScrollingData data) => this._data = data;

            protected void Initialize(bool nearDirection) => this._nearDirection = nearDirection;

            protected bool NearDirection => this._nearDirection;

            protected ScrollingData Data => this._data;

            protected UIClass Origin => this._data._lastFocusedItem.UI;

            protected ViewItem Target => this._data._targetItem;

            protected void NoteFutileMoveAttempt()
            {
                this.Data.NoteFutileMoveAttempt(this._nearDirection);
            }

            public abstract void Go();
        }

        private class NavigateAction : ScrollingData.PostLayoutAction
        {
            private Direction _direction;

            private NavigateAction(ScrollingData data)
                : base(data)
            {
            }

            public static ScrollingData.NavigateAction GetInstance(
                ScrollingData data,
                bool nearDirection,
                Direction direction)
            {
                if (data._navigateAction == null)
                    data._navigateAction = new ScrollingData.NavigateAction(data);
                ScrollingData.NavigateAction navigateAction = data._navigateAction;
                navigateAction.Initialize(nearDirection);
                navigateAction._direction = direction;
                return navigateAction;
            }

            public override void Go()
            {
                if (this.Origin.IsDisposed)
                    return;
                UIClass origin = this.Origin.KeyFocus ? this.Origin : (UIClass)null;
                UIClass resultUI;
                if (this.Origin.FindNextFocusablePeer(this._direction, RectangleF.Zero, out resultUI) && resultUI != null && resultUI != origin && this.Target.HasDescendant((TreeNode)resultUI.RootItem))
                {
                    this.Data.NavigateToUI(resultUI);
                    this.Data.EnableScrollIntoView();
                }
                else
                    this.NoteFutileMoveAttempt();
            }
        }

        private class AssignFocusAction : ScrollingData.PostLayoutAction
        {
            private bool _tryNonBiasedSearchFirst;
            private PointF _assignPoint;
            private float _lockedPosition;

            private AssignFocusAction(ScrollingData data)
                : base(data)
            {
            }

            public static ScrollingData.AssignFocusAction GetInstance(
                ScrollingData data,
                PointF restorePoint,
                bool nearEndpoint,
                bool forceToEndpoint)
            {
                if (data._assignFocusAction == null)
                    data._assignFocusAction = new ScrollingData.AssignFocusAction(data);
                ScrollingData.AssignFocusAction assignFocusAction = data._assignFocusAction;
                assignFocusAction.Initialize(nearEndpoint);
                assignFocusAction.LockToEdge(nearEndpoint);
                assignFocusAction._assignPoint = restorePoint;
                assignFocusAction._tryNonBiasedSearchFirst = !forceToEndpoint;
                return assignFocusAction;
            }

            public override void Go()
            {
                UIClass ui = (UIClass)null;
                if (this._tryNonBiasedSearchFirst)
                    ui = this.FindNavigationResult(true);
                if (ui == null)
                    ui = this.FindNavigationResult(false);
                if (ui != null)
                    this.Data.NavigateToUI(ui);
                else
                    this.NoteFutileMoveAttempt();
                this.ApplyLockedPosition();
            }

            private UIClass FindNavigationResult(bool findNearest)
            {
                UIClass navigationResult = (UIClass)null;
                INavigationSite result;
                bool fromPoint;
                if (findNearest)
                {
                    fromPoint = NavigationServices.FindFromPoint((INavigationSite)this.Target, this._assignPoint, out result);
                }
                else
                {
                    Direction direction = this.Data.NearFarToDirection(this.NearDirection);
                    this._assignPoint = this.Data.GetMoveToEndpointPoint(this.NearDirection);
                    fromPoint = NavigationServices.FindFromPoint((INavigationSite)this.Target, direction, this._assignPoint, out result);
                }
                if (fromPoint && result != null)
                {
                    ViewItem viewItem = result as ViewItem;
                    if (viewItem.UI != this.Origin || !this.Origin.KeyFocus)
                        navigationResult = viewItem.UI;
                }
                return navigationResult;
            }

            private void LockToEdge(bool near)
            {
                if (near)
                    this._lockedPosition = 0.0f;
                else
                    this._lockedPosition = 1f;
            }

            protected void ApplyLockedPosition()
            {
                if (this.Data.NonDefaultUserDisposition())
                    this.Data._input.SecondaryScrollIntoViewDisposition = this.Data.ScrollIntoViewDisposition;
                this.Data.ActualScrollIntoViewDisposition.LockedPosition = this._lockedPosition;
                this.Data.ActualScrollIntoViewDisposition.LockedAlignment = this._lockedPosition;
                this.Data.ActualScrollIntoViewDisposition.Locked = true;
                this.Data.ActualScrollIntoViewDisposition.ContentPositioningBehavior = ContentPositioningPolicy.ShowMaximalContent;
                this.Data.ActualScrollIntoViewDisposition.Enabled = true;
                this.Data._useUserDisposition = true;
                this.Data.OnLayoutInputChanged();
            }
        }
    }
}
