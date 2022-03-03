// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.WindowOptions
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using System;

namespace Microsoft.Iris.Render
{
    [Flags]
    public enum WindowOptions
    {
        EnableCursor = 1,
        TrackMouseIdle = 2,
        NativeScreensaver = 4,
        PreventInterruption = 8,
        MaximizeExclusive = 16, // 0x00000010
        LockMouseActive = 32, // 0x00000020
        FreeformResize = 64, // 0x00000040
        ShowFormShadow = 128, // 0x00000080
        StartCentered = 256, // 0x00000100
        StartInWorkArea = 512, // 0x00000200
        MouseleaveOnIdle = 1024, // 0x00000400
        RespectStartupSettings = 2048, // 0x00000800
        MaximizeFullScreen = 4096, // 0x00001000
    }
}
