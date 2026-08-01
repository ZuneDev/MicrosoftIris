using System;
using System.Runtime.InteropServices;
using Silk.NET.OpenGL;

namespace Microsoft.Iris.Render.OpenGL.Scene
{
    /// <summary>
    /// An image backed by an OpenGL texture. Pixel content supplied through
    /// <see cref="LoadContent"/> is copied into managed memory and uploaded lazily on
    /// the render thread (GL calls are only valid there), then cached as a texture.
    ///
    /// Deliberately uses its own private lock (the base class's parameterless
    /// constructor), not GLRenderSession.SyncRoot: nothing outside a GLImage ever needs
    /// its pixel/texture state to be atomic with the rest of the scene graph, and
    /// EnsureUploaded's first-use path can synchronously trigger a real, potentially
    /// slow CPU image decode (via ImageCacheItem/ImageLoader) -- sharing the global lock
    /// here would mean decoding one image blocks every other app-thread scene mutation
    /// for the decode's whole duration, which is exactly the responsiveness regression
    /// this design was changed to avoid. See logs/UIX.RenderApi.OpenGL/Implementation.md.
    /// </summary>
    public sealed class GLImage : SharedRenderObject, IImage
    {
        private readonly ContentNotifyHandler? m_notify;
        private byte[]? m_pixelsBgra;   // always stored as tightly-packed BGRA (A8R8G8B8 little-endian)
        private bool m_dirty = true;
        private bool m_loadRequested;
        private Size m_size;
        private ImageFormat m_format = ImageFormat.None;

        public GLImage(string identifier, ContentNotifyHandler? notify)
        {
            Identifier = identifier;
            m_notify = notify;
        }

        public Size Size { get { lock (SyncRoot) return m_size; } }
        public ImageFormat Format { get { lock (SyncRoot) return m_format; } }
        public string Identifier { get; }

        internal uint TextureId { get; private set; }

        // LoadContent (app thread, via ImageLoader/ImageCacheItem -- or the render
        // thread, if reached through EnsureUploaded's own Acquire callback below) and
        // EnsureUploaded (render thread, every frame) touch the same pixel/dirty
        // state, so both take the lock for their full body rather than field by
        // field -- a torn dirty-flag check-and-clear here would mean a just-loaded
        // image never gets uploaded, or an upload races a half-written pixel buffer.
        public bool LoadContent(ImageFormat format, Size size, int stride, IntPtr data)
        {
            if (data == IntPtr.Zero || size.Width <= 0 || size.Height <= 0)
                return false;

            int bpp = format == ImageFormat.A8 ? 1 : 4;
            byte[] packed = new byte[size.Width * size.Height * 4];

            for (int y = 0; y < size.Height; y++)
            {
                IntPtr row = data + y * stride;
                for (int x = 0; x < size.Width; x++)
                {
                    int dst = (y * size.Width + x) * 4;
                    if (format == ImageFormat.A8)
                    {
                        byte a = Marshal.ReadByte(row, x);
                        packed[dst + 0] = 255;
                        packed[dst + 1] = 255;
                        packed[dst + 2] = 255;
                        packed[dst + 3] = a;
                    }
                    else
                    {
                        int src = x * bpp;
                        byte b = Marshal.ReadByte(row, src + 0);
                        byte g = Marshal.ReadByte(row, src + 1);
                        byte r = Marshal.ReadByte(row, src + 2);
                        byte a = format == ImageFormat.X8R8G8B8 ? (byte)255 : Marshal.ReadByte(row, src + 3);
                        packed[dst + 0] = b;
                        packed[dst + 1] = g;
                        packed[dst + 2] = r;
                        packed[dst + 3] = a;
                    }
                }
            }

            lock (SyncRoot)
            {
                m_pixelsBgra = packed;
                m_size = size;
                m_format = format;
                m_dirty = true;
            }
            return true;
        }

        /// <summary>Upload pending pixel content to the GPU. Must run on the render thread.</summary>
        internal unsafe void EnsureUploaded(GL gl)
        {
            lock (SyncRoot)
            {
                // Content is loaded lazily: request it the first time it's actually needed
                // for drawing, mirroring the original Image.OnUsageChange -> AcquireContent
                // request/response contract (Acquire = "please load me", LoadContent = the
                // response). Firing Acquire from inside LoadContent itself (as this used to)
                // re-enters ImageCacheItem.ReloadImage -> LoadBuffer -> ImageLoader.FromBuffer
                // -> LoadContent and recurses forever, so Release is raised here too, after
                // the Acquire call has fully returned, never from within LoadContent's own
                // call stack. The Acquire callback runs synchronously on the render thread,
                // reentering this same lock (Monitor is reentrant) -- fine as long as it
                // doesn't itself try to marshal back onto the UI dispatcher and block.
                if (m_pixelsBgra == null && !m_loadRequested)
                {
                    m_loadRequested = true;
                    m_notify?.Invoke(ContentNotification.Acquire, this, IntPtr.Zero);
                    if (m_pixelsBgra != null)
                        m_notify?.Invoke(ContentNotification.Release, this, IntPtr.Zero);
                }

                if (!m_dirty || m_pixelsBgra == null)
                    return;
                m_dirty = false;

                if (TextureId == 0)
                    TextureId = gl.GenTexture();

                gl.BindTexture(TextureTarget.Texture2D, TextureId);
                gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.Linear);
                gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Linear);
                gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.ClampToEdge);
                gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GLEnum.ClampToEdge);

                fixed (byte* p = m_pixelsBgra)
                {
                    gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba,
                        (uint)m_size.Width, (uint)m_size.Height, 0,
                        PixelFormat.Bgra, PixelType.UnsignedByte, p);
                }
                gl.BindTexture(TextureTarget.Texture2D, 0);
            }
        }

        internal void DeleteTexture(GL gl)
        {
            if (TextureId != 0)
            {
                gl.DeleteTexture(TextureId);
                TextureId = 0;
            }
        }
    }
}
