// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.SepiaSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Drawing;
using Microsoft.Iris.Library;
using Microsoft.Iris.Render;
using Microsoft.Iris.Session;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class SepiaSchema
    {
        public static UIXTypeSchema Type;

        private static void SetLightColor(ref object instanceObj, object valueObj) => ((SepiaElement)instanceObj).LightColor = ((Color)valueObj).RenderConvert();

        private static void SetDarkColor(ref object instanceObj, object valueObj) => ((SepiaElement)instanceObj).DarkColor = ((Color)valueObj).RenderConvert();

        private static object GetDesaturate(object instanceObj) => ((SepiaElement)instanceObj).Desaturate;

        private static void SetDesaturate(ref object instanceObj, object valueObj)
        {
            SepiaElement sepiaElement = (SepiaElement)instanceObj;
            float num = (float)valueObj;
            Result result = SingleSchema.ValidateNotNegative(valueObj);
            if (result.Failed)
                ErrorManager.ReportError(result.Error);
            else
                sepiaElement.Desaturate = num;
        }

        private static object GetTone(object instanceObj) => ((SepiaElement)instanceObj).Tone;

        private static void SetTone(ref object instanceObj, object valueObj)
        {
            SepiaElement sepiaElement = (SepiaElement)instanceObj;
            float num = (float)valueObj;
            Result result = SingleSchema.ValidateNotNegative(valueObj);
            if (result.Failed)
                ErrorManager.ReportError(result.Error);
            else
                sepiaElement.Tone = num;
        }

        private static object Construct() => new SepiaElement();

        public static void Pass1Initialize() => Type = new UIXTypeSchema(188, "Sepia", null, 80, typeof(SepiaElement), UIXTypeFlags.None);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(188, "LightColor", 35, -1, ExpressionRestriction.None, false, null, false, null, new SetValueHandler(SetLightColor), false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(188, "DarkColor", 35, -1, ExpressionRestriction.None, false, null, false, null, new SetValueHandler(SetDarkColor), false);
            UIXPropertySchema uixPropertySchema3 = new UIXPropertySchema(188, "Desaturate", 194, -1, ExpressionRestriction.None, false, SingleSchema.ValidateNotNegative, false, new GetValueHandler(GetDesaturate), new SetValueHandler(SetDesaturate), false);
            UIXPropertySchema uixPropertySchema4 = new UIXPropertySchema(188, "Tone", 194, -1, ExpressionRestriction.None, false, SingleSchema.ValidateNotNegative, false, new GetValueHandler(GetTone), new SetValueHandler(SetTone), false);
            Type.Initialize(new DefaultConstructHandler(Construct), null, new PropertySchema[4]
            {
         uixPropertySchema2,
         uixPropertySchema3,
         uixPropertySchema1,
         uixPropertySchema4
            }, null, null, null, null, null, null, null, null, null);
        }
    }
}
