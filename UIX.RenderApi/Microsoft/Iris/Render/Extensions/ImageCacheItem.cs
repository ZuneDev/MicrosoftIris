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
        protected ImageRequirements m_req;
        protected Size m_size;
        protected BitmapInformation m_info;
        protected IntPtr m_buffer;
        protected uint m_length;
        private int m_countLoadsInProgress;
        private bool m_fFullLoadRequested;
        private ArrayList m_prevLoadInfos;
        private DateTime m_dtLastUsed;

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
            m_buffer = buffer;
            m_length = length;
        }

        public ImageCacheItem(
          IRenderSession renderSession,
          string identifier,
          Size maxSize,
          bool flippable,
          bool antialiasEdges)
          : this(renderSession, identifier)
        {
            m_req = new ImageRequirements();
            m_req.MaximumSize = maxSize;
            m_req.Flippable = flippable;
            m_req.AntialiasEdges = antialiasEdges;
        }

        protected ImageCacheItem(IRenderSession renderSession, string identifier)
        {
            Identifier = identifier;
            RenderImage = renderSession.CreateImage(this, Identifier, ReloadImage);
            UpdateLastUsedTime();
        }

        ~ImageCacheItem() => Dispose(false);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool fInDispose)
        {
            if (!fInDispose)
                return;
            OnDispose();
        }

        protected virtual void OnDispose()
        {
            if (RenderImage != null)
            {
                RenderImage.UnregisterUsage(this);
                RenderImage = null;
            }
            if (m_info != null)
            {
                m_info.Dispose();
                m_info = null;
            }
            m_fObjectDisposed = true;
        }

        public virtual void ReleaseImage()
        {
            if (RenderImage == null)
                return;
            RenderImage.UnregisterUsage(this);
            RenderImage = null;
        }

        public int UsageCount { get; private set; }

        public void RegisterUsage(object user)
        {
            AssertValidState();
            ++UsageCount;
            UpdateLastUsedTime();
        }

        public void UnregisterUsage(object user)
        {
            AssertValidState();
            --UsageCount;
            UpdateLastUsedTime();
        }

        public string Identifier { get; }

        public IImage RenderImage { get; private set; }

        public Size ImageSize
        {
            get
            {
                if (m_size.IsZero)
                    LoadBuffer();
                return m_size;
            }
        }

        public bool HasLoadsInProgress => m_countLoadsInProgress > 0;

        public bool InUse => RenderImage != null && RenderImage.UsageCount > 1 || UsageCount > 0;

        public virtual void RemoveData()
        {
            if (HasLoadsInProgress)
            {
                m_prevLoadInfos ??= new ArrayList();
                m_prevLoadInfos.Add(m_info);
                m_size = Size.Zero;
            }
            else
            {
                m_size = Size.Zero;
                m_info?.Dispose();
            }

            m_info = null;
        }

        public virtual void StartLoad() => LoadBuffer();

        protected void SetBuffer(IntPtr buffer, uint length)
        {
            m_buffer = buffer;
            m_length = length;
        }

        protected void SetSize(Size size) => m_size = size;

        internal void ReloadImage(ContentNotification notification, IImage image, IntPtr data)
        {
            switch (notification)
            {
                case ContentNotification.Acquire:
                    if (RenderImage == null)
                        break;
                    m_fFullLoadRequested = true;
                    LoadBuffer();
                    break;
                case ContentNotification.Release:
                    EndLoadImageData();
                    break;
            }
        }

        private void LoadBuffer()
        {
            if (!EnsureBuffer())
                return;
            ProcessBuffer();
        }

        protected bool ProcessBuffer()
        {
            UpdateLastUsedTime();
            return !m_fFullLoadRequested ? DoHeaderLoad() : BeginLoadImageData();
        }

        private bool BeginLoadImageData()
        {
            if (!DoImageLoad())
                return false;
            ++m_countLoadsInProgress;
            return true;
        }

        private void EndLoadImageData()
        {
            AssertValidState();
            --m_countLoadsInProgress;
            OnImageLoadComplete();
            if (!HasLoadsInProgress)
            {
                if (m_info != null)
                {
                    m_info.Dispose();
                    m_info = null;
                }
                if (m_prevLoadInfos != null)
                {
                    foreach (BitmapInformation prevLoadInfo in m_prevLoadInfos)
                        prevLoadInfo?.Dispose();
                    m_prevLoadInfos.Clear();
                    m_prevLoadInfos = null;
                }
            }
            UpdateLastUsedTime();
        }

        protected virtual bool EnsureBuffer() => true;

        protected virtual bool DoHeaderLoad()
        {
            if (m_buffer == IntPtr.Zero
                || m_length <= 0U
                || !ImageLoader.LoadHeader(m_buffer, (int)m_length, m_req, out var header))
                return false;
            
            SetSize(header.sizeActualPxl);
            return true;
        }

        protected virtual bool DoImageLoad()
        {
            BitmapInformation bitmapInfo = null;
            var loadSuccess = false;
            if (RenderImage != null)
            {
                if (m_buffer != IntPtr.Zero)
                {
                    loadSuccess = ImageLoader.FromBuffer(RenderImage, m_buffer, (int)m_length, m_req.MaximumSize,
                        m_req.Flippable, m_req.AntialiasEdges, m_req.BorderWidth, m_req.BorderColor,
                        out bitmapInfo);
                }
                else
                {
                    loadSuccess = ImageLoader.FromFile(RenderImage, RenderImage.Identifier, m_req.MaximumSize,
                        m_req.Flippable,m_req.AntialiasEdges, m_req.BorderWidth, m_req.BorderColor,
                        out bitmapInfo);
                }
            }

            if (!loadSuccess)
                return false;
            
            m_info = bitmapInfo;
            SetSize(m_info.imageInfo.Header.sizeActualPxl);
            return true;
        }

        protected virtual void OnImageLoadComplete()
        {
        }

        internal ImageCache ImageCacheOwner { get; set; }

        protected void UpdateLastUsedTime()
        {
            AssertValidState();
            if (DateTime.UtcNow - m_dtLastUsed <= s_tsUpdateThreshhold)
                return;
            
            m_dtLastUsed = DateTime.UtcNow;
            ImageCacheOwner?.UpdateLastUsedItem(this);
        }

        public bool IsOlder(DateTime dtCompare)
        {
            AssertValidState();
            return m_dtLastUsed < dtCompare;
        }

        protected void AssertValidState()
        {
        }

        public override string ToString() => Identifier;
    }
}
