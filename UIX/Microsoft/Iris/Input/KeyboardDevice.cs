// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Input.KeyboardDevice
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;

namespace Microsoft.Iris.Input
{
    internal class KeyboardDevice : InputDevice
    {
        internal const int KeyDown = 0;
        internal const int KeyUp = 1;
        internal const int KeyChar = 2;
        internal const int SysKeyDown = 3;
        internal const int SysKeyUp = 4;
        internal const int SysChar = 5;
        private const int k_initialArrayLength = 4;
        private ExpandableArray _keyStates;
        private InputModifiers _keyModifiers;
        private bool _modifiersDirty;

        public KeyboardDevice(InputManager manager)
          : base(manager)
        {
            _keyStates = new ExpandableArray(4);
            Reset();
        }

        public InputModifiers KeyboardModifiers
        {
            get
            {
                if (_modifiersDirty)
                    UpdateModifierState();
                return _keyModifiers & InputModifiers.AllKeys;
            }
        }

        public bool Alt => (KeyboardModifiers & InputModifiers.AltKey) != InputModifiers.None;

        public bool Ctrl => (KeyboardModifiers & InputModifiers.ControlKey) != InputModifiers.None;

        public bool Shift => (KeyboardModifiers & InputModifiers.ShiftKey) != InputModifiers.None;

        public bool Win => (KeyboardModifiers & InputModifiers.WindowsKey) != InputModifiers.None;

        internal KeyInfo OnRawInput(
          uint message,
          InputModifiers modifiers,
          ref RawKeyboardData args)
        {
            Manager.HACK_UpdateSystemModifiers(modifiers);
            return message == 2U || message == 5U ? OnRawKeyCharacter(message, ref args) : OnRawKeyState(message, modifiers, ref args);
        }

        internal KeyInfo OnRawKeyCharacter(uint message, ref RawKeyboardData args) => KeyCharacterInfo.Create(KeyAction.Character, args._deviceType, Manager.Modifiers, args._repCount, (char)args._virtualKey, message == 5U, message, args._scanCode, args._flags);

        internal KeyInfo OnRawKeyState(
          uint message,
          InputModifiers rawModifiers,
          ref RawKeyboardData args)
        {
            KeyStateInfo keyStateInfo = null;
            bool systemKey = false;
            KeyAction action;
            switch (message)
            {
                case 0:
                    action = KeyAction.Down;
                    break;
                case 1:
                    action = KeyAction.Up;
                    break;
                case 3:
                    systemKey = true;
                    goto case 0;
                case 4:
                    systemKey = true;
                    goto case 1;
                default:
                    return null;
            }
            if (TrackKey(action, args._virtualKey, args._scanCode))
            {
                InputModifiers modifiers = Manager.Modifiers & ~MapKeyToModifier(args._virtualKey);
                keyStateInfo = KeyStateInfo.Create(action, args._deviceType, modifiers, args._repCount, args._virtualKey, systemKey, message, args._scanCode, args._flags);
            }
            return keyStateInfo;
        }

        public bool IsKeyDown(Keys key)
        {
            foreach (KeyboardDevice.KeyState keyState in _keyStates)
            {
                if (keyState.VKey == key && keyState.IsDown)
                    return true;
            }
            return false;
        }

        public bool IsKeyHandled(Keys key)
        {
            foreach (KeyboardDevice.KeyState keyState in _keyStates)
            {
                if (keyState.VKey == key)
                    return keyState.IsHandled;
            }
            return false;
        }

        internal void MarkKeyHandled(Keys key)
        {
            foreach (KeyboardDevice.KeyState keyState in _keyStates)
            {
                if (keyState.VKey == key && keyState.IsDown)
                {
                    keyState.MarkHandled();
                    break;
                }
            }
        }

        public void Reset()
        {
            _keyStates.Clear();
            _modifiersDirty = true;
            _keyModifiers = InputModifiers.None;
        }

        private bool TrackKey(KeyAction action, Keys vkey, int scanCode)
        {
            bool flag = false;
            if (vkey == Keys.None)
                return false;
            if (IsModifier(vkey))
                MarkModifiersInvalid();
            switch (action)
            {
                case KeyAction.Up:
                    flag = TrackKeyUp(vkey, scanCode);
                    break;
                case KeyAction.Down:
                    flag = TrackKeyDown(vkey, scanCode);
                    break;
            }
            return flag;
        }

        private bool TrackKeyDown(Keys vkey, int scanCode)
        {
            KeyboardDevice.KeyState keyState1 = null;
            for (int index = 0; index < _keyStates.Length; ++index)
            {
                KeyboardDevice.KeyState keyState2 = (KeyboardDevice.KeyState)_keyStates[index];
                if (keyState2 != null)
                {
                    if (keyState2.IsDown)
                    {
                        if (keyState2.VKey == vkey && keyState2.ScanCode == scanCode)
                        {
                            keyState1 = keyState2;
                            break;
                        }
                    }
                    else
                    {
                        _keyStates[index] = null;
                        keyState2.Dispose();
                    }
                }
            }
            if (keyState1 == null)
            {
                KeyboardDevice.KeyState keyState2 = new KeyboardDevice.KeyState(vkey, scanCode);
                _keyStates.Add(keyState2);
                keyState2.IsDown = true;
            }
            return true;
        }

        private bool TrackKeyUp(Keys vkey, int scanCode)
        {
            KeyboardDevice.KeyState keyState1 = null;
            bool flag = false;
            foreach (KeyboardDevice.KeyState keyState2 in _keyStates)
            {
                if (keyState2.VKey == vkey && keyState2.ScanCode == scanCode)
                {
                    keyState1 = keyState2;
                    break;
                }
            }
            if (keyState1 != null)
            {
                flag = keyState1.IsDown;
                keyState1.IsDown = false;
            }
            int num = flag ? 1 : 0;
            return flag;
        }

        private bool IsModifier(Keys vkey) => MapKeyToModifier(vkey) != InputModifiers.None;

        private void MarkModifiersInvalid() => _modifiersDirty = true;

        private InputModifiers MapKeyToModifier(Keys vkey)
        {
            switch (vkey)
            {
                case Keys.ShiftKey:
                case Keys.LShiftKey:
                case Keys.RShiftKey:
                    return InputModifiers.ShiftKey;
                case Keys.ControlKey:
                case Keys.LControlKey:
                case Keys.RControlKey:
                    return InputModifiers.ControlKey;
                case Keys.Menu:
                case Keys.LMenu:
                case Keys.RMenu:
                    return InputModifiers.AltKey;
                case Keys.LWin:
                case Keys.RWin:
                    return InputModifiers.WindowsKey;
                default:
                    return InputModifiers.None;
            }
        }

        private void UpdateModifierState()
        {
            if (!_modifiersDirty)
                return;
            _keyModifiers = InputModifiers.None;
            foreach (KeyboardDevice.KeyState keyState in _keyStates)
            {
                if (keyState.IsDown)
                    _keyModifiers |= MapKeyToModifier(keyState.VKey);
            }
            _modifiersDirty = false;
        }

        private class KeyState
        {
            private Keys _vkey;
            private int _scanCode;
            private bool _down;
            private bool _handled;

            public KeyState(Keys vkey, int scanCode)
            {
                _vkey = vkey;
                _scanCode = scanCode;
                _down = false;
                _handled = false;
            }

            public void Dispose() => Dispose(true);

            protected void Dispose(bool inDispose)
            {
                if (!inDispose)
                    return;
                _vkey = Keys.None;
                _scanCode = -1;
            }

            public Keys VKey => _vkey;

            public int ScanCode => _scanCode;

            public bool IsDown
            {
                get => _down;
                set => _down = value;
            }

            public bool IsHandled => _handled;

            public void MarkHandled() => _handled = true;
        }
    }
}
