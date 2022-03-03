// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Extensions.ImageLoader
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Internal;
using System;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Extensions
{
    public static class ImageLoader
    {
        public static bool LoadHeader(IntPtr rgData, int length, out ImageHeader header)
        {
            ImageRequirements req = new ImageRequirements();
            return LoadHeader(rgData, length, req, out header);
        }

        public static bool LoadHeader(
          IntPtr rgData,
          int length,
          ImageRequirements req,
          out ImageHeader header)
        {
            Debug2.Validate(rgData != IntPtr.Zero, typeof(ArgumentNullException), "Must provide valid data to load");
            Debug2.Validate(length > 0, typeof(ArgumentOutOfRangeException), "Must provide non-zero length data to load");
            Debug2.Validate(req != null, typeof(ArgumentNullException), "Must provide valid ImageRequirements");
            ExtensionsApi.BitmapOptions nOptions = ExtensionsApi.BitmapOptions.None;
            BitmapInformation bitmapInformation = new BitmapInformation();
            HRESULT hresult = new HRESULT(-1);
            try
            {
                hresult = ExtensionsApi.SpBitmapLoadBuffer(rgData, (uint)length, req, nOptions, out bitmapInformation.hBitmap, out bitmapInformation.imageInfo);
                if (!hresult.IsSuccess())
                    header = new ImageHeader();
                else
                    header = bitmapInformation.imageInfo.Header;
            }
            finally
            {
                bitmapInformation.Dispose();
            }
            return hresult.IsSuccess();
        }

        public static bool FromFile(
          IImage image,
          string filename,
          Size maxSize,
          bool flipRTL,
          bool antialiasEdges,
          int borderWidth,
          ColorF borderColor,
          out BitmapInformation bitmapInfo)
        {
            Debug2.Validate(image != null, typeof(ArgumentNullException), "Image must be valid");
            ImageRequirements req = new ImageRequirements();
            req.BorderWidth = borderWidth;
            req.BorderColor = borderColor;
            req.Flippable = flipRTL;
            req.AntialiasEdges = antialiasEdges;
            req.MaximumSize = maxSize;
            ExtensionsApi.BitmapOptions nOptions = ExtensionsApi.BitmapOptions.Decode;
            if (flipRTL)
                nOptions |= ExtensionsApi.BitmapOptions.Flip;
            BitmapInformation bitmapInformation = new BitmapInformation();
            if (!ExtensionsApi.SpBitmapLoadFile(filename, req, nOptions, out bitmapInformation.hBitmap, out bitmapInformation.imageInfo).IsSuccess())
            {
                bitmapInfo = null;
                return false;
            }
            bool flag = image.LoadContent(SurfaceFormatInfo.ToImageFormat(bitmapInformation.imageInfo.Header.nFormat), bitmapInformation.imageInfo.Header.sizeActualPxl, bitmapInformation.imageInfo.Header.nStride, bitmapInformation.imageInfo.Data.rgData);
            if (flag)
            {
                bitmapInfo = bitmapInformation;
            }
            else
            {
                bitmapInfo = null;
                bitmapInformation.Dispose();
            }
            return flag;
        }

        public static bool FromResource(
          IImage image,
          string moduleName,
          string resourceID,
          Size maxSize,
          bool flipRTL,
          bool antialiasEdges,
          int borderWidth,
          ColorF borderColor,
          out BitmapInformation bitmapInfo)
        {
            Debug2.Validate(image != null, typeof(ArgumentNullException), "Image must be valid");
            ImageRequirements req = new ImageRequirements();
            req.BorderWidth = borderWidth;
            req.BorderColor = borderColor;
            req.Flippable = flipRTL;
            req.AntialiasEdges = antialiasEdges;
            req.MaximumSize = maxSize;
            Win32Api.HINSTANCE hinst = ModuleManager.Instance.LoadModule(moduleName);
            if (hinst == Win32Api.HINSTANCE.NULL)
            {
                bitmapInfo = null;
                return false;
            }
            ExtensionsApi.BitmapOptions nOptions = ExtensionsApi.BitmapOptions.Decode;
            if (flipRTL)
                nOptions |= ExtensionsApi.BitmapOptions.Flip;
            BitmapInformation bitmapInformation = new BitmapInformation();
            if (!ExtensionsApi.SpBitmapLoadResource(hinst, resourceID, 10, req, nOptions, out bitmapInformation.hBitmap, out bitmapInformation.imageInfo).IsSuccess())
            {
                bitmapInfo = null;
                return false;
            }
            bool flag = image.LoadContent(SurfaceFormatInfo.ToImageFormat(bitmapInformation.imageInfo.Header.nFormat), bitmapInformation.imageInfo.Header.sizeActualPxl, bitmapInformation.imageInfo.Header.nStride, bitmapInformation.imageInfo.Data.rgData);
            if (flag)
            {
                bitmapInfo = bitmapInformation;
            }
            else
            {
                bitmapInfo = null;
                bitmapInformation.Dispose();
            }
            return flag;
        }

        public static bool FromBuffer(
          IImage image,
          IntPtr buffer,
          int length,
          Size maxSize,
          bool flipRTL,
          bool antialiasEdges,
          int borderWidth,
          ColorF borderColor,
          out BitmapInformation bitmapInfo)
        {
            Debug2.Validate(image != null, typeof(ArgumentNullException), "Image must be valid");
            Debug2.Validate(length > 0, typeof(ArgumentException), "Do not call for zero-length buffer");
            ImageRequirements req = new ImageRequirements();
            req.BorderWidth = borderWidth;
            req.BorderColor = borderColor;
            req.Flippable = flipRTL;
            req.AntialiasEdges = antialiasEdges;
            req.MaximumSize = maxSize;
            ExtensionsApi.BitmapOptions nOptions = ExtensionsApi.BitmapOptions.Decode;
            if (flipRTL)
                nOptions |= ExtensionsApi.BitmapOptions.Flip;
            BitmapInformation bitmapInformation = new BitmapInformation();
            if (!ExtensionsApi.SpBitmapLoadBuffer(buffer, (uint)length, req, nOptions, out bitmapInformation.hBitmap, out bitmapInformation.imageInfo).IsSuccess())
            {
                bitmapInfo = null;
                return false;
            }
            bool flag = image.LoadContent(SurfaceFormatInfo.ToImageFormat(bitmapInformation.imageInfo.Header.nFormat), bitmapInformation.imageInfo.Header.sizeActualPxl, bitmapInformation.imageInfo.Header.nStride, bitmapInformation.imageInfo.Data.rgData);
            if (flag)
            {
                bitmapInfo = bitmapInformation;
            }
            else
            {
                bitmapInfo = null;
                bitmapInformation.Dispose();
            }
            return flag;
        }

        public static bool FromRaw(
          IImage image,
          IntPtr buffer,
          int length,
          Size imageSize,
          int stride,
          SurfaceFormat format,
          Size maxSize,
          bool flipRTL,
          bool antialiasEdges,
          int borderWidth,
          ColorF borderColor,
          out BitmapInformation bitmapInfo)
        {
            Debug2.Validate(image != null, typeof(ArgumentNullException), "Image must be valid");
            Debug2.Validate(length > 0, typeof(ArgumentException), "Do not call for zero-length buffer");
            ImageRequirements req = new ImageRequirements();
            req.BorderWidth = borderWidth;
            req.BorderColor = borderColor;
            req.Flippable = flipRTL;
            req.AntialiasEdges = antialiasEdges;
            req.MaximumSize = maxSize;
            ExtensionsApi.BitmapOptions nOptions = ExtensionsApi.BitmapOptions.Decode;
            if (flipRTL)
                nOptions |= ExtensionsApi.BitmapOptions.Flip;
            BitmapInformation bitmapInformation = new BitmapInformation();
            if (!ExtensionsApi.SpBitmapLoadRaw(imageSize, stride, format, buffer, req, nOptions, out bitmapInformation.hBitmap, out bitmapInformation.imageInfo).IsSuccess())
            {
                bitmapInfo = null;
                return false;
            }
            bool flag = image.LoadContent(SurfaceFormatInfo.ToImageFormat(bitmapInformation.imageInfo.Header.nFormat), bitmapInformation.imageInfo.Header.sizeActualPxl, bitmapInformation.imageInfo.Header.nStride, bitmapInformation.imageInfo.Data.rgData);
            if (flag)
            {
                bitmapInfo = bitmapInformation;
            }
            else
            {
                bitmapInfo = null;
                bitmapInformation.Dispose();
            }
            return flag;
        }

        public static bool FromGradient(
          IImage image,
          Size sizeImage,
          ImageFormat format,
          ImageLoader.GradientValue[] horizontalRamp,
          ImageLoader.GradientValue[] verticalRamp,
          out GradientInformation gradientInfo)
        {
            Debug2.Validate(image != null, typeof(ArgumentException), "Image must be valid");
            Debug2.Validate(sizeImage.Width > 0 && sizeImage.Height > 0, typeof(ArgumentException), "Image size must be valid");
            Debug2.Validate(horizontalRamp != null || verticalRamp != null, typeof(ArgumentException), "Must provide at least one ramp");
            Debug2.Validate(format == ImageFormat.X8R8G8B8 || format == ImageFormat.A8R8G8B8 || format == ImageFormat.A8, typeof(ArgumentException), "Image format not supported");
            ValidateRamp(verticalRamp);
            ValidateRamp(horizontalRamp);
            SurfaceFormat nFormat = SurfaceFormatInfo.FromImageFormat(format);
            int num1 = SurfaceFormatInfo.GetBitsPerPixel(nFormat) / 8;
            int num2 = sizeImage.Width * num1;
            int length = num2 * num1 * sizeImage.Height;
            int num3 = 0;
            byte[] numArray = new byte[length];
            for (int index = 0; index < sizeImage.Height; ++index)
            {
                ColorF colorF1 = new ColorF(0, 0, 0, 0);
                if (verticalRamp != null)
                {
                    float flValue = sizeImage.Height > 1 ? index / (float)(sizeImage.Height - 1) : 0.0f;
                    colorF1 = SampleGradient(verticalRamp, flValue);
                    colorF1.Clamp();
                }
                int num4 = 0;
                int num5 = 0;
                while (num4 < sizeImage.Width)
                {
                    ColorF colorF2 = colorF1;
                    if (horizontalRamp != null)
                    {
                        float flValue = sizeImage.Width > 1 ? num4 / (float)(sizeImage.Width - 1) : 0.0f;
                        colorF2 += SampleGradient(horizontalRamp, flValue);
                        colorF2.Clamp();
                    }
                    switch (nFormat)
                    {
                        case SurfaceFormat.A8:
                            numArray[num3 + num5] = (byte)(colorF2.A * (double)byte.MaxValue);
                            break;
                        case SurfaceFormat.RGB32:
                            numArray[num3 + num5] = (byte)(colorF2.B * (double)byte.MaxValue);
                            numArray[num3 + num5 + 1] = (byte)(colorF2.G * (double)byte.MaxValue);
                            numArray[num3 + num5 + 2] = (byte)(colorF2.R * (double)byte.MaxValue);
                            numArray[num3 + num5 + 3] = byte.MaxValue;
                            break;
                        case SurfaceFormat.ARGB32:
                            numArray[num3 + num5] = (byte)(colorF2.B * (double)byte.MaxValue);
                            numArray[num3 + num5 + 1] = (byte)(colorF2.G * (double)byte.MaxValue);
                            numArray[num3 + num5 + 2] = (byte)(colorF2.R * (double)byte.MaxValue);
                            numArray[num3 + num5 + 3] = (byte)(colorF2.A * (double)byte.MaxValue);
                            break;
                        default:
                            Debug2.Throw(false, "Unsupported format type");
                            break;
                    }
                    ++num4;
                    num5 += num1;
                }
                num3 += num2;
            }
            GradientInformation gradientInformation = new GradientInformation()
            {
                imageInfo = {
          Header = {
            sizeActualPxl = sizeImage,
            sizeOriginalPxl = sizeImage,
            nStride = num2,
            nFormat = nFormat
          }
        },
                gcData = GCHandle.Alloc(numArray, GCHandleType.Pinned)
            };
            gradientInformation.imageInfo.Data.rgData = gradientInformation.gcData.AddrOfPinnedObject();
            bool flag = image.LoadContent(SurfaceFormatInfo.ToImageFormat(gradientInformation.imageInfo.Header.nFormat), gradientInformation.imageInfo.Header.sizeActualPxl, gradientInformation.imageInfo.Header.nStride, gradientInformation.imageInfo.Data.rgData);
            if (flag)
            {
                gradientInfo = gradientInformation;
            }
            else
            {
                gradientInfo = null;
                ((IDisposable)gradientInformation).Dispose();
            }
            return flag;
        }

        private static ColorF SampleGradient(
          ImageLoader.GradientValue[] rampValues,
          float flValue)
        {
            if (rampValues.Length == 1)
                return rampValues[0].value;
            int index = 0;
            ImageLoader.GradientValue rampValue1;
            do
            {
                rampValue1 = rampValues[index++];
            }
            while (index < rampValues.Length && flValue > (double)rampValues[index].position);
            if (rampValues.Length <= index)
                return rampValue1.value;
            ImageLoader.GradientValue rampValue2 = rampValues[index];
            float num = (float)((flValue - (double)rampValue1.position) / (rampValue2.position - (double)rampValue1.position));
            return rampValue1.value * (1f - num) + rampValue2.value * num;
        }

        private static void ValidateRamp(ImageLoader.GradientValue[] rampValues)
        {
            Debug2.Validate(rampValues == null || rampValues.Length > 0, typeof(ArgumentException), "Gradient must contain at least one sample point");
            if (rampValues == null)
                return;
            float num = 0.0f;
            for (int index = 0; index < rampValues.Length; ++index)
            {
                Debug2.Validate(rampValues[index].position >= (double)num, typeof(ArgumentException), "Gradient ramp values must be monotomically increasing");
                Debug2.Validate(rampValues[index].position >= 0.0 && rampValues[index].position <= 1.0, typeof(ArgumentException), "Gradient ramp values must be [0, 1]");
                num = rampValues[index].position;
            }
        }

        public struct GradientValue
        {
            public float position;
            public ColorF value;

            public GradientValue(float pos, ColorF val)
            {
                this.position = pos;
                this.value = val;
            }
        }
    }
}
