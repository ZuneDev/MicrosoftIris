// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.OrientationKeyframeSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Animations;
using Microsoft.Iris.Drawing;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class OrientationKeyframeSchema
    {
        public static UIXTypeSchema Type;

        private static object GetValue(object instanceObj) => ((BaseRotationKeyframe)instanceObj).Value;

        private static void SetValue(ref object instanceObj, object valueObj) => ((BaseRotationKeyframe)instanceObj).Value = (Rotation)valueObj;

        private static object Construct() => new OrientationKeyframe();

        public static void Pass1Initialize() => Type = new UIXTypeSchema(155, "OrientationKeyframe", null, 130, typeof(OrientationKeyframe), UIXTypeFlags.None);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema = new UIXPropertySchema(155, "Value", 176, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetValue), new SetValueHandler(SetValue), false);
            Type.Initialize(new DefaultConstructHandler(Construct), null, new PropertySchema[1]
            {
         uixPropertySchema
            }, null, null, null, null, null, null, null, null, null);
        }
    }
}
