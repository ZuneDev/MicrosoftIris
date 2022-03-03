// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.HWND
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using System;

namespace Microsoft.Iris.Render
{
    [Serializable]
    public struct HWND
    {
        public IntPtr h;

        public HWND(IntPtr hwnd) => this.h = hwnd;

        public static HWND NULL => new HWND()
        {
            h = IntPtr.Zero
        };

        public static HWND HWND_TOP => new HWND()
        {
            h = new IntPtr(0)
        };

        public static HWND HWND_BOTTOM => new HWND()
        {
            h = new IntPtr(1)
        };

        public static HWND HWND_TOPMOST => new HWND()
        {
            h = new IntPtr(-1)
        };

        public static HWND HWND_NOTOPMOST => new HWND()
        {
            h = new IntPtr(-2)
        };

        public static HWND HWND_MESSAGE => new HWND()
        {
            h = new IntPtr(-3)
        };

        public static bool operator ==(HWND hl, HWND hr) => hl.h == hr.h;

        public static bool operator !=(HWND hl, HWND hr) => hl.h != hr.h;

        public override bool Equals(object oCompare) => this.h == (IntPtr)oCompare;

        public override int GetHashCode() => (int)this.h.ToInt64();

        public override string ToString() => base.ToString();
    }
}
