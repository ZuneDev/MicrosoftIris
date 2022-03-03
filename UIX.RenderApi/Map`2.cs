// Decompiled with JetBrains decompiler
// Type: Map`2
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using System;
using System.Collections.Generic;

public class Map<K, V>
{
    private int[] _buckets;
    private Map<K, V>.Entry[] _entries;
    private int _count;
    private int _version;
    private int _freeList;
    private int _freeCount;

    public Map()
      : this(0)
    {
    }

    public Map(int capacity)
    {
        if (capacity <= 0)
            return;
        this.Initialize(capacity);
    }

    public int Count => this._count - this._freeCount;

    public Map<K, V>.KeyCollection Keys => new Map<K, V>.KeyCollection(this);

    public Map<K, V>.ValueCollection Values => new Map<K, V>.ValueCollection(this);

    public V this[K key]
    {
        get
        {
            int entry = this.FindEntry(key);
            return entry >= 0 ? this._entries[entry].value : default(V);
        }
        set => this.Insert(key, value, false);
    }

    public void Add(K key, V value) => this.Insert(key, value, true);

    public void Clear()
    {
        if (this._count <= 0)
            return;
        for (int index = 0; index < this._buckets.Length; ++index)
            this._buckets[index] = -1;
        Array.Clear(_entries, 0, this._count);
        this._freeList = -1;
        this._count = 0;
        this._freeCount = 0;
    }

    public bool ContainsKey(K key) => this.FindEntry(key) >= 0;

    public Map<K, V>.Enumerator GetEnumerator() => new Map<K, V>.Enumerator(this);

    private int FindEntry(K key)
    {
        if (this._buckets != null)
        {
            int num = this.GetKeyHashCode(key) & int.MaxValue;
            for (int index = this._buckets[num % this._buckets.Length]; index >= 0; index = this._entries[index].next)
            {
                if (this._entries[index].hashCode == num && this.KeyEquals(this._entries[index].key, key))
                    return index;
            }
        }
        return -1;
    }

    private void Initialize(int capacity)
    {
        int prime = HashHelpers.GetPrime(capacity);
        this._buckets = new int[prime];
        for (int index = 0; index < this._buckets.Length; ++index)
            this._buckets[index] = -1;
        this._entries = new Map<K, V>.Entry[prime];
        this._freeList = -1;
    }

    private void Insert(K key, V value, bool add)
    {
        if (this._buckets == null)
            this.Initialize(0);
        int num = this.GetKeyHashCode(key) & int.MaxValue;
        for (int index = this._buckets[num % this._buckets.Length]; index >= 0; index = this._entries[index].next)
        {
            if (this._entries[index].hashCode == num && this.KeyEquals(this._entries[index].key, key))
            {
                if (add)
                    throw new ArgumentException("Adding duplicate entry to Dictionary");
                this._entries[index].value = value;
                ++this._version;
                return;
            }
        }
        int index1;
        if (this._freeCount > 0)
        {
            index1 = this._freeList;
            this._freeList = this._entries[index1].next;
            --this._freeCount;
        }
        else
        {
            if (this._count == this._entries.Length)
                this.Resize();
            index1 = this._count;
            ++this._count;
        }
        int index2 = num % this._buckets.Length;
        this._entries[index1].hashCode = num;
        this._entries[index1].next = this._buckets[index2];
        this._entries[index1].key = key;
        this._entries[index1].value = value;
        this._buckets[index2] = index1;
        ++this._version;
    }

    private void Resize()
    {
        int prime = HashHelpers.GetPrime(this._count * 2);
        int[] numArray = new int[prime];
        for (int index = 0; index < numArray.Length; ++index)
            numArray[index] = -1;
        Map<K, V>.Entry[] entryArray = new Map<K, V>.Entry[prime];
        Array.Copy(_entries, 0, entryArray, 0, this._count);
        for (int index1 = 0; index1 < this._count; ++index1)
        {
            int index2 = entryArray[index1].hashCode % prime;
            entryArray[index1].next = numArray[index2];
            numArray[index2] = index1;
        }
        this._buckets = numArray;
        this._entries = entryArray;
    }

    public bool Remove(K key)
    {
        if (this._buckets != null)
        {
            int num = this.GetKeyHashCode(key) & int.MaxValue;
            int index1 = num % this._buckets.Length;
            int index2 = -1;
            for (int index3 = this._buckets[index1]; index3 >= 0; index3 = this._entries[index3].next)
            {
                if (this._entries[index3].hashCode == num && this.KeyEquals(this._entries[index3].key, key))
                {
                    if (index2 < 0)
                        this._buckets[index1] = this._entries[index3].next;
                    else
                        this._entries[index2].next = this._entries[index3].next;
                    this._entries[index3].hashCode = -1;
                    this._entries[index3].next = this._freeList;
                    this._entries[index3].key = default(K);
                    this._entries[index3].value = default(V);
                    this._freeList = index3;
                    ++this._freeCount;
                    ++this._version;
                    return true;
                }
                index2 = index3;
            }
        }
        return false;
    }

    public bool TryGetValue(K key, out V value)
    {
        int entry = this.FindEntry(key);
        if (entry >= 0)
        {
            value = this._entries[entry].value;
            return true;
        }
        value = default(V);
        return false;
    }

    private bool KeyEquals(K x, K y) => x.Equals(y);

    private int GetKeyHashCode(K obj) => obj.GetHashCode();

    public struct Enumerator
    {
        private Map<K, V> _dictionary;
        private int _version;
        private int _index;
        private KeyValueEntry<K, V> _current;

        internal Enumerator(Map<K, V> dictionary)
        {
            this._dictionary = dictionary;
            this._version = this._dictionary._version;
            this._index = 0;
            this._current = new KeyValueEntry<K, V>(default(K), default(V));
        }

        public bool MoveNext()
        {
            if (this._version != this._dictionary._version)
                throw new InvalidOperationException("Dictionary was modified while enumerating");
            for (; (uint)this._index < (uint)this._dictionary._count; ++this._index)
            {
                if (this._dictionary._entries[this._index].hashCode >= 0)
                {
                    this._current = new KeyValueEntry<K, V>(this._dictionary._entries[this._index].key, this._dictionary._entries[this._index].value);
                    ++this._index;
                    return true;
                }
            }
            this._index = this._dictionary._count + 1;
            this._current = new KeyValueEntry<K, V>(default(K), default(V));
            return false;
        }

        public KeyValueEntry<K, V> Current => this._current;
    }

    public struct KeyCollection
    {
        private Map<K, V> _dictionary;

        public KeyCollection(Map<K, V> dictionary) => this._dictionary = dictionary != null ? dictionary : throw new ArgumentNullException(nameof(dictionary));

        public Map<K, V>.KeyCollection.Enumerator GetEnumerator() => new Map<K, V>.KeyCollection.Enumerator(this._dictionary);

        public int Count => this._dictionary.Count;

        public struct Enumerator
        {
            private Map<K, V> _dictionary;
            private int _index;
            private int _version;
            private K _currentKey;

            internal Enumerator(Map<K, V> dictionary)
            {
                this._dictionary = dictionary;
                this._version = dictionary._version;
                this._index = 0;
                this._currentKey = default(K);
            }

            public bool MoveNext()
            {
                if (this._version != this._dictionary._version)
                    throw new InvalidOperationException("Dictionary was modified while enumerating");
                for (; (uint)this._index < (uint)this._dictionary._count; ++this._index)
                {
                    if (this._dictionary._entries[this._index].hashCode >= 0)
                    {
                        this._currentKey = this._dictionary._entries[this._index].key;
                        ++this._index;
                        return true;
                    }
                }
                this._index = this._dictionary._count + 1;
                this._currentKey = default(K);
                return false;
            }

            public K Current => this._currentKey;
        }
    }

    public struct ValueCollection
    {
        private Map<K, V> _dictionary;

        public ValueCollection(Map<K, V> dictionary) => this._dictionary = dictionary != null ? dictionary : throw new ArgumentNullException(nameof(dictionary));

        public Map<K, V>.ValueCollection.Enumerator GetEnumerator() => new Map<K, V>.ValueCollection.Enumerator(this._dictionary);

        public int Count => this._dictionary.Count;

        public struct Enumerator
        {
            private Map<K, V> _dictionary;
            private int _index;
            private int _version;
            private V _currentValue;

            internal Enumerator(Map<K, V> dictionary)
            {
                this._dictionary = dictionary;
                this._version = dictionary._version;
                this._index = 0;
                this._currentValue = default(V);
            }

            public bool MoveNext()
            {
                if (this._version != this._dictionary._version)
                    throw new InvalidOperationException("Dictionary was modified while enumerating");
                for (; (uint)this._index < (uint)this._dictionary._count; ++this._index)
                {
                    if (this._dictionary._entries[this._index].hashCode >= 0)
                    {
                        this._currentValue = this._dictionary._entries[this._index].value;
                        ++this._index;
                        return true;
                    }
                }
                this._index = this._dictionary._count + 1;
                this._currentValue = default(V);
                return false;
            }

            public V Current => this._currentValue;
        }
    }

    private struct Entry
    {
        public int hashCode;
        public int next;
        public K key;
        public V value;
    }
}
