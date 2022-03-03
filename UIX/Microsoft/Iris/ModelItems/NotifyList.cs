// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.ModelItems.NotifyList
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Data;
using Microsoft.Iris.Library;
using Microsoft.Iris.Markup;
using System;
using System.Collections;

namespace Microsoft.Iris.ModelItems
{
    internal class NotifyList : NotifyObjectBase, INotifyList, IList, ICollection, IEnumerable
    {
        private IList _source;

        public NotifyList()
          : this(new ArrayList())
        {
        }

        public NotifyList(IList source) => _source = source;

        public int Add(object value)
        {
            int newIndex = _source.Add(value);
            FireSetChanged(UIListContentsChangeType.Add, -1, newIndex);
            FireNotification(NotificationID.Count);
            return newIndex;
        }

        public void Clear()
        {
            _source.Clear();
            FireSetChanged(UIListContentsChangeType.Clear, -1, -1);
            FireNotification(NotificationID.Count);
        }

        public bool Contains(object value) => _source.Contains(value);

        public int IndexOf(object value) => _source.IndexOf(value);

        public void Insert(int index, object value)
        {
            _source.Insert(index, value);
            FireSetChanged(UIListContentsChangeType.Insert, -1, index);
            FireNotification(NotificationID.Count);
        }

        public void Remove(object value)
        {
            int index = IndexOf(value);
            if (index == -1)
                return;
            RemoveAt(index);
        }

        public void RemoveAt(int index)
        {
            object obj = this[index];
            _source.RemoveAt(index);
            FireSetChanged(UIListContentsChangeType.Remove, index, -1);
            FireNotification(NotificationID.Count);
        }

        public bool IsFixedSize => false;

        public bool IsReadOnly => false;

        public object this[int index]
        {
            get => _source[index];
            set
            {
                if (_source[index] == value)
                    return;
                _source[index] = value;
                FireSetChanged(UIListContentsChangeType.Modified, index, index);
            }
        }

        public void CopyTo(Array array, int index) => _source.CopyTo(array, index);

        public int Count => _source.Count;

        public bool IsSynchronized => false;

        public object SyncRoot => _source.SyncRoot;

        public IEnumerator GetEnumerator() => _source.GetEnumerator();

        public void Move(int oldIndex, int newIndex)
        {
            object obj = this[oldIndex];
            _source.RemoveAt(oldIndex);
            _source.Insert(newIndex, obj);
            FireSetChanged(UIListContentsChangeType.Move, oldIndex, newIndex);
        }

        public event UIListContentsChangedHandler ContentsChanged;

        private void FireSetChanged(UIListContentsChangeType type, int oldIndex, int newIndex)
        {
            if (ContentsChanged == null)
                return;
            ContentsChanged(this, new UIListContentsChangedArgs(type, oldIndex, newIndex));
        }
    }
}
