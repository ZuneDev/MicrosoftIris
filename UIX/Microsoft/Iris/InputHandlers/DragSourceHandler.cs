// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.InputHandlers.DragSourceHandler
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Input;
using Microsoft.Iris.Markup;
using Microsoft.Iris.OS;
using Microsoft.Iris.Session;
using Microsoft.Iris.UI;
using System;

namespace Microsoft.Iris.InputHandlers
{
    internal class DragSourceHandler : InputHandler
    {
        private object _value;
        private DropAction _allowedDropActions;
        private DropAction _currentDropAction;
        private bool _pendingDrag;
        private bool _dragCanceled;
        private int _initialX;
        private int _initialY;
        private CursorID _moveCursor;
        private CursorID _copyCursor;
        private CursorID _cancelCursor;

        internal DragSourceHandler()
        {
            _moveCursor = CursorID.Move;
            _copyCursor = CursorID.Copy;
            _cancelCursor = CursorID.Cancel;
        }

        public override void OnZoneDetached()
        {
            if (Dragging)
                EndDrag(null, InputModifiers.None, DropAction.None);
            base.OnZoneDetached();
        }

        protected override void ConfigureInteractivity()
        {
            base.ConfigureInteractivity();
            if (!HandleDirect)
                return;
            UI.MouseInteractive = true;
        }

        public object Value
        {
            get => _value;
            set
            {
                if (_value == value)
                    return;
                _value = value;
                FireNotification(NotificationID.Value);
            }
        }

        public DropAction AllowedDropActions
        {
            get => _allowedDropActions;
            set
            {
                if (_allowedDropActions == value)
                    return;
                _allowedDropActions = value;
                FireNotification(NotificationID.AllowedDropActions);
            }
        }

        public DropAction CurrentDropAction => _currentDropAction;

        private void SetCurrentDropAction(DropAction value)
        {
            if (_currentDropAction == value)
                return;
            _currentDropAction = value;
            FireNotification(NotificationID.CurrentDropAction);
            UpdateCursor();
        }

        public bool Dragging => DragDropHelper.DraggingInternally && DragDropHelper.SourceHandler == this;

        public CursorID MoveCursor
        {
            get => _moveCursor;
            set
            {
                if (_moveCursor == value)
                    return;
                _moveCursor = value;
                FireNotification(NotificationID.MoveCursor);
            }
        }

        public CursorID CopyCursor
        {
            get => _copyCursor;
            set
            {
                if (_copyCursor == value)
                    return;
                _copyCursor = value;
                FireNotification(NotificationID.CopyCursor);
            }
        }

        public CursorID CancelCursor
        {
            get => _cancelCursor;
            set
            {
                if (_cancelCursor == value)
                    return;
                _cancelCursor = value;
                FireNotification(NotificationID.CancelCursor);
            }
        }

        protected override void OnMousePrimaryDown(UIClass ui, MouseButtonInfo info)
        {
            _pendingDrag = true;
            _initialX = info.ScreenX;
            _initialY = info.ScreenY;
            base.OnMousePrimaryDown(ui, info);
        }

        protected override void OnMouseMove(UIClass ui, MouseMoveInfo info)
        {
            if (Dragging)
            {
                DragDropHelper.Requery(info.NaturalHit, info.ScreenX, info.ScreenY, info.Modifiers);
                info.MarkHandled();
            }
            else if (_pendingDrag)
            {
                if (Math.Abs(info.ScreenX - _initialX) >= Win32Api.GetSystemMetrics(68) || Math.Abs(info.ScreenY - _initialY) >= Win32Api.GetSystemMetrics(69))
                {
                    _pendingDrag = false;
                    DragDropHelper.BeginDrag(this, info.Target, info.NaturalHit, 0, 0, info.Modifiers);
                    UI.SessionInput += new SessionInputHandler(OnSessionInput);
                    FireNotification(NotificationID.Dragging);
                    FireNotification(NotificationID.Started);
                    UpdateCursor();
                    info.MarkHandled();
                }
            }
            else if (_dragCanceled)
                info.MarkHandled();
            base.OnMouseMove(ui, info);
        }

        protected override void OnMousePrimaryUp(UIClass sender, MouseButtonInfo info)
        {
            _pendingDrag = false;
            if (Dragging)
            {
                EndDrag(info?.NaturalHit, info.Modifiers, CurrentDropAction);
                info.MarkHandled();
            }
            else if (_dragCanceled)
            {
                _dragCanceled = false;
                info.MarkHandled();
            }
            base.OnMousePrimaryUp(sender, info);
        }

        protected override void OnDragComplete(UIClass sender, DragDropInfo info)
        {
            DragDropHelper.OnDragComplete();
            info.MarkHandled();
            base.OnDragComplete(sender, info);
        }

        private void OnSessionInput(InputInfo originalEvent, EventRouteStages handledStage)
        {
            switch (originalEvent)
            {
                case KeyStateInfo keyStateInfo:
                    switch (keyStateInfo.Key)
                    {
                        case Keys.ControlKey:
                            InputModifiers modifiers = keyStateInfo.Modifiers;
                            if (keyStateInfo.Action == KeyAction.Down)
                                modifiers |= InputModifiers.ControlKey;
                            DragDropHelper.Requery(modifiers);
                            break;
                        case Keys.Escape:
                            EndDrag(null, keyStateInfo.Modifiers, DropAction.None);
                            _dragCanceled = true;
                            break;
                    }
                    keyStateInfo.MarkHandled();
                    break;
                case MouseButtonInfo mouseButtonInfo:
                    if (mouseButtonInfo.Button == MouseButtons.Left)
                        break;
                    mouseButtonInfo.MarkHandled();
                    break;
            }
        }

        protected override void OnLoseKeyFocus(UIClass sender, KeyFocusInfo info)
        {
            _pendingDrag = false;
            if (Dragging)
                EndDrag(null, DragDropHelper.Modifiers, DropAction.None);
            base.OnLoseKeyFocus(sender, info);
        }

        protected override void OnLoseMouseFocus(UIClass sender, MouseFocusInfo info)
        {
            _pendingDrag = false;
            if (Dragging)
                EndDrag(null, DragDropHelper.Modifiers, DropAction.None);
            base.OnLoseMouseFocus(sender, info);
        }

        private void EndDrag(IRawInputSite target, InputModifiers modifiers, DropAction action)
        {
            UI.SessionInput -= new SessionInputHandler(OnSessionInput);
            DragDropHelper.EndDrag(target, modifiers, action);
        }

        internal void OnEndDrag(DropAction action)
        {
            FireNotification(NotificationID.Dragging);
            switch (action)
            {
                case DropAction.None:
                    FireNotification(NotificationID.Canceled);
                    break;
                case DropAction.Copy:
                    FireNotification(NotificationID.Copied);
                    break;
                case DropAction.Move:
                    FireNotification(NotificationID.Moved);
                    break;
            }
            UpdateCursor();
        }

        internal void UpdateCurrentAction()
        {
            DropAction dropAction = DragDropHelper.AllowedDropActions & AllowedDropActions;
            if ((dropAction & DropAction.All) == DropAction.All)
            {
                if ((DragDropHelper.Modifiers & InputModifiers.ControlKey) == InputModifiers.ControlKey)
                    SetCurrentDropAction(DropAction.Copy);
                else
                    SetCurrentDropAction(DropAction.Move);
            }
            else
                SetCurrentDropAction(dropAction);
        }

        internal override CursorID GetCursor()
        {
            if (Dragging)
            {
                switch (CurrentDropAction)
                {
                    case DropAction.None:
                        return CancelCursor;
                    case DropAction.Copy:
                        return CopyCursor;
                    case DropAction.Move:
                        return MoveCursor;
                }
            }
            return CursorID.NotSpecified;
        }
    }
}
