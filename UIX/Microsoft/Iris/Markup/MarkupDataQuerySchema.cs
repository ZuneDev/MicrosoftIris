// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.MarkupDataQuerySchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;
using Microsoft.Iris.Markup.UIX;
using Microsoft.Iris.Session;
using System;

namespace Microsoft.Iris.Markup
{
    internal class MarkupDataQuerySchema : ClassTypeSchema
    {
        private TypeSchema _resultType;
        private string _providerName;
        private MarkupDataQueryPreDefinedPropertySchema _resultProperty;
        private MarkupDataQueryPreDefinedPropertySchema[] _predefinedProperties;
        private MarkupDataQueryRefreshMethodSchema _refreshMethod;

        public MarkupDataQuerySchema(MarkupLoadResult owner, string name)
          : base(owner, name)
        {
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            if (_predefinedProperties != null)
            {
                foreach (DisposableObject predefinedProperty in _predefinedProperties)
                    predefinedProperty.Dispose(this);
            }
            if (_refreshMethod == null)
                return;
            _refreshMethod.Dispose(this);
        }

        public override void BuildProperties()
        {
            base.BuildProperties();
            _predefinedProperties = new MarkupDataQueryPreDefinedPropertySchema[3]
            {
        _resultProperty = new MarkupDataQueryPreDefinedPropertySchema(this, _resultType != null ? _resultType :  ObjectSchema.Type, "Result", new GetValueHandler(GetResultProperty),  null),
        new MarkupDataQueryPreDefinedPropertySchema(this, UIXLoadResultExports.DataQueryStatusType, "Status", new GetValueHandler(GetStatusProperty),  null),
        new MarkupDataQueryPreDefinedPropertySchema(this,  BooleanSchema.Type, "Enabled", new GetValueHandler(GetEnabledProperty), new SetValueHandler(SetEnabledProperty))
            };
            _refreshMethod = new MarkupDataQueryRefreshMethodSchema(this);
        }

        public override MarkupType MarkupType => MarkupType.DataQuery;

        protected override TypeSchema DefaultBase => MarkupDataQueryInstanceSchema.Type;

        public override Type RuntimeType => typeof(MarkupDataQuery);

        public override object ConstructDefault()
        {
            IDataProvider dataProvider = MarkupDataProvider.GetDataProvider(_providerName);
            if (dataProvider != null)
                return dataProvider.Build(this);
            ErrorManager.ReportError("Could not find provider '{0}'; verify that it has been registered", _providerName);
            return null;
        }

        public override PropertySchema FindProperty(string name)
        {
            foreach (PropertySchema predefinedProperty in _predefinedProperties)
            {
                if (name == predefinedProperty.Name)
                    return predefinedProperty;
            }
            return base.FindProperty(name);
        }

        public override MethodSchema FindMethod(string name, TypeSchema[] parameters) => _refreshMethod.Name == name && parameters.Length == 0 ? _refreshMethod : base.FindMethod(name, parameters);

        public string ProviderName
        {
            get => _providerName;
            set => _providerName = value;
        }

        public TypeSchema ResultType
        {
            get => _resultType;
            set => _resultType = value;
        }

        public bool InvalidatesQuery(string propertyName) => FindPropertyDeep(propertyName) is MarkupDataQueryPropertySchema propertyDeep && propertyDeep.InvalidatesQuery;

        private static object GetResultProperty(object instance) => ((MarkupDataQuery)instance).Result;

        private static object GetStatusProperty(object instance) => ((MarkupDataQuery)instance).Status;

        private static object GetEnabledProperty(object instance) => ((MarkupDataQuery)instance).Enabled;

        private static void SetEnabledProperty(ref object instance, object value) => ((MarkupDataQuery)instance).Enabled = (bool)value;
    }
}
