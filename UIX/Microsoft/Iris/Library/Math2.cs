// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Library.Math2
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

namespace Microsoft.Iris.Library
{
    internal class Math2
    {
        public static int FindPowerOf2(int value) => FindPowerOf2(value, 1);

        public static int FindPowerOf2(int value, int startValue)
        {
            int num = startValue;
            while (num < value)
                num *= 2;
            return num;
        }

        public static int RoundUp(float value) => (int)(value + 0.5);

        public static int Clamp(int value, int min, int max)
        {
            if (value < min)
                return min;
            return value > max ? max : value;
        }

        public static float Clamp(float value, float min, float max)
        {
            if (value < (double)min)
                return min;
            return value > (double)max ? max : value;
        }

        public static bool WithinEpsilon(float value1, float value2)
        {
            float num = value1 - value2;
            return -9.99999974737875E-06 <= num && num <= 9.99999974737875E-06;
        }

        public static double Blend(double a, double b, double weight, bool allowOutOfRangeWeights) => (1.0 - weight) * a + weight * b;
    }
}
