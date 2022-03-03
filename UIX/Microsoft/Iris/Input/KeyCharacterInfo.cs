// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Input.KeyCharacterInfo
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;

namespace Microsoft.Iris.Input
{
    internal class KeyCharacterInfo : KeyActionInfo
    {
        private static InputInfo.InfoType s_poolType = InfoType.KeyCharacter;
        private char _character;

        static KeyCharacterInfo() => SetPoolLimitMode(s_poolType, false);

        private KeyCharacterInfo()
        {
        }

        public static KeyCharacterInfo Create(
          KeyAction action,
          InputDeviceType deviceType,
          InputModifiers modifiers,
          uint repeatCount,
          char character,
          bool systemKey,
          uint nativeMessageID,
          int scanCode,
          ushort eventFlags)
        {
            KeyCharacterInfo keyCharacterInfo = (KeyCharacterInfo)GetFromPool(s_poolType) ?? new KeyCharacterInfo();
            keyCharacterInfo.Initialize(action, deviceType, modifiers, repeatCount, character, systemKey, nativeMessageID, scanCode, eventFlags);
            return keyCharacterInfo;
        }

        private void Initialize(
          KeyAction action,
          InputDeviceType deviceType,
          InputModifiers modifiers,
          uint repeatCount,
          char character,
          bool systemKey,
          uint nativeMessageID,
          int scanCode,
          ushort eventFlags)
        {
            _character = character;
            Initialize(action, InputEventType.KeyCharacter, deviceType, modifiers, repeatCount, systemKey, nativeMessageID, scanCode, eventFlags);
        }

        public char Character => _character;

        protected override InputInfo.InfoType PoolType => s_poolType;

        public override string ToString() => InvariantString.Format("{0}({1}, Key={2})", GetType().Name, Action, _character);
    }
}
