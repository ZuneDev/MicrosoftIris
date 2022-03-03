// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.TransformByAttributeAnimationSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Animations;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class TransformByAttributeAnimationSchema
    {
        public static UIXTypeSchema Type;

        private static object GetAttribute(object instanceObj) => ((TransformByAttributeAnimation)instanceObj).Attribute;

        private static void SetAttribute(ref object instanceObj, object valueObj) => ((TransformByAttributeAnimation)instanceObj).Attribute = (TransformAttribute)valueObj;

        private static object GetMaxTimeScale(object instanceObj) => ((TransformByAttributeAnimation)instanceObj).MaxTimeScale;

        private static void SetMaxTimeScale(ref object instanceObj, object valueObj) => ((TransformByAttributeAnimation)instanceObj).MaxTimeScale = (float)valueObj;

        private static object GetMaxDelay(object instanceObj) => ((TransformByAttributeAnimation)instanceObj).MaxDelay;

        private static void SetMaxDelay(ref object instanceObj, object valueObj) => ((TransformByAttributeAnimation)instanceObj).MaxDelay = (float)valueObj;

        private static object GetMaxMagnitude(object instanceObj) => ((TransformByAttributeAnimation)instanceObj).MaxMagnitude;

        private static void SetMaxMagnitude(ref object instanceObj, object valueObj) => ((TransformByAttributeAnimation)instanceObj).MaxMagnitude = (float)valueObj;

        private static object GetOverride(object instanceObj) => ((TransformByAttributeAnimation)instanceObj).Override;

        private static void SetOverride(ref object instanceObj, object valueObj) => ((TransformByAttributeAnimation)instanceObj).Override = (float)valueObj;

        private static object GetValueTransformer(object instanceObj) => ((TransformByAttributeAnimation)instanceObj).ValueTransformer;

        private static void SetValueTransformer(ref object instanceObj, object valueObj) => ((TransformByAttributeAnimation)instanceObj).ValueTransformer = (ValueTransformer)valueObj;

        private static object Construct() => new TransformByAttributeAnimation();

        public static void Pass1Initialize() => Type = new UIXTypeSchema(224, "TransformByAttributeAnimation", null, 222, typeof(TransformByAttributeAnimation), UIXTypeFlags.None);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(224, "Attribute", 223, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetAttribute), new SetValueHandler(SetAttribute), false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(224, "MaxTimeScale", 194, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetMaxTimeScale), new SetValueHandler(SetMaxTimeScale), false);
            UIXPropertySchema uixPropertySchema3 = new UIXPropertySchema(224, "MaxDelay", 194, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetMaxDelay), new SetValueHandler(SetMaxDelay), false);
            UIXPropertySchema uixPropertySchema4 = new UIXPropertySchema(224, "MaxMagnitude", 194, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetMaxMagnitude), new SetValueHandler(SetMaxMagnitude), false);
            UIXPropertySchema uixPropertySchema5 = new UIXPropertySchema(224, "Override", 194, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetOverride), new SetValueHandler(SetOverride), false);
            UIXPropertySchema uixPropertySchema6 = new UIXPropertySchema(224, "ValueTransformer", 232, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetValueTransformer), new SetValueHandler(SetValueTransformer), false);
            Type.Initialize(new DefaultConstructHandler(Construct), null, new PropertySchema[6]
            {
         uixPropertySchema1,
         uixPropertySchema3,
         uixPropertySchema4,
         uixPropertySchema2,
         uixPropertySchema5,
         uixPropertySchema6
            }, null, null, null, null, null, null, null, null, null);
        }
    }
}
