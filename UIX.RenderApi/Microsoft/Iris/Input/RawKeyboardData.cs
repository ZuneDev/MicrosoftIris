// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Input.RawKeyboardData
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using System;

namespace Microsoft.Iris.Input
{
    [Serializable]
    public struct RawKeyboardData
    {
        public Keys _virtualKey;
        public int _scanCode;
        public uint _repCount;
        public InputDeviceType _deviceType;
        public ushort _flags;

        public RawKeyboardData(
          Keys virtualKey,
          int scanCode,
          uint repCount,
          ushort flags,
          InputDeviceType deviceType)
        {
            this._virtualKey = virtualKey;
            this._scanCode = scanCode;
            this._repCount = repCount;
            this._deviceType = deviceType;
            this._flags = flags;
        }
    }
}
