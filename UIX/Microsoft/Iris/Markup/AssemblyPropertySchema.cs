// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.AssemblyPropertySchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System.Reflection;

namespace Microsoft.Iris.Markup
{
    internal class AssemblyPropertySchema : PropertySchema
    {
        private PropertyInfo _propertyInfo;
        private TypeSchema _propertyTypeSchema;
        private bool _isStatic;
        private FastReflectionInvokeHandler _getMethod;
        private FastReflectionInvokeHandler _setMethod;
        private object[] _setMethodParams;

        public AssemblyPropertySchema(AssemblyTypeSchema owner, PropertyInfo propertyInfo)
          : base(owner)
        {
            _propertyInfo = propertyInfo;
            _propertyTypeSchema = AssemblyLoadResult.MapType(_propertyInfo.PropertyType);
            _isStatic = (_propertyInfo.GetGetMethod() ?? _propertyInfo.GetSetMethod()).IsStatic;
        }

        public override string Name => _propertyInfo.Name;

        public override TypeSchema PropertyType => _propertyTypeSchema;

        public override TypeSchema AlternateType => (TypeSchema)null;

        public override bool CanRead => _propertyInfo.CanRead;

        public override bool CanWrite => _propertyInfo.CanWrite;

        public override bool IsStatic => _isStatic;

        public override ExpressionRestriction ExpressionRestriction => ExpressionRestriction.None;

        public override bool RequiredForCreation => false;

        public override RangeValidator RangeValidator => (RangeValidator)null;

        public override bool NotifiesOnChange
        {
            get
            {
                AssemblyTypeSchema owner = (AssemblyTypeSchema)Owner;
                return !_isStatic && owner.NotifiesOnChange;
            }
        }

        public override object GetValue(object instance)
        {
            object target = AssemblyLoadResult.UnwrapObject(instance);
            if (_getMethod == null)
                _getMethod = ReflectionHelper.CreateMethodInvoke(_propertyInfo.GetGetMethod());
            return AssemblyLoadResult.WrapObject(_propertyTypeSchema, _getMethod(target, null));
        }

        public override void SetValue(ref object instance, object value)
        {
            object target = AssemblyLoadResult.UnwrapObject(instance);
            object obj1 = AssemblyLoadResult.UnwrapObject(value);
            if (_setMethod == null)
            {
                _setMethod = ReflectionHelper.CreateMethodInvoke(_propertyInfo.GetSetMethod());
                _setMethodParams = new object[1];
            }
            _setMethodParams[0] = obj1;
            object obj2 = _setMethod(target, _setMethodParams);
            _setMethodParams[0] = null;
        }
    }
}
