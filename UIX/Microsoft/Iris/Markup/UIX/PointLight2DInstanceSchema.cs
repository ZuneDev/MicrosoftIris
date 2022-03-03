// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.PointLight2DInstanceSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Animations;
using Microsoft.Iris.Drawing;
using Microsoft.Iris.Render;
using Microsoft.Iris.UI;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class PointLight2DInstanceSchema
    {
        public static UIXTypeSchema Type;

        private static void SetPosition(ref object instanceObj, object valueObj) => ((EffectElementWrapper)instanceObj).SetProperty("Position", (Vector3)valueObj);

        private static void SetRadius(ref object instanceObj, object valueObj) => ((EffectElementWrapper)instanceObj).SetProperty("Radius", (float)valueObj);

        private static void SetLightColor(ref object instanceObj, object valueObj) => ((EffectElementWrapper)instanceObj).SetProperty("LightColor", (Color)valueObj);

        private static void SetAmbientColor(ref object instanceObj, object valueObj) => ((EffectElementWrapper)instanceObj).SetProperty("AmbientColor", (Color)valueObj);

        private static void SetAttenuation(ref object instanceObj, object valueObj) => ((EffectElementWrapper)instanceObj).SetProperty("Attenuation", (Vector3)valueObj);

        private static object CallPlayPositionAnimationEffectVector3Animation(
          object instanceObj,
          object[] parameters)
        {
            ((EffectElementWrapper)instanceObj).PlayAnimation(EffectProperty.Position, (EffectAnimation)parameters[0]);
            return null;
        }

        private static object CallPlayRadiusAnimationEffectFloatAnimation(
          object instanceObj,
          object[] parameters)
        {
            ((EffectElementWrapper)instanceObj).PlayAnimation(EffectProperty.Radius, (EffectAnimation)parameters[0]);
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

        private static object CallPlayAttenuationAnimationEffectVector3Animation(
          object instanceObj,
          object[] parameters)
        {
            ((EffectElementWrapper)instanceObj).PlayAnimation(EffectProperty.Attenuation, (EffectAnimation)parameters[0]);
            return null;
        }

        public static void Pass1Initialize() => Type = new UIXTypeSchema(160, "PointLight2DInstance", null, 74, typeof(EffectElementWrapper), UIXTypeFlags.None);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(160, "Position", 234, -1, ExpressionRestriction.None, false, null, false, null, new SetValueHandler(SetPosition), false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(160, "Radius", 194, -1, ExpressionRestriction.None, false, null, false, null, new SetValueHandler(SetRadius), false);
            UIXPropertySchema uixPropertySchema3 = new UIXPropertySchema(160, "LightColor", 35, -1, ExpressionRestriction.None, false, null, false, null, new SetValueHandler(SetLightColor), false);
            UIXPropertySchema uixPropertySchema4 = new UIXPropertySchema(160, "AmbientColor", 35, -1, ExpressionRestriction.None, false, null, false, null, new SetValueHandler(SetAmbientColor), false);
            UIXPropertySchema uixPropertySchema5 = new UIXPropertySchema(160, "Attenuation", 234, -1, ExpressionRestriction.None, false, null, false, null, new SetValueHandler(SetAttenuation), false);
            UIXMethodSchema uixMethodSchema1 = new UIXMethodSchema(160, "PlayPositionAnimation", new short[1]
            {
         81
            }, 240, new InvokeHandler(CallPlayPositionAnimationEffectVector3Animation), false);
            UIXMethodSchema uixMethodSchema2 = new UIXMethodSchema(160, "PlayRadiusAnimation", new short[1]
            {
         75
            }, 240, new InvokeHandler(CallPlayRadiusAnimationEffectFloatAnimation), false);
            UIXMethodSchema uixMethodSchema3 = new UIXMethodSchema(160, "PlayLightColorAnimation", new short[1]
            {
         71
            }, 240, new InvokeHandler(CallPlayLightColorAnimationEffectColorAnimation), false);
            UIXMethodSchema uixMethodSchema4 = new UIXMethodSchema(160, "PlayAmbientColorAnimation", new short[1]
            {
         71
            }, 240, new InvokeHandler(CallPlayAmbientColorAnimationEffectColorAnimation), false);
            UIXMethodSchema uixMethodSchema5 = new UIXMethodSchema(160, "PlayAttenuationAnimation", new short[1]
            {
         81
            }, 240, new InvokeHandler(CallPlayAttenuationAnimationEffectVector3Animation), false);
            Type.Initialize(null, null, new PropertySchema[5]
            {
         uixPropertySchema4,
         uixPropertySchema5,
         uixPropertySchema3,
         uixPropertySchema1,
         uixPropertySchema2
            }, new MethodSchema[5]
            {
         uixMethodSchema1,
         uixMethodSchema2,
         uixMethodSchema3,
         uixMethodSchema4,
         uixMethodSchema5
            }, null, null, null, null, null, null, null, null);
        }
    }
}
