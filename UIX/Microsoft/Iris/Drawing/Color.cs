// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Drawing.Color
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Render;
using System;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.Iris.Drawing
{
    [Serializable]
    public struct Color : ISerializable, IStringEncodable
    {
        private const int ARGBAlphaShift = 24;
        private const int ARGBRedShift = 16;
        private const int ARGBGreenShift = 8;
        private const int ARGBBlueShift = 0;
        private readonly uint value;

        public Color(int red, int green, int blue) => this = FromArgb(byte.MaxValue, red, green, blue);

        public Color(float red, float green, float blue) => this = FromArgb(1f, red, green, blue);

        public Color(int alpha, int red, int green, int blue) => this = FromArgb(alpha, red, green, blue);

        public Color(float alpha, float red, float green, float blue) => this = FromArgb(alpha, red, green, blue);

        internal Color(uint value) => this.value = value;

        internal Color(SerializationInfo info, StreamingContext context)
        {
            value = info.GetUInt32(nameof(Value));
        }

        public byte R
        {
            get => (byte)(Value >> 16 & byte.MaxValue);
            set => this = FromArgb(A, value, G, B);
        }

        public byte G
        {
            get => (byte)(Value >> 8 & byte.MaxValue);
            set => this = FromArgb(A, R, value, B);
        }

        public byte B
        {
            get => (byte)(Value & byte.MaxValue);
            set => this = FromArgb(A, R, G, value);
        }

        public byte A
        {
            get => (byte)(Value >> 24 & byte.MaxValue);
            set => this = FromArgb(value, R, G, B);
        }

        internal void GetArgb(out float a, out float r, out float g, out float b)
        {
            a = A / (float)byte.MaxValue;
            r = R / (float)byte.MaxValue;
            g = G / (float)byte.MaxValue;
            b = B / (float)byte.MaxValue;
        }

        internal uint Value => value;

        private static void CheckByte(int value, string name)
        {
        }

        private static int ChannelFromFloat(float value) => (int)(value * (double)byte.MaxValue);

        private static uint MakeArgb(byte alpha, byte red, byte green, byte blue) => (uint)(red << 16 | green << 8 | blue | alpha << 24);

        internal static Color FromArgb(uint argb) => new Color(argb);

        internal static Color FromArgb(int alpha, int red, int green, int blue)
        {
            CheckByte(alpha, nameof(alpha));
            CheckByte(red, nameof(red));
            CheckByte(green, nameof(green));
            CheckByte(blue, nameof(blue));
            return new Color(MakeArgb((byte)alpha, (byte)red, (byte)green, (byte)blue));
        }

        internal static Color FromArgb(float alpha, float red, float green, float blue) => FromArgb(ChannelFromFloat(alpha), ChannelFromFloat(red), ChannelFromFloat(green), ChannelFromFloat(blue));

        internal static Color FromArgb(int alpha, Color baseColor)
        {
            CheckByte(alpha, nameof(alpha));
            return new Color(MakeArgb((byte)alpha, baseColor.R, baseColor.G, baseColor.B));
        }

        internal static Color FromArgb(int red, int green, int blue) => FromArgb(byte.MaxValue, red, green, blue);

        internal float GetValue()
        {
            float num1 = R / (float)byte.MaxValue;
            float num2 = G / (float)byte.MaxValue;
            float num3 = B / (float)byte.MaxValue;
            float num4 = num1;
            float num5 = num1;
            if (num2 > (double)num4)
                num4 = num2;
            if (num3 > (double)num4)
                num4 = num3;
            if (num2 < (double)num5)
                num5 = num2;
            if (num3 < (double)num5)
                num5 = num3;
            return (float)((num4 + (double)num5) / 2.0);
        }

        internal float GetHue()
        {
            if (R == G && G == B)
                return 0.0f;
            float num1 = R / (float)byte.MaxValue;
            float num2 = G / (float)byte.MaxValue;
            float num3 = B / (float)byte.MaxValue;
            float num4 = 0.0f;
            float num5 = num1;
            float num6 = num1;
            if (num2 > (double)num5)
                num5 = num2;
            if (num3 > (double)num5)
                num5 = num3;
            if (num2 < (double)num6)
                num6 = num2;
            if (num3 < (double)num6)
                num6 = num3;
            float num7 = num5 - num6;
            if (num1 == (double)num5)
                num4 = (num2 - num3) / num7;
            else if (num2 == (double)num5)
                num4 = (float)(2.0 + (num3 - (double)num1) / num7);
            else if (num3 == (double)num5)
                num4 = (float)(4.0 + (num1 - (double)num2) / num7);
            float num8 = num4 * 60f;
            if (num8 < 0.0)
                num8 += 360f;
            return num8;
        }

        internal float GetSaturation()
        {
            float num1 = R / (float)byte.MaxValue;
            float num2 = G / (float)byte.MaxValue;
            float num3 = B / (float)byte.MaxValue;
            float num4 = 0.0f;
            float num5 = num1;
            float num6 = num1;
            if (num2 > (double)num5)
                num5 = num2;
            if (num3 > (double)num5)
                num5 = num3;
            if (num2 < (double)num6)
                num6 = num2;
            if (num3 < (double)num6)
                num6 = num3;
            if (num5 != (double)num6)
                num4 = (num5 + (double)num6) / 2.0 > 0.5 ? (float)((num5 - (double)num6) / (2.0 - num5 - num6)) : (float)((num5 - (double)num6) / (num5 + (double)num6));
            return num4;
        }

        internal static Color FromHSV(int nAlpha, float flHue, float flSaturation, float flValue)
        {
            if (flSaturation == 0.0)
                return new Color(nAlpha, (int)(flValue * (double)byte.MaxValue), (int)(flValue * (double)byte.MaxValue), (int)(flValue * (double)byte.MaxValue));
            float num1 = flHue / 60f;
            int num2 = (int)Math.Floor(num1) % 6;
            float num3 = num1 - num2;
            float num4 = flValue * (1f - flSaturation);
            float num5 = flValue * (float)(1.0 - num3 * (double)flSaturation);
            float num6 = flValue * (float)(1.0 - (1.0 - num3) * flSaturation);
            switch (num2)
            {
                case 0:
                    return new Color(nAlpha, (int)(flValue * (double)byte.MaxValue), (int)(num6 * (double)byte.MaxValue), (int)(num4 * (double)byte.MaxValue));
                case 1:
                    return new Color(nAlpha, (int)(num5 * (double)byte.MaxValue), (int)(flValue * (double)byte.MaxValue), (int)(num4 * (double)byte.MaxValue));
                case 2:
                    return new Color(nAlpha, (int)(num4 * (double)byte.MaxValue), (int)(flValue * (double)byte.MaxValue), (int)(num6 * (double)byte.MaxValue));
                case 3:
                    return new Color(nAlpha, (int)(num4 * (double)byte.MaxValue), (int)(num5 * (double)byte.MaxValue), (int)(flValue * (double)byte.MaxValue));
                case 4:
                    return new Color(nAlpha, (int)(num6 * (double)byte.MaxValue), (int)(num4 * (double)byte.MaxValue), (int)(flValue * (double)byte.MaxValue));
                case 5:
                    return new Color(nAlpha, (int)(flValue * (double)byte.MaxValue), (int)(num4 * (double)byte.MaxValue), (int)(num5 * (double)byte.MaxValue));
                default:
                    return new Color(nAlpha, 0, 0, 0);
            }
        }

        internal int ToArgb() => (int)Value;

        internal ColorF RenderConvert() => new(A, R, G, B);

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder(32);
            stringBuilder.Append("A=");
            stringBuilder.Append(A);
            stringBuilder.Append(", R=");
            stringBuilder.Append(R);
            stringBuilder.Append(", G=");
            stringBuilder.Append(G);
            stringBuilder.Append(", B=");
            stringBuilder.Append(B);
            return stringBuilder.ToString();
        }

        public static bool operator ==(Color left, Color right) => (int)left.value == (int)right.value;

        public static bool operator !=(Color left, Color right) => !(left == right);

        public override bool Equals(object obj) => obj is Color color && (int)value == (int)color.value;

        public bool Equals(Color right) => (int)value == (int)right.value;

        public override int GetHashCode() => value.GetHashCode();

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Value), Value);
        }

        public string EncodeString() => string.Join(", ", A, R, G, B);

        internal static Color Transparent => new(0U);

        internal static Color Black => new(0xFF00_0000U);

        internal static Color White => new(uint.MaxValue);
    }
}
