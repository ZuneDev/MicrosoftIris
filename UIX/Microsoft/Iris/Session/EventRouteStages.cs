// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Session.EventRouteStages
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System;

namespace Microsoft.Iris.Session
{
    [Flags]
    internal enum EventRouteStages
    {
        None = 0,
        Preview = 1,
        Direct = 2,
        Bubbled = 4,
        Routed = 8,
        Unhandled = 16, // 0x00000010
        All = Unhandled | Routed | Bubbled | Direct | Preview, // 0x0000001F
    }
}
