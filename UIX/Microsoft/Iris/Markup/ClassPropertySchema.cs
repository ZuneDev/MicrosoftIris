// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.ClassPropertySchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.UI;

namespace Microsoft.Iris.Markup
{
    internal class ClassPropertySchema : MarkupPropertySchema
    {
        public ClassPropertySchema(MarkupTypeSchema owner, string name, TypeSchema propertyType)
          : base(owner, name, propertyType)
        {
        }

        public override bool IsStatic => ((ClassTypeSchema)Owner).IsShared;

        public override object GetValue(object instance)
        {
            ClassTypeSchema owner = (ClassTypeSchema)Owner;
            return (instance == null ? owner.SharedInstance : (Class)instance).GetProperty(Name);
        }

        public override void SetValue(ref object instance, object value)
        {
            ClassTypeSchema owner = (ClassTypeSchema)Owner;
            (instance == null ? owner.SharedInstance : (Class)instance).SetProperty(Name, value);
        }
    }
}
