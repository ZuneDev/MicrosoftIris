// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.MarkupDataProvider
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Session;
using System.Collections.Generic;

namespace Microsoft.Iris.Markup
{
    internal static class MarkupDataProvider
    {
        private static Map<MarkupDataProvider.MappingKey, MarkupDataMapping> s_mappings = new Map<MarkupDataProvider.MappingKey, MarkupDataMapping>();
        private static Dictionary<string, IDataProvider> s_providers = new Dictionary<string, IDataProvider>();

        public static void AddDataMapping(MarkupDataMapping mapping)
        {
            MarkupDataProvider.MappingKey key = new MarkupDataProvider.MappingKey(mapping.TargetType, mapping.Provider);
            if (!s_mappings.ContainsKey(key))
                s_mappings[key] = mapping;
            else
                ErrorManager.ReportError("Data mapping already defined for type '{0}', provider '{1}'", mapping.TargetType.Name, mapping.Provider);
        }

        public static void RemoveDataMapping(MarkupDataMapping mapping)
        {
            MarkupDataProvider.MappingKey key = new MarkupDataProvider.MappingKey(mapping.TargetType, mapping.Provider);
            s_mappings.Remove(key);
        }

        public static MarkupDataMapping FindDataMapping(
          string providerName,
          MarkupDataTypeSchema typeSchema)
        {
            MarkupDataProvider.MappingKey key = new MarkupDataProvider.MappingKey(typeSchema, providerName);
            MarkupDataMapping markupDataMapping;
            if (!s_mappings.TryGetValue(key, out markupDataMapping))
            {
                markupDataMapping = new MarkupDataMapping(null);
                markupDataMapping.Provider = providerName;
                markupDataMapping.TargetType = typeSchema;
                markupDataMapping.Mappings = FillInDefaultMappings(typeSchema, null);
                s_mappings[key] = markupDataMapping;
            }
            return markupDataMapping;
        }

        public static void RegisterDataProvider(IDataProvider provider) => s_providers[provider.Name] = provider;

        public static IDataProvider GetDataProvider(string providerName)
        {
            IDataProvider dataProvider;
            return s_providers.TryGetValue(providerName, out dataProvider) ? dataProvider : null;
        }

        public static object GetDefaultValueForType(TypeSchema type) => type.IsNullAssignable ? null : type.ConstructDefault();

        public static MarkupDataMappingEntry[] FillInDefaultMappings(
          MarkupDataTypeSchema targetType,
          Map<string, MarkupDataMappingEntry> entries)
        {
            if (entries == null)
                entries = new Map<string, MarkupDataMappingEntry>();
            for (MarkupDataTypeSchema markupDataTypeSchema = targetType; markupDataTypeSchema != null; markupDataTypeSchema = markupDataTypeSchema.Base as MarkupDataTypeSchema)
            {
                foreach (MarkupDataTypePropertySchema property in markupDataTypeSchema.Properties)
                {
                    if (!entries.ContainsKey(property.Name))
                        entries[property.Name] = new MarkupDataMappingEntry()
                        {
                            Property = property,
                            DefaultValue = GetDefaultValueForType(property.PropertyType)
                        };
                }
            }
            MarkupDataMappingEntry[] dataMappingEntryArray = new MarkupDataMappingEntry[entries.Count];
            int num = 0;
            foreach (MarkupDataMappingEntry dataMappingEntry in entries.Values)
                dataMappingEntryArray[num++] = dataMappingEntry;
            return dataMappingEntryArray;
        }

        private struct MappingKey
        {
            private MarkupDataTypeSchema _targetType;
            private string _provider;

            public MappingKey(MarkupDataTypeSchema targetType, string provider)
            {
                _targetType = targetType;
                _provider = provider;
            }

            public override bool Equals(object rhs) => rhs is MarkupDataProvider.MappingKey mappingKey && _targetType == mappingKey._targetType && _provider == mappingKey._provider;

            public override int GetHashCode() => _targetType.GetHashCode() ^ _provider.GetHashCode();
        }
    }
}
