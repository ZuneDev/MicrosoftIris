// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Common.SmartMap`1
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using System;

namespace Microsoft.Iris.Render.Common
{
    internal struct SmartMap<T>
    {
        private SmartMap<T>.Entry[] m_entryList;

        public int Count
        {
            get
            {
                int num = 0;
                if (this.m_entryList != null)
                    num = this.m_entryList.Length;
                return num;
            }
        }

        public bool IsEmpty => this.Count == 0;

        public bool Lookup(T desired, out uint key)
        {
            if (this.m_entryList != null)
            {
                foreach (SmartMap<T>.Entry entry in this.m_entryList)
                {
                    if (entry.oData.Equals(desired))
                    {
                        key = entry.key;
                        return true;
                    }
                }
            }
            key = 0U;
            return false;
        }

        public bool TryGetValue(uint key, out T value)
        {
            int index = this.IndexOf(key);
            if (index >= 0)
            {
                value = this.m_entryList[index].oData;
                return true;
            }
            value = default(T);
            return false;
        }

        public void Remove(uint key)
        {
            int slotIndex = this.IndexOf(key);
            if (slotIndex < 0)
                return;
            this.Remove(slotIndex);
        }

        public void SetValue(uint key, T value)
        {
            int index = this.IndexOf(key);
            if (index >= 0)
                this.m_entryList[index].oData = value;
            else
                this.Add(key, value, ~index);
        }

        private int IndexOf(uint key)
        {
            if (this.m_entryList == null)
                return -1;
            uint num1 = uint.MaxValue;
            int index1 = 0;
            if (this.m_entryList.Length > 4)
            {
                int num2 = 0;
                int num3 = this.m_entryList.Length - 1;
                while (num2 <= num3)
                {
                    index1 = (num3 + num2) / 2;
                    num1 = this.m_entryList[index1].key;
                    if ((int)key == (int)num1)
                        return index1;
                    if (key < num1)
                        num3 = index1 - 1;
                    else
                        num2 = index1 + 1;
                }
            }
            else
            {
                for (int index2 = 0; index2 < this.m_entryList.Length; ++index2)
                {
                    index1 = index2;
                    num1 = this.m_entryList[index1].key;
                    if ((int)key == (int)num1)
                        return index1;
                    if (key < num1)
                        break;
                }
            }
            if (key > num1)
                ++index1;
            return ~index1;
        }

        private void Add(uint key, T data, int slotIndex)
        {
            SmartMap<T>.Entry[] entryArray;
            if (this.m_entryList == null)
            {
                entryArray = new SmartMap<T>.Entry[1];
            }
            else
            {
                int length = this.m_entryList.Length;
                entryArray = new SmartMap<T>.Entry[length + 1];
                Array.Copy(m_entryList, entryArray, slotIndex);
                Array.Copy(m_entryList, slotIndex, entryArray, slotIndex + 1, length - slotIndex);
            }
            this.m_entryList = entryArray;
            this.m_entryList[slotIndex].key = key;
            this.m_entryList[slotIndex].oData = data;
        }

        private void Remove(int slotIndex)
        {
            int length = this.m_entryList.Length;
            if (length > 0)
            {
                SmartMap<T>.Entry[] entryArray = new SmartMap<T>.Entry[length - 1];
                Array.Copy(m_entryList, entryArray, slotIndex);
                Array.Copy(m_entryList, slotIndex + 1, entryArray, slotIndex, length - (slotIndex + 1));
                this.m_entryList = entryArray;
            }
            else
                this.m_entryList = null;
        }

        private struct Entry
        {
            internal uint key;
            internal T oData;
        }
    }
}
