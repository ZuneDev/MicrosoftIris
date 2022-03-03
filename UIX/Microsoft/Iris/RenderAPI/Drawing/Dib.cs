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

        public Dib(IntPtr hdib, IntPtr prgbData, Size sizePxl)
        {
            m_hdib = hdib;
            m_prgbData = prgbData;
            m_sizePxl = sizePxl;
        }

        ~Dib() => Dispose(false);

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        private void Dispose(bool fInDispose)
        {
            if (!(m_hdib != IntPtr.Zero))
                return;
            NativeApi.SpFreeDib(m_hdib);
            m_hdib = IntPtr.Zero;
            m_prgbData = IntPtr.Zero;
        }

        public Size ContentSize => m_sizePxl;

        public int Stride => m_sizePxl.Width * 4;

        public ImageFormat ImageFormat => ImageFormat.A8R8G8B8;

        internal IntPtr Data => m_prgbData;
    }
}
