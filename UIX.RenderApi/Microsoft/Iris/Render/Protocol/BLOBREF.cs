// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocol.BLOBREF
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Protocol
{
    [ComVisible(false)]
    internal struct BLOBREF
    {
        public static readonly BLOBREF NULL = new BLOBREF();
        private uint value;

        private BLOBREF(uint value) => this.value = value;

        public static bool operator ==(BLOBREF hl, BLOBREF hr) => (int)hl.value == (int)hr.value;

        public static bool operator !=(BLOBREF hl, BLOBREF hr) => (int)hl.value != (int)hr.value;

        public override bool Equals(object oCompare) => (int)this.value == (int)(uint)oCompare;

        public override int GetHashCode() => (int)this.value;

        public static BLOBREF FromUInt32(uint value) => new BLOBREF(value);

        public static uint ToUInt32(BLOBREF handle) => handle.value;
    }
}
