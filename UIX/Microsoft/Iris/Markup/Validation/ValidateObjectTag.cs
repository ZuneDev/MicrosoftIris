// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.Validation.ValidateObjectTag
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;
using Microsoft.Iris.Markup.UIX;

namespace Microsoft.Iris.Markup.Validation
{
    internal class ValidateObjectTag : ValidateObject
    {
        private ValidateTypeIdentifier _typeIdentifier;
        private ValidateProperty _propertyList;
        private TypeSchema _foundType;
        private int _foundTypeIndex;
        private int _propertyCount;
        private string _name;
        private ValidateObject _indirectedObject;
        private ValidateObject _dynamicConstructionType;
        private bool _isRequired;
        private MarkupPropertySchema _propertySchemaExport;

        public ValidateObjectTag(
          SourceMarkupLoader owner,
          ValidateTypeIdentifier typeIdentifier,
          int line,
          int offset)
          : base(owner, line, offset, ObjectSourceType.ObjectTag)
        {
            _typeIdentifier = typeIdentifier;
        }

        public ValidateTypeIdentifier TypeIdentifier => _typeIdentifier;

        public ValidateProperty PropertyList => _propertyList;

        public ValidateObjectTag Next
        {
            get => (ValidateObjectTag)base.Next;
            set => base.Next = value;
        }

        public void AddProperty(ValidateProperty property)
        {
            if (_propertyList == null)
                _propertyList = property;
            else
                _propertyList.AppendToEnd(property);
        }

        public override TypeSchema ObjectType => _foundType;

        public string Name
        {
            get => _name;
            set => _name = value;
        }

        public ValidateObject DynamicConstructionType
        {
            get => _dynamicConstructionType;
            set => _dynamicConstructionType = value;
        }

        public ValidateObject IndirectedObject
        {
            get => _indirectedObject;
            set => _indirectedObject = value;
        }

        public bool PropertyIsRequiredForCreation
        {
            get => _isRequired;
            set => _isRequired = value;
        }

        public virtual PropertyOverrideCriteria PropertyOverrideCriteria => (PropertyOverrideCriteria)null;

        public MarkupPropertySchema PropertySchemaExport
        {
            get => _propertySchemaExport;
            set => _propertySchemaExport = value;
        }

        public virtual void NotifyParseComplete()
        {
        }

        protected bool ValidateObjectTagType()
        {
            if (_typeIdentifier.HasErrors || _typeIdentifier.FoundType != null)
                return !_typeIdentifier.HasErrors;
            _typeIdentifier.Validate();
            if (_typeIdentifier.HasErrors)
            {
                MarkHasErrors();
                return false;
            }
            _foundType = _typeIdentifier.FoundType;
            _foundTypeIndex = _typeIdentifier.FoundTypeIndex;
            return true;
        }

        public override void Validate(TypeRestriction typeRestriction, ValidateContext context)
        {
            if (!ValidateObjectTagType())
                return;
            if (context.CurrentPass == LoadPass.PopulatePublicModel)
            {
                if (_foundType == null || !(GetInlinePropertyValueNoValidate(_foundType.Name) == "$Required"))
                    return;
                PropertyIsRequiredForCreation = true;
            }
            else
            {
                if (context.CurrentPass != LoadPass.Full || !typeRestriction.Check(this, _foundType))
                    return;
                if (!_foundType.HasDefaultConstructor && !ForceAbstractAsConcrete && FindProperty(_foundType.Name) == null)
                {
                    ReportError("Type '{0}' may only be created via indirection (i.e. <{0} {0}=\"...\"/>)", _foundType.Name);
                }
                else
                {
                    context.NotifyObjectTagScopeEnter(this);
                    try
                    {
                        ValidateProperties(context);
                    }
                    finally
                    {
                        Result result = context.NotifyObjectTagScopeExit(this);
                        if (result.Failed)
                            ReportError(result.Error);
                    }
                    if (_propertySchemaExport == null)
                        return;
                    _propertySchemaExport.SetOverrideCriteria(PropertyOverrideCriteria);
                }
            }
        }

        protected virtual void ValidateProperties(ValidateContext context)
        {
            _propertyCount = 0;
            for (ValidateProperty property = _propertyList; property != null; property = property.Next)
            {
                ValidateDuplicateProperties(property);
                property.Validate(this, context);
                if (property.HasErrors)
                    MarkHasErrors();
                if (!property.IsPseudo)
                    ++_propertyCount;
            }
            if (PropertyIsRequiredForCreation)
            {
                PropertySchema activePropertyScope = context.GetActivePropertyScope();
                if (activePropertyScope == ValidateContext.ClassPropertiesProperty || activePropertyScope == ValidateContext.UIPropertiesProperty)
                    return;
                ReportError("'$Required' can't be used in this context. It may only be used for declaring required properties on Classes and UIs");
            }
            else if (_indirectedObject != null)
            {
                if (_indirectedObject.HasErrors)
                    return;
                if (!_foundType.IsAssignableFrom(_indirectedObject.ObjectType))
                {
                    ReportError("'{0}' cannot be used in this context (expecting types compatible with '{1}')", _indirectedObject.ObjectType.Name, _foundType.Name);
                }
                else
                {
                    for (ValidateProperty validateProperty = _propertyList; validateProperty != null; validateProperty = validateProperty.Next)
                    {
                        if (!validateProperty.IsPseudo)
                        {
                            if (validateProperty.PropertyName != "Name")
                            {
                                ReportError("Property sets are not supported on type indirection tag. Property specified was '{0}'", validateProperty.PropertyName);
                                break;
                            }
                            validateProperty.MarkAsPseudo();
                        }
                    }
                }
            }
            else
                ValidateRequiredProperties();
        }

        private void ValidateDuplicateProperties(ValidateProperty property)
        {
            bool flag = false;
            for (ValidateProperty validateProperty = _propertyList; validateProperty != null; validateProperty = validateProperty.Next)
            {
                if (validateProperty.PropertyName == property.PropertyName)
                {
                    if (flag)
                    {
                        property.ReportError("Property '{0}' was specified more than once", property.PropertyName);
                        MarkHasErrors();
                        break;
                    }
                    flag = true;
                }
            }
        }

        private void ValidateRequiredProperties()
        {
            foreach (string str in _foundType.FindRequiredPropertyNamesDeep())
            {
                bool flag = false;
                for (ValidateProperty validateProperty = _propertyList; validateProperty != null; validateProperty = validateProperty.Next)
                {
                    if (validateProperty.PropertyName == str)
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    ReportError("Property '{0}' must be specified for '{1}' to function", str, _foundType.Name);
                    MarkHasErrors();
                }
            }
        }

        protected virtual bool ForceAbstractAsConcrete => false;

        public ValidateProperty FindProperty(string propertyName) => FindProperty(propertyName, false);

        public ValidateProperty FindProperty(string propertyName, bool remove)
        {
            ValidateProperty validateProperty1 = _propertyList;
            ValidateProperty validateProperty2 = null;
            for (; validateProperty1 != null; validateProperty1 = validateProperty1.Next)
            {
                if (propertyName == validateProperty1.PropertyName)
                {
                    if (remove)
                    {
                        if (validateProperty2 != null)
                            validateProperty2.Next = validateProperty1.Next;
                        if (validateProperty1 == _propertyList)
                            _propertyList = validateProperty1.Next;
                        validateProperty1.Next = null;
                    }
                    return validateProperty1;
                }
                validateProperty2 = validateProperty1;
            }
            return null;
        }

        public void RemoveProperty(ValidateProperty propertyRemove)
        {
            ValidateProperty validateProperty1 = _propertyList;
            ValidateProperty validateProperty2 = null;
            for (; validateProperty1 != null; validateProperty1 = validateProperty1.Next)
            {
                if (validateProperty1 == propertyRemove)
                {
                    if (validateProperty2 != null)
                        validateProperty2.Next = validateProperty1.Next;
                    if (validateProperty1 == _propertyList)
                        _propertyList = validateProperty1.Next;
                    validateProperty1.Next = null;
                    break;
                }
                validateProperty2 = validateProperty1;
            }
        }

        public string GetInlinePropertyValueNoValidate(string propertyName)
        {
            ValidateProperty property = FindProperty(propertyName);
            return property != null && property.IsFromStringValue ? ((ValidateFromString)property.Value).FromString : null;
        }

        public void MovePropertyToFront(string propertyName)
        {
            ValidateProperty property = FindProperty(propertyName, true);
            if (property == null)
                return;
            property.Next = _propertyList;
            _propertyList = property;
        }

        public void AddStringProperty(string propertyName, string value)
        {
            ValidateFromString validateFromString = new ValidateFromString(Owner, value, false, Line, Column);
            ValidateProperty validateProperty = new ValidateProperty(Owner, propertyName, validateFromString, Line, Column);
            if (_propertyList == null)
                _propertyList = validateProperty;
            else
                _propertyList.AppendToEnd(validateProperty);
        }

        public TypeSchema ExtractTypeSchemaProperty(
          string propertyName,
          ValidateContext context,
          bool required)
        {
            ValidateProperty property = FindProperty(propertyName, true);
            if (property != null)
            {
                ValidateTypeIdentifier.PromoteSimplifiedTypeSyntax(property);
                if (property.IsExpressionValue)
                {
                    ValidateExpression validateExpression = (ValidateExpression)property.Value;
                    if (validateExpression.ExpressionType == ExpressionType.TypeOf)
                    {
                        validateExpression.Validate(new TypeRestriction(TypeSchemaDefinition.Type), context);
                        if (!validateExpression.HasErrors)
                            return ((ValidateExpressionTypeOf)validateExpression).TypeIdentifier.FoundType;
                        MarkHasErrors();
                    }
                    else
                        ReportError("Invalid expression for '{0}', expecting plain type identifier", property.PropertyName);
                }
                else if (property.IsFromStringValue)
                    ReportError("Invalid value '{0}' for attribute '{1}'", ((ValidateFromString)property.Value).FromString, property.PropertyName);
                else
                    ReportError("Property '{0}' does not support expanded value syntax", property.PropertyName);
            }
            else if (required)
                ReportError("Property '{0}' must be specified", propertyName);
            return null;
        }

        public bool ExtractBooleanProperty(
          string propertyName,
          ValidateContext context,
          bool required,
          out bool value)
        {
            ValidateProperty property = FindProperty(propertyName, true);
            if (property != null)
            {
                if (property.IsFromStringValue)
                {
                    ValidateFromString validateFromString = (ValidateFromString)property.Value;
                    validateFromString.Validate(new TypeRestriction(BooleanSchema.Type), context);
                    if (!validateFromString.HasErrors)
                    {
                        value = (bool)validateFromString.FromStringInstance;
                        return true;
                    }
                    MarkHasErrors();
                }
                else
                    ReportError("Property '{0}' does not support expanded value syntax", property.PropertyName);
            }
            else if (required)
                ReportError("Property '{0}' must be specified", propertyName);
            value = false;
            return false;
        }

        public string ExtractStringProperty(string propertyName, bool required)
        {
            ValidateProperty property = FindProperty(propertyName, true);
            if (property != null)
            {
                if (property.IsFromStringValue)
                    return ((ValidateFromString)property.Value).FromString;
                ReportError("Property '{0}' does not support expanded value syntax", propertyName);
            }
            else if (required)
                ReportError("Property '{0}' must be specified", propertyName);
            return null;
        }

        public TypeSchema FoundType => _foundType;

        public int FoundTypeIndex => _foundTypeIndex;

        public int PropertyCount => _propertyCount;

        public override string ToString()
        {
            string str = "";
            if (_name != null)
                str += string.Format("Name='{0}'", _name);
            if (_indirectedObject != null)
            {
                if (_name != null)
                    str += ", ";
                str += string.Format("Indirected=[{0}]", _indirectedObject.ToString());
            }
            return string.Format("Tag : {0} {1}", _typeIdentifier, str);
        }
    }
}
