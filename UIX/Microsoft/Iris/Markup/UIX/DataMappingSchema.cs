// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.DataMappingSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

namespace Microsoft.Iris.Markup.UIX
{
    internal static class DataMappingSchema
    {
        public static UIXTypeSchema Type;

        private static void SetTargetType(ref object instanceObj, object valueObj)
        {
        }

        private static void SetProvider(ref object instanceObj, object valueObj)
        {
        }

        private static object GetMappings(object instanceObj) => (object)null;

        public static void Pass1Initialize() => Type = new UIXTypeSchema(45, "DataMapping", null, -1, typeof(object), UIXTypeFlags.None);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(45, "TargetType", 208, -1, ExpressionRestriction.NoAccess, false, null, true, null, new SetValueHandler(SetTargetType), false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(45, "Provider", 208, -1, ExpressionRestriction.NoAccess, false, null, true, null, new SetValueHandler(SetProvider), false);
            UIXPropertySchema uixPropertySchema3 = new UIXPropertySchema(45, "Mappings", 138, 140, ExpressionRestriction.NoAccess, false, null, true, new GetValueHandler(GetMappings), null, false);
            Type.Initialize(null, null, new PropertySchema[3]
            {
         uixPropertySchema3,
         uixPropertySchema2,
         uixPropertySchema1
            }, null, null, null, null, null, null, null, null, null);
        }
    }
}
