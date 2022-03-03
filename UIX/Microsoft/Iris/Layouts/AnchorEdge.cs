// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Layouts.AnchorEdge
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System.Text;

namespace Microsoft.Iris.Layouts
{
    internal class AnchorEdge
    {
        private string _idName;
        private float _percentValue;
        private int _offsetValue;
        private float _maximumPercentValue;
        private int _maximumOffsetValue;
        private float _minimumPercentValue;
        private int _minimumOffsetValue;
        private bool _maximumSetFlag;
        private bool _minimumSetFlag;

        public AnchorEdge()
        {
        }

        public AnchorEdge(string id, float percent)
        {
            Id = id;
            Percent = percent;
        }

        public string Id
        {
            get => _idName;
            set => _idName = value;
        }

        public float Percent
        {
            get => _percentValue;
            set => _percentValue = value;
        }

        public int Offset
        {
            get => _offsetValue;
            set => _offsetValue = value;
        }

        public float MaximumPercent
        {
            get => _maximumPercentValue;
            set
            {
                _maximumPercentValue = value;
                _maximumSetFlag = true;
            }
        }

        public int MaximumOffset
        {
            get => _maximumOffsetValue;
            set
            {
                _maximumOffsetValue = value;
                _maximumSetFlag = true;
            }
        }

        public float MinimumPercent
        {
            get => _minimumPercentValue;
            set
            {
                _minimumPercentValue = value;
                _minimumSetFlag = true;
            }
        }

        public int MinimumOffset
        {
            get => _minimumOffsetValue;
            set
            {
                _minimumOffsetValue = value;
                _minimumSetFlag = true;
            }
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("(");
            stringBuilder.Append(_idName);
            stringBuilder.Append(", ");
            stringBuilder.Append(_percentValue);
            if (_offsetValue != 0)
            {
                stringBuilder.Append(", ");
                stringBuilder.Append(_offsetValue);
            }
            if (_maximumPercentValue > 0.0)
            {
                stringBuilder.Append(", MaximumPercent=");
                stringBuilder.Append(_maximumPercentValue);
                stringBuilder.Append(", MaximumOffset=");
                stringBuilder.Append(_maximumOffsetValue);
            }
            if (_minimumPercentValue > 0.0)
            {
                stringBuilder.Append(", MinimumPercent=");
                stringBuilder.Append(_minimumPercentValue);
                stringBuilder.Append(", MinimumOffset=");
                stringBuilder.Append(_minimumOffsetValue);
            }
            stringBuilder.Append(")");
            return stringBuilder.ToString();
        }

        public static bool operator ==(AnchorEdge lhs, AnchorEdge rhs)
        {
            if ((object)lhs == null && (object)rhs == null)
                return true;
            return (object)lhs != null && lhs.Equals(rhs);
        }

        public static bool operator !=(AnchorEdge lhs, AnchorEdge rhs) => !(lhs == rhs);

        public override bool Equals(object obj)
        {
            AnchorEdge anchorEdge = obj as AnchorEdge;
            return (object)anchorEdge != null && Id == anchorEdge.Id && (Percent == (double)anchorEdge.Percent && Offset == anchorEdge.Offset) && (MaximumSet == anchorEdge.MaximumSet && MaximumPercent == (double)anchorEdge.MaximumPercent && (MaximumOffset == anchorEdge.MaximumOffset && MinimumSet == anchorEdge.MinimumSet)) && MinimumPercent == (double)anchorEdge.MinimumPercent && MinimumOffset == anchorEdge.MinimumOffset;
        }

        public override int GetHashCode() => (Id != null ? Id.GetHashCode() : 0) ^ Percent.GetHashCode() ^ Offset << 8 ^ MaximumSet.GetHashCode() ^ MaximumPercent.GetHashCode() ^ MaximumOffset << 16 ^ MinimumSet.GetHashCode() ^ MinimumPercent.GetHashCode() ^ MinimumOffset << 24;

        internal bool MaximumSet => _maximumSetFlag;

        internal bool MinimumSet => _minimumSetFlag;
    }
}
