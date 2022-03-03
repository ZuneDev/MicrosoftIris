// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.CodeModel.Cpp.TypeSchemaHandleTable
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Markup;

namespace Microsoft.Iris.CodeModel.Cpp
{
    internal class TypeSchemaHandleTable : DllProxyHandleTable
    {
        public bool LookupByHandle(ulong handle, out TypeSchema obj)
        {
            object obj1;
            bool flag = LookupByHandleWorker(handle, out obj1);
            obj = (TypeSchema)obj1;
            return flag;
        }
    }
}
