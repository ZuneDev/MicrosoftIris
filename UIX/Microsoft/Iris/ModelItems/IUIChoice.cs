// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.ModelItems.IUIChoice
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;
using Microsoft.Iris.Markup;
using System.Collections;

namespace Microsoft.Iris.ModelItems
{
    internal interface IUIChoice : IUIValueRange, IDisposableObject, INotifyObject
    {
        object ChosenValue { get; }

        int ChosenIndex { get; set; }

        int DefaultIndex { get; set; }

        IList Options { get; set; }

        bool HasSelection { get; }

        bool Wrap { get; set; }

        void PreviousValue(bool wrap);

        void NextValue(bool wrap);

        void DefaultValue();

        void Clear();

        bool ValidateIndex(int index, out string error);

        bool ValidateOption(object value, out int index, out string error);

        bool ValidateOptionsList(IList value, out string error);
    }
}
