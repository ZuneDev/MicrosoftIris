// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.MarkupDataMapping
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

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

        public override string ToString() => string.Format("({0}, {1})", _provider, _targetType);
    }
}
