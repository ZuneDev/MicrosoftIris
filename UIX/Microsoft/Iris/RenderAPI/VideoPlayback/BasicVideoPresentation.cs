// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.RenderAPI.VideoPlayback.BasicVideoPresentation
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.RenderAPI.Drawing;

namespace Microsoft.Iris.RenderAPI.VideoPlayback
{
    internal class BasicVideoPresentation
    {
        private BasicVideoGeometry m_geometry;

        internal BasicVideoPresentation(BasicVideoGeometry geometry) => m_geometry = geometry;

        public RectangleF DisplayedSource => m_geometry.rcfSrcVideoBounds;

        public RectangleF DisplayedDestination => m_geometry.rcfDestViewBounds;

        public BasicVideoGeometry GetGeometry() => new BasicVideoGeometry()
        {
            arrcfSrcVideo = (RectangleF[])m_geometry.arrcfSrcVideo.Clone(),
            arrcfDestView = (RectangleF[])m_geometry.arrcfDestView.Clone(),
            arrcfBorders = (RectangleF[])m_geometry.arrcfBorders.Clone(),
            rcfSrcVideoBounds = m_geometry.rcfSrcVideoBounds,
            rcfDestViewBounds = m_geometry.rcfDestViewBounds
        };
    }
}
