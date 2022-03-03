// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.UI.RootUI
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Accessibility;
using Microsoft.Iris.Markup;

namespace Microsoft.Iris.UI
{
    internal class RootUI : UIClass
    {
        public RootUI(UIZone zone)
          : base(MarkupSystem.RootGlobal.RootType)
        {
            DeclareOwner(zone);
            PropagateZone(zone);
            NotifyInitialized();
        }

        protected override void OnOwnerDeclared(object owner)
        {
        }

        protected override AccessibleProxy OnCreateAccessibleProxy(
          UIClass ui,
          Accessible data)
        {
            return new RootAccessibleProxy(ui, data);
        }
    }
}
