// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.DestinationElementInstanceSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Animations;
using Microsoft.Iris.Render;
using Microsoft.Iris.UI;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class DestinationElementInstanceSchema
    {
        public static UIXTypeSchema Type;

        private static void SetDownsample(ref object instanceObj, object valueObj) => ((EffectElementWrapper)instanceObj).SetProperty("Downsample", (float)valueObj);

        private static void SetUVOffset(ref object instanceObj, object valueObj) => ((EffectElementWrapper)instanceObj).SetProperty("UVOffset", (Vector2)valueObj);

        private static object CallPlayDownsampleAnimationEffectFloatAnimation(
          object instanceObj,
          object[] parameters)
        {
            ((EffectElementWrapper)instanceObj).PlayAnimation(EffectProperty.Downsample, (EffectAnimation)parameters[0]);
            return null;
        }

        public static void Pass1Initialize() => Type = new UIXTypeSchema(57, "DestinationElementInstance", null, 74, typeof(EffectElementWrapper), UIXTypeFlags.None);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(57, "Downsample", 194, -1, ExpressionRestriction.None, false, null, false, null, new SetValueHandler(SetDownsample), false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(57, "UVOffset", 233, -1, ExpressionRestriction.None, false, null, false, null, new SetValueHandler(SetUVOffset), false);
            UIXMethodSchema uixMethodSchema = new UIXMethodSchema(57, "PlayDownsampleAnimation", new short[1]
            {
         75
            }, 240, new InvokeHandler(CallPlayDownsampleAnimationEffectFloatAnimation), false);
            Type.Initialize(null, null, new PropertySchema[2]
            {
         uixPropertySchema1,
         uixPropertySchema2
            }, new MethodSchema[1]
            {
         uixMethodSchema
            }, null, null, null, null, null, null, null, null);
        }
    }
}
