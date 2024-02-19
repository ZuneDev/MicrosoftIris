// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Drawing.UIImage
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;
using Microsoft.Iris.Markup;
using Microsoft.Iris.Render;
using Microsoft.Iris.Render.Extensions;
using Microsoft.Iris.Session;

namespace Microsoft.Iris.Drawing
{
    public abstract class UIImage : NotifyObjectBase, IStringEncodable
    {
        private static Size s_sizeMaximumSurface = new Size(-1, -1);
        protected string _source;
        private Inset _nineGrid;
        protected Size _maximumSize;
        protected bool _flippable;
        protected bool _antialiasEdges;
        private ImageStatus _status;
        protected Size _contentSize;

        public UIImage(Inset nineGrid, Size maximumSize, bool flippable, bool antialiasEdges)
        {
            _nineGrid = nineGrid;
            _maximumSize = maximumSize;
            _flippable = flippable;
            _antialiasEdges = antialiasEdges;
        }

        public void Load() => Load(false);

        private void Load(bool inDraw)
        {
            if (_status != ImageStatus.PendingLoad)
                return;
            bool needAsyncLoad;
            ImageCacheItem cacheItem = GetCacheItem(out needAsyncLoad);
            if (cacheItem == null)
                SetStatus(ImageStatus.Error);
            else if (needAsyncLoad)
            {
                SetStatus(ImageStatus.Loading);
                cacheItem.StartLoad();
            }
            else
                SetStatus(ImageStatus.Complete);
        }

        private ImageCacheItem GetCacheItem() => GetCacheItem(out bool _);

        protected abstract ImageCacheItem GetCacheItem(out bool needAsyncLoad);

        protected abstract void EnsureSizeMetrics();

        protected void OnHeaderLoadComplete()
        {
            if (LoadComplete == null)
                return;
            LoadComplete(this, Status);
        }

        protected void SetStatus(ImageStatus status)
        {
            _status = status;
            if ((_status == ImageStatus.Error || _status == ImageStatus.Complete) && LoadComplete != null)
                LoadComplete(this, _status);
            FireNotification(NotificationID.Status);
        }

        internal IImage RenderImage => GetCacheItem()?.RenderImage;

        public ImageStatus Status => _status;

        public string Source
        {
            get => _source;
            set
            {
                if (!(_source != value))
                    return;
                _source = value;
                OnImageAttributeChanged();
            }
        }

        public Inset NineGrid
        {
            get => _nineGrid;
            set
            {
                if (!(_nineGrid != value))
                    return;
                _nineGrid = value;
                OnImageAttributeChanged();
            }
        }

        public Size MaximumSize
        {
            get => _maximumSize;
            set
            {
                if (!(_maximumSize != value))
                    return;
                _maximumSize = value;
                OnImageAttributeChanged();
            }
        }

        public bool Flippable
        {
            get => _flippable;
            set
            {
                if (_flippable == value)
                    return;
                _flippable = value;
                OnImageAttributeChanged();
            }
        }

        public bool IsFlipped => _flippable && UISession.Default.IsRtl;

        public bool AntialiasEdges
        {
            get => _antialiasEdges;
            set
            {
                if (_antialiasEdges == value)
                    return;
                _antialiasEdges = value;
                OnImageAttributeChanged();
            }
        }

        internal Size Size
        {
            get
            {
                EnsureSizeMetrics();
                return _contentSize;
            }
        }

        public int Width
        {
            get
            {
                EnsureSizeMetrics();
                return _contentSize.Width;
            }
        }

        public int Height
        {
            get
            {
                EnsureSizeMetrics();
                return _contentSize.Height;
            }
        }

        internal void AddUser(object user) => GetCacheItem()?.RegisterUsage(user);

        public void RemoveUser(object user) => GetCacheItem()?.UnregisterUsage(user);

        internal static Size MaximumSurfaceSize(UISession session)
        {
            if (s_sizeMaximumSurface.Width == -1)
                s_sizeMaximumSurface = session.RenderSession.GraphicsDevice.MaximumImageSize;
            return s_sizeMaximumSurface;
        }

        internal static Size ClampSize(Size maxSizeImage)
        {
            Size size = MaximumSurfaceSize(UISession.Default);
            if (maxSizeImage.Width == 0 || maxSizeImage.Width > size.Width)
                maxSizeImage.Width = size.Width;
            if (maxSizeImage.Height == 0 || maxSizeImage.Height > size.Height)
                maxSizeImage.Height = size.Height;
            return maxSizeImage;
        }

        protected virtual void OnImageAttributeChanged()
        {
        }

        public event ContentLoadCompleteHandler LoadComplete;

        public override string ToString() => $"{{UIImage {RenderImage?.ToString() ?? "null"}}}";

        public string EncodeString()
        {
            System.Collections.Generic.List<string> props = new(5)
            {
                Source,
            };

            if (NineGrid != default)
                props.Add(NineGrid.EncodeString());
            if (MaximumSize != default)
                props.Add(MaximumSize.EncodeString());
            if (Flippable != default)
                props.Add(Flippable.ToString());
            if (AntialiasEdges != default)
                props.Add(AntialiasEdges.ToString());

            return string.Join(", ", props);
        }
    }
}
