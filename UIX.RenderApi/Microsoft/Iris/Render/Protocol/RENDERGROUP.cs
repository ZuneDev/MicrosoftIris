// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocol.RENDERGROUP
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using System;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Protocol
{
    [ComVisible(false)]
    [Serializable]
    internal struct RENDERGROUP
    {
        public static readonly RENDERGROUP NULL = new RENDERGROUP();
        private uint value;

        private RENDERGROUP(uint value) => this.value = value;

        public static bool operator ==(RENDERGROUP hl, RENDERGROUP hr) => (int)hl.value == (int)hr.value;

        public static bool operator !=(RENDERGROUP hl, RENDERGROUP hr) => (int)hl.value != (int)hr.value;

        public override bool Equals(object oCompare) => oCompare is RENDERGROUP rendergroup && (int)this.value == (int)rendergroup.value;

        public override int GetHashCode() => (int)this.value;

        public static RENDERGROUP FromUInt32(uint value) => new RENDERGROUP(value);

        public static uint ToUInt32(RENDERGROUP handle) => handle.value;
    }
}
