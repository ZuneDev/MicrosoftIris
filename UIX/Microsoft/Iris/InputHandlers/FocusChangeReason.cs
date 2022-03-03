// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.InputHandlers.FocusChangeReason
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System;

namespace Microsoft.Iris.InputHandlers
{
    [Flags]
    internal enum FocusChangeReason
    {
        Mouse = 1,
        Directional = 2,
        Tab = 4,
        Key = Tab | Directional, // 0x00000006
        Other = 8,
        Any = Other | Key | Mouse, // 0x0000000F
    }
}
