// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Input.InputHandlerFlags
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using System;

namespace Microsoft.Iris.Input
{
    [Flags]
    public enum InputHandlerFlags
    {
        None = 0,
        Keyboard = 1,
        Mouse = 2,
        Drag = 4,
        Hid = 8,
        AppCommand = 16, // 0x00000010
        All = AppCommand | Hid | Drag | Mouse | Keyboard, // 0x0000001F
    }
}
