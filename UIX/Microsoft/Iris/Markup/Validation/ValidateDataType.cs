// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.Validation.ValidateDataType
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Markup.UIX;

namespace Microsoft.Iris.Markup.Validation
{
    internal class ValidateDataType : ValidateClass
    {
        private string _provider;
        private Map<string, MarkupDataMappingEntry> _foundInlineMappings;
        private Vector<MarkupDataMapping> _foundDataMappingSet;

        public ValidateDataType(
          SourceMarkupLoader owner,
          ValidateTypeIdentifier typeIdentifier,
          int line,
          int offset)
          : base(owner, typeIdentifier, line, offset)
        {
        }

        protected override void PopulatePublicModel(ValidateContext context)
        {
            base.PopulatePublicModel(context);
            _provider = ExtractStringProperty("Provider", false);
        }

        protected override void AnnotateProperty(
          ValidateObjectTag objectTag,
          ValidateContext context,
          MarkupPropertySchema propertyExport)
        {
            if (ListSchema.Type.IsAssignableFrom(objectTag.FoundType))
            {
                TypeSchema typeSchemaProperty = objectTag.ExtractTypeSchemaProperty("UnderlyingCollectionType", context, false);
                if (typeSchemaProperty != null)
                    ((MarkupDataTypePropertySchema)propertyExport).SetUnderlyingCollectionType(typeSchemaProperty);
            }
            string stringProperty1 = objectTag.ExtractStringProperty("DefaultValue", false);
            string stringProperty2 = objectTag.ExtractStringProperty("Source", false);
            string stringProperty3 = objectTag.ExtractStringProperty("Target", false);
            if (stringProperty1 != null || stringProperty2 != null || stringProperty3 != null)
            {
                if (_foundInlineMappings == null)
                    _foundInlineMappings = new Map<string, MarkupDataMappingEntry>();
                _foundInlineMappings[propertyExport.Name] = new MarkupDataMappingEntry()
                {
                    DefaultValue = ValidateDataMapping.ConvertDefaultValue(this, propertyExport.PropertyType, stringProperty1),
                    Source = stringProperty2,
                    Target = stringProperty3,
                    Property = (MarkupDataTypePropertySchema)propertyExport
                };
            }
            if (!objectTag.HasErrors)
                return;
            MarkHasErrors();
        }

        protected override void FullValidation(ValidateContext context)
        {
            base.FullValidation(context);
            if (_foundInlineMappings != null && _provider == null)
                ReportError("Inline mappings are allowed only on provider-specific types.  Must specify 'Provider' attribute on '{0}'", Name);
            if (_provider == null)
                return;
            if (_foundInlineMappings == null)
                _foundInlineMappings = new Map<string, MarkupDataMappingEntry>();
            MarkupDataTypeSchema typeExport = (MarkupDataTypeSchema)TypeExport;
            MarkupDataMappingEntry[] mappingEntries = MarkupDataProvider.FillInDefaultMappings(typeExport, _foundInlineMappings);
            ValidateDataMapping.AddDataMappingProviderList(ref _foundDataMappingSet, Owner.LoadResultTarget, null, typeExport, _provider, mappingEntries);
            foreach (MarkupDataMappingEntry dataMappingEntry in mappingEntries)
                Owner.TrackImportedProperty(dataMappingEntry.Property);
        }

        public Vector<MarkupDataMapping> FoundDataMappingSet => _foundDataMappingSet;
    }
}
