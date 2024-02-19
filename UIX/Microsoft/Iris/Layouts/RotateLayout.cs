// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Layouts.RotateLayout
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Drawing;
using Microsoft.Iris.Layout;
using Microsoft.Iris.Render;

namespace Microsoft.Iris.Layouts
{
    internal class RotateLayout : ILayout
    {
        private int _angleDegrees;

        public int AngleDegrees
        {
            get => _angleDegrees;
            set => _angleDegrees = value;
        }

        public ItemAlignment DefaultChildAlignment => ItemAlignment.Default;

        private bool IsVertical => AngleDegrees == 90 || AngleDegrees == 270;

        Size ILayout.Measure(ILayoutNode layoutNode, Size constraint)
        {
            if (IsVertical)
                constraint = new Size(constraint.Height, constraint.Width);
            Size size = DefaultLayout.Measure(layoutNode, constraint);
            if (IsVertical)
                size = new Size(size.Height, size.Width);
            return size;
        }

        void ILayout.Arrange(ILayoutNode layoutNode, LayoutSlot slot)
        {
            if (layoutNode.LayoutChildrenCount <= 0)
                return;
            Rectangle rectangle = new Rectangle(slot.Bounds);
            switch (AngleDegrees)
            {
                case 90:
                    Point offset1 = new Point(rectangle.Width, rectangle.Width);
                    rectangle = RotateRect90(rectangle, offset1);
                    slot.View = RotateRect90(slot.View, offset1);
                    slot.PeripheralView = RotateRect90(slot.PeripheralView, offset1);
                    break;
                case 180:
                    Point offset2 = new Point(2 * rectangle.Width, 2 * rectangle.Height);
                    rectangle = RotateRect180(rectangle, offset2);
                    slot.View = RotateRect180(slot.View, offset2);
                    slot.PeripheralView = RotateRect180(slot.PeripheralView, offset2);
                    break;
                case 270:
                    Point offset3 = new Point(rectangle.Height, rectangle.Height);
                    rectangle = RotateRect270(rectangle, offset3);
                    slot.View = RotateRect270(slot.View, offset3);
                    slot.PeripheralView = RotateRect270(slot.PeripheralView, offset3);
                    break;
            }
            Rotation rotation = Rotation.Default;
            rotation.AngleDegrees = AngleDegrees;
            foreach (ILayoutNode layoutChild in layoutNode.LayoutChildren)
                layoutChild.Arrange(slot, rectangle, Vector3.UnitVector, rotation);
        }

        private static Rectangle RotateRect90(Rectangle rect, Point offset) => new Rectangle(rect.Y + offset.X, -rect.Right + offset.Y, rect.Height, rect.Width);

        private static Rectangle RotateRect180(Rectangle rect, Point offset) => new Rectangle(-rect.Right + offset.X, -rect.Bottom + offset.Y, rect.Width, rect.Height);

        private static Rectangle RotateRect270(Rectangle rect, Point offset) => new Rectangle(-rect.Bottom + offset.X, rect.X + offset.Y, rect.Height, rect.Width);

        public bool Equals(ILayout other)
        {
            if (other is not RotateLayout o) return false;

            return DefaultChildAlignment == o.DefaultChildAlignment
                && AngleDegrees == o.AngleDegrees;
        }
    }
}
