// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Internal.Math2
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

namespace Microsoft.Iris.Render.Internal
{
    internal class Math2
    {
        public static int FindPowerOf2(int nValue) => FindPowerOf2(nValue, 1);

        public static int FindPowerOf2(int nValue, int nStart)
        {
            int num = nStart;
            while (num < nValue)
                num *= 2;
            return num;
        }

        public static int RoundUp(float flValue) => (int)(flValue + 0.5);

        public static int Clamp(int nValue, int nMin, int nMax)
        {
            if (nValue < nMin)
                return nMin;
            return nValue > nMax ? nMax : nValue;
        }

        public static float Clamp(float flValue, float flMin, float flMax)
        {
            if (flValue < (double)flMin)
                return flMin;
            return flValue > (double)flMax ? flMax : flValue;
        }

        public static bool WithinEpsilon(float flValue1, float flValue2)
        {
            float num = flValue1 - flValue2;
            return -9.99999974737875E-06 <= num && num <= 9.99999974737875E-06;
        }
    }
}
