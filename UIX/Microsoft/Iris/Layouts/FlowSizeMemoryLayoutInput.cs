// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Layouts.FlowSizeMemoryLayoutInput
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Layout;
using Microsoft.Iris.Library;
using Microsoft.Iris.Render;
using System.Text;

namespace Microsoft.Iris.Layouts
{
    internal class FlowSizeMemoryLayoutInput : ILayoutInput
    {
        private static readonly DataCookie s_dataProperty = DataCookie.ReserveSlot();
        private Vector<Size> _cache;

        public FlowSizeMemoryLayoutInput() => _cache = new Vector<Size>();

        public Vector<Size> KnownSizes
        {
            get => _cache;
            set => _cache = value;
        }

        DataCookie ILayoutInput.Data => Data;

        public static DataCookie Data => s_dataProperty;

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            bool flag = true;
            int num = 0;
            foreach (Size size in _cache)
            {
                if (!flag)
                    stringBuilder.Append(", ");
                flag = false;
                stringBuilder.Append("[");
                stringBuilder.Append(num);
                stringBuilder.Append("]=");
                stringBuilder.Append(size);
                ++num;
            }
            return InvariantString.Format("{0}({1})", GetType().Name, stringBuilder);
        }
    }
}
