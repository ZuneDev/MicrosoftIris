// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Accessibility.AccSelFlags
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System;

namespace Microsoft.Iris.Accessibility
{
    [Flags]
    internal enum AccSelFlags
    {
        None = 0,
        TakeFocus = 1,
        TakeSelection = 2,
        ExtendSelection = 4,
        AddSelection = 8,
        RemoveSelection = 16, // 0x00000010
        Valid = RemoveSelection | AddSelection | ExtendSelection | TakeSelection | TakeFocus, // 0x0000001F
    }
}
