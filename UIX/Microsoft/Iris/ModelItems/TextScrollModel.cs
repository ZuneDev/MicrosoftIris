// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.ModelItems.TextScrollModel
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;
using Microsoft.Iris.Markup;
using Microsoft.Iris.Session;
using System;

namespace Microsoft.Iris.ModelItems
{
    internal class TextScrollModel : ScrollModelBase
    {
        private ITextScrollModelCallback _handler;
        private int _scrollStep;
        private int _min;
        private int _extent;
        private int _viewExtent;
        private int _scrollAmount;
        private bool _canScrollUp;
        private bool _canScrollDown;

        public void AttachCallbacks(ITextScrollModelCallback handler)
        {
            if (_handler != null)
                ErrorManager.ReportError("TextScrollModel can't be attached to multiple TextEditingHandlers");
            _handler = handler;
        }

        public void DetachCallbacks() => _handler = null;

        public override int ScrollStep
        {
            get => _scrollStep;
            set
            {
                if (_scrollStep == value)
                    return;
                _scrollStep = value;
                FireNotification(NotificationID.ScrollStep);
            }
        }

        public override void Scroll(int amount)
        {
            if (_handler == null || AvailableScrollSpace <= 0)
                return;
            _handler.ScrollToPosition(this, Math2.Clamp(_scrollAmount + amount, 0, AvailableScrollSpace));
        }

        public override void ScrollUp()
        {
            if (_handler == null)
                return;
            if (_scrollStep == 0)
                _handler.ScrollUp(this);
            else
                Scroll(-_scrollStep);
        }

        public override void ScrollDown()
        {
            if (_handler == null)
                return;
            if (_scrollStep == 0)
                _handler.ScrollDown(this);
            else
                Scroll(_scrollStep);
        }

        public override void PageUp()
        {
            if (_handler == null)
                return;
            _handler.PageUp(this);
        }

        public override void PageDown()
        {
            if (_handler == null)
                return;
            _handler.PageDown(this);
        }

        public override void Home()
        {
            if (_handler == null || AvailableScrollSpace <= 0)
                return;
            _handler.ScrollToPosition(this, 0);
        }

        public override void End()
        {
            if (_handler == null || AvailableScrollSpace <= 0)
                return;
            _handler.ScrollToPosition(this, AvailableScrollSpace);
        }

        public override void ScrollToPosition(float scrollAmount)
        {
            if (_handler == null || AvailableScrollSpace <= 0)
                return;
            _handler.ScrollToPosition(this, (int)(AvailableScrollSpace * (double)scrollAmount));
        }

        public override bool CanScrollUp => _canScrollUp;

        public override bool CanScrollDown => _canScrollDown;

        public override float CurrentPage => _viewExtent != 0 ? (float)(_scrollAmount / (double)_viewExtent + 1.0) : 0.0f;

        public override float TotalPages => _viewExtent != 0 ? (float)(AvailableScrollSpace / (double)_viewExtent + 1.0) : 0.0f;

        public override float ViewNear => _extent != 0 ? Math.Max(_scrollAmount / (float)_extent, 0.0f) : 0.0f;

        public override float ViewFar => _extent != 0 ? Math.Min((_scrollAmount + _viewExtent) / (float)_extent, 1f) : 0.0f;

        private int AvailableScrollSpace => _extent - _viewExtent + 1;

        public void UpdateState(TextScrollModel.State newState)
        {
            if (newState.ViewStuffValid)
            {
                bool flag = false;
                if (newState.Min != _min)
                {
                    _min = newState.Min;
                    flag = true;
                }
                if (newState.Extent != _extent)
                {
                    _extent = newState.Extent;
                    flag = true;
                }
                if (newState.ViewExtent != _viewExtent)
                {
                    _viewExtent = newState.ViewExtent;
                    flag = true;
                }
                int num = newState.ScrollAmount;
                if (num + _viewExtent > _extent)
                    num = _extent - _viewExtent;
                if (num != _scrollAmount)
                {
                    _scrollAmount = num;
                    flag = true;
                }
                if (flag)
                {
                    FireNotification(NotificationID.ViewNear);
                    FireNotification(NotificationID.ViewFar);
                    FireNotification(NotificationID.CurrentPage);
                    FireNotification(NotificationID.TotalPages);
                }
            }
            if (!newState.EnabledStuffValid)
                return;
            if (newState.CanScrollUp != _canScrollUp)
            {
                _canScrollUp = newState.CanScrollUp;
                FireNotification(NotificationID.CanScrollUp);
            }
            if (newState.CanScrollDown == _canScrollDown)
                return;
            _canScrollDown = newState.CanScrollDown;
            FireNotification(NotificationID.CanScrollDown);
        }

        public struct State
        {
            private bool _viewStuffValid;
            private bool _enabledStuffValid;
            private bool _canScrollUp;
            private bool _canScrollDown;
            private int _min;
            private int _extent;
            private int _viewExtent;
            private int _scrollAmount;

            public int Min
            {
                get => _min;
                set
                {
                    _min = value;
                    _viewStuffValid = true;
                }
            }

            public int Extent
            {
                get => _extent;
                set
                {
                    _extent = value;
                    _viewStuffValid = true;
                }
            }

            public int ViewExtent
            {
                get => _viewExtent;
                set
                {
                    _viewExtent = value;
                    _viewStuffValid = true;
                }
            }

            public int ScrollAmount
            {
                get => _scrollAmount;
                set
                {
                    _scrollAmount = value;
                    _viewStuffValid = true;
                }
            }

            public bool CanScrollUp
            {
                get => _canScrollUp;
                set
                {
                    _canScrollUp = value;
                    _enabledStuffValid = true;
                }
            }

            public bool CanScrollDown
            {
                get => _canScrollDown;
                set
                {
                    _canScrollDown = value;
                    _enabledStuffValid = true;
                }
            }

            public bool EnabledStuffValid => _enabledStuffValid;

            public bool ViewStuffValid => _viewStuffValid;
        }
    }
}
