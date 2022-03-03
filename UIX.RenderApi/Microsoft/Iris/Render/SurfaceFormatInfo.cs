// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.SurfaceFormatInfo
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

namespace Microsoft.Iris.Render
{
    internal static class SurfaceFormatInfo
    {
        public static int GetBitsPerPixel(SurfaceFormat nFormat) => (int)((uint)(nFormat & (SurfaceFormat)16711680) >> 16);

        public static SurfaceFormat FromImageFormat(ImageFormat fmt)
        {
            switch (fmt)
            {
                case ImageFormat.A8R8G8B8:
                    return SurfaceFormat.ARGB32;
                case ImageFormat.X8R8G8B8:
                    return SurfaceFormat.RGB32;
                case ImageFormat.A8:
                    return SurfaceFormat.A8;
                default:
                    return SurfaceFormat.None;
            }
        }

        public static ImageFormat ToImageFormat(SurfaceFormat fmt)
        {
            switch (fmt)
            {
                case SurfaceFormat.A8:
                    return ImageFormat.A8;
                case SurfaceFormat.RGB32:
                    return ImageFormat.X8R8G8B8;
                case SurfaceFormat.ARGB32:
                    return ImageFormat.A8R8G8B8;
                default:
                    return ImageFormat.None;
            }
        }
    }
}
