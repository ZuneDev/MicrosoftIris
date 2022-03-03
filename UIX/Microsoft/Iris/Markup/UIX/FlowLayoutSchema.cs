// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.FlowLayoutSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Layout;
using Microsoft.Iris.Layouts;
using Microsoft.Iris.Render;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class FlowLayoutSchema
    {
        public static UIXTypeSchema Type;

        private static object GetOrientation(object instanceObj) => ((FlowLayout)instanceObj).Orientation;

        private static void SetOrientation(ref object instanceObj, object valueObj) => ((FlowLayout)instanceObj).Orientation = (Orientation)valueObj;

        private static object GetSpacing(object instanceObj) => ((FlowLayout)instanceObj).Spacing;

        private static void SetSpacing(ref object instanceObj, object valueObj) => ((FlowLayout)instanceObj).Spacing = (MajorMinor)valueObj;

        private static object GetAllowWrap(object instanceObj) => BooleanBoxes.Box(((FlowLayout)instanceObj).AllowWrap);

        private static void SetAllowWrap(ref object instanceObj, object valueObj) => ((FlowLayout)instanceObj).AllowWrap = (bool)valueObj;

        private static object GetStripAlignment(object instanceObj) => ((FlowLayout)instanceObj).StripAlignment;

        private static void SetStripAlignment(ref object instanceObj, object valueObj) => ((FlowLayout)instanceObj).StripAlignment = (StripAlignment)valueObj;

        private static object GetRepeat(object instanceObj) => ((FlowLayout)instanceObj).Repeat;

        private static void SetRepeat(ref object instanceObj, object valueObj) => ((FlowLayout)instanceObj).Repeat = (RepeatPolicy)valueObj;

        private static object GetRepeatGap(object instanceObj) => ((FlowLayout)instanceObj).RepeatGap;

        private static void SetRepeatGap(ref object instanceObj, object valueObj) => ((FlowLayout)instanceObj).RepeatGap = (MajorMinor)valueObj;

        private static object GetMissingItemPolicy(object instanceObj) => ((FlowLayout)instanceObj).MissingItemPolicy;

        private static void SetMissingItemPolicy(ref object instanceObj, object valueObj) => ((FlowLayout)instanceObj).MissingItemPolicy = (MissingItemPolicy)valueObj;

        private static object GetMinimumSampleSize(object instanceObj) => ((FlowLayout)instanceObj).MinimumSampleSize;

        private static void SetMinimumSampleSize(ref object instanceObj, object valueObj) => ((FlowLayout)instanceObj).MinimumSampleSize = (int)valueObj;

        private static object GetDefaultChildAlignment(object instanceObj) => ((FlowLayout)instanceObj).DefaultChildAlignment;

        private static void SetDefaultChildAlignment(ref object instanceObj, object valueObj) => ((FlowLayout)instanceObj).DefaultChildAlignment = (ItemAlignment)valueObj;

        private static object Construct() => new FlowLayout();

        public static void Pass1Initialize() => Type = new UIXTypeSchema(90, "FlowLayout", null, 132, typeof(FlowLayout), UIXTypeFlags.Immutable);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(90, "Orientation", 154, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetOrientation), new SetValueHandler(SetOrientation), false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(90, "Spacing", 139, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetSpacing), new SetValueHandler(SetSpacing), false);
            UIXPropertySchema uixPropertySchema3 = new UIXPropertySchema(90, "AllowWrap", 15, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetAllowWrap), new SetValueHandler(SetAllowWrap), false);
            UIXPropertySchema uixPropertySchema4 = new UIXPropertySchema(90, "StripAlignment", 209, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetStripAlignment), new SetValueHandler(SetStripAlignment), false);
            UIXPropertySchema uixPropertySchema5 = new UIXPropertySchema(90, "Repeat", 172, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetRepeat), new SetValueHandler(SetRepeat), false);
            UIXPropertySchema uixPropertySchema6 = new UIXPropertySchema(90, "RepeatGap", 139, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetRepeatGap), new SetValueHandler(SetRepeatGap), false);
            UIXPropertySchema uixPropertySchema7 = new UIXPropertySchema(90, "MissingItemPolicy", 148, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetMissingItemPolicy), new SetValueHandler(SetMissingItemPolicy), false);
            UIXPropertySchema uixPropertySchema8 = new UIXPropertySchema(90, "MinimumSampleSize", 115, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetMinimumSampleSize), new SetValueHandler(SetMinimumSampleSize), false);
            UIXPropertySchema uixPropertySchema9 = new UIXPropertySchema(90, "DefaultChildAlignment", sbyte.MaxValue, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetDefaultChildAlignment), new SetValueHandler(SetDefaultChildAlignment), false);
            Type.Initialize(new DefaultConstructHandler(Construct), null, new PropertySchema[9]
            {
         uixPropertySchema3,
         uixPropertySchema9,
         uixPropertySchema8,
         uixPropertySchema7,
         uixPropertySchema1,
         uixPropertySchema5,
         uixPropertySchema6,
         uixPropertySchema2,
         uixPropertySchema4
            }, null, null, null, null, null, null, null, null, null);
        }
    }
}
