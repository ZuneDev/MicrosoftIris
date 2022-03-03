// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.ProviderlessDataProviderObject
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Markup;

namespace Microsoft.Iris
{
    internal class ProviderlessDataProviderObject : DataProviderObject
    {
        private MarkupDataType _internalObject;

        public ProviderlessDataProviderObject(
          MarkupDataType internalObject,
          MarkupDataTypeSchema typeSchema)
          : base(typeSchema)
        {
            _internalObject = internalObject;
        }

        public override object GetProperty(string propertyName)
        {
            propertyName = NotifyService.CanonicalizeString(propertyName);
            return AssemblyLoadResult.UnwrapObject(_internalObject.GetProperty(propertyName));
        }

        public override void SetProperty(string propertyName, object value)
        {
            propertyName = NotifyService.CanonicalizeString(propertyName);
            _internalObject.SetProperty(propertyName, AssemblyLoadResult.WrapObject(value));
        }
    }
}
