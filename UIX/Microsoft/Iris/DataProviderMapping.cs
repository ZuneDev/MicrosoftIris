// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.DataProviderMapping
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Markup;
using Microsoft.Iris.Markup.UIX;
using System;

namespace Microsoft.Iris
{
    public class DataProviderMapping
    {
        private string _source;
        private string _target;
        private object _defaultValue;
        private PropertySchema _propertySchema;
        private Type _assemblyPropertyType;
        private Type _assemblyAlternateType;

        public object PropertyTypeCookie => _propertySchema.PropertyType;

        public object UnderlyingCollectionTypeCookie => _propertySchema.AlternateType;

        public string PropertyName => _propertySchema.Name;

        public string PropertyTypeName => GetCanonicalTypeName(_propertySchema.PropertyType);

        public Type PropertyType => _assemblyPropertyType;

        public string UnderlyingCollectionTypeName => GetCanonicalTypeName(_propertySchema.AlternateType);

        public Type UnderlyingCollectionType => _assemblyAlternateType;

        public string Source => _source;

        public string Target => _target;

        public object DefaultValue => _defaultValue;

        internal DataProviderMapping(PropertySchema propertySchema, object defaultValue)
          : this(propertySchema, null, null, defaultValue)
        {
        }

        internal DataProviderMapping(
          PropertySchema propertySchema,
          string source,
          string target,
          object defaultValue)
        {
            _propertySchema = propertySchema;
            _source = source;
            _target = target;
            _defaultValue = defaultValue;
            _assemblyPropertyType = AssemblyLoadResult.MapType(_propertySchema.PropertyType);
            if (_propertySchema.AlternateType == null)
                return;
            _assemblyAlternateType = AssemblyLoadResult.MapType(_propertySchema.AlternateType);
        }

        internal static string GetCanonicalTypeName(TypeSchema typeSchema)
        {
            if (typeSchema == null)
                return null;
            return typeSchema == ListSchema.Type ? "List" : typeSchema.Name;
        }
    }
}
