// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Debug.DebugOutlines
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Drawing;
using Microsoft.Iris.Markup;
using Microsoft.Iris.OS;
using Microsoft.Iris.Render;
using Microsoft.Iris.RenderAPI.Drawing;
using Microsoft.Iris.Session;
using Microsoft.Iris.UI;
using Microsoft.Iris.ViewItems;
using System;

namespace Microsoft.Iris.Debug
{
    internal class DebugOutlines : ViewItem
    {
        private const int TotalScopeModes = 4;
        private const int TotalLabelModes = 3;
        private static bool _enabled;
        private ViewItem _rootItem;
        private DebugOutlineScope _scope;
        private DebugLabelFormat _label;
        private RichText _sharedRichText;
        private Color _outlineColor;
        private Color _hostOutlineColor;
        private Color _textColor;
        private Font _textFont;
        private UIImage _mouseInteractiveImage;
        private UIImage _mouseFocusImage;
        private UIImage _keyInteractiveImage;
        private UIImage _keyFocusImage;
        private int _thickness = 1;
        private Map<object, RawImage> _outlineImageEffectCache = new Map<object, RawImage>();
        private static DebugOutlines.SomethingChangedHandler SomethingChanged;

        public DebugOutlines()
        {
            _sharedRichText = new RichText(false);
            _textFont = new Font("Arial", 30f);
            _outlineColor = Color.White;
            _hostOutlineColor = Color.White;
            _textColor = Color.Black;
            SomethingChanged += new DebugOutlines.SomethingChangedHandler(OnSomethingChanged);
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            SomethingChanged -= new DebugOutlines.SomethingChangedHandler(OnSomethingChanged);
            _sharedRichText.Dispose();
        }

        public ViewItem Root
        {
            get => _rootItem;
            set
            {
                if (_rootItem == value)
                    return;
                _rootItem = value;
                FireNotification(NotificationID.Root);
                MarkPaintInvalid();
            }
        }

        public static bool Enabled
        {
            get => _enabled;
            set
            {
                if (_enabled == value)
                    return;
                _enabled = value;
                SomethingChanged(null);
            }
        }

        public DebugOutlineScope OutlineScope
        {
            get => _scope;
            set
            {
                if (_scope == value)
                    return;
                _scope = value;
                FireNotification(NotificationID.OutlineScope);
                MarkPaintInvalid();
            }
        }

        public void NextScopeMode() => OutlineScope = (DebugOutlineScope)((int)(_scope + 1) % 4);

        public DebugLabelFormat OutlineLabel
        {
            get => _label;
            set
            {
                if (_label == value)
                    return;
                _label = value;
                FireNotification(NotificationID.OutlineLabel);
                MarkPaintInvalid();
            }
        }

        public void NextLabelMode() => OutlineLabel = (DebugLabelFormat)((int)(_label + 1) % 3);

        public Color OutlineColor
        {
            get => _outlineColor;
            set
            {
                if (!(_outlineColor != value))
                    return;
                _outlineColor = value;
                FireNotification(NotificationID.OutlineColor);
                MarkPaintInvalid();
            }
        }

        public Color HostOutlineColor
        {
            get => _hostOutlineColor;
            set
            {
                if (!(_hostOutlineColor != value))
                    return;
                _hostOutlineColor = value;
                FireNotification(NotificationID.HostOutlineColor);
                MarkPaintInvalid();
            }
        }

        public Color TextColor
        {
            get => _textColor;
            set
            {
                if (!(_textColor != value))
                    return;
                _textColor = value;
                FireNotification(NotificationID.TextColor);
                MarkPaintInvalid();
            }
        }

        public Font TextFont
        {
            get => _textFont;
            set
            {
                if (_textFont == value)
                    return;
                _textFont = value;
                FireNotification(NotificationID.TextFont);
                MarkPaintInvalid();
            }
        }

        public UIImage MouseInteractiveImage
        {
            get => _mouseInteractiveImage;
            set
            {
                if (_mouseInteractiveImage == value)
                    return;
                _mouseInteractiveImage = value;
                FireNotification(NotificationID.MouseInteractiveImage);
                MarkPaintInvalid();
            }
        }

        public UIImage MouseFocusImage
        {
            get => _mouseFocusImage;
            set
            {
                if (_mouseFocusImage == value)
                    return;
                _mouseFocusImage = value;
                FireNotification(NotificationID.MouseFocusImage);
                MarkPaintInvalid();
            }
        }

        public UIImage KeyInteractiveImage
        {
            get => _keyInteractiveImage;
            set
            {
                if (_keyInteractiveImage == value)
                    return;
                _keyInteractiveImage = value;
                FireNotification(NotificationID.KeyInteractiveImage);
                MarkPaintInvalid();
            }
        }

        public UIImage KeyFocusImage
        {
            get => _keyFocusImage;
            set
            {
                if (_keyFocusImage == value)
                    return;
                _keyFocusImage = value;
                FireNotification(NotificationID.KeyFocusImage);
                MarkPaintInvalid();
            }
        }

        protected override void OnPaint(bool visible)
        {
            base.OnPaint(visible);
            VisualContainer.RemoveAllChildren();
            if (!_enabled || _rootItem == null || !_rootItem.HasVisual)
                return;
            SizeF extentPtr = new SizeF(_rootItem.VisualSize.X, _rootItem.VisualSize.Y);
            PaintWorker(_rootItem, PointF.Zero, extentPtr);
        }

        private void PaintWorker(ViewItem vi, PointF offsetPointF, SizeF extentPtr)
        {
            if (vi == this)
                return;
            PaintOutline(vi, offsetPointF, extentPtr);
            foreach (ViewItem child in vi.Children)
            {
                if (child.HasVisual)
                {
                    PointF offsetPointF1 = offsetPointF;
                    offsetPointF1.X += child.VisualPosition.X;
                    offsetPointF1.Y += child.VisualPosition.Y;
                    SizeF extentPtr1 = new SizeF(child.VisualSize.X, child.VisualSize.Y);
                    PaintWorker(child, offsetPointF1, extentPtr1);
                }
            }
        }

        private void PaintOutline(ViewItem vi, PointF offsetPointF, SizeF extentPtr)
        {
            Color outlineColor;
            string name;
            UIImage[] iconsList;
            if (!ShouldPaintOutlines(vi, out outlineColor, out name, out iconsList))
                return;
            RawImage outlineImage = CreateOutlineImage(outlineColor);
            DrawImage(outlineImage.RenderImage, offsetPointF, extentPtr, outlineImage.NineGrid);
            IImage image = null;
            RectangleF rectangle = RectangleF.Zero;
            float num = 0.0f;
            if (name != null)
            {
                TextMeasureParams measureParams = new TextMeasureParams();
                measureParams.Initialize();
                measureParams.SetConstraint(new SizeF(4095f, 8191f));
                measureParams.SetFormat(LineAlignment.Near, new TextStyle()
                {
                    FontFace = _textFont.FontName,
                    FontSize = _textFont.FontSize,
                    Color = _textColor
                });
                TextFlow textFlow = _sharedRichText.Measure(name, ref measureParams);
                measureParams.Dispose();
                TextRun run = textFlow[0];
                image = Text.GetImageForRun(UISession.Default, run, Color.FromArgb(byte.MaxValue, run.Color));
                rectangle = RectangleF.FromRectangle(textFlow.Bounds);
                rectangle.Offset(offsetPointF);
                num = textFlow.Bounds.Width;
            }
            if (iconsList != null)
            {
                foreach (UIImage uiImage in iconsList)
                {
                    if (uiImage != null)
                    {
                        uiImage.Load();
                        rectangle.Width += uiImage.Width;
                        rectangle.Height = Math.Max(rectangle.Height, uiImage.Height);
                    }
                }
            }
            rectangle.Y -= rectangle.Height;
            if (name != null || iconsList != null)
                DrawEffect(EffectManager.CreateColorFillEffect(this, outlineColor), rectangle);
            if (image != null)
            {
                SizeF size = new SizeF(image.Size.Width, image.Size.Height);
                DrawImage(image, rectangle.Location, size);
            }
            if (iconsList == null)
                return;
            foreach (UIImage uiImage in iconsList)
            {
                if (uiImage != null)
                {
                    DrawImage(uiImage.RenderImage, new RectangleF(rectangle.X + num, rectangle.Y, uiImage.Width, uiImage.Height));
                    num += uiImage.Width;
                }
            }
        }

        private void DrawImage(IImage image, RectangleF rectangle) => DrawImage(image, rectangle.Location, rectangle.Size);

        private void DrawImage(IImage image, PointF position, SizeF size) => DrawImage(image, position, size, Inset.Zero);

        private void DrawImage(IImage image, PointF position, SizeF size, Inset nineGrid) => DrawEffect(EffectManager.CreateBasicImageEffect(this, image), position, size, nineGrid);

        private void DrawEffect(IEffect effect, RectangleF rectangle) => DrawEffect(effect, rectangle.Location, rectangle.Size, Inset.Zero);

        private void DrawEffect(IEffect effect, PointF position, SizeF size, Inset nineGrid)
        {
            ISprite sprite = UISession.RenderSession.CreateSprite(this, this);
            sprite.Effect = effect;
            sprite.Effect.UnregisterUsage(this);
            sprite.Position = new Vector3(position.X, position.Y, 0.0f);
            sprite.Size = new Vector2(size.Width, size.Height);
            if (nineGrid != Inset.Zero)
                sprite.SetNineGrid(nineGrid.Left, nineGrid.Top, nineGrid.Right, nineGrid.Bottom);
            VisualContainer.AddChild(sprite, null, VisualOrder.First);
            sprite.UnregisterUsage(this);
        }

        private unsafe RawImage CreateOutlineImage(Color outlineColor)
        {
            RawImage rawImage;
            _outlineImageEffectCache.TryGetValue(outlineColor, out rawImage);
            if (rawImage == null)
            {
                int num1 = _thickness + 1;
                int num2 = num1 * 2 + 1;
                IntPtr data = NativeApi.MemAlloc((uint)(num2 * num2 * 4), true);
                int* pointer = (int*)data.ToPointer();
                for (int index = 0; index < num2 * num2; ++index)
                {
                    int num3 = index / num2;
                    int num4 = index % num2;
                    if (num3 < _thickness || num3 >= num2 - _thickness || (num4 < _thickness || num4 >= num2 - _thickness))
                        pointer[index] = outlineColor.ToArgb();
                }
                rawImage = new RawImage("OutlineImage" + outlineColor, new Size(num2, num2), num2 * 4, SurfaceFormat.ARGB32, data, true, new Inset(num1, num1, num1, num1), new Size(num2, num2), false, false);
                _outlineImageEffectCache[outlineColor] = rawImage;
            }
            return rawImage;
        }

        private bool ShouldPaintOutlines(
          ViewItem vi,
          out Color outlineColor,
          out string name,
          out UIImage[] iconsList)
        {
            outlineColor = Color.Transparent;
            name = null;
            iconsList = null;
            bool flag = false;
            switch (_scope)
            {
                case DebugOutlineScope.FlaggedOnly:
                    flag = vi.DebugOutline != Color.Transparent;
                    break;
                case DebugOutlineScope.Input:
                    bool mouseInteractiveFlag1;
                    bool keyInteractiveFlag1;
                    DetermineInteractivity(vi, out mouseInteractiveFlag1, out bool _, out keyInteractiveFlag1, out bool _);
                    flag = mouseInteractiveFlag1 || keyInteractiveFlag1;
                    break;
                case DebugOutlineScope.Hosts:
                    flag = vi is Host;
                    break;
                case DebugOutlineScope.All:
                    flag = true;
                    break;
            }
            if (flag)
            {
                Host host = vi as Host;
                outlineColor = !(vi.DebugOutline != Color.Transparent) ? (host == null ? _outlineColor : _hostOutlineColor) : vi.DebugOutline;
                switch (_label)
                {
                    case DebugLabelFormat.Name:
                        name = vi.Name;
                        if (host != null)
                        {
                            string str = host.Source;
                            if (host.ChildUI != null)
                                str = host.ChildUI.TypeSchema.Name;
                            name = name == null ? str : str + " ('" + name + "')";
                            break;
                        }
                        break;
                    case DebugLabelFormat.Full:
                        bool mouseInteractiveFlag2;
                        bool mouseFocusFlag;
                        bool keyInteractiveFlag2;
                        bool keyFocusFlag;
                        int interactivity = DetermineInteractivity(vi, out mouseInteractiveFlag2, out mouseFocusFlag, out keyInteractiveFlag2, out keyFocusFlag);
                        if (interactivity > 0)
                        {
                            iconsList = new UIImage[interactivity];
                            int num1 = 0;
                            if (mouseInteractiveFlag2)
                                iconsList[num1++] = !mouseFocusFlag ? _mouseInteractiveImage : _mouseFocusImage;
                            int num2;
                            if (keyInteractiveFlag2)
                            {
                                if (keyFocusFlag)
                                {
                                    UIImage[] uiImageArray = iconsList;
                                    int index = num1;
                                    num2 = index + 1;
                                    UIImage keyFocusImage = _keyFocusImage;
                                    uiImageArray[index] = keyFocusImage;
                                    goto case DebugLabelFormat.Name;
                                }
                                else
                                {
                                    UIImage[] uiImageArray = iconsList;
                                    int index = num1;
                                    num2 = index + 1;
                                    UIImage interactiveImage = _keyInteractiveImage;
                                    uiImageArray[index] = interactiveImage;
                                    goto case DebugLabelFormat.Name;
                                }
                            }
                            else
                                goto case DebugLabelFormat.Name;
                        }
                        else
                            goto case DebugLabelFormat.Name;
                }
            }
            return flag;
        }

        private int DetermineInteractivity(
          ViewItem vi,
          out bool mouseInteractiveFlag,
          out bool mouseFocusFlag,
          out bool keyInteractiveFlag,
          out bool keyFocusFlag)
        {
            int num = 0;
            mouseInteractiveFlag = false;
            mouseFocusFlag = false;
            keyInteractiveFlag = false;
            keyFocusFlag = false;
            if (vi is Host host && host.ChildUI != null)
            {
                UIClass childUi = host.ChildUI;
                if (childUi != null)
                {
                    mouseInteractiveFlag = childUi.MouseInteractive;
                    if (mouseInteractiveFlag)
                    {
                        mouseFocusFlag = childUi.DirectMouseFocus;
                        ++num;
                    }
                    keyInteractiveFlag = childUi.KeyInteractive;
                    if (keyInteractiveFlag)
                    {
                        keyFocusFlag = childUi.DirectKeyFocus;
                        ++num;
                    }
                }
            }
            return num;
        }

        public static void NotifyLayoutChange(ViewItem vi) => SomethingChanged(vi);

        public static void NotifyInteractivityChange(ViewItem vi) => SomethingChanged(vi);

        private void OnSomethingChanged(ViewItem vi)
        {
            if (vi != null && !IsViewItemUnderRoot(vi))
                return;
            MarkPaintInvalid();
        }

        private bool IsViewItemUnderRoot(ViewItem vi) => _rootItem != null && _rootItem.HasDescendant(vi);

        private delegate void SomethingChangedHandler(ViewItem vi);
    }
}
