// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocol.ConnectionInfo
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using System;

namespace Microsoft.Iris.Render.Protocol
{
    [Serializable]
    internal abstract class ConnectionInfo
    {
        public static readonly ConnectionInfo Default = new LocalConnectionInfo();
    }
}
