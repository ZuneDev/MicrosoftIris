// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.InputHandlers.ClickType
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System;

namespace Microsoft.Iris.InputHandlers
{
    [Flags]
    internal enum ClickType
    {
        None = 0,
        LeftMouse = 1,
        RightMouse = 2,
        MiddleMouse = 4,
        SpaceKey = 8,
        EnterKey = 16, // 0x00000010
        GamePadA = 32, // 0x00000020
        GamePadStart = 64, // 0x00000040
        Mouse = MiddleMouse | RightMouse | LeftMouse, // 0x00000007
        Key = EnterKey | SpaceKey, // 0x00000018
        GamePad = GamePadStart | GamePadA, // 0x00000060
        Any = GamePad | Key | Mouse, // 0x0000007F
    }
}
