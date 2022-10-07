// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.MarkupDataMappingEntry
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

namespace Microsoft.Iris.Markup
{
    public class MarkupDataMappingEntry
    {
        private string _source;
        private string _target;
        private MarkupDataTypePropertySchema _property;
        private object _defaultValue;

        public string Source
        {
            get => _source;
            set => _source = value;
        }

        public string Target
        {
            get => _target;
            set => _target = value;
        }

        public MarkupDataTypePropertySchema Property
        {
            get => _property;
            set => _property = value;
        }

        public object DefaultValue
        {
            get => _defaultValue;
            set => _defaultValue = value;
        }
    }
}
