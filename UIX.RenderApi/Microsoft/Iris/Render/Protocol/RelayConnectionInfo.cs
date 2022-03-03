// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocol.RelayConnectionInfo
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using System;

namespace Microsoft.Iris.Render.Protocol
{
    [Serializable]
    internal sealed class RelayConnectionInfo : RemoteConnectionInfoBase
    {
        private string m_serverName;

        public RelayConnectionInfo(RemoteConnectionInfo serverInfo, string serverName)
          : base(serverInfo.TransportProtocol, serverInfo.SessionName, serverInfo.SwapByteOrder)
        {
            Debug2.Validate(serverName != null, typeof(ArgumentNullException), nameof(serverName));
            this.m_serverName = serverName;
        }

        public string Server => this.m_serverName;
    }
}
