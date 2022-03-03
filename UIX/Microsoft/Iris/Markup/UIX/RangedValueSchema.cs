// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.RangedValueSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.ModelItems;
using Microsoft.Iris.Session;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class RangedValueSchema
    {
        public static UIXTypeSchema Type;

        private static object GetMinValue(object instanceObj) => ((IUIRangedValue)instanceObj).MinValue;

        private static void SetMinValue(ref object instanceObj, object valueObj)
        {
            IUIRangedValue uiRangedValue = (IUIRangedValue)instanceObj;
            float num = (float)valueObj;
            if (num > (double)uiRangedValue.MaxValue)
                ErrorManager.ReportError("Script runtime failure: Invalid '{0}' value is out of range for '{1}'", num, "MinValue");
            else
                uiRangedValue.MinValue = num;
        }

        private static object GetMaxValue(object instanceObj) => ((IUIRangedValue)instanceObj).MaxValue;

        private static void SetMaxValue(ref object instanceObj, object valueObj)
        {
            IUIRangedValue uiRangedValue = (IUIRangedValue)instanceObj;
            float num = (float)valueObj;
            if (num < (double)uiRangedValue.MinValue)
                ErrorManager.ReportError("Script runtime failure: Invalid '{0}' value is out of range for '{1}'", num, "MaxValue");
            else
                uiRangedValue.MaxValue = num;
        }

        private static object GetStep(object instanceObj) => ((IUIRangedValue)instanceObj).Step;

        private static void SetStep(ref object instanceObj, object valueObj) => ((IUIRangedValue)instanceObj).Step = (float)valueObj;

        private static object GetRange(object instanceObj) => ((IUIRangedValue)instanceObj).Range;

        private static object GetValue(object instanceObj) => ((IUIRangedValue)instanceObj).Value;

        private static void SetValue(ref object instanceObj, object valueObj) => ((IUIRangedValue)instanceObj).Value = (float)valueObj;

        private static object Construct() => new Microsoft.Iris.ModelItems.RangedValue();

        public static void Pass1Initialize() => Type = new UIXTypeSchema(168, "RangedValue", null, 231, typeof(IUIRangedValue), UIXTypeFlags.None);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(168, "MinValue", 194, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetMinValue), new SetValueHandler(SetMinValue), false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(168, "MaxValue", 194, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetMaxValue), new SetValueHandler(SetMaxValue), false);
            UIXPropertySchema uixPropertySchema3 = new UIXPropertySchema(168, "Step", 194, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetStep), new SetValueHandler(SetStep), false);
            UIXPropertySchema uixPropertySchema4 = new UIXPropertySchema(168, "Range", 194, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetRange), null, false);
            UIXPropertySchema uixPropertySchema5 = new UIXPropertySchema(168, "Value", 194, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetValue), new SetValueHandler(SetValue), false);
            Type.Initialize(new DefaultConstructHandler(Construct), null, new PropertySchema[5]
            {
         uixPropertySchema2,
         uixPropertySchema1,
         uixPropertySchema4,
         uixPropertySchema3,
         uixPropertySchema5
            }, null, null, null, null, null, null, null, null, null);
        }
    }
}
