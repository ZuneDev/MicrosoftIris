// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Input.KeyStateInfo
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;

namespace Microsoft.Iris.Input
{
    internal class KeyStateInfo : KeyActionInfo
    {
        private static InputInfo.InfoType s_poolType = InfoType.KeyState;
        private Keys _key;

        static KeyStateInfo() => SetPoolLimitMode(s_poolType, false);

        private KeyStateInfo()
        {
        }

        public static KeyStateInfo Create(
          KeyAction action,
          InputDeviceType deviceType,
          InputModifiers modifiers,
          uint repeatCount,
          Keys key,
          bool systemKey,
          uint nativeMessageID,
          int scanCode,
          ushort eventFlags)
        {
            KeyStateInfo keyStateInfo = (KeyStateInfo)GetFromPool(s_poolType) ?? new KeyStateInfo();
            keyStateInfo.Initialize(action, deviceType, modifiers, repeatCount, key, systemKey, nativeMessageID, scanCode, eventFlags);
            return keyStateInfo;
        }

        private void Initialize(
          KeyAction action,
          InputDeviceType deviceType,
          InputModifiers modifiers,
          uint repeatCount,
          Keys key,
          bool systemKey,
          uint nativeMessageID,
          int scanCode,
          ushort eventFlags)
        {
            _key = key;
            Initialize(action, action == KeyAction.Down ? InputEventType.KeyDown : InputEventType.KeyUp, deviceType, modifiers, repeatCount, systemKey, nativeMessageID, scanCode, eventFlags);
        }

        public bool IsRepeatOf(KeyStateInfo other) => other != null && _key == other._key && IsRepeatOf((KeyActionInfo)other);

        public KeyStateInfo MakeRepeatableCopy() => Create(Action, DeviceType, Modifiers, RepeatCount, _key, SystemKey, NativeMessageID, ScanCode, KeyboardFlags);

        public Keys Key => _key;

        protected override InputInfo.InfoType PoolType => s_poolType;

        public override string ToString() => InvariantString.Format("{0}({1}, Key={2})", GetType().Name, Action, _key);
    }
}
