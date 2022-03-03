// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.LightShaftInstanceSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Animations;
using Microsoft.Iris.Render;
using Microsoft.Iris.UI;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class LightShaftInstanceSchema
    {
        public static UIXTypeSchema Type;

        private static void SetPosition(ref object instanceObj, object valueObj) => ((EffectElementWrapper)instanceObj).SetProperty("Position", (Vector3)valueObj);

        private static void SetDecay(ref object instanceObj, object valueObj) => ((EffectElementWrapper)instanceObj).SetProperty("Decay", (float)valueObj);

        private static void SetDensity(ref object instanceObj, object valueObj) => ((EffectElementWrapper)instanceObj).SetProperty("Density", (float)valueObj);

        private static void SetFallOff(ref object instanceObj, object valueObj) => ((EffectElementWrapper)instanceObj).SetProperty("FallOff", (float)valueObj);

        private static void SetIntensity(ref object instanceObj, object valueObj) => ((EffectElementWrapper)instanceObj).SetProperty("Intensity", (float)valueObj);

        private static void SetWeight(ref object instanceObj, object valueObj) => ((EffectElementWrapper)instanceObj).SetProperty("Weight", (float)valueObj);

        private static object CallPlayPositionAnimationEffectVector3Animation(
          object instanceObj,
          object[] parameters)
        {
            ((EffectElementWrapper)instanceObj).PlayAnimation(EffectProperty.Position, (EffectAnimation)parameters[0]);
            return null;
        }

        private static object CallPlayDecayAnimationEffectFloatAnimation(
          object instanceObj,
          object[] parameters)
        {
            ((EffectElementWrapper)instanceObj).PlayAnimation(EffectProperty.Decay, (EffectAnimation)parameters[0]);
            return null;
        }

        private static object CallPlayDensityAnimationEffectFloatAnimation(
          object instanceObj,
          object[] parameters)
        {
            ((EffectElementWrapper)instanceObj).PlayAnimation(EffectProperty.Density, (EffectAnimation)parameters[0]);
            return null;
        }

        private static object CallPlayIntensityAnimationEffectFloatAnimation(
          object instanceObj,
          object[] parameters)
        {
            ((EffectElementWrapper)instanceObj).PlayAnimation(EffectProperty.Intensity, (EffectAnimation)parameters[0]);
            return null;
        }

        private static object CallPlayFallOffAnimationEffectFloatAnimation(
          object instanceObj,
          object[] parameters)
        {
            ((EffectElementWrapper)instanceObj).PlayAnimation(EffectProperty.FallOff, (EffectAnimation)parameters[0]);
            return null;
        }

        private static object CallPlayWeightAnimationEffectFloatAnimation(
          object instanceObj,
          object[] parameters)
        {
            ((EffectElementWrapper)instanceObj).PlayAnimation(EffectProperty.Weight, (EffectAnimation)parameters[0]);
            return null;
        }

        public static void Pass1Initialize() => Type = new UIXTypeSchema(136, "LightShaftInstance", null, 74, typeof(EffectElementWrapper), UIXTypeFlags.None);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(136, "Position", 234, -1, ExpressionRestriction.None, false, null, false, null, new SetValueHandler(SetPosition), false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(136, "Decay", 194, -1, ExpressionRestriction.None, false, null, false, null, new SetValueHandler(SetDecay), false);
            UIXPropertySchema uixPropertySchema3 = new UIXPropertySchema(136, "Density", 194, -1, ExpressionRestriction.None, false, null, false, null, new SetValueHandler(SetDensity), false);
            UIXPropertySchema uixPropertySchema4 = new UIXPropertySchema(136, "FallOff", 194, -1, ExpressionRestriction.None, false, null, false, null, new SetValueHandler(SetFallOff), false);
            UIXPropertySchema uixPropertySchema5 = new UIXPropertySchema(136, "Intensity", 194, -1, ExpressionRestriction.None, false, null, false, null, new SetValueHandler(SetIntensity), false);
            UIXPropertySchema uixPropertySchema6 = new UIXPropertySchema(136, "Weight", 194, -1, ExpressionRestriction.None, false, null, false, null, new SetValueHandler(SetWeight), false);
            UIXMethodSchema uixMethodSchema1 = new UIXMethodSchema(136, "PlayPositionAnimation", new short[1]
            {
         81
            }, 240, new InvokeHandler(CallPlayPositionAnimationEffectVector3Animation), false);
            UIXMethodSchema uixMethodSchema2 = new UIXMethodSchema(136, "PlayDecayAnimation", new short[1]
            {
         75
            }, 240, new InvokeHandler(CallPlayDecayAnimationEffectFloatAnimation), false);
            UIXMethodSchema uixMethodSchema3 = new UIXMethodSchema(136, "PlayDensityAnimation", new short[1]
            {
         75
            }, 240, new InvokeHandler(CallPlayDensityAnimationEffectFloatAnimation), false);
            UIXMethodSchema uixMethodSchema4 = new UIXMethodSchema(136, "PlayIntensityAnimation", new short[1]
            {
         75
            }, 240, new InvokeHandler(CallPlayIntensityAnimationEffectFloatAnimation), false);
            UIXMethodSchema uixMethodSchema5 = new UIXMethodSchema(136, "PlayFallOffAnimation", new short[1]
            {
         75
            }, 240, new InvokeHandler(CallPlayFallOffAnimationEffectFloatAnimation), false);
            UIXMethodSchema uixMethodSchema6 = new UIXMethodSchema(136, "PlayWeightAnimation", new short[1]
            {
         75
            }, 240, new InvokeHandler(CallPlayWeightAnimationEffectFloatAnimation), false);
            Type.Initialize(null, null, new PropertySchema[6]
            {
         uixPropertySchema2,
         uixPropertySchema3,
         uixPropertySchema4,
         uixPropertySchema5,
         uixPropertySchema1,
         uixPropertySchema6
            }, new MethodSchema[6]
            {
         uixMethodSchema1,
         uixMethodSchema2,
         uixMethodSchema3,
         uixMethodSchema4,
         uixMethodSchema5,
         uixMethodSchema6
            }, null, null, null, null, null, null, null, null);
        }
    }
}
