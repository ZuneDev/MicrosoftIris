// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Image
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Drawing;
using Microsoft.Iris.Markup;
using Microsoft.Iris.Render;
using Microsoft.Iris.Session;
using System;

namespace Microsoft.Iris
{
    public class Image : AssemblyObjectProxyHelper.IFrameworkProxyObject
    {
        private UIImage _uiImage;
        private ImageLoadCompleteHandler _completeHandlers;

        public Image(string source)
          : this(source, 0, 0, false)
        {
        }

        public Image(string source, ImageInset imageInset)
          : this(source, imageInset.ToInset(), 0, 0, false, false)
        {
        }

        public Image(string source, int maximumWidth, int maximumHeight)
          : this(source, maximumWidth, maximumHeight, false)
        {
        }

        public Image(string source, int maximumWidth, int maximumHeight, bool flippable)
          : this(source, maximumWidth, maximumHeight, flippable, false)
        {
        }

        public Image(
          string source,
          int maximumWidth,
          int maximumHeight,
          bool flippable,
          bool antialiasEdges)
          : this(source, Inset.Zero, maximumWidth, maximumHeight, flippable, antialiasEdges)
        {
        }

        internal Image(
          string source,
          Inset inset,
          int maximumWidth,
          int maximumHeight,
          bool flippable,
          bool antialiasEdges)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            _uiImage = new UriImage(source, inset, new Size(maximumWidth, maximumHeight), flippable, antialiasEdges);
        }

        public Image(
          string uniqueID,
          int imageWidth,
          int imageHeight,
          int stride,
          RawImageFormat format,
          IntPtr data)
          : this(uniqueID, imageWidth, imageHeight, stride, format, data, 0, 0, false)
        {
        }

        public Image(
          string uniqueID,
          int imageWidth,
          int imageHeight,
          int stride,
          RawImageFormat format,
          IntPtr data,
          int maximumWidth,
          int maximumHeight,
          bool flippable)
          : this(uniqueID, imageWidth, imageHeight, stride, format, data, maximumWidth, maximumHeight, flippable, false)
        {
        }

        public Image(
          string uniqueID,
          int imageWidth,
          int imageHeight,
          int stride,
          RawImageFormat format,
          IntPtr data,
          int maximumWidth,
          int maximumHeight,
          bool flippable,
          bool anitaliasEdges)
        {
            SurfaceFormat surfaceFormat;
            if (!ImageFormatUtils.RawImageFormatToSurfaceFormat(format, out surfaceFormat))
                throw new ArgumentException(nameof(format));
            uniqueID = "RAW:" + uniqueID;
            _uiImage = new RawImage(uniqueID, new Size(imageWidth, imageHeight), stride, surfaceFormat, data, false, Inset.Zero, new Size(maximumWidth, maximumHeight), flippable, anitaliasEdges);
        }

        public string Source => _uiImage.Source;

        public bool Load()
        {
            UIDispatcher.VerifyOnApplicationThread();
            _uiImage.Load();
            return _uiImage.Status == ImageStatus.Loading;
        }

        public event ImageLoadCompleteHandler ImageLoadComplete
        {
            add
            {
                UIDispatcher.VerifyOnApplicationThread();
                if (_uiImage.Status == ImageStatus.PendingLoad)
                    throw new InvalidOperationException("Image Load event cannot be used before Load is called");
                if (_completeHandlers == null)
                    _uiImage.LoadComplete += new ContentLoadCompleteHandler(OnContentLoadComplete);
                _completeHandlers += value;
            }
            remove
            {
                UIDispatcher.VerifyOnApplicationThread();
                if (_uiImage.Status == ImageStatus.PendingLoad)
                    throw new InvalidOperationException("Image Load event cannot be used before Load is called");
                _completeHandlers -= value;
                if (_completeHandlers != null)
                    return;
                _uiImage.LoadComplete -= new ContentLoadCompleteHandler(OnContentLoadComplete);
            }
        }

        public int Width
        {
            get
            {
                UIDispatcher.VerifyOnApplicationThread();
                return _uiImage.Width;
            }
        }

        public int Height
        {
            get
            {
                UIDispatcher.VerifyOnApplicationThread();
                return _uiImage.Height;
            }
        }

        public static void RemoveCache(string source) => RemoveCache(source, 0, 0, false, false);

        public static void RemoveCache(string source, int maximumWidth, int maximumHeight) => RemoveCache(source, maximumWidth, maximumHeight, false, false);

        public static void RemoveCache(
          string source,
          int maximumWidth,
          int maximumHeight,
          bool flippable)
        {
            RemoveCache(source, maximumWidth, maximumHeight, flippable, false);
        }

        public static void RemoveCache(
          string source,
          int maximumWidth,
          int maximumHeight,
          bool flippable,
          bool antialiasEdges)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            UriImage.RemoveCache(source, new Size(maximumWidth, maximumHeight), flippable, antialiasEdges);
        }

        private void OnContentLoadComplete(object owner, ImageStatus status) => _completeHandlers(this);

        internal UIImage UIImage
        {
            get
            {
                UIDispatcher.VerifyOnApplicationThread();
                return _uiImage;
            }
        }

        object AssemblyObjectProxyHelper.IFrameworkProxyObject.FrameworkObject => _uiImage;
    }
}
