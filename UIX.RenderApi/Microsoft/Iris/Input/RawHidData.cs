// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Input.RawHidData
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using System;

namespace Microsoft.Iris.Input
{
    [Serializable]
    public struct RawHidData
    {
        public uint _commandCode;
        public uint _usagePage;
        public KeyAction _action;
        public InputDeviceType _deviceType;

        public RawHidData(
          uint commandCode,
          uint usagePage,
          KeyAction action,
          InputDeviceType deviceType)
        {
            this._commandCode = commandCode;
            this._usagePage = usagePage;
            this._action = action;
            this._deviceType = deviceType;
        }
    }
}
