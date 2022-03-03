// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Library.Bits
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System.Diagnostics;

namespace Microsoft.Iris.Library
{
    internal static class Bits
    {
        public static bool TestFlag(int value, int maskValue) => (value & maskValue) != 0;

        public static bool TestFlag(uint value, uint maskValue) => ((int)value & (int)maskValue) != 0;

        public static bool TestAnyFlags(int value, int maskValue) => (value & maskValue) != 0;

        public static bool TestAnyFlags(uint value, uint maskValue) => ((int)value & (int)maskValue) != 0;

        public static bool TestAllFlags(int value, int maskValue) => (value & maskValue) == maskValue;

        public static bool TestAllFlags(uint value, uint maskValue) => ((int)value & (int)maskValue) == (int)maskValue;

        public static void SetFlag(ref int value, int maskValue) => value |= maskValue;

        public static void SetFlag(ref uint value, uint maskValue) => value |= maskValue;

        public static void ClearFlag(ref int value, int maskValue) => value &= ~maskValue;

        public static void ClearFlag(ref uint value, uint maskValue) => value &= ~maskValue;

        public static void ChangeFlags(ref int value, int newValue, int maskValue) => value = newValue & maskValue | value & ~maskValue;

        public static void ChangeFlags(ref uint value, uint newValue, uint maskValue) => value = (uint)((int)newValue & (int)maskValue | (int)value & ~(int)maskValue);

        public static void ChangeFlag(ref int value, bool flag, int maskValue)
        {
            int num = flag ? maskValue : 0;
            value = num & maskValue | value & ~maskValue;
        }

        public static void ChangeFlag(ref uint value, bool flag, uint maskValue)
        {
            uint num = flag ? maskValue : 0U;
            value = (uint)((int)num & (int)maskValue | (int)value & ~(int)maskValue);
        }

        public static void FlipFlags(ref int value, int maskValue) => value ^= maskValue;

        public static void FlipFlags(ref uint value, uint maskValue) => value ^= maskValue;

        public static int ExtractFlags(int value, int maskValue) => value & maskValue;

        public static int ExtractFlags(int value, int maskValue, int shiftBitsCount) => (value & maskValue) >> shiftBitsCount;

        public static uint ExtractFlags(uint value, uint maskValue) => value & maskValue;

        public static uint ExtractFlags(uint value, uint maskValue, int shiftBitsCount) => (value & maskValue) >> shiftBitsCount;

        [Conditional("DEBUG")]
        public static void AssertFlags(int testFlagsValue, int validFlagsValue)
        {
        }

        [Conditional("DEBUG")]
        public static void AssertFlags(uint testFlagsValue, uint validFlagsValue)
        {
        }

        public static void PromptFlags(int testFlagsValue, int validFlagsValue)
        {
        }

        public static void PromptFlags(uint testFlagsValue, uint validFlagsValue)
        {
        }
    }
}
