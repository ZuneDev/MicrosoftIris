// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocol.RemoteObject
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using System;

namespace Microsoft.Iris.Render.Protocol
{
    internal abstract class RemoteObject : RenderObject
    {
        private bool m_fFreeOnDispose;
        protected RenderPort m_renderPort;
        internal RENDERHANDLE m_renderHandle;

        public RemoteObject(RenderPort port, IRenderHandleOwner owner)
        {
            RENDERHANDLE handle = port.AllocHandle(owner);
            this.Init(port, handle, true);
        }

        public RemoteObject(RenderPort port, RENDERHANDLE handle, bool fFreeOnDispose) => this.Init(port, handle, fFreeOnDispose);

        private void Init(RenderPort port, RENDERHANDLE handle, bool fFreeOnDispose)
        {
            this.m_renderPort = port;
            this.m_renderHandle = handle;
            this.m_fFreeOnDispose = fFreeOnDispose;
            if (this.m_fFreeOnDispose)
                return;
            GC.SuppressFinalize(this);
        }

        protected RemoteObject()
        {
        }

        protected override void Dispose(bool fInDispose)
        {
            try
            {
                if (!fInDispose)
                    return;
                this.SafeDelete();
            }
            finally
            {
                base.Dispose(fInDispose);
            }
        }

        public bool IsValid => this.m_renderHandle != RENDERHANDLE.NULL;

        public RenderPort Port => this.m_renderPort;

        public RENDERHANDLE RenderHandle => this.m_renderHandle;

        public override bool Equals(object other) => other is RemoteObject && this.m_renderHandle == ((RemoteObject)other).m_renderHandle;

        public override int GetHashCode() => this.m_renderHandle.GetHashCode();

        private void SafeDelete()
        {
            if (this.IsValid && this.m_fFreeOnDispose && this.m_renderPort != null)
                this.m_renderPort.FreeHandle(this.m_renderHandle);
            this.m_renderHandle = RENDERHANDLE.NULL;
            this.m_renderPort = null;
        }
    }
}
