// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.MarkupDataTypeSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Markup.UIX;
using System;

namespace Microsoft.Iris.Markup
{
    public class MarkupDataTypeSchema : ClassTypeSchema
    {
        public MarkupDataTypeSchema(MarkupLoadResult owner, string name)
          : base(owner, name)
        {
        }

        public override object ConstructDefault() => new ProviderlessMarkupDataType(this);

        public override MarkupType MarkupType => MarkupType.DataType;

        protected override TypeSchema DefaultBase => MarkupDataTypeInstanceSchema.Type;

        public override Type RuntimeType => typeof(MarkupDataType);
    }
}
