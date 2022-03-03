// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.RenderAPI.Memory
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System;

namespace Microsoft.Iris.RenderAPI
{
    internal static class Memory
    {
        public static unsafe bool IsOverlap(IntPtr pvA, int cbA, IntPtr pvB, int cbB)
        {
            byte* pointer1 = (byte*)pvA.ToPointer();
            byte* pointer2 = (byte*)pvB.ToPointer();
            if (pointer1 >= pointer2 && pointer1 < pointer2 + cbB)
                return true;
            return pointer2 >= pointer1 && pointer2 < pointer1 + cbA;
        }

        public static unsafe void Zero(IntPtr pvDest, int cbZero)
        {
            uint* pointer = (uint*)pvDest.ToPointer();
            int num1 = cbZero / 4;
            int num2 = num1;
            while (num2-- > 0)
                *pointer++ = 0U;
            byte* numPtr = (byte*)pointer;
            int num3 = cbZero - num1 * 4;
            while (num3-- > 0)
                *numPtr++ = 0;
        }

        public static unsafe void Copy(IntPtr pvDest, IntPtr pvSrc, int cbCopy)
        {
            uint* pointer1 = (uint*)pvDest.ToPointer();
            uint* pointer2 = (uint*)pvSrc.ToPointer();
            int num1 = cbCopy / 4;
            int num2 = num1;
            while (num2-- > 0)
                *pointer1++ = *pointer2++;
            byte* numPtr1 = (byte*)pointer1;
            byte* numPtr2 = (byte*)pointer2;
            int num3 = cbCopy - num1 * 4;
            while (num3-- > 0)
                *numPtr1++ = *numPtr2++;
        }

        public static unsafe void ConvertEndian(
          IntPtr pvDest,
          IntPtr pvSrc,
          int cbConvert,
          int cUnitBits)
        {
            switch (cUnitBits)
            {
                case 16:
                    uint* pointer1 = (uint*)pvDest.ToPointer();
                    uint* pointer2 = (uint*)pvSrc.ToPointer();
                    int num1 = cbConvert / 4;
                    while (num1-- > 0)
                    {
                        uint num2 = *pointer2++;
                        *pointer1++ = (uint)((int)((num2 & 4278190080U) >> 8) | ((int)num2 & 16711680) << 8 | (int)((num2 & 65280U) >> 8) | ((int)num2 & byte.MaxValue) << 8);
                    }
                    if (cbConvert % 4 == 0)
                        break;
                    ushort* numPtr1 = (ushort*)pointer1;
                    ushort* numPtr2 = (ushort*)pointer1;
                    ushort* numPtr3 = numPtr2 + 2;
                    ushort num3 = *numPtr2;
                    ushort* numPtr4 = numPtr1;
                    ushort* numPtr5 = numPtr4 + 2;
                    int num4 = (ushort)((num3 & 65280) >> 8 | (num3 & byte.MaxValue) << 8);
                    *numPtr4 = (ushort)num4;
                    break;
                case 32:
                    uint* pointer3 = (uint*)pvDest.ToPointer();
                    uint* pointer4 = (uint*)pvSrc.ToPointer();
                    int num5 = cbConvert / 4;
                    while (num5-- > 0)
                    {
                        uint num2 = *pointer4++;
                        *pointer3++ = (uint)((int)((num2 & 4278190080U) >> 24) | (int)((num2 & 16711680U) >> 8) | ((int)num2 & 65280) << 8 | ((int)num2 & byte.MaxValue) << 24);
                    }
                    break;
            }
        }

        public static uint ConvertEndian(uint src) => (uint)((int)((src & 4278190080U) >> 24) | (int)((src & 16711680U) >> 8) | ((int)src & 65280) << 8 | ((int)src & byte.MaxValue) << 24);
    }
}
