// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.ViewItems.Scroller
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Layout;
using Microsoft.Iris.Layouts;
using Microsoft.Iris.Markup;
using Microsoft.Iris.ModelItems;
using Microsoft.Iris.UI;

namespace Microsoft.Iris.ViewItems
{
    internal class Scroller : Clip
    {
        private ScrollModel _model;

        public Scroller()
        {
            Layout = new ScrollingLayout(Orientation, 50);
            ScrollModel = new ScrollModel();
        }

        protected override void OnDispose()
        {
            ScrollModel = null;
            base.OnDispose();
        }

        public override void OnOrientationChanged()
        {
            _model.ScrollOrientation = Orientation;
            ((ScrollingLayout)Layout).Orientation = Orientation;
            MarkLayoutInvalid();
        }

        public ScrollModel ScrollModel
        {
            get => _model;
            set
            {
                if (_model == value)
                    return;
                if (_model != null)
                    _model.DetachFromViewItem(this);
                _model = value;
                if (_model != null)
                {
                    _model.AttachToViewItem(this);
                    _model.ScrollOrientation = Orientation;
                }
                FireNotification(NotificationID.ScrollModel);
            }
        }

        public ScrollIntoViewDisposition ScrollIntoViewDisposition
        {
            get => ScrollModel.ScrollIntoViewDisposition;
            set => ScrollModel.ScrollIntoViewDisposition = value;
        }

        public int Prefetch
        {
            get => ((ScrollingLayout)Layout).Prefetch;
            set
            {
                if (Prefetch == value)
                    return;
                ((ScrollingLayout)Layout).Prefetch = value;
                MarkLayoutInvalid();
                FireNotification(NotificationID.Prefetch);
            }
        }
    }
}
