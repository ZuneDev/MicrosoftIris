// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.ModelItems.IUIList
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System.Collections;

namespace Microsoft.Iris.ModelItems
{
    internal interface IUIList : IList, ICollection, IEnumerable
    {
        bool CanSearch { get; }

        int SearchForString(string searchString);

        void Move(int oldIndex, int newIndex);
    }
}
