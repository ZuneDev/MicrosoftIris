// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.ScrollingDataSchema
// Assembly: UIX, Version=2.1.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: 57C02C21-1B9A-4AE2-836C-8816A259A92A
// Assembly location: D:\Downloads\Zune Software v2.5\packages\Zune-x86\UIX.dll

using Microsoft.Iris.Layouts;
using Microsoft.Iris.Library;
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
        public static UIXTypeSchema Type;

        private static object Construct() => new ScrollingData();

        private static object GetEnabled(object instanceObj)
        {
            return BooleanBoxes.Box(((ScrollingData)instanceObj).Enabled);
        }

        private static void SetEnabled(ref object instanceObj, object valueObj)
        {
            ScrollingData scrollingData = (ScrollingData)instanceObj;
            bool flag = (bool)valueObj;
            scrollingData.Enabled = flag;
            instanceObj = scrollingData;
        }

        private static object GetScrollStep(object instanceObj)
        {
            return ((ScrollingData)instanceObj).ScrollStep;
        }

        private static void SetScrollStep(ref object instanceObj, object valueObj)
        {
            ScrollingData scrollingData = (ScrollingData)instanceObj;
            int num = (int)valueObj;
            Result result = Int32Schema.ValidateNotNegative(num);
            if (result.Failed)
            {
                ErrorManager.ReportError(result.Error);
            }
            else
            {
                scrollingData.ScrollStep = num;
                instanceObj = scrollingData;
            }
        }

        private static object GetPageStep(object instanceObj)
        {
            return ((ScrollingData)instanceObj).PageStep;
        }

        private static void SetPageStep(ref object instanceObj, object valueObj)
        {
            ScrollingData scrollingData = (ScrollingData)instanceObj;
            float num = (float)valueObj;
            Result result = SingleSchema.ValidateNotNegative(num);
            if (result.Failed)
            {
                ErrorManager.ReportError(result.Error);
            }
            else
            {
                scrollingData.PageStep = num;
                instanceObj = scrollingData;
            }
        }

        private static object GetPageSizedScrollStep(object instanceObj)
        {
            return BooleanBoxes.Box(((ScrollingData)instanceObj).PageSizedScrollStep);
        }

        private static void SetPageSizedScrollStep(ref object instanceObj, object valueObj)
        {
            ScrollingData scrollingData = (ScrollingData)instanceObj;
            bool flag = (bool)valueObj;
            scrollingData.PageSizedScrollStep = flag;
            instanceObj = scrollingData;
        }

        private static object GetBeginPadding(object instanceObj)
        {
            return ((ScrollingData)instanceObj).BeginPadding;
        }

        private static void SetBeginPadding(ref object instanceObj, object valueObj)
        {
            ScrollingData scrollingData = (ScrollingData)instanceObj;
            int num = (int)valueObj;
            scrollingData.BeginPadding = num;
            instanceObj = scrollingData;
        }

        private static object GetEndPadding(object instanceObj)
        {
            return ((ScrollingData)instanceObj).EndPadding;
        }

        private static void SetEndPadding(ref object instanceObj, object valueObj)
        {
            ScrollingData scrollingData = (ScrollingData)instanceObj;
            int num = (int)valueObj;
            scrollingData.EndPadding = num;
            instanceObj = scrollingData;
        }

        private static object GetBeginPaddingRelativeTo(object instanceObj)
        {
            return ((ScrollingData)instanceObj).BeginPaddingRelativeTo;
        }

        private static void SetBeginPaddingRelativeTo(ref object instanceObj, object valueObj)
        {
            ScrollingData scrollingData = (ScrollingData)instanceObj;
            RelativeEdge relativeEdge = (RelativeEdge)valueObj;
            scrollingData.BeginPaddingRelativeTo = relativeEdge;
            instanceObj = scrollingData;
        }

        private static object GetEndPaddingRelativeTo(object instanceObj)
        {
            return ((ScrollingData)instanceObj).EndPaddingRelativeTo;
        }

        private static void SetEndPaddingRelativeTo(ref object instanceObj, object valueObj)
        {
            ScrollingData scrollingData = (ScrollingData)instanceObj;
            RelativeEdge relativeEdge = (RelativeEdge)valueObj;
            scrollingData.EndPaddingRelativeTo = relativeEdge;
            instanceObj = scrollingData;
        }

        private static object GetLocked(object instanceObj)
        {
            return BooleanBoxes.Box(((ScrollingData)instanceObj).Locked);
        }

        private static void SetLocked(ref object instanceObj, object valueObj)
        {
            ScrollingData scrollingData = (ScrollingData)instanceObj;
            bool flag = (bool)valueObj;
            scrollingData.Locked = flag;
            instanceObj = scrollingData;
        }

        private static object GetLockedPosition(object instanceObj)
        {
            return ((ScrollingData)instanceObj).LockedPosition;
        }

        private static void SetLockedPosition(ref object instanceObj, object valueObj)
        {
            ScrollingData scrollingData = (ScrollingData)instanceObj;
            float num = (float)valueObj;
            scrollingData.LockedPosition = num;
            instanceObj = scrollingData;
        }

        private static object GetLockedAlignment(object instanceObj)
        {
            return ((ScrollingData)instanceObj).LockedAlignment;
        }

        private static void SetLockedAlignment(ref object instanceObj, object valueObj)
        {
            ScrollingData scrollingData = (ScrollingData)instanceObj;
            float num = (float)valueObj;
            scrollingData.LockedAlignment = num;
            instanceObj = scrollingData;
        }

        private static object GetContentPositioningBehavior(object instanceObj)
        {
            return ((ScrollingData)instanceObj).ContentPositioningBehavior;
        }

        private static void SetContentPositioningBehavior(ref object instanceObj, object valueObj)
        {
            ScrollingData scrollingData = (ScrollingData)instanceObj;
            ContentPositioningPolicy positioningPolicy = (ContentPositioningPolicy)valueObj;
            scrollingData.ContentPositioningBehavior = positioningPolicy;
            instanceObj = scrollingData;
        }

        private static object GetCanScrollUp(object instanceObj)
        {
            return BooleanBoxes.Box(((ScrollingData)instanceObj).CanScrollUp);
        }

        private static object GetCanScrollDown(object instanceObj)
        {
            return BooleanBoxes.Box(((ScrollingData)instanceObj).CanScrollDown);
        }

        private static object GetCurrentPage(object instanceObj)
        {
            return ((ScrollingData)instanceObj).CurrentPage;
        }

        private static object GetTotalPages(object instanceObj)
        {
            return ((ScrollingData)instanceObj).TotalPages;
        }

        private static object CallScrollInt32(object instanceObj, object[] parameters)
        {
            ((ScrollingData)instanceObj).Scroll((int)parameters[0]);
            return null;
        }

        private static object CallScrollFocusIntoView(object instanceObj, object[] parameters)
        {
            ((ScrollingData)instanceObj).ScrollFocusIntoView();
            return null;
        }

        private static object CallScrollUp(object instanceObj, object[] parameters)
        {
            ((ScrollingData)instanceObj).ScrollUp();
            return null;
        }

        private static object CallScrollDown(object instanceObj, object[] parameters)
        {
            ((ScrollingData)instanceObj).ScrollDown();
            return null;
        }

        private static object CallPageUp(object instanceObj, object[] parameters)
        {
            ((ScrollingData)instanceObj).PageUp();
            return null;
        }

        private static object CallPageDown(object instanceObj, object[] parameters)
        {
            ((ScrollingData)instanceObj).PageDown();
            return null;
        }

        private static object CallHome(object instanceObj, object[] parameters)
        {
            ((ScrollingData)instanceObj).Home();
            return null;
        }

        private static object CallEnd(object instanceObj, object[] parameters)
        {
            ((ScrollingData)instanceObj).End();
            return null;
        }

        private static object CallScrollToPositionSingle(object instanceObj, object[] parameters)
        {
            ((ScrollingData)instanceObj).ScrollToPosition((float)parameters[0]);
            return null;
        }

        public static void Pass1Initialize() => Type = new UIXTypeSchema(UIXTypeID.ScrollingData, "ScrollingData", null, UIXTypeID.Object, typeof(ScrollingData), UIXTypeFlags.None);

        public static void Pass2Initialize()
        {
            var ownerTypeId = UIXTypeID.ScrollingData;
            EnabledProperty = new UIXPropertySchema(ownerTypeId, "Enabled", UIXTypeID.Boolean, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetEnabled), new SetValueHandler(SetEnabled), false);
            ScrollStepProperty = new UIXPropertySchema(ownerTypeId, "ScrollStep", UIXTypeID.Int32, -1, ExpressionRestriction.None, false, Int32Schema.ValidateNotNegative, true, new GetValueHandler(GetScrollStep), new SetValueHandler(SetScrollStep), false);
            PageStepProperty = new UIXPropertySchema(ownerTypeId, "PageStep", UIXTypeID.Single, -1, ExpressionRestriction.None, false, SingleSchema.ValidateNotNegative, true, new GetValueHandler(GetPageStep), new SetValueHandler(SetPageStep), false);
            PageSizedScrollStepProperty = new UIXPropertySchema(ownerTypeId, "PageSizedScrollStep", UIXTypeID.Boolean, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetPageSizedScrollStep), new SetValueHandler(SetPageSizedScrollStep), false);
            BeginPaddingProperty = new UIXPropertySchema(ownerTypeId, "BeginPadding", UIXTypeID.Int32, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetBeginPadding), new SetValueHandler(SetBeginPadding), false);
            EndPaddingProperty = new UIXPropertySchema(ownerTypeId, "EndPadding", UIXTypeID.Int32, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetEndPadding), new SetValueHandler(SetEndPadding), false);
            BeginPaddingRelativeToProperty = new UIXPropertySchema(ownerTypeId, "BeginPaddingRelativeTo", UIXTypeID.RelativeEdge, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetBeginPaddingRelativeTo), new SetValueHandler(SetBeginPaddingRelativeTo), false);
            EndPaddingRelativeToProperty = new UIXPropertySchema(ownerTypeId, "EndPaddingRelativeTo", UIXTypeID.RelativeEdge, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetEndPaddingRelativeTo), new SetValueHandler(SetEndPaddingRelativeTo), false);
            LockedProperty = new UIXPropertySchema(ownerTypeId, "Locked", UIXTypeID.Boolean, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetLocked), new SetValueHandler(SetLocked), false);
            LockedPositionProperty = new UIXPropertySchema(ownerTypeId, "LockedPosition", UIXTypeID.Single, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetLockedPosition), new SetValueHandler(SetLockedPosition), false);
            LockedAlignmentProperty = new UIXPropertySchema(ownerTypeId, "LockedAlignment", UIXTypeID.Single, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetLockedAlignment), new SetValueHandler(SetLockedAlignment), false);
            ContentPositioningBehaviorProperty = new UIXPropertySchema(ownerTypeId, "ContentPositioningBehavior", UIXTypeID.ContentPositioningPolicy, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetContentPositioningBehavior), new SetValueHandler(SetContentPositioningBehavior), false);
            CanScrollUpProperty = new UIXPropertySchema(ownerTypeId, "CanScrollUp", UIXTypeID.Boolean, -1, ExpressionRestriction.ReadOnly, false, null, true, new GetValueHandler(GetCanScrollUp), null, false);
            CanScrollDownProperty = new UIXPropertySchema(ownerTypeId, "CanScrollDown", UIXTypeID.Boolean, -1, ExpressionRestriction.ReadOnly, false, null, true, new GetValueHandler(GetCanScrollDown), null, false);
            CurrentPageProperty = new UIXPropertySchema(ownerTypeId, "CurrentPage", UIXTypeID.Single, -1, ExpressionRestriction.ReadOnly, false, null, true, new GetValueHandler(GetCurrentPage), null, false);
            TotalPagesProperty = new UIXPropertySchema(ownerTypeId, "TotalPages", UIXTypeID.Single, -1, ExpressionRestriction.ReadOnly, false, null, true, new GetValueHandler(GetTotalPages), null, false);

            ScrollInt32Method = new UIXMethodSchema(ownerTypeId, "Scroll", [UIXTypeID.Int32], UIXTypeID.Void, new InvokeHandler(CallScrollInt32), false);
            ScrollFocusIntoViewMethod = new UIXMethodSchema(ownerTypeId, "ScrollFocusIntoView", [], UIXTypeID.Void, new InvokeHandler(CallScrollFocusIntoView), false);
            ScrollUpMethod = new UIXMethodSchema(ownerTypeId, "ScrollUp", [], UIXTypeID.Void, new InvokeHandler(CallScrollUp), false);
            ScrollDownMethod = new UIXMethodSchema(ownerTypeId, "ScrollDown", [], UIXTypeID.Void, new InvokeHandler(CallScrollDown), false);
            PageUpMethod = new UIXMethodSchema(ownerTypeId, "PageUp", [], UIXTypeID.Void, new InvokeHandler(CallPageUp), false);
            PageDownMethod = new UIXMethodSchema(ownerTypeId, "PageDown", [], UIXTypeID.Void, new InvokeHandler(CallPageDown), false);
            HomeMethod = new UIXMethodSchema(ownerTypeId, "Home", [], UIXTypeID.Void, new InvokeHandler(CallHome), false);
            EndMethod = new UIXMethodSchema(ownerTypeId, "End", [], UIXTypeID.Void, new InvokeHandler(CallEnd), false);
            ScrollToPositionSingleMethod = new UIXMethodSchema(ownerTypeId, "ScrollToPosition", [UIXTypeID.Single], UIXTypeID.Void, new InvokeHandler(CallScrollToPositionSingle), false);

            Type.Initialize(Construct, ConstructorSchema.EmptyList,
            [   // Properties
                BeginPaddingProperty,
                BeginPaddingRelativeToProperty,
                CanScrollDownProperty,
                CanScrollUpProperty,
                ContentPositioningBehaviorProperty,
                CurrentPageProperty,
                EnabledProperty,
                EndPaddingProperty,
                EndPaddingRelativeToProperty,
                LockedProperty,
                LockedAlignmentProperty,
                LockedPositionProperty,
                PageSizedScrollStepProperty,
                PageStepProperty,
                ScrollStepProperty,
                TotalPagesProperty
            ],
            [   // Methods
                ScrollInt32Method,
                ScrollFocusIntoViewMethod,
                ScrollUpMethod,
                ScrollDownMethod,
                PageUpMethod,
                PageDownMethod,
                HomeMethod,
                EndMethod,
                ScrollToPositionSingleMethod
            ], EventSchema.EmptyList, null, null, null, null, null, null, null);
        }
    }
}
