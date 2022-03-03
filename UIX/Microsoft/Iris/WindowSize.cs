// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.WindowSize
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System;

namespace Microsoft.Iris
{
    public struct WindowSize
    {
        private int _width;
        private int _height;

        public WindowSize(int width, int height)
        {
            if (width < 0)
                throw new ArgumentOutOfRangeException(nameof(width));
            if (height < 0)
                throw new ArgumentOutOfRangeException(nameof(Height));
            _width = width;
            _height = height;
        }

        public int Width => _width;

        public int Height => _height;

        public static bool operator ==(WindowSize a, WindowSize b) => a.Width == b.Width && a.Height == b.Height;

        public static bool operator !=(WindowSize a, WindowSize b) => !(a == b);

        public override bool Equals(object o) => o is WindowSize windowSize && windowSize == this;

        public override int GetHashCode() => Width ^ Height;
    }
}
