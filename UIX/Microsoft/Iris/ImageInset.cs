// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.ImageInset
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Render;

namespace Microsoft.Iris
{
    public struct ImageInset
    {
        private int _left;
        private int _top;
        private int _right;
        private int _bottom;

        public ImageInset(int left, int top, int right, int bottom)
        {
            _left = left;
            _top = top;
            _right = right;
            _bottom = bottom;
        }

        public int Left => _left;

        public int Top => _top;

        public int Right => _right;

        public int Bottom => _bottom;

        internal Inset ToInset() => new Inset(_left, _top, _right, _bottom);
    }
}
