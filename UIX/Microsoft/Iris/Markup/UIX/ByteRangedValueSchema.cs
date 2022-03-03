// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.ByteRangedValueSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.ModelItems;
using Microsoft.Iris.Session;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class ByteRangedValueSchema
    {
        public static UIXTypeSchema Type;

        private static object GetMinValue(object instanceObj) => (byte)((IUIRangedValue)instanceObj).MinValue;

        private static void SetMinValue(ref object instanceObj, object valueObj)
        {
            IUIByteRangedValue uiByteRangedValue = (IUIByteRangedValue)instanceObj;
            byte num = (byte)valueObj;
            if (num > (double)uiByteRangedValue.MaxValue)
                ErrorManager.ReportError("Script runtime failure: Invalid '{0}' value is out of range for '{1}'", num, "MinValue");
            else
                uiByteRangedValue.MinValue = num;
        }

        private static object GetMaxValue(object instanceObj) => (byte)((IUIRangedValue)instanceObj).MaxValue;

        private static void SetMaxValue(ref object instanceObj, object valueObj)
        {
            IUIByteRangedValue uiByteRangedValue = (IUIByteRangedValue)instanceObj;
            byte num = (byte)valueObj;
            if (num < (double)uiByteRangedValue.MinValue)
                ErrorManager.ReportError("Script runtime failure: Invalid '{0}' value is out of range for '{1}'", num, "MaxValue");
            else
                uiByteRangedValue.MaxValue = num;
        }

        private static object GetStep(object instanceObj) => (byte)((IUIRangedValue)instanceObj).Step;

        private static void SetStep(ref object instanceObj, object valueObj) => ((IUIRangedValue)instanceObj).Step = (byte)valueObj;

        private static object GetValue(object instanceObj) => (byte)((IUIRangedValue)instanceObj).Value;

        private static void SetValue(ref object instanceObj, object valueObj) => ((IUIRangedValue)instanceObj).Value = (byte)valueObj;

        private static object Construct() => new Microsoft.Iris.ModelItems.ByteRangedValue();

        public static void Pass1Initialize() => Type = new UIXTypeSchema(20, "ByteRangedValue", null, 168, typeof(IUIByteRangedValue), UIXTypeFlags.None);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(20, "MinValue", 19, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetMinValue), new SetValueHandler(SetMinValue), false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(20, "MaxValue", 19, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetMaxValue), new SetValueHandler(SetMaxValue), false);
            UIXPropertySchema uixPropertySchema3 = new UIXPropertySchema(20, "Step", 19, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetStep), new SetValueHandler(SetStep), false);
            UIXPropertySchema uixPropertySchema4 = new UIXPropertySchema(20, "Value", 19, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetValue), new SetValueHandler(SetValue), false);
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
