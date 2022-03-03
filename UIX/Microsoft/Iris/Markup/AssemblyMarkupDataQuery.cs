// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.AssemblyMarkupDataQuery
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System;

namespace Microsoft.Iris.Markup
{
    internal class AssemblyMarkupDataQuery : MarkupDataQuery
    {
        private IDataProviderQuery _externalQuery;

        public AssemblyMarkupDataQuery(MarkupDataQuerySchema type, AssemblyDataProviderWrapper provider)
          : base(type)
        {
            _externalQuery = provider.ConstructQuery(type);
            _externalQuery.DeclareOwner(this);
            _externalQuery.SetInternalObject(this);
            ApplyDefaultValues();
        }

        public override void NotifyInitialized()
        {
            base.NotifyInitialized();
            if (_externalQuery == null)
                return;
            _externalQuery.OnInitialize();
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            if (_externalQuery == null)
                return;
            _externalQuery.Dispose(this);
        }

        public override void Refresh() => _externalQuery.Refresh();

        public override object Result
        {
            get => AssemblyLoadResult.WrapObject(_externalQuery.Result);
            set => _externalQuery.Result = AssemblyLoadResult.UnwrapObject(value);
        }

        public override DataProviderQueryStatus Status => _externalQuery.Status;

        public override bool Enabled
        {
            get => _externalQuery.Enabled;
            set => _externalQuery.Enabled = value;
        }

        protected override bool ExternalObjectGetProperty(string propertyName, out object value)
        {
            object property = _externalQuery.GetProperty(propertyName);
            value = AssemblyLoadResult.WrapObject(property);
            return true;
        }

        protected override bool ExternalObjectSetProperty(string propertyName, object value)
        {
            object obj = AssemblyLoadResult.UnwrapObject(value);
            _externalQuery.SetProperty(propertyName, obj);
            return true;
        }

        protected override IDataProviderBaseObject ExternalAssemblyObject => _externalQuery;

        public override IntPtr ExternalNativeObject => IntPtr.Zero;
    }
}
