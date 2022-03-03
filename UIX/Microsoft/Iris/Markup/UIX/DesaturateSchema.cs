// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.DesaturateSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;
using Microsoft.Iris.Render;
using Microsoft.Iris.Session;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class DesaturateSchema
    {
        public static UIXTypeSchema Type;

        private static object GetDesaturate(object instanceObj) => ((DesaturateElement)instanceObj).Desaturate;

        private static void SetDesaturate(ref object instanceObj, object valueObj)
        {
            DesaturateElement desaturateElement = (DesaturateElement)instanceObj;
            float num = (float)valueObj;
            Result result = SingleSchema.Validate0to1(valueObj);
            if (result.Failed)
                ErrorManager.ReportError(result.Error);
            else
                desaturateElement.Desaturate = num;
        }

        private static object Construct() => new DesaturateElement();

        public static void Pass1Initialize() => Type = new UIXTypeSchema(54, "Desaturate", null, 80, typeof(DesaturateElement), UIXTypeFlags.None);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema = new UIXPropertySchema(54, "Desaturate", 194, -1, ExpressionRestriction.None, false, SingleSchema.Validate0to1, false, new GetValueHandler(GetDesaturate), new SetValueHandler(SetDesaturate), false);
            Type.Initialize(new DefaultConstructHandler(Construct), null, new PropertySchema[1]
            {
         uixPropertySchema
            }, null, null, null, null, null, null, null, null, null);
        }
    }
}
