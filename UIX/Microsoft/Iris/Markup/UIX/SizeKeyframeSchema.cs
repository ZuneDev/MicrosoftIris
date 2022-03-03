// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.SizeKeyframeSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Animations;
using Microsoft.Iris.Render;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class SizeKeyframeSchema
    {
        public static UIXTypeSchema Type;

        private static object GetValue(object instanceObj) => ((BaseVector2Keyframe)instanceObj).Value;

        private static void SetValue(ref object instanceObj, object valueObj) => ((BaseVector2Keyframe)instanceObj).Value = (Vector2)valueObj;

        private static object Construct() => new SizeKeyframe();

        public static void Pass1Initialize() => Type = new UIXTypeSchema(196, "SizeKeyframe", null, 130, typeof(SizeKeyframe), UIXTypeFlags.None);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema = new UIXPropertySchema(196, "Value", 233, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetValue), new SetValueHandler(SetValue), false);
            Type.Initialize(new DefaultConstructHandler(Construct), null, new PropertySchema[1]
            {
         uixPropertySchema
            }, null, null, null, null, null, null, null, null, null);
        }
    }
}
