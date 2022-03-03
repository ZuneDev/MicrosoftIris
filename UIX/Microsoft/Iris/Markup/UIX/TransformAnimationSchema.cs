// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.TransformAnimationSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Animations;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class TransformAnimationSchema
    {
        public static UIXTypeSchema Type;

        private static object GetDelay(object instanceObj) => ((TransformAnimation)instanceObj).Delay;

        private static void SetDelay(ref object instanceObj, object valueObj) => ((TransformAnimation)instanceObj).Delay = (float)valueObj;

        private static object GetFilter(object instanceObj) => ((TransformAnimation)instanceObj).Filter;

        private static void SetFilter(ref object instanceObj, object valueObj) => ((TransformAnimation)instanceObj).Filter = (KeyframeFilter)valueObj;

        private static object GetMagnitude(object instanceObj) => ((TransformAnimation)instanceObj).Magnitude;

        private static void SetMagnitude(ref object instanceObj, object valueObj) => ((TransformAnimation)instanceObj).Magnitude = (float)valueObj;

        private static object GetTimeScale(object instanceObj) => ((TransformAnimation)instanceObj).TimeScale;

        private static void SetTimeScale(ref object instanceObj, object valueObj) => ((TransformAnimation)instanceObj).TimeScale = (float)valueObj;

        private static object GetSource(object instanceObj) => ((ReferenceAnimation)instanceObj).Source;

        private static void SetSource(ref object instanceObj, object valueObj) => ((ReferenceAnimation)instanceObj).Source = (IAnimationProvider)valueObj;

        private static object GetType(object instanceObj) => ((ReferenceAnimation)instanceObj).Type;

        private static object Construct() => new TransformAnimation();

        public static void Pass1Initialize() => Type = new UIXTypeSchema(222, "TransformAnimation", null, 104, typeof(TransformAnimation), UIXTypeFlags.None);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(222, "Delay", 194, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetDelay), new SetValueHandler(SetDelay), false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(222, "Filter", 131, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetFilter), new SetValueHandler(SetFilter), false);
            UIXPropertySchema uixPropertySchema3 = new UIXPropertySchema(222, "Magnitude", 194, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetMagnitude), new SetValueHandler(SetMagnitude), false);
            UIXPropertySchema uixPropertySchema4 = new UIXPropertySchema(222, "TimeScale", 194, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetTimeScale), new SetValueHandler(SetTimeScale), false);
            UIXPropertySchema uixPropertySchema5 = new UIXPropertySchema(222, "Source", 104, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetSource), new SetValueHandler(SetSource), false);
            UIXPropertySchema uixPropertySchema6 = new UIXPropertySchema(222, "Type", 10, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetType), null, false);
            Type.Initialize(new DefaultConstructHandler(Construct), null, new PropertySchema[6]
            {
         uixPropertySchema1,
         uixPropertySchema2,
         uixPropertySchema3,
         uixPropertySchema5,
         uixPropertySchema4,
         uixPropertySchema6
            }, null, null, null, null, null, null, null, null, null);
        }
    }
}
