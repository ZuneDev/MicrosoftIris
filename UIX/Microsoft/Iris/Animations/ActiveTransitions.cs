// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Animations.ActiveTransitions
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System;

namespace Microsoft.Iris.Animations
{
    [Flags]
    public enum ActiveTransitions
    {
        None = 0,
        Move = 1,
        Scale = 2,
        Alpha = 4,
        Rotate = 8,
        Size = 16, // 0x00000010
        Effect = 32, // 0x00000020
        PositionX = 64, // 0x00000040
        PositionY = 128, // 0x00000080
        SizeX = 256, // 0x00000100
        SizeY = 512, // 0x00000200
        ScaleX = 1024, // 0x00000400
        ScaleY = 2048, // 0x00000800
        Orientation = 4096, // 0x00001000
        CameraEye = 8192, // 0x00002000
        CameraAt = 16384, // 0x00004000
        CameraUp = 32768, // 0x00008000
        CameraZn = 65536, // 0x00010000
    }
}
