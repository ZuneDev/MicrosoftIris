// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Graphics.Dx9TextureRequirements
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using System;

namespace Microsoft.Iris.Render.Graphics
{
    [Flags]
    internal enum Dx9TextureRequirements
    {
        TexelSize = 1,
        SampleBackbuffer = 2,
        TexUVSize = 4,
        TexUVRefPoint = 8,
        DisablePerspectiveCorrection = 16, // 0x00000010
        ValidMask = DisablePerspectiveCorrection | TexUVRefPoint | TexUVSize | SampleBackbuffer | TexelSize, // 0x0000001F
    }
}
