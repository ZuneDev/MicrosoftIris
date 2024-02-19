// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Layouts.DockLayout
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Layout;
using Microsoft.Iris.Library;
using Microsoft.Iris.Render;

namespace Microsoft.Iris.Layouts
{
    internal class DockLayout : ILayout
    {
        private DockLayoutInput _defaultLayoutInput;
        private ItemAlignment _defaultChildAlignment;
        private static int s_numberOfAdditionalItemsToRequestAtATime = 42;
        private static DockLayoutInput s_defaultLayoutInput = DockLayoutInput.Client;
        private static readonly DataCookie s_dataProperty = DataCookie.ReserveSlot();

        internal static DataCookie DockData => s_dataProperty;

        public DockLayoutInput DefaultLayoutInput
        {
            get => _defaultLayoutInput;
            set => _defaultLayoutInput = value;
        }

        public ItemAlignment DefaultChildAlignment
        {
            get => _defaultChildAlignment;
            set => _defaultChildAlignment = value;
        }

        Size ILayout.Measure(ILayoutNode layoutNode, Size constraint)
        {
            Size size1 = constraint;
            Size sz2_1 = Size.Zero;
            Size constraint1 = size1;
            bool flag = false;
            foreach (ILayoutNode layoutChild in layoutNode.LayoutChildren)
            {
                Size size2 = layoutChild.Measure(constraint1);
                sz2_1 = Size.Max(size1 - constraint1 + size2, sz2_1);
                DockLayoutInput layoutInputForNode = GetLayoutInputForNode(layoutChild);
                if (layoutInputForNode == DockLayoutInput.Left || layoutInputForNode == DockLayoutInput.Right)
                    constraint1.Width -= size2.Width;
                else if (layoutInputForNode == DockLayoutInput.Top || layoutInputForNode == DockLayoutInput.Bottom)
                    constraint1.Height -= size2.Height;
                else if (layoutInputForNode == DockLayoutInput.Client)
                    flag = true;
            }
            if (flag)
            {
                Size sz2_2 = Size.Zero;
                foreach (ILayoutNode layoutChild in layoutNode.LayoutChildren)
                {
                    if (GetLayoutInputForNode(layoutChild) == DockLayoutInput.Client)
                        sz2_2 = Size.Max(layoutChild.Measure(constraint1), sz2_2);
                }
                sz2_1 = Size.Max(size1 - constraint1 + sz2_2, sz2_1);
            }
            if (sz2_1.Width < size1.Width || sz2_1.Height < size1.Height)
                layoutNode.RequestMoreChildren(s_numberOfAdditionalItemsToRequestAtATime);
            return sz2_1;
        }

        void ILayout.Arrange(ILayoutNode layoutNode, LayoutSlot slot)
        {
            Rectangle bounds1 = new Rectangle(Point.Zero, slot.Bounds);
            bool flag = false;
            foreach (ILayoutNode layoutChild in layoutNode.LayoutChildren)
            {
                Size desiredSize = layoutChild.DesiredSize;
                Rectangle bounds2 = bounds1;
                DockLayoutInput layoutInputForNode = GetLayoutInputForNode(layoutChild);
                if (layoutInputForNode == DockLayoutInput.Left)
                {
                    bounds2.Width = desiredSize.Width;
                    bounds1.X += desiredSize.Width;
                    bounds1.Width -= desiredSize.Width;
                }
                else if (layoutInputForNode == DockLayoutInput.Right)
                {
                    bounds2.X = bounds1.Right - desiredSize.Width;
                    bounds2.Width = desiredSize.Width;
                    bounds1.Width -= desiredSize.Width;
                }
                else if (layoutInputForNode == DockLayoutInput.Top)
                {
                    bounds2.Height = desiredSize.Height;
                    bounds1.Y += desiredSize.Height;
                    bounds1.Height -= desiredSize.Height;
                }
                else if (layoutInputForNode == DockLayoutInput.Bottom)
                {
                    bounds2.Y = bounds1.Bottom - desiredSize.Height;
                    bounds2.Height = desiredSize.Height;
                    bounds1.Height -= desiredSize.Height;
                }
                else if (layoutInputForNode == DockLayoutInput.Client)
                {
                    flag = true;
                    continue;
                }
                layoutChild.Arrange(slot, bounds2);
            }
            if (!flag)
                return;
            foreach (ILayoutNode layoutChild in layoutNode.LayoutChildren)
            {
                if (GetLayoutInputForNode(layoutChild) == DockLayoutInput.Client)
                    layoutChild.Arrange(slot, bounds1);
            }
        }

        private DockLayoutInput GetLayoutInputForNode(ILayoutNode node)
        {
            if (!(node.GetLayoutInput(s_dataProperty) is DockLayoutInput dockLayoutInput))
                dockLayoutInput = _defaultLayoutInput ?? s_defaultLayoutInput;
            return dockLayoutInput;
        }

        public bool Equals(ILayout other)
        {
            if (other is not DockLayout o) return false;

            return DefaultChildAlignment == o.DefaultChildAlignment
                && DefaultLayoutInput?.ToString() == o.DefaultLayoutInput?.ToString();
        }
    }
}
