// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.ProviderlessMarkupDataType
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System;

namespace Microsoft.Iris.Markup
{
    internal class ProviderlessMarkupDataType : MarkupDataType
    {
        private IDataProviderObject _externalAssemblyObject;

        public ProviderlessMarkupDataType(MarkupDataTypeSchema typeSchema)
          : base(typeSchema)
        {
        }

        protected override IDataProviderBaseObject ExternalAssemblyObject
        {
            get
            {
                if (_externalAssemblyObject == null)
                    _externalAssemblyObject = new ProviderlessDataProviderObject(this, (MarkupDataTypeSchema)TypeSchema);
                return _externalAssemblyObject;
            }
        }

        public override IntPtr ExternalNativeObject => IntPtr.Zero;

        public override object ReadSymbol(SymbolReference symbolRef)
        {
            lock (SynchronizedPropertyStorage)
                return base.ReadSymbol(symbolRef);
        }

        public override void WriteSymbol(SymbolReference symbolRef, object value)
        {
            lock (SynchronizedPropertyStorage)
                base.WriteSymbol(symbolRef, value);
        }

        public override object GetProperty(string name)
        {
            lock (SynchronizedPropertyStorage)
                return base.GetProperty(name);
        }

        public override void SetProperty(string name, object value)
        {
            lock (SynchronizedPropertyStorage)
                base.SetProperty(name, value);
        }

        private object SynchronizedPropertyStorage => _storage;
    }
}
