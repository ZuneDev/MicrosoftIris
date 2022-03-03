// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.KeyframeSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Animations;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class KeyframeSchema
    {
        public static UIXTypeSchema Type;

        private static object GetTime(object instanceObj) => ((BaseKeyframe)instanceObj).Time;

        private static void SetTime(ref object instanceObj, object valueObj) => ((BaseKeyframe)instanceObj).Time = (float)valueObj;

        private static object GetRelativeTo(object instanceObj) => ((BaseKeyframe)instanceObj).RelativeTo;

        private static void SetRelativeTo(ref object instanceObj, object valueObj) => ((BaseKeyframe)instanceObj).RelativeTo = (RelativeTo)valueObj;

        private static object GetInterpolation(object instanceObj) => ((BaseKeyframe)instanceObj).Interpolation;

        private static void SetInterpolation(ref object instanceObj, object valueObj) => ((BaseKeyframe)instanceObj).Interpolation = (Interpolation)valueObj;

        public static void Pass1Initialize() => Type = new UIXTypeSchema(130, "Keyframe", null, 153, typeof(BaseKeyframe), UIXTypeFlags.None);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(130, "Time", 194, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetTime), new SetValueHandler(SetTime), false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(130, "RelativeTo", 171, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetRelativeTo), new SetValueHandler(SetRelativeTo), false);
            UIXPropertySchema uixPropertySchema3 = new UIXPropertySchema(130, "Interpolation", 121, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetInterpolation), new SetValueHandler(SetInterpolation), false);
            Type.Initialize(null, null, new PropertySchema[3]
            {
         uixPropertySchema3,
         uixPropertySchema2,
         uixPropertySchema1
            }, null, null, null, null, null, null, null, null, null);
        }
    }
}
