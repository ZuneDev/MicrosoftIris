// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.ViewItems.UIPropertyRecord
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Markup;

namespace Microsoft.Iris.ViewItems
{
    internal class UIPropertyRecord
    {
        public string Name;
        public object Value;
        public PropertySchema Schema;

        public static void AddToList(Vector<UIPropertyRecord> list, string name, object value) => list.Add(new UIPropertyRecord()
        {
            Name = name,
            Value = value
        });

        public static UIPropertyRecord FindInList(
          Vector<UIPropertyRecord> list,
          string name)
        {
            foreach (UIPropertyRecord uiPropertyRecord in list)
            {
                if (uiPropertyRecord.Name == name)
                    return uiPropertyRecord;
            }
            return null;
        }

        public static bool IsInList(Vector<UIPropertyRecord> list, string name) => FindInList(list, name) != null;
    }
}
