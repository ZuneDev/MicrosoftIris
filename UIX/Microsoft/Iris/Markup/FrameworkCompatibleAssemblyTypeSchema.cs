// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.FrameworkCompatibleAssemblyTypeSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System;

namespace Microsoft.Iris.Markup
{
    internal class FrameworkCompatibleAssemblyTypeSchema : AssemblyTypeSchema
    {
        private Type _frameworkType;
        private Type _constructDefaultType;

        public FrameworkCompatibleAssemblyTypeSchema(Type assemblyType)
          : this(assemblyType, assemblyType, null, null)
        {
        }

        public FrameworkCompatibleAssemblyTypeSchema(Type assemblyType, Type frameworkType)
          : this(assemblyType, frameworkType, null, null)
        {
        }

        public FrameworkCompatibleAssemblyTypeSchema(
          Type assemblyType,
          Type frameworkType,
          Type constructDefaultType)
          : this(assemblyType, frameworkType, constructDefaultType, null)
        {
        }

        public FrameworkCompatibleAssemblyTypeSchema(
          Type assemblyType,
          Type frameworkType,
          Type constructDefaultType,
          TypeSchema baseType)
          : base(assemblyType, baseType)
        {
            _frameworkType = frameworkType;
            _constructDefaultType = constructDefaultType;
        }

        public override Type RuntimeType => _frameworkType;

        public override bool HasDefaultConstructor => base.HasDefaultConstructor || _constructDefaultType != null;

        public override object ConstructDefault() => base.HasDefaultConstructor ? base.ConstructDefault() : AssemblyLoadResult.WrapObject(this, Activator.CreateInstance(_constructDefaultType));
    }
}
