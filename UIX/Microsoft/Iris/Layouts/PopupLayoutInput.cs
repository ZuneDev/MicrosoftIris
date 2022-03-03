// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Layouts.PopupLayoutInput
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Layout;
using Microsoft.Iris.Library;
using Microsoft.Iris.Markup;
using Microsoft.Iris.Render;
using Microsoft.Iris.RenderAPI.Drawing;
using Microsoft.Iris.UI;

namespace Microsoft.Iris.Layouts
{
    internal class PopupLayoutInput : NotifyObjectBase, ILayoutInput
    {
        private ViewItem _target;
        private PlacementMode _placementMode;
        private Point _offset;
        private bool _stayInBounds;
        private bool _respectMenuDropAlignment;
        private bool _constrainToTarget;
        private RectangleF _mouseRect = RectangleF.Zero;
        private bool _flippedHorizontally;
        private bool _flippedVertically;
        private static PopupLayoutInput s_default;

        public PopupLayoutInput()
        {
            _stayInBounds = true;
            _respectMenuDropAlignment = true;
            Placement = PlacementMode.Origin;
        }

        public ViewItem PlacementTarget
        {
            get => _target;
            set => _target = value;
        }

        public PlacementMode Placement
        {
            get => _placementMode;
            set => _placementMode = value;
        }

        public Point Offset
        {
            get => _offset;
            set => _offset = value;
        }

        public bool StayInBounds
        {
            get => _stayInBounds;
            set => _stayInBounds = value;
        }

        public bool RespectMenuDropAlignment
        {
            get => _respectMenuDropAlignment;
            set => _respectMenuDropAlignment = value;
        }

        public bool ConstrainToTarget
        {
            get => _constrainToTarget;
            set => _constrainToTarget = value;
        }

        DataCookie ILayoutInput.Data => Data;

        internal static DataCookie Data => PopupLayout.DataCookie;

        internal RectangleF MouseRect
        {
            get => _mouseRect;
            set => _mouseRect = value;
        }

        public bool FlippedHorizontally
        {
            get => _flippedHorizontally;
            set
            {
                if (_flippedHorizontally == value)
                    return;
                _flippedHorizontally = value;
                FireNotification(NotificationID.FlippedHorizontally);
            }
        }

        public bool FlippedVertically
        {
            get => _flippedVertically;
            set
            {
                if (_flippedVertically == value)
                    return;
                _flippedVertically = value;
                FireNotification(NotificationID.FlippedVertically);
            }
        }

        internal bool TargetIsMouse => _placementMode != null && _placementMode.MouseTarget == MouseTarget.Fixed;

        internal bool TargetIsFollowMouse => _placementMode != null && _placementMode.MouseTarget == MouseTarget.Follow;

        internal static PopupLayoutInput Default
        {
            get
            {
                if (s_default == null)
                    s_default = new PopupLayoutInput();
                return s_default;
            }
        }
    }
}
