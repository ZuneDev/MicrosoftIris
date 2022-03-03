// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Extensions.ImageCacheItem
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using System;
using System.Collections;

namespace Microsoft.Iris.Render.Extensions
{
    public class ImageCacheItem : IDisposable
    {
        private static TimeSpan s_tsUpdateThreshhold = new TimeSpan(0, 0, 2);
        private bool m_fObjectDisposed;
        private string m_stIdentifier;
        private IImage m_image;
        protected ImageRequirements m_req;
        protected Size m_size;
        protected BitmapInformation m_info;
        protected IntPtr m_buffer;
        protected uint m_length;
        private int m_countUsers;
        private int m_countLoadsInProgress;
        private bool m_fFullLoadRequested;
        private ArrayList m_prevLoadInfos;
        private DateTime m_dtLastUsed;
        private ImageCache m_owner;

        public ImageCacheItem(
          IRenderSession renderSession,
          string identifier,
          IntPtr buffer,
          uint length,
          Size maxSize,
          bool flippable,
          bool antialiasEdges)
          : this(renderSession, identifier, maxSize, flippable, antialiasEdges)
        {
            this.m_buffer = buffer;
            this.m_length = length;
        }

        public ImageCacheItem(
          IRenderSession renderSession,
          string identifier,
          Size maxSize,
          bool flippable,
          bool antialiasEdges)
          : this(renderSession, identifier)
        {
            this.m_req = new ImageRequirements();
            this.m_req.MaximumSize = maxSize;
            this.m_req.Flippable = flippable;
            this.m_req.AntialiasEdges = antialiasEdges;
        }

        protected ImageCacheItem(IRenderSession renderSession, string identifier)
        {
            this.m_stIdentifier = identifier;
            this.m_image = renderSession.CreateImage(this, this.m_stIdentifier, new ContentNotifyHandler(this.ReloadImage));
            this.UpdateLastUsedTime();
        }

        ~ImageCacheItem() => this.Dispose(false);

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool fInDispose)
        {
            if (!fInDispose)
                return;
            this.OnDispose();
        }

        protected virtual void OnDispose()
        {
            if (this.m_image != null)
            {
                this.m_image.UnregisterUsage(this);
                this.m_image = null;
            }
            if (this.m_info != null)
            {
                this.m_info.Dispose();
                this.m_info = null;
            }
            this.m_fObjectDisposed = true;
        }

        public virtual void ReleaseImage()
        {
            if (this.m_image == null)
                return;
            this.m_image.UnregisterUsage(this);
            this.m_image = null;
        }

        public int UsageCount => this.m_countUsers;

        public void RegisterUsage(object user)
        {
            this.AssertValidState();
            ++this.m_countUsers;
            this.UpdateLastUsedTime();
        }

        public void UnregisterUsage(object user)
        {
            this.AssertValidState();
            --this.m_countUsers;
            this.UpdateLastUsedTime();
        }

        public string Identifier => this.m_stIdentifier;

        public IImage RenderImage => this.m_image;

        public Size ImageSize
        {
            get
            {
                if (this.m_size.IsZero)
                    this.LoadBuffer();
                return this.m_size;
            }
        }

        public bool HasLoadsInProgress => this.m_countLoadsInProgress > 0;

        public bool InUse => this.m_image != null && this.m_image.UsageCount > 1 || this.m_countUsers > 0;

        public virtual void RemoveData()
        {
            if (this.HasLoadsInProgress)
            {
                if (this.m_prevLoadInfos == null)
                    this.m_prevLoadInfos = new ArrayList();
                this.m_prevLoadInfos.Add(m_info);
                this.m_size = Size.Zero;
                this.m_info = null;
            }
            else
            {
                this.m_size = Size.Zero;
                if (this.m_info == null)
                    return;
                this.m_info.Dispose();
                this.m_info = null;
            }
        }

        public virtual void StartLoad() => this.LoadBuffer();

        protected void SetBuffer(IntPtr buffer, uint length)
        {
            this.m_buffer = buffer;
            this.m_length = length;
        }

        protected void SetSize(Size size) => this.m_size = size;

        internal void ReloadImage(ContentNotification notification, IImage image, IntPtr data)
        {
            switch (notification)
            {
                case ContentNotification.Acquire:
                    if (this.m_image == null)
                        break;
                    this.m_fFullLoadRequested = true;
                    this.LoadBuffer();
                    break;
                case ContentNotification.Release:
                    this.EndLoadImageData();
                    break;
            }
        }

        private void LoadBuffer()
        {
            if (!this.EnsureBuffer())
                return;
            this.ProcessBuffer();
        }

        protected bool ProcessBuffer()
        {
            this.UpdateLastUsedTime();
            return !this.m_fFullLoadRequested ? this.DoHeaderLoad() : this.BeginLoadImageData();
        }

        private bool BeginLoadImageData()
        {
            if (!this.DoImageLoad())
                return false;
            ++this.m_countLoadsInProgress;
            return true;
        }

        private void EndLoadImageData()
        {
            this.AssertValidState();
            --this.m_countLoadsInProgress;
            this.OnImageLoadComplete();
            if (!this.HasLoadsInProgress)
            {
                if (this.m_info != null)
                {
                    this.m_info.Dispose();
                    this.m_info = null;
                }
                if (this.m_prevLoadInfos != null)
                {
                    foreach (BitmapInformation prevLoadInfo in this.m_prevLoadInfos)
                        prevLoadInfo?.Dispose();
                    this.m_prevLoadInfos.Clear();
                    this.m_prevLoadInfos = null;
                }
            }
            this.UpdateLastUsedTime();
        }

        protected virtual bool EnsureBuffer() => true;

        protected virtual bool DoHeaderLoad()
        {
            ImageHeader header;
            if (!(this.m_buffer != IntPtr.Zero) || this.m_length <= 0U || !ImageLoader.LoadHeader(this.m_buffer, (int)this.m_length, this.m_req, out header))
                return false;
            this.SetSize(header.sizeActualPxl);
            return true;
        }

        protected virtual bool DoImageLoad()
        {
            BitmapInformation bitmapInfo = null;
            bool flag = false;
            if (this.m_image != null)
                flag = !(this.m_buffer == IntPtr.Zero) ? ImageLoader.FromBuffer(this.m_image, this.m_buffer, (int)this.m_length, this.m_req.MaximumSize, this.m_req.Flippable, this.m_req.AntialiasEdges, this.m_req.BorderWidth, this.m_req.BorderColor, out bitmapInfo) : ImageLoader.FromFile(this.m_image, this.m_image.Identifier, this.m_req.MaximumSize, this.m_req.Flippable, this.m_req.AntialiasEdges, this.m_req.BorderWidth, this.m_req.BorderColor, out bitmapInfo);
            if (flag)
            {
                this.m_info = bitmapInfo;
                this.SetSize(this.m_info.imageInfo.Header.sizeActualPxl);
            }
            return flag;
        }

        protected virtual void OnImageLoadComplete()
        {
        }

        internal ImageCache ImageCacheOwner
        {
            get => this.m_owner;
            set => this.m_owner = value;
        }

        protected void UpdateLastUsedTime()
        {
            this.AssertValidState();
            if (!(DateTime.UtcNow - this.m_dtLastUsed > s_tsUpdateThreshhold))
                return;
            this.m_dtLastUsed = DateTime.UtcNow;
            if (this.m_owner == null)
                return;
            this.m_owner.UpdateLastUsedItem(this);
        }

        public bool IsOlder(DateTime dtCompare)
        {
            this.AssertValidState();
            return this.m_dtLastUsed < dtCompare;
        }

        protected void AssertValidState()
        {
        }

        public override string ToString() => this.m_stIdentifier;
    }
}
