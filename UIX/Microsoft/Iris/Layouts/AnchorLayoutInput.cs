// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Layouts.AnchorLayoutInput
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Layout;
using Microsoft.Iris.Library;

namespace Microsoft.Iris.Layouts
{
    internal class AnchorLayoutInput : ILayoutInput
    {
        private AnchorEdge _anchorLeft;
        private AnchorEdge _anchorTop;
        private AnchorEdge _anchorRight;
        private AnchorEdge _anchorBottom;
        private bool _contributesToWidthFlag = true;
        private bool _contributesToHeightFlag = true;

        public AnchorEdge Left
        {
            get => _anchorLeft;
            set => _anchorLeft = value;
        }

        public AnchorEdge Top
        {
            get => _anchorTop;
            set => _anchorTop = value;
        }

        public AnchorEdge Right
        {
            get => _anchorRight;
            set => _anchorRight = value;
        }

        public AnchorEdge Bottom
        {
            get => _anchorBottom;
            set => _anchorBottom = value;
        }

        public bool ContributesToWidth
        {
            get => _contributesToWidthFlag;
            set => _contributesToWidthFlag = value;
        }

        public bool ContributesToHeight
        {
            get => _contributesToHeightFlag;
            set => _contributesToHeightFlag = value;
        }

        DataCookie ILayoutInput.Data => Data;

        internal static DataCookie Data => AnchorLayout.InputData;

        public override string ToString() => InvariantString.Format("AnchorLayoutInput(Left={0}, Top={1}, Right={2}, Bottom={3}{4}{5})", Left, Top, Right, Bottom, ContributesToWidth ? ", ContributesToWidth=true" : string.Empty, ContributesToHeight ? ", ContributesToHeight=true" : string.Empty);
    }
}
