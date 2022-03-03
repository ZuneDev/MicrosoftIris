// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Layouts.ScaleLayout
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Drawing;
using Microsoft.Iris.Layout;
using Microsoft.Iris.Render;
using Microsoft.Iris.RenderAPI.Drawing;
using System;

namespace Microsoft.Iris.Layouts
{
    internal class ScaleLayout : ILayout
    {
        private Vector2 _minimumScale;
        private Vector2 _maximumScale;
        private bool _maintainAspectRatio;

        public ScaleLayout()
        {
            _minimumScale = new Vector2(0.0f, 0.0f);
            _maximumScale = new Vector2(1f, 1f);
            _maintainAspectRatio = true;
        }

        public Vector2 MinimumScale
        {
            get => _minimumScale;
            set => _minimumScale = value;
        }

        public Vector2 MaximumScale
        {
            get => _maximumScale;
            set => _maximumScale = value;
        }

        public bool MaintainAspectRatio
        {
            get => _maintainAspectRatio;
            set => _maintainAspectRatio = value;
        }

        public ItemAlignment DefaultChildAlignment => ItemAlignment.Default;

        Size ILayout.Measure(ILayoutNode layoutNode, Size constraint)
        {
            Size zero = Size.Zero;
            Size constraint1 = new Size(UnscaleConstraint(constraint.Width, MinimumScale.X), UnscaleConstraint(constraint.Height, MinimumScale.Y));
            Size size = DefaultLayout.Measure(layoutNode, constraint1);
            SizeF sizeF = new SizeF(1f, 1f);
            if (!size.IsZero)
                sizeF = new SizeF(constraint.Width / (float)size.Width, constraint.Height / (float)size.Height);
            if (_maintainAspectRatio)
            {
                float num = Math.Min(sizeF.Width, sizeF.Height);
                sizeF = new SizeF(num, num);
            }
            sizeF.Width = Math.Max(sizeF.Width, MinimumScale.X);
            sizeF.Height = Math.Max(sizeF.Height, MinimumScale.Y);
            if (MaximumScale.X != 0.0)
                sizeF.Width = Math.Min(sizeF.Width, MaximumScale.X);
            if (MaximumScale.Y != 0.0)
                sizeF.Height = Math.Min(sizeF.Height, MaximumScale.Y);
            layoutNode.MeasureData = sizeF;
            size.Width = (int)Math.Round(size.Width * (double)sizeF.Width);
            size.Height = (int)Math.Round(size.Height * (double)sizeF.Height);
            return size;
        }

        private int UnscaleConstraint(int constraint, float minScale) => minScale == 0.0 ? 16777215 : Math.Min((int)Math.Round(constraint / (double)minScale), 16777215);

        void ILayout.Arrange(ILayoutNode layoutNode, LayoutSlot slot)
        {
            if (layoutNode.LayoutChildrenCount <= 0)
                return;
            SizeF measureData = (SizeF)layoutNode.MeasureData;
            Vector3 scale = new Vector3(measureData.Width, measureData.Height, 1f);
            slot.View = ScaleView(slot.View, measureData);
            slot.PeripheralView = ScaleView(slot.PeripheralView, measureData);
            foreach (ILayoutNode layoutChild in layoutNode.LayoutChildren)
                layoutChild.Arrange(slot, new Rectangle(Point.Zero, layoutChild.DesiredSize), scale, Rotation.Default);
        }

        private static Rectangle ScaleView(Rectangle view, SizeF scale)
        {
            view.X = (int)Math.Round(view.X / (double)scale.Width);
            view.Y = (int)Math.Round(view.Y / (double)scale.Height);
            view.Width = (int)Math.Round(view.Width / (double)scale.Width);
            view.Height = (int)Math.Round(view.Height / (double)scale.Height);
            return view;
        }
    }
}
