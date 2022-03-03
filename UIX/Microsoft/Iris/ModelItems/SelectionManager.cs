// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.ModelItems.SelectionManager
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Data;
using Microsoft.Iris.Library;
using Microsoft.Iris.Markup;
using Microsoft.Iris.Session;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Iris.ModelItems
{
    internal class SelectionManager : DisposableNotifyObjectBase
    {
        private Vector<Range> _selected;
        private int? _anchor;
        private IList _sourceList;
        private int _count;
        private IList _selectedIndicesCache;
        private IList _selectedItemsCache;
        private bool _singleSelect;
        public static readonly object[] s_emptyList = new object[0];

        public SelectionManager() => _selected = new Vector<Range>();

        protected override void OnDispose()
        {
            base.OnDispose();
            UnhookContentsChangedHandler();
        }

        public bool IsSelected(int index)
        {
            foreach (Range range in _selected)
            {
                if (range.Contains(index))
                    return true;
            }
            return false;
        }

        public bool IsRangeSelected(int begin, int end) => IsRangeSelected(new Range(begin, end));

        private bool IsRangeSelected(Range range)
        {
            for (int begin = range.Begin; begin <= range.End; ++begin)
            {
                if (!IsSelected(begin))
                    return false;
            }
            return true;
        }

        public IList SelectedIndices
        {
            get
            {
                if (_selectedIndicesCache == null)
                {
                    List<int> list = new List<int>();
                    foreach (Range range in _selected)
                    {
                        foreach (int num in range.ToList())
                        {
                            if (!IntListUtility.Contains(list, num))
                                list.Add(num);
                        }
                    }
                    list.Sort();
                    _selectedIndicesCache = new SelectionManager.ReadOnlyList(list, nameof(SelectedIndices));
                }
                return _selectedIndicesCache;
            }
        }

        public IList SelectedItems
        {
            get
            {
                if (_selectedItemsCache == null)
                {
                    IList originalList = null;
                    if (Count > 0 && SourceList != null)
                    {
                        originalList = new List<object>();
                        foreach (int original in (List<int>)((SelectionManager.ReadOnlyList)SelectedIndices).OriginalList)
                        {
                            if (IsValidIndex(original))
                                originalList.Add(SourceList[original]);
                        }
                    }
                    if (originalList == null)
                        originalList = s_emptyList;
                    _selectedItemsCache = new SelectionManager.ReadOnlyList(originalList, nameof(SelectedItems));
                }
                return _selectedItemsCache;
            }
        }

        public int SelectedIndex
        {
            get => Count > 0 ? (int)SelectedIndices[0] : -1;
            set
            {
                if (!IsValidIndex(value) || Count == 1 && IsSelected(value))
                    return;
                Clear();
                if (value == -1)
                    return;
                Select(value, true);
            }
        }

        public object SelectedItem => Count > 0 ? SelectedItems[0] : null;

        public int Count => _count;

        public IList SourceList
        {
            get => _sourceList;
            set
            {
                if (_sourceList == value)
                    return;
                Clear(true);
                UnhookContentsChangedHandler();
                _sourceList = value;
                if (value != null && value is INotifyList notifyList)
                    notifyList.ContentsChanged += new UIListContentsChangedHandler(OnListContentsChanged);
                FireNotification(NotificationID.SourceList);
            }
        }

        private void UnhookContentsChangedHandler()
        {
            if (_sourceList == null || !(_sourceList is INotifyList sourceList))
                return;
            sourceList.ContentsChanged -= new UIListContentsChangedHandler(OnListContentsChanged);
        }

        public int Anchor
        {
            get => !_anchor.HasValue ? 0 : _anchor.Value;
            set => SetAnchor(new int?(value));
        }

        private void SetAnchor(int? value)
        {
            if (_anchor.HasValue == value.HasValue && (!_anchor.HasValue || _anchor.Value == value.Value))
                return;
            _anchor = value;
            FireNotification(NotificationID.Anchor);
        }

        public bool SingleSelect
        {
            get => _singleSelect;
            set
            {
                if (_singleSelect == value)
                    return;
                if (value && Count > 1)
                    SelectedIndex = (int)SelectedIndices[0];
                _singleSelect = value;
                FireNotification(NotificationID.SingleSelect);
            }
        }

        private bool IsValidIndex(int index)
        {
            if (SourceList == null)
                return true;
            return index >= 0 && index < SourceList.Count;
        }

        private void ValidateMultiSelect(string operation)
        {
            if (!SingleSelect)
                return;
            ErrorManager.ReportError("Calling {0} is not supported on a SelectionManager in single selection modes.", operation);
        }

        private void OnListContentsChanged(IList senderList, UIListContentsChangedArgs args)
        {
            UIListContentsChangeType type = args.Type;
            int oldIndex = args.OldIndex;
            int newIndex = args.NewIndex;
            int count = args.Count;
            bool flag1 = false;
            bool countChanged = false;
            switch (type)
            {
                case UIListContentsChangeType.Add:
                case UIListContentsChangeType.AddRange:
                case UIListContentsChangeType.Insert:
                case UIListContentsChangeType.InsertRange:
                    Vector<Range> vector = new Vector<Range>();
                    for (int index = 0; index < _selected.Count; ++index)
                    {
                        Range range = _selected[index];
                        if (range.End >= newIndex)
                        {
                            int num = range.Begin;
                            if (range.Contains(newIndex - 1))
                            {
                                Range before;
                                range.Split(newIndex - 1, out before, out Range _);
                                vector.Add(before);
                                num = newIndex;
                            }
                            _selected[index] = new Range(num + count, range.End + count);
                            flag1 = true;
                        }
                    }
                    if (vector.Count > 0)
                    {
                        foreach (Range range in vector)
                            _selected.Add(range);
                    }
                    if (Anchor >= newIndex)
                    {
                        Anchor += count;
                        break;
                    }
                    break;
                case UIListContentsChangeType.Remove:
                    if (SourceList.Count > 0)
                    {
                        if (IsSelected(oldIndex))
                        {
                            RemoveRange(new Range(oldIndex, oldIndex), false);
                            flag1 = true;
                            countChanged = true;
                        }
                        for (int index = 0; index < _selected.Count; ++index)
                        {
                            Range range = _selected[index];
                            if (range.End > oldIndex)
                            {
                                _selected[index] = new Range(range.Begin - count, range.End - count);
                                flag1 = true;
                            }
                        }
                        if (Anchor >= oldIndex)
                        {
                            Anchor -= count;
                            break;
                        }
                        break;
                    }
                    Clear(true);
                    break;
                case UIListContentsChangeType.Move:
                    if (oldIndex != newIndex)
                    {
                        bool flag2 = IsSelected(oldIndex);
                        if (flag2)
                        {
                            Select(oldIndex, false, false, false);
                            flag1 = true;
                        }
                        bool flag3 = IsSelected(newIndex);
                        if (flag3)
                        {
                            Select(newIndex, false, false, false);
                            flag1 = true;
                        }
                        int num = oldIndex < newIndex ? -1 : 1;
                        Range other = new Range(oldIndex, newIndex);
                        for (int index = 0; index < _selected.Count; ++index)
                        {
                            Range range = _selected[index];
                            if (range.Intersects(other))
                            {
                                _selected[index] = new Range(range.Begin + num, range.End + num);
                                flag1 = true;
                            }
                        }
                        if (flag2)
                            Select(newIndex, true, false, false);
                        if (flag3)
                            Select(newIndex + num, true, false, false);
                        if (Anchor == oldIndex)
                        {
                            Anchor = newIndex;
                            break;
                        }
                        if (other.Contains(Anchor))
                        {
                            Anchor += num;
                            break;
                        }
                        break;
                    }
                    break;
                case UIListContentsChangeType.Clear:
                case UIListContentsChangeType.Reset:
                    Clear(true);
                    break;
            }
            if (!flag1)
                return;
            OnSelectionChanged(countChanged);
        }

        public void Clear() => Clear(false);

        private void Clear(bool resetAnchor)
        {
            if (_selected.Count > 0)
            {
                _selected.Clear();
                _count = 0;
                OnSelectedIndicesChanged();
            }
            if (!resetAnchor)
                return;
            SetAnchor(new int?());
        }

        public bool Select(int index, bool select) => Select(index, select, true, true);

        private bool Select(int index, bool select, bool rememberAnchor, bool fireSelectionChanged)
        {
            if (!IsValidIndex(index))
                return false;
            if (IsSelected(index) == select)
                return true;
            Range range = new Range(index, index);
            if (select)
            {
                if (SingleSelect)
                    Clear();
                AddRange(range, fireSelectionChanged);
            }
            else
                RemoveRange(range, fireSelectionChanged);
            if (rememberAnchor)
                Anchor = index;
            return true;
        }

        public bool Select(IList indices, bool select)
        {
            ValidateMultiSelect("Select(IList, bool)");
            bool flag = true;
            foreach (object index in indices)
                flag &= Select((int)index, select, false, true);
            return flag;
        }

        public bool ToggleSelect(int item) => Select(item, !IsSelected(item));

        public bool ToggleSelect(IList items)
        {
            ValidateMultiSelect("ToggleSelect(IList)");
            bool flag = true;
            foreach (int index in items)
                flag &= Select(index, !IsSelected(index), false, true);
            return flag;
        }

        public bool SelectRange(int begin, int end) => SelectRange(new Range(begin, end), begin);

        private bool SelectRange(Range range, int anchor)
        {
            ValidateMultiSelect(nameof(SelectRange));
            if (!IsValidIndex(range.Begin) || !IsValidIndex(range.End))
                return false;
            AddRange(range, true);
            Anchor = anchor;
            return true;
        }

        public bool SelectRangeFromAnchor(int end)
        {
            ValidateMultiSelect(nameof(SelectRangeFromAnchor));
            int num = Anchor;
            if (SourceList != null)
                num = Math.Max(0, Math.Min(num, SourceList.Count - 1));
            return SelectRange(new Range(num, end), num);
        }

        public bool SelectRangeFromAnchor(int rangeStart, int rangeEnd)
        {
            ValidateMultiSelect(nameof(SelectRangeFromAnchor));
            Range range = new Range(rangeStart, rangeEnd);
            if (range.Begin > Anchor)
                return SelectRangeFromAnchor(range.End);
            return Anchor > range.End ? SelectRangeFromAnchor(range.Begin) : SelectRange(range, rangeStart);
        }

        public bool ToggleSelectRange(int begin, int end)
        {
            ValidateMultiSelect(nameof(ToggleSelectRange));
            if (!IsValidIndex(begin) || !IsValidIndex(end))
                return false;
            Range range = new Range(begin, end);
            if (IsRangeSelected(range))
                RemoveRange(range, true);
            else
                AddRange(range, true);
            Anchor = range.Begin;
            return true;
        }

        private void AddRange(Range addRange, bool fireSelectionChanged)
        {
            int count = Count;
            for (int begin = addRange.Begin; begin <= addRange.End; ++begin)
            {
                if (!IsSelected(begin))
                    ++_count;
            }
            if (count == _count)
                return;
            _selected.Add(addRange);
            if (!fireSelectionChanged)
                return;
            OnSelectedIndicesChanged();
        }

        private void RemoveRange(Range removeRange, bool fireSelectionChanged)
        {
            int count = Count;
            for (int begin = removeRange.Begin; begin <= removeRange.End; ++begin)
            {
                if (IsSelected(begin))
                    --_count;
            }
            if (count == _count)
                return;
            Vector<Range> vector1 = new Vector<Range>();
            Vector<Range> vector2 = new Vector<Range>();
            foreach (Range other in _selected)
            {
                if (removeRange.Intersects(other))
                {
                    Range range;
                    if (other.Contains(removeRange.Begin - 1))
                    {
                        Range before;
                        other.Split(removeRange.Begin - 1, out before, out range);
                        vector2.Add(before);
                    }
                    Range after;
                    if (other.Contains(removeRange.End) && other.Split(removeRange.End, out range, out after))
                        vector2.Add(after);
                    vector1.Add(other);
                }
            }
            if (vector1.Count <= 0 && vector2.Count <= 0)
                return;
            foreach (Range range in vector1)
                _selected.Remove(range);
            foreach (Range range in vector2)
                _selected.Add(range);
            if (!fireSelectionChanged)
                return;
            OnSelectedIndicesChanged();
        }

        private void OnSelectedIndicesChanged() => OnSelectionChanged(true);

        private void OnSelectionChanged(bool countChanged)
        {
            _selectedIndicesCache = null;
            FireNotification(NotificationID.SelectedIndices);
            FireNotification(NotificationID.SelectedIndex);
            if (!countChanged)
                return;
            FireNotification(NotificationID.Count);
            if (SourceList == null)
                return;
            _selectedItemsCache = null;
            FireNotification(NotificationID.SelectedItems);
            FireNotification(NotificationID.SelectedItem);
        }

        internal class ReadOnlyList : IList, ICollection, IEnumerable
        {
            private IList _originalList;
            private string _listName;

            public ReadOnlyList(IList originalList, string listName)
            {
                _originalList = originalList;
                _listName = listName;
            }

            public IList OriginalList => _originalList;

            public object this[int index]
            {
                get => _originalList[index];
                set => ErrorManager.ReportError("Cannot modify selection through the list returned by {0}.  Use the methods on SelectionManager instead.", _listName);
            }

            public int Count => _originalList.Count;

            public bool Contains(object value) => _originalList.Contains(value);

            public int IndexOf(object value) => _originalList.IndexOf(value);

            public bool IsFixedSize => true;

            public bool IsReadOnly => true;

            public bool IsSynchronized => false;

            public object SyncRoot => _originalList;

            public IEnumerator GetEnumerator() => _originalList.GetEnumerator();

            public void CopyTo(Array array, int index) => _originalList.CopyTo(array, index);

            public int Add(object value)
            {
                ErrorManager.ReportError("Cannot modify selection through the list returned by {0}.  Use the methods on SelectionManager instead.", _listName);
                return -1;
            }

            public void Clear() => ErrorManager.ReportError("Cannot modify selection through the list returned by {0}.  Use the methods on SelectionManager instead.", _listName);

            public void Insert(int index, object value) => ErrorManager.ReportError("Cannot modify selection through the list returned by {0}.  Use the methods on SelectionManager instead.", _listName);

            public void Remove(object value) => ErrorManager.ReportError("Cannot modify selection through the list returned by {0}.  Use the methods on SelectionManager instead.", _listName);

            public void RemoveAt(int index) => ErrorManager.ReportError("Cannot modify selection through the list returned by {0}.  Use the methods on SelectionManager instead.", _listName);
        }
    }
}
