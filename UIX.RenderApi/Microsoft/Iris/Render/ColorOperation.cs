// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.ColorOperation
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

namespace Microsoft.Iris.Render
{
    public enum ColorOperation
    {
        Add = 0,
        LinearDodge = 0,
        Subtract = 1,
        Multiply = 2,
        Lighten = 3,
        Darken = 4,
        ColorDodge = 5,
        ColorBurn = 6,
        Screen = 7,
        Overlay = 8,
        SoftLight = 9,
        HardLight = 10, // 0x0000000A
        LinearBurn = 11, // 0x0000000B
    }
}
