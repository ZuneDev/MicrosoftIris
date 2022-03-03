// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.ContrastSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;
using Microsoft.Iris.Render;
using Microsoft.Iris.Session;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class ContrastSchema
    {
        public static UIXTypeSchema Type;

        private static object GetContrast(object instanceObj) => ((ContrastElement)instanceObj).Contrast;

        private static void SetContrast(ref object instanceObj, object valueObj)
        {
            ContrastElement contrastElement = (ContrastElement)instanceObj;
            float num = (float)valueObj;
            Result result = SingleSchema.ValidateNotNegative(valueObj);
            if (result.Failed)
                ErrorManager.ReportError(result.Error);
            else
                contrastElement.Contrast = num;
        }

        private static object Construct() => new ContrastElement();

        public static void Pass1Initialize() => Type = new UIXTypeSchema(42, "Contrast", null, 80, typeof(ContrastElement), UIXTypeFlags.None);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema = new UIXPropertySchema(42, "Contrast", 194, -1, ExpressionRestriction.None, false, SingleSchema.ValidateNotNegative, false, new GetValueHandler(GetContrast), new SetValueHandler(SetContrast), false);
            Type.Initialize(new DefaultConstructHandler(Construct), null, new PropertySchema[1]
            {
         uixPropertySchema
            }, null, null, null, null, null, null, null, null, null);
        }
    }
}
