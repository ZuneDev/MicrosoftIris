// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Internal.Bits
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using System;
using System.Diagnostics;

namespace Microsoft.Iris.Render.Internal
{
    internal static class Bits
    {
        public static bool TestFlag(int nValue, int nMask) => (nValue & nMask) != 0;

        public static bool TestFlag(uint nValue, uint nMask) => ((int)nValue & (int)nMask) != 0;

        public static bool TestAnyFlags(int nValue, int nMask) => (nValue & nMask) != 0;

        public static bool TestAnyFlags(uint nValue, uint nMask) => ((int)nValue & (int)nMask) != 0;

        public static bool TestAllFlags(int nValue, int nMask) => (nValue & nMask) == nMask;

        public static bool TestAllFlags(uint nValue, uint nMask) => ((int)nValue & (int)nMask) == (int)nMask;

        public static void SetFlag(ref int nValue, int nMask) => nValue |= nMask;

        public static void SetFlag(ref uint nValue, uint nMask) => nValue |= nMask;

        public static void ClearFlag(ref int nValue, int nMask) => nValue &= ~nMask;

        public static void ClearFlag(ref uint nValue, uint nMask) => nValue &= ~nMask;

        public static void ChangeFlags(ref int nValue, int nNewValue, int nMask) => nValue = nNewValue & nMask | nValue & ~nMask;

        public static void ChangeFlags(ref uint nValue, uint nNewValue, uint nMask) => nValue = (uint)((int)nNewValue & (int)nMask | (int)nValue & ~(int)nMask);

        public static void ChangeFlag(ref int nValue, bool fFlag, int nMask)
        {
            int num = fFlag ? nMask : 0;
            nValue = num & nMask | nValue & ~nMask;
        }

        public static void ChangeFlag(ref uint nValue, bool fFlag, uint nMask)
        {
            uint num = fFlag ? nMask : 0U;
            nValue = (uint)((int)num & (int)nMask | (int)nValue & ~(int)nMask);
        }

        public static void FlipFlags(ref int nValue, int nMask) => nValue ^= nMask;

        public static void FlipFlags(ref uint nValue, uint nMask) => nValue ^= nMask;

        public static int ExtractFlags(int nValue, int nMask) => nValue & nMask;

        public static int ExtractFlags(int nValue, int nMask, int cShiftBits) => (nValue & nMask) >> cShiftBits;

        public static uint ExtractFlags(uint nValue, uint nMask) => nValue & nMask;

        public static uint ExtractFlags(uint nValue, uint nMask, int cShiftBits) => (nValue & nMask) >> cShiftBits;

        [Conditional("DEBUG")]
        public static void AssertFlags(int nTestFlags, int nValidFlags)
        {
        }

        [Conditional("DEBUG")]
        public static void AssertFlags(uint nTestFlags, uint nValidFlags)
        {
        }

        public static void PromptFlags(int nTestFlags, int nValidFlags) => Debug2.Validate((nTestFlags & nValidFlags) == nTestFlags, typeof(ArgumentException), "Ensure valid flags");

        public static void PromptFlags(uint nTestFlags, uint nValidFlags) => Debug2.Validate(((int)nTestFlags & (int)nValidFlags) == (int)nTestFlags, typeof(ArgumentException), "Ensure valid flags");
    }
}
