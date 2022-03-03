// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Accessibility.RootAccessibleProxy
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Accessibility;
using Microsoft.Iris.Session;
using Microsoft.Iris.UI;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Accessibility
{
    [ComVisible(true)]
    public class RootAccessibleProxy : AccessibleProxy
    {
        private IAccessible _clientBridge;

        internal RootAccessibleProxy(UIClass ui, Accessible data)
          : base(ui, data)
        {
        }

        internal override IAccessible Parent => (IAccessible)_clientBridge.accParent;

        internal override IAccessible Navigate(AccNavDirs navDir)
        {
            IAccessible accessible = null;
            switch (navDir)
            {
                case AccNavDirs.Up:
                case AccNavDirs.Down:
                case AccNavDirs.Left:
                case AccNavDirs.Right:
                case AccNavDirs.Next:
                case AccNavDirs.Previous:
                    accessible = (IAccessible)_clientBridge.accNavigate((int)navDir, 0);
                    break;
                case AccNavDirs.FirstChild:
                case AccNavDirs.LastChild:
                    accessible = base.Navigate(navDir);
                    break;
            }
            return accessible;
        }

        internal override string Name => UIApplication.ApplicationName;

        internal IAccessible ClientBridge => _clientBridge;

        internal void AttachClientBridge(IAccessible clientBridge) => _clientBridge = clientBridge;
    }
}
