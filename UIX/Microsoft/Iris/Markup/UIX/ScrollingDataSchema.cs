// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.ScrollingDataSchema
// Assembly: UIX, Version=2.1.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: 57C02C21-1B9A-4AE2-836C-8816A259A92A
// Assembly location: D:\Downloads\Zune Software v2.5\packages\Zune-x86\UIX.dll

using Microsoft.Iris.Layouts;
using Microsoft.Iris.Library;
using Microsoft.Iris.ModelItems;
using Microsoft.Iris.Session;

#nullable disable
namespace Microsoft.Iris.Markup.UIX
{
    internal static class ScrollingDataSchema
    {
        private static PropertySchema EnabledProperty;
        private static PropertySchema ScrollStepProperty;
        private static PropertySchema PageStepProperty;
        private static PropertySchema PageSizedScrollStepProperty;
        private static PropertySchema BeginPaddingProperty;
        private static PropertySchema EndPaddingProperty;
        private static PropertySchema BeginPaddingRelativeToProperty;
        private static PropertySchema EndPaddingRelativeToProperty;
        private static PropertySchema LockedProperty;
        private static PropertySchema LockedPositionProperty;
        private static PropertySchema LockedAlignmentProperty;
        private static PropertySchema ContentPositioningBehaviorProperty;
        private static PropertySchema CanScrollUpProperty;
        private static PropertySchema CanScrollDownProperty;
        private static PropertySchema CurrentPageProperty;
        private static PropertySchema TotalPagesProperty;
        private static MethodSchema ScrollInt32Method;
        private static MethodSchema ScrollFocusIntoViewMethod;
        private static MethodSchema ScrollUpMethod;
        private static MethodSchema ScrollDownMethod;
        private static MethodSchema PageUpMethod;
        private static MethodSchema PageDownMethod;
        private static MethodSchema HomeMethod;
        private static MethodSchema EndMethod;
        private static MethodSchema ScrollToPositionSingleMethod;
        public static UIXTypeSchema Type = new UIXTypeSchema("ScrollingData", (string) null);

        private static object Construct() => (object) new ScrollingData();

        private static object GetEnabled(object instanceObj)
        {
            return BooleanBoxes.Box(((ScrollingData) instanceObj).Enabled);
        }

        private static void SetEnabled(ref object instanceObj, object valueObj)
        {
            ScrollingData scrollingData = (ScrollingData) instanceObj;
            bool flag = (bool) valueObj;
            scrollingData.Enabled = flag;
            instanceObj = (object) scrollingData;
        }

        private static object GetScrollStep(object instanceObj)
        {
            return (object) ((ScrollingData) instanceObj).ScrollStep;
        }

        private static void SetScrollStep(ref object instanceObj, object valueObj)
        {
            ScrollingData scrollingData = (ScrollingData) instanceObj;
            int num = (int) valueObj;
            Result result = Int32Schema.ValidateNotNegative((object) num);
            if (result.Failed)
            {
                ErrorManager.ReportError(result.Error);
            }
            else
            {
                scrollingData.ScrollStep = num;
                instanceObj = (object) scrollingData;
            }
        }

        private static object GetPageStep(object instanceObj)
        {
            return (object) ((ScrollingData) instanceObj).PageStep;
        }

        private static void SetPageStep(ref object instanceObj, object valueObj)
        {
            ScrollingData scrollingData = (ScrollingData) instanceObj;
            float num = (float) valueObj;
            Result result = SingleSchema.ValidateNotNegative((object) num);
            if (result.Failed)
            {
                ErrorManager.ReportError(result.Error);
            }
            else
            {
                scrollingData.PageStep = num;
                instanceObj = (object) scrollingData;
            }
        }

        private static object GetPageSizedScrollStep(object instanceObj)
        {
            return BooleanBoxes.Box(((ScrollingData) instanceObj).PageSizedScrollStep);
        }

        private static void SetPageSizedScrollStep(ref object instanceObj, object valueObj)
        {
            ScrollingData scrollingData = (ScrollingData) instanceObj;
            bool flag = (bool) valueObj;
            scrollingData.PageSizedScrollStep = flag;
            instanceObj = (object) scrollingData;
        }

        private static object GetBeginPadding(object instanceObj)
        {
            return (object) ((ScrollingData) instanceObj).BeginPadding;
        }

        private static void SetBeginPadding(ref object instanceObj, object valueObj)
        {
            ScrollingData scrollingData = (ScrollingData) instanceObj;
            int num = (int) valueObj;
            scrollingData.BeginPadding = num;
            instanceObj = (object) scrollingData;
        }

        private static object GetEndPadding(object instanceObj)
        {
            return (object) ((ScrollingData) instanceObj).EndPadding;
        }

        private static void SetEndPadding(ref object instanceObj, object valueObj)
        {
            ScrollingData scrollingData = (ScrollingData) instanceObj;
            int num = (int) valueObj;
            scrollingData.EndPadding = num;
            instanceObj = (object) scrollingData;
        }

        private static object GetBeginPaddingRelativeTo(object instanceObj)
        {
            return (object) ((ScrollingData) instanceObj).BeginPaddingRelativeTo;
        }

        private static void SetBeginPaddingRelativeTo(ref object instanceObj, object valueObj)
        {
            ScrollingData scrollingData = (ScrollingData) instanceObj;
            RelativeEdge relativeEdge = (RelativeEdge) valueObj;
            scrollingData.BeginPaddingRelativeTo = relativeEdge;
            instanceObj = (object) scrollingData;
        }

        private static object GetEndPaddingRelativeTo(object instanceObj)
        {
            return (object) ((ScrollingData) instanceObj).EndPaddingRelativeTo;
        }

        private static void SetEndPaddingRelativeTo(ref object instanceObj, object valueObj)
        {
            ScrollingData scrollingData = (ScrollingData) instanceObj;
            RelativeEdge relativeEdge = (RelativeEdge) valueObj;
            scrollingData.EndPaddingRelativeTo = relativeEdge;
            instanceObj = (object) scrollingData;
        }

        private static object GetLocked(object instanceObj)
        {
            return BooleanBoxes.Box(((ScrollingData) instanceObj).Locked);
        }

        private static void SetLocked(ref object instanceObj, object valueObj)
        {
            ScrollingData scrollingData = (ScrollingData) instanceObj;
            bool flag = (bool) valueObj;
            scrollingData.Locked = flag;
            instanceObj = (object) scrollingData;
        }

        private static object GetLockedPosition(object instanceObj)
        {
            return (object) ((ScrollingData) instanceObj).LockedPosition;
        }

        private static void SetLockedPosition(ref object instanceObj, object valueObj)
        {
            ScrollingData scrollingData = (ScrollingData) instanceObj;
            float num = (float) valueObj;
            scrollingData.LockedPosition = num;
            instanceObj = (object) scrollingData;
        }

        private static object GetLockedAlignment(object instanceObj)
        {
            return (object) ((ScrollingData) instanceObj).LockedAlignment;
        }

        private static void SetLockedAlignment(ref object instanceObj, object valueObj)
        {
            ScrollingData scrollingData = (ScrollingData) instanceObj;
            float num = (float) valueObj;
            scrollingData.LockedAlignment = num;
            instanceObj = (object) scrollingData;
        }

        private static object GetContentPositioningBehavior(object instanceObj)
        {
            return (object) ((ScrollingData) instanceObj).ContentPositioningBehavior;
        }

        private static void SetContentPositioningBehavior(ref object instanceObj, object valueObj)
        {
            ScrollingData scrollingData = (ScrollingData) instanceObj;
            ContentPositioningPolicy positioningPolicy = (ContentPositioningPolicy) valueObj;
            scrollingData.ContentPositioningBehavior = positioningPolicy;
            instanceObj = (object) scrollingData;
        }

        private static object GetCanScrollUp(object instanceObj)
        {
            return BooleanBoxes.Box(((ScrollingData) instanceObj).CanScrollUp);
        }

        private static object GetCanScrollDown(object instanceObj)
        {
            return BooleanBoxes.Box(((ScrollingData) instanceObj).CanScrollDown);
        }

        private static object GetCurrentPage(object instanceObj)
        {
            return (object) ((ScrollingData) instanceObj).CurrentPage;
        }

        private static object GetTotalPages(object instanceObj)
        {
            return (object) ((ScrollingData) instanceObj).TotalPages;
        }

        private static object CallScrollInt32(object instanceObj, object[] parameters)
        {
            ((ScrollingData) instanceObj).Scroll((int) parameters[0]);
            return (object) null;
        }

        private static object CallScrollFocusIntoView(object instanceObj, object[] parameters)
        {
            ((ScrollingData) instanceObj).ScrollFocusIntoView();
            return (object) null;
        }

        private static object CallScrollUp(object instanceObj, object[] parameters)
        {
            ((ScrollingData) instanceObj).ScrollUp();
            return (object) null;
        }

        private static object CallScrollDown(object instanceObj, object[] parameters)
        {
            ((ScrollingData) instanceObj).ScrollDown();
            return (object) null;
        }

        private static object CallPageUp(object instanceObj, object[] parameters)
        {
            ((ScrollingData) instanceObj).PageUp();
            return (object) null;
        }

        private static object CallPageDown(object instanceObj, object[] parameters)
        {
            ((ScrollingData) instanceObj).PageDown();
            return (object) null;
        }

        private static object CallHome(object instanceObj, object[] parameters)
        {
            ((ScrollingData) instanceObj).Home();
            return (object) null;
        }

        private static object CallEnd(object instanceObj, object[] parameters)
        {
            ((ScrollingData) instanceObj).End();
            return (object) null;
        }

        private static object CallScrollToPositionSingle(object instanceObj, object[] parameters)
        {
            ((ScrollingData) instanceObj).ScrollToPosition((float) parameters[0]);
            return (object) null;
        }

        public static void Pass2Initialize()
        {
            ScrollingDataSchema.EnabledProperty = (PropertySchema) new UIXPropertySchema(ScrollingDataSchema.Type, "Enabled", (TypeSchema) BooleanSchema.Type, (TypeSchema) null, ExpressionRestriction.None, false, (RangeValidator) null, true, new GetValueHandler(ScrollingDataSchema.GetEnabled), new SetValueHandler(ScrollingDataSchema.SetEnabled), false);
            ScrollingDataSchema.ScrollStepProperty = (PropertySchema) new UIXPropertySchema(ScrollingDataSchema.Type, "ScrollStep", (TypeSchema) Int32Schema.Type, (TypeSchema) null, ExpressionRestriction.None, false, Int32Schema.ValidateNotNegative, true, new GetValueHandler(ScrollingDataSchema.GetScrollStep), new SetValueHandler(ScrollingDataSchema.SetScrollStep), false);
            ScrollingDataSchema.PageStepProperty = (PropertySchema) new UIXPropertySchema(ScrollingDataSchema.Type, "PageStep", (TypeSchema) SingleSchema.Type, (TypeSchema) null, ExpressionRestriction.None, false, SingleSchema.ValidateNotNegative, true, new GetValueHandler(ScrollingDataSchema.GetPageStep), new SetValueHandler(ScrollingDataSchema.SetPageStep), false);
            ScrollingDataSchema.PageSizedScrollStepProperty = (PropertySchema) new UIXPropertySchema(ScrollingDataSchema.Type, "PageSizedScrollStep", (TypeSchema) BooleanSchema.Type, (TypeSchema) null, ExpressionRestriction.None, false, (RangeValidator) null, true, new GetValueHandler(ScrollingDataSchema.GetPageSizedScrollStep), new SetValueHandler(ScrollingDataSchema.SetPageSizedScrollStep), false);
            ScrollingDataSchema.BeginPaddingProperty = (PropertySchema) new UIXPropertySchema(ScrollingDataSchema.Type, "BeginPadding", (TypeSchema) Int32Schema.Type, (TypeSchema) null, ExpressionRestriction.None, false, (RangeValidator) null, true, new GetValueHandler(ScrollingDataSchema.GetBeginPadding), new SetValueHandler(ScrollingDataSchema.SetBeginPadding), false);
            ScrollingDataSchema.EndPaddingProperty = (PropertySchema) new UIXPropertySchema(ScrollingDataSchema.Type, "EndPadding", (TypeSchema) Int32Schema.Type, (TypeSchema) null, ExpressionRestriction.None, false, (RangeValidator) null, true, new GetValueHandler(ScrollingDataSchema.GetEndPadding), new SetValueHandler(ScrollingDataSchema.SetEndPadding), false);
            ScrollingDataSchema.BeginPaddingRelativeToProperty = (PropertySchema) new UIXPropertySchema(ScrollingDataSchema.Type, "BeginPaddingRelativeTo", (TypeSchema) RelativeEdgeSchema.Type, (TypeSchema) null, ExpressionRestriction.None, false, (RangeValidator) null, true, new GetValueHandler(ScrollingDataSchema.GetBeginPaddingRelativeTo), new SetValueHandler(ScrollingDataSchema.SetBeginPaddingRelativeTo), false);
            ScrollingDataSchema.EndPaddingRelativeToProperty = (PropertySchema) new UIXPropertySchema(ScrollingDataSchema.Type, "EndPaddingRelativeTo", (TypeSchema) RelativeEdgeSchema.Type, (TypeSchema) null, ExpressionRestriction.None, false, (RangeValidator) null, true, new GetValueHandler(ScrollingDataSchema.GetEndPaddingRelativeTo), new SetValueHandler(ScrollingDataSchema.SetEndPaddingRelativeTo), false);
            ScrollingDataSchema.LockedProperty = (PropertySchema) new UIXPropertySchema(ScrollingDataSchema.Type, "Locked", (TypeSchema) BooleanSchema.Type, (TypeSchema) null, ExpressionRestriction.None, false, (RangeValidator) null, true, new GetValueHandler(ScrollingDataSchema.GetLocked), new SetValueHandler(ScrollingDataSchema.SetLocked), false);
            ScrollingDataSchema.LockedPositionProperty = (PropertySchema) new UIXPropertySchema(ScrollingDataSchema.Type, "LockedPosition", (TypeSchema) SingleSchema.Type, (TypeSchema) null, ExpressionRestriction.None, false, (RangeValidator) null, true, new GetValueHandler(ScrollingDataSchema.GetLockedPosition), new SetValueHandler(ScrollingDataSchema.SetLockedPosition), false);
            ScrollingDataSchema.LockedAlignmentProperty = (PropertySchema) new UIXPropertySchema(ScrollingDataSchema.Type, "LockedAlignment", (TypeSchema) SingleSchema.Type, (TypeSchema) null, ExpressionRestriction.None, false, (RangeValidator) null, true, new GetValueHandler(ScrollingDataSchema.GetLockedAlignment), new SetValueHandler(ScrollingDataSchema.SetLockedAlignment), false);
            ScrollingDataSchema.ContentPositioningBehaviorProperty = (PropertySchema) new UIXPropertySchema(ScrollingDataSchema.Type, "ContentPositioningBehavior", (TypeSchema) ContentPositioningPolicySchema.Type, (TypeSchema) null, ExpressionRestriction.None, false, (RangeValidator) null, true, new GetValueHandler(ScrollingDataSchema.GetContentPositioningBehavior), new SetValueHandler(ScrollingDataSchema.SetContentPositioningBehavior), false);
            ScrollingDataSchema.CanScrollUpProperty = (PropertySchema) new UIXPropertySchema(ScrollingDataSchema.Type, "CanScrollUp", (TypeSchema) BooleanSchema.Type, (TypeSchema) null, ExpressionRestriction.ReadOnly, false, (RangeValidator) null, true, new GetValueHandler(ScrollingDataSchema.GetCanScrollUp), (SetValueHandler) null, false);
            ScrollingDataSchema.CanScrollDownProperty = (PropertySchema) new UIXPropertySchema(ScrollingDataSchema.Type, "CanScrollDown", (TypeSchema) BooleanSchema.Type, (TypeSchema) null, ExpressionRestriction.ReadOnly, false, (RangeValidator) null, true, new GetValueHandler(ScrollingDataSchema.GetCanScrollDown), (SetValueHandler) null, false);
            ScrollingDataSchema.CurrentPageProperty = (PropertySchema) new UIXPropertySchema(ScrollingDataSchema.Type, "CurrentPage", (TypeSchema) SingleSchema.Type, (TypeSchema) null, ExpressionRestriction.ReadOnly, false, (RangeValidator) null, true, new GetValueHandler(ScrollingDataSchema.GetCurrentPage), (SetValueHandler) null, false);
            ScrollingDataSchema.TotalPagesProperty = (PropertySchema) new UIXPropertySchema(ScrollingDataSchema.Type, "TotalPages", (TypeSchema) SingleSchema.Type, (TypeSchema) null, ExpressionRestriction.ReadOnly, false, (RangeValidator) null, true, new GetValueHandler(ScrollingDataSchema.GetTotalPages), (SetValueHandler) null, false);
            ScrollingDataSchema.ScrollInt32Method = (MethodSchema) new UIXMethodSchema(ScrollingDataSchema.Type, "Scroll", new TypeSchema[1]
            {
                (TypeSchema) Int32Schema.Type
            }, (TypeSchema) VoidSchema.Type, new InvokeHandler(ScrollingDataSchema.CallScrollInt32), false);
            ScrollingDataSchema.ScrollFocusIntoViewMethod = (MethodSchema) new UIXMethodSchema(ScrollingDataSchema.Type, "ScrollFocusIntoView", TypeSchema.EmptyList, (TypeSchema) VoidSchema.Type, new InvokeHandler(ScrollingDataSchema.CallScrollFocusIntoView), false);
            ScrollingDataSchema.ScrollUpMethod = (MethodSchema) new UIXMethodSchema(ScrollingDataSchema.Type, "ScrollUp", TypeSchema.EmptyList, (TypeSchema) VoidSchema.Type, new InvokeHandler(ScrollingDataSchema.CallScrollUp), false);
            ScrollingDataSchema.ScrollDownMethod = (MethodSchema) new UIXMethodSchema(ScrollingDataSchema.Type, "ScrollDown", TypeSchema.EmptyList, (TypeSchema) VoidSchema.Type, new InvokeHandler(ScrollingDataSchema.CallScrollDown), false);
            ScrollingDataSchema.PageUpMethod = (MethodSchema) new UIXMethodSchema(ScrollingDataSchema.Type, "PageUp", TypeSchema.EmptyList, (TypeSchema) VoidSchema.Type, new InvokeHandler(ScrollingDataSchema.CallPageUp), false);
            ScrollingDataSchema.PageDownMethod = (MethodSchema) new UIXMethodSchema(ScrollingDataSchema.Type, "PageDown", TypeSchema.EmptyList, (TypeSchema) VoidSchema.Type, new InvokeHandler(ScrollingDataSchema.CallPageDown), false);
            ScrollingDataSchema.HomeMethod = (MethodSchema) new UIXMethodSchema(ScrollingDataSchema.Type, "Home", TypeSchema.EmptyList, (TypeSchema) VoidSchema.Type, new InvokeHandler(ScrollingDataSchema.CallHome), false);
            ScrollingDataSchema.EndMethod = (MethodSchema) new UIXMethodSchema(ScrollingDataSchema.Type, "End", TypeSchema.EmptyList, (TypeSchema) VoidSchema.Type, new InvokeHandler(ScrollingDataSchema.CallEnd), false);
            ScrollingDataSchema.ScrollToPositionSingleMethod = (MethodSchema) new UIXMethodSchema(ScrollingDataSchema.Type, "ScrollToPosition", new TypeSchema[1]
            {
                (TypeSchema) SingleSchema.Type
            }, (TypeSchema) VoidSchema.Type, new InvokeHandler(ScrollingDataSchema.CallScrollToPositionSingle), false);
            ScrollingDataSchema.Type.Initialize((TypeSchema) ObjectSchema.Type, false, new DefaultConstructHandler(ScrollingDataSchema.Construct), ConstructorSchema.EmptyList, new PropertySchema[16]
            {
                ScrollingDataSchema.BeginPaddingProperty,
                ScrollingDataSchema.BeginPaddingRelativeToProperty,
                ScrollingDataSchema.CanScrollDownProperty,
                ScrollingDataSchema.CanScrollUpProperty,
                ScrollingDataSchema.ContentPositioningBehaviorProperty,
                ScrollingDataSchema.CurrentPageProperty,
                ScrollingDataSchema.EnabledProperty,
                ScrollingDataSchema.EndPaddingProperty,
                ScrollingDataSchema.EndPaddingRelativeToProperty,
                ScrollingDataSchema.LockedProperty,
                ScrollingDataSchema.LockedAlignmentProperty,
                ScrollingDataSchema.LockedPositionProperty,
                ScrollingDataSchema.PageSizedScrollStepProperty,
                ScrollingDataSchema.PageStepProperty,
                ScrollingDataSchema.ScrollStepProperty,
                ScrollingDataSchema.TotalPagesProperty
            }, new MethodSchema[9]
            {
                ScrollingDataSchema.ScrollInt32Method,
                ScrollingDataSchema.ScrollFocusIntoViewMethod,
                ScrollingDataSchema.ScrollUpMethod,
                ScrollingDataSchema.ScrollDownMethod,
                ScrollingDataSchema.PageUpMethod,
                ScrollingDataSchema.PageDownMethod,
                ScrollingDataSchema.HomeMethod,
                ScrollingDataSchema.EndMethod,
                ScrollingDataSchema.ScrollToPositionSingleMethod
            }, EventSchema.EmptyList, (FindCanonicalInstanceHandler) null, (TypeConverterHandler) null, (SupportsTypeConversionHandler) null, (EncodeBinaryHandler) null, (DecodeBinaryHandler) null, (PerformOperationHandler) null, (SupportsOperationHandler) null, typeof (ScrollingData), false);
        }
    }
}
