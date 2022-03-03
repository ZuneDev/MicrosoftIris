// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.GroupedList
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Data;
using Microsoft.Iris.Session;
using System;
using System.Collections;

namespace Microsoft.Iris
{
    public class GroupedList : VirtualList
    {
        private IList _source;
        private IComparer _comparer;
        private Vector<Group> _groups = new Vector<Group>();
        private bool _repairGroupsPending;
        private bool _adjustCountPending;

        public GroupedList()
          : base(true)
        {
        }

        public GroupedList(IList source, IComparer comparer, int count)
          : base(true)
        {
            Comparer = comparer;
            SetSource(source, count);
        }

        public IList Source
        {
            get => _source;
            set => SetSource(value, 1, false);
        }

        public void SetSource(IList source, int groupCount) => SetSource(source, groupCount, false);

        private void SetSource(IList value, int groupCount, bool disposing)
        {
            if (_source == value)
                return;
            if (_source is INotifyList sourceNotifyA)
                sourceNotifyA.ContentsChanged -= new UIListContentsChangedHandler(SourceListModified);
            if (_source is IVirtualList sourceVirtual && sourceVirtual.SlowDataRequestsEnabled)
                sourceVirtual.SlowDataAcquireCompleteHandler = null;
            _source = value;
            if (_source is INotifyList sourceNotifyB)
                sourceNotifyB.ContentsChanged += new UIListContentsChangedHandler(SourceListModified);
            if (_source is IVirtualList sourceVirtualB && sourceVirtualB.SlowDataRequestsEnabled)
                sourceVirtualB.SlowDataAcquireCompleteHandler = new SlowDataAcquireCompleteHandler(SourceSlowDataAcquired);
            if (disposing)
                return;
            FirePropertyChanged("Source");
            Regroup(groupCount);
        }

        public IComparer Comparer
        {
            get => _comparer;
            set
            {
                if (_comparer == value)
                    return;
                _comparer = value;
                Regroup();
                FirePropertyChanged(nameof(Comparer));
            }
        }

        public void Regroup() => Regroup(1);

        public void Regroup(int groupCount)
        {
            foreach (ModelItem group in _groups)
                group.Dispose();
            _groups.Clear();
            Clear();
            Count = Source == null || Source.Count <= 0 ? 0 : Math.Max(1, groupCount);
        }

        protected override object OnRequestItem(int index)
        {
            if (_repairGroupsPending && index >= _groups.Count)
                return null;
            EnsureGroup(index);
            return index >= _groups.Count ? null : (object)_groups[index];
        }

        private void ScheduleAdjustCount()
        {
            if (_adjustCountPending || GetCountAdjustment() == 0)
                return;
            DeferredCall.Post(DispatchPriority.Housekeeping, new DeferredHandler(AdjustCount));
        }

        private int GetCountAdjustment()
        {
            int num1 = 0;
            int num2 = Source != null ? Source.Count : 0;
            Group lastGroup = GetLastGroup();
            int num3 = lastGroup != null ? lastGroup.EndIndex + 1 : 0;
            int num4 = num2 - num3;
            int num5 = Count - _groups.Count;
            if (_groups.Count == Count && num4 > 0)
            {
                int num6 = num3 / _groups.Count;
                num1 = Math.Max(1, num4 / num6);
            }
            else if (lastGroup == null || num5 > num4)
                num1 = num4 - num5;
            return num1;
        }

        private void AdjustCount(object args)
        {
            _adjustCountPending = false;
            int countAdjustment = GetCountAdjustment();
            if (countAdjustment > 0)
            {
                AddRange(countAdjustment);
            }
            else
            {
                if (countAdjustment >= 0)
                    return;
                int num = -countAdjustment;
                for (int index = 0; index < num; ++index)
                    RemoveAt(Count - 1);
            }
        }

        private bool SourceSlowDataAcquired(IVirtualList list, int sourceIndex)
        {
            Group groupForSourceIndex = GetGroupForSourceIndex(sourceIndex);
            groupForSourceIndex?.NotifySlowDataAcquireComplete(sourceIndex - groupForSourceIndex.StartIndex);
            return false;
        }

        private void EnsureGroup(int maxGroup)
        {
            if (Comparer != null && Source != null && Source.Count > 0)
            {
                int previousGroupIndex = _groups.Count - 1;
                Group lastGroup = GetLastGroup();
                GroupItems(ref lastGroup, ref previousGroupIndex, Source.Count - 1, maxGroup, true, false);
            }
            ScheduleAdjustCount();
        }

        private void GroupItems(
          ref Group previousGroup,
          ref int previousGroupIndex,
          int endIndex,
          int maxGroup,
          bool createGroups,
          bool notify)
        {
            if (_groups.Count > maxGroup)
                return;
            if (previousGroup == null)
            {
                ++previousGroupIndex;
                previousGroup = InsertGroup(previousGroupIndex, 0, notify);
            }
            for (int index = previousGroup.EndIndex + 1; index <= endIndex; ++index)
            {
                if (!TryInsertItemIntoGroup(previousGroup, index))
                {
                    if (!createGroups || _groups.Count > maxGroup)
                        break;
                    ++previousGroupIndex;
                    previousGroup = InsertGroup(previousGroupIndex, index, notify);
                }
            }
        }

        protected override void OnDispose(bool disposing)
        {
            if (disposing)
                SetSource(null, 0, true);
            base.OnDispose(disposing);
        }

        private void SourceListModified(IList listSender, UIListContentsChangedArgs args)
        {
            if (Comparer == null)
                return;
            bool flag = false;
            switch (args.Type)
            {
                case UIListContentsChangeType.Add:
                case UIListContentsChangeType.AddRange:
                case UIListContentsChangeType.Insert:
                case UIListContentsChangeType.InsertRange:
                    if (Count != 0)
                    {
                        Group lastGroup = GetLastGroup();
                        if (lastGroup != null && args.NewIndex <= lastGroup.EndIndex + 1)
                        {
                            flag = true;
                            int newIndex = args.NewIndex;
                            int num = args.NewIndex + args.Count - 1;
                            int groupIndex;
                            Group groupForSourceIndex = GetGroupForSourceIndex(newIndex - 1, out groupIndex);
                            for (int index = groupIndex + 1; index < _groups.Count; ++index)
                                _groups[index].StartIndex += args.Count;
                            if (groupForSourceIndex != null && groupForSourceIndex.ContainsSourceIndex(newIndex))
                            {
                                SplitGroup(groupForSourceIndex, groupIndex, newIndex - 1, num + 1);
                                break;
                            }
                            break;
                        }
                        break;
                    }
                    Count = 1;
                    break;
                case UIListContentsChangeType.Remove:
                    int groupIndex1;
                    Group groupForSourceIndex1 = GetGroupForSourceIndex(args.OldIndex, out groupIndex1);
                    for (int index = groupIndex1 + 1; index < _groups.Count; ++index)
                        --_groups[index].StartIndex;
                    if (groupForSourceIndex1 != null)
                    {
                        if (groupForSourceIndex1.Count == 1)
                        {
                            RemoveGroup(groupIndex1);
                            flag = true;
                            break;
                        }
                        groupForSourceIndex1.RemoveAt(args.OldIndex - groupForSourceIndex1.StartIndex);
                        break;
                    }
                    break;
                case UIListContentsChangeType.Move:
                case UIListContentsChangeType.Modified:
                    throw new NotImplementedException();
                default:
                    Regroup();
                    break;
            }
            if (!flag || _repairGroupsPending)
                return;
            _repairGroupsPending = true;
            DeferredCall.Post(DispatchPriority.Housekeeping, new DeferredHandler(RepairGroups));
        }

        private void RepairGroups(object args)
        {
            _repairGroupsPending = false;
            for (int previousGroupIndex = -1; previousGroupIndex < _groups.Count; ++previousGroupIndex)
            {
                Group previousGroup = previousGroupIndex > -1 ? _groups[previousGroupIndex] : null;
                Group group = previousGroupIndex + 1 < _groups.Count ? _groups[previousGroupIndex + 1] : null;
                int num1 = previousGroup != null ? previousGroup.EndIndex + 1 : 0;
                int num2 = group != null ? group.StartIndex - 1 : Source.Count - 1;
                if (group != null)
                {
                    while (num2 >= num1 && TryInsertItemIntoGroup(group, num2))
                        --num2;
                }
                if (num1 <= num2)
                    GroupItems(ref previousGroup, ref previousGroupIndex, num2, int.MaxValue, group != null, true);
                if (TryMergeWithNext(previousGroupIndex))
                    --previousGroupIndex;
            }
            AdjustCount(null);
        }

        private bool IsEqualToNext(int sourceIndex) => Comparer.Compare(Source[sourceIndex], Source[sourceIndex + 1]) == 0;

        private bool TryMergeWithNext(int i)
        {
            if (i >= 0 && i < _groups.Count - 1)
            {
                Group group1 = _groups[i];
                if (IsEqualToNext(group1.EndIndex))
                {
                    Group group2 = _groups[i + 1];
                    group1.AddRange(group2.Count);
                    RemoveGroup(i + 1);
                    return true;
                }
            }
            return false;
        }

        private void RemoveGroup(int groupIndex)
        {
            _groups[groupIndex].Dispose();
            _groups.RemoveAt(groupIndex);
            RemoveAt(groupIndex);
        }

        private bool TryInsertItemIntoGroup(Group group, int index)
        {
            int sourceIndex = index - 1;
            if (index < group.StartIndex)
                sourceIndex = index;
            if (!IsEqualToNext(sourceIndex))
                return false;
            group.StartIndex = Math.Min(group.StartIndex, index);
            group.Insert(index - group.StartIndex);
            return true;
        }

        private Group SplitGroup(
          Group group,
          int groupIndex,
          int firstGroupSourceEndIndex,
          int secondGroupSourceStartIndex)
        {
            int count = group.EndIndex - firstGroupSourceEndIndex;
            for (int index = 0; index < count; ++index)
                group.RemoveAt(group.Count - 1);
            return InsertGroup(groupIndex + 1, secondGroupSourceStartIndex, count, true);
        }

        private Group InsertGroup(int groupInsertIndex, int sourceIndex, bool notify) => InsertGroup(groupInsertIndex, sourceIndex, 1, notify);

        private Group InsertGroup(
          int groupInsertIndex,
          int startSourceIndex,
          int count,
          bool notify)
        {
            Group group = new Group(this, startSourceIndex, count);
            group.Count = count;
            _groups.Insert(groupInsertIndex, group);
            if (notify)
                Insert(groupInsertIndex);
            return group;
        }

        private Group GetLastGroup() => _groups.Count <= 0 ? null : _groups[_groups.Count - 1];

        private Group GetGroupForSourceIndex(int sourceIndex, out int groupIndex)
        {
            int num1 = 0;
            int num2 = _groups.Count - 1;
            groupIndex = num1 + (num2 - num1) / 2;
            while (num1 <= num2)
            {
                Group group = _groups[groupIndex];
                if (group.StartIndex <= sourceIndex && sourceIndex <= group.EndIndex)
                    return group;
                if (group.StartIndex < sourceIndex)
                    num1 = groupIndex + 1;
                else
                    num2 = groupIndex - 1;
                groupIndex = num1 + (num2 - num1) / 2;
            }
            --groupIndex;
            return null;
        }

        public Group GetGroupForSourceIndex(int sourceIndex) => GetGroupForSourceIndex(sourceIndex, out int _);

        public int GetGroupIndexForSourceIndex(int sourceIndex)
        {
            for (int index = 0; index < Count; ++index)
            {
                Group group = (Group)this[index];
                if (group != null)
                {
                    if (sourceIndex < group.Count)
                        return index;
                    sourceIndex -= group.Count;
                }
                else
                    break;
            }
            return -1;
        }
    }
}
