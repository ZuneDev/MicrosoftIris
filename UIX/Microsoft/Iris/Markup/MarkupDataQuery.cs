// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.MarkupDataQuery
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

namespace Microsoft.Iris.Markup
{
    internal abstract class MarkupDataQuery : MarkupDataTypeBaseObject
    {
        private MarkupDataQuerySchema _owner;

        public MarkupDataQuery(MarkupDataQuerySchema type)
          : base(type)
          => _owner = type;

        protected void ApplyDefaultValues()
        {
            Map map = null;
            for (MarkupDataQuerySchema owner = _owner; owner != null; owner = owner.Base as MarkupDataQuerySchema)
            {
                foreach (MarkupDataQueryPropertySchema property in owner.Properties)
                {
                    if (property.DefaultValue != null && (map == null || !map.ContainsKey(property.Name)))
                    {
                        object instance = this;
                        property.SetValue(ref instance, property.DefaultValue);
                        if (map == null)
                            map = new Map();
                        map[property.Name] = null;
                    }
                }
            }
        }

        public abstract void Refresh();

        public abstract object Result { get; set; }

        public abstract DataProviderQueryStatus Status { get; }

        public abstract bool Enabled { get; set; }
    }
}
