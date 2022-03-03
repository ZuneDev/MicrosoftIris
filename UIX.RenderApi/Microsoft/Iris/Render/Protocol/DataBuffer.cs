// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocol.DataBuffer
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Protocols.Splash.Messaging;
using System;

namespace Microsoft.Iris.Render.Protocol
{
    internal class DataBuffer : IDataBufferCallback, IRenderHandleOwner
    {
        private RenderPort m_port;
        private RemoteDataBuffer m_remoteBuffer;
        private unsafe void* m_pvData;
        private uint m_cbSize;

        public unsafe DataBuffer(RenderPort port, void* pvData, uint cbSize)
        {
            this.m_port = port;
            this.m_remoteBuffer = null;
            this.m_pvData = pvData;
            this.m_cbSize = cbSize;
        }

        ~DataBuffer() => this.Dispose(false);

        public void Dispose()
        {
            this.m_port.Session.AssertOwningThread();
            GC.SuppressFinalize(this);
            this.Dispose(true);
        }

        private void Dispose(bool fInDispose)
        {
            if (fInDispose && this.m_remoteBuffer != null)
                this.m_remoteBuffer.Dispose();
            this.m_remoteBuffer = null;
        }

        public RenderPort Port => this.m_port;

        public uint DataSize => this.m_cbSize;

        public unsafe void* Data => this.m_pvData;

        public RemoteDataBuffer RemoteStub => this.m_remoteBuffer;

        public unsafe void Commit()
        {
            RENDERHANDLE renderhandle = this.m_port.AllocHandle(this);
            try
            {
                this.m_port.SendDataBuffer(this.m_pvData, this.m_cbSize, renderhandle);
                this.m_remoteBuffer = RemoteDataBuffer.CreateFromHandle(this.m_port, renderhandle);
                this.m_remoteBuffer.SendRegisterOwner(this.m_port.MessagingProtocol.LocalDataBufferCallbackHandle);
                renderhandle = RENDERHANDLE.NULL;
            }
            finally
            {
                if (renderhandle != RENDERHANDLE.NULL)
                {
                    if (this.m_remoteBuffer != null)
                    {
                        this.m_remoteBuffer.Dispose();
                        this.m_remoteBuffer = null;
                    }
                    else
                        this.m_port.FreeHandle(renderhandle);
                }
            }
        }

        public event EventHandler DataConsumed;

        protected virtual void OnDataConsumed(object sender, EventArgs args)
        {
            this.OnReleaseLocalData();
            if (this.DataConsumed == null)
                return;
            this.DataConsumed(sender, args);
        }

        protected void OnDataConsumed() => this.OnDataConsumed(this, EventArgs.Empty);

        public event EventHandler ReleaseLocalData;

        protected virtual void OnReleaseLocalData(object sender, EventArgs args)
        {
            if (this.ReleaseLocalData == null)
                return;
            this.ReleaseLocalData(sender, args);
        }

        protected void OnReleaseLocalData() => this.OnReleaseLocalData(this, EventArgs.Empty);

        RENDERHANDLE IRenderHandleOwner.RenderHandle => this.m_remoteBuffer.RenderHandle;

        void IRenderHandleOwner.OnDisconnect() => this.m_remoteBuffer = null;

        void IDataBufferCallback.OnComplete(RENDERHANDLE target) => this.OnDataConsumed();
    }
}
