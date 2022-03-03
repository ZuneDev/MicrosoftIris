// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Point
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using System;

namespace Microsoft.Iris.Render
{
    [Serializable]
    public struct Point
    {
        private int m_x;
        private int m_y;
        public static readonly Point Zero = new Point(0, 0);

        public Point(int x, int y)
        {
            this.m_x = x;
            this.m_y = y;
        }

        public Point(Size size)
        {
            this.m_x = size.Width;
            this.m_y = size.Height;
        }

        public int X
        {
            get => this.m_x;
            set => this.m_x = value;
        }

        public int Y
        {
            get => this.m_y;
            set => this.m_y = value;
        }

        public int GetDimension(Orientation orientation) => orientation != Orientation.Horizontal ? this.Y : this.X;

        public void SetDimension(Orientation orientation, int value)
        {
            if (orientation == Orientation.Horizontal)
                this.X = value;
            else
                this.Y = value;
        }

        public static Point operator +(Point left, Point right) => new Point(left.X + right.X, left.Y + right.Y);

        public static Point Add(Point left, Point right) => left + right;

        public static Point operator -(Point left, Point right) => new Point(left.X - right.X, left.Y - right.Y);

        public static Point operator -(Point pt) => new Point(-pt.X, -pt.Y);

        public static Point Subtract(Point left, Point right) => left - right;

        public static Point Offset(Point point, int x, int y) => new Point(point.X + x, point.Y + y);

        public override bool Equals(object obj) => obj is Point point && point.X == this.X && point.Y == this.Y;

        public bool Equals(Point comp) => comp.X == this.X && comp.Y == this.Y;

        public static bool operator ==(Point left, Point right) => left.X == right.X && left.Y == right.Y;

        public static bool operator !=(Point left, Point right) => !(left == right);

        public override int GetHashCode() => this.X ^ this.Y;

        public override string ToString() => base.ToString();

        public bool IsZero => this.Equals(Zero);

        public void Offset(int dx, int dy)
        {
            this.X += dx;
            this.Y += dy;
        }

        public Size ToSize() => new Size(this.X, this.Y);

        public static Point Negate(Point point) => new Point(-point.X, -point.Y);
    }
}
