// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.SpotLight2DInstanceSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Animations;
using Microsoft.Iris.Drawing;
using Microsoft.Iris.Render;
using Microsoft.Iris.UI;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class SpotLight2DInstanceSchema
    {
        public static UIXTypeSchema Type;

        private static void SetPosition(ref object instanceObj, object valueObj) => ((EffectElementWrapper)instanceObj).SetProperty("Position", (Vector3)valueObj);

        private static void SetDirectionAngle(ref object instanceObj, object valueObj) => ((EffectElementWrapper)instanceObj).SetProperty("DirectionAngle", (float)valueObj);

        private static void SetLightColor(ref object instanceObj, object valueObj) => ((EffectElementWrapper)instanceObj).SetProperty("LightColor", (Color)valueObj);

        private static void SetAmbientColor(ref object instanceObj, object valueObj) => ((EffectElementWrapper)instanceObj).SetProperty("AmbientColor", (Color)valueObj);

        private static void SetInnerConeAngle(ref object instanceObj, object valueObj) => ((EffectElementWrapper)instanceObj).SetProperty("InnerConeAngle", (float)valueObj);

        private static void SetOuterConeAngle(ref object instanceObj, object valueObj) => ((EffectElementWrapper)instanceObj).SetProperty("OuterConeAngle", (float)valueObj);

        private static void SetIntensity(ref object instanceObj, object valueObj) => ((EffectElementWrapper)instanceObj).SetProperty("Intensity", (float)valueObj);

        private static void SetAttenuation(ref object instanceObj, object valueObj) => ((EffectElementWrapper)instanceObj).SetProperty("Attenuation", (Vector3)valueObj);

        private static object CallPlayPositionAnimationEffectVector3Animation(
          object instanceObj,
          object[] parameters)
        {
            ((EffectElementWrapper)instanceObj).PlayAnimation(EffectProperty.Position, (EffectAnimation)parameters[0]);
            return null;
        }

        private static object CallPlayDirectionAngleAnimationEffectFloatAnimation(
          object instanceObj,
          object[] parameters)
        {
            ((EffectElementWrapper)instanceObj).PlayAnimation(EffectProperty.DirectionAngle, (EffectAnimation)parameters[0]);
            return null;
        }

        private static object CallPlayLightColorAnimationEffectColorAnimation(
          object instanceObj,
          object[] parameters)
        {
            ((EffectElementWrapper)instanceObj).PlayAnimation(EffectProperty.LightColor, (EffectAnimation)parameters[0]);
            return null;
        }

        private static object CallPlayAmbientColorAnimationEffectColorAnimation(
          object instanceObj,
          object[] parameters)
        {
            ((EffectElementWrapper)instanceObj).PlayAnimation(EffectProperty.AmbientColor, (EffectAnimation)parameters[0]);
            return null;
        }

        private static object CallPlayInnerConeAngleAnimationEffectFloatAnimation(
          object instanceObj,
          object[] parameters)
        {
            ((EffectElementWrapper)instanceObj).PlayAnimation(EffectProperty.InnerConeAngle, (EffectAnimation)parameters[0]);
            return null;
        }

        private static object CallPlayOuterConeAngleAnimationEffectFloatAnimation(
          object instanceObj,
          object[] parameters)
        {
            ((EffectElementWrapper)instanceObj).PlayAnimation(EffectProperty.OuterConeAngle, (EffectAnimation)parameters[0]);
            return null;
        }

        private static object CallPlayIntensityAnimationEffectFloatAnimation(
          object instanceObj,
          object[] parameters)
        {
            ((EffectElementWrapper)instanceObj).PlayAnimation(EffectProperty.Intensity, (EffectAnimation)parameters[0]);
            return null;
        }

        private static object CallPlayAttenuationAnimationEffectVector3Animation(
          object instanceObj,
          object[] parameters)
        {
            ((EffectElementWrapper)instanceObj).PlayAnimation(EffectProperty.Attenuation, (EffectAnimation)parameters[0]);
            return null;
        }

        public static void Pass1Initialize() => Type = new UIXTypeSchema(203, "SpotLight2DInstance", null, 74, typeof(EffectElementWrapper), UIXTypeFlags.None);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(203, "Position", 234, -1, ExpressionRestriction.None, false, null, false, null, new SetValueHandler(SetPosition), false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(203, "DirectionAngle", 194, -1, ExpressionRestriction.None, false, null, false, null, new SetValueHandler(SetDirectionAngle), false);
            UIXPropertySchema uixPropertySchema3 = new UIXPropertySchema(203, "LightColor", 35, -1, ExpressionRestriction.None, false, null, false, null, new SetValueHandler(SetLightColor), false);
            UIXPropertySchema uixPropertySchema4 = new UIXPropertySchema(203, "AmbientColor", 35, -1, ExpressionRestriction.None, false, null, false, null, new SetValueHandler(SetAmbientColor), false);
            UIXPropertySchema uixPropertySchema5 = new UIXPropertySchema(203, "InnerConeAngle", 194, -1, ExpressionRestriction.None, false, null, false, null, new SetValueHandler(SetInnerConeAngle), false);
            UIXPropertySchema uixPropertySchema6 = new UIXPropertySchema(203, "OuterConeAngle", 194, -1, ExpressionRestriction.None, false, null, false, null, new SetValueHandler(SetOuterConeAngle), false);
            UIXPropertySchema uixPropertySchema7 = new UIXPropertySchema(203, "Intensity", 194, -1, ExpressionRestriction.None, false, null, false, null, new SetValueHandler(SetIntensity), false);
            UIXPropertySchema uixPropertySchema8 = new UIXPropertySchema(203, "Attenuation", 234, -1, ExpressionRestriction.None, false, null, false, null, new SetValueHandler(SetAttenuation), false);
            UIXMethodSchema uixMethodSchema1 = new UIXMethodSchema(203, "PlayPositionAnimation", new short[1]
            {
         81
            }, 240, new InvokeHandler(CallPlayPositionAnimationEffectVector3Animation), false);
            UIXMethodSchema uixMethodSchema2 = new UIXMethodSchema(203, "PlayDirectionAngleAnimation", new short[1]
            {
         75
            }, 240, new InvokeHandler(CallPlayDirectionAngleAnimationEffectFloatAnimation), false);
            UIXMethodSchema uixMethodSchema3 = new UIXMethodSchema(203, "PlayLightColorAnimation", new short[1]
            {
         71
            }, 240, new InvokeHandler(CallPlayLightColorAnimationEffectColorAnimation), false);
            UIXMethodSchema uixMethodSchema4 = new UIXMethodSchema(203, "PlayAmbientColorAnimation", new short[1]
            {
         71
            }, 240, new InvokeHandler(CallPlayAmbientColorAnimationEffectColorAnimation), false);
            UIXMethodSchema uixMethodSchema5 = new UIXMethodSchema(203, "PlayInnerConeAngleAnimation", new short[1]
            {
         75
            }, 240, new InvokeHandler(CallPlayInnerConeAngleAnimationEffectFloatAnimation), false);
            UIXMethodSchema uixMethodSchema6 = new UIXMethodSchema(203, "PlayOuterConeAngleAnimation", new short[1]
            {
         75
            }, 240, new InvokeHandler(CallPlayOuterConeAngleAnimationEffectFloatAnimation), false);
            UIXMethodSchema uixMethodSchema7 = new UIXMethodSchema(203, "PlayIntensityAnimation", new short[1]
            {
         75
            }, 240, new InvokeHandler(CallPlayIntensityAnimationEffectFloatAnimation), false);
            UIXMethodSchema uixMethodSchema8 = new UIXMethodSchema(203, "PlayAttenuationAnimation", new short[1]
            {
         81
            }, 240, new InvokeHandler(CallPlayAttenuationAnimationEffectVector3Animation), false);
            Type.Initialize(null, null, new PropertySchema[8]
            {
         uixPropertySchema4,
         uixPropertySchema8,
         uixPropertySchema2,
         uixPropertySchema5,
         uixPropertySchema7,
         uixPropertySchema3,
         uixPropertySchema6,
         uixPropertySchema1
            }, new MethodSchema[8]
            {
         uixMethodSchema1,
         uixMethodSchema2,
         uixMethodSchema3,
         uixMethodSchema4,
         uixMethodSchema5,
         uixMethodSchema6,
         uixMethodSchema7,
         uixMethodSchema8
            }, null, null, null, null, null, null, null, null);
        }
    }
}
