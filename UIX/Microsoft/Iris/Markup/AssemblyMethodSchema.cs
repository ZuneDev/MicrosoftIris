// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.AssemblyMethodSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System.Reflection;

namespace Microsoft.Iris.Markup
{
    internal class AssemblyMethodSchema : MethodSchema
    {
        private MethodInfo _methodInfo;
        private TypeSchema[] _parameterTypes;
        private TypeSchema _returnTypeSchema;
        private FastReflectionInvokeHandler _method;

        public AssemblyMethodSchema(
          AssemblyTypeSchema owner,
          MethodInfo methodInfo,
          TypeSchema[] parameterTypes)
          : base(owner)
        {
            _methodInfo = methodInfo;
            _parameterTypes = parameterTypes;
            _returnTypeSchema = AssemblyLoadResult.MapType(_methodInfo.ReturnType);
        }

        public override string Name => _methodInfo.Name;

        public override TypeSchema[] ParameterTypes => _parameterTypes;

        public override TypeSchema ReturnType => _returnTypeSchema;

        public override bool IsStatic => _methodInfo.IsStatic;

        public override object Invoke(object instance, object[] parameters)
        {
            object target = AssemblyLoadResult.UnwrapObject(instance);
            object[] paramters = AssemblyLoadResult.UnwrapObjectList(parameters);
            if (_method == null)
                _method = ReflectionHelper.CreateMethodInvoke(_methodInfo);
            return AssemblyLoadResult.WrapObject(_returnTypeSchema, _method(target, paramters));
        }
    }
}
