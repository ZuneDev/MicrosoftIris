// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.Validation.ValidateProperty
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;
using Microsoft.Iris.Markup.UIX;

namespace Microsoft.Iris.Markup.Validation
{
    internal class ValidateProperty : Microsoft.Iris.Markup.Validation.Validate
    {
        public const string c_requiredValue = "$Required";
        public const string c_overrideProperty = "Override";
        private string _propertyName;
        private ValidateObject _value;
        private bool _multipleValueProperty;
        private int _valueCount;
        private ValueApplyMode _valueApplyMode;
        private PropertyAttribute _propertyAttributeList;
        private bool _allowPropertyAttributes;
        private PropertySchema _foundProperty;
        private int _foundPropertyIndex;
        private int _foundPropertyTypeIndex;
        private bool _isPseudo;
        private bool _shouldSkipDictionaryAddIfContains;
        private ValidateProperty _next;

        public ValidateProperty(SourceMarkupLoader owner, string propertyName, int line, int column)
          : this(owner, propertyName, null, line, column)
        {
        }

        public ValidateProperty(
          SourceMarkupLoader owner,
          string propertyName,
          ValidateObject value,
          int line,
          int column)
          : base(owner, line, column)
        {
            _propertyName = propertyName;
            _value = value;
        }

        public string PropertyName => _propertyName;

        public ValidateObject Value
        {
            get => _value;
            set => _value = value;
        }

        public PropertyAttribute PropertyAttributeList => _propertyAttributeList;

        public void AllowPropertyAttributes() => _allowPropertyAttributes = true;

        public ValidateProperty Next
        {
            get => _next;
            set => _next = value;
        }

        public void AppendToEnd(ValidateProperty item)
        {
            ValidateProperty validateProperty = this;
            while (validateProperty.Next != null)
                validateProperty = validateProperty.Next;
            validateProperty.Next = item;
        }

        public void AddValue(ValidateObject value)
        {
            if (_value == null)
                _value = value;
            else if (_value is ValidateObjectTag && value is ValidateObjectTag)
                _value.AppendToEnd(value);
            else if (_value is ValidateCode && value is ValidateCode)
                _value.AppendToEnd(value);
            else
                ReportError("Property '{0}' cannot contain both object-tag and script block values", _propertyName);
        }

        public void AddAttribute(PropertyAttribute attribute)
        {
            attribute.Next = _propertyAttributeList;
            _propertyAttributeList = attribute;
        }

        public void Validate(ValidateObjectTag targetObject, ValidateContext context)
        {
            TypeSchema objectType = targetObject.ObjectType;
            if (_propertyName == "Override" && ValidateOverrideProperty(targetObject, context))
                return;
            string name = _propertyName;
            int length = _propertyName.IndexOf('.');
            if (length >= 0 && _propertyName.Length > length + 1 && _propertyName.Substring(0, length) == targetObject.TypeIdentifier.TypeName)
                name = _propertyName.Substring(length + 1);
            _foundProperty = objectType.FindPropertyDeep(name);
            if (_foundProperty == null)
            {
                if (_propertyName == "Name")
                    ValidateNameProperty(true, targetObject, context);
                else if (_propertyName == targetObject.TypeIdentifier.TypeName)
                    ValidatePseudoConstructionProperty(targetObject, context);
                else
                    ReportError("Property '{0}' does not exist on '{1}'", _propertyName, objectType.Name);
            }
            else
            {
                if (_propertyName == "Name")
                    ValidateNameProperty(false, targetObject, context);
                UpdateFoundProperty(_foundProperty);
                if (_propertyAttributeList != null && !_allowPropertyAttributes)
                    ReportError("Property attributes are not supported on property '{0}'", _propertyName);
                if (IsCodeValue && ((ValidateCode)_value).Next != null)
                {
                    ReportError("Property '{0}' does not support multi-value script blocks", _propertyName);
                }
                else
                {
                    bool flag1 = DictionarySchema.Type.IsAssignableFrom(_foundProperty.PropertyType);
                    bool flag2 = ListSchema.Type.IsAssignableFrom(_foundProperty.PropertyType);
                    _multipleValueProperty = flag1 || flag2;
                    bool flag3 = IsObjectTagValue && ((ValidateObjectTag)_value).Next != null;
                    if (!_multipleValueProperty && flag3)
                    {
                        ReportError("Property '{0}' does not support multiple values (property type is '{1}')", _propertyName, _foundProperty.PropertyType.Name);
                    }
                    else
                    {
                        context.NotifyPropertyScopeEnter(this);
                        try
                        {
                            if (!_multipleValueProperty)
                            {
                                if (_value == null)
                                {
                                    ReportError("Property '{0}' requires a value to be provided", _propertyName);
                                    return;
                                }
                                _valueCount = 1;
                                _valueApplyMode = ValueApplyMode.SingleValueSet;
                                _value.Validate(new TypeRestriction(_foundProperty.PropertyType, _foundProperty.AlternateType), context);
                                if (_value.HasErrors)
                                {
                                    MarkHasErrors();
                                    return;
                                }
                                if (_foundProperty.RangeValidator != null && IsFromStringValue)
                                {
                                    Result result = _foundProperty.RangeValidator(((ValidateFromString)_value).FromStringInstance);
                                    if (result.Failed)
                                        ReportError(result.Error);
                                }
                            }
                            else
                            {
                                TypeSchema primary = _foundProperty.AlternateType ?? ObjectSchema.Type;
                                _valueApplyMode = !flag1 ? ValueApplyMode.MultiValueList : ValueApplyMode.MultiValueDictionary;
                                if (_foundProperty.CanWrite && _foundProperty.PropertyType.HasDefaultConstructor)
                                    _valueApplyMode |= ValueApplyMode.CollectionPopulateAndSet;
                                else
                                    _valueApplyMode |= ValueApplyMode.CollectionAdd;
                                for (ValidateObject validateObject = _value; validateObject != null; validateObject = validateObject.ObjectSourceType != ObjectSourceType.ObjectTag ? null : (ValidateObject)((ValidateObjectTag)validateObject).Next)
                                {
                                    ++_valueCount;
                                    TypeRestriction typeRestriction = !flag3 ? new TypeRestriction(primary, _foundProperty.PropertyType) : new TypeRestriction(primary);
                                    validateObject.Validate(typeRestriction, context);
                                    if (validateObject.HasErrors)
                                    {
                                        MarkHasErrors();
                                    }
                                    else
                                    {
                                        if (!flag3 && _foundProperty.PropertyType.IsAssignableFrom(validateObject.ObjectType))
                                        {
                                            bool flag4 = true;
                                            if (flag1)
                                            {
                                                if (validateObject.ObjectSourceType == ObjectSourceType.ObjectTag && ((ValidateObjectTag)validateObject).Name != null)
                                                    flag4 = false;
                                            }
                                            else if (!_foundProperty.CanWrite)
                                                flag4 = false;
                                            if (flag4)
                                            {
                                                _valueApplyMode = ValueApplyMode.SingleValueSet;
                                                break;
                                            }
                                        }
                                        if (validateObject.ObjectSourceType != ObjectSourceType.ObjectTag)
                                        {
                                            ReportError("Collection property '{0}' may only have multiple values specified in expanded form", _propertyName);
                                            break;
                                        }
                                        if ((_valueApplyMode & ValueApplyMode.MultiValueDictionary) != ValueApplyMode.SingleValueSet && ((ValidateObjectTag)validateObject).Name == null)
                                            ReportError("Dictionary property '{0}' requires all values be named", _propertyName);
                                    }
                                }
                            }
                            if (_valueApplyMode == ValueApplyMode.SingleValueSet)
                            {
                                if (_foundProperty.CanWrite)
                                    return;
                                ReportError("Property '{0}' does not support setting", _foundProperty.Name);
                            }
                            else
                            {
                                if (_foundProperty.CanRead)
                                    return;
                                ReportError("Property '{0}' must support reading to retrieve its collection for multiple value adds", _foundProperty.Name);
                            }
                        }
                        finally
                        {
                            context.NotifyPropertyScopeExit(this);
                        }
                    }
                }
            }
        }

        private void ValidateNameProperty(
          bool isPseudo,
          ValidateObjectTag targetObject,
          ValidateContext context)
        {
            if (isPseudo)
                MarkAsPseudo();
            if (!IsFromStringValue)
            {
                ReportError("Property '{0}' does not support expanded value syntax", _propertyName);
            }
            else
            {
                ValidateFromString validateFromString = (ValidateFromString)_value;
                validateFromString.Validate(new TypeRestriction(StringSchema.Type), context);
                if (validateFromString.HasErrors)
                    MarkHasErrors();
                else
                    targetObject.Name = validateFromString.FromString;
            }
        }

        private void ValidatePseudoConstructionProperty(
          ValidateObjectTag targetObject,
          ValidateContext context)
        {
            bool flag = false;
            MarkAsPseudo();
            if (IsObjectTagValue && ((ValidateObjectTag)_value).Next != null)
            {
                ReportError("Property '{0}' does not support multiple values (property type is '{1}')", _propertyName, targetObject.ObjectType.Name);
            }
            else
            {
                if (IsFromStringValue && ((ValidateFromString)_value).FromString == "$Required")
                {
                    flag = true;
                    targetObject.PropertyIsRequiredForCreation = flag;
                }
                if (flag)
                    return;
                _value.Validate(new TypeRestriction(targetObject.ObjectType, TypeSchemaDefinition.Type), context);
                if (_value.HasErrors)
                    MarkHasErrors();
                else if (targetObject.ObjectType.IsAssignableFrom(_value.ObjectType))
                    targetObject.IndirectedObject = _value;
                else
                    targetObject.DynamicConstructionType = _value;
            }
        }

        private bool ValidateOverrideProperty(ValidateObjectTag targetObject, ValidateContext context)
        {
            if (context.ActiveSymbolScope != SymbolOrigin.Properties)
                return false;
            bool result;
            if (IsFromStringValue && bool.TryParse(((ValidateFromString)_value).FromString, out result))
            {
                if (targetObject.PropertySchemaExport != null && context.Owner.TypeExport != null)
                {
                    PropertySchema propertySchema = null;
                    if (context.Owner.TypeExport.MarkupTypeBase != null)
                        propertySchema = context.Owner.TypeExport.MarkupTypeBase.FindPropertyDeep(targetObject.PropertySchemaExport.Name);
                    if (result && propertySchema == null)
                        ReportError("Property '{0}' was specified as Override='true' but no property was found to override", targetObject.PropertySchemaExport.Name);
                    else if (!result && propertySchema != null)
                        ReportError("Property '{0}' was specified as Override='false' but a base property was found", targetObject.PropertySchemaExport.Name);
                }
            }
            else
                ReportError("Property '{0}' must be Boolean 'true' or 'false'", "Override");
            MarkAsPseudo();
            return true;
        }

        public bool IsPseudo => _isPseudo;

        public void MarkAsPseudo() => _isPseudo = true;

        public void RepurposeProperty(string newName, PropertyAttribute newAttributes)
        {
            _propertyName = newName;
            _propertyAttributeList = newAttributes;
        }

        public bool IsObjectTagValue => _value != null && _value.ObjectSourceType == ObjectSourceType.ObjectTag;

        public bool IsFromStringValue => _value != null && _value.ObjectSourceType == ObjectSourceType.FromString;

        public bool IsCodeValue => _value != null && _value.ObjectSourceType == ObjectSourceType.Code;

        public bool IsExpressionValue => _value != null && _value.ObjectSourceType == ObjectSourceType.Expression;

        public void UpdateFoundProperty(PropertySchema newFoundProperty)
        {
            _foundProperty = newFoundProperty;
            _foundPropertyIndex = Owner.TrackImportedProperty(_foundProperty);
            _foundPropertyTypeIndex = Owner.TrackImportedType(_foundProperty.PropertyType);
        }

        public PropertySchema FoundProperty => _foundProperty;

        public int FoundPropertyIndex => _foundPropertyIndex;

        public int FoundPropertyTypeIndex => _foundPropertyTypeIndex;

        public int ValueCount => _valueCount;

        public bool ShouldSkipDictionaryAddIfContains
        {
            get => _shouldSkipDictionaryAddIfContains;
            set => _shouldSkipDictionaryAddIfContains = value;
        }

        public ValueApplyMode ValueApplyMode => _valueApplyMode;

        public void ReversePropertyValues()
        {
            ValidateObjectTag next;
            for (ValidateObjectTag validateObjectTag = (ValidateObjectTag)_value; validateObjectTag != null; validateObjectTag = next)
            {
                next = validateObjectTag.Next;
                validateObjectTag.Next = null;
                if (validateObjectTag != _value)
                {
                    validateObjectTag.Next = (ValidateObjectTag)_value;
                    _value = validateObjectTag;
                }
            }
        }

        public override string ToString() => _value != null ? string.Format("{0} = {1}", _propertyName, _value.ToString()) : "Unavailable";
    }
}
