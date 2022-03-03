// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.ValueRangeSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.ModelItems;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class ValueRangeSchema
    {
        public static UIXTypeSchema Type;

        private static object GetObjectValue(object instanceObj) => ((IUIValueRange)instanceObj).ObjectValue;

        private static object GetHasPreviousValue(object instanceObj) => BooleanBoxes.Box(((IUIValueRange)instanceObj).HasPreviousValue);

        private static object GetHasNextValue(object instanceObj) => BooleanBoxes.Box(((IUIValueRange)instanceObj).HasNextValue);

        private static object CallPreviousValue(object instanceObj, object[] parameters)
        {
            ((IUIValueRange)instanceObj).PreviousValue();
            return null;
        }

        private static object CallNextValue(object instanceObj, object[] parameters)
        {
            ((IUIValueRange)instanceObj).NextValue();
            return null;
        }

        public static void Pass1Initialize() => Type = new UIXTypeSchema(231, "ValueRange", null, 153, typeof(IUIValueRange), UIXTypeFlags.None);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(231, "ObjectValue", 153, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetObjectValue), null, false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(231, "HasPreviousValue", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetHasPreviousValue), null, false);
            UIXPropertySchema uixPropertySchema3 = new UIXPropertySchema(231, "HasNextValue", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetHasNextValue), null, false);
            UIXMethodSchema uixMethodSchema1 = new UIXMethodSchema(231, "PreviousValue", null, 240, new InvokeHandler(CallPreviousValue), false);
            UIXMethodSchema uixMethodSchema2 = new UIXMethodSchema(231, "NextValue", null, 240, new InvokeHandler(CallNextValue), false);
            Type.Initialize(null, null, new PropertySchema[3]
            {
         uixPropertySchema3,
         uixPropertySchema2,
         uixPropertySchema1
            }, new MethodSchema[2]
            {
         uixMethodSchema1,
         uixMethodSchema2
            }, null, null, null, null, null, null, null, null);
        }
    }
}
