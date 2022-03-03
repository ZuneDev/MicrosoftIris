// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Layout.LayoutSlot
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Render;
using System.Text;

namespace Microsoft.Iris.Layout
{
    internal struct LayoutSlot
    {
        private Size _bounds;
        private Point _offset;
        private Rectangle _viewBounds;
        private Rectangle _peripheralViewBounds;

        public LayoutSlot(Size extent)
          : this(extent, Point.Zero, new Rectangle(extent), new Rectangle(extent))
        {
        }

        public LayoutSlot(
          Size extent,
          Point offset,
          Rectangle viewBounds,
          Rectangle viewPeripheralBounds)
        {
            _bounds = extent;
            _offset = offset;
            _viewBounds = viewBounds;
            _peripheralViewBounds = viewPeripheralBounds;
        }

        public Size Bounds
        {
            get => _bounds;
            set => _bounds = value;
        }

        public Point Offset
        {
            get => _offset;
            set => _offset = value;
        }

        public Rectangle View
        {
            get => _viewBounds;
            set => _viewBounds = value;
        }

        public Rectangle PeripheralView
        {
            get => _peripheralViewBounds;
            set => _peripheralViewBounds = value;
        }

        public void Deflate(Inset inset)
        {
            int num1 = inset.Left + inset.Right;
            int num2 = inset.Top + inset.Bottom;
            _bounds.Width -= num1;
            if (_bounds.Width < 0)
                _bounds.Width = 0;
            _bounds.Height -= num2;
            if (_bounds.Height < 0)
                _bounds.Height = 0;
            _viewBounds.X -= inset.Left;
            _viewBounds.Y -= inset.Top;
            _peripheralViewBounds.X -= inset.Left;
            _peripheralViewBounds.Y -= inset.Top;
        }

        public static bool operator ==(LayoutSlot left, LayoutSlot right) => left._bounds == right._bounds && left._offset == right._offset && left._viewBounds == right._viewBounds && left._peripheralViewBounds == right._peripheralViewBounds;

        public static bool operator !=(LayoutSlot left, LayoutSlot right) => !(left == right);

        public override bool Equals(object obj) => obj is LayoutSlot layoutSlot && this == layoutSlot;

        public override int GetHashCode() => _bounds.GetHashCode() ^ _offset.GetHashCode() ^ _viewBounds.GetHashCode() ^ _peripheralViewBounds.GetHashCode();

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(GetType().Name);
            stringBuilder.Append("(");
            stringBuilder.Append("Bounds=");
            stringBuilder.Append(_bounds);
            stringBuilder.Append(", Offset=");
            stringBuilder.Append(_offset);
            stringBuilder.Append(", View=");
            stringBuilder.Append(_viewBounds);
            if (_peripheralViewBounds != _viewBounds)
            {
                stringBuilder.Append(", Peripheral=");
                stringBuilder.Append(_peripheralViewBounds);
            }
            stringBuilder.Append(")");
            return stringBuilder.ToString();
        }
    }
}
