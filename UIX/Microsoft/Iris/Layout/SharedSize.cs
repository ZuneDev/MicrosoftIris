// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Layout.SharedSize
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;
using Microsoft.Iris.Markup;
using Microsoft.Iris.Render;
using Microsoft.Iris.Session;
using Microsoft.Iris.UI;
using System;

namespace Microsoft.Iris.Layout
{
    internal class SharedSize : NotifyObjectBase
    {
        private Size _minimumSize;
        private Size _maximumSize;
        private Size _desiredSize;
        private bool _accumulatingSize;
        private bool _applyPending;
        private Vector<ViewItem> _dependents;

        public SharedSize()
        {
            _minimumSize = Size.Zero;
            _maximumSize = Size.Zero;
            _desiredSize = Size.Zero;
            _accumulatingSize = true;
        }

        public Size MaximumSize
        {
            get => _maximumSize;
            set
            {
                if (!(_maximumSize != value))
                    return;
                Size size1 = Size;
                _maximumSize = value;
                Size size2 = Size;
                FireNotification(NotificationID.MaximumSize);
                if (!(size1 != size2))
                    return;
                FireNotification(NotificationID.Size);
                InvalidateDependents(true);
            }
        }

        public Size MinimumSize
        {
            get => _minimumSize;
            set
            {
                if (!(_minimumSize != value))
                    return;
                Size size1 = Size;
                _minimumSize = value;
                Size size2 = Size;
                FireNotification(NotificationID.MinimumSize);
                if (!(size1 != size2))
                    return;
                FireNotification(NotificationID.Size);
                InvalidateDependents(true);
            }
        }

        public Size Size
        {
            get
            {
                Size size = Size.Max(_desiredSize, MinimumSize);
                Size maximumSize = MaximumSize;
                if (maximumSize.Width > 0)
                    size.Width = Math.Min(size.Width, maximumSize.Width);
                if (maximumSize.Height > 0)
                    size.Height = Math.Min(size.Height, maximumSize.Height);
                return size;
            }
            set => SetSize(value, true);
        }

        public void AutoSize()
        {
            SetSize(Size.Zero, false);
            InvalidateDependents(true);
        }

        private void SetSize(Size value, bool stopAccumulating)
        {
            if (_desiredSize != value)
            {
                Size size1 = Size;
                _desiredSize = value;
                Size size2 = Size;
                if (size1 != size2)
                {
                    FireNotification(NotificationID.Size);
                    if (stopAccumulating)
                        InvalidateDependents(true);
                }
            }
            _accumulatingSize = !stopAccumulating;
        }

        public void Register(ViewItem viewitem)
        {
            if (_dependents == null)
                _dependents = new Vector<ViewItem>();
            _dependents.Add(viewitem);
        }

        public void Unregister(ViewItem viewitem) => _dependents.Remove(viewitem);

        public void AdjustConstraint(ref Size constraint, ref Size minSize, SharedSizePolicy policy)
        {
            Size size1 = Size;
            Size maximumSize = MaximumSize;
            Size size2 = constraint;
            Size size3 = size1;
            Size size4 = minSize;
            if ((policy & SharedSizePolicy.SharesWidth) != 0)
            {
                if (!_accumulatingSize)
                {
                    int num = Math.Max(size4.Width, Math.Min(size1.Width, size2.Width));
                    if (num >= MinimumSize.Width && (num <= maximumSize.Width || maximumSize.Width == 0))
                    {
                        size4.Width = num;
                        size2.Width = num;
                    }
                    else
                    {
                        size2.Width = 0;
                        size4.Width = 1;
                    }
                }
                else
                {
                    if (maximumSize.Width != 0)
                        size2.Width = Math.Min(maximumSize.Width, size2.Width);
                    size4.Width = Math.Max(size3.Width, size4.Width);
                }
            }
            if ((policy & SharedSizePolicy.SharesHeight) != 0)
            {
                if (!_accumulatingSize)
                {
                    int num = Math.Max(size4.Height, Math.Min(size1.Height, size2.Height));
                    if (num >= MinimumSize.Height && (num <= maximumSize.Height || maximumSize.Height == 0))
                    {
                        size4.Height = num;
                        size2.Height = num;
                    }
                    else
                    {
                        size2.Height = 0;
                        size4.Height = 1;
                    }
                }
                else
                {
                    if (maximumSize.Height != 0)
                        size2.Height = Math.Min(maximumSize.Height, size2.Height);
                    size4.Height = Math.Max(size3.Height, size4.Height);
                }
            }
            constraint = size2;
            minSize = size4;
        }

        public void AccumulateSize(Size size, SharedSizePolicy policy)
        {
            if (!_accumulatingSize)
                return;
            Size size1 = Size;
            if ((policy & SharedSizePolicy.ContributesToWidth) != 0 && size.Width > size1.Width)
                size1.Width = size.Width;
            if ((policy & SharedSizePolicy.ContributesToHeight) != 0 && size.Height > size1.Height)
                size1.Height = size.Height;
            SetSize(size1, false);
            EnsureApplySize();
        }

        private void EnsureApplySize()
        {
            if (_applyPending)
                return;
            _applyPending = true;
            DeferredCall.Post(DispatchPriority.LayoutPass2, new SimpleCallback(ApplySize));
        }

        private void ApplySize()
        {
            _accumulatingSize = false;
            _applyPending = false;
            InvalidateDependents(false);
        }

        private void InvalidateDependents(bool forceInvalid)
        {
            if (_dependents == null)
                return;
            Size size1 = Size;
            foreach (ViewItem dependent in _dependents)
            {
                if (forceInvalid)
                {
                    dependent.MarkLayoutInvalid();
                }
                else
                {
                    SharedSizePolicy sharedSizePolicy = dependent.SharedSizePolicy;
                    Size size2 = ((ILayoutNode)dependent).DesiredSize - dependent.Margins.Size;
                    if (size1.Width != size2.Width && (sharedSizePolicy & SharedSizePolicy.SharesWidth) != 0 || size1.Height != size2.Height && (sharedSizePolicy & SharedSizePolicy.SharesHeight) != 0)
                        dependent.MarkLayoutInvalid();
                }
            }
        }
    }
}
