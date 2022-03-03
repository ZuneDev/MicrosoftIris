// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.RenderAPI.Drawing.RectangleF
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;
using Microsoft.Iris.Render;
using System;
using System.Globalization;
using System.Text;

namespace Microsoft.Iris.RenderAPI.Drawing
{
    [Serializable]
    internal struct RectangleF
    {
        public static readonly RectangleF Zero = new RectangleF(0.0f, 0.0f, 0.0f, 0.0f);
        private float x;
        private float y;
        private float width;
        private float height;

        public RectangleF(float x, float y, float width, float height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        public RectangleF(PointF location, SizeF size)
        {
            x = location.X;
            y = location.Y;
            width = size.Width;
            height = size.Height;
        }

        public RectangleF(Point location, Microsoft.Iris.Render.Size size)
        {
            x = location.X;
            y = location.Y;
            width = size.Width;
            height = size.Height;
        }

        public static RectangleF FromLTRB(float left, float top, float right, float bottom) => new RectangleF(left, top, right - left, bottom - top);

        public static RectangleF FromRectangle(Rectangle r) => new RectangleF(r.X, r.Y, r.Width, r.Height);

        public PointF Location
        {
            get => new PointF(X, Y);
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }

        public SizeF Size
        {
            get => new SizeF(Width, Height);
            set
            {
                Width = value.Width;
                Height = value.Height;
            }
        }

        public float X
        {
            get => x;
            set => x = value;
        }

        public float Y
        {
            get => y;
            set => y = value;
        }

        public float Width
        {
            get => width;
            set => width = value;
        }

        public float Height
        {
            get => height;
            set => height = value;
        }

        public float Left => X;

        public float Top => Y;

        public float Right => X + Width;

        public float Bottom => Y + Height;

        public bool IsEmpty => Math2.WithinEpsilon(width, 0.0f) || Math2.WithinEpsilon(height, 0.0f);

        public override bool Equals(object obj) => obj is RectangleF rectangleF && rectangleF.X == (double)X && (rectangleF.Y == (double)Y && rectangleF.Width == (double)Width) && rectangleF.Height == (double)Height;

        public static bool operator ==(RectangleF left, RectangleF right) => left.X == (double)right.X && left.Y == (double)right.Y && left.Width == (double)right.Width && left.Height == (double)right.Height;

        public static bool operator !=(RectangleF left, RectangleF right) => !(left == right);

        public bool Contains(float x, float y) => X <= (double)x && x < X + (double)Width && Y <= (double)y && y < Y + (double)Height;

        public bool Contains(PointF pt) => Contains(pt.X, pt.Y);

        public bool Contains(RectangleF rect) => X <= (double)rect.X && rect.X + (double)rect.Width <= X + (double)Width && Y <= (double)rect.Y && rect.Y + (double)rect.Height <= Y + (double)Height;

        public override int GetHashCode() => (int)(uint)X ^ ((int)(uint)Y << 13 | (int)((uint)Y >> 19)) ^ ((int)(uint)Width << 26 | (int)((uint)Width >> 6)) ^ ((int)(uint)Height << 7 | (int)((uint)Height >> 25));

        public void Inflate(float x, float y)
        {
            X -= x;
            Y -= y;
            Width += 2f * x;
            Height += 2f * y;
        }

        public void Inflate(SizeF size) => Inflate(size.Width, size.Height);

        public static RectangleF Inflate(RectangleF rect, float x, float y)
        {
            RectangleF rectangleF = rect;
            rectangleF.Inflate(x, y);
            return rectangleF;
        }

        public void Intersect(RectangleF rect)
        {
            RectangleF rectangleF = Intersect(rect, this);
            X = rectangleF.X;
            Y = rectangleF.Y;
            Width = rectangleF.Width;
            Height = rectangleF.Height;
        }

        public static RectangleF Intersect(RectangleF a, RectangleF b)
        {
            float x = Math.Max(a.X, b.X);
            float num1 = Math.Min(a.X + a.Width, b.X + b.Width);
            float y = Math.Max(a.Y, b.Y);
            float num2 = Math.Min(a.Y + a.Height, b.Y + b.Height);
            return num1 >= (double)x && num2 >= (double)y ? new RectangleF(x, y, num1 - x, num2 - y) : Zero;
        }

        public bool IntersectsWith(RectangleF rect) => Left < (double)rect.Right && Top < (double)rect.Bottom && Right > (double)rect.Left && Bottom > (double)rect.Top;

        public static RectangleF Union(RectangleF a, RectangleF b)
        {
            float x = Math.Min(a.X, b.X);
            float num1 = Math.Max(a.X + a.Width, b.X + b.Width);
            float y = Math.Min(a.Y, b.Y);
            float num2 = Math.Max(a.Y + a.Height, b.Y + b.Height);
            return new RectangleF(x, y, num1 - x, num2 - y);
        }

        public void Offset(PointF pos) => Offset(pos.X, pos.Y);

        public void Offset(float x, float y)
        {
            X += x;
            Y += y;
        }

        public static RectangleF Offset(RectangleF rect, PointF pos)
        {
            rect.Offset(pos);
            return rect;
        }

        public PointF TopLeft => new PointF(Left, Top);

        public PointF TopRight => new PointF(Right, Top);

        public PointF BottomLeft => new PointF(Left, Bottom);

        public PointF BottomRight => new PointF(Right, Bottom);

        public PointF Center => new PointF(x + width / 2f, y + height / 2f);

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder(128);
            stringBuilder.Append("(X=");
            stringBuilder.Append(X.ToString(NumberFormatInfo.InvariantInfo));
            stringBuilder.Append(", Y=");
            stringBuilder.Append(Y.ToString(NumberFormatInfo.InvariantInfo));
            stringBuilder.Append(", Width=");
            stringBuilder.Append(Width.ToString(NumberFormatInfo.InvariantInfo));
            stringBuilder.Append(", Height=");
            stringBuilder.Append(Height.ToString(NumberFormatInfo.InvariantInfo));
            stringBuilder.Append(")");
            return stringBuilder.ToString();
        }
    }
}
