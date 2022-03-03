// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.ModelItems.CaretInfo
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;
using Microsoft.Iris.Markup;
using Microsoft.Iris.OS;
using Microsoft.Iris.Render;

namespace Microsoft.Iris.ModelItems
{
    internal class CaretInfo : NotifyObjectBase
    {
        private Point _position;
        private Size _suggestedSize;
        private int _idealWidth;
        private bool _visible;
        private bool _ignoreIdealWidth;

        public CaretInfo() => _idealWidth = GetSystemCaretWidth();

        public int IdealWidth
        {
            get => _idealWidth;
            set
            {
                if (_idealWidth == value)
                    return;
                _idealWidth = value;
                FireThreadSafeNotification(NotificationID.IdealWidth);
                UpdateSuggestedSize(SuggestedSize);
            }
        }

        public Point Position => _position;

        public Size SuggestedSize => _suggestedSize;

        public bool Visible => _visible && _position.X >= 0 && _position.Y >= 0;

        internal bool IgnoreIdealWidth
        {
            get => _ignoreIdealWidth;
            set => _ignoreIdealWidth = value;
        }

        public void CreateCaret(Size size) => UpdateSuggestedSize(size);

        public void SetVisible(bool visible)
        {
            if (_visible == visible)
                return;
            _visible = visible;
            FireThreadSafeNotification(NotificationID.Visible);
        }

        public void SetCaretPosition(Point position)
        {
            if (_position.X == position.X && _position.Y == position.Y)
                return;
            bool visible = Visible;
            _position = position;
            FireThreadSafeNotification(NotificationID.Position);
            if (Visible == visible)
                return;
            FireThreadSafeNotification(NotificationID.Visible);
        }

        private void UpdateSuggestedSize(Size newSize)
        {
            if (newSize.Width == 1 && _idealWidth != 0 && !IgnoreIdealWidth)
                newSize.Width = _idealWidth;
            if (!(newSize != _suggestedSize))
                return;
            _suggestedSize = newSize;
            FireThreadSafeNotification(NotificationID.SuggestedSize);
        }

        private int GetSystemCaretWidth() => Win32Api.GetCaretWidth();

        public float BlinkTime
        {
            get
            {
                int num = Win32Api.GetCaretBlinkTime();
                if (num < 0)
                    num = 0;
                return num / 1000f;
            }
        }
    }
}
