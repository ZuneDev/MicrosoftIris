// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.NativeMarkupDataQuery
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.CodeModel.Cpp;
using Microsoft.Iris.OS;
using System;

namespace Microsoft.Iris.Markup
{
    internal class NativeMarkupDataQuery : MarkupDataQuery
    {
        private IntPtr _externalQuery;
        private ulong _handleToMe;
        private ulong _typeHandle;
        private ulong _resultTypeHandle;
        private static MarkupDataQueryHandleTable s_handleTable;

        public static void InitializeStatics() => s_handleTable = new MarkupDataQueryHandleTable();

        public NativeMarkupDataQuery(MarkupDataQuerySchema type, NativeDataProviderWrapper provider)
          : base(type)
        {
            _handleToMe = s_handleTable.RegisterProxy(this);
            _typeHandle = type.UniqueId;
            _resultTypeHandle = type.ResultType.UniqueId;
            _externalQuery = provider.ConstructQuery(type.ProviderName, _typeHandle, _resultTypeHandle, _handleToMe);
            ApplyDefaultValues();
        }

        public override void NotifyInitialized()
        {
            base.NotifyInitialized();
            int num = (int)NativeApi.SpDataQueryNotifyInitialized(_externalQuery);
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            s_handleTable.ReleaseProxy(_handleToMe);
            int num = (int)NativeApi.SpDataBaseObjectSetInternalHandle(_externalQuery, 0UL);
            NativeApi.SpReleaseExternalObject(_externalQuery);
        }

        public override void Refresh()
        {
            int num = (int)NativeApi.SpDataQueryRefresh(_externalQuery);
        }

        public override unsafe object Result
        {
            get
            {
                UIXVariant propertyValue;
                int resultProperty = (int)NativeApi.SpDataQueryGetResultProperty(_externalQuery, out propertyValue);
                return UIXVariant.GetValue(propertyValue, TypeSchema.Owner);
            }
            set
            {
                UIXVariant* uixVariantPtr = stackalloc UIXVariant[sizeof(UIXVariant)];
                UIXVariant.MarshalObject(value, uixVariantPtr);
                int num = (int)NativeApi.SpDataQuerySetResultProperty(_externalQuery, uixVariantPtr);
            }
        }

        public override DataProviderQueryStatus Status
        {
            get
            {
                DataProviderQueryStatus propertyValue;
                int statusProperty = (int)NativeApi.SpDataQueryGetStatusProperty(_externalQuery, out propertyValue);
                return propertyValue;
            }
        }

        public override bool Enabled
        {
            get
            {
                bool propertyValue;
                int enabledProperty = (int)NativeApi.SpDataQueryGetEnabledProperty(_externalQuery, out propertyValue);
                return propertyValue;
            }
            set
            {
                int num = (int)NativeApi.SpDataQuerySetEnabledProperty(_externalQuery, value);
            }
        }

        protected override bool ExternalObjectGetProperty(string propertyName, out object value)
        {
            UIXVariant propertyValue;
            int property = (int)NativeApi.SpDataBaseObjectGetProperty(_externalQuery, propertyName, out propertyValue);
            TypeSchema.FindPropertyDeep(propertyName);
            value = UIXVariant.GetValue(propertyValue, TypeSchema.Owner);
            return true;
        }

        protected override unsafe bool ExternalObjectSetProperty(string propertyName, object value)
        {
            // ISSUE: untyped stack allocation
            UIXVariant* uixVariantPtr = stackalloc UIXVariant[sizeof(UIXVariant)];
            UIXVariant.MarshalObject(value, uixVariantPtr);
            int num = (int)NativeApi.SpDataBaseObjectSetProperty(_externalQuery, propertyName, uixVariantPtr);
            return true;
        }

        protected override IDataProviderBaseObject ExternalAssemblyObject => (IDataProviderBaseObject)null;

        public override IntPtr ExternalNativeObject => _externalQuery;

        public ulong Handle => _handleToMe;

        public static MarkupDataQuery LookupByHandle(ulong handle)
        {
            MarkupDataQuery markupDataQuery;
            s_handleTable.LookupByHandle(handle, out markupDataQuery);
            return markupDataQuery;
        }
    }
}
