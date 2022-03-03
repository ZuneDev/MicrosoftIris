// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Graphics.Image
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Internal;
using Microsoft.Iris.Render.Protocol;
using Microsoft.Iris.Render.Remote;
using System;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Graphics
{
    internal class Image : SharedResource, IImage, ISharedRenderObject, ISurfaceContentOwner
    {
        private SurfaceManager m_surfaceManager;
        private Surface m_surface;
        private ImageFormat m_imageFormat;
        private Size m_sizeImage;
        private Size m_sizeGutterPxl;
        private DataBufferTracker m_dataTracker;
        private ContentNotifyHandler m_handlerContent;
        private string m_stIdentifier;
        private ImageLoadInfo m_loadInfo;
        private bool m_insideReleaseCallback;

        internal Image(
          RenderSession session,
          SurfaceManager surfaceManager,
          string identifier,
          ContentNotifyHandler handler,
          Size sizeGutter)
          : base(session)
        {
            session.AssertOwningThread();
            this.m_session = session;
            this.m_surfaceManager = surfaceManager;
            this.m_stIdentifier = identifier;
            this.m_handlerContent = handler;
            this.m_sizeGutterPxl = sizeGutter;
        }

        protected override void Dispose(bool fInDispose)
        {
            try
            {
                if (fInDispose && this.m_dataTracker != null)
                    MessagingSession.Current.ReturnDataBufferTracker(this.m_dataTracker);
                this.m_surface = null;
                this.m_surfaceManager = null;
                this.m_dataTracker = null;
                this.m_handlerContent = null;
                this.m_session = null;
            }
            finally
            {
                base.Dispose(fInDispose);
            }
        }

        internal Size Size => this.m_sizeImage;

        internal ImageFormat Format => this.m_imageFormat;

        internal Surface Surface => this.m_surface;

        internal string Identifier => this.m_stIdentifier;

        internal Size GutterSize
        {
            get => this.m_sizeGutterPxl;
            set
            {
                if (value.Width <= this.m_sizeGutterPxl.Width && value.Height <= this.m_sizeGutterPxl.Height)
                    return;
                this.m_sizeGutterPxl.Width = Math.Max(value.Width, this.m_sizeGutterPxl.Width);
                this.m_sizeGutterPxl.Height = Math.Max(value.Height, this.m_sizeGutterPxl.Height);
                if (!this.InUse)
                    return;
                this.AcquireContent();
            }
        }

        void ISurfaceContentOwner.OnSurfaceDisposed(Surface surface)
        {
            if (this.m_surface != surface)
                return;
            this.m_surface = null;
        }

        void ISurfaceContentOwner.OnRestoreContent(Surface surface)
        {
            if (this.m_surface != surface)
                return;
            this.AcquireContent();
        }

        Size IImage.Size => this.m_sizeImage;

        ImageFormat IImage.Format => this.m_imageFormat;

        string IImage.Identifier => this.m_stIdentifier;

        bool IImage.LoadContent(
          ImageFormat imageFormat,
          Size sizeImage,
          int nStride,
          IntPtr rgData)
        {
            this.m_session.AssertOwningThread();
            bool flag1 = false;
            bool flag2 = false;
            DataBuffer buffer = null;
            try
            {
                SurfaceFormat nFormat = SurfaceFormatInfo.FromImageFormat(imageFormat);
                if (nFormat == SurfaceFormat.RGB32)
                    nFormat = SurfaceFormat.ARGB32;
                if (this.m_surface != null && (this.m_imageFormat != imageFormat || this.m_sizeImage != sizeImage || this.m_sizeGutterPxl != this.m_surface.GutterSize))
                {
                    if (this.InUse)
                        this.m_surface.RemoveActiveUser(this);
                    this.m_surface = null;
                }
                if (this.m_surface == null)
                {
                    this.m_surface = this.m_surfaceManager.RequestSurface(this, nFormat, sizeImage, this.m_sizeGutterPxl);
                    flag2 = true;
                    if (this.InUse)
                        this.m_surface.AddActiveUser(this);
                }
                IntPtr num1 = IntPtr.Zero;
                if (this.m_session.IsForeignByteOrderOnWindowing)
                {
                    int bitsPerPixel = SurfaceFormatInfo.GetBitsPerPixel(nFormat);
                    int num2 = sizeImage.Height * nStride;
                    num1 = Marshal.AllocHGlobal(num2);
                    Memory.ConvertEndian(num1, rgData, num2, bitsPerPixel);
                }
                this.m_imageFormat = imageFormat;
                this.m_sizeImage = sizeImage;
                uint cbSize = (uint)(nStride * this.m_sizeImage.Height);
                buffer = this.m_session.BuildDataBuffer(num1 == IntPtr.Zero ? rgData : num1, cbSize);
                this.Track(buffer, num1, rgData, nStride);
                buffer.Commit();
                ++this.m_loadInfo.nLoadsInProgress;
                ImageHeader header;
                header.sizeActualPxl = this.m_sizeImage;
                header.sizeOriginalPxl = this.m_sizeImage;
                header.nStride = nStride;
                header.nFormat = nFormat;
                this.m_session.LoadSurface(buffer, this.m_surface, header);
                if (flag2 && this.ContentReloadedEvent != null)
                    this.ContentReloadedEvent(this);
                flag1 = true;
                return flag1;
            }
            finally
            {
                if (!flag1)
                {
                    if (buffer != null)
                        this.StopTracking(buffer);
                    this.m_surface = null;
                }
            }
        }

        internal event Image.ContentReloadedHandler ContentReloadedEvent;

        private void AcquireContent()
        {
            Debug2.Validate(!this.m_insideReleaseCallback, typeof(InvalidOperationException), "AcquireContent must not be called while inside ContentNotify(Release) callback");
            ContentNotifyHandler handlerContent = this.m_handlerContent;
            if (this.m_handlerContent != null)
            {
                if (this.m_loadInfo != null && this.m_loadInfo.nLoadsInProgress > 0)
                {
                    ++this.m_loadInfo.nSystemLoadRequests;
                    ((IImage)this).LoadContent(this.m_imageFormat, this.m_sizeImage, this.m_loadInfo.nStride, this.m_loadInfo.rgAppData);
                }
                else
                    this.m_handlerContent(ContentNotification.Acquire, this, IntPtr.Zero);
            }
            Surface surface = this.m_surface;
        }

        protected override void OnUsageChange(bool fUsed)
        {
            if (fUsed)
            {
                if (this.m_surface == null)
                    this.AcquireContent();
                else
                    this.m_surface.AddActiveUser(this);
            }
            else
            {
                if (this.m_surface == null)
                    return;
                this.m_surface.RemoveActiveUser(this);
            }
        }

        private void Track(DataBuffer buffer, IntPtr rgConvertData, IntPtr rgAppData, int nStride)
        {
            ImageLoadInfo imageLoadInfo;
            if (this.m_loadInfo != null && this.m_loadInfo.rgAppData == rgAppData && this.m_loadInfo.nStride == nStride)
            {
                imageLoadInfo = this.m_loadInfo;
            }
            else
            {
                imageLoadInfo = new ImageLoadInfo();
                imageLoadInfo.imageOwner = this;
                imageLoadInfo.nLoadsInProgress = 0;
                imageLoadInfo.nSystemLoadRequests = 0;
                imageLoadInfo.handlerNotify = this.m_handlerContent;
                imageLoadInfo.rgAppData = rgAppData;
                imageLoadInfo.nStride = nStride;
                if (this.m_dataTracker != null)
                {
                    MessagingSession.Current.ReturnDataBufferTracker(this.m_dataTracker);
                    this.m_dataTracker = null;
                }
                this.m_dataTracker = MessagingSession.Current.CreateDataBufferTracker(this);
            }
            DataBufferCleanupInfo bufferCleanupInfo = new DataBufferCleanupInfo();
            bufferCleanupInfo.rgConvertData = rgConvertData;
            bufferCleanupInfo.imageLoadInfo = imageLoadInfo;
            DataBufferTracker.CleanupEventHandler handler = new DataBufferTracker.CleanupEventHandler(OnDataBufferConsumed);
            this.m_dataTracker.Track(buffer, handler, bufferCleanupInfo);
            this.m_loadInfo = imageLoadInfo;
        }

        private void StopTracking(DataBuffer buffer)
        {
            if (this.m_dataTracker == null)
                return;
            this.m_dataTracker.Release(buffer, DataBufferTracker.Users.Valid);
        }

        private static void OnDataBufferConsumed(object sender, DataBufferTracker.CleanupEventArgs args)
        {
            DataBufferCleanupInfo contextData = (DataBufferCleanupInfo)args.ContextData;
            ImageLoadInfo imageLoadInfo = contextData.imageLoadInfo;
            --imageLoadInfo.nLoadsInProgress;
            if (imageLoadInfo.nSystemLoadRequests > 0)
            {
                --imageLoadInfo.nSystemLoadRequests;
            }
            else
            {
                if (imageLoadInfo.handlerNotify != null)
                {
                    imageLoadInfo.imageOwner.m_insideReleaseCallback = true;
                    imageLoadInfo.handlerNotify(ContentNotification.Release, imageLoadInfo.imageOwner, imageLoadInfo.rgAppData);
                    imageLoadInfo.imageOwner.m_insideReleaseCallback = false;
                }
                imageLoadInfo.rgAppData = IntPtr.Zero;
                imageLoadInfo.handlerNotify = null;
                imageLoadInfo.imageOwner = null;
            }
            if (!(contextData.rgConvertData != IntPtr.Zero))
                return;
            Marshal.FreeHGlobal(contextData.rgConvertData);
            contextData.rgConvertData = IntPtr.Zero;
        }

        internal delegate void ContentReloadedHandler(Image image);
    }
}
