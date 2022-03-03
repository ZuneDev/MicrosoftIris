// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Data.UpdateHelper
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Session;
using Microsoft.Iris.ViewItems;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.Iris.Data
{
    internal class UpdateHelper
    {
        private const int c_millisecondsBetweenUpdates = 100;
        private const int c_millisecondsAllowedPerUpdate = 10;
        private UpdateHelper.ItemDistanceComparer _itemDistanceComparer;
        private IVirtualList _virtualList;
        private List<int> _itemsToUpdate;
        private int _lastInterestIndex;
        private DispatcherTimer _timer;
        private bool _listIsDirty;
        private List<int> _outstandingUpdates;
        private int _throttle;

        public UpdateHelper(IVirtualList virtualList)
        {
            _virtualList = virtualList;
            _itemsToUpdate = new List<int>();
            _throttle = Environment.ProcessorCount * 2;
            _outstandingUpdates = new List<int>();
        }

        public void AddIndex(int index)
        {
            _itemsToUpdate.Add(index);
            _listIsDirty = true;
            TriggerNextUpdate();
        }

        public void AdjustIndices(int lowThreshold, int amount) => AdjustIndices(lowThreshold, int.MaxValue, amount);

        public void AdjustIndices(int lowThreshold, int highThreshold, int amt)
        {
            for (int index = 0; index < _itemsToUpdate.Count; ++index)
            {
                int num = _itemsToUpdate[index];
                if (num >= lowThreshold && num <= highThreshold)
                    _itemsToUpdate[index] = num + amt;
            }
            if (_lastInterestIndex < lowThreshold || _lastInterestIndex > highThreshold)
                return;
            _lastInterestIndex += amt;
        }

        public void Clear() => _itemsToUpdate.Clear();

        public void RemoveIndex(int index)
        {
            int count = _itemsToUpdate.Count;
            int index1 = 0;
            while (index1 < count && _itemsToUpdate[index1] != index)
                ++index1;
            if (index1 >= count)
                return;
            int index2 = index1;
            for (int index3 = index1 + 1; index3 < count; ++index3)
            {
                int num = _itemsToUpdate[index3];
                if (num != index)
                {
                    _itemsToUpdate[index2] = num;
                    ++index2;
                }
            }
            _itemsToUpdate.RemoveRange(index2, count - index2);
        }

        public void Dispose()
        {
            if (_timer == null)
                return;
            _timer.Enabled = false;
        }

        private void TriggerNextUpdate()
        {
            EnsureTimer();
            _timer.Enabled = true;
        }

        private void DeliverNextUpdate(object senderObject, EventArgs unusedArgs)
        {
            if (_itemsToUpdate.Count == 0)
                return;
            if (_outstandingUpdates.Count == _throttle)
            {
                TriggerNextUpdate();
            }
            else
            {
                Repeater repeaterHost = _virtualList.RepeaterHost;
                if (repeaterHost == null)
                    return;
                int index1 = -1;
                repeaterHost.GetFocusedIndex(ref index1);
                if (repeaterHost.GetExtendedLayoutOutput(VisibleIndexRangeLayoutOutput.DataCookie) is VisibleIndexRangeLayoutOutput extendedLayoutOutput)
                {
                    if (index1 != -1 && (index1 < extendedLayoutOutput.BeginVisible || index1 > extendedLayoutOutput.EndVisible))
                        index1 = -1;
                    if (index1 == -1)
                        index1 = extendedLayoutOutput.BeginVisible;
                }
                if (index1 != -1)
                {
                    int dataIndex;
                    ListUtility.GetWrappedIndex(index1, _virtualList.Count, out dataIndex, out int _);
                    if (_lastInterestIndex != dataIndex)
                        _listIsDirty = true;
                    _lastInterestIndex = dataIndex;
                }
                if (_listIsDirty)
                {
                    if (_itemDistanceComparer == null)
                        _itemDistanceComparer = new UpdateHelper.ItemDistanceComparer();
                    _itemDistanceComparer.Initialize(_lastInterestIndex, _virtualList.Count);
                    _itemsToUpdate.Sort(_itemDistanceComparer);
                    _listIsDirty = false;
                }
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                int count = 0;
                do
                {
                    ++count;
                    int index2 = _itemsToUpdate[_itemsToUpdate.Count - count];
                    _outstandingUpdates.Add(index2);
                    _virtualList.NotifyRequestSlowData(index2);
                }
                while (stopwatch.ElapsedMilliseconds < 10L && _itemsToUpdate.Count != count && _outstandingUpdates.Count < _throttle);
                _itemsToUpdate.RemoveRange(_itemsToUpdate.Count - count, count);
                if (_itemsToUpdate.Count == 0)
                    return;
                TriggerNextUpdate();
            }
        }

        public int Throttle
        {
            get => _throttle;
            set => _throttle = value;
        }

        public bool NotifySlowDataAcquireComplete(int index) => _outstandingUpdates.Remove(index);

        private void EnsureTimer()
        {
            if (_timer != null)
                return;
            _timer = new DispatcherTimer();
            _timer.AutoRepeat = false;
            _timer.Interval = 100;
            _timer.Tick += new EventHandler(DeliverNextUpdate);
        }

        private class ItemDistanceComparer : IComparer<int>
        {
            private int _focusedIndex;
            private int _totalCount;
            private int _midPoint;

            public void Initialize(int focusedIndex, int totalCount)
            {
                _focusedIndex = focusedIndex;
                _totalCount = totalCount;
                _midPoint = totalCount / 2;
            }

            public int Compare(int left, int right) => GetDistance(right) - GetDistance(left);

            private int GetDistance(int potential)
            {
                int num = Math.Abs(potential - _focusedIndex);
                if (num > _midPoint)
                    num = _totalCount - num + 1;
                return num;
            }
        }
    }
}
