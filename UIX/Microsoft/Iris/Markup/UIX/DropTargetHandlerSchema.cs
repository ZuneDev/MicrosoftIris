// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.DropTargetHandlerSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.InputHandlers;
using Microsoft.Iris.UI;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class DropTargetHandlerSchema
    {
        public static UIXTypeSchema Type;

        private static object GetAllowedDropActions(object instanceObj) => ((DropTargetHandler)instanceObj).AllowedDropActions;

        private static void SetAllowedDropActions(ref object instanceObj, object valueObj) => ((DropTargetHandler)instanceObj).AllowedDropActions = (DropAction)valueObj;

        private static object GetDragging(object instanceObj) => BooleanBoxes.Box(((DropTargetHandler)instanceObj).Dragging);

        private static object GetHandlerStage(object instanceObj) => ((InputHandler)instanceObj).HandlerStage;

        private static void SetHandlerStage(ref object instanceObj, object valueObj) => ((InputHandler)instanceObj).HandlerStage = (InputHandlerStage)valueObj;

        private static object GetEventContext(object instanceObj) => ((DropTargetHandler)instanceObj).EventContext;

        private static object Construct() => new DropTargetHandler();

        private static object CallGetValue(object instanceObj, object[] parameters) => ((DropTargetHandler)instanceObj).GetValue();

        public static void Pass1Initialize() => Type = new UIXTypeSchema(65, "DropTargetHandler", null, 110, typeof(DropTargetHandler), UIXTypeFlags.Disposable);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(65, "AllowedDropActions", 64, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetAllowedDropActions), new SetValueHandler(SetAllowedDropActions), false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(65, "Dragging", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetDragging), null, false);
            UIXPropertySchema uixPropertySchema3 = new UIXPropertySchema(65, "HandlerStage", 112, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetHandlerStage), new SetValueHandler(SetHandlerStage), false);
            UIXPropertySchema uixPropertySchema4 = new UIXPropertySchema(65, "EventContext", 153, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetEventContext), null, false);
            UIXEventSchema uixEventSchema1 = new UIXEventSchema(65, "DragEnter");
            UIXEventSchema uixEventSchema2 = new UIXEventSchema(65, "DragOver");
            UIXEventSchema uixEventSchema3 = new UIXEventSchema(65, "DragLeave");
            UIXEventSchema uixEventSchema4 = new UIXEventSchema(65, "Dropped");
            UIXMethodSchema uixMethodSchema = new UIXMethodSchema(65, "GetValue", null, 153, new InvokeHandler(CallGetValue), false);
            Type.Initialize(new DefaultConstructHandler(Construct), null, new PropertySchema[4]
            {
         uixPropertySchema1,
         uixPropertySchema2,
         uixPropertySchema4,
         uixPropertySchema3
            }, new MethodSchema[1]
            {
         uixMethodSchema
            }, new EventSchema[4]
            {
         uixEventSchema1,
         uixEventSchema2,
         uixEventSchema3,
         uixEventSchema4
            }, null, null, null, null, null, null, null);
        }
    }
}
