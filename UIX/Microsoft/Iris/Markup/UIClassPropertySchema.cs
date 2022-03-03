// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIClassPropertySchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.ViewItems;

namespace Microsoft.Iris.Markup
{
    internal class UIClassPropertySchema : MarkupPropertySchema
    {
        public UIClassPropertySchema(UIClassTypeSchema owner, string name, TypeSchema propertyType)
          : base(owner, name, propertyType)
        {
        }

        public override object GetValue(object instance) => ((Host)instance).GetChildUIProperty(Name);

        public override void SetValue(ref object instance, object value) => ((Host)instance).SetChildUIProperty(Name, value);
    }
}
