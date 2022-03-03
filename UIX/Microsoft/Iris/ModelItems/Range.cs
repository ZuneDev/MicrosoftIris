// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.ModelItems.Range
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System.Collections.Generic;

namespace Microsoft.Iris.ModelItems
{
    internal struct Range
    {
        private int _begin;
        private int _end;

        public Range(int begin, int end)
        {
            if (begin > end)
            {
                int num = begin;
                begin = end;
                end = num;
            }
            _begin = begin;
            _end = end;
        }

        public int Begin
        {
            get => _begin;
            set => _begin = value;
        }

        public int End
        {
            get => _end;
            set => _end = value;
        }

        public bool IsEmpty => _begin == _end;

        public bool IsEqual(Range other) => other.Begin == Begin && other.End == End;

        public override bool Equals(object other) => other is Range other1 && IsEqual(other1);

        public override int GetHashCode() => Begin.GetHashCode() ^ End.GetHashCode();

        public bool Contains(int point) => point >= _begin && point <= _end;

        public bool Intersects(Range other) => Begin <= other.End && End >= other.Begin;

        public bool Split(int split, out Range before, out Range after)
        {
            before = new Range(_begin, split);
            bool flag;
            if (split < _end)
            {
                after = new Range(split + 1, _end);
                flag = true;
            }
            else
            {
                after = new Range();
                flag = false;
            }
            return flag;
        }

        public List<int> ToList()
        {
            List<int> intList = new List<int>(_end - _begin);
            for (int begin = _begin; begin <= _end; ++begin)
                intList.Add(begin);
            return intList;
        }

        public override string ToString() => string.Format("{{{0} to {1}}}", _begin, _end);
    }
}
