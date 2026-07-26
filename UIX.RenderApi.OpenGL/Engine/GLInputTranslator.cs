using System;
using System.Collections.Generic;
using Microsoft.Iris.Input;
using Silk.NET.Input;
using SilkKey = Silk.NET.Input.Key;
using SilkMouseButton = Silk.NET.Input.MouseButton;

namespace Microsoft.Iris.Render.OpenGL
{
    /// <summary>
    /// Translates Silk.NET.Input keyboard/mouse events into the render API's raw-input
    /// protocol (<see cref="IRawInputCallbacks"/>). Message identifiers match what the
    /// UIX <c>InputManager</c> expects: keyboard uses the <see cref="KeyboardMessageId"/>
    /// ordinals, mouse uses Win32 <c>WM_*</c> codes.
    /// </summary>
    public sealed class GLInputTranslator : IDisposable
    {
        // Win32 mouse messages (the values MouseDevice.OnRawInput switches on).
        private const uint WM_MOUSEMOVE = 0x0200;
        private const uint WM_LBUTTONDOWN = 0x0201, WM_LBUTTONUP = 0x0202, WM_LBUTTONDBLCLK = 0x0203;
        private const uint WM_RBUTTONDOWN = 0x0204, WM_RBUTTONUP = 0x0205, WM_RBUTTONDBLCLK = 0x0206;
        private const uint WM_MBUTTONDOWN = 0x0207, WM_MBUTTONUP = 0x0208, WM_MBUTTONDBLCLK = 0x0209;
        private const uint WM_MOUSEWHEEL = 0x020A;
        private const uint WM_XBUTTONDOWN = 0x020B, WM_XBUTTONUP = 0x020C, WM_XBUTTONDBLCLK = 0x020D;

        private readonly IInputContext m_context;
        private readonly GLInputSystem m_input;
        private readonly GLRenderWindow m_window;

        // Per-key repeat counters so RawKeyboardData._repCount reflects auto-repeat,
        // which the InputManager uses for key coalescing.
        private readonly Dictionary<SilkKey, uint> m_repeat = new Dictionary<SilkKey, uint>();

        public GLInputTranslator(IInputContext context, GLInputSystem input, GLRenderWindow window)
        {
            m_context = context;
            m_input = input;
            m_window = window;

            foreach (IKeyboard keyboard in context.Keyboards)
                Hook(keyboard);
            foreach (IMouse mouse in context.Mice)
                Hook(mouse);

            context.ConnectionChanged += OnConnectionChanged;
        }

        private void OnConnectionChanged(IInputDevice device, bool connected)
        {
            if (!connected)
                return;
            if (device is IKeyboard keyboard)
                Hook(keyboard);
            else if (device is IMouse mouse)
                Hook(mouse);
        }

        private void Hook(IKeyboard keyboard)
        {
            keyboard.KeyDown += OnKeyDown;
            keyboard.KeyUp += OnKeyUp;
            keyboard.KeyChar += OnKeyChar;
        }

        private void Hook(IMouse mouse)
        {
            mouse.MouseMove += OnMouseMove;
            mouse.MouseDown += OnMouseDown;
            mouse.MouseUp += OnMouseUp;
            mouse.Scroll += OnScroll;
            mouse.DoubleClick += OnDoubleClick;
        }

        // ---- Keyboard ------------------------------------------------------------
        private void OnKeyDown(IKeyboard keyboard, SilkKey key, int scanCode)
        {
            IRawInputCallbacks? cb = m_input.Callbacks;
            if (cb == null)
                return;

            uint repeat = m_repeat.TryGetValue(key, out uint prev) ? prev + 1 : 1;
            m_repeat[key] = repeat;

            bool system = IsAltDown();
            uint message = (uint)(system ? KeyboardMessageId.SysDown : KeyboardMessageId.Down);
            var data = new RawKeyboardData(MapKey(key), scanCode, repeat, 0, InputDeviceType.Keyboard);
            cb.HandleRawKeyboardInput(message, ComputeModifiers(), ref data);
        }

        private void OnKeyUp(IKeyboard keyboard, SilkKey key, int scanCode)
        {
            m_repeat.Remove(key);

            IRawInputCallbacks? cb = m_input.Callbacks;
            if (cb == null)
                return;

            bool system = IsAltDown();
            uint message = (uint)(system ? KeyboardMessageId.SysUp : KeyboardMessageId.Up);
            var data = new RawKeyboardData(MapKey(key), scanCode, 1, 0, InputDeviceType.Keyboard);
            cb.HandleRawKeyboardInput(message, ComputeModifiers(), ref data);
        }

        private void OnKeyChar(IKeyboard keyboard, char character)
        {
            IRawInputCallbacks? cb = m_input.Callbacks;
            if (cb == null)
                return;

            bool system = IsAltDown();
            uint message = (uint)(system ? KeyboardMessageId.SysChar : KeyboardMessageId.Char);
            // Character messages carry the char in _virtualKey (see KeyboardDevice.OnRawKeyCharacter).
            var data = new RawKeyboardData((Keys)character, 0, 1, 0, InputDeviceType.Keyboard);
            cb.HandleRawKeyboardInput(message, ComputeModifiers(), ref data);
        }

        // ---- Mouse ---------------------------------------------------------------
        private void OnMouseMove(IMouse mouse, System.Numerics.Vector2 position)
            => DispatchMouse(mouse, WM_MOUSEMOVE, MouseButtons.None, 0);

        private void OnMouseDown(IMouse mouse, SilkMouseButton button)
        {
            (uint message, MouseButtons iris) = button switch
            {
                SilkMouseButton.Left => (WM_LBUTTONDOWN, MouseButtons.Left),
                SilkMouseButton.Right => (WM_RBUTTONDOWN, MouseButtons.Right),
                SilkMouseButton.Middle => (WM_MBUTTONDOWN, MouseButtons.Middle),
                SilkMouseButton.Button4 => (WM_XBUTTONDOWN, MouseButtons.XButton1),
                SilkMouseButton.Button5 => (WM_XBUTTONDOWN, MouseButtons.XButton2),
                _ => (0u, MouseButtons.None),
            };
            if (message != 0)
                DispatchMouse(mouse, message, iris, 0);
        }

        private void OnMouseUp(IMouse mouse, SilkMouseButton button)
        {
            (uint message, MouseButtons iris) = button switch
            {
                SilkMouseButton.Left => (WM_LBUTTONUP, MouseButtons.Left),
                SilkMouseButton.Right => (WM_RBUTTONUP, MouseButtons.Right),
                SilkMouseButton.Middle => (WM_MBUTTONUP, MouseButtons.Middle),
                SilkMouseButton.Button4 => (WM_XBUTTONUP, MouseButtons.XButton1),
                SilkMouseButton.Button5 => (WM_XBUTTONUP, MouseButtons.XButton2),
                _ => (0u, MouseButtons.None),
            };
            if (message != 0)
                DispatchMouse(mouse, message, iris, 0);
        }

        private void OnDoubleClick(IMouse mouse, SilkMouseButton button, System.Numerics.Vector2 position)
        {
            (uint message, MouseButtons iris) = button switch
            {
                SilkMouseButton.Left => (WM_LBUTTONDBLCLK, MouseButtons.Left),
                SilkMouseButton.Right => (WM_RBUTTONDBLCLK, MouseButtons.Right),
                SilkMouseButton.Middle => (WM_MBUTTONDBLCLK, MouseButtons.Middle),
                SilkMouseButton.Button4 => (WM_XBUTTONDBLCLK, MouseButtons.XButton1),
                SilkMouseButton.Button5 => (WM_XBUTTONDBLCLK, MouseButtons.XButton2),
                _ => (0u, MouseButtons.None),
            };
            if (message != 0)
                DispatchMouse(mouse, message, iris, 0, doubleClick: true);
        }

        private void OnScroll(IMouse mouse, ScrollWheel wheel)
            => DispatchMouse(mouse, WM_MOUSEWHEEL, MouseButtons.None, (int)(wheel.Y * 120f));

        private void DispatchMouse(IMouse mouse, uint message, MouseButtons button, int wheelDelta, bool doubleClick = false)
        {
            IRawInputCallbacks? cb = m_input.Callbacks;
            if (cb == null)
                return;

            System.Numerics.Vector2 pos = mouse.Position;
            int clientX = (int)pos.X;
            int clientY = (int)pos.Y;

            IVisual? natural = m_window.HitTest(new Silk.NET.Maths.Vector2D<float>(pos.X, pos.Y));
            IVisual? capture = m_input.CaptureSite as IVisual ?? natural;

            var data = new RawMouseData(
                visCapture: capture!,
                visNatural: natural!,
                positionX: clientX,
                positionY: clientY,
                naturalX: clientX,
                naturalY: clientY,
                physicalX: clientX,
                physicalY: clientY,
                screenX: clientX + m_window.Left,
                screenY: clientY + m_window.Top,
                button: button,
                wheelDelta: wheelDelta);

            InputModifiers modifiers = ComputeModifiers(mouse);
            if (doubleClick)
                modifiers |= InputModifiers.DoubleClick;

            cb.HandleRawMouseInput(message, modifiers, ref data);
        }

        // ---- Modifiers -----------------------------------------------------------
        private bool IsAltDown()
        {
            foreach (IKeyboard k in m_context.Keyboards)
            {
                if (k.IsKeyPressed(SilkKey.AltLeft) || k.IsKeyPressed(SilkKey.AltRight))
                    return true;
            }
            return false;
        }

        private InputModifiers ComputeModifiers(IMouse? mouse = null)
        {
            InputModifiers m = InputModifiers.None;
            foreach (IKeyboard k in m_context.Keyboards)
            {
                if (k.IsKeyPressed(SilkKey.ControlLeft) || k.IsKeyPressed(SilkKey.ControlRight)) m |= InputModifiers.ControlKey;
                if (k.IsKeyPressed(SilkKey.ShiftLeft) || k.IsKeyPressed(SilkKey.ShiftRight)) m |= InputModifiers.ShiftKey;
                if (k.IsKeyPressed(SilkKey.AltLeft) || k.IsKeyPressed(SilkKey.AltRight)) m |= InputModifiers.AltKey;
                if (k.IsKeyPressed(SilkKey.SuperLeft) || k.IsKeyPressed(SilkKey.SuperRight)) m |= InputModifiers.WindowsKey;
            }

            mouse ??= FirstMouse();
            if (mouse != null)
            {
                if (mouse.IsButtonPressed(SilkMouseButton.Left)) m |= InputModifiers.LeftMouse;
                if (mouse.IsButtonPressed(SilkMouseButton.Right)) m |= InputModifiers.RightMouse;
                if (mouse.IsButtonPressed(SilkMouseButton.Middle)) m |= InputModifiers.MiddleMouse;
                if (mouse.IsButtonPressed(SilkMouseButton.Button4)) m |= InputModifiers.XMouse1;
                if (mouse.IsButtonPressed(SilkMouseButton.Button5)) m |= InputModifiers.XMouse2;
            }
            return m;
        }

        private IMouse? FirstMouse()
        {
            foreach (IMouse mouse in m_context.Mice)
                return mouse;
            return null;
        }

        public void Dispose()
        {
            m_context.ConnectionChanged -= OnConnectionChanged;
            foreach (IKeyboard keyboard in m_context.Keyboards)
            {
                keyboard.KeyDown -= OnKeyDown;
                keyboard.KeyUp -= OnKeyUp;
                keyboard.KeyChar -= OnKeyChar;
            }
            foreach (IMouse mouse in m_context.Mice)
            {
                mouse.MouseMove -= OnMouseMove;
                mouse.MouseDown -= OnMouseDown;
                mouse.MouseUp -= OnMouseUp;
                mouse.Scroll -= OnScroll;
                mouse.DoubleClick -= OnDoubleClick;
            }
            m_context.Dispose();
        }

        // ---- Key mapping (Silk.NET.Input.Key -> Win32 virtual key codes) ---------
        private static Keys MapKey(SilkKey key) => key switch
        {
            >= SilkKey.A and <= SilkKey.Z => Keys.A + (key - SilkKey.A),
            >= SilkKey.Number0 and <= SilkKey.Number9 => Keys.D0 + (key - SilkKey.Number0),
            >= SilkKey.Keypad0 and <= SilkKey.Keypad9 => Keys.NumPad0 + (key - SilkKey.Keypad0),
            >= SilkKey.F1 and <= SilkKey.F24 => Keys.F1 + (key - SilkKey.F1),

            SilkKey.Space => Keys.Space,
            SilkKey.Enter => Keys.Return,
            SilkKey.KeypadEnter => Keys.Return,
            SilkKey.Escape => Keys.Escape,
            SilkKey.Tab => Keys.Tab,
            SilkKey.Backspace => Keys.Back,
            SilkKey.Insert => Keys.Insert,
            SilkKey.Delete => Keys.Delete,
            SilkKey.Home => Keys.Home,
            SilkKey.End => Keys.End,
            SilkKey.PageUp => Keys.PageUp,
            SilkKey.PageDown => Keys.PageDown,
            SilkKey.Left => Keys.Left,
            SilkKey.Right => Keys.Right,
            SilkKey.Up => Keys.Up,
            SilkKey.Down => Keys.Down,

            SilkKey.CapsLock => Keys.CapsLock,
            SilkKey.NumLock => Keys.NumLock,
            SilkKey.ScrollLock => Keys.Scroll,
            SilkKey.PrintScreen => Keys.PrintScreen,
            SilkKey.Pause => Keys.Pause,
            SilkKey.Menu => Keys.Apps,

            SilkKey.ShiftLeft => Keys.LShiftKey,
            SilkKey.ShiftRight => Keys.RShiftKey,
            SilkKey.ControlLeft => Keys.LControlKey,
            SilkKey.ControlRight => Keys.RControlKey,
            SilkKey.AltLeft => Keys.LMenu,
            SilkKey.AltRight => Keys.RMenu,
            SilkKey.SuperLeft => Keys.LWin,
            SilkKey.SuperRight => Keys.RWin,

            SilkKey.KeypadDivide => Keys.Divide,
            SilkKey.KeypadMultiply => Keys.Multiply,
            SilkKey.KeypadSubtract => Keys.Subtract,
            SilkKey.KeypadAdd => Keys.Add,
            SilkKey.KeypadDecimal => Keys.Decimal,

            SilkKey.Semicolon => Keys.OemSemicolon,
            SilkKey.Equal => Keys.OemPlus,
            SilkKey.Comma => Keys.OemComma,
            SilkKey.Minus => Keys.OemMinus,
            SilkKey.Period => Keys.OemPeriod,
            SilkKey.Slash => Keys.OemQuestion,
            SilkKey.GraveAccent => Keys.OemTilde,
            SilkKey.LeftBracket => Keys.OemOpenBrackets,
            SilkKey.BackSlash => Keys.OemPipe,
            SilkKey.RightBracket => Keys.OemCloseBrackets,
            SilkKey.Apostrophe => Keys.OemQuotes,

            _ => Keys.None,
        };
    }
}
