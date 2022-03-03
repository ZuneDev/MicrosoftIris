// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.DragSourceHandlerSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Input;
using Microsoft.Iris.InputHandlers;
using Microsoft.Iris.UI;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class DragSourceHandlerSchema
    {
        public static UIXTypeSchema Type;

        private static object GetAllowedDropActions(object instanceObj) => ((DragSourceHandler)instanceObj).AllowedDropActions;

        private static void SetAllowedDropActions(ref object instanceObj, object valueObj) => ((DragSourceHandler)instanceObj).AllowedDropActions = (DropAction)valueObj;

        private static object GetCurrentDropAction(object instanceObj) => ((DragSourceHandler)instanceObj).CurrentDropAction;

        private static object GetValue(object instanceObj) => ((DragSourceHandler)instanceObj).Value;

        private static void SetValue(ref object instanceObj, object valueObj) => ((DragSourceHandler)instanceObj).Value = valueObj;

        private static object GetDragging(object instanceObj) => BooleanBoxes.Box(((DragSourceHandler)instanceObj).Dragging);

        private static object GetHandlerStage(object instanceObj) => ((InputHandler)instanceObj).HandlerStage;

        private static void SetHandlerStage(ref object instanceObj, object valueObj) => ((InputHandler)instanceObj).HandlerStage = (InputHandlerStage)valueObj;

        private static object GetMoveCursor(object instanceObj) => ((DragSourceHandler)instanceObj).MoveCursor;

        private static void SetMoveCursor(ref object instanceObj, object valueObj) => ((DragSourceHandler)instanceObj).MoveCursor = (CursorID)valueObj;

        private static object GetCopyCursor(object instanceObj) => ((DragSourceHandler)instanceObj).CopyCursor;

        private static void SetCopyCursor(ref object instanceObj, object valueObj) => ((DragSourceHandler)instanceObj).CopyCursor = (CursorID)valueObj;

        private static object GetCancelCursor(object instanceObj) => ((DragSourceHandler)instanceObj).CancelCursor;

        private static void SetCancelCursor(ref object instanceObj, object valueObj) => ((DragSourceHandler)instanceObj).CancelCursor = (CursorID)valueObj;

        private static object Construct() => new DragSourceHandler();

        public static void Pass1Initialize() => Type = new UIXTypeSchema(63, "DragSourceHandler", null, 110, typeof(DragSourceHandler), UIXTypeFlags.Disposable);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(63, "AllowedDropActions", 64, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetAllowedDropActions), new SetValueHandler(SetAllowedDropActions), false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(63, "CurrentDropAction", 64, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetCurrentDropAction), null, false);
            UIXPropertySchema uixPropertySchema3 = new UIXPropertySchema(63, "Value", 153, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetValue), new SetValueHandler(SetValue), false);
            UIXPropertySchema uixPropertySchema4 = new UIXPropertySchema(63, "Dragging", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetDragging), null, false);
            UIXPropertySchema uixPropertySchema5 = new UIXPropertySchema(63, "HandlerStage", 112, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetHandlerStage), new SetValueHandler(SetHandlerStage), false);
            UIXPropertySchema uixPropertySchema6 = new UIXPropertySchema(63, "MoveCursor", 44, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetMoveCursor), new SetValueHandler(SetMoveCursor), false);
            UIXPropertySchema uixPropertySchema7 = new UIXPropertySchema(63, "CopyCursor", 44, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetCopyCursor), new SetValueHandler(SetCopyCursor), false);
            UIXPropertySchema uixPropertySchema8 = new UIXPropertySchema(63, "CancelCursor", 44, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetCancelCursor), new SetValueHandler(SetCancelCursor), false);
            UIXEventSchema uixEventSchema1 = new UIXEventSchema(63, "Started");
            UIXEventSchema uixEventSchema2 = new UIXEventSchema(63, "Moved");
            UIXEventSchema uixEventSchema3 = new UIXEventSchema(63, "Copied");
            UIXEventSchema uixEventSchema4 = new UIXEventSchema(63, "Canceled");
            Type.Initialize(new DefaultConstructHandler(Construct), null, new PropertySchema[8]
            {
         uixPropertySchema1,
         uixPropertySchema8,
         uixPropertySchema7,
         uixPropertySchema2,
         uixPropertySchema4,
         uixPropertySchema5,
         uixPropertySchema6,
         uixPropertySchema3
            }, null, new EventSchema[4]
            {
         uixEventSchema1,
         uixEventSchema2,
         uixEventSchema3,
         uixEventSchema4
            }, null, null, null, null, null, null, null);
        }
    }
}
