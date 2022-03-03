// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Navigation.INavigationSite
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Render;
using System.Collections;

namespace Microsoft.Iris.Navigation
{
    internal interface INavigationSite
    {
        object UniqueId { get; }

        INavigationSite Parent { get; }

        ICollection Children { get; }

        bool Visible { get; }

        NavigationClass Navigability { get; }

        NavigationPolicies Mode { get; }

        int FocusOrder { get; }

        bool IsLogicalJunction { get; }

        string Description { get; }

        object StateCache { get; set; }

        bool ComputeBounds(out Vector3 positionPxlVector, out Vector3 sizePxlVector);

        INavigationSite LookupChildById(object uniqueIdObject);
    }
}
