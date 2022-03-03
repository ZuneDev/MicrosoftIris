// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocol.RemoteConnectionInfoBase
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using System;

namespace Microsoft.Iris.Render.Protocol
{
    [Serializable]
    internal abstract class RemoteConnectionInfoBase : ConnectionInfo
    {
        private string m_sessionName;
        private TransportProtocol m_protocol;
        private bool m_swapByteOrder;

        public RemoteConnectionInfoBase(
          TransportProtocol protocol,
          string sessionName,
          bool swapByteOrder)
        {
            Debug2.Validate(protocol >= TransportProtocol.Min && protocol <= TransportProtocol.PIPE, typeof(ArgumentException), nameof(protocol));
            Debug2.Validate(sessionName != null, typeof(ArgumentNullException), nameof(sessionName));
            this.m_protocol = protocol;
            this.m_sessionName = sessionName;
            this.m_swapByteOrder = swapByteOrder;
        }

        public string SessionName => this.m_sessionName;

        public TransportProtocol TransportProtocol => this.m_protocol;

        public bool SwapByteOrder => this.m_swapByteOrder;
    }
}
