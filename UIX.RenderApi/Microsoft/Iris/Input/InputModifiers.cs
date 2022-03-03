// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Input.InputModifiers
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using System;

namespace Microsoft.Iris.Input
{
    [Flags]
    public enum InputModifiers
    {
        None = 0,
        ControlKey = 1,
        ShiftKey = 2,
        AltKey = 4,
        WindowsKey = 8,
        LeftMouse = 16, // 0x00000010
        RightMouse = 32, // 0x00000020
        MiddleMouse = 64, // 0x00000040
        XMouse1 = 128, // 0x00000080
        XMouse2 = 256, // 0x00000100
        DoubleClick = 512, // 0x00000200
        AllKeys = WindowsKey | AltKey | ShiftKey | ControlKey, // 0x0000000F
        AllButtons = XMouse2 | XMouse1 | MiddleMouse | RightMouse | LeftMouse, // 0x000001F0
        AllMouse = AllButtons | DoubleClick, // 0x000003F0
    }
}
