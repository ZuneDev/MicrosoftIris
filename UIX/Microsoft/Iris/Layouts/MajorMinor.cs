// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Layouts.MajorMinor
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;
using Microsoft.Iris.Render;
using Microsoft.Iris.RenderAPI.Drawing;
using System;

namespace Microsoft.Iris.Layouts
{
    internal struct MajorMinor : IStringEncodable
    {
        private int major;
        private int minor;
        public static readonly MajorMinor Zero = new MajorMinor(0, 0);

        public MajorMinor(int major, int minor)
        {
            this.major = major;
            this.minor = minor;
        }

        public MajorMinor(Size size, Orientation o)
        {
            switch (o)
            {
                case Orientation.Horizontal:
                    major = size.Width;
                    minor = size.Height;
                    break;
                default:
                    major = size.Height;
                    minor = size.Width;
                    break;
            }
        }

        public SizeF ToSizeF(Orientation o)
        {
            switch (o)
            {
                case Orientation.Horizontal:
                    return new SizeF(major, minor);
                default:
                    return new SizeF(minor, major);
            }
        }

        public Size ToSize(Orientation o)
        {
            switch (o)
            {
                case Orientation.Horizontal:
                    return new Size(major, minor);
                default:
                    return new Size(minor, major);
            }
        }

        public Point ToPoint(Orientation o) => ToSize(o).ToPoint();

        public static MajorMinor Min(MajorMinor a, MajorMinor b) => new MajorMinor(Math.Min(a.Major, b.Major), Math.Min(a.Minor, b.Minor));

        public static MajorMinor Max(MajorMinor a, MajorMinor b) => new MajorMinor(Math.Max(a.Major, b.Major), Math.Max(a.Minor, b.Minor));

        public static MajorMinor Clamp(MajorMinor original, MajorMinor min, MajorMinor max) => new MajorMinor(Math2.Clamp(original.Major, min.Major, max.Major), Math2.Clamp(original.Minor, min.Minor, max.Minor));

        public static MajorMinor operator +(MajorMinor left, MajorMinor right) => new MajorMinor(left.Major + right.Major, left.Minor + right.Minor);

        public static MajorMinor operator -(MajorMinor left, MajorMinor right) => new MajorMinor(left.Major - right.Major, left.Minor - right.Minor);

        public static MajorMinor operator *(MajorMinor left, MajorMinor right) => new MajorMinor(left.Major * right.Major, left.Minor * right.Minor);

        public static MajorMinor operator /(MajorMinor left, MajorMinor right) => new MajorMinor(left.Major / right.Major, left.Minor / right.Minor);

        public static bool operator ==(MajorMinor left, MajorMinor right) => left.Major == right.Major && left.Minor == right.Minor;

        public static bool operator !=(MajorMinor left, MajorMinor right) => !(left == right);

        public override bool Equals(object obj) => obj is MajorMinor majorMinor && majorMinor == this;

        public override int GetHashCode() => major ^ minor;

        public MajorMinor Swap() => new MajorMinor(Minor, Major);

        public int Major
        {
            get => major;
            set => major = value;
        }

        public int Minor
        {
            get => minor;
            set => minor = value;
        }

        public bool IsEmpty => Major == 0 || Minor == 0;

        public override string ToString() => InvariantString.Format("(Major={0}, Minor={1})", Major, Minor);

        public string EncodeString() => $"{Major}, {Minor}";
    }
}
