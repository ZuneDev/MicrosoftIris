// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.PropertySet
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Iris
{
    public class PropertySet : ModelItem, IDictionary, ICollection, IEnumerable
    {
        private Dictionary<object, object> _valuesTable = new Dictionary<object, object>();

        public PropertySet(IModelItemOwner owner, string description)
          : base(owner, description)
        {
        }

        public PropertySet(IModelItemOwner owner)
          : this(owner, null)
        {
        }

        public PropertySet()
          : this(null)
        {
        }

        public IDictionary Entries
        {
            get
            {
                using (ThreadValidator)
                    return this;
            }
        }

        public object this[object key]
        {
            get
            {
                using (ThreadValidator)
                {
                    object obj;
                    return _valuesTable.TryGetValue(key, out obj) ? obj : null;
                }
            }
            set
            {
                using (ThreadValidator)
                {
                    object a;
                    if (_valuesTable.TryGetValue(key, out a) && IsEqual(a, value))
                        return;
                    _valuesTable[key] = value;
                    NotifyEntryChange(key);
                }
            }
        }

        bool IDictionary.Contains(object key)
        {
            using (ThreadValidator)
                return _valuesTable.ContainsKey(key);
        }

        void IDictionary.Add(object key, object value)
        {
            using (ThreadValidator)
            {
                if (_valuesTable.ContainsKey(key))
                    return;
                _valuesTable.Add(key, value);
                NotifyEntryChange(key);
            }
        }

        void IDictionary.Remove(object key)
        {
            using (ThreadValidator)
            {
                if (_valuesTable.ContainsKey(key))
                    return;
                _valuesTable.Remove(key);
                NotifyEntryChange(key);
            }
        }

        void IDictionary.Clear()
        {
            using (ThreadValidator)
            {
                foreach (KeyValuePair<object, object> keyValuePair in _valuesTable)
                    NotifyEntryChange(keyValuePair.Key);
                _valuesTable.Clear();
            }
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            using (ThreadValidator)
                return _valuesTable.GetEnumerator();
        }

        bool IDictionary.IsFixedSize => false;

        bool IDictionary.IsReadOnly => false;

        ICollection IDictionary.Keys
        {
            get
            {
                using (ThreadValidator)
                    return _valuesTable.Keys;
            }
        }

        ICollection IDictionary.Values
        {
            get
            {
                using (ThreadValidator)
                    return _valuesTable.Values;
            }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            using (ThreadValidator)
                ((ICollection)_valuesTable).CopyTo(array, index);
        }

        int ICollection.Count
        {
            get
            {
                using (ThreadValidator)
                    return _valuesTable.Count;
            }
        }

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot
        {
            get
            {
                using (ThreadValidator)
                    return _valuesTable;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            using (ThreadValidator)
                return _valuesTable.GetEnumerator();
        }

        private void NotifyEntryChange(object key) => FirePropertyChanged("#" + key.ToString());

        private static bool IsEqual(object a, object b) => a == null ? b == null : a.Equals(b);
    }
}
