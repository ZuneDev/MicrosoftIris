// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Size
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using System;

namespace Microsoft.Iris.Render
{
    public struct Size : IStringEncodable
    {
        private int m_width;
        private int m_height;
        public static readonly Size Zero = new Size(0, 0);
        public static readonly object ZeroBox = Zero;

        public Size(int width, int height)
        {
            this.m_width = width;
            this.m_height = height;
        }

        public Size(Point point)
        {
            this.m_width = point.X;
            this.m_height = point.Y;
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

        public int GetDimension(Orientation orientation) => orientation != Orientation.Horizontal ? this.Height : this.Width;

        public void SetDimension(Orientation orientation, int value)
        {
            if (orientation == Orientation.Horizontal)
                this.Width = value;
            else
                this.Height = value;
        }

        public static Size operator +(Size left, Size right) => new Size(left.Width + right.Width, left.Height + right.Height);

        public static Size Add(Size left, Size right) => left + right;

        public static Size operator -(Size left, Size right) => new Size(left.Width - right.Width, left.Height - right.Height);

        public static Size Subtract(Size left, Size right) => left - right;

        public void Deflate(Size amount)
        {
            this.Width = Math.Max(0, this.Width - amount.Width);
            this.Height = Math.Max(0, this.Height - amount.Height);
        }

        public static bool operator ==(Size left, Size right) => left.Width == right.Width && left.Height == right.Height;

        public static bool operator !=(Size left, Size right) => !(left == right);

        public override bool Equals(object obj) => obj is Size size && size.Width == this.Width && size.Height == this.Height;

        public bool Equals(Size comp) => comp.Width == this.Width && comp.Height == this.Height;

        public override int GetHashCode() => this.Width ^ this.Height;

        public override string ToString() => $"({Width} x {Height})";

        public Point ToPoint() => new Point(this.Width, this.Height);

        public static Size Min(Size sz1, Size sz2) => new Size(Math.Min(sz1.Width, sz2.Width), Math.Min(sz1.Height, sz2.Height));

        public static Size Max(Size sz1, Size sz2) => new Size(Math.Max(sz1.Width, sz2.Width), Math.Max(sz1.Height, sz2.Height));

        public bool IsZero => this.Equals(Zero);

        public bool IsEmpty => this.Width == 0 || this.Height == 0;

        public static Size LargestFit(Size source, Size bounds)
        {
            float num = source.Width / (float)source.Height;
            Size size = new Size(bounds.Width, (int)Math.Ceiling(bounds.Width / (double)num));
            if (size.Height > bounds.Height)
                size = new Size((int)Math.Ceiling(bounds.Height * (double)num), bounds.Height);
            return size;
        }

        public void Scale(float flScale)
        {
            this.Width = (int)(Width * (double)flScale);
            this.Height = (int)(Height * (double)flScale);
        }

        public static Size Scale(Size size, float flScale)
        {
            Size size1 = size;
            size1.Scale(flScale);
            return size1;
        }

        public string EncodeString() => $"{Width}, {Height}";
    }
}
