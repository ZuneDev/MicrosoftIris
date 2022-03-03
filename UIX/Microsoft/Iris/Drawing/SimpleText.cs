// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Drawing.SimpleText
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.OS;
using Microsoft.Iris.Render;
using Microsoft.Iris.RenderAPI;
using Microsoft.Iris.Session;
using Microsoft.Iris.ViewItems;
using System;

namespace Microsoft.Iris.Drawing
{
    internal class SimpleText : IDisposable
    {
        private Win32Api.HANDLE _stoHandle;

        public SimpleText()
        {
            Size sizeMaximumSurface = Size.Zero;
            if (UISession.Default != null)
                sizeMaximumSurface = UIImage.MaximumSurfaceSize(UISession.Default);
            RendererApi.IFC(NativeApi.SpSimpleTextBuildObject(sizeMaximumSurface, out _stoHandle));
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            NativeApi.SpSimpleTextDestroyObject(_stoHandle);
            _stoHandle = Win32Api.HANDLE.NULL;
        }

        public unsafe bool CanMeasure(string content, TextStyle textStyle)
        {
            bool fPossible;
            fixed (char* chPtr = textStyle.FontFace)
            {
                var style = new TextStyle.MarshalledData(textStyle)
                {
                    _fontFace = chPtr
                };
                RendererApi.IFC(NativeApi.SpSimpleTextMeasurePossible(_stoHandle, content, &style, out fPossible));
            }
            return fPossible;
        }

        public unsafe TextFlow Measure(
          string content,
          LineAlignment alignment,
          TextStyle textStyle,
          Size constraint)
        {
            TextFlow textFlow = new TextFlow();
            if (content == null)
                content = string.Empty;
            short wAlignment = 0;
            switch (alignment)
            {
                case LineAlignment.Near:
                    wAlignment = 1;
                    break;
                case LineAlignment.Center:
                    wAlignment = 3;
                    break;
                case LineAlignment.Far:
                    wAlignment = 2;
                    break;
            }
            IntPtr hGlyphRunInfo;
            NativeApi.RasterizeRunPacket rasterizeRunPacket;
            fixed (char* chPtr = textStyle.FontFace)
            {
                var style = new TextStyle.MarshalledData(textStyle)
                {
                    _fontFace = chPtr
                };
                RendererApi.IFC(NativeApi.SpSimpleTextMeasure(_stoHandle, content, wAlignment,
                    &style, constraint, out hGlyphRunInfo, &rasterizeRunPacket));
            }
            TextRun run = TextRun.FromRunPacket(hGlyphRunInfo, &rasterizeRunPacket, content);
            textFlow.Add(run);
            return textFlow;
        }
    }
}
