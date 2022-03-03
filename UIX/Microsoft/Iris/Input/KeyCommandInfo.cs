// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Input.KeyCommandInfo
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;

namespace Microsoft.Iris.Input
{
    internal class KeyCommandInfo : KeyActionInfo
    {
        private static InputInfo.InfoType s_poolType = InfoType.KeyCommand;
        public CommandCode _command;

        static KeyCommandInfo() => SetPoolLimitMode(s_poolType, false);

        private KeyCommandInfo()
        {
        }

        public static KeyCommandInfo Create(
          KeyAction action,
          InputDeviceType deviceType,
          CommandCode command)
        {
            KeyCommandInfo keyCommandInfo = (KeyCommandInfo)GetFromPool(s_poolType) ?? new KeyCommandInfo();
            keyCommandInfo.Initialize(action, deviceType, command);
            return keyCommandInfo;
        }

        private void Initialize(KeyAction action, InputDeviceType deviceType, CommandCode command)
        {
            _command = command;
            Initialize(action, action == KeyAction.Down ? InputEventType.CommandDown : InputEventType.CommandUp, deviceType);
        }

        public CommandCode Command => _command;

        protected override InputInfo.InfoType PoolType => s_poolType;

        public override string ToString() => InvariantString.Format("{0}({1}, Command={2})", GetType().Name, Action, _command);
    }
}
