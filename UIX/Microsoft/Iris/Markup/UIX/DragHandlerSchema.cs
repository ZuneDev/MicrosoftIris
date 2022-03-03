// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.DragHandlerSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Input;
using Microsoft.Iris.InputHandlers;
using Microsoft.Iris.UI;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class DragHandlerSchema
    {
        public static UIXTypeSchema Type;

        private static object GetBeginDragPolicy(object instanceObj) => ((DragHandler)instanceObj).BeginDragPolicy;

        private static void SetBeginDragPolicy(ref object instanceObj, object valueObj) => ((DragHandler)instanceObj).BeginDragPolicy = (BeginDragPolicy)valueObj;

        private static object GetDragging(object instanceObj) => BooleanBoxes.Box(((DragHandler)instanceObj).Dragging);

        private static object GetBeginPosition(object instanceObj) => ((DragHandler)instanceObj).BeginPosition;

        private static object GetEndPosition(object instanceObj) => ((DragHandler)instanceObj).EndPosition;

        private static object GetScreenDragSize(object instanceObj) => ((DragHandler)instanceObj).ScreenDragSize;

        private static object GetLocalDragSize(object instanceObj) => ((DragHandler)instanceObj).LocalDragSize;

        private static object GetRelativeDragSize(object instanceObj) => ((DragHandler)instanceObj).RelativeDragSize;

        private static object GetActiveModifiers(object instanceObj) => ((DragHandler)instanceObj).ActiveModifiers;

        private static object GetDragCursor(object instanceObj) => ((DragHandler)instanceObj).DragCursor;

        private static void SetDragCursor(ref object instanceObj, object valueObj) => ((DragHandler)instanceObj).DragCursor = (CursorID)valueObj;

        private static object GetCancelOnEscape(object instanceObj) => BooleanBoxes.Box(((DragHandler)instanceObj).CancelOnEscape);

        private static void SetCancelOnEscape(ref object instanceObj, object valueObj) => ((DragHandler)instanceObj).CancelOnEscape = (bool)valueObj;

        private static object GetRelativeTo(object instanceObj) => ((DragHandler)instanceObj).RelativeTo;

        private static void SetRelativeTo(ref object instanceObj, object valueObj) => ((DragHandler)instanceObj).RelativeTo = (ViewItem)valueObj;

        private static object GetHandlerStage(object instanceObj) => ((InputHandler)instanceObj).HandlerStage;

        private static void SetHandlerStage(ref object instanceObj, object valueObj) => ((InputHandler)instanceObj).HandlerStage = (InputHandlerStage)valueObj;

        private static object Construct() => new DragHandler();

        private static object CallResetDragOrigin(object instanceObj, object[] parameters)
        {
            ((DragHandler)instanceObj).ResetDragOrigin();
            return null;
        }

        private static object CallCancelDrag(object instanceObj, object[] parameters)
        {
            ((DragHandler)instanceObj).CancelDrag();
            return null;
        }

        private static object CallGetEventContexts(object instanceObj, object[] parameters) => ((DragHandler)instanceObj).GetEventContexts();

        private static object CallGetAddedEventContexts(object instanceObj, object[] parameters) => ((DragHandler)instanceObj).GetAddedEventContexts();

        private static object CallGetRemovedEventContexts(object instanceObj, object[] parameters) => ((DragHandler)instanceObj).GetRemovedEventContexts();

        public static void Pass1Initialize() => Type = new UIXTypeSchema(62, "DragHandler", null, 110, typeof(DragHandler), UIXTypeFlags.Disposable);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(62, "BeginDragPolicy", 12, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetBeginDragPolicy), new SetValueHandler(SetBeginDragPolicy), false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(62, "Dragging", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetDragging), null, false);
            UIXPropertySchema uixPropertySchema3 = new UIXPropertySchema(62, "BeginPosition", 233, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetBeginPosition), null, false);
            UIXPropertySchema uixPropertySchema4 = new UIXPropertySchema(62, "EndPosition", 233, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetEndPosition), null, false);
            UIXPropertySchema uixPropertySchema5 = new UIXPropertySchema(62, "ScreenDragSize", 195, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetScreenDragSize), null, false);
            UIXPropertySchema uixPropertySchema6 = new UIXPropertySchema(62, "LocalDragSize", 233, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetLocalDragSize), null, false);
            UIXPropertySchema uixPropertySchema7 = new UIXPropertySchema(62, "RelativeDragSize", 233, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetRelativeDragSize), null, false);
            UIXPropertySchema uixPropertySchema8 = new UIXPropertySchema(62, "ActiveModifiers", 111, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetActiveModifiers), null, false);
            UIXPropertySchema uixPropertySchema9 = new UIXPropertySchema(62, "DragCursor", 44, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetDragCursor), new SetValueHandler(SetDragCursor), false);
            UIXPropertySchema uixPropertySchema10 = new UIXPropertySchema(62, "CancelOnEscape", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetCancelOnEscape), new SetValueHandler(SetCancelOnEscape), false);
            UIXPropertySchema uixPropertySchema11 = new UIXPropertySchema(62, "RelativeTo", 239, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetRelativeTo), new SetValueHandler(SetRelativeTo), false);
            UIXPropertySchema uixPropertySchema12 = new UIXPropertySchema(62, "HandlerStage", 112, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetHandlerStage), new SetValueHandler(SetHandlerStage), false);
            UIXMethodSchema uixMethodSchema1 = new UIXMethodSchema(62, "ResetDragOrigin", null, 240, new InvokeHandler(CallResetDragOrigin), false);
            UIXMethodSchema uixMethodSchema2 = new UIXMethodSchema(62, "CancelDrag", null, 240, new InvokeHandler(CallCancelDrag), false);
            UIXMethodSchema uixMethodSchema3 = new UIXMethodSchema(62, "GetEventContexts", null, 138, new InvokeHandler(CallGetEventContexts), false);
            UIXMethodSchema uixMethodSchema4 = new UIXMethodSchema(62, "GetAddedEventContexts", null, 138, new InvokeHandler(CallGetAddedEventContexts), false);
            UIXMethodSchema uixMethodSchema5 = new UIXMethodSchema(62, "GetRemovedEventContexts", null, 138, new InvokeHandler(CallGetRemovedEventContexts), false);
            UIXEventSchema uixEventSchema1 = new UIXEventSchema(62, "Started");
            UIXEventSchema uixEventSchema2 = new UIXEventSchema(62, "Canceled");
            UIXEventSchema uixEventSchema3 = new UIXEventSchema(62, "Ended");
            Type.Initialize(new DefaultConstructHandler(Construct), null, new PropertySchema[12]
            {
         uixPropertySchema8,
         uixPropertySchema1,
         uixPropertySchema3,
         uixPropertySchema10,
         uixPropertySchema9,
         uixPropertySchema2,
         uixPropertySchema4,
         uixPropertySchema12,
         uixPropertySchema6,
         uixPropertySchema7,
         uixPropertySchema11,
         uixPropertySchema5
            }, new MethodSchema[5]
            {
         uixMethodSchema1,
         uixMethodSchema2,
         uixMethodSchema3,
         uixMethodSchema4,
         uixMethodSchema5
            }, new EventSchema[3]
            {
         uixEventSchema1,
         uixEventSchema2,
         uixEventSchema3
            }, null, null, null, null, null, null, null);
        }
    }
}
