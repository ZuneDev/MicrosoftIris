// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIXEnumSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System;

namespace Microsoft.Iris.Markup
{
    internal class UIXEnumSchema : EnumSchema
    {
        private short _typeID;

        public UIXEnumSchema(short typeID, string name, Type runtimeType, bool isFlags)
          : base(MarkupSystem.UIXGlobal)
        {
            _typeID = typeID;
            Initialize(name, runtimeType, isFlags, null, null);
            UIXTypes.RegisterTypeForID(typeID, this);
        }

        protected override void InitializeNameToValueMap() => _nameToValueMap = UIXEnumData.GetDataForType(_typeID);

        public override string ToString() => Name;
    }
}
