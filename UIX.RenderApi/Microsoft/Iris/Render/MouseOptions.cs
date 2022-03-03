// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.MouseOptions
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using System;

namespace Microsoft.Iris.Render
{
    [Flags]
    public enum MouseOptions
    {
        Traversable = 1,
        Hittable = 2,
        Returnable = 4,
        ClipChildren = 8,
        None = 0,
        ValidMask = ClipChildren | Returnable | Hittable | Traversable, // 0x0000000F
    }
}
