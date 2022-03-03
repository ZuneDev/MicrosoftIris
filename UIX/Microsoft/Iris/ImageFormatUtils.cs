// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.ImageFormatUtils
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Render;

namespace Microsoft.Iris
{
    internal static class ImageFormatUtils
    {
        public static bool RawImageFormatToSurfaceFormat(
          RawImageFormat rawFormat,
          out SurfaceFormat surfaceFormat)
        {
            surfaceFormat = SurfaceFormat.None;
            bool flag = true;
            switch (rawFormat)
            {
                case RawImageFormat.A8R8G8B8:
                    surfaceFormat = SurfaceFormat.ARGB32;
                    break;
                case RawImageFormat.X8R8G8B8:
                    surfaceFormat = SurfaceFormat.RGB32;
                    break;
                case RawImageFormat.R8G8B8:
                    surfaceFormat = SurfaceFormat.RGB24;
                    break;
                default:
                    flag = false;
                    break;
            }
            return flag;
        }
    }
}
