// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Layout.SharedSizePolicy
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System;

namespace Microsoft.Iris.Layout
{
    [Flags]
    internal enum SharedSizePolicy
    {
        ContributesToWidth = 1,
        ContributesToHeight = 2,
        ContributesToSize = ContributesToHeight | ContributesToWidth, // 0x00000003
        SharesWidth = 4,
        SharesHeight = 8,
        SharesSize = SharesHeight | SharesWidth, // 0x0000000C
        Default = SharesSize | ContributesToSize, // 0x0000000F
    }
}
