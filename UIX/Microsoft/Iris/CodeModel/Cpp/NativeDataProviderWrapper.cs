// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.CodeModel.Cpp.NativeDataProviderWrapper
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Markup;
using Microsoft.Iris.OS;
using System;

namespace Microsoft.Iris.CodeModel.Cpp
{
    internal class NativeDataProviderWrapper : IDataProvider
    {
        private string _name;
        private IntPtr _factory;

        public NativeDataProviderWrapper(string name, IntPtr factory)
        {
            _name = name;
            _factory = factory;
        }

        public string Name => _name;

        public MarkupDataQuery Build(MarkupDataQuerySchema querySchema) => new NativeMarkupDataQuery(querySchema, this);

        public IntPtr ConstructQuery(
          string providerName,
          ulong queryTypeHandle,
          ulong resultTypeHandle,
          ulong queryHandle)
        {
            IntPtr query;
            int num = (int)NativeApi.SpDataProviderConstructQuery(_factory, providerName, queryTypeHandle, resultTypeHandle, queryHandle, out query);
            return query;
        }

        public override string ToString() => string.Format("{0} ({1})", _name, _factory);
    }
}
