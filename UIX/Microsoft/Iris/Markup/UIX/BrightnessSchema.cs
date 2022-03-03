// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.BrightnessSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Render;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class BrightnessSchema
    {
        public static UIXTypeSchema Type;

        private static object GetBrightness(object instanceObj) => ((BrightnessElement)instanceObj).Brightness;

        private static void SetBrightness(ref object instanceObj, object valueObj) => ((BrightnessElement)instanceObj).Brightness = (float)valueObj;

        private static object Construct() => new BrightnessElement();

        public static void Pass1Initialize() => Type = new UIXTypeSchema(17, "Brightness", null, 80, typeof(BrightnessElement), UIXTypeFlags.None);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema = new UIXPropertySchema(17, "Brightness", 194, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetBrightness), new SetValueHandler(SetBrightness), false);
            Type.Initialize(new DefaultConstructHandler(Construct), null, new PropertySchema[1]
            {
         uixPropertySchema
            }, null, null, null, null, null, null, null, null, null);
        }
    }
}
