// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Layouts.DefaultLayout
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Layout;
using Microsoft.Iris.Render;

namespace Microsoft.Iris.Layouts
{
    internal class DefaultLayout : ILayout
    {
        private static DefaultLayout s_sharedLayout = new DefaultLayout();

        private DefaultLayout()
        {
        }

        public static DefaultLayout Instance => s_sharedLayout;

        public ItemAlignment DefaultChildAlignment => ItemAlignment.Default;

        Size ILayout.Measure(ILayoutNode layoutNode, Size constraint) => Measure(layoutNode, constraint);

        public static Size Measure(ILayoutNode layoutNode, Size constraint)
        {
            Size sz1 = Size.Zero;
            foreach (ILayoutNode layoutChild in layoutNode.LayoutChildren)
            {
                Size sz2 = layoutChild.Measure(constraint);
                sz1 = Size.Max(sz1, sz2);
            }
            layoutNode.RequestMoreChildren(int.MaxValue);
            return sz1;
        }

        void ILayout.Arrange(ILayoutNode layoutNode, LayoutSlot slot) => Arrange(layoutNode, slot);

        public static void Arrange(ILayoutNode layoutNode, LayoutSlot slot)
        {
            foreach (ILayoutNode layoutChild in layoutNode.LayoutChildren)
                layoutChild.Arrange(slot);
        }
    }
}
