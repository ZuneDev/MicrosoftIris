// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.EffectColorAnimationSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Animations;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class EffectColorAnimationSchema
    {
        public static UIXTypeSchema Type;

        private static object GetKeyframes(object instanceObj) => ((AnimationTemplate)instanceObj).Keyframes;

        private static object Construct() => new EffectAnimation();

        public static void Pass1Initialize() => Type = new UIXTypeSchema(71, "EffectColorAnimation", null, 70, typeof(EffectAnimation), UIXTypeFlags.None);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema = new UIXPropertySchema(71, "Keyframes", 138, 72, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetKeyframes), null, false);
            Type.Initialize(new DefaultConstructHandler(Construct), null, new PropertySchema[1]
            {
         uixPropertySchema
            }, null, null, null, null, null, null, null, null, null);
        }
    }
}
