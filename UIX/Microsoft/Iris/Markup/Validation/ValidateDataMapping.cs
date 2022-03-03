// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.Validation.ValidateDataMapping
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Data;
using Microsoft.Iris.Library;
using Microsoft.Iris.Markup.UIX;

namespace Microsoft.Iris.Markup.Validation
{
    internal class ValidateDataMapping : ValidateObjectTag
    {
        private LoadPass _currentValidationPass;
        private string _previewName;
        private ValidateTypeIdentifier _baseTypeIdentifier;
        private Vector<MarkupDataMapping> _foundDataMappingSet;

        public ValidateDataMapping(
          SourceMarkupLoader owner,
          ValidateTypeIdentifier typeIdentifier,
          int line,
          int offset)
          : base(owner, typeIdentifier, line, offset)
        {
        }

        public void Validate(LoadPass currentPass)
        {
            if (_currentValidationPass >= currentPass)
                return;
            _currentValidationPass = currentPass;
            ValidateContext context = new ValidateContext(null, null, _currentValidationPass);
            Validate(TypeRestriction.None, context);
            if (_currentValidationPass != LoadPass.Full)
                return;
            var markupDataTypeSchema = (MarkupDataTypeSchema)null;
            TypeSchema typeSchemaProperty = ExtractTypeSchemaProperty("TargetType", context, true);
            switch (typeSchemaProperty)
            {
                case null:
                case MarkupDataTypeSchema _:
                    markupDataTypeSchema = (MarkupDataTypeSchema)typeSchemaProperty;
                    string stringProperty1 = ExtractStringProperty("Provider", true);
                    MarkupDataMappingEntry[] mappingEntries = null;
                    ValidateProperty property = FindProperty("Mappings");
                    if (property != null && property.IsObjectTagValue)
                    {
                        Map<string, MarkupDataMappingEntry> entries = new Map<string, MarkupDataMappingEntry>();
                        for (ValidateObjectTag next = (ValidateObjectTag)property.Value; next != null; next = next.Next)
                        {
                            string stringProperty2 = next.ExtractStringProperty("Property", true);
                            string stringProperty3 = next.ExtractStringProperty("Source", false);
                            string stringProperty4 = next.ExtractStringProperty("Target", false);
                            string stringProperty5 = next.ExtractStringProperty("DefaultValue", false);
                            if (stringProperty2 != null && markupDataTypeSchema != null)
                            {
                                MarkupDataTypePropertySchema propertyDeep = (MarkupDataTypePropertySchema)markupDataTypeSchema.FindPropertyDeep(stringProperty2);
                                if (propertyDeep != null)
                                {
                                    MarkupDataMappingEntry dataMappingEntry = new MarkupDataMappingEntry();
                                    dataMappingEntry.Property = propertyDeep;
                                    dataMappingEntry.Source = stringProperty3;
                                    dataMappingEntry.Target = stringProperty4;
                                    dataMappingEntry.DefaultValue = ConvertDefaultValue(this, propertyDeep.PropertyType, stringProperty5);
                                    if (!entries.ContainsKey(stringProperty2))
                                        entries[stringProperty2] = dataMappingEntry;
                                    else
                                        ReportError("Mapping for property '{0}' was already specified", stringProperty2);
                                }
                                else
                                    ReportError("Could not find property '{0}' on type '{1}' for data mapping with provider '{2}'", stringProperty2, markupDataTypeSchema.Name, stringProperty1);
                            }
                        }
                        mappingEntries = MarkupDataProvider.FillInDefaultMappings(markupDataTypeSchema, entries);
                        foreach (MarkupDataMappingEntry dataMappingEntry in mappingEntries)
                            Owner.TrackImportedProperty(dataMappingEntry.Property);
                    }
                    if (stringProperty1 == null || markupDataTypeSchema == null)
                        break;
                    AddDataMappingProviderList(ref _foundDataMappingSet, Owner.LoadResultTarget, Name, markupDataTypeSchema, stringProperty1, mappingEntries);
                    break;
                default:
                    ReportError("TargetType for DataMapping must be a markup-defined DataType, {0} is not valid", typeSchemaProperty.Name);
                    goto case null;
            }
        }

        public static void AddDataMappingProviderList(
          ref Vector<MarkupDataMapping> foundDataMappingSet,
          MarkupLoadResult owner,
          string name,
          MarkupDataTypeSchema targetDataType,
          string provider,
          MarkupDataMappingEntry[] mappingEntries)
        {
            if (provider.IndexOf(',') == -1)
            {
                AddDataMappingOneProvider(ref foundDataMappingSet, owner, name, targetDataType, provider, mappingEntries);
            }
            else
            {
                foreach (string providerName in StringUtility.SplitAndTrim(',', provider))
                    AddDataMappingOneProvider(ref foundDataMappingSet, owner, name, targetDataType, providerName, mappingEntries);
            }
        }

        private static void AddDataMappingOneProvider(
          ref Vector<MarkupDataMapping> foundDataMappingSet,
          MarkupLoadResult owner,
          string name,
          MarkupDataTypeSchema targetDataType,
          string providerName,
          MarkupDataMappingEntry[] mappingEntries)
        {
            if (providerName == null)
                return;
            if (foundDataMappingSet == null)
                foundDataMappingSet = new Vector<MarkupDataMapping>();
            foundDataMappingSet.Add(new MarkupDataMapping(name)
            {
                TargetType = targetDataType,
                Provider = providerName,
                Mappings = mappingEntries
            });
        }

        public static object ConvertDefaultValue(
          Microsoft.Iris.Markup.Validation.Validate validate,
          TypeSchema type,
          string valueString)
        {
            object instance = null;
            if (valueString != null)
            {
                if (type.SupportsTypeConversion(StringSchema.Type))
                {
                    Result result = type.TypeConverter(valueString, StringSchema.Type, out instance);
                    if (result.Failed)
                        validate.ReportError(result.Error);
                }
                else
                    validate.ReportError("String conversion is not available for '{0}'", type.Name);
                if (!type.SupportsBinaryEncoding)
                    validate.ReportError("DefaultValue can only be on types that support binary encoding; '{0}' does not", type.Name);
            }
            else
                instance = MarkupDataProvider.GetDefaultValueForType(type);
            return instance;
        }

        public override void NotifyParseComplete()
        {
            _previewName = GetInlinePropertyValueNoValidate("Name");
            string propertyValueNoValidate = GetInlinePropertyValueNoValidate("Base");
            if (propertyValueNoValidate != null && propertyValueNoValidate.IndexOf(':') != -1)
                _baseTypeIdentifier = new ValidateTypeIdentifier(Owner, propertyValueNoValidate, Line, Column);
            Owner.NotifyClassParseComplete(_previewName);
        }

        protected override bool ForceAbstractAsConcrete => true;

        public Vector<MarkupDataMapping> FoundDataMappingSet => _foundDataMappingSet;
    }
}
