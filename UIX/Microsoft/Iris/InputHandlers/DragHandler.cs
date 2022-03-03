// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.InputHandlers.DragHandler
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Input;
using Microsoft.Iris.Layout;
using Microsoft.Iris.Markup;
using Microsoft.Iris.OS;
using Microsoft.Iris.Render;
using Microsoft.Iris.RenderAPI.Drawing;
using Microsoft.Iris.Session;
using Microsoft.Iris.UI;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Iris.InputHandlers
{
    internal class DragHandler : InputHandler
    {
        private CursorID _dragCursor;
        private BeginDragPolicy _beginDragPolicy;
        private bool _pendingDrag;
        private bool _inDrag;
        private bool _hasRelativeTo;
        private Vector2 _initialPosition;
        private Point _initialScreenPosition;
        private Vector2 _beginPosition;
        private Vector2 _endPosition;
        private Point _screenBeginPosition;
        private Point _screenEndPosition;
        private Size _lastKnownSize;
        private InputHandlerModifiers _activeModifiers;
        private bool _cancelOnEscape;
        private ViewItem _relativeTo;
        private RectangleF _contextBounds;
        private List<object> _addedContexts;
        private List<object> _removedContexts;

        public DragHandler() => _beginDragPolicy = BeginDragPolicy.Down;

        public override void OnZoneDetached()
        {
            CancelDrag();
            base.OnZoneDetached();
        }

        protected override void ConfigureInteractivity()
        {
            base.ConfigureInteractivity();
            if (HandleDirect)
                UI.MouseInteractive = true;
            if (_relativeTo != null)
                return;
            SetRelativeTo(null);
        }

        public BeginDragPolicy BeginDragPolicy
        {
            get => _beginDragPolicy;
            set
            {
                if (_beginDragPolicy == value)
                    return;
                _beginDragPolicy = value;
                FireNotification(NotificationID.BeginDragPolicy);
            }
        }

        public bool Dragging => _inDrag;

        private void SetDragging(bool value)
        {
            if (_inDrag == value)
                return;
            _inDrag = value;
            FireNotification(NotificationID.Dragging);
        }

        public Vector2 BeginPosition => NormalizeCoordinates(_beginPosition);

        private void SetBeginPosition(Vector2 relativeCoordinate)
        {
            if (!(_beginPosition != relativeCoordinate))
                return;
            _beginPosition = relativeCoordinate;
            FireNotification(NotificationID.BeginPosition);
            FireNotification(NotificationID.RelativeDragSize);
        }

        public Vector2 EndPosition => NormalizeCoordinates(_endPosition);

        private void SetEndPosition(Vector2 relativeCoordinate)
        {
            if (!(_endPosition != relativeCoordinate))
                return;
            _endPosition = relativeCoordinate;
            FireNotification(NotificationID.EndPosition);
            FireNotification(NotificationID.RelativeDragSize);
        }

        public Size ScreenDragSize => (ScreenEndPosition - ScreenBeginPosition).ToSize();

        public Vector2 LocalDragSize
        {
            get
            {
                Size screenDragSize = ScreenDragSize;
                Vector3 vector3 = _relativeTo != null ? _relativeTo.ComputeEffectiveScale() : Vector3.UnitVector;
                return new Vector2(screenDragSize.Width / vector3.X, screenDragSize.Height / vector3.Y);
            }
        }

        public Vector2 RelativeDragSize => EndPosition - BeginPosition;

        public InputHandlerModifiers ActiveModifiers => _activeModifiers;

        private void SetActiveModifiers(InputHandlerModifiers value)
        {
            if (_activeModifiers == value)
                return;
            _activeModifiers = value;
            FireNotification(NotificationID.ActiveModifiers);
        }

        public CursorID DragCursor
        {
            get => _dragCursor;
            set
            {
                if (_dragCursor == value)
                    return;
                _dragCursor = value;
                FireNotification(NotificationID.DragCursor);
            }
        }

        private Point ScreenBeginPosition
        {
            get => _screenBeginPosition;
            set
            {
                if (!(_screenBeginPosition != value))
                    return;
                _screenBeginPosition = value;
                FireNotification(NotificationID.ScreenDragSize);
                FireNotification(NotificationID.LocalDragSize);
            }
        }

        private Point ScreenEndPosition
        {
            get => _screenEndPosition;
            set
            {
                if (!(_screenEndPosition != value))
                    return;
                _screenEndPosition = value;
                FireNotification(NotificationID.ScreenDragSize);
                FireNotification(NotificationID.LocalDragSize);
            }
        }

        public bool CancelOnEscape
        {
            get => _cancelOnEscape;
            set
            {
                if (_cancelOnEscape == value)
                    return;
                if (Dragging)
                    HookSessionInput(value);
                _cancelOnEscape = value;
                FireNotification(NotificationID.CancelOnEscape);
            }
        }

        public ViewItem RelativeTo
        {
            get => !_hasRelativeTo ? null : _relativeTo;
            set
            {
                bool hasRelativeTo = _hasRelativeTo;
                _hasRelativeTo = value != null;
                ViewItem relativeTo = _relativeTo;
                SetRelativeTo(value);
                if (hasRelativeTo == _hasRelativeTo && relativeTo == _relativeTo)
                    return;
                FireNotification(NotificationID.RelativeTo);
            }
        }

        private void SetRelativeTo(ViewItem relativeTo)
        {
            if (_relativeTo == relativeTo && relativeTo != null)
                return;
            LayoutCompleteEventHandler completeEventHandler = new LayoutCompleteEventHandler(OnRelativeToLayoutComplete);
            if (_relativeTo != null)
                _relativeTo.LayoutComplete -= completeEventHandler;
            _relativeTo = relativeTo;
            if (_relativeTo == null && UI != null)
                _relativeTo = UI.RootItem;
            if (_relativeTo == null)
                return;
            _relativeTo.LayoutComplete += completeEventHandler;
        }

        public void ResetDragOrigin()
        {
            SetBeginPosition(_endPosition);
            ScreenBeginPosition = ScreenEndPosition;
        }

        private void BeginDrag(
          Vector2 relativeBegin,
          Vector2 relativeEnd,
          Point screenBegin,
          Point screenEnd,
          InputHandlerModifiers modifiers)
        {
            SetDragging(true);
            _pendingDrag = false;
            _contextBounds = RectangleF.Zero;
            if (CancelOnEscape)
                HookSessionInput(true);
            SetBeginPosition(relativeBegin);
            SetEndPosition(relativeEnd);
            ScreenBeginPosition = screenBegin;
            ScreenEndPosition = screenEnd;
            SetActiveModifiers(modifiers);
            FireNotification(NotificationID.Started);
            UpdateCursor();
        }

        private void EndDrag(bool completed)
        {
            if (Dragging)
            {
                if (CancelOnEscape)
                    HookSessionInput(false);
                SetDragging(false);
                if (completed)
                    FireNotification(NotificationID.Ended);
                else
                    FireNotification(NotificationID.Canceled);
                UpdateCursor();
            }
            _pendingDrag = false;
        }

        private void InDrag(Vector2 uiPoint, Point screenPoint, InputHandlerModifiers modifiers)
        {
            if (screenPoint != ScreenEndPosition)
            {
                SetEndPosition(TransformToRelative(uiPoint));
                ScreenEndPosition = screenPoint;
            }
            SetActiveModifiers(modifiers);
        }

        protected override void OnMousePrimaryDown(UIClass ui, MouseButtonInfo info)
        {
            if (!Dragging)
            {
                Vector2 relative = TransformToRelative(TransformToUI(new Point(info.X, info.Y), (UIClass)info.Target));
                Point point = new Point(info.ScreenX, info.ScreenY);
                if (BeginDragPolicy == BeginDragPolicy.Down)
                {
                    BeginDrag(relative, relative, point, point, GetModifiers(info.Modifiers));
                    info.MarkHandled();
                }
                else
                {
                    _pendingDrag = true;
                    _initialPosition = relative;
                    _initialScreenPosition = point;
                }
            }
            base.OnMousePrimaryDown(ui, info);
        }

        protected override void OnMousePrimaryUp(UIClass ui, MouseButtonInfo info)
        {
            bool dragging = Dragging;
            if (dragging || _pendingDrag)
            {
                EndDrag(true);
                if (dragging)
                    info.MarkHandled();
            }
            base.OnMousePrimaryUp(ui, info);
        }

        protected override void OnMouseMove(UIClass ui, MouseMoveInfo info)
        {
            if ((info.Modifiers & InputModifiers.LeftMouse) == InputModifiers.None)
                EndDrag(true);
            else if (Dragging || _pendingDrag)
            {
                Vector2 ui1 = TransformToUI(new Point(info.X, info.Y), (UIClass)info.Target);
                Point point = new Point(info.ScreenX, info.ScreenY);
                if (Dragging)
                {
                    InDrag(ui1, point, GetModifiers(info.Modifiers));
                    info.MarkHandled();
                }
                else if (Math.Abs(point.X - _initialScreenPosition.X) >= Win32Api.GetSystemMetrics(68) || Math.Abs(point.Y - _initialScreenPosition.Y) >= Win32Api.GetSystemMetrics(69))
                {
                    BeginDrag(_initialPosition, TransformToRelative(ui1), _initialScreenPosition, point, GetModifiers(info.Modifiers));
                    info.MarkHandled();
                }
            }
            base.OnMouseMove(ui, info);
        }

        protected override void OnLoseMouseFocus(UIClass ui, MouseFocusInfo info)
        {
            CancelDrag();
            base.OnLoseMouseFocus(ui, info);
        }

        private void OnRelativeToLayoutComplete(object sender) => SetLastLayoutSize(_relativeTo.LayoutSize);

        protected void SetLastLayoutSize(Size size)
        {
            Vector2 beginPosition = BeginPosition;
            Vector2 endPosition = EndPosition;
            Vector2 relativeDragSize = RelativeDragSize;
            _lastKnownSize = size;
            if (!Dragging)
                return;
            Point client = _relativeTo.ScreenToClient(_screenEndPosition);
            SetEndPosition(new Vector2(client.X, client.Y));
            if (beginPosition != BeginPosition)
                FireNotification(NotificationID.BeginPosition);
            if (endPosition != EndPosition)
                FireNotification(NotificationID.EndPosition);
            if (!(relativeDragSize != RelativeDragSize))
                return;
            FireNotification(NotificationID.RelativeDragSize);
        }

        private void HookSessionInput(bool hook)
        {
            if (hook)
                UI.SessionInput += new SessionInputHandler(OnSessionInput);
            else
                UI.SessionInput -= new SessionInputHandler(OnSessionInput);
        }

        private void OnSessionInput(InputInfo originalEvent, EventRouteStages stageHandled)
        {
            if (!(originalEvent is KeyStateInfo keyStateInfo))
                return;
            if (keyStateInfo.Key == Keys.Escape)
                CancelDrag();
            keyStateInfo.MarkHandled();
        }

        private Vector2 TransformToRelative(Vector2 uiPoint)
        {
            if (_relativeTo != null)
            {
                RectangleF rectangleF = _relativeTo.TransformFromAncestor(UI.RootItem, new RectangleF(uiPoint.X, uiPoint.Y, 0.0f, 0.0f));
                uiPoint.X = rectangleF.X;
                uiPoint.Y = rectangleF.Y;
            }
            return uiPoint;
        }

        private RectangleF TransformFromRelative(RectangleF relativeBounds)
        {
            RectangleF rectangleF = relativeBounds;
            if (_relativeTo != null)
                rectangleF = _relativeTo.TransformToAncestor(UI.RootItem, relativeBounds);
            return rectangleF;
        }

        private Vector2 TransformToUI(Point uiPoint, UIClass reference)
        {
            float x = uiPoint.X;
            float y = uiPoint.Y;
            if (reference != UI)
            {
                RectangleF rect = new RectangleF(x, y, 0.0f, 0.0f);
                RectangleF ancestor = reference.RootItem.TransformToAncestor(UI.RootItem, rect);
                x = ancestor.X;
                y = ancestor.Y;
            }
            return new Vector2(x, y);
        }

        private Vector2 NormalizeCoordinates(Vector2 pt) => _lastKnownSize.Width == 0 || _lastKnownSize.Height == 0 ? Vector2.Zero : new Vector2(pt.X / _lastKnownSize.Width, pt.Y / _lastKnownSize.Height);

        internal override CursorID GetCursor() => _dragCursor != CursorID.NotSpecified && Dragging ? _dragCursor : CursorID.NotSpecified;

        public void CancelDrag() => EndDrag(false);

        private void GetDragBounds(out RectangleF relativeBounds, out RectangleF uiBounds)
        {
            relativeBounds = new RectangleF(Math.Min(_beginPosition.X, _endPosition.X), Math.Min(_beginPosition.Y, _endPosition.Y), Math.Abs(_beginPosition.X - _endPosition.X), Math.Abs(_beginPosition.Y - _endPosition.Y));
            uiBounds = TransformFromRelative(relativeBounds);
        }

        public IList GetEventContexts()
        {
            IList added = new List<object>();
            RectangleF uiBounds;
            GetDragBounds(out RectangleF _, out uiBounds);
            GetEventContexts(UI, added, null, RectangleF.Zero, uiBounds);
            return added;
        }

        public IList GetAddedEventContexts()
        {
            UpdateEventContexts();
            return _addedContexts;
        }

        public IList GetRemovedEventContexts()
        {
            UpdateEventContexts();
            return _removedContexts;
        }

        private void UpdateEventContexts()
        {
            RectangleF relativeBounds;
            RectangleF uiBounds;
            GetDragBounds(out relativeBounds, out uiBounds);
            if (!(relativeBounds != _contextBounds))
                return;
            _addedContexts = new List<object>();
            _removedContexts = new List<object>();
            GetEventContexts(UI, _addedContexts, _removedContexts, TransformFromRelative(_contextBounds), uiBounds);
            _contextBounds = relativeBounds;
        }

        private static void GetEventContexts(
          UIClass node,
          IList added,
          IList removed,
          RectangleF oldBounds,
          RectangleF newBounds)
        {
            ViewItem rootItem = node.RootItem;
            if (rootItem == null)
                return;
            object eventContext = node.GetEventContext();
            if (eventContext != null && rootItem.Parent != null)
            {
                RectangleF rectangleF = new RectangleF(Point.Zero, rootItem.Parent.LayoutSize);
                bool flag1 = rectangleF.IntersectsWith(oldBounds);
                bool flag2 = rectangleF.IntersectsWith(newBounds);
                if (flag2 && !flag1)
                    added.Add(eventContext);
                else if (removed != null && flag1 && !flag2)
                    removed.Add(eventContext);
            }
            for (UIClass node1 = (UIClass)node.FirstChild; node1 != null; node1 = (UIClass)node1.NextSibling)
            {
                if (node1.RootItem != null)
                {
                    RectangleF oldBounds1 = node1.RootItem.Parent.TransformFromAncestor(rootItem.Parent, oldBounds);
                    RectangleF newBounds1 = node1.RootItem.Parent.TransformFromAncestor(rootItem.Parent, newBounds);
                    GetEventContexts(node1, added, removed, oldBounds1, newBounds1);
                }
            }
        }
    }
}
