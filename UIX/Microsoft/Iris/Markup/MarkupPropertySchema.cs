// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.MarkupPropertySchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Markup.UIX;

namespace Microsoft.Iris.Markup
{
    public abstract class MarkupPropertySchema : PropertySchema
    {
        private string _name;
        protected TypeSchema _propertyType;
        private PropertyOverrideCriteria _overrideCriteria;
        private bool _requiredForCreation;

        public static MarkupPropertySchema Build(
          TypeSchema markupTypeBase,
          MarkupTypeSchema owner,
          string name,
          TypeSchema propertyType)
        {
            if (markupTypeBase == ClassSchema.Type || markupTypeBase == EffectSchema.Type)
                return new ClassPropertySchema(owner, name, propertyType);
            if (markupTypeBase == UISchema.Type)
                return new UIClassPropertySchema((UIClassTypeSchema)owner, name, propertyType);
            if (markupTypeBase == DataTypeSchema.Type)
                return new MarkupDataTypePropertySchema(owner, name, propertyType);
            return markupTypeBase == DataQuerySchema.Type ? new MarkupDataQueryPropertySchema(owner, name, propertyType) : null;
        }

        protected MarkupPropertySchema(MarkupTypeSchema owner, string name, TypeSchema propertyType)
          : base(owner)
        {
            _name = NotifyService.CanonicalizeString(name);
            _propertyType = propertyType;
        }

        public void SetOverrideCriteria(PropertyOverrideCriteria overrideCriteria) => _overrideCriteria = overrideCriteria;

        public void SetRequiredForCreation(bool requiredForCreation) => _requiredForCreation = requiredForCreation;

        public override string Name => _name;

        public override TypeSchema PropertyType => _propertyType;

        public override TypeSchema AlternateType => (TypeSchema)null;

        public PropertyOverrideCriteria OverrideCriteria => _overrideCriteria;

        public override bool CanRead => true;

        public override bool CanWrite => true;

        public override bool IsStatic => false;

        public override ExpressionRestriction ExpressionRestriction => ExpressionRestriction.None;

        public override bool RequiredForCreation => _requiredForCreation;

        public override RangeValidator RangeValidator => (RangeValidator)null;

        public override bool NotifiesOnChange => true;
    }
}
