// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.DataTypeSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

namespace Microsoft.Iris.Markup.UIX
{
    internal static class DataTypeSchema
    {
        public static UIXTypeSchema Type;

        private static void SetProvider(ref object instanceObj, object valueObj)
        {
            MarkupDataType markupDataType = (MarkupDataType)instanceObj;
        }

        public static void Pass1Initialize() => Type = new UIXTypeSchema(48, "DataType", null, 29, typeof(MarkupDataType), UIXTypeFlags.Disposable);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema = new UIXPropertySchema(48, "Provider", 208, -1, ExpressionRestriction.NoAccess, false, null, true, null, new SetValueHandler(SetProvider), false);
            Type.Initialize(null, null, new PropertySchema[1]
            {
         uixPropertySchema
            }, null, null, null, null, null, null, null, null, null);
        }
    }
}
