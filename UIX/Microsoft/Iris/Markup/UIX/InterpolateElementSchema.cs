// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.InterpolateElementSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;
using Microsoft.Iris.Render;
using Microsoft.Iris.Session;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class InterpolateElementSchema
    {
        public static UIXTypeSchema Type;

        private static object GetInput1(object instanceObj) => ((InterpolateElement)instanceObj).Input1;

        private static void SetInput1(ref object instanceObj, object valueObj) => ((InterpolateElement)instanceObj).Input1 = (EffectInput)valueObj;

        private static object GetInput2(object instanceObj) => ((InterpolateElement)instanceObj).Input2;

        private static void SetInput2(ref object instanceObj, object valueObj) => ((InterpolateElement)instanceObj).Input2 = (EffectInput)valueObj;

        private static object GetValue(object instanceObj) => ((InterpolateElement)instanceObj).Value;

        private static void SetValue(ref object instanceObj, object valueObj)
        {
            InterpolateElement interpolateElement = (InterpolateElement)instanceObj;
            float num = (float)valueObj;
            Result result = SingleSchema.Validate0to1(valueObj);
            if (result.Failed)
                ErrorManager.ReportError(result.Error);
            else
                interpolateElement.Value = num;
        }

        private static object Construct() => new InterpolateElement();

        public static void Pass1Initialize() => Type = new UIXTypeSchema(119, "InterpolateElement", null, 77, typeof(InterpolateElement), UIXTypeFlags.None);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(119, "Input1", 77, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetInput1), new SetValueHandler(SetInput1), false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(119, "Input2", 77, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetInput2), new SetValueHandler(SetInput2), false);
            UIXPropertySchema uixPropertySchema3 = new UIXPropertySchema(119, "Value", 194, -1, ExpressionRestriction.None, false, SingleSchema.Validate0to1, false, new GetValueHandler(GetValue), new SetValueHandler(SetValue), false);
            Type.Initialize(new DefaultConstructHandler(Construct), null, new PropertySchema[3]
            {
         uixPropertySchema1,
         uixPropertySchema2,
         uixPropertySchema3
            }, null, null, null, null, null, null, null, null, null);
        }
    }
}
