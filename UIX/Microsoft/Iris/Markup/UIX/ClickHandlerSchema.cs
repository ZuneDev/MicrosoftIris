// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.ClickHandlerSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.InputHandlers;
using Microsoft.Iris.Library;
using Microsoft.Iris.ModelItems;
using Microsoft.Iris.Session;
using Microsoft.Iris.UI;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class ClickHandlerSchema
    {
        public static UIXTypeSchema Type;

        private static object GetClicking(object instanceObj) => BooleanBoxes.Box(((ClickHandler)instanceObj).Clicking);

        private static object GetClickCount(object instanceObj) => ((ClickHandler)instanceObj).ClickCount;

        private static void SetClickCount(ref object instanceObj, object valueObj) => ((ClickHandler)instanceObj).ClickCount = (ClickCount)valueObj;

        private static object GetClickType(object instanceObj) => ((ClickHandler)instanceObj).ClickType;

        private static void SetClickType(ref object instanceObj, object valueObj) => ((ClickHandler)instanceObj).ClickType = (ClickType)valueObj;

        private static object GetCommand(object instanceObj) => ((ClickHandler)instanceObj).Command;

        private static void SetCommand(ref object instanceObj, object valueObj) => ((ClickHandler)instanceObj).Command = (IUICommand)valueObj;

        private static object GetHandle(object instanceObj) => BooleanBoxes.Box(((ClickHandler)instanceObj).Handle);

        private static void SetHandle(ref object instanceObj, object valueObj) => ((ClickHandler)instanceObj).Handle = (bool)valueObj;

        private static object GetHandlerTransition(object instanceObj) => ((ModifierInputHandler)instanceObj).HandlerTransition;

        private static void SetHandlerTransition(ref object instanceObj, object valueObj) => ((ModifierInputHandler)instanceObj).HandlerTransition = (InputHandlerTransition)valueObj;

        private static object GetRequiredModifiers(object instanceObj) => ((ModifierInputHandler)instanceObj).RequiredModifiers;

        private static void SetRequiredModifiers(ref object instanceObj, object valueObj) => ((ModifierInputHandler)instanceObj).RequiredModifiers = (InputHandlerModifiers)valueObj;

        private static object GetDisallowedModifiers(object instanceObj) => ((ModifierInputHandler)instanceObj).DisallowedModifiers;

        private static void SetDisallowedModifiers(ref object instanceObj, object valueObj) => ((ModifierInputHandler)instanceObj).DisallowedModifiers = (InputHandlerModifiers)valueObj;

        private static object GetRepeat(object instanceObj) => BooleanBoxes.Box(((ClickHandler)instanceObj).Repeat);

        private static void SetRepeat(ref object instanceObj, object valueObj) => ((ClickHandler)instanceObj).Repeat = (bool)valueObj;

        private static object GetRepeatDelay(object instanceObj) => ((ClickHandler)instanceObj).RepeatDelay;

        private static void SetRepeatDelay(ref object instanceObj, object valueObj)
        {
            ClickHandler clickHandler = (ClickHandler)instanceObj;
            int num = (int)valueObj;
            Result result = Int32Schema.ValidateNotNegative(valueObj);
            if (result.Failed)
                ErrorManager.ReportError(result.Error);
            else
                clickHandler.RepeatDelay = num;
        }

        private static object GetRepeatRate(object instanceObj) => ((ClickHandler)instanceObj).RepeatRate;

        private static void SetRepeatRate(ref object instanceObj, object valueObj)
        {
            ClickHandler clickHandler = (ClickHandler)instanceObj;
            int num = (int)valueObj;
            Result result = Int32Schema.ValidateNotNegative(valueObj);
            if (result.Failed)
                ErrorManager.ReportError(result.Error);
            else
                clickHandler.RepeatRate = num;
        }

        private static object GetHandlerStage(object instanceObj) => ((InputHandler)instanceObj).HandlerStage;

        private static void SetHandlerStage(ref object instanceObj, object valueObj) => ((InputHandler)instanceObj).HandlerStage = (InputHandlerStage)valueObj;

        private static object GetEventContext(object instanceObj) => ((ClickHandler)instanceObj).EventContext;

        private static object Construct() => new ClickHandler();

        public static void Pass1Initialize() => Type = new UIXTypeSchema(32, "ClickHandler", null, 110, typeof(ClickHandler), UIXTypeFlags.Disposable);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(32, "Clicking", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetClicking), null, false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(32, "ClickCount", 31, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetClickCount), new SetValueHandler(SetClickCount), false);
            UIXPropertySchema uixPropertySchema3 = new UIXPropertySchema(32, "ClickType", 33, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetClickType), new SetValueHandler(SetClickType), false);
            UIXPropertySchema uixPropertySchema4 = new UIXPropertySchema(32, "Command", 40, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetCommand), new SetValueHandler(SetCommand), false);
            UIXPropertySchema uixPropertySchema5 = new UIXPropertySchema(32, "Handle", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetHandle), new SetValueHandler(SetHandle), false);
            UIXPropertySchema uixPropertySchema6 = new UIXPropertySchema(32, "HandlerTransition", 113, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetHandlerTransition), new SetValueHandler(SetHandlerTransition), false);
            UIXPropertySchema uixPropertySchema7 = new UIXPropertySchema(32, "RequiredModifiers", 111, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetRequiredModifiers), new SetValueHandler(SetRequiredModifiers), false);
            UIXPropertySchema uixPropertySchema8 = new UIXPropertySchema(32, "DisallowedModifiers", 111, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetDisallowedModifiers), new SetValueHandler(SetDisallowedModifiers), false);
            UIXPropertySchema uixPropertySchema9 = new UIXPropertySchema(32, "Repeat", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetRepeat), new SetValueHandler(SetRepeat), false);
            UIXPropertySchema uixPropertySchema10 = new UIXPropertySchema(32, "RepeatDelay", 115, -1, ExpressionRestriction.None, false, Int32Schema.ValidateNotNegative, true, new GetValueHandler(GetRepeatDelay), new SetValueHandler(SetRepeatDelay), false);
            UIXPropertySchema uixPropertySchema11 = new UIXPropertySchema(32, "RepeatRate", 115, -1, ExpressionRestriction.None, false, Int32Schema.ValidateNotNegative, true, new GetValueHandler(GetRepeatRate), new SetValueHandler(SetRepeatRate), false);
            UIXPropertySchema uixPropertySchema12 = new UIXPropertySchema(32, "HandlerStage", 112, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetHandlerStage), new SetValueHandler(SetHandlerStage), false);
            UIXPropertySchema uixPropertySchema13 = new UIXPropertySchema(32, "EventContext", 153, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetEventContext), null, false);
            UIXEventSchema uixEventSchema = new UIXEventSchema(32, "Invoked");
            Type.Initialize(new DefaultConstructHandler(Construct), null, new PropertySchema[13]
            {
         uixPropertySchema2,
         uixPropertySchema3,
         uixPropertySchema1,
         uixPropertySchema4,
         uixPropertySchema8,
         uixPropertySchema13,
         uixPropertySchema5,
         uixPropertySchema12,
         uixPropertySchema6,
         uixPropertySchema9,
         uixPropertySchema10,
         uixPropertySchema11,
         uixPropertySchema7
            }, null, new EventSchema[1]
            {
         uixEventSchema
            }, null, null, null, null, null, null, null);
        }
    }
}
