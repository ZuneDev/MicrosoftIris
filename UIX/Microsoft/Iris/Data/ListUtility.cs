// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Data.ListUtility
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System.Collections;

namespace Microsoft.Iris.Data
{
    internal static class ListUtility
    {
        public static bool IsNullOrEmpty(IList list) => list == null || list.Count <= 0;

        public static bool IsNullOrEmpty(IVector list) => list == null || list.Count <= 0;

        public static bool IsValidIndex(IList list, int idx) => IsValidIndex(idx, list.Count);

        public static bool IsValidIndex(IVector list, int idx) => IsValidIndex(idx, list.Count);

        public static bool IsValidIndex(int idx, int itemsCount)
        {
            int num = itemsCount - 1;
            return idx >= 0 && idx <= num;
        }

        public static bool AreContentsEqual(IVector a, IVector b)
        {
            int num1 = 0;
            if (a != null)
                num1 = a.Count;
            int num2 = 0;
            if (b != null)
                num2 = b.Count;
            if (num1 != num2)
                return false;
            for (int index = 0; index < num1; ++index)
            {
                if (!IsEqual(a[index], b[index]))
                    return false;
            }
            return true;
        }

        private static bool IsEqual(object a, object b) => a == null ? b == null : a.Equals(b);

        public static void GetWrappedIndex(
          int idx,
          int itemsCount,
          out int dataIndex,
          out int generationValue)
        {
            if (itemsCount == 0)
            {
                dataIndex = 0;
                generationValue = 0;
            }
            else
            {
                dataIndex = idx % itemsCount;
                if (dataIndex < 0)
                    dataIndex = itemsCount + dataIndex;
                generationValue = idx / itemsCount;
                if (idx >= 0 || dataIndex == 0)
                    return;
                --generationValue;
            }
        }

        public static int GetUnwrappedIndex(int dataIndex, int generationValue, int itemsCount)
        {
            int num = dataIndex + generationValue * itemsCount;
            if (num < 0 && generationValue > 0)
                num = int.MaxValue;
            else if (num > 0 && generationValue < 0)
                num = int.MinValue;
            return num;
        }
    }
}
