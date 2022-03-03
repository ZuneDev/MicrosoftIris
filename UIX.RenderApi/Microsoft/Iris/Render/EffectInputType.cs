// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.EffectInputType
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

namespace Microsoft.Iris.Render
{
    internal enum EffectInputType : byte
    {
        Color = 1,
        Image = 2,
        Blend = 3,
        Interpolate = 4,
        Layer = 5,
        ComplexImage = 6,
        Video = 7,
        Spotlight2D = 8,
        PointLight2D = 9,
        Destination = 10, // 0x0A
        MaxValue = 11, // 0x0B
    }
}
