// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.SepiaInstanceSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Animations;
using Microsoft.Iris.Drawing;
using Microsoft.Iris.Library;
using Microsoft.Iris.Session;
using Microsoft.Iris.UI;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class SepiaInstanceSchema
    {
        public static UIXTypeSchema Type;

        private static void SetLightColor(ref object instanceObj, object valueObj) => ((EffectElementWrapper)instanceObj).SetProperty("Color", (Color)valueObj);

        private static void SetDarkColor(ref object instanceObj, object valueObj) => ((EffectElementWrapper)instanceObj).SetProperty("Color", (Color)valueObj);

        private static void SetDesaturate(ref object instanceObj, object valueObj)
        {
            EffectElementWrapper effectElementWrapper = (EffectElementWrapper)instanceObj;
            float num = (float)valueObj;
            Result result = SingleSchema.ValidateNotNegative(valueObj);
            if (result.Failed)
                ErrorManager.ReportError(result.Error);
            else
                effectElementWrapper.SetProperty("Desaturate", num);
        }

        private static void SetTone(ref object instanceObj, object valueObj)
        {
            EffectElementWrapper effectElementWrapper = (EffectElementWrapper)instanceObj;
            float num = (float)valueObj;
            Result result = SingleSchema.ValidateNotNegative(valueObj);
            if (result.Failed)
                ErrorManager.ReportError(result.Error);
            else
                effectElementWrapper.SetProperty("Tone", num);
        }

        private static object CallPlayLightColorAnimationEffectColorAnimation(
          object instanceObj,
          object[] parameters)
        {
            ((EffectElementWrapper)instanceObj).PlayAnimation(EffectProperty.LightColor, (EffectAnimation)parameters[0]);
            return null;
        }

        private static object CallPlayDarkColorAnimationEffectColorAnimation(
          object instanceObj,
          object[] parameters)
        {
            ((EffectElementWrapper)instanceObj).PlayAnimation(EffectProperty.DarkColor, (EffectAnimation)parameters[0]);
            return null;
        }

        private static object CallPlayDesaturateAnimationEffectFloatAnimation(
          object instanceObj,
          object[] parameters)
        {
            ((EffectElementWrapper)instanceObj).PlayAnimation(EffectProperty.Desaturate, (EffectAnimation)parameters[0]);
            return null;
        }

        private static object CallPlayToneAnimationEffectFloatAnimation(
          object instanceObj,
          object[] parameters)
        {
            ((EffectElementWrapper)instanceObj).PlayAnimation(EffectProperty.Tone, (EffectAnimation)parameters[0]);
            return null;
        }

        public static void Pass1Initialize() => Type = new UIXTypeSchema(189, "SepiaInstance", null, 74, typeof(EffectElementWrapper), UIXTypeFlags.None);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(189, "LightColor", 35, -1, ExpressionRestriction.None, false, null, false, null, new SetValueHandler(SetLightColor), false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(189, "DarkColor", 35, -1, ExpressionRestriction.None, false, null, false, null, new SetValueHandler(SetDarkColor), false);
            UIXPropertySchema uixPropertySchema3 = new UIXPropertySchema(189, "Desaturate", 194, -1, ExpressionRestriction.None, false, SingleSchema.ValidateNotNegative, false, null, new SetValueHandler(SetDesaturate), false);
            UIXPropertySchema uixPropertySchema4 = new UIXPropertySchema(189, "Tone", 194, -1, ExpressionRestriction.None, false, SingleSchema.ValidateNotNegative, false, null, new SetValueHandler(SetTone), false);
            UIXMethodSchema uixMethodSchema1 = new UIXMethodSchema(189, "PlayLightColorAnimation", new short[1]
            {
         71
            }, 240, new InvokeHandler(CallPlayLightColorAnimationEffectColorAnimation), false);
            UIXMethodSchema uixMethodSchema2 = new UIXMethodSchema(189, "PlayDarkColorAnimation", new short[1]
            {
         71
            }, 240, new InvokeHandler(CallPlayDarkColorAnimationEffectColorAnimation), false);
            UIXMethodSchema uixMethodSchema3 = new UIXMethodSchema(189, "PlayDesaturateAnimation", new short[1]
            {
         75
            }, 240, new InvokeHandler(CallPlayDesaturateAnimationEffectFloatAnimation), false);
            UIXMethodSchema uixMethodSchema4 = new UIXMethodSchema(189, "PlayToneAnimation", new short[1]
            {
         75
            }, 240, new InvokeHandler(CallPlayToneAnimationEffectFloatAnimation), false);
            Type.Initialize(null, null, new PropertySchema[4]
            {
         uixPropertySchema2,
         uixPropertySchema3,
         uixPropertySchema1,
         uixPropertySchema4
            }, new MethodSchema[4]
            {
         uixMethodSchema1,
         uixMethodSchema2,
         uixMethodSchema3,
         uixMethodSchema4
            }, null, null, null, null, null, null, null, null);
        }
    }
}
