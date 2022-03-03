// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Drawing.RawImageItem
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Render;
using Microsoft.Iris.Render.Extensions;
using System;

namespace Microsoft.Iris.Drawing
{
    internal class RawImageItem : ImageCacheItem
    {
        private int _stride;
        private SurfaceFormat _format;
        private object _oKeepAlive;

        internal RawImageItem(
          IRenderSession renderSession,
          RawImage rawImage,
          string source,
          IntPtr data,
          uint length,
          Size imageSize,
          int stride,
          SurfaceFormat format,
          Size maxSize,
          bool flippable,
          bool antialiasEdges)
          : base(renderSession, source, maxSize, flippable, antialiasEdges)
        {
            _oKeepAlive = rawImage;
            SetSize(imageSize);
            SetBuffer(data, length);
            _stride = stride;
            _format = format;
        }

        protected override void OnDispose()
        {
            _oKeepAlive = null;
            m_buffer = IntPtr.Zero;
            base.OnDispose();
        }

        protected override bool DoImageLoad() => ImageLoader.FromRaw(RenderImage, m_buffer, (int)m_length, m_size, _stride, _format, m_req.MaximumSize, m_req.Flippable, m_req.AntialiasEdges, m_req.BorderWidth, m_req.BorderColor, out m_info);
    }
}
