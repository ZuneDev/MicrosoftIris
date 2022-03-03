// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.AssemblyMarkupDataType
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System;

namespace Microsoft.Iris.Markup
{
    internal class AssemblyMarkupDataType : MarkupDataType
    {
        private IDataProviderObject _externalObject;

        public AssemblyMarkupDataType(MarkupDataTypeSchema type, IDataProviderObject externalObject)
          : base(type)
          => _externalObject = externalObject;

        protected override bool ExternalObjectGetProperty(string propertyName, out object value)
        {
            object property = _externalObject.GetProperty(propertyName);
            value = AssemblyLoadResult.WrapObject(property);
            return true;
        }

        protected override bool ExternalObjectSetProperty(string propertyName, object value)
        {
            object obj = AssemblyLoadResult.UnwrapObject(value);
            _externalObject.SetProperty(propertyName, obj);
            return true;
        }

        protected override IDataProviderBaseObject ExternalAssemblyObject => _externalObject;

        public override IntPtr ExternalNativeObject => IntPtr.Zero;
    }
}
