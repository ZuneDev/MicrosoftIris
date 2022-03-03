// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Extensions.HSpBitmap
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using System;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Extensions
{
    [ComVisible(false)]
    internal struct HSpBitmap
    {
        internal IntPtr h;
        internal static readonly HSpBitmap NULL = new HSpBitmap();

        public static bool operator ==(HSpBitmap hA, HSpBitmap hB) => hA.h == hB.h;

        public static bool operator !=(HSpBitmap hA, HSpBitmap hB) => hA.h != hB.h;

        public override bool Equals(object oCompare) => oCompare is HSpBitmap hspBitmap && this.h == hspBitmap.h;

        public override int GetHashCode() => (int)this.h.ToInt64();
    }
}
