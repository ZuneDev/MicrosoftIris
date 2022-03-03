// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Inset
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

namespace Microsoft.Iris.Render
{
    public struct Inset
    {
        private int _left;
        private int _top;
        private int _right;
        private int _bottom;
        public static readonly Inset Zero = new Inset(0, 0, 0, 0);

        public Inset(int left, int top, int right, int bottom)
        {
            this._left = left;
            this._top = top;
            this._right = right;
            this._bottom = bottom;
        }

        internal Inset(Point topLeft, Point rightBottom)
        {
            this._left = topLeft.X;
            this._top = topLeft.Y;
            this._right = rightBottom.X;
            this._bottom = rightBottom.Y;
        }

        public int Left
        {
            get => this._left;
            set => this._left = value;
        }

        public int Top
        {
            get => this._top;
            set => this._top = value;
        }

        public int Right
        {
            get => this._right;
            set => this._right = value;
        }

        public int Bottom
        {
            get => this._bottom;
            set => this._bottom = value;
        }

        public static Inset operator +(Inset left, Inset right) => new Inset(left.Left + right.Left, left.Top + right.Top, left.Right + right.Right, left.Bottom + right.Bottom);

        public static Inset Add(Inset left, Inset right) => left + right;

        public static Inset operator -(Inset left, Inset right) => new Inset(left.Left - right.Left, left.Top - right.Top, left.Right - right.Right, left.Bottom - right.Bottom);

        public static Inset operator -(Inset left) => new Inset(-left.Left, -left.Top, -left.Right, -left.Bottom);

        public static Inset Subtract(Inset left, Inset right) => left - right;

        public override bool Equals(object obj) => obj is Inset inset && this == inset;

        public static bool operator ==(Inset left, Inset right) => left.Left == right.Left && left.Top == right.Top && left.Right == right.Right && left.Bottom == right.Bottom;

        public static bool operator !=(Inset left, Inset right) => !(left == right);

        public override int GetHashCode() => this.Left ^ (this.Top << 13 | (int)((uint)this.Top >> 19)) ^ (this.Right << 26 | (int)((uint)this.Right >> 6)) ^ (this.Bottom << 7 | (int)((uint)this.Bottom >> 25));

        public override string ToString() => base.ToString();

        internal Point TopLeft
        {
            get => new Point(this.Left, this.Top);
            set
            {
                this.Left = value.X;
                this.Top = value.Y;
            }
        }

        internal Point BottomRight
        {
            get => new Point(this.Right, this.Bottom);
            set
            {
                this.Right = value.X;
                this.Bottom = value.Y;
            }
        }

        public Size Size => new Size(this.Left + this.Right, this.Top + this.Bottom);

        public Rectangle ToRect => new Rectangle(this.Left, this.Right, this.Top, this.Bottom);
    }
}
