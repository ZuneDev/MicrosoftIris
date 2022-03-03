// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Layouts.ScrollingLayoutInput
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Layout;
using Microsoft.Iris.Library;

namespace Microsoft.Iris.Layouts
{
    internal class ScrollingLayoutInput : ILayoutInput
    {
        private static readonly DataCookie s_dataProperty = DataCookie.ReserveSlot();
        private int _scrollAmount;
        private int _pendingScrollAmount;
        private int _pendingPageCommands;
        private float _pendingScrollPosition;
        private bool _havePendingScrollPosition;
        private bool _enabled;
        private float _pageStep;
        private ScrollIntoViewDisposition _scrollIntoView;
        private ScrollIntoViewDisposition _secondaryScrollIntoView;

        public ScrollingLayoutInput()
        {
            _pageStep = 1f;
            _enabled = true;
            _scrollIntoView = new ScrollIntoViewDisposition();
        }

        public bool Enabled
        {
            get => _enabled;
            set
            {
                if (value == _enabled)
                    return;
                _enabled = value;
                if (!value)
                    return;
                _scrollAmount = 0;
                _pendingScrollAmount = 0;
                _pendingPageCommands = 0;
                _pendingScrollPosition = 0.0f;
                _havePendingScrollPosition = false;
            }
        }

        public void Scroll(int amount) => _scrollAmount += amount;

        public void PageUp() => --_pendingPageCommands;

        public void PageDown() => ++_pendingPageCommands;

        public void Home() => ScrollToPosition(0.0f);

        public void End() => ScrollToPosition(1f);

        public void ScrollToPosition(float position)
        {
            _havePendingScrollPosition = true;
            _pendingScrollPosition = position;
        }

        public bool GetPendingScrollPosition(out float position)
        {
            if (!_havePendingScrollPosition)
            {
                position = 0.0f;
                return false;
            }
            position = _pendingScrollPosition;
            return true;
        }

        public bool GetPendingPageRequests(out int amount)
        {
            amount = _pendingPageCommands;
            return _pendingPageCommands != 0;
        }

        internal int ScrollAmount => _scrollAmount;

        internal void SetScrollAmount(int scrollAmount) => _pendingScrollAmount = scrollAmount;

        internal void OnLayoutComplete()
        {
            SecondaryScrollIntoViewDisposition = null;
            _havePendingScrollPosition = false;
            _pendingPageCommands = 0;
            _scrollAmount = _pendingScrollAmount;
        }

        internal float PageStep
        {
            get => _pageStep;
            set => _pageStep = value;
        }

        public ScrollIntoViewDisposition ScrollIntoViewDisposition
        {
            get => _scrollIntoView;
            set => _scrollIntoView = value;
        }

        public ScrollIntoViewDisposition SecondaryScrollIntoViewDisposition
        {
            get => _secondaryScrollIntoView;
            set => _secondaryScrollIntoView = value;
        }

        DataCookie ILayoutInput.Data => Data;

        public static DataCookie Data => s_dataProperty;

        public override string ToString() => InvariantString.Format("{0}(ScrollAmount={1}, PageAmount={2}, PageStep={3}, Disposition=({4}))", GetType().Name, _pendingScrollAmount, _pendingPageCommands, _pageStep, _scrollIntoView);
    }
}
