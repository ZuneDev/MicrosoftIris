// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.RenderAPI.VideoPlayback.LinearVideoStretch
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.RenderAPI.Drawing;
using System;

namespace Microsoft.Iris.RenderAPI.VideoPlayback
{
    internal static class LinearVideoStretch
    {
        public static readonly VideoZoomHandler ShrinkToFit = new VideoZoomHandler(ComputeShrinkToFitZoom);
        public static readonly VideoZoomHandler GrowToFit = new VideoZoomHandler(ComputeGrowToFitZoom);
        public static readonly VideoZoomHandler StretchToFill = new VideoZoomHandler(ComputeStretchToFillZoom);

        private static void ComputeShrinkToFitZoom(
          RectangleF rcfBoundSrcVideoPxl,
          RectangleF rcfBoundDestViewPxl,
          out RectangleF[] arrcfOutputSrcVideoPxl,
          out RectangleF[] arrcfOutputDestViewPxl)
        {
            RectangleF rectangleF1;
            RectangleF rectangleF2;
            if (rcfBoundDestViewPxl.Width / rcfBoundSrcVideoPxl.Width < (double)(rcfBoundDestViewPxl.Height / rcfBoundSrcVideoPxl.Height))
            {
                float num1 = rcfBoundDestViewPxl.Width / rcfBoundSrcVideoPxl.Width;
                float height = rcfBoundSrcVideoPxl.Height * num1;
                float num2 = rcfBoundDestViewPxl.Height - height;
                rectangleF1 = rcfBoundSrcVideoPxl;
                rectangleF2 = new RectangleF(rcfBoundDestViewPxl.X, rcfBoundDestViewPxl.Y + num2 / 2f, rcfBoundDestViewPxl.Width, height);
            }
            else
            {
                float num1 = rcfBoundDestViewPxl.Height / rcfBoundSrcVideoPxl.Height;
                float width = rcfBoundSrcVideoPxl.Width * num1;
                float num2 = rcfBoundDestViewPxl.Width - width;
                rectangleF1 = rcfBoundSrcVideoPxl;
                rectangleF2 = new RectangleF(rcfBoundDestViewPxl.X + num2 / 2f, rcfBoundDestViewPxl.Y, width, rcfBoundDestViewPxl.Height);
            }
            arrcfOutputSrcVideoPxl = new RectangleF[1]
            {
        rectangleF1
            };
            arrcfOutputDestViewPxl = new RectangleF[1]
            {
        rectangleF2
            };
        }

        private static void ComputeGrowToFitZoom(
          RectangleF rcfBoundSrcVideoPxl,
          RectangleF rcfBoundDestViewPxl,
          out RectangleF[] arrcfOutputSrcVideoPxl,
          out RectangleF[] arrcfOutputDestViewPxl)
        {
            ApplyPillarboxAdjustment(ref rcfBoundSrcVideoPxl, rcfBoundDestViewPxl);
            RectangleF rectangleF1;
            RectangleF rectangleF2;
            if (rcfBoundDestViewPxl.Width / rcfBoundSrcVideoPxl.Width < (double)(rcfBoundDestViewPxl.Height / rcfBoundSrcVideoPxl.Height))
            {
                float num1 = rcfBoundDestViewPxl.Height / rcfBoundSrcVideoPxl.Height;
                float width = rcfBoundDestViewPxl.Width / num1;
                float num2 = rcfBoundSrcVideoPxl.Width - width;
                rectangleF1 = new RectangleF(rcfBoundSrcVideoPxl.X + num2 / 2f, rcfBoundSrcVideoPxl.Y, width, rcfBoundSrcVideoPxl.Height);
                rectangleF2 = rcfBoundDestViewPxl;
            }
            else
            {
                float num1 = rcfBoundDestViewPxl.Width / rcfBoundSrcVideoPxl.Width;
                float height = rcfBoundDestViewPxl.Height / num1;
                float num2 = rcfBoundSrcVideoPxl.Height - height;
                rectangleF1 = new RectangleF(rcfBoundSrcVideoPxl.X, rcfBoundSrcVideoPxl.Y + num2 / 2f, rcfBoundSrcVideoPxl.Width, height);
                rectangleF2 = rcfBoundDestViewPxl;
            }
            arrcfOutputSrcVideoPxl = new RectangleF[1]
            {
        rectangleF1
            };
            arrcfOutputDestViewPxl = new RectangleF[1]
            {
        rectangleF2
            };
        }

        private static void ComputeStretchToFillZoom(
          RectangleF rcfBoundSrcVideoPxl,
          RectangleF rcfBoundDestViewPxl,
          out RectangleF[] arrcfOutputSrcVideoPxl,
          out RectangleF[] arrcfOutputDestViewPxl)
        {
            ApplyPillarboxAdjustment(ref rcfBoundSrcVideoPxl, rcfBoundDestViewPxl);
            arrcfOutputSrcVideoPxl = new RectangleF[1]
            {
        rcfBoundSrcVideoPxl
            };
            arrcfOutputDestViewPxl = new RectangleF[1]
            {
        rcfBoundDestViewPxl
            };
        }

        internal static void ApplyPillarboxAdjustment(
          ref RectangleF rcfBoundSrcVideoPxl,
          RectangleF rcfBoundDestViewPxl)
        {
            if (Math.Abs(rcfBoundDestViewPxl.Width / rcfBoundSrcVideoPxl.Width - rcfBoundDestViewPxl.Height / rcfBoundSrcVideoPxl.Height) >= 0.1 || rcfBoundDestViewPxl.Width / (double)rcfBoundDestViewPxl.Height <= 1.5 || rcfBoundSrcVideoPxl.Width / (double)rcfBoundSrcVideoPxl.Height <= 1.5)
                return;
            float num = (float)(rcfBoundSrcVideoPxl.Height * 4.0 / 3.0);
            rcfBoundSrcVideoPxl.X += (float)((rcfBoundSrcVideoPxl.Width - (double)num) / 2.0);
            rcfBoundSrcVideoPxl.Width = num;
        }
    }
}
