// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.ModelItems.ScrollModel
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Input;
using Microsoft.Iris.Layout;
using Microsoft.Iris.Layouts;
using Microsoft.Iris.Markup;
using Microsoft.Iris.Navigation;
using Microsoft.Iris.Render;
using Microsoft.Iris.RenderAPI.Drawing;
using Microsoft.Iris.Session;
using Microsoft.Iris.UI;

namespace Microsoft.Iris.ModelItems
{
    internal class ScrollModel : ScrollModelBase
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
        private ScrollModel.PostLayoutAction _postLayoutAction;
        private ScrollModel.NavigateAction _navigateAction;
        private ScrollModel.AssignFocusAction _assignFocusAction;

        public ScrollModel()
        {
            _input = new ScrollingLayoutInput();
            _output = new ScrollingLayoutOutput();
            ScrollStep = 50;
            PageStep = 0.8f;
            _userDisposition = new ScrollIntoViewDisposition(0);
            _userDisposition.Enabled = true;
            _useUserDisposition = true;
            ActualScrollIntoViewDisposition = new ScrollIntoViewDisposition(0);
            ActualScrollIntoViewDisposition.Enabled = true;
            _canDoMoveUp = true;
            _canDoMoveDown = true;
            _scrollOrientation = Orientation.Horizontal;
        }

        public void AttachToViewItem(ViewItem vi)
        {
            vi.LayoutInput = _input;
            vi.LayoutComplete += new LayoutCompleteEventHandler(OnLayoutComplete);
            _targetItem = vi;
        }

        public void DetachFromViewItem(ViewItem vi)
        {
            _targetItem.LayoutComplete -= new LayoutCompleteEventHandler(OnLayoutComplete);
            _targetItem.SetLayoutInput(ScrollingLayoutInput.Data, null);
            _targetItem = null;
            _output = null;
            _lastFocusedItem = null;
            _uiWithAreaOfInterestToClear = null;
            _postLayoutAction = null;
            _navigateAction = null;
            _assignFocusAction = null;
        }

        public ViewItem TargetViewItem
        {
            get => _targetItem;
            set
            {
                if (_targetItem == value)
                    return;
                AttachToViewItem(value);
            }
        }

        internal Orientation ScrollOrientation
        {
            get => _scrollOrientation;
            set
            {
                if (_scrollOrientation == value)
                    return;
                _scrollOrientation = value;
            }
        }

        public bool Enabled
        {
            get => _input.Enabled;
            set
            {
                if (_input.Enabled == value)
                    return;
                _input.Enabled = value;
                OnLayoutInputChanged();
                FireNotification(NotificationID.Enabled);
            }
        }

        public override int ScrollStep
        {
            get => _scrollStepValue;
            set
            {
                if (_scrollStepValue == value)
                    return;
                _scrollStepValue = value;
                FireNotification(NotificationID.ScrollStep);
            }
        }

        public float PageStep
        {
            get => _input.PageStep;
            set
            {
                if (_input.PageStep == (double)value)
                    return;
                _input.PageStep = value;
                OnLayoutInputChanged();
                FireNotification(NotificationID.PageStep);
            }
        }

        public bool PageSizedScrollStep
        {
            get => _pageSizedScrollStepFlag;
            set
            {
                if (_pageSizedScrollStepFlag == value)
                    return;
                _pageSizedScrollStepFlag = value;
                FireNotification(NotificationID.PageSizedScrollStep);
            }
        }

        public override void Scroll(int amount)
        {
            DisableScrollIntoView();
            _input.Scroll(amount);
            OnLayoutInputChanged();
        }

        public override void ScrollUp() => ScrollUp(1);

        public void ScrollUp(bool attemptFocusBehavior)
        {
            if (attemptFocusBehavior)
                MoveDirection(true);
            else
                ScrollUp();
        }

        public void ScrollUp(int numTimes)
        {
            if (_pageSizedScrollStepFlag)
                PageUp(numTimes);
            else
                Scroll(numTimes * -_scrollStepValue);
        }

        public override void ScrollDown() => ScrollDown(1);

        public void ScrollDown(bool attemptFocusBehavior)
        {
            if (attemptFocusBehavior)
                MoveDirection(false);
            else
                ScrollDown();
        }

        public void ScrollDown(int numTimes)
        {
            if (_pageSizedScrollStepFlag)
                PageDown(numTimes);
            else
                Scroll(numTimes * _scrollStepValue);
        }

        public override void PageUp() => PageUp(1);

        public void PageUp(bool attemptFocusBehavior)
        {
            if (attemptFocusBehavior)
                MovePage(true);
            else
                PageUp();
        }

        public void PageUp(int numTimes)
        {
            DisableScrollIntoView();
            for (; numTimes > 0; --numTimes)
                _input.PageUp();
            OnLayoutInputChanged();
        }

        public override void PageDown() => PageDown(1);

        public void PageDown(bool attemptFocusBehavior)
        {
            if (attemptFocusBehavior)
                MovePage(false);
            else
                PageDown();
        }

        public void PageDown(int numTimes)
        {
            DisableScrollIntoView();
            for (; numTimes > 0; --numTimes)
                _input.PageDown();
            OnLayoutInputChanged();
        }

        public override void Home()
        {
            DisableScrollIntoView();
            _input.Home();
            OnLayoutInputChanged();
        }

        public void Home(bool attemptFocusBehavior)
        {
            if (attemptFocusBehavior)
                MoveToEndPoint(true);
            else
                Home();
        }

        public override void End()
        {
            DisableScrollIntoView();
            _input.End();
            OnLayoutInputChanged();
        }

        public void End(bool attemptFocusBehavior)
        {
            if (attemptFocusBehavior)
                MoveToEndPoint(false);
            else
                End();
        }

        public override void ScrollToPosition(float position)
        {
            if (position < 0.0)
                position = 0.0f;
            else if (position > 1.0)
                position = 1f;
            DisableScrollIntoView();
            _input.ScrollToPosition(position);
            OnLayoutInputChanged();
        }

        public void ScrollFocusIntoView() => EnableScrollIntoView();

        internal ScrollIntoViewDisposition ScrollIntoViewDisposition
        {
            get => _userDisposition;
            set
            {
                if (_userDisposition.Equals(value))
                    return;
                _userDisposition = value;
                EnableScrollIntoView();
            }
        }

        private ScrollIntoViewDisposition ActualScrollIntoViewDisposition
        {
            get => _input.ScrollIntoViewDisposition;
            set => _input.ScrollIntoViewDisposition = value;
        }

        public int BeginPadding
        {
            get => ScrollIntoViewDisposition.BeginPadding;
            set
            {
                if (ScrollIntoViewDisposition.BeginPadding == value)
                    return;
                ScrollIntoViewDisposition.BeginPadding = value;
                EnableScrollIntoView();
                FireNotification(NotificationID.BeginPadding);
            }
        }

        public int EndPadding
        {
            get => ScrollIntoViewDisposition.EndPadding;
            set
            {
                if (ScrollIntoViewDisposition.EndPadding == value)
                    return;
                ScrollIntoViewDisposition.EndPadding = value;
                EnableScrollIntoView();
                FireNotification(NotificationID.EndPadding);
            }
        }

        public RelativeEdge BeginPaddingRelativeTo
        {
            get => ScrollIntoViewDisposition.BeginPaddingRelativeTo;
            set
            {
                if (ScrollIntoViewDisposition.BeginPaddingRelativeTo == value)
                    return;
                ScrollIntoViewDisposition.BeginPaddingRelativeTo = value;
                EnableScrollIntoView();
                FireNotification(NotificationID.BeginPaddingRelativeTo);
            }
        }

        public RelativeEdge EndPaddingRelativeTo
        {
            get => ScrollIntoViewDisposition.EndPaddingRelativeTo;
            set
            {
                if (ScrollIntoViewDisposition.EndPaddingRelativeTo == value)
                    return;
                ScrollIntoViewDisposition.EndPaddingRelativeTo = value;
                EnableScrollIntoView();
                FireNotification(NotificationID.EndPaddingRelativeTo);
            }
        }

        public float LockedPosition
        {
            get => ScrollIntoViewDisposition.LockedPosition;
            set
            {
                if (ScrollIntoViewDisposition.LockedPosition == (double)value)
                    return;
                ScrollIntoViewDisposition.LockedPosition = value;
                EnableScrollIntoView();
                FireNotification(NotificationID.LockedPosition);
            }
        }

        public bool Locked
        {
            get => ScrollIntoViewDisposition.Locked;
            set
            {
                if (ScrollIntoViewDisposition.Locked == value)
                    return;
                ScrollIntoViewDisposition.Locked = value;
                EnableScrollIntoView();
                FireNotification(NotificationID.Locked);
            }
        }

        public ContentPositioningPolicy ContentPositioningBehavior
        {
            get => ScrollIntoViewDisposition.ContentPositioningBehavior;
            set
            {
                if (ScrollIntoViewDisposition.ContentPositioningBehavior == value)
                    return;
                ScrollIntoViewDisposition.ContentPositioningBehavior = value;
                EnableScrollIntoView();
                FireNotification(NotificationID.ContentPositioningBehavior);
            }
        }

        public float LockedAlignment
        {
            get => ScrollIntoViewDisposition.LockedAlignment;
            set
            {
                if (ScrollIntoViewDisposition.LockedAlignment == (double)value)
                    return;
                ScrollIntoViewDisposition.LockedAlignment = value;
                EnableScrollIntoView();
                FireNotification(NotificationID.LockedAlignment);
            }
        }

        public override bool CanScrollUp => Enabled && _output.CanScrollNegative;

        public override bool CanScrollDown => Enabled && _output.CanScrollPositive;

        public override float CurrentPage => _output.CurrentPage;

        public override float TotalPages => _output.TotalPages;

        public override float ViewNear => _output.ViewNear;

        public override float ViewFar => _output.ViewFar;

        private void MovePage(bool nearDirection)
        {
            if (HadFocus())
            {
                if (!CanMoveDirection(nearDirection))
                    return;
                bool flag = true;
                switch (GetLastFocusLocation())
                {
                    case ItemLocation.OffscreenInNearDirection:
                        flag = nearDirection;
                        break;
                    case ItemLocation.Onscreen:
                        flag = !PotentialNavigationTargetIsOnscreen(NearFarToDirection(nearDirection), out UIClass _);
                        if (!flag && _useUserDisposition && NonDefaultUserDisposition())
                        {
                            flag = true;
                            break;
                        }
                        break;
                    case ItemLocation.OffscreenInFarDirection:
                        flag = !nearDirection;
                        break;
                }
                ScrollModel.AssignFocusAction instance = AssignFocusAction.GetInstance(this, GetAssignFocusPoint(nearDirection), nearDirection, false);
                if (!flag)
                {
                    instance.Go();
                }
                else
                {
                    float num = !nearDirection ? 0.0f : 1f;
                    ActualScrollIntoViewDisposition.Reset();
                    ActualScrollIntoViewDisposition.Enabled = true;
                    ActualScrollIntoViewDisposition.LockedPosition = num;
                    ActualScrollIntoViewDisposition.LockedAlignment = num;
                    ActualScrollIntoViewDisposition.Locked = true;
                    SetPendingFocusAreaOfInterest(_lastFocusedItem.UI);
                    OnLayoutInputChanged();
                    SetPostLayoutAction(instance);
                }
            }
            else if (nearDirection)
            {
                if (!CanScrollUp)
                    return;
                PageUp();
            }
            else
            {
                if (!CanScrollDown)
                    return;
                PageDown();
            }
        }

        public void NotifyFocusChange(UIClass newUI)
        {
            if (newUI == null || newUI == _uiWithAreaOfInterestToClear || _lastFocusedItem != null && _lastFocusedItem.UI == newUI)
                return;
            EnableScrollIntoView();
        }

        private void SetPostLayoutAction(ScrollModel.PostLayoutAction action) => _postLayoutAction = action;

        private void DeliverPostLayoutAction()
        {
            if (_postLayoutAction == null)
                return;
            _postLayoutAction.Go();
            _postLayoutAction = null;
        }

        private Direction NearFarToDirection(bool near)
        {
            if (ScrollOrientation == Orientation.Vertical)
                return !near ? Direction.South : Direction.North;
            near ^= _targetItem.Zone.Session.IsRtl;
            return !near ? Direction.East : Direction.West;
        }

        private bool LastFocusIsOnscreen() => GetItemLocation(_lastFocusedItem) == ItemLocation.Onscreen;

        private ScrollModel.ItemLocation GetLastFocusLocation() => GetItemLocation(_lastFocusedItem);

        private bool ItemIsOnscreen(ViewItem item) => GetItemLocation(item) == ItemLocation.Onscreen;

        private ScrollModel.ItemLocation GetItemLocation(ViewItem item)
        {
            RectangleF scrollerRect = GetScrollerRect(false);
            RectangleF viewItemRect = GetViewItemRect(item, false);
            return !(RectangleF.Intersect(scrollerRect, viewItemRect) == viewItemRect) ? (ScrollOrientation != Orientation.Horizontal ? (viewItemRect.Top >= (double)scrollerRect.Top ? ItemLocation.OffscreenInFarDirection : ItemLocation.OffscreenInNearDirection) : (viewItemRect.Left < (double)scrollerRect.Left || _targetItem.Zone.Session.IsRtl && viewItemRect.Right > (double)scrollerRect.Right ? ItemLocation.OffscreenInNearDirection : ItemLocation.OffscreenInFarDirection)) : ItemLocation.Onscreen;
        }

        private bool PotentialNavigationTargetIsOnscreen(Direction dir, out UIClass navigationResult) => PotentialNavigationTargetIsOnscreen(_lastFocusedItem.UI, dir, out navigationResult);

        private bool PotentialNavigationTargetIsOnscreen(
          UIClass ui,
          Direction dir,
          out UIClass navigationResult)
        {
            bool flag = false;
            if (PotentialNavigationTargetIsDescendant(ui, dir, out navigationResult))
                flag = ItemIsOnscreen(navigationResult.RootItem);
            return flag;
        }

        private bool PotentialNavigationTargetIsDescendant(Direction dir, out UIClass navigationResult) => PotentialNavigationTargetIsDescendant(_lastFocusedItem.UI, dir, out navigationResult);

        private bool PotentialNavigationTargetIsDescendant(
          UIClass ui,
          Direction dir,
          out UIClass navigationResult)
        {
            bool flag = false;
            if (ui.FindNextFocusablePeer(dir, RectangleF.Zero, out navigationResult) && navigationResult != null && _targetItem.HasDescendant(navigationResult.RootItem))
                flag = true;
            return flag;
        }

        private PointF GetMoveToEndpointPoint(bool near)
        {
            PointF zero = PointF.Zero;
            RectangleF scrollerRect = GetScrollerRect(true);
            bool isRtl = _targetItem.Zone.Session.IsRtl;
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
            RectangleF lastFocusRect = GetLastFocusRect(true);
            RectangleF scrollerRect = GetScrollerRect(true);
            PointF center = lastFocusRect.Center;
            float x;
            float y;
            if (ScrollOrientation == Orientation.Vertical)
            {
                x = center.X;
                float num = lastFocusRect.Height / 2f;
                y = !near ? scrollerRect.Bottom - num : scrollerRect.Top + num;
            }
            else
            {
                y = center.Y;
                float num = lastFocusRect.Width / 2f;
                x = !(near ^ _targetItem.Zone.Session.IsRtl) ? scrollerRect.Right - num : scrollerRect.Left + num;
            }
            return new PointF(x, y);
        }

        private void MoveToEndPoint(bool home)
        {
            bool flag1 = HadFocus();
            bool flag2 = false;
            bool flag3 = false;
            if (flag1)
                flag3 = CanMoveDirection(home);
            if (flag3 || !flag1)
                flag2 = !home ? CanScrollDown : CanScrollUp;
            if (flag2)
            {
                if (home)
                    Home();
                else
                    End();
            }
            if (!flag3)
                return;
            ScrollModel.AssignFocusAction instance = AssignFocusAction.GetInstance(this, PointF.Zero, home, true);
            if (!flag2)
                instance.Go();
            else
                SetPostLayoutAction(instance);
        }

        private void MoveDirection(bool nearDirection)
        {
            if (HadFocus())
            {
                if (!CanMoveDirection(nearDirection))
                    return;
                Direction direction = NearFarToDirection(nearDirection);
                UIClass ui = _lastFocusedItem.UI;
                bool flag = true;
                UIClass navigationResult;
                if (LastFocusIsOnscreen() && PotentialNavigationTargetIsOnscreen(direction, out navigationResult))
                {
                    NavigateToUI(navigationResult);
                    EnableScrollIntoView();
                    flag = false;
                }
                if (!flag)
                    return;
                ActualScrollIntoViewDisposition.Reset();
                ActualScrollIntoViewDisposition.Enabled = true;
                SetPendingFocusAreaOfInterest(_lastFocusedItem.UI);
                SetPostLayoutAction(NavigateAction.GetInstance(this, nearDirection, direction));
                OnLayoutInputChanged();
            }
            else if (nearDirection)
            {
                if (!CanScrollUp)
                    return;
                ScrollUp();
            }
            else
            {
                if (!CanScrollDown)
                    return;
                ScrollDown();
            }
        }

        private void DisableScrollIntoView()
        {
            ActualScrollIntoViewDisposition.Enabled = false;
            _useUserDisposition = false;
        }

        private void EnableScrollIntoView()
        {
            ActualScrollIntoViewDisposition.CopyFrom(_userDisposition);
            _useUserDisposition = true;
            OnLayoutInputChanged();
        }

        private void OnLayoutInputChanged()
        {
            if (_targetItem == null)
                return;
            _targetItem.MarkLayoutInvalid();
        }

        private void OnLayoutOutputChanged()
        {
            ScrollingLayoutOutput output = _output;
            if (!(_targetItem.GetExtendedLayoutOutput(ScrollingLayoutOutput.DataCookie) is ScrollingLayoutOutput scrollingLayoutOutput))
                scrollingLayoutOutput = new ScrollingLayoutOutput();
            _output = scrollingLayoutOutput;
            if (_output.ProcessedExplicitScrollIntoViewRequest || !_output.ScrollFocusIntoView)
                DisableScrollIntoView();
            if (_output.CanScrollNegative != output.CanScrollNegative)
                FireNotification(NotificationID.CanScrollUp);
            if (_output.CanScrollPositive != output.CanScrollPositive)
                FireNotification(NotificationID.CanScrollDown);
            if (_output.CurrentPage != (double)output.CurrentPage)
                FireNotification(NotificationID.CurrentPage);
            if (_output.TotalPages != (double)output.TotalPages)
                FireNotification(NotificationID.TotalPages);
            if (_output.ViewNear != (double)output.ViewNear)
                FireNotification(NotificationID.ViewNear);
            if (_output.ViewFar == (double)output.ViewFar)
                return;
            FireNotification(NotificationID.ViewFar);
        }

        private void OnLayoutComplete(object sender)
        {
            OnLayoutOutputChanged();
            UIClass keyFocusDescendant = _targetItem.UI.KeyFocusDescendant;
            if (keyFocusDescendant != null && keyFocusDescendant != _targetItem.UI && _targetItem.HasDescendant(keyFocusDescendant.RootItem))
                _lastFocusedItem = keyFocusDescendant.RootItem;
            ClearPendingFocusAreaOfInterest();
            if (!_useUserDisposition)
            {
                ActualScrollIntoViewDisposition.Reset();
                ActualScrollIntoViewDisposition.Enabled = false;
            }
            else
                ActualScrollIntoViewDisposition.CopyFrom(_userDisposition);
            _input.OnLayoutComplete();
            DeliverPostLayoutAction();
        }

        private void ClearPendingFocusAreaOfInterest()
        {
            if (_uiWithAreaOfInterestToClear == null)
                return;
            if (!_uiWithAreaOfInterestToClear.IsDisposed)
                _uiWithAreaOfInterestToClear.ClearAreaOfInterest(AreaOfInterestID.PendingFocus);
            _uiWithAreaOfInterestToClear = null;
        }

        private void SetPendingFocusAreaOfInterest(UIClass ui)
        {
            ClearPendingFocusAreaOfInterest();
            if (ui.DirectKeyFocus)
                return;
            ui.SetAreaOfInterest(AreaOfInterestID.PendingFocus);
            _uiWithAreaOfInterestToClear = ui;
        }

        private bool HadFocus()
        {
            if (_lastFocusedItem != null && _lastFocusedItem.IsDisposed)
                _lastFocusedItem = null;
            return _lastFocusedItem != null;
        }

        private bool NonDefaultUserDisposition() => !_userDisposition.IsDefault;

        private RectangleF GetLastFocusRect(bool forNavigation) => GetViewItemRect(_lastFocusedItem, forNavigation);

        private RectangleF GetScrollerRect(bool forNavigation) => GetViewItemRect(_targetItem, forNavigation);

        private RectangleF GetViewItemRect(ViewItem item, bool forNavigation)
        {
            if (!forNavigation)
                return RectangleF.FromRectangle(((ITrackableUIElement)item).EstimatePosition(_targetItem));
            Vector3 positionPxlVector;
            Vector3 sizePxlVector;
            ((INavigationSite)item).ComputeBounds(out positionPxlVector, out sizePxlVector);
            return new RectangleF(positionPxlVector.X, positionPxlVector.Y, sizePxlVector.X, sizePxlVector.Y);
        }

        private void NavigateToUI(UIClass ui)
        {
            ui.NotifyNavigationDestination(KeyFocusReason.Directional);
            SetPendingFocusAreaOfInterest(ui);
        }

        public void BeginCamp() => _isCamping = true;

        public void EndCamp()
        {
            _isCamping = false;
            _canDoMoveUp = true;
            _canDoMoveDown = true;
        }

        private bool CanMoveDirection(bool nearDirection) => nearDirection ? _canDoMoveUp : _canDoMoveDown;

        private void NoteFutileMoveAttempt(bool near)
        {
            if (!_isCamping)
                return;
            if (near)
                _canDoMoveUp = false;
            else
                _canDoMoveDown = false;
        }

        private enum ItemLocation
        {
            OffscreenInNearDirection,
            Onscreen,
            OffscreenInFarDirection,
        }

        private abstract class PostLayoutAction
        {
            private ScrollModel _data;
            private bool _nearDirection;

            protected PostLayoutAction(ScrollModel data) => _data = data;

            protected void Initialize(bool nearDirection) => _nearDirection = nearDirection;

            protected bool NearDirection => _nearDirection;

            protected ScrollModel Data => _data;

            protected UIClass Origin => _data._lastFocusedItem.UI;

            protected ViewItem Target => _data._targetItem;

            protected void NoteFutileMoveAttempt() => Data.NoteFutileMoveAttempt(_nearDirection);

            public abstract void Go();
        }

        private class NavigateAction : ScrollModel.PostLayoutAction
        {
            private Direction _direction;

            private NavigateAction(ScrollModel data)
              : base(data)
            {
            }

            public static ScrollModel.NavigateAction GetInstance(
              ScrollModel data,
              bool nearDirection,
              Direction direction)
            {
                if (data._navigateAction == null)
                    data._navigateAction = new ScrollModel.NavigateAction(data);
                ScrollModel.NavigateAction navigateAction = data._navigateAction;
                navigateAction.Initialize(nearDirection);
                navigateAction._direction = direction;
                return navigateAction;
            }

            public override void Go()
            {
                if (Origin == null || Origin.IsDisposed)
                    return;
                UIClass uiClass = Origin.DirectKeyFocus ? Origin : null;
                UIClass resultUI;
                if (Origin.FindNextFocusablePeer(_direction, RectangleF.Zero, out resultUI) && resultUI != null && (resultUI != uiClass && Target.HasDescendant(resultUI.RootItem)))
                {
                    Data.NavigateToUI(resultUI);
                    Data.EnableScrollIntoView();
                }
                else
                    NoteFutileMoveAttempt();
            }
        }

        private class AssignFocusAction : ScrollModel.PostLayoutAction
        {
            private bool _tryNonBiasedSearchFirst;
            private PointF _assignPoint;
            private float _lockedPosition;

            private AssignFocusAction(ScrollModel data)
              : base(data)
            {
            }

            public static ScrollModel.AssignFocusAction GetInstance(
              ScrollModel data,
              PointF restorePoint,
              bool nearEndpoint,
              bool forceToEndpoint)
            {
                if (data._assignFocusAction == null)
                    data._assignFocusAction = new ScrollModel.AssignFocusAction(data);
                ScrollModel.AssignFocusAction assignFocusAction = data._assignFocusAction;
                assignFocusAction.Initialize(nearEndpoint);
                assignFocusAction.LockToEdge(nearEndpoint);
                assignFocusAction._assignPoint = restorePoint;
                assignFocusAction._tryNonBiasedSearchFirst = !forceToEndpoint;
                return assignFocusAction;
            }

            public override void Go()
            {
                UIClass ui = null;
                if (_tryNonBiasedSearchFirst)
                    ui = FindNavigationResult(true);
                if (ui == null)
                    ui = FindNavigationResult(false);
                if (ui != null)
                    Data.NavigateToUI(ui);
                else
                    NoteFutileMoveAttempt();
                ApplyLockedPosition();
            }

            private UIClass FindNavigationResult(bool findNearest)
            {
                UIClass uiClass = null;
                INavigationSite result;
                bool fromPoint;
                if (findNearest)
                {
                    fromPoint = NavigationServices.FindFromPoint(Target, _assignPoint, out result);
                }
                else
                {
                    Direction direction = Data.NearFarToDirection(NearDirection);
                    _assignPoint = Data.GetMoveToEndpointPoint(NearDirection);
                    fromPoint = NavigationServices.FindFromPoint(Target, direction, _assignPoint, out result);
                }
                if (fromPoint && result != null)
                {
                    ViewItem viewItem = result as ViewItem;
                    if (viewItem.UI != Origin || !Origin.DirectKeyFocus)
                        uiClass = viewItem.UI;
                }
                return uiClass;
            }

            private void LockToEdge(bool near)
            {
                if (near)
                    _lockedPosition = 0.0f;
                else
                    _lockedPosition = 1f;
            }

            protected void ApplyLockedPosition()
            {
                if (Data.NonDefaultUserDisposition())
                    Data._input.SecondaryScrollIntoViewDisposition = Data.ScrollIntoViewDisposition;
                Data.ActualScrollIntoViewDisposition.LockedPosition = _lockedPosition;
                Data.ActualScrollIntoViewDisposition.LockedAlignment = _lockedPosition;
                Data.ActualScrollIntoViewDisposition.Locked = true;
                Data.ActualScrollIntoViewDisposition.ContentPositioningBehavior = ContentPositioningPolicy.ShowMaximalContent;
                Data.ActualScrollIntoViewDisposition.Enabled = true;
                Data._useUserDisposition = true;
                Data.OnLayoutInputChanged();
            }
        }
    }
}
