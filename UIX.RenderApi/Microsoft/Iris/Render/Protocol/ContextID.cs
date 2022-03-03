// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocol.ContextID
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using System;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Protocol
{
    [ComVisible(false)]
    [Serializable]
    internal struct ContextID
    {
        public static readonly ContextID NULL = new ContextID();
        public static readonly ContextID CURRENT = new ContextID(uint.MaxValue);
        private uint value;

        private ContextID(uint value) => this.value = value;

        public static bool operator ==(ContextID hl, ContextID hr) => (int)hl.value == (int)hr.value;

        public static bool operator !=(ContextID hl, ContextID hr) => (int)hl.value != (int)hr.value;

        public override bool Equals(object oCompare) => oCompare is ContextID contextId && (int)this.value == (int)contextId.value;

        public override int GetHashCode() => (int)this.value;

        public static ContextID FromUInt32(uint value) => new ContextID(value);

        public static uint ToUInt32(ContextID handle) => handle.value;
    }
}
