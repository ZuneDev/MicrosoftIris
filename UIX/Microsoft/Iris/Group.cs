// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Group
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Data;

namespace Microsoft.Iris
{
    public class Group : VirtualList
    {
        private GroupedList _groupedList;
        private int _startIndex;

        internal Group(GroupedList groupedList, int startIndex, int count)
          : base(groupedList, true, null)
        {
            _groupedList = groupedList;
            _startIndex = startIndex;
            Count = count;
        }

        public int StartIndex
        {
            get => _startIndex;
            internal set
            {
                if (_startIndex == value)
                    return;
                _startIndex = value;
                FirePropertyChanged(nameof(StartIndex));
                FirePropertyChanged("EndIndex");
            }
        }

        public int EndIndex => StartIndex + Count - 1;

        internal override void OnCountChanged() => FirePropertyChanged("EndIndex");

        private int GetSourceIndex(int groupedIndex) => StartIndex + groupedIndex;

        internal bool ContainsSourceIndex(int sourceIndex) => StartIndex <= sourceIndex && sourceIndex <= EndIndex;

        protected override object OnRequestItem(int index) => _groupedList.Source[GetSourceIndex(index)];

        protected override void OnRequestSlowData(int groupedIndex)
        {
            if (!(_groupedList.Source is IVirtualList source) || !source.SlowDataRequestsEnabled)
                return;
            int sourceIndex = GetSourceIndex(groupedIndex);
            if (sourceIndex >= source.Count)
                return;
            source.NotifyRequestSlowData(sourceIndex);
        }

        public override string ToString() => "Group [" + StartIndex + "-" + EndIndex + "]";
    }
}
