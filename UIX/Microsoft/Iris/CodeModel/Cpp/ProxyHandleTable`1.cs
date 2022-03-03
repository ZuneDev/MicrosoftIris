// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.CodeModel.Cpp.ProxyHandleTable`1
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System;
using System.Diagnostics;

namespace Microsoft.Iris.CodeModel.Cpp
{
    internal class ProxyHandleTable<T> : ProxyHandleTable
    {
        private const int c_uninitialized = 0;
        private const int c_invalidFreeListValue = -1;
        private const int c_slotInUse = -2;
        private const int c_tableShift = 30;
        private const int c_lifetimeCountMask = 1073741823;
        private const ulong c_uniquenessMask = 4294967295;
        private const ulong c_indexMask = 4294967295;
        private const int c_uniquenessShift = 32;
        private ProxyHandleTable<T>.ListEntry[] _entries;
        private int _oldestFreeIndex;
        private int _newestFreeIndex;
        private int _filledBoundary;
        private uint _lifetimeCount;

        public ProxyHandleTable()
        {
            _oldestFreeIndex = -1;
            _newestFreeIndex = -1;
            _filledBoundary = 0;
        }

        protected bool InternalLookupByHandle(ulong handle, out T obj)
        {
            bool flag = false;
            obj = default(T);
            int index;
            uint uniquenessBits;
            DecodeHandle(handle, out index, out uniquenessBits);
            ProxyHandleTable<T>.ListEntry entry = _entries[index];
            if ((int)uniquenessBits == (int)entry.Uniquifier)
            {
                flag = true;
                obj = entry.Value;
            }
            return flag;
        }

        protected ulong AllocateHandle(T obj)
        {
            EnsureSpaceForNewEntry();
            int index = ReserveIndexForNewEntry();
            return StoreNewEntry(obj, index);
        }

        private void EnsureSpaceForNewEntry()
        {
            if (_entries != null && (_oldestFreeIndex != -1 || _filledBoundary != _entries.Length))
                return;
            EnlargeList();
        }

        private int ReserveIndexForNewEntry()
        {
            int index;
            if (_oldestFreeIndex != -1)
            {
                index = FreeListIndexToRealIndex(_oldestFreeIndex);
                _oldestFreeIndex = _entries[index].FreeIndex;
                if (_oldestFreeIndex == -1)
                    _newestFreeIndex = -1;
            }
            else
                index = _filledBoundary++;
            return index;
        }

        private ulong StoreNewEntry(T obj, int index)
        {
            ulong handle;
            uint uniquifier;
            GenerateHandle(index, out handle, out uniquifier);
            int index1 = -2;
            AddRef(ref index1);
            _entries[index].Uniquifier = uniquifier;
            _entries[index].Value = obj;
            _entries[index].FreeIndex = index1;
            return handle;
        }

        private int FreeListIndexToRealIndex(int index) => index - 1;

        private int RealIndexToFreeListIndex(int index) => index + 1;

        private void GenerateHandle(int insertIndex, out ulong handle, out uint uniquifier)
        {
            ++_lifetimeCount;
            uniquifier = TableIndex << 30;
            uniquifier |= _lifetimeCount & 1073741823U;
            handle = (ulong)uniquifier << 32;
            uint num = (uint)insertIndex;
            handle |= num;
        }

        private void DecodeHandle(ulong handle, out int index, out uint uniquenessBits)
        {
            index = (int)((long)handle & uint.MaxValue);
            ulong num = 18446744069414584320;
            uniquenessBits = (uint)((handle & num) >> 32);
        }

        private void EnlargeList()
        {
            if (_entries == null)
            {
                _entries = new ProxyHandleTable<T>.ListEntry[4];
            }
            else
            {
                ProxyHandleTable<T>.ListEntry[] listEntryArray = new ProxyHandleTable<T>.ListEntry[_entries.Length * 2];
                Array.Copy(_entries, listEntryArray, _entries.Length);
                _entries = listEntryArray;
            }
        }

        protected void AddRefHandle(ulong handle)
        {
            int index;
            DecodeHandle(handle, out index, out uint _);
            AddRef(ref _entries[index].FreeIndex);
        }

        private void AddRef(ref int index) => --index;

        private void Release(ref int index) => ++index;

        protected bool ReleaseHandle(ulong handle, out T oldValue)
        {
            bool flag = false;
            int index;
            DecodeHandle(handle, out index, out uint _);
            Release(ref _entries[index].FreeIndex);
            oldValue = _entries[index].Value;
            if (_entries[index].FreeIndex == -2)
            {
                flag = true;
                RemoveEntry(index);
            }
            return flag;
        }

        private void RemoveEntry(int index)
        {
            _entries[index].Value = default(T);
            int freeListIndex = RealIndexToFreeListIndex(index);
            if (_newestFreeIndex != -1)
                _entries[FreeListIndexToRealIndex(_newestFreeIndex)].FreeIndex = freeListIndex;
            else
                _oldestFreeIndex = freeListIndex;
            _newestFreeIndex = freeListIndex;
            _entries[index].FreeIndex = -1;
        }

        protected ProxyHandleTable<T>.ProxyHandleTableEnumerator GetTableEnumerator() => new ProxyHandleTable<T>.ProxyHandleTableEnumerator(this);

        [Conditional("DEBUG")]
        public void ValidateList()
        {
        }

        private struct ListEntry
        {
            public T Value;
            public int FreeIndex;
            public uint Uniquifier;
        }

        internal struct ProxyHandleTableEnumerator
        {
            private const int START_INVALID_INDEX = -1;
            private ProxyHandleTable<T> _table;
            private int _index;

            public ProxyHandleTableEnumerator(ProxyHandleTable<T> table)
            {
                _table = table;
                _index = -1;
            }

            public bool MoveNext()
            {
                do
                {
                    ++_index;
                }
                while (_index < _table._filledBoundary && _table._entries[_index].FreeIndex > -2);
                return _index < _table._filledBoundary;
            }

            public T Current => _table._entries[_index].Value;

            public void Reset() => _index = -1;
        }
    }
}
