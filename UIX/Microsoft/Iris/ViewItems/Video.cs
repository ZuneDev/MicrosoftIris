// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.ViewItems.Video
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Drawing;
using Microsoft.Iris.Markup;
using Microsoft.Iris.Render;
using Microsoft.Iris.RenderAPI.Drawing;
using Microsoft.Iris.RenderAPI.VideoPlayback;
using Microsoft.Iris.Session;
using System;

namespace Microsoft.Iris.ViewItems
{
    internal class Video : ContentViewItem, IUIVideoPortal, ITrackableUIElement
    {
        private const int c_spriteBorderCount = 2;
        private Color _fillLetterbox;
        private IUIVideoStream _videoStream;
        private ISprite[] _letterBoxSprites;
        private EventHandler _portalChangeEvent;

        public Video()
        {
            _fillLetterbox = Color.Black;
            ((ITrackableUIElementEvents)this).UIChange += new EventHandler(OnUIChange);
        }

        protected override void OnDispose()
        {
            if (_videoStream != null)
                _videoStream.RevokePortal(this);
            _videoStream = null;
            ((ITrackableUIElementEvents)this).UIChange -= new EventHandler(OnUIChange);
            base.OnDispose();
        }

        public IUIVideoStream VideoStream
        {
            get => _videoStream;
            set
            {
                if (_videoStream == value)
                    return;
                ForceContentChange();
                if (_videoStream != null)
                    _videoStream.RevokePortal(this);
                _videoStream = value;
                if (_videoStream != null)
                    _videoStream.RegisterPortal(this);
                FireNotification(NotificationID.VideoStream);
            }
        }

        public Color LetterboxColor
        {
            get => _fillLetterbox;
            set
            {
                if (!(value != LetterboxColor))
                    return;
                _fillLetterbox = value;
                if (_letterBoxSprites != null)
                    _letterBoxSprites[0].Effect.SetProperty("ColorElem.Color", _fillLetterbox.RenderConvert());
                FireNotification(NotificationID.LetterboxColor);
            }
        }

        protected override bool HasContent() => _videoStream != null;

        protected override void CreateContent()
        {
            base.CreateContent();
            if (!UISession.Default.RenderSession.GraphicsDevice.IsVideoComposited)
                return;
            IRenderSession renderSession = UISession.Default.RenderSession;
            VideoElement videoElement = new VideoElement("VideoElement", null);
            IEffectTemplate effectTemplate = renderSession.CreateEffectTemplate(this, nameof(Video));
            effectTemplate.Build(videoElement);
            IEffect instance = effectTemplate.CreateInstance(this);
            _contents.Effect = instance;
            instance.UnregisterUsage(this);
            effectTemplate.UnregisterUsage(this);
            _contents.Effect.SetProperty("VideoElement.Video", (_videoStream as Microsoft.Iris.VideoStream).RenderStream);
            CreateBorderSprites(renderSession);
        }

        protected override void DisposeContent(bool removeFromTree)
        {
            base.DisposeContent(removeFromTree);
            if (_letterBoxSprites == null)
                return;
            foreach (ISprite letterBoxSprite in _letterBoxSprites)
            {
                if (removeFromTree)
                    letterBoxSprite.Remove();
                letterBoxSprite.UnregisterUsage(this);
            }
            _letterBoxSprites = null;
        }

        protected override void OnPaint(bool visible)
        {
            base.OnPaint(visible);
            if (_contents == null || !UISession.Default.RenderSession.GraphicsDevice.IsVideoComposited)
                return;
            BasicVideoPresentation presentation = _videoStream.GetPresentation(this);
            _contents.Position = new Vector3(presentation.DisplayedDestination.Left, presentation.DisplayedDestination.Top, 0.0f);
            _contents.Size = new Vector2(presentation.DisplayedDestination.Size.Width, presentation.DisplayedDestination.Size.Height);
            BasicVideoGeometry geometry = presentation.GetGeometry();
            for (int index = 0; index < 2; ++index)
            {
                if (index < geometry.arrcfBorders.Length)
                {
                    RectangleF arrcfBorder = geometry.arrcfBorders[index];
                    _letterBoxSprites[index].Visible = true;
                    _letterBoxSprites[index].Position = new Vector3(arrcfBorder.Left, arrcfBorder.Top, 0.0f);
                    _letterBoxSprites[index].Size = new Vector2(arrcfBorder.Width, arrcfBorder.Height);
                }
                else
                    _letterBoxSprites[index].Visible = false;
            }
        }

        private void CreateBorderSprites(IRenderSession renderSession)
        {
            _letterBoxSprites = new ISprite[2];
            IEffect colorFillEffect = EffectManager.CreateColorFillEffect(this, LetterboxColor);
            for (int index = 0; index < 2; ++index)
            {
                ISprite sprite = renderSession.CreateSprite(this, this);
                sprite.Effect = colorFillEffect;
                VisualContainer.AddChild(sprite, ContentVisual, VisualOrder.Before);
                _letterBoxSprites[index] = sprite;
            }
            colorFillEffect.UnregisterUsage(this);
        }

        Rectangle IUIVideoPortal.LogicalContentRect => HasVisual ? new Rectangle(0.0f, 0.0f, VisualSize.X, VisualSize.Y) : Rectangle.Zero;

        void IUIVideoPortal.OnStreamChange(bool formatChangedFlag)
        {
            if (formatChangedFlag)
                MarkLayoutInvalid();
            MarkPaintInvalid();
        }

        void IUIVideoPortal.OnRevokeStream()
        {
            MarkLayoutInvalid();
            MarkPaintInvalid();
        }

        event EventHandler IUIVideoPortal.PortalChange
        {
            add => _portalChangeEvent += value;
            remove => _portalChangeEvent -= value;
        }

        private void OnUIChange(object sender, EventArgs args)
        {
            if (_portalChangeEvent == null)
                return;
            _portalChangeEvent(sender, args);
        }
    }
}
