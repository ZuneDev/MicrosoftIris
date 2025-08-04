// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.ViewItems.Graphic
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Drawing;
using Microsoft.Iris.Layout;
using Microsoft.Iris.Layouts;
using Microsoft.Iris.Library;
using Microsoft.Iris.Markup;
using Microsoft.Iris.Render;
using Microsoft.Iris.RenderAPI.Drawing;
using Microsoft.Iris.UI;

namespace Microsoft.Iris.ViewItems
{
    internal class Graphic : ContentViewItem
    {
        private const string s_effectElementName = "BaseImage";
        private const string s_effectPropertyName = "BaseImage.Image";
        private static readonly DataCookie s_pendingLoadCompleteHandlerProperty = DataCookie.ReserveSlot();
        private static readonly DataCookie s_acquiringImageProperty = DataCookie.ReserveSlot();
        private static readonly DataCookie s_errorImageProperty = DataCookie.ReserveSlot();
        private static string s_AcquiringDefaultImageUri = "clr-res://UIX!AcquiringImage.png";
        private static string s_ErrorDefaultImageUri = "clr-res://UIX!ErrorImage.png";
        private static UIImage s_AcquiringDefaultImage;
        private static UIImage s_ErrorDefaultImage;
        private static object s_NullImage = new object();
        private UIImage _contentImage;
        private UIImage _preloadImage;
        private StripAlignment _horizontalAlignment;
        private StripAlignment _verticalAlignment;
        private StretchingPolicy _stretchMode;

        public Graphic()
        {
            _horizontalAlignment = StripAlignment.Near;
            _verticalAlignment = StripAlignment.Near;
            _stretchMode = StretchingPolicy.Fill;
            SizingPolicy = SizingPolicy.SizeToContent;
        }

        public static void EnsureFallbackImages()
        {
            if (s_AcquiringDefaultImage != null)
                return;
            s_AcquiringDefaultImage = new UriImage(s_AcquiringDefaultImageUri, new Inset(3, 3, 53, 53), Size.Zero, false);
            s_AcquiringDefaultImage.Load();
            s_ErrorDefaultImage = new UriImage(s_ErrorDefaultImageUri, new Inset(3, 3, 53, 53), Size.Zero, false);
            s_ErrorDefaultImage.Load();
        }

        protected override void OnDispose()
        {
            ReleaseInUseImage(_contentImage, null);
            ReleaseInUseImage(_preloadImage, null);
            ReleaseInUseImage(AcquiringImage, s_AcquiringDefaultImage);
            ReleaseInUseImage(ErrorImage, s_ErrorDefaultImage);
            _preloadImage = null;
            _contentImage = null;
            AsyncLoadCompleteHandler = null;
            base.OnDispose();
        }

        protected override void OnEffectChanged()
        {
            if (_contents == null)
                return;
            _contents.Effect = null;
        }

        public UIImage Content
        {
            get => _contentImage;
            set
            {
                if (_contentImage == value)
                    return;
                if (_contentImage != null)
                {
                    RemoveAsyncLoadCompleteHandler(_contentImage);
                    _contentImage.RemoveUser(this);
                }
                _contentImage = value;
                if (_contentImage != null)
                {
                    _contentImage.AddUser(this);
                    _contentImage.Load();
                    if (_contentImage.Status == ImageStatus.Loading || _contentImage.Status == ImageStatus.PendingLoad)
                        AttachAsyncLoadCompleteHandler(_contentImage);
                }
                FireNotification(NotificationID.Content);
                NotifyContentChange();
            }
        }

        public UIImage AcquiringImage
        {
            get => GetStatusImage(s_acquiringImageProperty, s_AcquiringDefaultImage);
            set => SetStatusImage(value, NotificationID.AcquiringImage, s_acquiringImageProperty, s_AcquiringDefaultImage);
        }

        public UIImage ErrorImage
        {
            get => GetStatusImage(s_errorImageProperty, s_ErrorDefaultImage);
            set => SetStatusImage(value, NotificationID.ErrorImage, s_errorImageProperty, s_ErrorDefaultImage);
        }

        public UIImage PreloadContent
        {
            get => _preloadImage;
            set
            {
                if (_preloadImage != null)
                    _preloadImage.RemoveUser(this);
                _preloadImage = value;
                if (_preloadImage == null)
                    return;
                _preloadImage.AddUser(this);
                _preloadImage.Load();
            }
        }

        private UIImage GetStatusImage(DataCookie cookie, UIImage defaultImage)
        {
            object data = GetData(cookie);
            return data != null ? (data != s_NullImage ? (UIImage)data : null) : defaultImage;
        }

        private void SetStatusImage(
          UIImage value,
          string name,
          DataCookie cookie,
          UIImage defaultImage)
        {
            UIImage statusImage = GetStatusImage(cookie, defaultImage);
            if (value == statusImage)
                return;
            object obj;
            if (value != null)
            {
                obj = value;
                value.Load();
                if (value.Status == ImageStatus.Loading || value.Status == ImageStatus.PendingLoad)
                    AttachAsyncLoadCompleteHandler(value);
                value.AddUser(this);
            }
            else
                obj = s_NullImage;
            ReleaseInUseImage(statusImage, defaultImage);
            SetData(cookie, obj);
            MarkPaintInvalid();
            FireNotification(name);
        }

        private void OnAsyncLoadComplete(object image, ImageStatus status)
        {
            RemoveAsyncLoadCompleteHandler((UIImage)image);
            NotifyContentChange();
        }

        private void AttachAsyncLoadCompleteHandler(UIImage image)
        {
            ContentLoadCompleteHandler loadCompleteHandler = AsyncLoadCompleteHandler;
            if (loadCompleteHandler == null)
            {
                loadCompleteHandler = new ContentLoadCompleteHandler(OnAsyncLoadComplete);
                AsyncLoadCompleteHandler = loadCompleteHandler;
            }
            image.LoadComplete += loadCompleteHandler;
        }

        private void RemoveAsyncLoadCompleteHandler(UIImage image)
        {
            if (image == null)
                return;
            image.LoadComplete -= AsyncLoadCompleteHandler;
        }

        private void ReleaseInUseImage(UIImage image, UIImage defaultImage)
        {
            if (image != null && image != defaultImage && (image.Status == ImageStatus.Loading || image.Status == ImageStatus.PendingLoad))
                RemoveAsyncLoadCompleteHandler(image);
            if (image == null || image == defaultImage)
                return;
            image.RemoveUser(this);
        }

        private void NotifyContentChange()
        {
            ForceContentChange();
            if (Layout is ImageLayout layout && UpdateSourceOnLayout(layout))
                MarkLayoutInvalid();
            MarkPaintInvalid();
        }

        protected override void OnMinimumSizeChanged()
        {
            if (!(Layout is ImageLayout layout))
                return;
            layout.MinimumSize = MinimumSize;
        }

        public SizingPolicy SizingPolicy
        {
            get
            {
                if (!(Layout is ImageLayout layout))
                    return SizingPolicy.SizeToChildren;
                return layout.Fill ? SizingPolicy.SizeToConstraint : SizingPolicy.SizeToContent;
            }
            set
            {
                if (value == SizingPolicy)
                    return;
                if (value == SizingPolicy.SizeToChildren)
                {
                    Layout = DefaultLayout.Instance;
                }
                else
                {
                    ImageLayout imageLayout = new ImageLayout();
                    UpdateSourceOnLayout(imageLayout);
                    UpdateMaintainAspectRatioOnLayout(imageLayout);
                    imageLayout.MinimumSize = MinimumSize;
                    imageLayout.Fill = value == SizingPolicy.SizeToConstraint;
                    Layout = imageLayout;
                }
                FireNotification(NotificationID.SizingPolicy);
            }
        }

        public StretchingPolicy StretchingPolicy
        {
            get => _stretchMode;
            set
            {
                if (_stretchMode == value)
                    return;
                _stretchMode = value;
                if (UpdateMaintainAspectRatioOnLayout(Layout as ImageLayout))
                    MarkLayoutInvalid();
                MarkPaintInvalid();
                FireNotification(NotificationID.StretchingPolicy);
            }
        }

        private bool UpdateMaintainAspectRatioOnLayout(ImageLayout imageLayout)
        {
            bool flag1 = false;
            if (imageLayout != null)
            {
                bool flag2 = _stretchMode == StretchingPolicy.Uniform;
                if (imageLayout.MaintainAspectRatio != flag2)
                {
                    imageLayout.MaintainAspectRatio = flag2;
                    flag1 = true;
                }
            }
            return flag1;
        }

        private bool UpdateSourceOnLayout(ImageLayout layout)
        {
            bool flag = false;
            Size size = Size.Zero;
            if (_contentImage != null)
            {
                UIImage activeImage = GetActiveImage();
                if (activeImage != null)
                    size = activeImage.Size;
            }
            if (layout.SourceSize != size)
            {
                layout.SourceSize = size;
                flag = true;
            }
            return flag;
        }

        public StripAlignment HorizontalAlignment
        {
            get => _horizontalAlignment;
            set
            {
                if (_horizontalAlignment == value)
                    return;
                _horizontalAlignment = value;
                MarkPaintInvalid();
                FireNotification(NotificationID.HorizontalAlignment);
            }
        }

        public StripAlignment VerticalAlignment
        {
            get => _verticalAlignment;
            set
            {
                if (_verticalAlignment == value)
                    return;
                _verticalAlignment = value;
                MarkPaintInvalid();
                FireNotification(NotificationID.VerticalAlignment);
            }
        }

        private ContentLoadCompleteHandler AsyncLoadCompleteHandler
        {
            get => (ContentLoadCompleteHandler)GetData(s_pendingLoadCompleteHandlerProperty);
            set => SetData(s_pendingLoadCompleteHandlerProperty, value);
        }

        public void CommitPreload() => Content = PreloadContent;

        protected override bool HasContent() => _contentImage != null;

        protected override void OnPaint(bool visible)
        {
            base.OnPaint(visible);
            if (_contents == null)
                return;
            if (_contents.Effect == null)
            {
                _contents.Effect = EffectClass.CreateImageRenderEffectWithFallback(Effect, this, null);
                _contents.Effect.UnregisterUsage(this);
            }
            UpdateEffectContents();
            UpdateCoordinateMaps();
        }

        private void UpdateEffectContents() => EffectClass.SetDefaultEffectProperty(Effect, _contents.Effect, GetActiveImage()?.RenderImage);

        internal void UpdateCoordinateMaps()
        {
            UIImage activeImage = GetActiveImage();
            if (activeImage == null)
                return;
            if (_stretchMode == StretchingPolicy.Fill && activeImage.NineGrid != Inset.Zero)
            {
                _contents.RelativeSize = true;
                _contents.Size = Vector2.UnitVector;
                _contents.SetNineGrid(activeImage.NineGrid.Left, activeImage.NineGrid.Top, activeImage.NineGrid.Right, activeImage.NineGrid.Bottom);
            }
            else
            {
                Vector2 visualSize = VisualSize;
                Size layoutSize = new Size((int)visualSize.X, (int)visualSize.Y);
                Size size = activeImage.Size;
                RectangleF source;
                RectangleF destination;
                CalculatePaintRectangles(size, layoutSize, out source, out destination);
                switch (size.IsEmpty ? 1 : (int)_stretchMode)
                {
                    case 0:
                    case 2:
                    case 3:
                        RectangleF rectangleF = new RectangleF(Point.Zero, size);
                        CoordMap coordMap = null;
                        if (source != rectangleF)
                        {
                            float flValue1 = source.Left / size.Width;
                            float flValue2 = source.Right / size.Width;
                            float flValue3 = source.Top / size.Height;
                            float flValue4 = source.Bottom / size.Height;
                            coordMap = new CoordMap();
                            coordMap.AddValue(0.0f, flValue1, Orientation.Horizontal);
                            coordMap.AddValue(0.0f, flValue3, Orientation.Vertical);
                            coordMap.AddValue(1f, flValue2, Orientation.Horizontal);
                            coordMap.AddValue(1f, flValue4, Orientation.Vertical);
                        }
                        _contents.RelativeSize = false;
                        _contents.Position = new Vector3(destination.Left, destination.Top, 0.0f);
                        _contents.Size = new Vector2(destination.Width, destination.Height);
                        _contents.SetCoordMap(0, coordMap);
                        break;
                    case 1:
                        _contents.RelativeSize = true;
                        _contents.Size = Vector2.UnitVector;
                        _contents.SetCoordMap(0, null);
                        break;
                }
            }
        }

        private UIImage GetActiveImage() => _contentImage.Status != ImageStatus.Complete ? (_contentImage.Status == ImageStatus.Loading || _contentImage.Status == ImageStatus.PendingLoad ? AcquiringImage : ErrorImage) : _contentImage;

        private void CalculatePaintRectangles(
          Size originalSourceSize,
          Size layoutSize,
          out RectangleF source,
          out RectangleF destination)
        {
            if (_stretchMode == StretchingPolicy.Fill)
            {
                source = RectangleF.Zero;
                destination = RectangleF.Zero;
            }
            else
            {
                Size size1 = originalSourceSize;
                Size size2 = layoutSize;
                if (size1.IsZero || size2.IsZero)
                {
                    source = RectangleF.Zero;
                    destination = RectangleF.Zero;
                }
                else
                {
                    switch (_stretchMode)
                    {
                        case StretchingPolicy.None:
                            size1 = Size.Min(size1, size2);
                            size2 = size1;
                            break;
                        case StretchingPolicy.Uniform:
                            size2 = Size.LargestFit(size1, size2);
                            break;
                        case StretchingPolicy.UniformToFill:
                            size1 = Size.LargestFit(size2, size1);
                            break;
                    }
                    float dimensionOffset1 = CalculateDimensionOffset(size1.Width, originalSourceSize.Width, _horizontalAlignment);
                    float dimensionOffset2 = CalculateDimensionOffset(size2.Width, layoutSize.Width, _horizontalAlignment);
                    float dimensionOffset3 = CalculateDimensionOffset(size1.Height, originalSourceSize.Height, _verticalAlignment);
                    float dimensionOffset4 = CalculateDimensionOffset(size2.Height, layoutSize.Height, _verticalAlignment);
                    source = new RectangleF(dimensionOffset1, dimensionOffset3, size1.Width, size1.Height);
                    destination = new RectangleF(dimensionOffset2, dimensionOffset4, size2.Width, size2.Height);
                }
            }
        }

        private float CalculateDimensionOffset(
          float actualSize,
          float availableSize,
          StripAlignment alignment)
        {
            float num = availableSize - actualSize;
            switch (alignment)
            {
                case StripAlignment.Near:
                    return 0.0f;
                case StripAlignment.Center:
                    return num / 2f;
                case StripAlignment.Far:
                    return num;
                default:
                    return 0.0f;
            }
        }
    }
}
