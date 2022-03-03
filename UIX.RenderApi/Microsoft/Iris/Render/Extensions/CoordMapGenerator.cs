// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Extensions.CoordMapGenerator
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using System;

namespace Microsoft.Iris.Render.Extensions
{
    public static class CoordMapGenerator
    {
        public static CoordMap CreateSubrect(Size sizeImage, Rectangle rect)
        {
            Debug2.Validate(rect.Right <= sizeImage.Width, typeof(ArgumentOutOfRangeException), "Desired rect cannot extend beyond the image width");
            Debug2.Validate(rect.Bottom <= sizeImage.Height, typeof(ArgumentOutOfRangeException), "Desired rect cannot extend beyond the image height");
            CoordMap coordMap = new CoordMap();
            float flValue1 = rect.Left / (float)sizeImage.Width;
            float flValue2 = rect.Right / (float)sizeImage.Width;
            float flValue3 = rect.Top / (float)sizeImage.Height;
            float flValue4 = rect.Bottom / (float)sizeImage.Height;
            coordMap.AddValue(0.0f, flValue1, Orientation.Horizontal);
            coordMap.AddValue(1f, flValue2, Orientation.Horizontal);
            coordMap.AddValue(0.0f, flValue3, Orientation.Vertical);
            coordMap.AddValue(1f, flValue4, Orientation.Vertical);
            return coordMap;
        }
    }
}
