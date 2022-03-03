// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.ViewItems.Clip
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Drawing;
using Microsoft.Iris.Layout;
using Microsoft.Iris.Markup;
using Microsoft.Iris.Render;
using Microsoft.Iris.RenderAPI.Drawing;
using Microsoft.Iris.UI;
using System;

namespace Microsoft.Iris.ViewItems
{
    internal class Clip : ViewItem
    {
        private bool _showNear;
        private bool _showFar;
        private float _nearOffset;
        private float _farOffset;
        private float _nearPercent;
        private float _farPercent;
        private EdgeFade _edgefade;

        public Clip()
        {
            _showNear = true;
            _showFar = true;
            _farPercent = 1f;
            ClipMouse = true;
            _edgefade = new EdgeFade();
        }

        protected override void OnDispose()
        {
            _edgefade.Dispose();
            _edgefade = null;
            base.OnDispose();
        }

        public Orientation Orientation
        {
            get => _edgefade.Orientation;
            set
            {
                if (Orientation == value)
                    return;
                _edgefade.Orientation = value;
                OnOrientationChanged();
                FireNotification(NotificationID.Orientation);
            }
        }

        public virtual void OnOrientationChanged()
        {
        }

        public float FadeSize
        {
            get => _edgefade.FadeSize;
            set
            {
                if (FadeSize == (double)value)
                    return;
                _edgefade.FadeSize = value;
                MarkPaintInvalid();
                FireNotification(NotificationID.FadeSize);
            }
        }

        public float NearOffset
        {
            get => _nearOffset;
            set
            {
                if (_nearOffset == (double)value)
                    return;
                _nearOffset = value;
                MarkPaintInvalid();
                FireNotification(NotificationID.NearOffset);
            }
        }

        public float FarOffset
        {
            get => _farOffset;
            set
            {
                if (_farOffset == (double)value)
                    return;
                _farOffset = value;
                MarkPaintInvalid();
                FireNotification(NotificationID.FarOffset);
            }
        }

        public float NearPercent
        {
            get => _nearPercent;
            set
            {
                if (_nearPercent == (double)value)
                    return;
                _nearPercent = value;
                MarkPaintInvalid();
                FireNotification(NotificationID.NearPercent);
            }
        }

        public float FarPercent
        {
            get => _farPercent;
            set
            {
                if (_farPercent == (double)value)
                    return;
                _farPercent = value;
                MarkPaintInvalid();
                FireNotification(NotificationID.FarPercent);
            }
        }

        public bool ShowNear
        {
            get => _showNear;
            set
            {
                if (_showNear == value)
                    return;
                _showNear = value;
                MarkPaintInvalid();
                FireNotification(NotificationID.ShowNear);
            }
        }

        public bool ShowFar
        {
            get => _showFar;
            set
            {
                if (_showFar == value)
                    return;
                _showFar = value;
                MarkPaintInvalid();
                FireNotification(NotificationID.ShowFar);
            }
        }

        public Color ColorMask
        {
            get => _edgefade.ColorMask;
            set
            {
                if (!(_edgefade.ColorMask != value))
                    return;
                _edgefade.ColorMask = value;
                FireNotification(NotificationID.ColorMask);
            }
        }

        public float FadeAmount
        {
            get => _edgefade.FadeAmount;
            set
            {
                if (_edgefade.FadeAmount == (double)value)
                    return;
                _edgefade.FadeAmount = value;
                MarkPaintInvalid();
                FireNotification(NotificationID.FadeAmount);
                UpdateEdgeFade();
            }
        }

        protected override void OnPaint(bool visible)
        {
            base.OnPaint(visible);
            UpdateEdgeFade();
        }

        protected void UpdateEdgeFade()
        {
            IVisualContainer visualContainer = VisualContainer;
            if (visualContainer == null)
                return;
            float num = Orientation == Orientation.Horizontal ? VisualSize.X : VisualSize.Y;
            _edgefade.MinOffset = num * NearPercent + NearOffset;
            _edgefade.MaxOffset = num * (FarPercent - 1f) + FarOffset;
            _edgefade.ApplyGradients(visualContainer, UISession.RenderSession, _showNear, _showFar);
        }

        public override void ClipAreaOfInterest(ref AreaOfInterest interest, Size usedSize)
        {
            if (interest.Id != AreaOfInterestID.Focus && interest.Id != AreaOfInterestID.FocusOverride)
                return;
            Rectangle displayRectangle = interest.DisplayRectangle;
            int fadeSize = (int)FadeSize;
            if (Orientation == Orientation.Horizontal)
            {
                int right = displayRectangle.Right;
                if (ShowNear)
                {
                    displayRectangle.X = Math.Max(displayRectangle.X, fadeSize);
                    displayRectangle.Width = right - displayRectangle.X;
                }
                if (ShowFar)
                    displayRectangle.Width = Math.Min(right, usedSize.Width - fadeSize) - displayRectangle.X;
            }
            else
            {
                int bottom = displayRectangle.Bottom;
                if (ShowNear)
                {
                    displayRectangle.Y = Math.Max(displayRectangle.Y, fadeSize);
                    displayRectangle.Height = bottom - displayRectangle.Y;
                }
                if (ShowFar)
                    displayRectangle.Height = Math.Min(bottom, usedSize.Height - fadeSize) - displayRectangle.Y;
            }
            if (displayRectangle.Width > 0 && displayRectangle.Height > 0)
                interest.DisplayRectangle = displayRectangle;
            else
                interest.DisplayRectangle = Rectangle.Zero;
        }
    }
}
