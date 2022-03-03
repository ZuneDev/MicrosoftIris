// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Layout.ItemAlignment
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Render;

namespace Microsoft.Iris.Layout
{
    internal struct ItemAlignment
    {
        private Alignment _horizontal;
        private Alignment _vertical;
        internal static readonly ItemAlignment Default = new ItemAlignment();

        public ItemAlignment(Alignment horizontal, Alignment vertical)
        {
            _horizontal = horizontal;
            _vertical = vertical;
        }

        public Alignment Horizontal
        {
            get => _horizontal;
            set => _horizontal = value;
        }

        public Alignment Vertical
        {
            get => _vertical;
            set => _vertical = value;
        }

        public Alignment GetAlignment(Orientation orientation) => orientation != Orientation.Horizontal ? Vertical : Horizontal;

        public override bool Equals(object obj) => obj is ItemAlignment itemAlignment && this == itemAlignment;

        public override int GetHashCode() => (int)((Alignment)((int)Horizontal << 16) ^ Vertical);

        public static bool operator ==(ItemAlignment left, ItemAlignment right) => left.Horizontal == right.Horizontal && left.Vertical == right.Vertical;

        public static bool operator !=(ItemAlignment left, ItemAlignment right) => !(left == right);

        public static ItemAlignment Merge(ItemAlignment alignment, ItemAlignment fallback)
        {
            if (alignment.Horizontal == Alignment.Unspecified)
                alignment.Horizontal = fallback.Horizontal;
            if (alignment.Vertical == Alignment.Unspecified)
                alignment.Vertical = fallback.Vertical;
            return alignment;
        }
    }
}
