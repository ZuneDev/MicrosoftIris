// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIXPropertySchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

namespace Microsoft.Iris.Markup
{
    internal class UIXPropertySchema : PropertySchema
    {
        private string _name;
        private TypeSchema _propertyType;
        private TypeSchema _alternateType;
        private bool _isStatic;
        private ExpressionRestriction _expressionRestriction;
        private bool _requiredForCreation;
        private RangeValidator _rangeValidator;
        private bool _notifiesOnChange;
        private GetValueHandler _getValue;
        private SetValueHandler _setValue;

        public UIXPropertySchema(
          short ownerTypeID,
          string name,
          short typeID,
          short alternateTypeID,
          ExpressionRestriction expressionRestriction,
          bool requiredForCreation,
          RangeValidator rangeValidator,
          bool notifiesOnChange,
          GetValueHandler getValue,
          SetValueHandler setValue,
          bool isStatic)
          : base(UIXTypes.MapIDToType(ownerTypeID))
        {
            _name = name;
            _propertyType = UIXTypes.MapIDToType(typeID);
            _alternateType = UIXTypes.MapIDToType(alternateTypeID);
            _expressionRestriction = expressionRestriction;
            _requiredForCreation = requiredForCreation;
            _rangeValidator = rangeValidator;
            _notifiesOnChange = notifiesOnChange;
            _getValue = getValue;
            _setValue = setValue;
            _isStatic = isStatic;
        }

        public override string Name => _name;

        public override TypeSchema PropertyType => _propertyType;

        public override TypeSchema AlternateType => _alternateType;

        public override bool CanRead => _getValue != null;

        public override bool CanWrite => _setValue != null;

        public override bool IsStatic => _isStatic;

        public override ExpressionRestriction ExpressionRestriction => _expressionRestriction;

        public override bool RequiredForCreation => _requiredForCreation;

        public override RangeValidator RangeValidator => _rangeValidator;

        public override bool NotifiesOnChange => _notifiesOnChange;

        public override object GetValue(object instance) => _getValue(instance);

        public override void SetValue(ref object instance, object value) => _setValue(ref instance, value);
    }
}
