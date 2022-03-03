// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.VideoStream
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;
using Microsoft.Iris.Render;
using Microsoft.Iris.RenderAPI.Drawing;
using Microsoft.Iris.RenderAPI.VideoPlayback;
using Microsoft.Iris.Session;
using System;
using System.Collections;

namespace Microsoft.Iris
{
    public sealed class VideoStream : IUIVideoStream
    {
        private ArrayList _clients;
        private VideoPresentationBuilder _presentationBuilder;
        private IVideoStream _renderStream;
        private bool _disposed;
        private bool _deferredInvalidate;
        private Rectangle _srcVideo;
        private Size _srcAspect;
        private float _contentOverscanPer;
        private WindowPosition _largestPosition;
        private WindowSize _largestSize;
        private bool _isRendering;

        public VideoStream()
        {
            UIDispatcher.VerifyOnApplicationThread();
            _clients = new ArrayList();
            if (UISession.Default.RenderSession.GraphicsDevice.IsVideoComposited)
            {
                _renderStream = UISession.Default.RenderSession.CreateVideoStream(this);
                _renderStream.InvalidateContentEvent += new InvalidateContentHandler(OnRenderVideoStreamChange);
            }
            _presentationBuilder = new VideoPresentationBuilder();
            _presentationBuilder.DisplayMode = VideoDisplayMode.FullInScene;
            _presentationBuilder.ContentOverscanMode = VideoOverscanMode.InvalidContent;
            _largestPosition = new WindowPosition();
            _largestSize = new WindowSize();
            _isRendering = false;
        }

        public void Dispose()
        {
            UIDispatcher.VerifyOnApplicationThread();
            if (_disposed)
                throw new InvalidOperationException("The stream has already been disposed");
            Dispose(true);
            _disposed = true;
        }

        private void Dispose(bool inDisposeFlag)
        {
            if (!inDisposeFlag)
                return;
            if (_renderStream != null)
            {
                _renderStream.InvalidateContentEvent -= new InvalidateContentHandler(OnRenderVideoStreamChange);
                _renderStream.UnregisterUsage(this);
                _renderStream = null;
            }
            _presentationBuilder = null;
            _clients.Clear();
        }

        public int StreamID
        {
            get
            {
                UIDispatcher.VerifyOnApplicationThread();
                return _renderStream.StreamID;
            }
        }

        public float ContentOverscanPercent
        {
            get
            {
                UIDispatcher.VerifyOnApplicationThread();
                return _contentOverscanPer;
            }
            set
            {
                UIDispatcher.VerifyOnApplicationThread();
                if (value < 0.0 || value > 0.5)
                    throw new ArgumentException("Valdid range for content overscan is [0, .5]");
                if (Math2.WithinEpsilon(_contentOverscanPer, value))
                    return;
                _contentOverscanPer = value;
                Invalidate(true);
            }
        }

        public int ContentWidth
        {
            get
            {
                UIDispatcher.VerifyOnApplicationThread();
                return _srcVideo.Width;
            }
            set
            {
                UIDispatcher.VerifyOnApplicationThread();
                if (value < 0)
                    throw new ArgumentException("ContentWidth cannot be negative");
                if (_srcVideo.Width == value)
                    return;
                _srcVideo.Width = value;
                Invalidate(true);
            }
        }

        public int ContentHeight
        {
            get
            {
                UIDispatcher.VerifyOnApplicationThread();
                return _srcVideo.Height;
            }
            set
            {
                UIDispatcher.VerifyOnApplicationThread();
                if (value < 0)
                    throw new ArgumentException("ContentHeight cannot be negative");
                if (_srcVideo.Height == value)
                    return;
                _srcVideo.Height = value;
                Invalidate(true);
            }
        }

        public int ContentAspectWidth
        {
            get
            {
                UIDispatcher.VerifyOnApplicationThread();
                return _srcAspect.Width;
            }
            set
            {
                UIDispatcher.VerifyOnApplicationThread();
                if (value < 0)
                    throw new ArgumentException("ContentAspectWidth cannot be negative");
                if (_srcAspect.Width == value)
                    return;
                _srcAspect.Width = value;
                Invalidate(true);
            }
        }

        public int ContentAspectHeight
        {
            get
            {
                UIDispatcher.VerifyOnApplicationThread();
                return _srcAspect.Height;
            }
            set
            {
                UIDispatcher.VerifyOnApplicationThread();
                if (value < 0)
                    throw new ArgumentException("ContentAspectHeight cannot be negative");
                if (_srcAspect.Height == value)
                    return;
                _srcAspect.Height = value;
                Invalidate(true);
            }
        }

        public WindowPosition DisplayPosition => _largestPosition;

        public WindowSize DisplaySize => _largestSize;

        public bool DisplayVisibility => _isRendering;

        internal IVideoStream RenderStream => _renderStream;

        void IUIVideoStream.RegisterPortal(IUIVideoPortal portal)
        {
            portal.PortalChange += new EventHandler(OnVideoClientChange);
            _clients.Add(portal);
            portal.OnStreamChange(true);
            Invalidate(false);
        }

        void IUIVideoStream.RevokePortal(IUIVideoPortal portal)
        {
            portal.PortalChange -= new EventHandler(OnVideoClientChange);
            if (_clients.Contains(portal))
            {
                _clients.Remove(portal);
            }
            else
            {
                int num = _disposed ? 1 : 0;
            }
            portal.OnRevokeStream();
            Invalidate(false);
        }

        BasicVideoPresentation IUIVideoStream.GetPresentation(
          IUIVideoPortal portal)
        {
            BasicVideoPresentation videoPresentation = null;
            if (!_disposed)
            {
                _presentationBuilder.CompleteDestination = RectangleF.FromRectangle(portal.LogicalContentRect);
                _presentationBuilder.DestinationAspectRatio = new SizeF(portal.LogicalContentRect.Width, portal.LogicalContentRect.Height);
                videoPresentation = _presentationBuilder.BuildPresentation();
            }
            return videoPresentation;
        }

        private void OnVideoClientChange(object sender, EventArgs args)
        {
            Invalidate(false);
            Rectangle rectangle1 = new Rectangle();
            int num1 = 0;
            foreach (IUIVideoPortal client in _clients)
            {
                if (client.IsUIVisible)
                {
                    Rectangle rectangle2 = client.EstimatePosition(null);
                    int num2 = rectangle2.Width * rectangle2.Height;
                    if (num2 > num1)
                    {
                        num1 = num2;
                        rectangle1 = rectangle2;
                    }
                }
            }
            _largestPosition = new WindowPosition(rectangle1.Left, rectangle1.Top);
            _largestSize = new WindowSize(rectangle1.Width, rectangle1.Height);
            Invalidate(false);
        }

        private void OnRenderVideoStreamChange()
        {
            _srcAspect.Height = _renderStream.ContentAspectHeight;
            _srcAspect.Width = _renderStream.ContentAspectWidth;
            _srcVideo.Height = _renderStream.ContentHeight;
            _srcVideo.Width = _renderStream.ContentWidth;
            Invalidate(true);
        }

        public event EventHandler DisplayDetailsChanged;

        private void Invalidate(bool streamChange)
        {
            if (_deferredInvalidate)
                return;
            DeferredCall.Post(DispatchPriority.AppEvent, new DeferredHandler(InvalidateWorker), streamChange);
            _deferredInvalidate = true;
        }

        private void InvalidateWorker(object param)
        {
            bool fFormatChanged = (bool)param;
            bool flag = _srcAspect.Width > 0 && _srcAspect.Height > 0 && _srcVideo.Width >= 0 && _srcVideo.Height >= 0;
            if (fFormatChanged && flag)
            {
                _presentationBuilder.SourceDimensions = new SizeF(_srcVideo.Width, _srcVideo.Height);
                _presentationBuilder.ContentAspectRatio = new SizeF(_srcAspect.Width, _srcAspect.Height);
                _presentationBuilder.ContentOverscanFactor = _contentOverscanPer * 100f;
            }
            _isRendering = false;
            foreach (IUIVideoPortal client in _clients)
            {
                client.OnStreamChange(fFormatChanged);
                if (client.IsUIVisible)
                    _isRendering = true;
            }
            _deferredInvalidate = false;
            FireDisplayDetailsChanged();
        }

        private void FireDisplayDetailsChanged()
        {
            if (DisplayDetailsChanged == null)
                return;
            DisplayDetailsChanged(this, EventArgs.Empty);
        }
    }
}
