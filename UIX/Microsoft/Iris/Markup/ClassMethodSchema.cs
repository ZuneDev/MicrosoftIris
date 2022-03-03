// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.ClassMethodSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

namespace Microsoft.Iris.Markup
{
    internal class ClassMethodSchema : MarkupMethodSchema
    {
        public ClassMethodSchema(
          ClassTypeSchema owner,
          string name,
          TypeSchema returnType,
          TypeSchema[] parameterTypes,
          string[] parameterNames)
          : base(owner, name, returnType, parameterTypes, parameterNames)
        {
        }

        protected override IMarkupTypeBase GetMarkupTypeBase(object instance) => instance == null ? ((ClassTypeSchema)Owner).SharedInstance : (IMarkupTypeBase)instance;

        public override bool IsStatic => ((ClassTypeSchema)Owner).IsShared;
    }
}
