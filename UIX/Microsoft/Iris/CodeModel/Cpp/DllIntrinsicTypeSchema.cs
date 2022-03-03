// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.CodeModel.Cpp.DllIntrinsicTypeSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;
using Microsoft.Iris.Markup;
using Microsoft.Iris.Markup.UIX;

namespace Microsoft.Iris.CodeModel.Cpp
{
    internal class DllIntrinsicTypeSchema : DllTypeSchemaBase
    {
        public DllIntrinsicTypeSchema(DllLoadResult owner, uint ID, TypeSchema equivalentType)
          : base(owner, ID)
        {
            _baseType = ObjectSchema.Type;
            _name = InvariantString.Format("<DLL Intrinsic> {0}", equivalentType.Name);
            _marshalAs = ID;
            RegisterOneWayEquivalence(this, equivalentType);
        }
    }
}
