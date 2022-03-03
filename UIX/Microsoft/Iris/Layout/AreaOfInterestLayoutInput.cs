// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Layout.AreaOfInterestLayoutInput
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;
using Microsoft.Iris.Render;

namespace Microsoft.Iris.Layout
{
    internal class AreaOfInterestLayoutInput : ILayoutInput
    {
        private AreaOfInterestID _id;
        private Inset _margins;
        private static readonly DataCookie s_dataProperty = DataCookie.ReserveSlot();

        public AreaOfInterestLayoutInput(AreaOfInterestID idName, Inset margins)
        {
            _id = idName;
            _margins = margins;
        }

        public AreaOfInterestLayoutInput()
        {
        }

        public AreaOfInterestID Id
        {
            get => _id;
            set => _id = value;
        }

        public Inset Margins => _margins;

        DataCookie ILayoutInput.Data => Data;

        public static DataCookie Data => s_dataProperty;

        public override string ToString() => InvariantString.Format("{0}({1})", GetType().Name, _id);
    }
}
