// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.ViewItems.RootViewItem
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Navigation;
using Microsoft.Iris.Render;
using Microsoft.Iris.Session;
using Microsoft.Iris.UI;

namespace Microsoft.Iris.ViewItems
{
    internal class RootViewItem : Host
    {
        private bool _rootVisibleFlag;

        public RootViewItem(UIZone zone, UIClass rootUI, Form form)
        {
            DeclareOwner(rootUI);
            PropagateZone(zone);
            IVisualContainer rootVisual = form.RootVisual;
            rootVisual.MouseOptions = MouseOptions.Traversable;
            VisualContainer = rootVisual;
        }

        internal void ApplyRootLayoutOutput(bool parentFullyVisibleFlag, out bool visibilityChangeFlag)
        {
            Rectangle layoutBounds = LayoutBounds;
            VisualPosition = new Vector3(layoutBounds.Left, layoutBounds.Top, 0.0f);
            VisualSize = new Vector2(layoutBounds.Width, layoutBounds.Height);
            VisualScale = LayoutScale;
            if (LayoutVisible)
                parentFullyVisibleFlag = false;
            visibilityChangeFlag = _rootVisibleFlag != parentFullyVisibleFlag;
            if (!visibilityChangeFlag)
                return;
            _rootVisibleFlag = parentFullyVisibleFlag;
        }

        protected override NavigationPolicies ForcedNavigationFlags => NavigationPolicies.RememberFocus | NavigationPolicies.WrapTabOrder;
    }
}
