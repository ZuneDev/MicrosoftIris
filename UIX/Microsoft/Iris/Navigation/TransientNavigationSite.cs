// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Navigation.TransientNavigationSite
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Render;
using System.Collections;

namespace Microsoft.Iris.Navigation
{
    internal class TransientNavigationSite : INavigationSite
    {
        private Vector3 _positionVector;
        private Vector3 _sizeVector;
        private string _descriptionName;
        private NavigationPolicies _mode;
        private ICollection _children;
        private INavigationSite _parent;

        public TransientNavigationSite(
          string descriptionName,
          INavigationSite parent,
          ICollection children,
          NavigationPolicies mode,
          Vector3 positionVector,
          Vector3 sizeVector)
        {
            _descriptionName = descriptionName;
            _parent = parent;
            _children = children;
            _mode = mode;
            _positionVector = positionVector;
            _sizeVector = sizeVector;
        }

        object INavigationSite.UniqueId => (object)null;

        INavigationSite INavigationSite.Parent => _parent;

        ICollection INavigationSite.Children => _children;

        bool INavigationSite.Visible => true;

        NavigationClass INavigationSite.Navigability => NavigationClass.None;

        NavigationPolicies INavigationSite.Mode => _mode;

        int INavigationSite.FocusOrder => int.MaxValue;

        bool INavigationSite.IsLogicalJunction => false;

        string INavigationSite.Description => _descriptionName;

        object INavigationSite.StateCache
        {
            get => (object)null;
            set
            {
            }
        }

        bool INavigationSite.ComputeBounds(
          out Vector3 positionPxlVector,
          out Vector3 sizePxlVector)
        {
            positionPxlVector = _positionVector;
            sizePxlVector = _sizeVector;
            return true;
        }

        INavigationSite INavigationSite.LookupChildById(
          object uniqueIdObject)
        {
            foreach (INavigationSite child in _children)
            {
                if (child != null && child.UniqueId != null && child.UniqueId.Equals(uniqueIdObject))
                    return child;
            }
            return null;
        }

        public override string ToString() => _descriptionName;
    }
}
