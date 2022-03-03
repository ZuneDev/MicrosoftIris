// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Drawing.RawImage
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.OS;
using Microsoft.Iris.Render;
using Microsoft.Iris.Render.Extensions;
using Microsoft.Iris.RenderAPI;
using Microsoft.Iris.Session;
using System;

namespace Microsoft.Iris.Drawing
{
    internal class RawImage : UIImage
    {
        private string _uniqueID;
        private Size _imageSize;
        private int _stride;
        private SurfaceFormat _format;
        private IntPtr _data;
        private uint _length;
        private RawImageItemKey _cacheItemKey;

        public RawImage(
          string uniqueID,
          Size imageSize,
          int stride,
          SurfaceFormat format,
          IntPtr data,
          bool takeOwnership,
          Inset nineGrid,
          Size maximumSize,
          bool flippable,
          bool antialiasEdges)
          : base(nineGrid, maximumSize, flippable, antialiasEdges)
        {
            Source = uniqueID;
            _uniqueID = uniqueID;
            _imageSize = imageSize;
            _stride = stride;
            _format = format;
            _contentSize = imageSize;
            int cbCopy = _imageSize.Height * Math.Abs(stride);
            _length = (uint)cbCopy;
            if (!takeOwnership)
            {
                _data = NativeApi.MemAlloc(_length, false);
                Memory.Copy(_data, data, cbCopy);
            }
            else
                _data = data;
        }

        ~RawImage()
        {
            FreeBuffer(_data);
            _data = IntPtr.Zero;
        }

        internal static void FreeBuffer(IntPtr data)
        {
            if (!(data != IntPtr.Zero))
                return;
            NativeApi.MemFree(data);
        }

        protected override ImageCacheItem GetCacheItem(out bool needAsyncLoad)
        {
            ImageCache instance = ScavengeImageCache.Instance;
            ImageCacheItem imageCacheItem = null;
            string str = GetHashCode().ToString();
            if (_cacheItemKey == null)
                _cacheItemKey = new RawImageItemKey(str);
            else
                imageCacheItem = instance.Lookup(_cacheItemKey);
            if (imageCacheItem == null)
            {
                Size maxSize = ClampSize(_maximumSize);
                imageCacheItem = new RawImageItem(UISession.Default.RenderSession, this, str, _data, _length, _imageSize, _stride, _format, maxSize, IsFlipped, _antialiasEdges);
                instance.Add(_cacheItemKey, imageCacheItem);
            }
            needAsyncLoad = false;
            return imageCacheItem;
        }

        protected override void EnsureSizeMetrics()
        {
        }
    }
}
