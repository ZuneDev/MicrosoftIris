// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Extensions.ImageCacheKey
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

namespace Microsoft.Iris.Render.Extensions
{
    public class ImageCacheKey
    {
        private string m_stIdentifier;
        private Size m_sizeMaxPxl;
        private bool m_fFlippable;
        private bool m_fAntialiasEdges;
        protected int m_nHashCode;

        protected ImageCacheKey(string identifier) => this.m_stIdentifier = identifier;

        public ImageCacheKey(string identifier, Size maxSize, bool flippable, bool antialiasEdges)
        {
            this.m_stIdentifier = identifier;
            this.m_sizeMaxPxl = maxSize;
            this.m_fFlippable = flippable;
            this.m_fAntialiasEdges = antialiasEdges;
            this.m_nHashCode = this.m_stIdentifier.GetHashCode() + this.m_sizeMaxPxl.Width + this.m_sizeMaxPxl.Height + (this.m_fFlippable ? 1 : 0) + (this.m_fAntialiasEdges ? 1 : 0);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;
            return obj is ImageCacheKey imageCacheKey && this.m_stIdentifier.Equals(imageCacheKey.m_stIdentifier) && (this.m_sizeMaxPxl == imageCacheKey.m_sizeMaxPxl && this.m_fFlippable == imageCacheKey.m_fFlippable) && this.m_fAntialiasEdges == imageCacheKey.m_fAntialiasEdges;
        }

        public override int GetHashCode() => this.m_nHashCode;
    }
}
