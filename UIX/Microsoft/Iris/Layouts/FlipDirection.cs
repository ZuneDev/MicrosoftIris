// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Layouts.FlipDirection
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System;

namespace Microsoft.Iris.Layouts
{
    [Flags]
    internal enum FlipDirection
    {
        None = 0,
        Horizontal = 1,
        Vertical = 2,
        Both = Vertical | Horizontal, // 0x00000003
    }
}
