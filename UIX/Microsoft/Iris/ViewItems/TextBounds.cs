// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.ViewItems.TextBounds
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System;

namespace Microsoft.Iris.ViewItems
{
    [Flags]
    internal enum TextBounds
    {
        Full = 0,
        AlignToAscender = 1,
        AlignToBaseline = 2,
        TrimLeftSideBearing = 4,
        Tight = TrimLeftSideBearing | AlignToBaseline | AlignToAscender, // 0x00000007
    }
}
