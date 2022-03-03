// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.ViewItems.IndexLayoutInput
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Layout;
using Microsoft.Iris.Library;

namespace Microsoft.Iris.ViewItems
{
    internal class IndexLayoutInput : ILayoutInput
    {
        private static readonly DataCookie s_dataProperty = DataCookie.ReserveSlot();
        private Index _index;
        private IndexType _type;

        public IndexLayoutInput(Index index, IndexType type)
        {
            _index = index;
            _type = type;
        }

        public Index Index => _index;

        public IndexType Type => _type;

        DataCookie ILayoutInput.Data => Data;

        public static DataCookie Data => s_dataProperty;

        public override string ToString() => InvariantString.Format("{0}(Index={1}, Type={2})", GetType().Name, Index, _type);
    }
}
