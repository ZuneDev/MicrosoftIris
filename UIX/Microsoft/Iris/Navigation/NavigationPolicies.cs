// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Navigation.NavigationPolicies
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System;

namespace Microsoft.Iris.Navigation
{
    [Flags]
    public enum NavigationPolicies
    {
        None = 0,
        Group = 1,
        Row = 2,
        Column = 4,
        TabGroup = 16, // 0x00000010
        RememberFocus = 32, // 0x00000020
        PreferFocusOrder = 64, // 0x00000040
        ContainVertical = 256, // 0x00000100
        ContainHorizontal = 512, // 0x00000200
        ContainTabOrder = 1024, // 0x00000400
        ContainDirectional = ContainHorizontal | ContainVertical, // 0x00000300
        ContainAll = ContainDirectional | ContainTabOrder, // 0x00000700
        WrapVertical = 4096, // 0x00001000
        WrapHorizontal = 8192, // 0x00002000
        WrapTabOrder = 16384, // 0x00004000
        WrapDirectional = WrapHorizontal | WrapVertical, // 0x00003000
        WrapAll = WrapDirectional | WrapTabOrder, // 0x00007000
        FlowHorizontal = 65536, // 0x00010000
        FlowVertical = 131072, // 0x00020000
        PreferContainerFocus = 262144, // 0x00040000
    }
}
