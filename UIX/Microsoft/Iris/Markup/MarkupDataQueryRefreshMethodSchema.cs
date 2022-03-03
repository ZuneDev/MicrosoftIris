// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.MarkupDataQueryRefreshMethodSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Markup.UIX;

namespace Microsoft.Iris.Markup
{
    internal class MarkupDataQueryRefreshMethodSchema : MethodSchema
    {
        public MarkupDataQueryRefreshMethodSchema(MarkupDataQuerySchema owner)
          : base(owner)
        {
        }

        public override string Name => "Refresh";

        public override TypeSchema[] ParameterTypes => TypeSchema.EmptyList;

        public override TypeSchema ReturnType => VoidSchema.Type;

        public override bool IsStatic => false;

        public override object Invoke(object instance, object[] parameters)
        {
            ((MarkupDataQuery)instance).Refresh();
            return null;
        }
    }
}
