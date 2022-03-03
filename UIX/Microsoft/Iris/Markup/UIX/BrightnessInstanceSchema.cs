// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.BrightnessInstanceSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Animations;
using Microsoft.Iris.UI;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class BrightnessInstanceSchema
    {
        public static UIXTypeSchema Type;

        private static void SetBrightness(ref object instanceObj, object valueObj) => ((EffectElementWrapper)instanceObj).SetProperty("Brightness", (float)valueObj);

        private static object CallPlayBrightnessAnimationEffectFloatAnimation(
          object instanceObj,
          object[] parameters)
        {
            ((EffectElementWrapper)instanceObj).PlayAnimation(EffectProperty.Brightness, (EffectAnimation)parameters[0]);
            return null;
        }

        public static void Pass1Initialize() => Type = new UIXTypeSchema(18, "BrightnessInstance", null, 74, typeof(EffectElementWrapper), UIXTypeFlags.None);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema = new UIXPropertySchema(18, "Brightness", 194, -1, ExpressionRestriction.None, false, null, false, null, new SetValueHandler(SetBrightness), false);
            UIXMethodSchema uixMethodSchema = new UIXMethodSchema(18, "PlayBrightnessAnimation", new short[1]
            {
         75
            }, 240, new InvokeHandler(CallPlayBrightnessAnimationEffectFloatAnimation), false);
            Type.Initialize(null, null, new PropertySchema[1]
            {
         uixPropertySchema
            }, new MethodSchema[1]
            {
         uixMethodSchema
            }, null, null, null, null, null, null, null, null);
        }
    }
}
