// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.RenderAPI.Drawing.SizeF
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System;
using System.Globalization;
using System.Text;

namespace Microsoft.Iris.RenderAPI.Drawing
{
    [Serializable]
    public struct SizeF
    {
        private float width;
        private float height;
        public static readonly SizeF Zero = new SizeF(0.0f, 0.0f);

        public SizeF(SizeF size)
        {
            width = size.width;
            height = size.height;
        }

        public SizeF(PointF pt)
        {
            width = pt.X;
            height = pt.Y;
        }

        public SizeF(float width, float height)
        {
            this.width = width;
            this.height = height;
        }

        public static SizeF operator +(SizeF sz1, SizeF sz2) => new SizeF(sz1.Width + sz2.Width, sz1.Height + sz2.Height);

        public static SizeF operator -(SizeF sz1, SizeF sz2) => new SizeF(sz1.Width - sz2.Width, sz1.Height - sz2.Height);

        public static bool operator ==(SizeF sz1, SizeF sz2) => sz1.Width == (double)sz2.Width && sz1.Height == (double)sz2.Height;

        public static bool operator !=(SizeF sz1, SizeF sz2) => !(sz1 == sz2);

        public PointF ToPointF() => new PointF(Width, Height);

        internal bool IsZero => Width == 0.0 && Height == 0.0;

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

        public void Scale(float flScale)
        {
            Width *= flScale;
            Height *= flScale;
        }

        public static SizeF Scale(SizeF size, float flScale)
        {
            SizeF sizeF = size;
            sizeF.Scale(flScale);
            return sizeF;
        }

        public override bool Equals(object obj) => obj is SizeF sizeF && sizeF.Width == (double)Width && sizeF.Height == (double)Height;

        public bool Equals(SizeF comp) => comp.Width == (double)Width && comp.Height == (double)Height;

        public override int GetHashCode() => width.GetHashCode() ^ height.GetHashCode();

        public static SizeF Min(SizeF sz1, SizeF sz2) => new SizeF(Math.Min(sz1.Width, sz2.Width), Math.Min(sz1.Height, sz2.Height));

        public static SizeF Max(SizeF sz1, SizeF sz2) => new SizeF(Math.Max(sz1.Width, sz2.Width), Math.Max(sz1.Height, sz2.Height));

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder(32);
            stringBuilder.Append("(Width=");
            stringBuilder.Append(Width.ToString(NumberFormatInfo.InvariantInfo));
            stringBuilder.Append(", Height=");
            stringBuilder.Append(Height.ToString(NumberFormatInfo.InvariantInfo));
            stringBuilder.Append(")");
            return stringBuilder.ToString();
        }
    }
}
