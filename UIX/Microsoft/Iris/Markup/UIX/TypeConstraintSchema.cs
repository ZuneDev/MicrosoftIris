// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.TypeConstraintSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

namespace Microsoft.Iris.Markup.UIX
{
    internal static class TypeConstraintSchema
    {
        public static UIXTypeSchema Type;

        private static object GetType(object instanceObj) => ((TypeConstraint)instanceObj).Type;

        private static void SetType(ref object instanceObj, object valueObj) => ((TypeConstraint)instanceObj).Type = (TypeSchema)valueObj;

        private static object GetConstraint(object instanceObj) => ((TypeConstraint)instanceObj).Constraint;

        private static void SetConstraint(ref object instanceObj, object valueObj) => ((TypeConstraint)instanceObj).Constraint = (TypeSchema)valueObj;

        private static object Construct() => new TypeConstraint();

        public static void Pass1Initialize() => Type = new UIXTypeSchema(226, "TypeConstraint", null, 153, typeof(TypeConstraint), UIXTypeFlags.Immutable);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(226, "Type", 225, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetType), new SetValueHandler(SetType), false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(226, "Constraint", 225, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetConstraint), new SetValueHandler(SetConstraint), false);
            Type.Initialize(new DefaultConstructHandler(Construct), null, new PropertySchema[2]
            {
         uixPropertySchema2,
         uixPropertySchema1
            }, null, null, null, null, null, null, null, null, null);
        }
    }
}
