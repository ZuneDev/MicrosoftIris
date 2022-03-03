// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.ListDataSet
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Data;
using Microsoft.Iris.Library;
using System;
using System.Collections;

namespace Microsoft.Iris
{
    public class ListDataSet : ModelItem, INotifyList, IList, ICollection, IEnumerable
    {
        private static readonly EventCookie s_listContentsChangedEvent = EventCookie.ReserveSlot();
        private IList _sourceList;

        protected ListDataSet()
          : this(null)
        {
        }

        public ListDataSet(IList source)
          : this(null, source)
        {
        }

        public ListDataSet(IModelItemOwner owner, IList source)
          : base(owner)
          => _sourceList = source;

        public IList Source
        {
            get
            {
                using (ThreadValidator)
                    return _sourceList;
            }
            set
            {
                using (ThreadValidator)
                {
                    if (_sourceList == value)
                        return;
                    _sourceList = !(_sourceList is IVirtualList) ? value : throw new ArgumentException(InvariantString.Format("ListDataSet does not support IVirtualList.  Cannot associate with source list: {0}", value));
                    FirePropertyChanged(nameof(Source));
                    FirePropertyChanged("Count");
                    FireSetChanged(UIListContentsChangeType.Reset, -1, -1);
                }
            }
        }

        public virtual int Count
        {
            get
            {
                using (ThreadValidator)
                    return _sourceList == null ? 0 : _sourceList.Count;
            }
        }

        public virtual bool IsSynchronized
        {
            get
            {
                using (ThreadValidator)
                    return _sourceList != null && _sourceList.IsSynchronized;
            }
        }

        public virtual object SyncRoot
        {
            get
            {
                using (ThreadValidator)
                    return _sourceList == null ? null : _sourceList.SyncRoot;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                using (ThreadValidator)
                    return _sourceList == null || _sourceList.IsReadOnly;
            }
        }

        public bool IsFixedSize
        {
            get
            {
                using (ThreadValidator)
                    return _sourceList == null || _sourceList.IsFixedSize;
            }
        }

        public object this[int itemIndex]
        {
            get
            {
                using (ThreadValidator)
                    return _sourceList == null ? null : _sourceList[itemIndex];
            }
            set
            {
                using (ThreadValidator)
                {
                    if (_sourceList == null)
                        throw new InvalidOperationException("Cannot use the this indexer without first specifying a Source for this ListDataSet.");
                    if (itemIndex < 0 || itemIndex >= Count)
                        throw new ArgumentOutOfRangeException(nameof(itemIndex), itemIndex, "Given index is out of the range of this list.");
                    if (_sourceList[itemIndex] == value)
                        return;
                    _sourceList[itemIndex] = value;
                    FireSetChanged(UIListContentsChangeType.Modified, itemIndex, itemIndex);
                }
            }
        }

        public virtual void Clear()
        {
            using (ThreadValidator)
            {
                if (_sourceList == null)
                    return;
                _sourceList.Clear();
                FireSetChanged(UIListContentsChangeType.Clear, -1, -1);
                FirePropertyChanged("Count");
            }
        }

        public virtual void CopyTo(Array array, int index)
        {
            using (ThreadValidator)
            {
                if (_sourceList == null)
                    throw new NotImplementedException("The \"CopyTo\" operation is not supported by this ListDataSet");
                _sourceList.CopyTo(array, index);
            }
        }

        public void CopyFrom(IEnumerable source)
        {
            using (ThreadValidator)
            {
                if (source == null)
                    throw new ArgumentNullException(nameof(source));
                foreach (object obj in source)
                    Add(obj);
            }
        }

        public virtual IEnumerator GetEnumerator()
        {
            using (ThreadValidator)
                return _sourceList != null ? _sourceList.GetEnumerator() : throw new NotImplementedException("The \"GetEnumerator\" operation is not supported by this UIDataSet");
        }

        public bool Contains(object item)
        {
            using (ThreadValidator)
                return _sourceList != null && _sourceList.Contains(item);
        }

        public int IndexOf(object item)
        {
            using (ThreadValidator)
                return _sourceList == null ? -1 : _sourceList.IndexOf(item);
        }

        public void Remove(object item)
        {
            using (ThreadValidator)
            {
                int index = IndexOf(item);
                if (index <= -1)
                    return;
                RemoveAt(index);
            }
        }

        public void RemoveAt(int index)
        {
            using (ThreadValidator)
            {
                if (_sourceList == null)
                    throw new ArgumentException(InvariantString.Format("Empty list cannot remove item at index {0}", index));
                object obj = this[index];
                _sourceList.RemoveAt(index);
                FireSetChanged(UIListContentsChangeType.Remove, index, -1);
                FirePropertyChanged("Count");
            }
        }

        public void Move(int oldIndex, int newIndex)
        {
            using (ThreadValidator)
            {
                if (_sourceList == null)
                    throw new ArgumentException(InvariantString.Format("Empty list cannot move item from {0} to {1}", oldIndex, newIndex));
                object obj = this[oldIndex];
                if (_sourceList is INotifyList sourceList)
                {
                    sourceList.Move(oldIndex, newIndex);
                }
                else
                {
                    _sourceList.RemoveAt(oldIndex);
                    _sourceList.Insert(newIndex, obj);
                }
                FireSetChanged(UIListContentsChangeType.Move, oldIndex, newIndex);
            }
        }

        public int Reorder(IList indices, int newIndex)
        {
            using (ThreadValidator)
            {
                if (indices == null)
                    throw new ArgumentNullException(nameof(indices));
                if (newIndex < 0 || newIndex > Count)
                    throw new ArgumentOutOfRangeException(nameof(newIndex), "newIndex must be greater than 0 and less than or equal to the size of the collection");
                int[] numArray = null;
                if (indices.IsReadOnly)
                    numArray = new int[indices.Count];
                int index1 = 0;
                foreach (object index2 in indices)
                {
                    if (!(index2 is int num))
                        throw new ArgumentException("indices[" + index1 + "] does not contain an int", nameof(indices));
                    if (num < 0 || num >= Count)
                        throw new ArgumentOutOfRangeException(nameof(indices), "indices[" + index1 + "] must be greater than 0 and less than the size of the collection");
                    if (numArray != null)
                        numArray[index1] = num;
                    ++index1;
                }
                if (numArray != null)
                    indices = numArray;
                int num1 = newIndex;
                for (int index2 = 0; index2 < indices.Count; ++index2)
                {
                    int index3 = (int)indices[index2];
                    if (index3 < newIndex)
                    {
                        --newIndex;
                        --num1;
                    }
                    if (index3 != newIndex)
                    {
                        Move(index3, newIndex);
                        for (int index4 = index2 + 1; index4 < indices.Count; ++index4)
                        {
                            int index5 = (int)indices[index4];
                            if (index3 < index5 && index5 <= newIndex)
                                indices[index4] = index5 - 1;
                            else if (newIndex <= index5 && index5 < index3)
                                indices[index4] = index5 + 1;
                            else if (index5 == index3)
                                indices[index4] = newIndex;
                        }
                    }
                    ++newIndex;
                }
                return num1;
            }
        }

        public void Insert(int index, object item)
        {
            using (ThreadValidator)
            {
                _sourceList.Insert(index, item);
                FireSetChanged(UIListContentsChangeType.Insert, -1, index);
                FirePropertyChanged("Count");
            }
        }

        public int Add(object item)
        {
            using (ThreadValidator)
            {
                int newIndex = _sourceList != null ? _sourceList.Add(item) : throw new ArgumentException("Empty list cannot add items.");
                FireSetChanged(UIListContentsChangeType.Add, -1, newIndex);
                FirePropertyChanged("Count");
                return newIndex;
            }
        }

        public void Sort(IComparer comparer)
        {
            using (ThreadValidator)
            {
                if (comparer == null)
                    throw new ArgumentException("Must provide a valid IComparer");
                SortWorker(comparer);
            }
        }

        public void Sort()
        {
            using (ThreadValidator)
                SortWorker(null);
        }

        private void SortWorker(IComparer cmp)
        {
            for (int index1 = 0; index1 < Count; ++index1)
            {
                for (int index2 = index1 + 1; index2 < Count; ++index2)
                {
                    if (Compare(this[index1], this[index2], cmp) > 0)
                        Move(index2, index1);
                }
            }
        }

        private int Compare(object objA, object objB, IComparer cmp)
        {
            if (cmp != null)
                return cmp.Compare(objA, objB);
            return objA is IComparable comparable ? comparable.CompareTo(objB) : 0;
        }

        event UIListContentsChangedHandler INotifyList.ContentsChanged
        {
            add
            {
                using (ThreadValidator)
                    AddEventHandler(s_listContentsChangedEvent, value);
            }
            remove
            {
                using (ThreadValidator)
                    RemoveEventHandler(s_listContentsChangedEvent, value);
            }
        }

        public event ListContentsChangedHandler ContentsChanged
        {
            add
            {
                using (ThreadValidator)
                    AddEventHandler(s_listContentsChangedEvent, ListContentsChangedProxy.Thunk(value));
            }
            remove
            {
                using (ThreadValidator)
                    RemoveEventHandler(s_listContentsChangedEvent, ListContentsChangedProxy.Thunk(value));
            }
        }

        internal void FireSetChanged(UIListContentsChangeType type, int oldIndex, int newIndex)
        {
            UIListContentsChangedHandler eventHandler = (UIListContentsChangedHandler)GetEventHandler(s_listContentsChangedEvent);
            if (eventHandler != null)
            {
                UIListContentsChangedArgs args = new UIListContentsChangedArgs(type, oldIndex, newIndex);
                eventHandler(this, args);
            }
            FirePropertyChanged("ContentsChanged");
        }
    }
}
