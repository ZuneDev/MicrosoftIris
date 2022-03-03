// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.WindowPosition
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

namespace Microsoft.Iris
{
    public struct WindowPosition
    {
        private int _x;
        private int _y;

        public WindowPosition(int x, int y)
        {
            _x = x;
            _y = y;
        }

        public int X => _x;

        public int Y => _y;

        public static bool operator ==(WindowPosition a, WindowPosition b) => a.X == b.X && a.Y == b.Y;

        public static bool operator !=(WindowPosition a, WindowPosition b) => !(a == b);

        public override bool Equals(object o) => o is WindowPosition windowPosition && windowPosition == this;

        public override int GetHashCode() => X ^ Y;
    }
}
