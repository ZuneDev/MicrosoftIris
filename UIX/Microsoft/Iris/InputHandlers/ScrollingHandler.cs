// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.InputHandlers.ScrollingHandler
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Input;
using Microsoft.Iris.Markup;
using Microsoft.Iris.ModelItems;
using Microsoft.Iris.Render;
using Microsoft.Iris.UI;
using Microsoft.Iris.ViewItems;
using System;

namespace Microsoft.Iris.InputHandlers
{
    internal class ScrollingHandler : InputHandler
    {
        private ScrollModel _model;
        private bool _handleDirectionalKeysFlag;
        private bool _handlePageKeysFlag;
        private bool _handleHomeEndKeysFlag;
        private bool _handlePageCommandsFlag;
        private bool _handleMouseWheelFlag;
        private bool _useFocusBehavior;
        private Keys _currentCampingKey;
        private int _cumulativeMouseWheelDelta;

        public ScrollingHandler()
        {
            _handleDirectionalKeysFlag = true;
            _handlePageKeysFlag = true;
            _handlePageCommandsFlag = true;
            _handleHomeEndKeysFlag = true;
            _handleMouseWheelFlag = true;
            _useFocusBehavior = true;
            HandlerStage = InputHandlerStage.Direct | InputHandlerStage.Bubbled;
        }

        protected override void ConfigureInteractivity()
        {
            base.ConfigureInteractivity();
            if (!HandleDirect)
                return;
            if (_handleDirectionalKeysFlag || _handlePageKeysFlag || (_handleHomeEndKeysFlag || _handlePageCommandsFlag))
                UI.KeyInteractive = true;
            if (!_handleMouseWheelFlag)
                return;
            UI.MouseInteractive = true;
        }

        public bool HandleDirectionalKeys
        {
            get => _handleDirectionalKeysFlag;
            set
            {
                if (_handleDirectionalKeysFlag == value)
                    return;
                _handleDirectionalKeysFlag = value;
                FireNotification(NotificationID.HandleDirectionalKeys);
            }
        }

        public bool HandlePageKeys
        {
            get => _handlePageKeysFlag;
            set
            {
                if (_handlePageKeysFlag == value)
                    return;
                _handlePageKeysFlag = value;
                FireNotification(NotificationID.HandlePageKeys);
            }
        }

        public bool HandleHomeEndKeys
        {
            get => _handleHomeEndKeysFlag;
            set
            {
                if (_handleHomeEndKeysFlag == value)
                    return;
                _handleHomeEndKeysFlag = value;
                FireNotification(NotificationID.HandleHomeEndKeys);
            }
        }

        public bool HandlePageCommands
        {
            get => _handlePageCommandsFlag;
            set
            {
                if (_handlePageCommandsFlag == value)
                    return;
                _handlePageCommandsFlag = value;
                FireNotification(NotificationID.HandlePageCommands);
            }
        }

        public bool HandleMouseWheel
        {
            get => _handleMouseWheelFlag;
            set
            {
                if (_handleMouseWheelFlag == value)
                    return;
                _handleMouseWheelFlag = value;
                FireNotification(NotificationID.HandleMouseWheel);
            }
        }

        public ScrollModel ScrollModel
        {
            get => _model;
            set
            {
                if (_model == value)
                    return;
                _model = value;
                FireNotification(NotificationID.ScrollModel);
            }
        }

        public bool UseFocusBehavior
        {
            get => _useFocusBehavior;
            set
            {
                if (_useFocusBehavior == value)
                    return;
                _useFocusBehavior = value;
                FireNotification(NotificationID.UseFocusBehavior);
            }
        }

        private bool ValidScrollModel => _model != null && _model.Enabled;

        protected override void OnGainKeyFocus(UIClass sender, KeyFocusInfo info)
        {
            if (!ValidScrollModel || !_useFocusBehavior)
                return;
            _model.NotifyFocusChange(info.Target as UIClass);
        }

        protected override void OnKeyDown(UIClass ui, KeyStateInfo info)
        {
            if (!ValidScrollModel)
                return;
            if (info.Key != _currentCampingKey)
                EndCamp();
            Keys key = info.Key;
            InputHandlerModifiers modifiers = GetModifiers(info.Modifiers);
            KeyHandler.TranslateKey(ref key, ref modifiers, Orientation);
            switch (ShouldHandleKey(key))
            {
                case HandleKeyPolicy.None:
                    return;
                case HandleKeyPolicy.Up:
                    _model.ScrollUp(_useFocusBehavior);
                    break;
                case HandleKeyPolicy.Down:
                    _model.ScrollDown(_useFocusBehavior);
                    break;
                case HandleKeyPolicy.PageUp:
                    _model.PageUp(_useFocusBehavior);
                    break;
                case HandleKeyPolicy.PageDown:
                    _model.PageDown(_useFocusBehavior);
                    break;
                case HandleKeyPolicy.Home:
                    _model.Home(_useFocusBehavior);
                    break;
                case HandleKeyPolicy.End:
                    _model.End(_useFocusBehavior);
                    break;
            }
            BeginCamp(info.Key);
            info.MarkHandled();
        }

        private void BeginCamp(Keys key)
        {
            if (!IsCamping)
                _model.BeginCamp();
            _currentCampingKey = key;
        }

        private void EndCamp()
        {
            if (!IsCamping)
                return;
            _model.EndCamp();
            _currentCampingKey = Keys.None;
        }

        private bool IsCamping => _currentCampingKey != Keys.None;

        protected override void OnKeyUp(UIClass ui, KeyStateInfo info)
        {
            if (!ValidScrollModel)
                return;
            if (info.Key == _currentCampingKey)
                EndCamp();
            Keys key = info.Key;
            InputHandlerModifiers modifiers = GetModifiers(info.Modifiers);
            KeyHandler.TranslateKey(ref key, ref modifiers, Orientation);
            if (ShouldHandleKey(key) == HandleKeyPolicy.None)
                return;
            info.MarkHandled();
        }

        protected override void OnLoseDeepKeyFocus() => EndCamp();

        protected override void OnCommandDown(UIClass ui, KeyCommandInfo info)
        {
            if (!ValidScrollModel || info.Action != KeyAction.Down)
                return;
            switch (info.Command)
            {
                case CommandCode.ChannelUp:
                    if (!_handlePageCommandsFlag)
                        break;
                    _model.PageUp();
                    info.MarkHandled();
                    break;
                case CommandCode.ChannelDown:
                    if (!_handlePageCommandsFlag)
                        break;
                    _model.PageDown();
                    info.MarkHandled();
                    break;
            }
        }

        protected override void OnCommandUp(UIClass ui, KeyCommandInfo info)
        {
            switch (info.Command)
            {
                case CommandCode.ChannelUp:
                case CommandCode.ChannelDown:
                    if (!_handlePageCommandsFlag)
                        break;
                    info.MarkHandled();
                    break;
            }
        }

        protected override void OnMouseWheel(UIClass ui, MouseWheelInfo info)
        {
            if (!_handleMouseWheelFlag || !ValidScrollModel || InputHasKeyModifiers(info))
                return;
            _cumulativeMouseWheelDelta += -info.WheelDelta;
            if (Math.Abs(_cumulativeMouseWheelDelta) >= 120)
            {
                if (_cumulativeMouseWheelDelta > 0)
                    _model.ScrollDown(_cumulativeMouseWheelDelta / 120);
                else
                    _model.ScrollUp(Math.Abs(_cumulativeMouseWheelDelta / 120));
                _cumulativeMouseWheelDelta %= 120;
            }
            info.MarkHandled();
        }

        private ScrollingHandler.HandleKeyPolicy ShouldHandleKey(Keys key)
        {
            ScrollingHandler.HandleKeyPolicy handleKeyPolicy = HandleKeyPolicy.None;
            switch (key)
            {
                case Keys.PageUp:
                    if (_handlePageKeysFlag)
                    {
                        handleKeyPolicy = HandleKeyPolicy.PageUp;
                        break;
                    }
                    break;
                case Keys.Next:
                    if (_handlePageKeysFlag)
                    {
                        handleKeyPolicy = HandleKeyPolicy.PageDown;
                        break;
                    }
                    break;
                case Keys.End:
                    if (_handleHomeEndKeysFlag)
                    {
                        handleKeyPolicy = HandleKeyPolicy.End;
                        break;
                    }
                    break;
                case Keys.Home:
                    if (_handleHomeEndKeysFlag)
                    {
                        handleKeyPolicy = HandleKeyPolicy.Home;
                        break;
                    }
                    break;
                case Keys.Left:
                    if (_handleDirectionalKeysFlag && Orientation == Orientation.Horizontal)
                    {
                        handleKeyPolicy = UI.Zone.Session.IsRtl ? HandleKeyPolicy.Down : HandleKeyPolicy.Up;
                        break;
                    }
                    break;
                case Keys.Up:
                    if (_handleDirectionalKeysFlag && Orientation == Orientation.Vertical)
                    {
                        handleKeyPolicy = HandleKeyPolicy.Up;
                        break;
                    }
                    break;
                case Keys.Right:
                    if (_handleDirectionalKeysFlag && Orientation == Orientation.Horizontal)
                    {
                        handleKeyPolicy = UI.Zone.Session.IsRtl ? HandleKeyPolicy.Up : HandleKeyPolicy.Down;
                        break;
                    }
                    break;
                case Keys.Down:
                    if (_handleDirectionalKeysFlag && Orientation == Orientation.Vertical)
                    {
                        handleKeyPolicy = HandleKeyPolicy.Down;
                        break;
                    }
                    break;
            }
            return handleKeyPolicy;
        }

        private Orientation Orientation
        {
            get
            {
                Orientation orientation = Orientation.Horizontal;
                if (_model.TargetViewItem is Scroller targetViewItem)
                    orientation = targetViewItem.Orientation;
                return orientation;
            }
        }

        private bool InputHasKeyModifiers(InputInfo info)
        {
            InputModifiers inputModifiers = InputModifiers.None;
            switch (info)
            {
                case KeyStateInfo _:
                    inputModifiers = ((KeyActionInfo)info).Modifiers;
                    break;
                case MouseActionInfo _:
                    inputModifiers = ((MouseActionInfo)info).Modifiers;
                    break;
            }
            return (inputModifiers & InputModifiers.AllKeys) != InputModifiers.None;
        }

        private enum HandleKeyPolicy
        {
            None,
            Up,
            Down,
            PageUp,
            PageDown,
            Home,
            End,
        }
    }
}
