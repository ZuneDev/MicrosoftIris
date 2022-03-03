// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.ViewItems.ContentViewItem
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Animations;
using Microsoft.Iris.Render;
using Microsoft.Iris.Session;
using Microsoft.Iris.UI;

namespace Microsoft.Iris.ViewItems
{
    internal abstract class ContentViewItem : ViewItem
    {
        protected ISprite _contents;

        protected override ISprite ContentVisual => _contents != null ? _contents : base.ContentVisual;

        protected abstract bool HasContent();

        protected override void DisposeAllContent()
        {
            base.DisposeAllContent();
            DisposeContent(true);
        }

        protected virtual void DisposeContent(bool removeFromTree)
        {
            if (_contents == null)
                return;
            if (removeFromTree)
                _contents.Remove();
            _contents.UnregisterUsage(this);
            _contents = null;
        }

        protected virtual void CreateContent()
        {
            ISprite contentVisual = ContentVisual;
            _contents = UISession.Default.RenderSession.CreateSprite(this, this);
            if (contentVisual != null)
                VisualContainer.AddChild(_contents, contentVisual, VisualOrder.Before);
            else
                VisualContainer.AddChild(_contents, null, VisualOrder.Last);
        }

        public override void OrphanVisuals(OrphanedVisualCollection orphans)
        {
            base.OrphanVisuals(orphans);
            DisposeContent(false);
        }

        protected override void OnPaint(bool visible)
        {
            base.OnPaint(visible);
            bool flag = visible && HasContent();
            if (flag && _contents == null)
            {
                CreateContent();
            }
            else
            {
                if (flag || _contents == null)
                    return;
                DisposeContent(true);
            }
        }
    }
}
