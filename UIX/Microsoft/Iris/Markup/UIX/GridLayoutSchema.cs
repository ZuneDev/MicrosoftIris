// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.GridLayoutSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Layout;
using Microsoft.Iris.Layouts;
using Microsoft.Iris.Library;
using Microsoft.Iris.Render;
using Microsoft.Iris.Session;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class GridLayoutSchema
    {
        public static UIXTypeSchema Type;

        private static object GetOrientation(object instanceObj) => ((GridLayout)instanceObj).Orientation;

        private static void SetOrientation(ref object instanceObj, object valueObj) => ((GridLayout)instanceObj).Orientation = (Orientation)valueObj;

        private static object GetAllowWrap(object instanceObj) => BooleanBoxes.Box(((GridLayout)instanceObj).AllowWrap);

        private static void SetAllowWrap(ref object instanceObj, object valueObj) => ((GridLayout)instanceObj).AllowWrap = (bool)valueObj;

        private static object GetReferenceSize(object instanceObj) => ((GridLayout)instanceObj).ReferenceSize;

        private static void SetReferenceSize(ref object instanceObj, object valueObj) => ((GridLayout)instanceObj).ReferenceSize = (Size)valueObj;

        private static object GetSpacing(object instanceObj) => ((GridLayout)instanceObj).Spacing;

        private static void SetSpacing(ref object instanceObj, object valueObj) => ((GridLayout)instanceObj).Spacing = (Size)valueObj;

        private static object GetRows(object instanceObj) => ((GridLayout)instanceObj).Rows;

        private static void SetRows(ref object instanceObj, object valueObj)
        {
            GridLayout gridLayout = (GridLayout)instanceObj;
            int num = (int)valueObj;
            Result result = Int32Schema.ValidateNotNegative(valueObj);
            if (result.Failed)
                ErrorManager.ReportError(result.Error);
            else
                gridLayout.Rows = num;
        }

        private static object GetColumns(object instanceObj) => ((GridLayout)instanceObj).Columns;

        private static void SetColumns(ref object instanceObj, object valueObj)
        {
            GridLayout gridLayout = (GridLayout)instanceObj;
            int num = (int)valueObj;
            Result result = Int32Schema.ValidateNotNegative(valueObj);
            if (result.Failed)
                ErrorManager.ReportError(result.Error);
            else
                gridLayout.Columns = num;
        }

        private static object GetRepeat(object instanceObj) => ((GridLayout)instanceObj).Repeat;

        private static void SetRepeat(ref object instanceObj, object valueObj) => ((GridLayout)instanceObj).Repeat = (RepeatPolicy)valueObj;

        private static object GetRepeatGap(object instanceObj) => ((GridLayout)instanceObj).RepeatGap;

        private static void SetRepeatGap(ref object instanceObj, object valueObj) => ((GridLayout)instanceObj).RepeatGap = (int)valueObj;

        private static object GetDefaultChildAlignment(object instanceObj) => ((GridLayout)instanceObj).DefaultChildAlignment;

        private static void SetDefaultChildAlignment(ref object instanceObj, object valueObj) => ((GridLayout)instanceObj).DefaultChildAlignment = (ItemAlignment)valueObj;

        private static object Construct() => new GridLayout();

        public static void Pass1Initialize() => Type = new UIXTypeSchema(99, "GridLayout", null, 132, typeof(GridLayout), UIXTypeFlags.Immutable);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(99, "Orientation", 154, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetOrientation), new SetValueHandler(SetOrientation), false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(99, "AllowWrap", 15, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetAllowWrap), new SetValueHandler(SetAllowWrap), false);
            UIXPropertySchema uixPropertySchema3 = new UIXPropertySchema(99, "ReferenceSize", 195, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetReferenceSize), new SetValueHandler(SetReferenceSize), false);
            UIXPropertySchema uixPropertySchema4 = new UIXPropertySchema(99, "Spacing", 195, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetSpacing), new SetValueHandler(SetSpacing), false);
            UIXPropertySchema uixPropertySchema5 = new UIXPropertySchema(99, "Rows", 115, -1, ExpressionRestriction.None, false, Int32Schema.ValidateNotNegative, false, new GetValueHandler(GetRows), new SetValueHandler(SetRows), false);
            UIXPropertySchema uixPropertySchema6 = new UIXPropertySchema(99, "Columns", 115, -1, ExpressionRestriction.None, false, Int32Schema.ValidateNotNegative, false, new GetValueHandler(GetColumns), new SetValueHandler(SetColumns), false);
            UIXPropertySchema uixPropertySchema7 = new UIXPropertySchema(99, "Repeat", 172, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetRepeat), new SetValueHandler(SetRepeat), false);
            UIXPropertySchema uixPropertySchema8 = new UIXPropertySchema(99, "RepeatGap", 115, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetRepeatGap), new SetValueHandler(SetRepeatGap), false);
            UIXPropertySchema uixPropertySchema9 = new UIXPropertySchema(99, "DefaultChildAlignment", sbyte.MaxValue, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetDefaultChildAlignment), new SetValueHandler(SetDefaultChildAlignment), false);
            Type.Initialize(new DefaultConstructHandler(Construct), null, new PropertySchema[9]
            {
         uixPropertySchema2,
         uixPropertySchema6,
         uixPropertySchema9,
         uixPropertySchema1,
         uixPropertySchema3,
         uixPropertySchema7,
         uixPropertySchema8,
         uixPropertySchema5,
         uixPropertySchema4
            }, null, null, null, null, null, null, null, null, null);
        }
    }
}
