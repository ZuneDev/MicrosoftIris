// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Remote.ImageHeader
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Remote
{
    [ComVisible(false)]
    public struct ImageHeader
    {
        public Size sizeActualPxl;
        public Size sizeOriginalPxl;
        public int nStride;
        public SurfaceFormat nFormat;
    }
}
