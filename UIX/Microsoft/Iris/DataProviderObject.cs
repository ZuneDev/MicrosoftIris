// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.DataProviderObject
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Markup;
using System;
using System.Collections.Generic;

namespace Microsoft.Iris
{
    public abstract class DataProviderObject :
      IDataProviderObject,
      IDataProviderBaseObject,
      AssemblyObjectProxyHelper.IFrameworkProxyObject
    {
        private DataProviderQuery _owner;
        private MarkupDataTypeSchema _typeSchema;
        private MarkupDataType _internalObject;
        private Dictionary<string, DataProviderMapping> _mappings;
        private static object SynchronizedFindDataMappings = new object();

        public abstract object GetProperty(string propertyName);

        public abstract void SetProperty(string propertyName, object value);

        protected DataProviderObject(DataProviderQuery owner, object typeCookie)
        {
            _owner = owner != null ? owner : throw new ArgumentNullException(nameof(owner));
            _typeSchema = typeCookie as MarkupDataTypeSchema;
            if (_typeSchema != null)
                return;
            if (typeCookie is TypeSchema)
                throw new ArgumentException(((TypeSchema)typeCookie).Name + " is not a valid type for a DataProviderObject", nameof(typeCookie));
            if (typeCookie == null)
                throw new ArgumentNullException(nameof(typeCookie));
            throw new ArgumentException("typeCookie must be a TypeCookie from a DataProviderMapping", nameof(typeCookie));
        }

        internal DataProviderObject(MarkupDataTypeSchema typeSchema) => _typeSchema = typeSchema;

        public DataProviderQuery Owner => _owner;

        public string TypeName => _typeSchema.Name;

        protected IDictionary<string, DataProviderMapping> Mappings
        {
            get
            {
                if (_mappings == null)
                {
                    lock (SynchronizedFindDataMappings)
                    {
                        MarkupDataMapping dataMapping = MarkupDataProvider.FindDataMapping(_owner != null ? _owner.ProviderName : string.Empty, _typeSchema);
                        if (dataMapping.AssemblyDataProviderCookie == null)
                        {
                            Dictionary<string, DataProviderMapping> dictionary = new Dictionary<string, DataProviderMapping>(dataMapping.Mappings.Length);
                            foreach (MarkupDataMappingEntry mapping in dataMapping.Mappings)
                                dictionary[mapping.Property.Name] = new DataProviderMapping(mapping.Property, mapping.Source, mapping.Target, AssemblyLoadResult.UnwrapObject(mapping.DefaultValue));
                            dataMapping.AssemblyDataProviderCookie = dictionary;
                        }
                        _mappings = (Dictionary<string, DataProviderMapping>)dataMapping.AssemblyDataProviderCookie;
                    }
                }
                return _mappings;
            }
        }

        protected void FirePropertyChanged(string propertyName)
        {
            OnPropertyChanged(propertyName);
            if (_internalObject == null)
                return;
            _internalObject.FireNotificationThreadSafe(propertyName);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
        }

        object AssemblyObjectProxyHelper.IFrameworkProxyObject.FrameworkObject
        {
            get
            {
                if (_internalObject == null)
                    _internalObject = new AssemblyMarkupDataType(_typeSchema, this);
                return _internalObject;
            }
        }

        public override string ToString() => _typeSchema == null ? base.ToString() : _typeSchema.Name;
    }
}
