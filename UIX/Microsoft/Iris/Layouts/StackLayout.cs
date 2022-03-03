// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Layouts.StackLayout
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Layout;
using Microsoft.Iris.Library;
using Microsoft.Iris.Render;
using System;

namespace Microsoft.Iris.Layouts
{
    internal class StackLayout : ILayout
    {
        private static StackLayoutInput s_defaultLayoutInput = new StackLayoutInput();
        private static readonly DataCookie s_dataProperty = DataCookie.ReserveSlot();

        internal static DataCookie Data => s_dataProperty;

        public ItemAlignment DefaultChildAlignment => ItemAlignment.Default;

        Size ILayout.Measure(ILayoutNode layoutNode, Size constraint)
        {
            Size size1 = new Size();
            if (layoutNode.LayoutChildrenCount > 0)
            {
                int num = 0;
                foreach (ILayoutNode layoutChild in layoutNode.LayoutChildren)
                {
                    StackLayoutInput layoutInputForNode = GetLayoutInputForNode(layoutChild);
                    Size size2 = layoutChild.Measure(constraint);
                    size1.Height = Math.Max(size1.Height, layoutChild.DesiredSize.Height);
                    if (layoutInputForNode.Priority == StackPriority.High)
                        num += size2.Width;
                    else if (layoutInputForNode.Priority == StackPriority.Medium)
                        num += layoutInputForNode.MinimumSize.Width;
                }
                size1.Width = num;
                layoutNode.MeasureData = num;
            }
            layoutNode.RequestMoreChildren(int.MaxValue);
            return size1;
        }

        void ILayout.Arrange(ILayoutNode layoutNode, LayoutSlot slot)
        {
            if (layoutNode.LayoutChildrenCount <= 0)
                return;
            int measureData = (int)layoutNode.MeasureData;
            int width1 = slot.Bounds.Width;
            int x = 0;
            int width2 = width1 - measureData;
            foreach (ILayoutNode layoutChild in layoutNode.LayoutChildren)
            {
                if (x < width1)
                {
                    int width3 = width1 - x;
                    StackLayoutInput layoutInputForNode = GetLayoutInputForNode(layoutChild);
                    if (layoutInputForNode.Priority == StackPriority.High)
                    {
                        if (layoutChild.DesiredSize.Width > width3)
                        {
                            Size constraint = new Size(width3, slot.Bounds.Height);
                            layoutChild.Measure(constraint);
                        }
                    }
                    else if (layoutInputForNode.Priority == StackPriority.Medium && layoutInputForNode.MinimumSize.Width <= width3)
                    {
                        Size constraint = new Size(layoutInputForNode.MinimumSize.Width + width2, slot.Bounds.Height);
                        layoutChild.Measure(constraint);
                        width2 -= layoutChild.DesiredSize.Width - layoutInputForNode.MinimumSize.Width;
                    }
                    else if (layoutInputForNode.Priority == StackPriority.Low && width2 > 0)
                    {
                        Size constraint = new Size(width2, slot.Bounds.Height);
                        layoutChild.Measure(constraint);
                        width2 -= layoutChild.DesiredSize.Width;
                    }
                    else
                    {
                        layoutChild.MarkHidden();
                        continue;
                    }
                    layoutChild.Arrange(slot, new Rectangle(x, 0, layoutChild.DesiredSize.Width, slot.Bounds.Height));
                    x += layoutChild.DesiredSize.Width;
                }
                else
                    layoutChild.MarkHidden();
            }
        }

        private StackLayoutInput GetLayoutInputForNode(ILayoutNode layoutNode)
        {
            if (!(layoutNode.GetLayoutInput(s_dataProperty) is StackLayoutInput stackLayoutInput))
                stackLayoutInput = s_defaultLayoutInput;
            return stackLayoutInput;
        }
    }
}
