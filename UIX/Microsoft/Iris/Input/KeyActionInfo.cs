// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Input.KeyActionInfo
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System;

namespace Microsoft.Iris.Input
{
    [Serializable]
    internal abstract class KeyActionInfo : KeyInfo
    {
        private KeyAction _action;
        private InputDeviceType _deviceType;
        private InputModifiers _modifiers;
        private uint _repeatCount;
        private bool _systemKey;
        private uint _nativeMessageID;
        private int _scanCode;
        private ushort _eventFlags;

        protected void Initialize(
          KeyAction action,
          InputEventType eventType,
          InputDeviceType deviceType,
          InputModifiers modifiers,
          uint repeatCount,
          bool systemKey,
          uint nativeMessageID,
          int scanCode,
          ushort eventFlags)
        {
            _systemKey = systemKey;
            _action = action;
            _deviceType = deviceType;
            _modifiers = modifiers;
            _repeatCount = repeatCount;
            _nativeMessageID = nativeMessageID;
            _scanCode = scanCode;
            _eventFlags = eventFlags;
            Initialize(eventType);
        }

        protected void Initialize(
          KeyAction action,
          InputEventType eventType,
          InputDeviceType deviceType)
        {
            Initialize(action, eventType, deviceType, InputModifiers.None, 1U, false, 0U, 0, 0);
        }

        public KeyAction Action => _action;

        public InputDeviceType DeviceType => _deviceType;

        public InputModifiers Modifiers => _modifiers;

        public uint RepeatCount => _repeatCount;

        public bool SystemKey => _systemKey;

        public uint NativeMessageID => _nativeMessageID;

        public int ScanCode => _scanCode;

        public ushort KeyboardFlags => _eventFlags;

        public bool IsRepeatOf(KeyActionInfo other) => other != null && _action == other._action && (_deviceType == other._deviceType && _systemKey == other._systemKey) && _modifiers == other._modifiers;
    }
}
