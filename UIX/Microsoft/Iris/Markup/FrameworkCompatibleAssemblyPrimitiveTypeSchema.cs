// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.FrameworkCompatibleAssemblyPrimitiveTypeSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;

namespace Microsoft.Iris.Markup
{
    internal class FrameworkCompatibleAssemblyPrimitiveTypeSchema :
      FrameworkCompatibleAssemblyTypeSchema
    {
        private TypeSchema _frameworkSchema;

        public FrameworkCompatibleAssemblyPrimitiveTypeSchema(TypeSchema frameworkSchema)
          : base(frameworkSchema.RuntimeType)
          => _frameworkSchema = frameworkSchema;

        public override Result TypeConverter(
          object from,
          TypeSchema fromType,
          out object instance)
        {
            return _frameworkSchema.TypeConverter(from, fromType, out instance);
        }

        public override bool SupportsTypeConversion(TypeSchema fromType) => _frameworkSchema.SupportsTypeConversion(fromType);

        public override void EncodeBinary(ByteCodeWriter writer, object instance) => _frameworkSchema.EncodeBinary(writer, instance);

        public override object DecodeBinary(ByteCodeReader reader) => _frameworkSchema.DecodeBinary(reader);

        public override bool SupportsBinaryEncoding => _frameworkSchema.SupportsBinaryEncoding;

        public override bool IsRuntimeImmutable => _frameworkSchema.IsRuntimeImmutable;

        public override object PerformOperation(object left, object right, OperationType op) => _frameworkSchema.PerformOperation(left, right, op);

        public override bool SupportsOperation(OperationType op) => _frameworkSchema.SupportsOperation(op);
    }
}
