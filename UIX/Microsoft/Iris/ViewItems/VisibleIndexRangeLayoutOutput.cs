// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.ViewItems.VisibleIndexRangeLayoutOutput
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Layout;
using Microsoft.Iris.Library;

namespace Microsoft.Iris.ViewItems
{
    internal class VisibleIndexRangeLayoutOutput : ExtendedLayoutOutput
    {
        private static readonly DataCookie s_dataProperty = DataCookie.ReserveSlot();
        private int _beginVisible;
        private int _endVisible;
        private int _beginVisibleOffscreen;
        private int _endVisibleOffscreen;
        private int? _focusedItem;

        public void Initialize(
          int beginVisible,
          int endVisible,
          int beginVisibleOffscreen,
          int endVisibleOffscreen,
          int? focusedItem)
        {
            _beginVisible = beginVisible;
            _endVisible = endVisible;
            _beginVisibleOffscreen = beginVisibleOffscreen;
            _endVisibleOffscreen = endVisibleOffscreen;
            _focusedItem = focusedItem;
        }

        public int? FocusedItem => _focusedItem;

        public int BeginVisible => _beginVisible;

        public int EndVisible => _endVisible;

        public int BeginVisibleOffscreen => _beginVisibleOffscreen;

        public int EndVisibleOffscreen => _endVisibleOffscreen;

        public override DataCookie OutputID => DataCookie;

        public static DataCookie DataCookie => s_dataProperty;

        public override string ToString() => InvariantString.Format("{0} (BeginVisibleOffscreen={1}, EndVisibleOffscreen={2})", GetType().Name, _beginVisibleOffscreen, _endVisibleOffscreen);
    }
}
