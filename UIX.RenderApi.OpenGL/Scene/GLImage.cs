using System;
using System.Runtime.InteropServices;
using Silk.NET.OpenGL;

namespace Microsoft.Iris.Render.OpenGL
{
    /// <summary>
    /// An image backed by an OpenGL texture. Pixel content supplied through
    /// <see cref="LoadContent"/> is copied into managed memory and uploaded lazily on
    /// the render thread (GL calls are only valid there), then cached as a texture.
    /// </summary>
    public sealed class GLImage : SharedRenderObject, IImage
    {
        private readonly ContentNotifyHandler? m_notify;
        private byte[]? m_pixelsBgra;   // always stored as tightly-packed BGRA (A8R8G8B8 little-endian)
        private bool m_dirty = true;
        private bool m_loadRequested;

        public GLImage(string identifier, ContentNotifyHandler? notify)
        {
            Identifier = identifier;
            m_notify = notify;
        }

        public Size Size { get; private set; }
        public ImageFormat Format { get; private set; } = ImageFormat.None;
        public string Identifier { get; }
        
        internal uint TextureId { get; private set; }

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

            m_pixelsBgra = packed;
            Size = size;
            Format = format;
            m_dirty = true;
            return true;
        }

        /// <summary>Upload pending pixel content to the GPU. Must run on the GL thread.</summary>
        internal unsafe void EnsureUploaded(GL gl)
        {
            // Content is loaded lazily: request it the first time it's actually needed for
            // drawing, mirroring the original Image.OnUsageChange -> AcquireContent request/
            // response contract (Acquire = "please load me", LoadContent = the response).
            // Firing Acquire from inside LoadContent itself (as this used to) re-enters
            // ImageCacheItem.ReloadImage -> LoadBuffer -> ImageLoader.FromBuffer -> LoadContent
            // and recurses forever, so Release is raised here too, after the Acquire call has
            // fully returned, never from within LoadContent's own call stack.
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
                    (uint)Size.Width, (uint)Size.Height, 0,
                    PixelFormat.Bgra, PixelType.UnsignedByte, p);
            }
            gl.BindTexture(TextureTarget.Texture2D, 0);
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
