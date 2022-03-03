// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.ScrollModelSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Layouts;
using Microsoft.Iris.Library;
using Microsoft.Iris.ModelItems;
using Microsoft.Iris.Session;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class ScrollModelSchema
    {
        public static UIXTypeSchema Type;

        private static object GetEnabled(object instanceObj) => BooleanBoxes.Box(((ScrollModel)instanceObj).Enabled);

        private static void SetEnabled(ref object instanceObj, object valueObj) => ((ScrollModel)instanceObj).Enabled = (bool)valueObj;

        private static object GetPageStep(object instanceObj) => ((ScrollModel)instanceObj).PageStep;

        private static void SetPageStep(ref object instanceObj, object valueObj)
        {
            ScrollModel scrollModel = (ScrollModel)instanceObj;
            float num = (float)valueObj;
            Result result = SingleSchema.ValidateNotNegative(valueObj);
            if (result.Failed)
                ErrorManager.ReportError(result.Error);
            else
                scrollModel.PageStep = num;
        }

        private static object GetPageSizedScrollStep(object instanceObj) => BooleanBoxes.Box(((ScrollModel)instanceObj).PageSizedScrollStep);

        private static void SetPageSizedScrollStep(ref object instanceObj, object valueObj) => ((ScrollModel)instanceObj).PageSizedScrollStep = (bool)valueObj;

        private static object GetBeginPadding(object instanceObj) => ((ScrollModel)instanceObj).BeginPadding;

        private static void SetBeginPadding(ref object instanceObj, object valueObj) => ((ScrollModel)instanceObj).BeginPadding = (int)valueObj;

        private static object GetEndPadding(object instanceObj) => ((ScrollModel)instanceObj).EndPadding;

        private static void SetEndPadding(ref object instanceObj, object valueObj) => ((ScrollModel)instanceObj).EndPadding = (int)valueObj;

        private static object GetBeginPaddingRelativeTo(object instanceObj) => ((ScrollModel)instanceObj).BeginPaddingRelativeTo;

        private static void SetBeginPaddingRelativeTo(ref object instanceObj, object valueObj) => ((ScrollModel)instanceObj).BeginPaddingRelativeTo = (RelativeEdge)valueObj;

        private static object GetEndPaddingRelativeTo(object instanceObj) => ((ScrollModel)instanceObj).EndPaddingRelativeTo;

        private static void SetEndPaddingRelativeTo(ref object instanceObj, object valueObj) => ((ScrollModel)instanceObj).EndPaddingRelativeTo = (RelativeEdge)valueObj;

        private static object GetLocked(object instanceObj) => BooleanBoxes.Box(((ScrollModel)instanceObj).Locked);

        private static void SetLocked(ref object instanceObj, object valueObj) => ((ScrollModel)instanceObj).Locked = (bool)valueObj;

        private static object GetLockedPosition(object instanceObj) => ((ScrollModel)instanceObj).LockedPosition;

        private static void SetLockedPosition(ref object instanceObj, object valueObj) => ((ScrollModel)instanceObj).LockedPosition = (float)valueObj;

        private static object GetLockedAlignment(object instanceObj) => ((ScrollModel)instanceObj).LockedAlignment;

        private static void SetLockedAlignment(ref object instanceObj, object valueObj) => ((ScrollModel)instanceObj).LockedAlignment = (float)valueObj;

        private static object GetContentPositioningBehavior(object instanceObj) => ((ScrollModel)instanceObj).ContentPositioningBehavior;

        private static void SetContentPositioningBehavior(ref object instanceObj, object valueObj) => ((ScrollModel)instanceObj).ContentPositioningBehavior = (ContentPositioningPolicy)valueObj;

        private static object Construct() => new ScrollModel();

        private static object CallScrollFocusIntoView(object instanceObj, object[] parameters)
        {
            ((ScrollModel)instanceObj).ScrollFocusIntoView();
            return null;
        }

        public static void Pass1Initialize() => Type = new UIXTypeSchema(182, "ScrollModel", null, 183, typeof(ScrollModel), UIXTypeFlags.None);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(182, "Enabled", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetEnabled), new SetValueHandler(SetEnabled), false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(182, "PageStep", 194, -1, ExpressionRestriction.None, false, SingleSchema.ValidateNotNegative, true, new GetValueHandler(GetPageStep), new SetValueHandler(SetPageStep), false);
            UIXPropertySchema uixPropertySchema3 = new UIXPropertySchema(182, "PageSizedScrollStep", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetPageSizedScrollStep), new SetValueHandler(SetPageSizedScrollStep), false);
            UIXPropertySchema uixPropertySchema4 = new UIXPropertySchema(182, "BeginPadding", 115, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetBeginPadding), new SetValueHandler(SetBeginPadding), false);
            UIXPropertySchema uixPropertySchema5 = new UIXPropertySchema(182, "EndPadding", 115, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetEndPadding), new SetValueHandler(SetEndPadding), false);
            UIXPropertySchema uixPropertySchema6 = new UIXPropertySchema(182, "BeginPaddingRelativeTo", 170, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetBeginPaddingRelativeTo), new SetValueHandler(SetBeginPaddingRelativeTo), false);
            UIXPropertySchema uixPropertySchema7 = new UIXPropertySchema(182, "EndPaddingRelativeTo", 170, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetEndPaddingRelativeTo), new SetValueHandler(SetEndPaddingRelativeTo), false);
            UIXPropertySchema uixPropertySchema8 = new UIXPropertySchema(182, "Locked", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetLocked), new SetValueHandler(SetLocked), false);
            UIXPropertySchema uixPropertySchema9 = new UIXPropertySchema(182, "LockedPosition", 194, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetLockedPosition), new SetValueHandler(SetLockedPosition), false);
            UIXPropertySchema uixPropertySchema10 = new UIXPropertySchema(182, "LockedAlignment", 194, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetLockedAlignment), new SetValueHandler(SetLockedAlignment), false);
            UIXPropertySchema uixPropertySchema11 = new UIXPropertySchema(182, "ContentPositioningBehavior", 41, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetContentPositioningBehavior), new SetValueHandler(SetContentPositioningBehavior), false);
            UIXMethodSchema uixMethodSchema = new UIXMethodSchema(182, "ScrollFocusIntoView", null, 240, new InvokeHandler(CallScrollFocusIntoView), false);
            Type.Initialize(new DefaultConstructHandler(Construct), null, new PropertySchema[11]
            {
         uixPropertySchema4,
         uixPropertySchema6,
         uixPropertySchema11,
         uixPropertySchema1,
         uixPropertySchema5,
         uixPropertySchema7,
         uixPropertySchema8,
         uixPropertySchema10,
         uixPropertySchema9,
         uixPropertySchema3,
         uixPropertySchema2
            }, new MethodSchema[1]
            {
         uixMethodSchema
            }, null, null, null, null, null, null, null, null);
        }
    }
}
