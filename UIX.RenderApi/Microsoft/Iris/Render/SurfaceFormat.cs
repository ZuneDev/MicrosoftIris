// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.SurfaceFormat
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

namespace Microsoft.Iris.Render
{
    public enum SurfaceFormat : uint
    {
        None = 0,
        Bpp8 = 524288, // 0x00080000
        A8 = 557056, // 0x00088000
        Bpp16 = 1048576, // 0x00100000
        RGB16_555 = 1049941, // 0x00100555
        RGB16_565 = 1049957, // 0x00100565
        ARGB16_1555 = 1054037, // 0x00101555
        Bpp24 = 1572864, // 0x00180000
        RGB24 = 1575048, // 0x00180888
        Bpp32 = 2097152, // 0x00200000
        RGB32 = 2099336, // 0x00200888
        ARGB32 = 2132104, // 0x00208888
        YUY2 = 554696704, // 0x21100000
        External = 2147483648, // 0x80000000
    }
}
