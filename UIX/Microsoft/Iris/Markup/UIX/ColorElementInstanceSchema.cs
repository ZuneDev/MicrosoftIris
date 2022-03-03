// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.ColorElementInstanceSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Animations;
using Microsoft.Iris.Drawing;
using Microsoft.Iris.UI;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class ColorElementInstanceSchema
    {
        public static UIXTypeSchema Type;

        private static void SetColor(ref object instanceObj, object valueObj) => ((EffectElementWrapper)instanceObj).SetProperty("Color", (Color)valueObj);

        private static object CallPlayColorAnimationEffectColorAnimation(
          object instanceObj,
          object[] parameters)
        {
            ((EffectElementWrapper)instanceObj).PlayAnimation(EffectProperty.Color, (EffectAnimation)parameters[0]);
            return null;
        }

        public static void Pass1Initialize() => Type = new UIXTypeSchema(37, "ColorElementInstance", null, 74, typeof(EffectElementWrapper), UIXTypeFlags.None);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema = new UIXPropertySchema(37, "Color", 35, -1, ExpressionRestriction.None, false, null, false, null, new SetValueHandler(SetColor), false);
            UIXMethodSchema uixMethodSchema = new UIXMethodSchema(37, "PlayColorAnimation", new short[1]
            {
         71
            }, 240, new InvokeHandler(CallPlayColorAnimationEffectColorAnimation), false);
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
