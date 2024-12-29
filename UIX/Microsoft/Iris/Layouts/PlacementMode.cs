// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Layouts.PlacementMode
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

namespace Microsoft.Iris.Layouts
{
    internal class PlacementMode : IHasCanonicalInstances
    {
        private PopupPosition[] _popupPositions;
        private MouseTarget _mouseTarget;
        private bool _usesTargetSize;
        private static PlacementMode s_origin;
        private static PlacementMode s_left;
        private static PlacementMode s_right;
        private static PlacementMode s_top;
        private static PlacementMode s_bottom;
        private static PlacementMode s_center;
        private static PlacementMode s_mouseOrigin;
        private static PlacementMode s_mouseBottom;
        private static PlacementMode s_followMouseOrigin;
        private static PlacementMode s_followMouseBottom;

        public PopupPosition[] PopupPositions
        {
            get => _popupPositions;
            set
            {
                _popupPositions = value;
                _usesTargetSize = false;
                if (_popupPositions == null)
                    return;
                for (int index = 0; index < _popupPositions.Length; ++index)
                {
                    if (_popupPositions[index].Target != InterestPoint.TopLeft)
                    {
                        _usesTargetSize = true;
                        break;
                    }
                }
            }
        }

        public MouseTarget MouseTarget
        {
            get => _mouseTarget;
            set => _mouseTarget = value;
        }

        internal bool UsesTargetSize => _usesTargetSize;

        public override string ToString() => GetCanonicalName() ?? base.ToString();

        public string GetCanonicalName()
        {
            if (this == s_origin)
                return "Origin";
            if (this == s_bottom)
                return "Bottom";
            if (this == s_top)
                return "Top";
            if (this == s_left)
                return "Left";
            if (this == s_right)
                return "Right";
            if (this == s_center)
                return "Center";
            if (this == s_mouseOrigin)
                return "MouseOrigin";
            if (this == s_mouseBottom)
                return "MouseBottom";
            if (this == s_followMouseOrigin)
                return "FollowMouseOrigin";
            if (this == s_followMouseBottom)
                return "FollowMouseBottom";

            return null;
        }

        public static PlacementMode Origin
        {
            get
            {
                if (s_origin == null)
                {
                    s_origin = new PlacementMode();
                    s_origin.PopupPositions = new PopupPosition[1]
                    {
            new PopupPosition(InterestPoint.TopLeft, InterestPoint.TopLeft, FlipDirection.None)
                    };
                }
                return s_origin;
            }
        }

        public static PlacementMode Bottom
        {
            get
            {
                if (s_bottom == null)
                {
                    s_bottom = new PlacementMode();
                    s_bottom.PopupPositions = new PopupPosition[2]
                    {
            new PopupPosition(InterestPoint.BottomLeft, InterestPoint.TopLeft, FlipDirection.None),
            new PopupPosition(InterestPoint.TopLeft, InterestPoint.BottomLeft, FlipDirection.Vertical)
                    };
                }
                return s_bottom;
            }
        }

        public static PlacementMode Center
        {
            get
            {
                if (s_center == null)
                {
                    s_center = new PlacementMode();
                    s_center.PopupPositions = new PopupPosition[1]
                    {
            new PopupPosition(InterestPoint.Center, InterestPoint.Center, FlipDirection.None)
                    };
                }
                return s_center;
            }
        }

        public static PlacementMode Right
        {
            get
            {
                if (s_right == null)
                {
                    s_right = new PlacementMode();
                    s_right.PopupPositions = new PopupPosition[4]
                    {
            new PopupPosition(InterestPoint.TopRight, InterestPoint.TopLeft, FlipDirection.None),
            new PopupPosition(InterestPoint.BottomRight, InterestPoint.BottomLeft, FlipDirection.Vertical),
            new PopupPosition(InterestPoint.TopLeft, InterestPoint.TopRight, FlipDirection.Horizontal),
            new PopupPosition(InterestPoint.BottomLeft, InterestPoint.BottomRight, FlipDirection.Both)
                    };
                }
                return s_right;
            }
        }

        public static PlacementMode Left
        {
            get
            {
                if (s_left == null)
                {
                    s_left = new PlacementMode();
                    s_left.PopupPositions = new PopupPosition[4]
                    {
            new PopupPosition(InterestPoint.TopLeft, InterestPoint.TopRight, FlipDirection.None),
            new PopupPosition(InterestPoint.BottomLeft, InterestPoint.BottomRight, FlipDirection.Vertical),
            new PopupPosition(InterestPoint.TopRight, InterestPoint.TopLeft, FlipDirection.Horizontal),
            new PopupPosition(InterestPoint.BottomRight, InterestPoint.BottomLeft, FlipDirection.Both)
                    };
                }
                return s_left;
            }
        }

        public static PlacementMode Top
        {
            get
            {
                if (s_top == null)
                {
                    s_top = new PlacementMode();
                    s_top.PopupPositions = new PopupPosition[2]
                    {
            new PopupPosition(InterestPoint.TopLeft, InterestPoint.BottomLeft, FlipDirection.None),
            new PopupPosition(InterestPoint.BottomLeft, InterestPoint.TopLeft, FlipDirection.Vertical)
                    };
                }
                return s_top;
            }
        }

        public static PlacementMode MouseOrigin
        {
            get
            {
                if (s_mouseOrigin == null)
                {
                    s_mouseOrigin = new PlacementMode();
                    s_mouseOrigin.MouseTarget = MouseTarget.Fixed;
                    s_mouseOrigin.PopupPositions = new PopupPosition[4]
                    {
            new PopupPosition(InterestPoint.TopLeft, InterestPoint.TopLeft, FlipDirection.None),
            new PopupPosition(InterestPoint.TopLeft, InterestPoint.TopRight, FlipDirection.Horizontal),
            new PopupPosition(InterestPoint.TopLeft, InterestPoint.BottomLeft, FlipDirection.Vertical),
            new PopupPosition(InterestPoint.TopLeft, InterestPoint.BottomRight, FlipDirection.Both)
                    };
                }
                return s_mouseOrigin;
            }
        }

        public static PlacementMode MouseBottom
        {
            get
            {
                if (s_mouseBottom == null)
                {
                    s_mouseBottom = new PlacementMode();
                    s_mouseBottom.MouseTarget = MouseTarget.Fixed;
                    s_mouseBottom.PopupPositions = Bottom.PopupPositions;
                }
                return s_mouseBottom;
            }
        }

        public static PlacementMode FollowMouseOrigin
        {
            get
            {
                if (s_followMouseOrigin == null)
                {
                    s_followMouseOrigin = new PlacementMode();
                    s_followMouseOrigin.MouseTarget = MouseTarget.Follow;
                    s_followMouseOrigin.PopupPositions = MouseOrigin.PopupPositions;
                }
                return s_followMouseOrigin;
            }
        }

        public static PlacementMode FollowMouseBottom
        {
            get
            {
                if (s_followMouseBottom == null)
                {
                    s_followMouseBottom = new PlacementMode();
                    s_followMouseBottom.MouseTarget = MouseTarget.Follow;
                    s_followMouseBottom.PopupPositions = MouseBottom.PopupPositions;
                }
                return s_followMouseBottom;
            }
        }
    }
}
