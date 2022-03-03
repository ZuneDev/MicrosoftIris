// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIXConstructorSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

namespace Microsoft.Iris.Markup
{
    internal class UIXConstructorSchema : ConstructorSchema
    {
        private TypeSchema[] _parameterTypes;
        private ConstructHandler _construct;

        public UIXConstructorSchema(
          short ownerTypeID,
          short[] parameterTypeIDs,
          ConstructHandler construct)
          : base(UIXTypes.MapIDToType(ownerTypeID))
        {
            _parameterTypes = UIXTypes.MapIDsToTypes(parameterTypeIDs);
            _construct = construct;
        }

        public override TypeSchema[] ParameterTypes => _parameterTypes;

        public override object Construct(object[] parameters) => _construct(parameters);
    }
}
