// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.UI.InputHandlerModifiers
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System;

namespace Microsoft.Iris.UI
{
    [Flags]
    internal enum InputHandlerModifiers
    {
        None = 0,
        Ctrl = 1,
        Shift = 2,
        Alt = 4,
        Windows = 8,
        All = Windows | Alt | Shift | Ctrl, // 0x0000000F
    }
}
