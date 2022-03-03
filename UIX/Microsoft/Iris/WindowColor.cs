// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.WindowColor
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Drawing;
using System;

namespace Microsoft.Iris
{
    public class WindowColor
    {
        private Color _color;

        public WindowColor(int red, int green, int blue)
        {
            CheckByte(red, nameof(red));
            CheckByte(green, nameof(green));
            CheckByte(blue, nameof(blue));
            _color = new Color(red, green, blue);
        }

        public WindowColor(float red, float green, float blue)
        {
            CheckFloat(red, nameof(red));
            CheckFloat(green, nameof(green));
            CheckFloat(blue, nameof(blue));
            _color = new Color(red, green, blue);
        }

        public byte R => _color.R;

        public byte G => _color.G;

        public byte B => _color.B;

        internal Color GetInternalColor() => _color;

        private static void CheckByte(int value, string name)
        {
            if (value < 0 || value > byte.MaxValue)
                throw new ArgumentException(string.Format("Invalid value ({0}) for {1} color channel. Expecting a value between 0 and 255.", value, name));
        }

        private static void CheckFloat(float value, string name)
        {
            if (value < 0.0 || value > 1.0)
                throw new ArgumentException(string.Format("Invalid value ({0}) for {1} color channel. Expecting a value between 0.0 and 1.0.", value, name));
        }
    }
}
