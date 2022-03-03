// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.ScrollModelBaseSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;
using Microsoft.Iris.ModelItems;
using Microsoft.Iris.Session;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class ScrollModelBaseSchema
    {
        public static UIXTypeSchema Type;

        private static object GetScrollStep(object instanceObj) => ((ScrollModelBase)instanceObj).ScrollStep;

        private static void SetScrollStep(ref object instanceObj, object valueObj)
        {
            ScrollModelBase scrollModelBase = (ScrollModelBase)instanceObj;
            int num = (int)valueObj;
            Result result = Int32Schema.ValidateNotNegative(valueObj);
            if (result.Failed)
                ErrorManager.ReportError(result.Error);
            else
                scrollModelBase.ScrollStep = num;
        }

        private static object GetCanScrollUp(object instanceObj) => BooleanBoxes.Box(((ScrollModelBase)instanceObj).CanScrollUp);

        private static object GetCanScrollDown(object instanceObj) => BooleanBoxes.Box(((ScrollModelBase)instanceObj).CanScrollDown);

        private static object GetCurrentPage(object instanceObj) => ((ScrollModelBase)instanceObj).CurrentPage;

        private static object GetTotalPages(object instanceObj) => ((ScrollModelBase)instanceObj).TotalPages;

        private static object GetViewNear(object instanceObj) => ((ScrollModelBase)instanceObj).ViewNear;

        private static object GetViewFar(object instanceObj) => ((ScrollModelBase)instanceObj).ViewFar;

        private static object CallScrollInt32(object instanceObj, object[] parameters)
        {
            ((ScrollModelBase)instanceObj).Scroll((int)parameters[0]);
            return null;
        }

        private static object CallScrollUp(object instanceObj, object[] parameters)
        {
            ((ScrollModelBase)instanceObj).ScrollUp();
            return null;
        }

        private static object CallScrollDown(object instanceObj, object[] parameters)
        {
            ((ScrollModelBase)instanceObj).ScrollDown();
            return null;
        }

        private static object CallPageUp(object instanceObj, object[] parameters)
        {
            ((ScrollModelBase)instanceObj).PageUp();
            return null;
        }

        private static object CallPageDown(object instanceObj, object[] parameters)
        {
            ((ScrollModelBase)instanceObj).PageDown();
            return null;
        }

        private static object CallHome(object instanceObj, object[] parameters)
        {
            ((ScrollModelBase)instanceObj).Home();
            return null;
        }

        private static object CallEnd(object instanceObj, object[] parameters)
        {
            ((ScrollModelBase)instanceObj).End();
            return null;
        }

        private static object CallScrollToPositionSingle(object instanceObj, object[] parameters)
        {
            ((ScrollModelBase)instanceObj).ScrollToPosition((float)parameters[0]);
            return null;
        }

        public static void Pass1Initialize() => Type = new UIXTypeSchema(183, "ScrollModelBase", null, 153, typeof(ScrollModelBase), UIXTypeFlags.None);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(183, "ScrollStep", 115, -1, ExpressionRestriction.None, false, Int32Schema.ValidateNotNegative, true, new GetValueHandler(GetScrollStep), new SetValueHandler(SetScrollStep), false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(183, "CanScrollUp", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetCanScrollUp), null, false);
            UIXPropertySchema uixPropertySchema3 = new UIXPropertySchema(183, "CanScrollDown", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetCanScrollDown), null, false);
            UIXPropertySchema uixPropertySchema4 = new UIXPropertySchema(183, "CurrentPage", 194, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetCurrentPage), null, false);
            UIXPropertySchema uixPropertySchema5 = new UIXPropertySchema(183, "TotalPages", 194, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetTotalPages), null, false);
            UIXPropertySchema uixPropertySchema6 = new UIXPropertySchema(183, "ViewNear", 194, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetViewNear), null, false);
            UIXPropertySchema uixPropertySchema7 = new UIXPropertySchema(183, "ViewFar", 194, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetViewFar), null, false);
            UIXMethodSchema uixMethodSchema1 = new UIXMethodSchema(183, "Scroll", new short[1]
            {
         115
            }, 240, new InvokeHandler(CallScrollInt32), false);
            UIXMethodSchema uixMethodSchema2 = new UIXMethodSchema(183, "ScrollUp", null, 240, new InvokeHandler(CallScrollUp), false);
            UIXMethodSchema uixMethodSchema3 = new UIXMethodSchema(183, "ScrollDown", null, 240, new InvokeHandler(CallScrollDown), false);
            UIXMethodSchema uixMethodSchema4 = new UIXMethodSchema(183, "PageUp", null, 240, new InvokeHandler(CallPageUp), false);
            UIXMethodSchema uixMethodSchema5 = new UIXMethodSchema(183, "PageDown", null, 240, new InvokeHandler(CallPageDown), false);
            UIXMethodSchema uixMethodSchema6 = new UIXMethodSchema(183, "Home", null, 240, new InvokeHandler(CallHome), false);
            UIXMethodSchema uixMethodSchema7 = new UIXMethodSchema(183, "End", null, 240, new InvokeHandler(CallEnd), false);
            UIXMethodSchema uixMethodSchema8 = new UIXMethodSchema(183, "ScrollToPosition", new short[1]
            {
         194
            }, 240, new InvokeHandler(CallScrollToPositionSingle), false);
            Type.Initialize(null, null, new PropertySchema[7]
            {
         uixPropertySchema3,
         uixPropertySchema2,
         uixPropertySchema4,
         uixPropertySchema1,
         uixPropertySchema5,
         uixPropertySchema7,
         uixPropertySchema6
            }, new MethodSchema[8]
            {
         uixMethodSchema1,
         uixMethodSchema2,
         uixMethodSchema3,
         uixMethodSchema4,
         uixMethodSchema5,
         uixMethodSchema6,
         uixMethodSchema7,
         uixMethodSchema8
            }, null, null, null, null, null, null, null, null);
        }
    }
}
