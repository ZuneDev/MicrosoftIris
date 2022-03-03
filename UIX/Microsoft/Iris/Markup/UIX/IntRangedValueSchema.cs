// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.IntRangedValueSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.ModelItems;
using Microsoft.Iris.Session;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class IntRangedValueSchema
    {
        public static UIXTypeSchema Type;

        private static object GetMinValue(object instanceObj) => (int)((IUIRangedValue)instanceObj).MinValue;

        private static void SetMinValue(ref object instanceObj, object valueObj)
        {
            IUIIntRangedValue uiIntRangedValue = (IUIIntRangedValue)instanceObj;
            int num = (int)valueObj;
            if (num > (double)uiIntRangedValue.MaxValue)
                ErrorManager.ReportError("Script runtime failure: Invalid '{0}' value is out of range for '{1}'", num, "MinValue");
            else
                uiIntRangedValue.MinValue = num;
        }

        private static object GetMaxValue(object instanceObj) => (int)((IUIRangedValue)instanceObj).MaxValue;

        private static void SetMaxValue(ref object instanceObj, object valueObj)
        {
            IUIIntRangedValue uiIntRangedValue = (IUIIntRangedValue)instanceObj;
            int num = (int)valueObj;
            if (num < (double)uiIntRangedValue.MinValue)
                ErrorManager.ReportError("Script runtime failure: Invalid '{0}' value is out of range for '{1}'", num, "MaxValue");
            else
                uiIntRangedValue.MaxValue = num;
        }

        private static object GetStep(object instanceObj) => (int)((IUIRangedValue)instanceObj).Step;

        private static void SetStep(ref object instanceObj, object valueObj) => ((IUIRangedValue)instanceObj).Step = (int)valueObj;

        private static object GetValue(object instanceObj) => (int)((IUIRangedValue)instanceObj).Value;

        private static void SetValue(ref object instanceObj, object valueObj) => ((IUIRangedValue)instanceObj).Value = (int)valueObj;

        private static object Construct() => new Microsoft.Iris.ModelItems.IntRangedValue();

        public static void Pass1Initialize() => Type = new UIXTypeSchema(117, "IntRangedValue", null, 168, typeof(IUIIntRangedValue), UIXTypeFlags.None);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(117, "MinValue", 115, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetMinValue), new SetValueHandler(SetMinValue), false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(117, "MaxValue", 115, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetMaxValue), new SetValueHandler(SetMaxValue), false);
            UIXPropertySchema uixPropertySchema3 = new UIXPropertySchema(117, "Step", 115, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetStep), new SetValueHandler(SetStep), false);
            UIXPropertySchema uixPropertySchema4 = new UIXPropertySchema(117, "Value", 115, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetValue), new SetValueHandler(SetValue), false);
            Type.Initialize(new DefaultConstructHandler(Construct), null, new PropertySchema[4]
            {
         uixPropertySchema2,
         uixPropertySchema1,
         uixPropertySchema3,
         uixPropertySchema4
            }, null, null, null, null, null, null, null, null, null);
        }
    }
}
