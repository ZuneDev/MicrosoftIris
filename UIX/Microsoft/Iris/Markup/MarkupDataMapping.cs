// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.MarkupDataMapping
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Iris.Markup
{
    internal class MarkupDataMapping
    {
        private string _name;
        private MarkupDataMappingEntry[] _mappings;
        private MarkupDataTypeSchema _targetType;
        private string _provider;
        private object _assemblyDataProviderCookie;

        public MarkupDataMapping(string name) => _name = name;

        public MarkupDataTypeSchema TargetType
        {
            get => _targetType;
            set => _targetType = value;
        }

        public string Provider
        {
            get => _provider;
            set => _provider = value;
        }

        public MarkupDataMappingEntry[] Mappings
        {
            get => _mappings;
            set => _mappings = value;
        }

        public object AssemblyDataProviderCookie
        {
            get => _assemblyDataProviderCookie;
            set => _assemblyDataProviderCookie = value;
        }

        public override string ToString() => $"({_provider}, {_targetType})";

        /// <summary>
        /// Generates C# code for a plain-old CLR object (POCO) that can
        /// be used to serialize and deserialize the data this mapping
        /// represents.
        /// </summary>
        public string GenerateModelCode()
        {
            const string indent = "    ";
            const string newline = "\r\n";

            string start = $@"using System;{newline}using System.Collections.Generic;{newline}{newline}namespace Zune.Xml;{newline}{newline}public class {TargetType.Name}{newline}{{{newline}";
            string end = $@"}}{newline}";

            var sb = new StringBuilder(start);

            foreach (var mapping in Mappings)
            {
                string propertyType = mapping.Property.PropertyType.Name;
                if (_knownTypeNames.TryGetValue(propertyType, out var knownTypeName))
                    propertyType = knownTypeName;

                string properyDeclaration = $"{indent}public {propertyType} {mapping.Property.Name} {{ get; set; }}";

                if (mapping.Source != null)
                {
                    string source = mapping.Source.StartsWith("/") ? mapping.Source.Substring(1) : mapping.Source;
                    string xmlElemAttribute = $"{indent}[XmlElement(\"{source}\")]{newline}";
                    properyDeclaration = xmlElemAttribute + properyDeclaration;
                }

                Type runtimeType = mapping.Property?.PropertyType?.RuntimeType;
                if (runtimeType != null)
                {
                    object defaultValue = (runtimeType.IsValueType && Nullable.GetUnderlyingType(runtimeType) == null)
                        ? Activator.CreateInstance(runtimeType)
                        : null;
                    string defaultValueStr = defaultValue?.ToString() ?? "null";

                    if (defaultValueStr != (mapping.DefaultValue?.ToString() ?? "null"))
                        properyDeclaration += $" = {defaultValueStr};";
                }

                sb.Append(properyDeclaration + newline + newline);
            }

            sb.Append(end);

            return sb.ToString();
        }

        private static readonly Dictionary<string, string> _knownTypeNames = new Dictionary<string, string>()
        {
            { "Boolean", "bool" },
            { "Int32", "int" },
            { "UInt32", "uint" },
            { "Int64", "long" },
            { "UInt64", "ulong" },
            { "Int16", "short" },
            { "UInt16", "ushort" },
            { "String", "string" },
            { "List", "List<object>" },
        };
    }
}
