// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.CaretInfoSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.ModelItems;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class CaretInfoSchema
    {
        public static UIXTypeSchema Type;

        private static object GetBlinkTime(object instanceObj) => ((CaretInfo)instanceObj).BlinkTime;

        private static object GetIdealWidth(object instanceObj) => ((CaretInfo)instanceObj).IdealWidth;

        private static void SetIdealWidth(ref object instanceObj, object valueObj) => ((CaretInfo)instanceObj).IdealWidth = (int)valueObj;

        private static object GetVisible(object instanceObj) => BooleanBoxes.Box(((CaretInfo)instanceObj).Visible);

        private static object GetPosition(object instanceObj) => ((CaretInfo)instanceObj).Position;

        private static object GetSuggestedSize(object instanceObj) => ((CaretInfo)instanceObj).SuggestedSize;

        private static object Construct() => new CaretInfo();

        public static void Pass1Initialize() => Type = new UIXTypeSchema(26, "CaretInfo", null, 153, typeof(CaretInfo), UIXTypeFlags.None);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(26, "BlinkTime", 194, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetBlinkTime), null, false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(26, "IdealWidth", 115, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetIdealWidth), new SetValueHandler(SetIdealWidth), false);
            UIXPropertySchema uixPropertySchema3 = new UIXPropertySchema(26, "Visible", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetVisible), null, false);
            UIXPropertySchema uixPropertySchema4 = new UIXPropertySchema(26, "Position", 158, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetPosition), null, false);
            UIXPropertySchema uixPropertySchema5 = new UIXPropertySchema(26, "SuggestedSize", 195, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetSuggestedSize), null, false);
            Type.Initialize(new DefaultConstructHandler(Construct), null, new PropertySchema[5]
            {
         uixPropertySchema1,
         uixPropertySchema2,
         uixPropertySchema4,
         uixPropertySchema5,
         uixPropertySchema3
            }, null, null, null, null, null, null, null, null, null);
        }
    }
}
