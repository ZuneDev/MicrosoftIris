// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Common.RenderObject
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using System;
using System.Diagnostics;

namespace Microsoft.Iris.Render.Common
{
    internal abstract class RenderObject : IRenderObject, IDisposable
    {
        private bool m_fObjectDisposed;

        internal RenderObject() => this.m_fObjectDisposed = false;

        ~RenderObject()
        {
            if (this.m_fObjectDisposed)
                return;
            this.Dispose(false);
        }

        public void Dispose()
        {
            if (this.m_fObjectDisposed)
                return;
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool fInDispose) => this.m_fObjectDisposed = true;

        internal string DebugDescription
        {
            get => "<unknown>";
            set
            {
            }
        }

        [Conditional("DEBUG")]
        protected virtual void Invariant() => Debug2.Validate(!this.m_fObjectDisposed, typeof(ObjectDisposedException), this.GetType().ToString());
    }
}
