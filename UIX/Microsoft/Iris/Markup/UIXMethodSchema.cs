// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIXMethodSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

namespace Microsoft.Iris.Markup
{
    internal class UIXMethodSchema : MethodSchema
    {
        private string _name;
        private TypeSchema[] _parameterTypes;
        private TypeSchema _returnType;
        private bool _isStatic;
        private InvokeHandler _invoke;

        public UIXMethodSchema(
          short ownerTypeID,
          string name,
          short[] parameterTypeIDs,
          short returnTypeID,
          InvokeHandler invoke,
          bool isStatic)
          : base(UIXTypes.MapIDToType(ownerTypeID))
        {
            _name = name;
            _parameterTypes = UIXTypes.MapIDsToTypes(parameterTypeIDs);
            _returnType = UIXTypes.MapIDToType(returnTypeID);
            _invoke = invoke;
            _isStatic = isStatic;
        }

        public override string Name => _name;

        public override TypeSchema[] ParameterTypes => _parameterTypes;

        public override TypeSchema ReturnType => _returnType;

        public override bool IsStatic => _isStatic;

        public override object Invoke(object instance, object[] parameters) => _invoke(instance, parameters);
    }
}
