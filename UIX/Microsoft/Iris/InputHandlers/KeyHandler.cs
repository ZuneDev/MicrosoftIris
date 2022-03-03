// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.InputHandlers.KeyHandler
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Input;
using Microsoft.Iris.Library;
using Microsoft.Iris.Markup;
using Microsoft.Iris.ModelItems;
using Microsoft.Iris.Render;
using Microsoft.Iris.Session;
using Microsoft.Iris.UI;
using System;
using System.Collections;

namespace Microsoft.Iris.InputHandlers
{
    internal class KeyHandler : ModifierInputHandler
    {
        private ArrayList _invokedKeys;
        private KeyHandlerKey _key;
        private IUICommand _command;
        private bool _pressing;
        private bool _handle;
        private bool _stopRoute;
        private bool _repeat;
        private WeakReference _eventContext;

        public KeyHandler()
        {
            _handle = true;
            HandlerTransition = InputHandlerTransition.Down;
            _key = KeyHandlerKey.None;
            _repeat = true;
        }

        protected override void ConfigureInteractivity()
        {
            base.ConfigureInteractivity();
            if (!HandleDirect)
                return;
            UI.KeyInteractive = true;
        }

        public bool Pressing => _pressing;

        public KeyHandlerKey Key
        {
            get => _key;
            set
            {
                if (_key == value)
                    return;
                _key = value;
                FireNotification(NotificationID.Key);
            }
        }

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

        public bool StopRoute
        {
            get => _stopRoute;
            set
            {
                _stopRoute = value;
                FireNotification(NotificationID.StopRoute);
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

        public object EventContext => CheckEventContext(ref _eventContext);

        public bool TrackInvokedKeys
        {
            get => _invokedKeys != null;
            set
            {
                if (TrackInvokedKeys == value)
                    return;
                _invokedKeys = !value ? null : new ArrayList();
                FireNotification(NotificationID.TrackInvokedKeys);
            }
        }

        public void GetInvokedKeys(IList copyTo)
        {
            if (TrackInvokedKeys)
            {
                foreach (object invokedKey in _invokedKeys)
                    copyTo.Add(invokedKey);
                _invokedKeys.Clear();
            }
            else
                ErrorManager.ReportError("KeyHandler needs to be marked TrackInvokedKeys=\"true\" in order to call GetInvokedKeys()");
        }

        protected override void OnKeyDown(UIClass ui, KeyStateInfo info)
        {
            Keys key = info.Key;
            InputHandlerModifiers modifiers = GetModifiers(info.Modifiers);
            if (key != (Keys)_key)
                TranslateKey(ref key, ref modifiers);
            if (!KeyMatches(key) || !ShouldHandleEvent(modifiers))
                return;
            bool flag;
            if (!_pressing)
            {
                _pressing = true;
                FireNotification(NotificationID.Pressing);
                flag = true;
            }
            else
                flag = _repeat;
            if (flag)
            {
                if (HandlerTransition == InputHandlerTransition.Down)
                    InvokeCommand(key);
                if (_handle)
                    info.MarkHandled();
            }
            if (_stopRoute)
                info.TruncateRoute();
            SetEventContext(info.Target, ref _eventContext, NotificationID.EventContext);
        }

        protected override void OnKeyUp(UIClass ui, KeyStateInfo info)
        {
            Keys key = info.Key;
            InputHandlerModifiers modifiers = GetModifiers(info.Modifiers);
            if (key != (Keys)_key)
                TranslateKey(ref key, ref modifiers);
            if (!KeyMatches(key) || !ShouldHandleEvent(modifiers))
                return;
            _pressing = false;
            FireNotification(NotificationID.Pressing);
            if (HandlerTransition == InputHandlerTransition.Up)
                InvokeCommand(key);
            if (_handle)
                info.MarkHandled();
            if (_stopRoute)
                info.TruncateRoute();
            SetEventContext(info.Target, ref _eventContext, NotificationID.EventContext);
        }

        protected override void OnLoseKeyFocus(UIClass ui, KeyFocusInfo info)
        {
            if (!_pressing)
                return;
            _pressing = false;
            FireNotification(NotificationID.Pressing);
        }

        private bool KeyMatches(Keys candidate)
        {
            if (_key >= KeyHandlerKey.None)
                return candidate == (Keys)_key;
            return _key == KeyHandlerKey.Any && candidate != Keys.None;
        }

        public static void TranslateKey(ref Keys key, ref InputHandlerModifiers modifiers) => TranslateKey(ref key, ref modifiers, Orientation.Horizontal);

        public static void TranslateKey(
          ref Keys key,
          ref InputHandlerModifiers modifiers,
          Orientation orientation)
        {
            InputHandlerModifiers handlerModifiers = modifiers;
            modifiers = InputHandlerModifiers.None;
            switch (key)
            {
                case Keys.GamePadA:
                case Keys.GamePadStart:
                    key = Keys.Enter;
                    break;
                case Keys.GamePadB:
                case Keys.GamePadBack:
                    key = Keys.Back;
                    break;
                case Keys.GamePadRShoulder:
                    key = Keys.Tab;
                    break;
                case Keys.GamePadLShoulder:
                    key = Keys.Tab;
                    modifiers = InputHandlerModifiers.Shift;
                    break;
                case Keys.GamePadLTrigger:
                    key = Keys.PageUp;
                    break;
                case Keys.GamePadRTrigger:
                    key = Keys.Next;
                    break;
                case Keys.GamePadDPadUp:
                case Keys.GamePadLThumbUp:
                    key = Keys.Up;
                    break;
                case Keys.GamePadDPadDown:
                case Keys.GamePadLThumbDown:
                    key = Keys.Down;
                    break;
                case Keys.GamePadDPadLeft:
                case Keys.GamePadLThumbLeft:
                    key = Keys.Left;
                    break;
                case Keys.GamePadDPadRight:
                case Keys.GamePadLThumbRight:
                    key = Keys.Right;
                    break;
                case Keys.GamePadLThumbUpLeft:
                    key = orientation == Orientation.Vertical ? Keys.Up : Keys.Left;
                    break;
                case Keys.GamePadLThumbUpRight:
                    key = orientation == Orientation.Vertical ? Keys.Up : Keys.Right;
                    break;
                case Keys.GamePadLThumbDownRight:
                    key = orientation == Orientation.Vertical ? Keys.Down : Keys.Right;
                    break;
                case Keys.GamePadLThumbDownLeft:
                    key = orientation == Orientation.Vertical ? Keys.Down : Keys.Left;
                    break;
                default:
                    modifiers = handlerModifiers;
                    break;
            }
        }

        private void InvokeCommand(Keys key)
        {
            FireNotification(NotificationID.Invoked);
            if (_command != null)
                _command.Invoke();
            if (!TrackInvokedKeys)
                return;
            _invokedKeys.Add((KeyHandlerKey)key);
        }

        public override string ToString() => InvariantString.Format("{0}({1})", GetType().Name, _key);
    }
}
