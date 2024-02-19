// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.ViewItems.TextRunRenderer
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Drawing;
using Microsoft.Iris.Layout;
using Microsoft.Iris.Layouts;
using Microsoft.Iris.Markup;
using Microsoft.Iris.Render;
using Microsoft.Iris.UI;

namespace Microsoft.Iris.ViewItems
{
    internal class TextRunRenderer : ContentViewItem, ILayout
    {
        private TextRunData _data;
        private Color _color;
        private TextFlowRenderingHelper _renderingHelper;
        private PaintInvalidEventHandler _paintHandler;

        public TextRunRenderer()
        {
            Layout = this;
            _renderingHelper = new TextFlowRenderingHelper();
            _paintHandler = new PaintInvalidEventHandler(OnRunPaintInvalid);
        }

        protected override void OnDispose()
        {
            Data = null;
            base.OnDispose();
        }

        public TextRunData Data
        {
            get => _data;
            set
            {
                if (_data == value)
                    return;
                if (_data != null)
                {
                    _data.PaintInvalid -= _paintHandler;
                    _data.Run.UnregisterUsage(this);
                }
                _data = value;
                if (_data != null)
                {
                    _data.PaintInvalid += _paintHandler;
                    _data.Run.RegisterUsage(this);
                }
                FireNotification(NotificationID.Data);
            }
        }

        public Color Color
        {
            get => _color;
            set
            {
                if (!(_color != value))
                    return;
                _color = value;
                MarkPaintInvalid();
                FireNotification(NotificationID.Color);
            }
        }

        public ItemAlignment DefaultChildAlignment => ItemAlignment.Default;

        private void OnRunPaintInvalid()
        {
            _renderingHelper.InvalidateGradients();
            MarkPaintInvalid();
        }

        protected override bool HasContent() => _data != null;

        protected override void CreateContent() => base.CreateContent();

        protected override void OnEffectChanged()
        {
            if (_contents == null)
                return;
            _contents.Effect = null;
        }

        protected override void OnPaint(bool visible)
        {
            base.OnPaint(visible);
            if (_contents == null || _data == null)
                return;
            TextRun run = _data.Run;
            Text textViewItem = _data.TextViewItem;
            if (!run.Visible)
                return;
            IImage imageForRun = Text.GetImageForRun(UISession, _data.Run, _color.A != 0 ? _color : _data.Color);
            if (_contents.Effect == null)
            {
                _contents.Effect = EffectClass.CreateImageRenderEffectWithFallback(Effect, this, null);
                _contents.Effect.UnregisterUsage(this);
            }
            EffectClass.SetDefaultEffectProperty(Effect, _contents.Effect, imageForRun);
            _contents.RelativeSize = true;
            _contents.Size = Vector2.UnitVector;
        }

        Size ILayout.Measure(ILayoutNode layoutNode, Size constraint)
        {
            Size constraint1 = Size.Min(_data.Size, constraint);
            DefaultLayout.Measure(layoutNode, constraint1);
            return constraint1;
        }

        void ILayout.Arrange(ILayoutNode layoutNode, LayoutSlot slot) => DefaultLayout.Arrange(layoutNode, slot);

        public bool Equals(ILayout other)
        {
            if (other is not TextRunRenderer o) return false;

            return DefaultChildAlignment == o.DefaultChildAlignment
                && Color == o.Color
                && Data.ToString() == o.Data.ToString();
        }
    }
}
