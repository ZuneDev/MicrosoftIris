// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocol.ProtocolInstance
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

namespace Microsoft.Iris.Render.Protocol
{
    internal abstract class ProtocolInstance
    {
        private RenderPort m_port;
        private string m_name;

        protected ProtocolInstance(RenderPort port, string name)
        {
            this.m_port = port;
            this.m_name = name;
        }

        public RenderPort Port => this.m_port;

        public string Name => this.m_name;

        internal void OnConnect() => this.Init();

        protected abstract void Init();
    }
}
