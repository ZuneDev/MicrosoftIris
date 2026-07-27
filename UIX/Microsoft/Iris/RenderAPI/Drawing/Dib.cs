// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.RenderAPI.Drawing.Dib
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.OS;
using Microsoft.Iris.Render;
using System;

namespace Microsoft.Iris.RenderAPI.Drawing
{
    internal sealed class Dib : IDisposable
    {
        private IntPtr m_hdib;
        private IntPtr m_prgbData;
        private Size m_sizePxl;
        private Action m_onDispose;

        public Dib(IntPtr hdib, IntPtr prgbData, Size sizePxl)
            : this(hdib, prgbData, sizePxl, null)
        {
        }

        // onDispose lets non-native-backed bitmaps (e.g. from a cross-platform
        // TextDocument backend, which has no hdib to free via SpFreeDib) supply
        // their own cleanup for the memory prgbData points to.
        public Dib(IntPtr hdib, IntPtr prgbData, Size sizePxl, Action onDispose)
        {
            m_hdib = hdib;
            m_prgbData = prgbData;
            m_sizePxl = sizePxl;
            m_onDispose = onDispose;
        }

        ~Dib() => Dispose(false);

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        private void Dispose(bool fInDispose)
        {
            if (m_hdib != IntPtr.Zero)
            {
                NativeApi.SpFreeDib(m_hdib);
                m_hdib = IntPtr.Zero;
            }
            m_prgbData = IntPtr.Zero;
            m_onDispose?.Invoke();
            m_onDispose = null;
        }

        public Size ContentSize => m_sizePxl;

        public int Stride => m_sizePxl.Width * 4;

        public ImageFormat ImageFormat => ImageFormat.A8R8G8B8;

        internal IntPtr Data => m_prgbData;
    }
}
