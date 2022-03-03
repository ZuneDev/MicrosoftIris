// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocol.LocalChannel
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using System;

namespace Microsoft.Iris.Render.Protocol
{
    internal class LocalChannel : RenderObject, IChannel
    {
        private IntPtr m_renderThread;

        public LocalChannel(LocalConnectionInfo connectionInfo)
        {
        }

        public bool IsConnected => this.m_renderThread != IntPtr.Zero;

        public void Connect(
          ContextID remoteContextId,
          RENDERHANDLE brokerClassHandle,
          MessageCookieLayout layout)
        {
            Debug2.Validate(remoteContextId != ContextID.NULL, typeof(ArgumentNullException), nameof(remoteContextId));
            var args = new EngineApi.InitArgs(layout, remoteContextId)
            {
                idObjectBrokerClass = brokerClassHandle
            };
            EngineApi.IFC(EngineApi.SpRenderThreadInit(ref args, out this.m_renderThread));
        }

        protected override void Dispose(bool inDispose)
        {
            try
            {
                if (!(this.m_renderThread != IntPtr.Zero))
                    return;
                EngineApi.IFC(EngineApi.SpRenderThreadUninit(this.m_renderThread));
                this.m_renderThread = IntPtr.Zero;
            }
            finally
            {
                base.Dispose(inDispose);
            }
        }
    }
}
