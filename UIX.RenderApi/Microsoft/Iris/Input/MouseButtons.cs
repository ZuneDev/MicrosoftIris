// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Input.MouseButtons
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using System;

namespace Microsoft.Iris.Input
{
    [Flags]
    public enum MouseButtons
    {
        None = 0,
        Left = 1,
        Right = 2,
        Middle = 4,
        XButton1 = 8,
        XButton2 = 16, // 0x00000010
        Primary = Left, // 0x00000001
        Secondary = XButton2 | XButton1 | Middle | Right, // 0x0000001E
    }
}
