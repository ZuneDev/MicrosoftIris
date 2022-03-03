// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Accessibility.AccStates
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System;

namespace Microsoft.Iris.Accessibility
{
    [Flags]
    internal enum AccStates
    {
        None = 0,
        Unavailable = 1,
        Selected = 2,
        Focused = 4,
        Pressed = 8,
        Checked = 16, // 0x00000010
        Mixed = 32, // 0x00000020
        Indeterminate = Mixed, // 0x00000020
        ReadOnly = 64, // 0x00000040
        HotTracked = 128, // 0x00000080
        Default = 256, // 0x00000100
        Expanded = 512, // 0x00000200
        Collapsed = 1024, // 0x00000400
        Busy = 2048, // 0x00000800
        Floating = 4096, // 0x00001000
        Marquee = 8192, // 0x00002000
        Animated = 16384, // 0x00004000
        Invisible = 32768, // 0x00008000
        OffScreen = 65536, // 0x00010000
        Sizeable = 131072, // 0x00020000
        Moveable = 262144, // 0x00040000
        SelfVoicing = 524288, // 0x00080000
        Focusable = 1048576, // 0x00100000
        Selectable = 2097152, // 0x00200000
        Linked = 4194304, // 0x00400000
        Traversed = 8388608, // 0x00800000
        MultiSelectable = 16777216, // 0x01000000
        ExtSelectable = 33554432, // 0x02000000
        Alert_Low = 67108864, // 0x04000000
        Alert_Medium = 134217728, // 0x08000000
        Alert_High = 268435456, // 0x10000000
        Protected = 536870912, // 0x20000000
        HasPopup = 1073741824, // 0x40000000
        Valid = HasPopup | Protected | Alert_High | Alert_Medium | Alert_Low | ExtSelectable | MultiSelectable | Traversed | Linked | Selectable | Focusable | SelfVoicing | Moveable | Sizeable | OffScreen | Invisible | Animated | Marquee | Floating | Busy | Collapsed | Expanded | Default | HotTracked | ReadOnly | Indeterminate | Checked | Pressed | Focused | Selected | Unavailable, // 0x7FFFFFFF
    }
}
