// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Layout.AreaOfInterestID
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System;

namespace Microsoft.Iris.Layout
{
    [Flags]
    public enum AreaOfInterestID
    {
        Focus = 1,
        FocusOverride = 2,
        PendingFocus = 4,
        ScrollIntoViewRequest = 8,
        ScrollableRange = 16, // 0x00000010
    }
}
