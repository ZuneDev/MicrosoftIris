// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.EffectOperationType
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

namespace Microsoft.Iris.Render
{
    public enum EffectOperationType : byte
    {
        Brightness = 1,
        Contrast = 2,
        Desaturate = 3,
        EdgeDetection = 4,
        Emboss = 5,
        GaussianBlur = 6,
        HSL = 7,
        HSV = 8,
        InvAlpha = 9,
        InvColor = 10, // 0x0A
        Invert = 11, // 0x0B
        LightShaft = 12, // 0x0C
        Sepia = 13, // 0x0D
        MaxValue = 14, // 0x0E
    }
}
