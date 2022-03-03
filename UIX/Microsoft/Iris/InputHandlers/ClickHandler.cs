// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.InputHandlers.ClickHandler
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Input;
using Microsoft.Iris.Markup;
using Microsoft.Iris.ModelItems;
using Microsoft.Iris.OS;
using Microsoft.Iris.Session;
using Microsoft.Iris.UI;
using System;

namespace Microsoft.Iris.InputHandlers
{
    internal class ClickHandler : ModifierInputHandler
    {
        private static int s_defaultRepeatDelay = Win32Api.GetDefaultKeyDelay();
        private static int s_defaultRepeatRate = Win32Api.GetDefaultKeyRepeat();
        private ClickCount _clickCount;
        private ClickType _clickType;
        private int _repeatDelay;
        private int _repeatRate;
        private IUICommand _command;
        private WeakReference _eventContext;
        private ClickType _clickTypeInProgress;
        private DispatcherTimer _repeatTimer;
        private bool _clickValidPosition;
        private bool _handleEscape;
        private bool _handle;
        private bool _repeat;

        public ClickHandler()
        {
            _clickType = ClickType.Key | ClickType.GamePad | ClickType.LeftMouse;
            _clickCount = ClickCount.Single;
            _repeat = true;
            _repeatDelay = DefaultRepeatDelay;
            _repeatRate = DefaultRepeatRate;
            _handle = true;
        }

        protected override void OnDispose()
        {
            if (_repeatTimer != null)
                _repeatTimer.Enabled = false;
            base.OnDispose();
        }

        protected override void ConfigureInteractivity()
        {
            base.ConfigureInteractivity();
            if (!HandleDirect)
                return;
            if (ShouldHandleEvent(ClickType.Mouse))
                UI.MouseInteractive = true;
            if (!ShouldHandleEvent(ClickType.Key | ClickType.GamePad))
                return;
            UI.KeyInteractive = true;
        }

        public ClickType ClickType
        {
            get => _clickType;
            set
            {
                if (_clickType == value)
                    return;
                _clickType = value;
                FireNotification(NotificationID.ClickType);
            }
        }

        public ClickCount ClickCount
        {
            get => _clickCount;
            set
            {
                if (_clickCount == value)
                    return;
                _clickCount = value;
                FireNotification(NotificationID.ClickCount);
            }
        }

        public bool Repeat
        {
            get => _repeat;
            set
            {
                if (_repeat == value)
                    return;
                _repeat = value;
                FireNotification(NotificationID.Repeat);
            }
        }

        public int RepeatDelay
        {
            get => _repeatDelay;
            set
            {
                if (_repeatDelay == value)
                    return;
                _repeatDelay = value;
                FireNotification(NotificationID.RepeatDelay);
            }
        }

        public int RepeatRate
        {
            get => _repeatRate;
            set
            {
                if (_repeatRate == value)
                    return;
                _repeatRate = value;
                FireNotification(NotificationID.RepeatRate);
            }
        }

        public bool Clicking => _clickTypeInProgress != ClickType.None && _clickValidPosition;

        public object EventContext => CheckEventContext(ref _eventContext);

        public IUICommand Command
        {
            get => _command;
            set
            {
                if (_command == value)
                    return;
                _command = value;
                FireNotification(NotificationID.Command);
            }
        }

        public bool Handle
        {
            get => _handle;
            set
            {
                if (_handle == value)
                    return;
                _handle = value;
                FireNotification(NotificationID.Handle);
            }
        }

        private void InvokeCommand()
        {
            FireNotification(NotificationID.Invoked);
            if (_command == null)
                return;
            _command.Invoke();
        }

        private void BeginClick(ClickType clickType, bool validPosition)
        {
            if (_clickTypeInProgress == ClickType.None)
            {
                _clickTypeInProgress = clickType;
                _clickValidPosition = validPosition;
                if (validPosition)
                    FireNotification(NotificationID.Clicking);
            }
            else if (_clickTypeInProgress != clickType)
                CancelClick(ClickType.Any);
            SetEventContext(null, ref _eventContext, NotificationID.EventContext);
        }

        private void EndClick(ICookedInputSite clickTarget, ClickType clickType)
        {
            if (_clickTypeInProgress == ClickType.None)
                return;
            SetEventContext(clickTarget, ref _eventContext, NotificationID.EventContext);
            if (_clickTypeInProgress == clickType && _clickValidPosition)
                InvokeCommand();
            CancelClick(ClickType.Any);
        }

        private void UpdateClickValidPosition(bool validPosition)
        {
            bool clicking = Clicking;
            _clickValidPosition = validPosition;
            if (Clicking == clicking)
                return;
            FireNotification(NotificationID.Clicking);
        }

        private void CancelClick(ClickType clickType)
        {
            if (_clickTypeInProgress != ClickType.None && Library.Bits.TestAllFlags((uint)clickType, (uint)_clickTypeInProgress))
            {
                bool clicking = Clicking;
                _clickTypeInProgress = ClickType.None;
                if (Clicking != clicking)
                    FireNotification(NotificationID.Clicking);
            }
            if (_repeatTimer == null)
                return;
            _repeatTimer.Enabled = false;
        }

        private bool ShouldHandleEvent(
          ClickType type,
          InputHandlerModifiers modifiers,
          ClickCount count)
        {
            return ShouldHandleEvent(type) && _clickCount == count && ShouldHandleEvent(modifiers);
        }

        private bool ShouldHandleEvent(ClickType type) => Library.Bits.TestAnyFlags((uint)_clickType, (uint)type);

        private bool OnClickEvent(
          ICookedInputSite clickTarget,
          ClickType type,
          InputHandlerTransition transition,
          InputHandlerModifiers modifiers)
        {
            return OnClickEvent(clickTarget, type, transition, modifiers, ClickCount.Single);
        }

        private bool OnClickEvent(
          ICookedInputSite clickTarget,
          ClickType type,
          InputHandlerTransition transition,
          InputHandlerModifiers modifiers,
          ClickCount count)
        {
            if (!ShouldHandleEvent(type, modifiers, count))
                return false;
            if (transition == InputHandlerTransition.Up)
            {
                if (HandlerTransition != InputHandlerTransition.Down)
                    EndClick(clickTarget, type);
                if (_repeatTimer != null)
                    _repeatTimer.Enabled = false;
            }
            else
            {
                BeginClick(type, true);
                if (HandlerTransition == InputHandlerTransition.Down)
                {
                    EndClick(clickTarget, type);
                    StartRepeat(new ClickHandler.ClickInfo()
                    {
                        type = type,
                        transition = transition,
                        modifiers = modifiers,
                        count = count,
                        target = clickTarget
                    });
                }
            }
            return true;
        }

        private ClickType GetClickType(MouseButtons buttons)
        {
            ClickType clickType = ClickType.None;
            if ((buttons & MouseButtons.Left) != MouseButtons.None)
                clickType |= ClickType.LeftMouse;
            if ((buttons & MouseButtons.Right) != MouseButtons.None)
                clickType |= ClickType.RightMouse;
            if ((buttons & MouseButtons.Middle) != MouseButtons.None)
                clickType |= ClickType.MiddleMouse;
            return clickType;
        }

        private void StartRepeat(ClickHandler.ClickInfo clickInfo)
        {
            if (!Repeat)
                return;
            if (_repeatTimer == null)
            {
                _repeatTimer = new DispatcherTimer();
                _repeatTimer.Tick += new EventHandler(SimulateClickEvent);
                _repeatTimer.AutoRepeat = true;
            }
            _repeatTimer.Interval = RepeatDelay;
            _repeatTimer.UserData = clickInfo;
            _repeatTimer.Enabled = true;
        }

        private void SimulateClickEvent(object sender, EventArgs args)
        {
            bool flag = false;
            ClickHandler.ClickInfo userData = (ClickHandler.ClickInfo)_repeatTimer.UserData;
            if (Enabled && Repeat && _clickValidPosition && (userData.target == null || userData.target.IsValid))
            {
                OnClickEvent(userData.target, userData.type, userData.transition, userData.modifiers, userData.count);
                flag = true;
            }
            if (flag)
            {
                _repeatTimer.Enabled = false;
                _repeatTimer = null;
                StartRepeat(userData);
                _repeatTimer.Interval = RepeatRate;
            }
            else
            {
                _repeatTimer.Enabled = false;
                _repeatTimer = null;
            }
        }

        private static int DefaultRepeatDelay => s_defaultRepeatDelay;

        private static int DefaultRepeatRate => s_defaultRepeatRate;

        protected override void OnMousePrimaryDown(UIClass ui, MouseButtonInfo info)
        {
            if (!OnClickEvent(info.Target, GetClickType(info.Button), InputHandlerTransition.Down, GetModifiers(info.Modifiers)) || !_handle)
                return;
            info.MarkHandled();
        }

        protected override void OnMouseSecondaryDown(UIClass ui, MouseButtonInfo info)
        {
            if (!OnClickEvent(info.Target, GetClickType(info.Button), InputHandlerTransition.Down, GetModifiers(info.Modifiers)) || !_handle)
                return;
            info.MarkHandled();
        }

        protected override void OnMousePrimaryUp(UIClass ui, MouseButtonInfo info)
        {
            if (!OnClickEvent(info.Target, GetClickType(info.Button), InputHandlerTransition.Up, GetModifiers(info.Modifiers)) || !_handle)
                return;
            info.MarkHandled();
        }

        protected override void OnMouseSecondaryUp(UIClass ui, MouseButtonInfo info)
        {
            if (!OnClickEvent(info.Target, GetClickType(info.Button), InputHandlerTransition.Up, GetModifiers(info.Modifiers)) || !_handle)
                return;
            info.MarkHandled();
        }

        protected override void OnMouseDoubleClick(UIClass ui, MouseButtonInfo info)
        {
            ClickType clickType = GetClickType(info.Button);
            if (!OnClickEvent(info.Target, clickType, InputHandlerTransition.Down, GetModifiers(info.Modifiers), ClickCount.Double))
                return;
            OnClickEvent(info.Target, clickType, InputHandlerTransition.Up, GetModifiers(info.Modifiers), ClickCount.Double);
            if (!_handle)
                return;
            info.MarkHandled();
        }

        protected override void OnMouseMove(UIClass ui, MouseMoveInfo info)
        {
            if (!ShouldHandleEvent(ClickType.Mouse))
                return;
            UpdateClickValidPosition(UI.HasDescendant(info.NaturalTarget as UIClass));
        }

        protected override void OnLoseMouseFocus(UIClass ui, MouseFocusInfo info)
        {
            if (!ShouldHandleEvent(ClickType.Mouse))
                return;
            CancelClick(ClickType.Mouse);
        }

        protected override void OnKeyDown(UIClass ui, KeyStateInfo info)
        {
            if (info.RepeatCount > 1U)
                return;
            switch (info.Key)
            {
                case Keys.Enter:
                    if (!OnClickEvent(info.Target, ClickType.EnterKey, InputHandlerTransition.Down, GetModifiers(info.Modifiers)) || !_handle)
                        break;
                    info.MarkHandled();
                    break;
                case Keys.Escape:
                    if (!Clicking)
                        break;
                    CancelClick(ClickType.Any);
                    if (_handle)
                        info.MarkHandled();
                    _handleEscape = true;
                    break;
                case Keys.Space:
                    if (!OnClickEvent(info.Target, ClickType.SpaceKey, InputHandlerTransition.Down, GetModifiers(info.Modifiers)) || !_handle)
                        break;
                    info.MarkHandled();
                    break;
                case Keys.GamePadA:
                    if (!OnClickEvent(info.Target, ClickType.GamePadA, InputHandlerTransition.Down, GetModifiers(info.Modifiers)) || !_handle)
                        break;
                    info.MarkHandled();
                    break;
                case Keys.GamePadStart:
                    if (!OnClickEvent(info.Target, ClickType.GamePadStart, InputHandlerTransition.Down, GetModifiers(info.Modifiers)) || !_handle)
                        break;
                    info.MarkHandled();
                    break;
            }
        }

        protected override void OnKeyUp(UIClass ui, KeyStateInfo info)
        {
            switch (info.Key)
            {
                case Keys.Enter:
                    if (!OnClickEvent(info.Target, ClickType.EnterKey, InputHandlerTransition.Up, GetModifiers(info.Modifiers)) || !_handle)
                        break;
                    info.MarkHandled();
                    break;
                case Keys.Escape:
                    if (!_handleEscape)
                        break;
                    if (_handle)
                        info.MarkHandled();
                    _handleEscape = false;
                    break;
                case Keys.Space:
                    if (!OnClickEvent(info.Target, ClickType.SpaceKey, InputHandlerTransition.Up, GetModifiers(info.Modifiers)) || !_handle)
                        break;
                    info.MarkHandled();
                    break;
                case Keys.GamePadA:
                    if (!OnClickEvent(info.Target, ClickType.GamePadA, InputHandlerTransition.Up, GetModifiers(info.Modifiers)) || !_handle)
                        break;
                    info.MarkHandled();
                    break;
                case Keys.GamePadStart:
                    if (!OnClickEvent(info.Target, ClickType.GamePadStart, InputHandlerTransition.Up, GetModifiers(info.Modifiers)) || !_handle)
                        break;
                    info.MarkHandled();
                    break;
            }
        }

        protected override void OnKeyCharacter(UIClass ui, KeyCharacterInfo info)
        {
            switch (info.Character)
            {
                case '\r':
                    if (!ShouldHandleEvent(ClickType.EnterKey, GetModifiers(info.Modifiers), ClickCount.Single) || !_handle)
                        break;
                    info.MarkHandled();
                    break;
                case '\x001B':
                    if (!_handleEscape || !_handle)
                        break;
                    info.MarkHandled();
                    break;
                case ' ':
                    if (!ShouldHandleEvent(ClickType.SpaceKey, GetModifiers(info.Modifiers), ClickCount.Single) || !_handle)
                        break;
                    info.MarkHandled();
                    break;
            }
        }

        protected override void OnLoseKeyFocus(UIClass ui, KeyFocusInfo info)
        {
            if (ShouldHandleEvent(ClickType.SpaceKey))
                CancelClick(ClickType.SpaceKey);
            if (ShouldHandleEvent(ClickType.EnterKey))
                CancelClick(ClickType.EnterKey);
            if (ShouldHandleEvent(ClickType.GamePadA))
                CancelClick(ClickType.GamePadA);
            if (ShouldHandleEvent(ClickType.GamePadStart))
                CancelClick(ClickType.GamePadStart);
            _handleEscape = false;
        }

        private struct ClickInfo
        {
            public ClickType type;
            public InputHandlerTransition transition;
            public InputHandlerModifiers modifiers;
            public ClickCount count;
            public ICookedInputSite target;
        }
    }
}
