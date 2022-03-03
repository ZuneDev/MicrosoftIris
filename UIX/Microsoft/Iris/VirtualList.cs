// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.VirtualList
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Data;
using Microsoft.Iris.Library;
using Microsoft.Iris.Session;
using Microsoft.Iris.ViewItems;
using System;
using System.Collections;
using System.Diagnostics;

namespace Microsoft.Iris
{
    public class VirtualList : ModelItem, IVirtualList, INotifyList, IList, ICollection, IEnumerable
    {
        private static readonly EventCookie s_listContentsChangedEvent = EventCookie.ReserveSlot();
        private int _count;
        private IndexedTree _items;
        private ItemCountHandler _itemCountHandler;
        private RequestItemHandler _requestItemHandler;
        private RequestSlowDataHandler _requestSlowDataHandler;
        private SlowDataAcquireCompleteHandler _slowDataAcquireCompleteHandler;
        private Vector<int> _callbackIndexesList = new Vector<int>(1);
        private UpdateHelper _updater;
        private ReleaseBehavior _releaseBehavior;
        private bool _storeQueryResults;
        private bool _countInitialized;
        private Repeater _repeater;

        public VirtualList(
          IModelItemOwner owner,
          bool enableSlowDataRequests,
          ItemCountHandler countHandler)
          : base(owner)
        {
            _items = new IndexedTree();
            _itemCountHandler = countHandler;
            _storeQueryResults = true;
            if (enableSlowDataRequests)
                _updater = new UpdateHelper(this);
            _releaseBehavior = ReleaseBehavior.KeepReference;
        }

        public VirtualList(bool enableSlowDataRequests)
          : this(null, enableSlowDataRequests, null)
        {
        }

        public VirtualList(ItemCountHandler countHandler)
          : this(null, false, countHandler)
        {
        }

        public VirtualList()
          : this(null, false, null)
        {
        }

        public int Count
        {
            get
            {
                using (ThreadValidator)
                {
                    if (_itemCountHandler != null)
                    {
                        ItemCountHandler itemCountHandler = _itemCountHandler;
                        _itemCountHandler = null;
                        itemCountHandler(this);
                    }
                    return _count;
                }
            }
            set
            {
                using (ThreadValidator)
                {
                    if (_count < 0)
                        throw new ArgumentException("Must specify a non-negative Count");
                    EnsureNotInCallback("Invalid to set count on virtual list while inside a GetItem callback");
                    _itemCountHandler = null;
                    if (_count == value && _countInitialized)
                        return;
                    _countInitialized = true;
                    Clear();
                    SetCount(value);
                    FireSetChanged(UIListContentsChangeType.Reset, -1, -1);
                }
            }
        }

        private void SetCount(int value)
        {
            if (_count == value)
                return;
            _count = value;
            FirePropertyChanged("Count");
            OnCountChanged();
        }

        internal virtual void OnCountChanged()
        {
        }

        public int UnsafeGetCount()
        {
            if (_itemCountHandler != null)
                throw new InvalidOperationException("Cannot get the count since the list is in virtualized (callback) count mode");
            return _count;
        }

        public bool IsSynchronized
        {
            get
            {
                using (ThreadValidator)
                    return false;
            }
        }

        public object SyncRoot
        {
            get
            {
                using (ThreadValidator)
                    return null;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                using (ThreadValidator)
                    return false;
            }
        }

        public bool IsFixedSize
        {
            get
            {
                using (ThreadValidator)
                    return false;
            }
        }

        public object this[int index]
        {
            get
            {
                using (ThreadValidator)
                {
                    if (IsItemAvailable(index))
                        return _items[index];
                    EnsureNotInCallback(index, "Invalid to fetch item {0} on a VirtualList while already inside a callback to create that item", index);
                    _callbackIndexesList.Add(index);
                    object obj;
                    try
                    {
                        obj = _requestItemHandler == null ? OnRequestItem(index) : _requestItemHandler(this, index);
                        if (_storeQueryResults)
                            ModifiedWorker(index, true, obj);
                    }
                    finally
                    {
                        _callbackIndexesList.Remove(index);
                    }
                    return obj;
                }
            }
            set
            {
                using (ThreadValidator)
                {
                    EnsureNotInCallback("Invalid to store items on a VirtualList while inside a callback to create an item.  Instead, set StoreQueryResults to true");
                    ModifiedWorker(index, true, value);
                }
            }
        }

        public RequestItemHandler RequestItemHandler
        {
            get
            {
                using (ThreadValidator)
                    return _requestItemHandler;
            }
            set
            {
                using (ThreadValidator)
                    _requestItemHandler = value == null || _requestItemHandler == null ? value : throw new InvalidOperationException("Only one handler may be attached at a time");
            }
        }

        public bool SlowDataRequestsEnabled
        {
            get
            {
                using (ThreadValidator)
                    return _updater != null;
            }
        }

        bool IVirtualList.SlowDataRequestsEnabled => _updater != null;

        public int SlowDataRequestThrottle
        {
            get
            {
                using (ThreadValidator)
                    return _updater != null ? _updater.Throttle : int.MaxValue;
            }
            set
            {
                using (ThreadValidator)
                {
                    if (_updater == null)
                        throw new InvalidOperationException("VirtualList is not configured for slow data notifications");
                    _updater.Throttle = value >= 0 ? value : throw new ArgumentOutOfRangeException(nameof(value));
                }
            }
        }

        public RequestSlowDataHandler RequestSlowDataHandler
        {
            get
            {
                using (ThreadValidator)
                    return _requestSlowDataHandler;
            }
            set
            {
                using (ThreadValidator)
                {
                    if (_updater == null)
                        throw new InvalidOperationException("VirtualList is not configured for slow data notifications");
                    _requestSlowDataHandler = value == null || _requestSlowDataHandler == null ? value : throw new InvalidOperationException("Only one handler may be attached at a time");
                }
            }
        }

        public bool StoreQueryResults
        {
            get
            {
                using (ThreadValidator)
                    return _storeQueryResults;
            }
            set
            {
                using (ThreadValidator)
                    _storeQueryResults = value;
            }
        }

        public ReleaseBehavior VisualReleaseBehavior
        {
            get
            {
                using (ThreadValidator)
                    return _releaseBehavior;
            }
            set
            {
                using (ThreadValidator)
                    _releaseBehavior = value;
            }
        }

        public static object UnavailableItem => Repeater.UnavailableItem;

        Repeater IVirtualList.RepeaterHost
        {
            get
            {
                using (ThreadValidator)
                    return _repeater;
            }
            set
            {
                using (ThreadValidator)
                {
                    if (value == _repeater)
                        return;
                    _repeater = value;
                    if (_updater == null)
                        return;
                    _updater.Clear();
                }
            }
        }

        void IVirtualList.RequestItem(int index, ItemRequestCallback callback)
        {
            using (ThreadValidator)
            {
                ValidateIndex(index);
                if (IsItemAvailable(index))
                {
                    callback(this, index, this[index]);
                }
                else
                {
                    EnsureNotInCallback(index, "Invalid to fetch item {0} on a VirtualList while already inside a callback to create that item", index);
                    _callbackIndexesList.Add(index);
                    try
                    {
                        object obj = _requestItemHandler == null ? OnRequestItem(index) : _requestItemHandler(this, index);
                        if (_storeQueryResults)
                            ModifiedWorker(index, true, obj);
                        callback(this, index, obj);
                    }
                    finally
                    {
                        _callbackIndexesList.Remove(index);
                    }
                }
            }
        }

        public bool IsItemAvailable(int index)
        {
            using (ThreadValidator)
            {
                ValidateIndex(index);
                return ContainsDataForIndex(index);
            }
        }

        public bool UnsafeGetItem(int index, out object item)
        {
            int count = UnsafeGetCount();
            if (!ListUtility.IsValidIndex(index, count))
                throw new IndexOutOfRangeException();
            item = null;
            if (!_items.Contains(index))
                return false;
            item = _items[index];
            return true;
        }

        protected virtual object OnRequestItem(int index)
        {
            using (ThreadValidator)
            {
                object obj = null;
                if (IsItemAvailable(index))
                    obj = _items[index];
                return obj;
            }
        }

        protected virtual void OnRequestSlowData(int index)
        {
        }

        public void NotifySlowDataAcquireComplete(int index)
        {
            using (ThreadValidator)
            {
                if (IsDisposed)
                    return;
                if (_updater == null)
                    throw new InvalidOperationException("VirtualList is not configured for slow data notifications");
                bool flag = false;
                if (_slowDataAcquireCompleteHandler != null)
                    flag = _slowDataAcquireCompleteHandler(this, index);
                if (flag)
                    return;
                _updater.NotifySlowDataAcquireComplete(index);
            }
        }

        SlowDataAcquireCompleteHandler IVirtualList.SlowDataAcquireCompleteHandler
        {
            get => _slowDataAcquireCompleteHandler;
            set => _slowDataAcquireCompleteHandler = value;
        }

        void IVirtualList.NotifyRequestSlowData(int index)
        {
            if (_requestSlowDataHandler != null)
                _requestSlowDataHandler(this, index);
            OnRequestSlowData(index);
        }

        protected virtual void OnVisualsCreated(int index)
        {
        }

        void IVirtualList.NotifyVisualsCreated(int index)
        {
            using (ThreadValidator)
            {
                if (_updater != null)
                    _updater.AddIndex(index);
                OnVisualsCreated(index);
            }
        }

        protected virtual void OnVisualsReleased(int index)
        {
        }

        void IVirtualList.NotifyVisualsReleased(int index)
        {
            using (ThreadValidator)
            {
                ValidateIndex(index);
                if (_items.Contains(index))
                {
                    switch (_releaseBehavior)
                    {
                        case ReleaseBehavior.ReleaseReference:
                            if (_updater != null)
                                _updater.RemoveIndex(index);
                            _items.Remove(index);
                            break;
                        case ReleaseBehavior.Dispose:
                            if (_updater != null)
                                _updater.RemoveIndex(index);
                            object obj = _items[index];
                            _items.Remove(index);
                            DisposeItem(obj);
                            break;
                    }
                }
                OnVisualsReleased(index);
            }
        }

        public void Clear()
        {
            using (ThreadValidator)
            {
                EnsureNotInCallback("Invalid to clear list on a VirtualList while inside a callback to fetch items");
                if (_releaseBehavior == ReleaseBehavior.Dispose)
                {
                    foreach (IndexedTree.TreeEntry treeEntry in _items)
                        DisposeItem(treeEntry.Value);
                }
                _items.Clear();
                if (_updater != null)
                    _updater.Clear();
                SetCount(0);
                FireSetChanged(UIListContentsChangeType.Clear, -1, -1);
            }
        }

        public void CopyTo(Array array, int index)
        {
            using (ThreadValidator)
            {
                if (array == null)
                    throw new ArgumentNullException(nameof(array));
                if (index < 0)
                    throw new ArgumentOutOfRangeException(nameof(index));
                int count = Count;
                if (array.Rank != 1 || index >= array.Length || array.Length - index < count)
                    throw new ArgumentException("Invalid array and index specified");
                foreach (object obj in this)
                {
                    array.SetValue(obj, index);
                    ++index;
                }
            }
        }

        public IEnumerator GetEnumerator()
        {
            using (ThreadValidator)
                return new StackIListEnumerator(this);
        }

        public bool Contains(object item)
        {
            using (ThreadValidator)
            {
                bool flag = false;
                foreach (object obj in this)
                {
                    if (obj != null && obj.Equals(item) || obj == item)
                    {
                        flag = true;
                        break;
                    }
                }
                return flag;
            }
        }

        public int IndexOf(object item)
        {
            using (ThreadValidator)
            {
                IndexedTree.TreeEnumerator enumerator = _items.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    IndexedTree.TreeEntry current = enumerator.Current;
                    if (current.Value != null && current.Value.Equals(item) || current.Value == item)
                        return current.Index;
                }
                return -1;
            }
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
                EnsureNotInCallback("Invalid to remove item {0} from a VirtualList while inside a GetItem callback", index);
                if (index < 0 || index >= _count)
                    throw new ArgumentException(InvariantString.Format("Invalid index '{0}' passed to RemoveAt", index));
                object obj = null;
                if (_items.Contains(index))
                {
                    obj = _items[index];
                    _items.RemoveIndex(index);
                }
                if (_updater != null)
                {
                    _updater.RemoveIndex(index);
                    _updater.AdjustIndices(index, -1);
                }
                SetCount(_count - 1);
                if (_releaseBehavior == ReleaseBehavior.Dispose && obj != null)
                    DisposeItem(obj);
                FireSetChanged(UIListContentsChangeType.Remove, index, -1);
            }
        }

        public void Insert(int index, object item)
        {
            using (ThreadValidator)
                InsertWorker(index, true, item);
        }

        public void Insert(int index)
        {
            using (ThreadValidator)
                InsertWorker(index, false, null);
        }

        public int Add()
        {
            using (ThreadValidator)
                return AddWorker(false, null);
        }

        public int Add(object item)
        {
            using (ThreadValidator)
                return AddWorker(true, item);
        }

        public void InsertRange(int index, IList items)
        {
            using (ThreadValidator)
            {
                if (items == null)
                    throw new ArgumentException("items should be non-null");
                if (items == this)
                    throw new ArgumentException("can't insert a list onto itself");
                if (items.Count <= 0)
                    return;
                InsertRangeWorker(index, items, items.Count);
            }
        }

        public void InsertRange(int index, int count)
        {
            using (ThreadValidator)
            {
                if (count < 0)
                    throw new ArgumentException("count should be non-negative");
                if (count <= 0)
                    return;
                InsertRangeWorker(index, null, count);
            }
        }

        public void AddRange(IList items)
        {
            using (ThreadValidator)
            {
                if (items == null)
                    throw new ArgumentException("items should be non-null");
                if (items == this)
                    throw new ArgumentException("can't add an item to itself");
                if (items.Count <= 0)
                    return;
                AddRangeWorker(items, items.Count);
            }
        }

        public void AddRange(int count)
        {
            using (ThreadValidator)
            {
                if (count < 0)
                    throw new ArgumentException("count should be non-negative");
                if (count <= 0)
                    return;
                AddRangeWorker(null, count);
            }
        }

        public void Move(int oldIndex, int newIndex)
        {
            using (ThreadValidator)
            {
                object data = this[oldIndex];
                _items.RemoveIndex(oldIndex);
                _items.Insert(newIndex, true, data);
                if (_updater != null)
                {
                    int lowThreshold;
                    int highThreshold;
                    int amt;
                    if (oldIndex < newIndex)
                    {
                        lowThreshold = oldIndex;
                        highThreshold = newIndex;
                        amt = -1;
                    }
                    else
                    {
                        lowThreshold = newIndex;
                        highThreshold = oldIndex;
                        amt = 1;
                    }
                    _updater.RemoveIndex(oldIndex);
                    _updater.AdjustIndices(lowThreshold, highThreshold, amt);
                    _updater.AddIndex(newIndex);
                }
                FireSetChanged(UIListContentsChangeType.Move, oldIndex, newIndex);
            }
        }

        public void Modified(int index)
        {
            using (ThreadValidator)
                ModifiedWorker(index, false, null);
        }

        [Conditional("DEBUG")]
        internal void DumpListContents()
        {
            foreach (IndexedTree.TreeEntry treeEntry in _items)
                ;
        }

        private void ValidateIndex(int index)
        {
            if (!ListUtility.IsValidIndex(index, Count))
                throw new IndexOutOfRangeException();
        }

        protected override void OnDispose(bool disposing)
        {
            if (disposing)
            {
                using (ThreadValidator)
                {
                    if (_updater != null)
                    {
                        _updater.Dispose();
                        _updater = null;
                    }
                    _items.Clear();
                }
            }
            base.OnDispose(disposing);
        }

        protected bool ContainsDataForIndex(int index)
        {
            using (ThreadValidator)
                return _items.Contains(index);
        }

        private void DisposeItem(object obj)
        {
            if (obj is IDisposable disposable)
                disposable.Dispose();
            else if (obj != UnavailableItem)
                throw new InvalidOperationException(InvariantString.Format("VirtualList {0} was configured with the {1} ReleaseBehavior.  This is only valid if the contents of the list implement IDisposable.  Unable to dispose object: {2}.", this, _releaseBehavior, obj));
        }

        private void InsertWorker(int index, bool setValue, object obj)
        {
            EnsureNotInCallback("Invalid to insert item {0} on VirtualList while inside a GetItem callback", index);
            if (index != Count)
                ValidateIndex(index);
            if (_updater != null)
                _updater.AdjustIndices(index, 1);
            _items.Insert(index, setValue, obj);
            SetCount(_count + 1);
            FireSetChanged(UIListContentsChangeType.Insert, -1, index);
        }

        private void InsertRangeWorker(int index, IList values, int count)
        {
            EnsureNotInCallback("Invalid to insert items into VirtualList while inside a GetItem callback");
            if (index != Count)
                ValidateIndex(index);
            if (_updater != null)
                _updater.AdjustIndices(index, count);
            if (values != null)
                CopyRange(values, index);
            else
                _items.InsertRange(index, count);
            SetCount(_count + count);
            FireSetChanged(UIListContentsChangeType.InsertRange, -1, index, count);
        }

        private int AddWorker(bool setValue, object obj)
        {
            EnsureNotInCallback("Invalid to add an item to VirtualList while inside a GetItem callback");
            int count = _count;
            if (setValue)
                _items[count] = obj;
            SetCount(_count + 1);
            FireSetChanged(UIListContentsChangeType.Add, -1, count);
            return count;
        }

        private void AddRangeWorker(IList values, int count)
        {
            EnsureNotInCallback("Invalid to add an item to VirtualList while inside a GetItem callback");
            int count1 = _count;
            if (values != null)
                CopyRange(values, _count);
            SetCount(_count + count);
            FireSetChanged(UIListContentsChangeType.AddRange, -1, count1, count);
        }

        private void CopyRange(IList values, int startIndex)
        {
            int count = values.Count;
            for (int index = 0; index < count; ++index)
                _items.Insert(startIndex + index, true, values[index]);
        }

        private void ModifiedWorker(int index, bool setValue, object value)
        {
            ValidateIndex(index);
            bool flag = ContainsDataForIndex(index);
            object obj = _items[index];
            if (setValue)
                _items[index] = value;
            else if (_items.Contains(index))
                _items.Remove(index);
            if (!flag)
                return;
            FireSetChanged(UIListContentsChangeType.Modified, index, index);
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

        private event ListContentsChangedHandler ContentsChanged
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

        internal void FireSetChanged(UIListContentsChangeType type, int oldIndex, int newIndex) => FireSetChanged(type, oldIndex, newIndex, 1);

        internal void FireSetChanged(
          UIListContentsChangeType type,
          int oldIndex,
          int newIndex,
          int count)
        {
            UIDispatcher.VerifyOnApplicationThread();
            UIListContentsChangedHandler eventHandler = (UIListContentsChangedHandler)GetEventHandler(s_listContentsChangedEvent);
            if (eventHandler != null)
            {
                UIListContentsChangedArgs args = new UIListContentsChangedArgs(type, oldIndex, newIndex, count);
                eventHandler(this, args);
            }
            FirePropertyChanged("ContentsChanged");
        }

        private void EnsureNotInCallback(int indexToVerify, string message)
        {
            if (_callbackIndexesList.Contains(indexToVerify))
                throw new InvalidOperationException(message);
        }

        private void EnsureNotInCallback(int indexToVerify, string message, int param)
        {
            if (_callbackIndexesList.Contains(indexToVerify))
                throw new InvalidOperationException(InvariantString.Format(message, param));
        }

        private void EnsureNotInCallback(string message)
        {
            if (_callbackIndexesList.Count > 0)
                throw new InvalidOperationException(message);
        }

        private void EnsureNotInCallback(string message, int param)
        {
            if (_callbackIndexesList.Count > 0)
                throw new InvalidOperationException(InvariantString.Format(message, param));
        }
    }
}
