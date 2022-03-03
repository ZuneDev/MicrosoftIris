// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Rectangle
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using System;

namespace Microsoft.Iris.Render
{
    [Serializable]
    public struct Rectangle
    {
        private int m_x;
        private int m_y;
        private int m_width;
        private int m_height;
        public static readonly Rectangle Zero = new Rectangle(0, 0, 0, 0);

        public Rectangle(int x, int y, int width, int height)
        {
            this.m_x = x;
            this.m_y = y;
            this.m_width = width;
            this.m_height = height;
        }

        public Rectangle(float x, float y, float width, float height)
        {
            this.m_x = (int)Math.Round(x);
            this.m_y = (int)Math.Round(y);
            this.m_width = (int)Math.Round(width);
            this.m_height = (int)Math.Round(height);
        }

        public Rectangle(Size extent)
          : this(Point.Zero, extent)
        {
        }

        public Rectangle(Point offset, Size extent)
        {
            this.m_x = offset.X;
            this.m_y = offset.Y;
            this.m_width = extent.Width;
            this.m_height = extent.Height;
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

        public int Width
        {
            get => this.m_width;
            set => this.m_width = value;
        }

        public int Height
        {
            get => this.m_height;
            set => this.m_height = value;
        }

        public bool Contains(Point point) => this.Contains(point.X, point.Y);

        public static Rectangle Inflate(Rectangle rectangle, int x, int y)
        {
            Rectangle rectangle1 = rectangle;
            rectangle1.Inflate(x, y);
            return rectangle1;
        }

        public static Rectangle Intersect(Rectangle left, Rectangle right)
        {
            int x = Math.Max(left.X, right.X);
            int num1 = Math.Min(left.X + left.Width, right.X + right.Width);
            int y = Math.Max(left.Y, right.Y);
            int num2 = Math.Min(left.Y + left.Height, right.Y + right.Height);
            return num1 >= x && num2 >= y ? new Rectangle(x, y, num1 - x, num2 - y) : Zero;
        }

        public static Rectangle Union(Rectangle left, Rectangle right)
        {
            int x = Math.Min(left.X, right.X);
            int num1 = Math.Max(left.X + left.Width, right.X + right.Width);
            int y = Math.Min(left.Y, right.Y);
            int num2 = Math.Max(left.Y + left.Height, right.Y + right.Height);
            return new Rectangle(x, y, num1 - x, num2 - y);
        }

        public override bool Equals(object obj) => obj is Rectangle rectangle && rectangle.X == this.X && (rectangle.Y == this.Y && rectangle.Width == this.Width) && rectangle.Height == this.Height;

        public static bool operator ==(Rectangle left, Rectangle right) => left.X == right.X && left.Y == right.Y && left.Width == right.Width && left.Height == right.Height;

        public static bool operator !=(Rectangle left, Rectangle right) => !(left == right);

        public override int GetHashCode() => this.X ^ (this.Y << 13 | (int)((uint)this.Y >> 19)) ^ (this.Width << 26 | (int)((uint)this.Width >> 6)) ^ (this.Height << 7 | (int)((uint)this.Height >> 25));

        public override string ToString() => base.ToString();

        public static Rectangle FromLTRB(int left, int top, int right, int bottom) => new Rectangle(left, top, right - left, bottom - top);

        public Point Location
        {
            get => new Point(this.X, this.Y);
            set
            {
                this.X = value.X;
                this.Y = value.Y;
            }
        }

        public Size Size
        {
            get => new Size(this.Width, this.Height);
            set
            {
                this.Width = value.Width;
                this.Height = value.Height;
            }
        }

        public int Left => this.X;

        public int Top => this.Y;

        public int Right => this.X + this.Width;

        public int Bottom => this.Y + this.Height;

        public bool IsEmpty => this.Height == 0 && this.Width == 0 && this.X == 0 && this.Y == 0;

        public int Area => this.Width * this.Height;

        public bool Contains(int x, int y) => this.X <= x && x < this.X + this.Width && this.Y <= y && y < this.Y + this.Height;

        public bool Contains(Rectangle rect) => this.X <= rect.X && rect.X + rect.Width <= this.X + this.Width && this.Y <= rect.Y && rect.Y + rect.Height <= this.Y + this.Height;

        public void Inflate(int width, int height)
        {
            this.X -= width;
            this.Y -= height;
            this.Width += 2 * width;
            this.Height += 2 * height;
        }

        public void Inflate(Size size) => this.Inflate(size.Width, size.Height);

        public static Rectangle Inflate(Rectangle source, Rectangle offset) => FromLTRB(source.Left - offset.Left, source.Top - offset.Top, source.Right + offset.Right, source.Bottom + offset.Bottom);

        public void Intersect(Rectangle rect)
        {
            Rectangle rectangle = Intersect(rect, this);
            this.X = rectangle.X;
            this.Y = rectangle.Y;
            this.Width = rectangle.Width;
            this.Height = rectangle.Height;
        }

        public bool IntersectsWith(Rectangle rect) => this.Left < rect.Right && this.Top < rect.Bottom && this.Right > rect.Left && this.Bottom > rect.Top;

        public void Union(Rectangle rect)
        {
            Rectangle rectangle = Union(rect, this);
            this.X = rectangle.X;
            this.Y = rectangle.Y;
            this.Width = rectangle.Width;
            this.Height = rectangle.Height;
        }

        public static Rectangle Offset(Rectangle rect, Point pos)
        {
            rect.Offset(pos);
            return rect;
        }

        public void Offset(Point pos) => this.Offset(pos.X, pos.Y);

        public static Rectangle Offset(Rectangle rect, int x, int y)
        {
            rect.Offset(x, y);
            return rect;
        }

        public void Offset(int x, int y)
        {
            this.X += x;
            this.Y += y;
        }

        public Point Center => new Point(this.X + this.Width / 2, this.Y + this.Height / 2);

        public bool IsZero => this.Height == 0 && this.Width == 0 && this.X == 0 && this.Y == 0;
    }
}
