// Decompiled with JetBrains decompiler
// Type: Vector
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using System;
using System.Collections;

public class Vector : IVector
{
    private const int k_defaultCapacity = 4;
    private static object[] s_emptyArray = new object[0];
    private object[] _items;
    private int _size;

    public Vector() => this._items = s_emptyArray;

    public Vector(int capacity) => this._items = new object[capacity];

    public Vector(IEnumerable collection)
    {
        if (collection is ICollection collection1)
        {
            int count = collection1.Count;
            this._items = new object[count];
            collection1.CopyTo(_items, 0);
            this._size = count;
        }
        else
        {
            this._size = 0;
            this._items = new object[4];
            foreach (object obj in collection)
                this.Add(obj);
        }
    }

    public Vector(IVector list)
    {
        int count = list.Count;
        this._size = count;
        this._items = new object[count];
        for (int index = 0; index < count; ++index)
            this._items[index] = list[index];
    }

    public int Capacity
    {
        get => this._items.Length;
        set
        {
            if (value == this._items.Length)
                return;
            if (value > 0)
            {
                object[] objArray = new object[value];
                if (this._size > 0)
                    Array.Copy(_items, 0, objArray, 0, this._size);
                this._items = objArray;
            }
            else
                this._items = s_emptyArray;
        }
    }

    public int Count => this._size;

    public object this[int index]
    {
        get => this._items[index];
        set => this._items[index] = value;
    }

    object IVector.this[int index] => this[index];

    public void Add(object item)
    {
        if (this._size == this._items.Length)
            this.EnsureCapacity(this._size + 1);
        this._items[this._size++] = item;
    }

    public void Clear()
    {
        Array.Clear(_items, 0, this._size);
        this._size = 0;
    }

    public bool Contains(object item) => Array.IndexOf<object>(this._items, item, 0, this._size) >= 0;

    public void CopyTo(Array array, int index)
    {
        if (this._items == null)
            return;
        try
        {
            Array.Copy(_items, 0, array, index, this._size);
        }
        catch (ArrayTypeMismatchException ex)
        {
            throw new ArgumentException(null);
        }
    }

    public void ExpandTo(int count)
    {
        if (this.Count >= count)
            return;
        this.EnsureCapacity(count);
        this._size = count;
    }

    public Vector.Enumerator GetEnumerator() => new Vector.Enumerator(this);

    public int IndexOf(object item) => Array.IndexOf<object>(this._items, item, 0, this._size);

    public void Insert(int index, object item)
    {
        if (this._size == this._items.Length)
            this.EnsureCapacity(this._size + 1);
        if (index < this._size)
            Array.Copy(_items, index, _items, index + 1, this._size - index);
        this._items[index] = item;
        ++this._size;
    }

    public bool Remove(object item)
    {
        int index = this.IndexOf(item);
        if (index < 0)
            return false;
        this.RemoveAt(index);
        return true;
    }

    public void RemoveAt(int index)
    {
        --this._size;
        if (index < this._size)
            Array.Copy(_items, index + 1, _items, index, this._size - index);
        this._items[this._size] = null;
    }

    public void RemoveRange(int index, int count)
    {
        if (count <= 0)
            return;
        this._size -= count;
        if (index < this._size)
            Array.Copy(_items, index + count, _items, index, this._size - index);
        Array.Clear(_items, this._size, count);
    }

    public void Sort() => this.Sort(0, this.Count, null);

    public void Sort(IComparer comparer) => this.Sort(0, this.Count, comparer);

    public void Sort(int index, int count, IComparer comparer) => Array.Sort(_items, index, count, comparer);

    public object[] ToArray()
    {
        object[] objArray = new object[this._size];
        Array.Copy(_items, 0, objArray, 0, this._size);
        return objArray;
    }

    public void TrimExcess()
    {
        if (this._size >= (int)(_items.Length * 0.9))
            return;
        this.Capacity = this._size;
    }

    private void EnsureCapacity(int minItemCount)
    {
        if (this._items.Length >= minItemCount)
            return;
        int num = this._items.Length == 0 ? 4 : this._items.Length * 2;
        if (num < minItemCount)
            num = minItemCount;
        this.Capacity = num;
    }

    public struct Enumerator
    {
        private Vector _list;
        private int _index;

        internal Enumerator(Vector list)
        {
            this._list = list;
            this._index = -1;
        }

        public bool MoveNext() => ++this._index < this._list._size;

        public object Current => this._list[this._index];
    }
}
