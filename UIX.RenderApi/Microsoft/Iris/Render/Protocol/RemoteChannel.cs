// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocol.RemoteChannel
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using System;

namespace Microsoft.Iris.Render.Protocol
{
    internal class RemoteChannel : RenderObject, IChannel
    {
        private string m_sessionName;
        private TransportProtocol m_protocol;
        private IntPtr m_pSendStream;
        private IntPtr m_pReceiveStream;
        private IntPtr m_pSession;

        public RemoteChannel(RemoteConnectionInfo connectionInfo)
        {
            this.m_sessionName = connectionInfo.SessionName;
            this.m_protocol = connectionInfo.TransportProtocol;
            EngineApi.IFC(EngineApi.SpRemoteCreateServerStreams(this.m_sessionName, this.m_protocol, out this.m_pSendStream, out this.m_pReceiveStream));
        }

        protected override void Dispose(bool inDispose)
        {
            try
            {
                if (!(this.m_pSession != IntPtr.Zero))
                    return;
                EngineApi.IFC(EngineApi.SpRemoteServerUninit(this.m_pSession, true, out ShutdownReason _));
                this.m_pSession = IntPtr.Zero;
            }
            finally
            {
                base.Dispose(inDispose);
            }
        }

        public bool IsConnected => this.m_pSession != IntPtr.Zero;

        public void Connect(
          ContextID idRemoteContext,
          RENDERHANDLE hBrokerClass,
          MessageCookieLayout layout)
        {
            EngineApi.IFC(EngineApi.SpRemoteWaitServerStreamsConnected(this.m_protocol, this.m_pSendStream, this.m_pReceiveStream));
            EngineApi.IFC(EngineApi.SpRemoteServerInit(this.m_pSendStream, this.m_pReceiveStream, new EngineApi.InitArgs(layout, idRemoteContext)
            {
                idObjectBrokerClass = hBrokerClass
            }, out this.m_pSession));
            EngineApi.SpObjectRelease(this.m_pSendStream);
            EngineApi.SpObjectRelease(this.m_pReceiveStream);
        }
    }
}
