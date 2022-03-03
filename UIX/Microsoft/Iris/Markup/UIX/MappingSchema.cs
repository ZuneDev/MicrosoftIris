// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.MappingSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

namespace Microsoft.Iris.Markup.UIX
{
    internal static class MappingSchema
    {
        public static UIXTypeSchema Type;

        private static void SetProperty(ref object instanceObj, object valueObj)
        {
        }

        private static void SetSource(ref object instanceObj, object valueObj)
        {
        }

        private static void SetTarget(ref object instanceObj, object valueObj)
        {
        }

        private static void SetDefaultValue(ref object instanceObj, object valueObj)
        {
        }

        private static object Construct() => new object();

        public static void Pass1Initialize() => Type = new UIXTypeSchema(140, "Mapping", null, -1, typeof(object), UIXTypeFlags.None);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(140, "Property", 208, -1, ExpressionRestriction.NoAccess, false, null, true, null, new SetValueHandler(SetProperty), false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(140, "Source", 208, -1, ExpressionRestriction.NoAccess, false, null, true, null, new SetValueHandler(SetSource), false);
            UIXPropertySchema uixPropertySchema3 = new UIXPropertySchema(140, "Target", 208, -1, ExpressionRestriction.NoAccess, false, null, true, null, new SetValueHandler(SetTarget), false);
            UIXPropertySchema uixPropertySchema4 = new UIXPropertySchema(140, "DefaultValue", 208, -1, ExpressionRestriction.NoAccess, false, null, true, null, new SetValueHandler(SetDefaultValue), false);
            Type.Initialize(new DefaultConstructHandler(Construct), null, new PropertySchema[4]
            {
         uixPropertySchema4,
         uixPropertySchema1,
         uixPropertySchema2,
         uixPropertySchema3
            }, null, null, null, null, null, null, null, null, null);
        }
    }
}
