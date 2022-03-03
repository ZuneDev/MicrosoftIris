// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.AggregateList
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Data;
using System;
using System.Collections;
using System.Diagnostics;

namespace Microsoft.Iris
{
    public class AggregateList : VirtualList
    {
        private IList[] _lists;

        public AggregateList()
          : this(null)
        {
        }

        public AggregateList(IList list1)
          : this(new IList[1] { list1 })
        {
        }

        public AggregateList(IList list1, IList list2)
          : this(new IList[2] { list1, list2 })
        {
        }

        public AggregateList(IList list1, IList list2, IList list3)
          : this(new IList[3] { list1, list2, list3 })
        {
        }

        public AggregateList(IList list1, IList list2, IList list3, IList list4)
          : this(new IList[4] { list1, list2, list3, list4 })
        {
        }

        public AggregateList(IList[] lists)
          : base(true)
        {
            if (lists == null)
                lists = new IList[0];
            _lists = lists;
            UIListContentsChangedHandler contentsChangedHandler = new UIListContentsChangedHandler(ChildListModified);
            SlowDataAcquireCompleteHandler acquireCompleteHandler = new SlowDataAcquireCompleteHandler(ChildListSlowDataAcquired);
            for (int index = 0; index < _lists.Length; ++index)
            {
                if (_lists[index] == null)
                    _lists[index] = new ArrayList();
                IList list = _lists[index];
                if (list is INotifyList notifyList)
                    notifyList.ContentsChanged += contentsChangedHandler;
                if (list is IVirtualList virtualList && virtualList.SlowDataRequestsEnabled)
                    virtualList.SlowDataAcquireCompleteHandler = acquireCompleteHandler;
            }
            Count = ItemCount;
        }

        protected override void OnDispose(bool disposing)
        {
            if (disposing)
            {
                UIListContentsChangedHandler contentsChangedHandler = new UIListContentsChangedHandler(ChildListModified);
                foreach (IList list in _lists)
                {
                    if (list is INotifyList notifyList)
                        notifyList.ContentsChanged -= contentsChangedHandler;
                    if (list is IVirtualList virtualList && virtualList.SlowDataRequestsEnabled)
                        virtualList.SlowDataAcquireCompleteHandler = null;
                }
            }
            base.OnDispose(disposing);
        }

        protected override object OnRequestItem(int index)
        {
            foreach (IList list in _lists)
            {
                if (index < list.Count)
                    return list[index];
                index -= list.Count;
            }
            throw new IndexOutOfRangeException(nameof(index));
        }

        protected override void OnRequestSlowData(int index)
        {
            foreach (IList list in _lists)
            {
                if (index < list.Count)
                {
                    if (list is IVirtualList virtualList && virtualList.SlowDataRequestsEnabled)
                    {
                        virtualList.NotifyRequestSlowData(index);
                        break;
                    }
                    break;
                }
                index -= list.Count;
            }
            NotifySlowDataAcquireComplete(index);
        }

        private bool ChildListSlowDataAcquired(IVirtualList childList, int index)
        {
            NotifySlowDataAcquireComplete(ListIndexToMasterIndex(childList, index));
            return true;
        }

        private void ChildListModified(IList listSender, UIListContentsChangedArgs args)
        {
            int masterIndex1 = ListIndexToMasterIndex(listSender, args.OldIndex);
            int masterIndex2 = ListIndexToMasterIndex(listSender, args.NewIndex);
            switch (args.Type)
            {
                case UIListContentsChangeType.Add:
                case UIListContentsChangeType.Insert:
                    Insert(masterIndex2);
                    break;
                case UIListContentsChangeType.AddRange:
                case UIListContentsChangeType.InsertRange:
                    InsertRange(masterIndex2, args.Count);
                    break;
                case UIListContentsChangeType.Remove:
                    RemoveAt(masterIndex1);
                    break;
                case UIListContentsChangeType.Move:
                    if (masterIndex1 == masterIndex2)
                        break;
                    Move(masterIndex1, masterIndex2);
                    break;
                case UIListContentsChangeType.Modified:
                    Modified(masterIndex2);
                    break;
                default:
                    Clear();
                    Count = ItemCount;
                    break;
            }
        }

        [Conditional("DEBUG")]
        private void DEBUG_ValidateConsistency()
        {
            int index1 = 0;
            for (int index2 = 0; index2 < _lists.Length; ++index2)
            {
                for (int index3 = 0; index3 < _lists[index2].Count; ++index3)
                {
                    object obj1 = _lists[index2][index3];
                    object obj2 = this[index1];
                    ++index1;
                }
            }
        }

        private int ListIndexToMasterIndex(IList list, int index)
        {
            int num = 0;
            for (int index1 = 0; index1 < _lists.Length && _lists[index1] != list; ++index1)
                num += _lists[index1].Count;
            return num + index;
        }

        private void MasterIndexToListIndex(int masterIndex, out IList list, out int index)
        {
            int num = 0;
            int index1 = 0;
            list = null;
            for (; index1 < _lists.Length; ++index1)
            {
                list = _lists[index1];
                if (num + list.Count <= masterIndex)
                    num += list.Count;
                else
                    break;
            }
            index = masterIndex - num;
        }

        private int ItemCount
        {
            get
            {
                int num = 0;
                foreach (IList list in _lists)
                    num += list.Count;
                return num;
            }
        }
    }
}
