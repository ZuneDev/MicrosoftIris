// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Layouts.ScrollIntoViewDisposition
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;

namespace Microsoft.Iris.Layouts
{
    internal class ScrollIntoViewDisposition
    {
        private int _beginPadding;
        private int _endPadding;
        private bool _locked;
        private float _lockedPosition;
        private float _lockedAlignment;
        private ContentPositioningPolicy _positioningPolicy;
        private bool _enabled;
        private RelativeEdge _relativeBeginPadding;
        private RelativeEdge _relativeEndPadding;

        public ScrollIntoViewDisposition()
          : this(0)
        {
        }

        public ScrollIntoViewDisposition(int paddingValue)
        {
            Reset();
            _beginPadding = _endPadding = paddingValue;
        }

        public void CopyFrom(ScrollIntoViewDisposition other)
        {
            _beginPadding = other._beginPadding;
            _endPadding = other._endPadding;
            _lockedPosition = other._lockedPosition;
            _lockedAlignment = other._lockedAlignment;
            _locked = other._locked;
            _enabled = other._enabled;
            _positioningPolicy = other._positioningPolicy;
            _relativeBeginPadding = other._relativeBeginPadding;
            _relativeEndPadding = other._relativeEndPadding;
        }

        public void Reset()
        {
            _beginPadding = _endPadding = 0;
            _lockedPosition = -1f;
            _lockedAlignment = 0.5f;
            _enabled = true;
            _locked = false;
            _positioningPolicy = ContentPositioningPolicy.RespectPaddingAndLocking;
            _relativeBeginPadding = RelativeEdge.Near;
            _relativeEndPadding = RelativeEdge.Far;
        }

        public bool IsDefault => _beginPadding == 0 && _endPadding == 0 && (BeginPaddingRelativeTo == RelativeEdge.Near && EndPaddingRelativeTo == RelativeEdge.Far) && !Locked;

        public int Padding
        {
            get => BeginPadding;
            set
            {
                EndPadding = value;
                BeginPadding = value;
            }
        }

        public int BeginPadding
        {
            get => _beginPadding;
            set => _beginPadding = value;
        }

        public int EndPadding
        {
            get => _endPadding;
            set => _endPadding = value;
        }

        public RelativeEdge BeginPaddingRelativeTo
        {
            get => _relativeBeginPadding;
            set => _relativeBeginPadding = value;
        }

        public RelativeEdge EndPaddingRelativeTo
        {
            get => _relativeEndPadding;
            set => _relativeEndPadding = value;
        }

        public bool Locked
        {
            get => _locked;
            set => _locked = value;
        }

        public float LockedPosition
        {
            get => _lockedPosition;
            set => _lockedPosition = value;
        }

        public float LockedAlignment
        {
            get => _lockedAlignment;
            set => _lockedAlignment = value;
        }

        public bool Enabled
        {
            get => _enabled;
            set => _enabled = value;
        }

        public ContentPositioningPolicy ContentPositioningBehavior
        {
            get => _positioningPolicy;
            set => _positioningPolicy = value;
        }

        public override string ToString()
        {
            string str1 = InvariantString.Format("{0}(", GetType().Name);
            string str2;
            if (!_enabled)
            {
                str2 = InvariantString.Format("{0}Disabled", str1);
            }
            else
            {
                str2 = InvariantString.Format("{0}(BeginPadding={1}({2}), EndPadding={3}({4})", str1, _beginPadding, _relativeBeginPadding, _endPadding, _relativeEndPadding);
                if (Locked)
                    str2 = InvariantString.Format("{0}, LockedPosition={1}, LockedAlignment={2}", str2, _lockedPosition, _lockedAlignment);
            }
            return str2 + ")";
        }
    }
}
