// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.AssemblyConstructorSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System.Reflection;

namespace Microsoft.Iris.Markup
{
    internal class AssemblyConstructorSchema : ConstructorSchema
    {
        private ConstructorInfo _constructorInfo;
        private TypeSchema[] _parameterTypes;
        private FastReflectionInvokeHandler _constructor;

        public AssemblyConstructorSchema(
          AssemblyTypeSchema owner,
          ConstructorInfo constructorInfo,
          TypeSchema[] parameterTypes)
          : base(owner)
        {
            _constructorInfo = constructorInfo;
            _parameterTypes = parameterTypes;
        }

        public override TypeSchema[] ParameterTypes => _parameterTypes;

        public override object Construct(object[] parameters)
        {
            object[] paramters = AssemblyLoadResult.UnwrapObjectList(parameters);
            AssemblyTypeSchema owner = (AssemblyTypeSchema)Owner;
            if (_constructor == null)
                _constructor = ReflectionHelper.CreateMethodInvoke(_constructorInfo);
            object instance = _constructor(null, paramters);
            return AssemblyLoadResult.WrapObject(owner, instance);
        }
    }
}
