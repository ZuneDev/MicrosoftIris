// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.ScrollingHandlerSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.InputHandlers;
using Microsoft.Iris.ModelItems;
using Microsoft.Iris.UI;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class ScrollingHandlerSchema
    {
        public static UIXTypeSchema Type;

        private static object GetHandleDirectionalKeys(object instanceObj) => BooleanBoxes.Box(((ScrollingHandler)instanceObj).HandleDirectionalKeys);

        private static void SetHandleDirectionalKeys(ref object instanceObj, object valueObj) => ((ScrollingHandler)instanceObj).HandleDirectionalKeys = (bool)valueObj;

        private static object GetHandlePageKeys(object instanceObj) => BooleanBoxes.Box(((ScrollingHandler)instanceObj).HandlePageKeys);

        private static void SetHandlePageKeys(ref object instanceObj, object valueObj) => ((ScrollingHandler)instanceObj).HandlePageKeys = (bool)valueObj;

        private static object GetHandleHomeEndKeys(object instanceObj) => BooleanBoxes.Box(((ScrollingHandler)instanceObj).HandleHomeEndKeys);

        private static void SetHandleHomeEndKeys(ref object instanceObj, object valueObj) => ((ScrollingHandler)instanceObj).HandleHomeEndKeys = (bool)valueObj;

        private static object GetHandlePageCommands(object instanceObj) => BooleanBoxes.Box(((ScrollingHandler)instanceObj).HandlePageCommands);

        private static void SetHandlePageCommands(ref object instanceObj, object valueObj) => ((ScrollingHandler)instanceObj).HandlePageCommands = (bool)valueObj;

        private static object GetHandleMouseWheel(object instanceObj) => BooleanBoxes.Box(((ScrollingHandler)instanceObj).HandleMouseWheel);

        private static void SetHandleMouseWheel(ref object instanceObj, object valueObj) => ((ScrollingHandler)instanceObj).HandleMouseWheel = (bool)valueObj;

        private static object GetScrollModel(object instanceObj) => ((ScrollingHandler)instanceObj).ScrollModel;

        private static void SetScrollModel(ref object instanceObj, object valueObj) => ((ScrollingHandler)instanceObj).ScrollModel = (ScrollModel)valueObj;

        private static object GetUseFocusBehavior(object instanceObj) => BooleanBoxes.Box(((ScrollingHandler)instanceObj).UseFocusBehavior);

        private static void SetUseFocusBehavior(ref object instanceObj, object valueObj) => ((ScrollingHandler)instanceObj).UseFocusBehavior = (bool)valueObj;

        private static object GetHandlerStage(object instanceObj) => ((InputHandler)instanceObj).HandlerStage;

        private static void SetHandlerStage(ref object instanceObj, object valueObj) => ((InputHandler)instanceObj).HandlerStage = (InputHandlerStage)valueObj;

        private static object Construct() => new ScrollingHandler();

        public static void Pass1Initialize() => Type = new UIXTypeSchema(185, "ScrollingHandler", null, 110, typeof(ScrollingHandler), UIXTypeFlags.Disposable);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(185, "HandleDirectionalKeys", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetHandleDirectionalKeys), new SetValueHandler(SetHandleDirectionalKeys), false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(185, "HandlePageKeys", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetHandlePageKeys), new SetValueHandler(SetHandlePageKeys), false);
            UIXPropertySchema uixPropertySchema3 = new UIXPropertySchema(185, "HandleHomeEndKeys", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetHandleHomeEndKeys), new SetValueHandler(SetHandleHomeEndKeys), false);
            UIXPropertySchema uixPropertySchema4 = new UIXPropertySchema(185, "HandlePageCommands", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetHandlePageCommands), new SetValueHandler(SetHandlePageCommands), false);
            UIXPropertySchema uixPropertySchema5 = new UIXPropertySchema(185, "HandleMouseWheel", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetHandleMouseWheel), new SetValueHandler(SetHandleMouseWheel), false);
            UIXPropertySchema uixPropertySchema6 = new UIXPropertySchema(185, "ScrollModel", 182, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetScrollModel), new SetValueHandler(SetScrollModel), false);
            UIXPropertySchema uixPropertySchema7 = new UIXPropertySchema(185, "UseFocusBehavior", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetUseFocusBehavior), new SetValueHandler(SetUseFocusBehavior), false);
            UIXPropertySchema uixPropertySchema8 = new UIXPropertySchema(185, "HandlerStage", 112, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetHandlerStage), new SetValueHandler(SetHandlerStage), false);
            Type.Initialize(new DefaultConstructHandler(Construct), null, new PropertySchema[8]
            {
         uixPropertySchema1,
         uixPropertySchema3,
         uixPropertySchema5,
         uixPropertySchema4,
         uixPropertySchema2,
         uixPropertySchema8,
         uixPropertySchema6,
         uixPropertySchema7
            }, null, null, null, null, null, null, null, null, null);
        }
    }
}
