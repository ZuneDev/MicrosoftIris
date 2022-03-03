// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Data.IntListUtility
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System.Collections.Generic;

namespace Microsoft.Iris.Data
{
    internal static class IntListUtility
    {
        public static int IndexOf(List<int> list, int item)
        {
            for (int index = 0; index < list.Count; ++index)
            {
                if (list[index] == item)
                    return index;
            }
            return -1;
        }

        public static int IndexOf(Vector<int> list, int item)
        {
            for (int index = 0; index < list.Count; ++index)
            {
                if (list[index] == item)
                    return index;
            }
            return -1;
        }

        public static bool Contains(List<int> list, int item) => IndexOf(list, item) != -1;

        public static bool Contains(Vector<int> list, int item) => IndexOf(list, item) != -1;
    }
}
