// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocol.RENDERHANDLE
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using System;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Protocol
{
    [ComVisible(false)]
    [Serializable]
    internal struct RENDERHANDLE
    {
        public static readonly RENDERHANDLE NULL = new RENDERHANDLE();
        private uint value;

        private RENDERHANDLE(uint value) => this.value = value;

        public static bool operator ==(RENDERHANDLE hl, RENDERHANDLE hr) => (int)hl.value == (int)hr.value;

        public static bool operator !=(RENDERHANDLE hl, RENDERHANDLE hr) => (int)hl.value != (int)hr.value;

        public override bool Equals(object oCompare) => oCompare is RENDERHANDLE renderhandle && (int)this.value == (int)renderhandle.value;

        public override int GetHashCode() => (int)this.value;

        public static RENDERHANDLE FromUInt32(uint value) => new RENDERHANDLE(value);

        public static uint ToUInt32(RENDERHANDLE handle) => handle.value;
    }
}
