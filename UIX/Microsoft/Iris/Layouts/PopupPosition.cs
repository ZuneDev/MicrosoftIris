// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Layouts.PopupPosition
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

namespace Microsoft.Iris.Layouts
{
    internal struct PopupPosition
    {
        private InterestPoint _popup;
        private InterestPoint _target;
        private FlipDirection _flipDirection;

        public PopupPosition(InterestPoint target, InterestPoint popup, FlipDirection flipDirection)
        {
            _target = target;
            _popup = popup;
            _flipDirection = flipDirection;
        }

        public InterestPoint Target
        {
            get => _target;
            set => _target = value;
        }

        public InterestPoint Popup
        {
            get => _popup;
            set => _popup = value;
        }

        public FlipDirection Flipped
        {
            get => _flipDirection;
            set => _flipDirection = value;
        }
    }
}
