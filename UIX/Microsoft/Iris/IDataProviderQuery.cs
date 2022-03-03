// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.IDataProviderQuery
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Markup;

namespace Microsoft.Iris
{
    internal interface IDataProviderQuery : IDataProviderBaseObject
    {
        void DeclareOwner(object owner);

        void Dispose(object owner);

        void SetInternalObject(MarkupDataQuery internalQuery);

        void OnInitialize();

        object Result { get; set; }

        DataProviderQueryStatus Status { get; set; }

        bool Enabled { get; set; }

        void Refresh();
    }
}
