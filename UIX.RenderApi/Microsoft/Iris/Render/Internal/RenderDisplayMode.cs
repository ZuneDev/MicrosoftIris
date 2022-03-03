// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Internal.RenderDisplayMode
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Internal
{
    [ComVisible(false)]
    internal struct RenderDisplayMode
    {
        public Size sizePhysicalPxl;
        public Size sizeLogicalPxl;
        public SurfaceFormat nFormat;
        public int nRefreshRate;
        private RenderDisplayMode.Flags m_nFlags;

        public bool Interlaced
        {
            get => Bits.TestFlag((uint)this.m_nFlags, 1U);
            set
            {
                uint nFlags = (uint)this.m_nFlags;
                Bits.ChangeFlag(ref nFlags, value, 1U);
                this.m_nFlags = (RenderDisplayMode.Flags)nFlags;
            }
        }

        [System.Flags]
        public enum Flags : uint
        {
            Interlaced = 1,
        }
    }
}
