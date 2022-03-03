// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Layouts.KeepAliveLayoutInput
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Layout;
using Microsoft.Iris.Library;

namespace Microsoft.Iris.Layouts
{
    internal class KeepAliveLayoutInput : ILayoutInput
    {
        private static readonly DataCookie s_dataProperty = DataCookie.ReserveSlot();
        private int _count;

        public static bool ShouldKeepVisible(ILayoutNode layoutNode) => layoutNode.GetLayoutInput(Data) != null;

        public int Count
        {
            get => _count;
            set => _count = value;
        }

        DataCookie ILayoutInput.Data => Data;

        public static DataCookie Data => s_dataProperty;

        public override string ToString() => InvariantString.Format("{0}", GetType().Name);
    }
}
