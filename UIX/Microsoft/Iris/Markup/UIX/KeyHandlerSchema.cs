// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.KeyHandlerSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.InputHandlers;
using Microsoft.Iris.ModelItems;
using Microsoft.Iris.Session;
using Microsoft.Iris.UI;
using System.Collections;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class KeyHandlerSchema
    {
        public static UIXTypeSchema Type;

        private static object GetCommand(object instanceObj) => ((KeyHandler)instanceObj).Command;

        private static void SetCommand(ref object instanceObj, object valueObj) => ((KeyHandler)instanceObj).Command = (IUICommand)valueObj;

        private static object GetHandle(object instanceObj) => BooleanBoxes.Box(((KeyHandler)instanceObj).Handle);

        private static void SetHandle(ref object instanceObj, object valueObj) => ((KeyHandler)instanceObj).Handle = (bool)valueObj;

        private static object GetStopRoute(object instanceObj) => BooleanBoxes.Box(((KeyHandler)instanceObj).StopRoute);

        private static void SetStopRoute(ref object instanceObj, object valueObj) => ((KeyHandler)instanceObj).StopRoute = (bool)valueObj;

        private static object GetKey(object instanceObj) => ((KeyHandler)instanceObj).Key;

        private static void SetKey(ref object instanceObj, object valueObj) => ((KeyHandler)instanceObj).Key = (KeyHandlerKey)valueObj;

        private static object GetHandlerTransition(object instanceObj) => ((ModifierInputHandler)instanceObj).HandlerTransition;

        private static void SetHandlerTransition(ref object instanceObj, object valueObj) => ((ModifierInputHandler)instanceObj).HandlerTransition = (InputHandlerTransition)valueObj;

        private static object GetRequiredModifiers(object instanceObj) => ((ModifierInputHandler)instanceObj).RequiredModifiers;

        private static void SetRequiredModifiers(ref object instanceObj, object valueObj) => ((ModifierInputHandler)instanceObj).RequiredModifiers = (InputHandlerModifiers)valueObj;

        private static object GetDisallowedModifiers(object instanceObj) => ((ModifierInputHandler)instanceObj).DisallowedModifiers;

        private static void SetDisallowedModifiers(ref object instanceObj, object valueObj) => ((ModifierInputHandler)instanceObj).DisallowedModifiers = (InputHandlerModifiers)valueObj;

        private static object GetPressing(object instanceObj) => BooleanBoxes.Box(((KeyHandler)instanceObj).Pressing);

        private static object GetRepeat(object instanceObj) => BooleanBoxes.Box(((KeyHandler)instanceObj).Repeat);

        private static void SetRepeat(ref object instanceObj, object valueObj) => ((KeyHandler)instanceObj).Repeat = (bool)valueObj;

        private static object GetTrackInvokedKeys(object instanceObj) => BooleanBoxes.Box(((KeyHandler)instanceObj).TrackInvokedKeys);

        private static void SetTrackInvokedKeys(ref object instanceObj, object valueObj) => ((KeyHandler)instanceObj).TrackInvokedKeys = (bool)valueObj;

        private static object GetHandlerStage(object instanceObj) => ((InputHandler)instanceObj).HandlerStage;

        private static void SetHandlerStage(ref object instanceObj, object valueObj) => ((InputHandler)instanceObj).HandlerStage = (InputHandlerStage)valueObj;

        private static object GetEventContext(object instanceObj) => ((KeyHandler)instanceObj).EventContext;

        private static object Construct() => new KeyHandler();

        private static object CallGetInvokedKeys(object instanceObj, object[] parameters)
        {
            KeyHandler keyHandler = (KeyHandler)instanceObj;
            ArrayList arrayList = new ArrayList();
            keyHandler.GetInvokedKeys(arrayList);
            return arrayList;
        }

        private static object CallGetInvokedKeysList(object instanceObj, object[] parameters)
        {
            KeyHandler keyHandler = (KeyHandler)instanceObj;
            IList parameter = (IList)parameters[0];
            if (parameter != null)
                keyHandler.GetInvokedKeys(parameter);
            else
                ErrorManager.ReportError("Script runtime failure: Invalid 'null' value for '{0}'", "copyTo");
            return null;
        }

        public static void Pass1Initialize() => Type = new UIXTypeSchema(128, "KeyHandler", null, 110, typeof(KeyHandler), UIXTypeFlags.Disposable);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(128, "Command", 40, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetCommand), new SetValueHandler(SetCommand), false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(128, "Handle", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetHandle), new SetValueHandler(SetHandle), false);
            UIXPropertySchema uixPropertySchema3 = new UIXPropertySchema(128, "StopRoute", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetStopRoute), new SetValueHandler(SetStopRoute), false);
            UIXPropertySchema uixPropertySchema4 = new UIXPropertySchema(128, "Key", 129, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetKey), new SetValueHandler(SetKey), false);
            UIXPropertySchema uixPropertySchema5 = new UIXPropertySchema(128, "HandlerTransition", 113, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetHandlerTransition), new SetValueHandler(SetHandlerTransition), false);
            UIXPropertySchema uixPropertySchema6 = new UIXPropertySchema(128, "RequiredModifiers", 111, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetRequiredModifiers), new SetValueHandler(SetRequiredModifiers), false);
            UIXPropertySchema uixPropertySchema7 = new UIXPropertySchema(128, "DisallowedModifiers", 111, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetDisallowedModifiers), new SetValueHandler(SetDisallowedModifiers), false);
            UIXPropertySchema uixPropertySchema8 = new UIXPropertySchema(128, "Pressing", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetPressing), null, false);
            UIXPropertySchema uixPropertySchema9 = new UIXPropertySchema(128, "Repeat", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetRepeat), new SetValueHandler(SetRepeat), false);
            UIXPropertySchema uixPropertySchema10 = new UIXPropertySchema(128, "TrackInvokedKeys", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetTrackInvokedKeys), new SetValueHandler(SetTrackInvokedKeys), false);
            UIXPropertySchema uixPropertySchema11 = new UIXPropertySchema(128, "HandlerStage", 112, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetHandlerStage), new SetValueHandler(SetHandlerStage), false);
            UIXPropertySchema uixPropertySchema12 = new UIXPropertySchema(128, "EventContext", 153, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetEventContext), null, false);
            UIXMethodSchema uixMethodSchema1 = new UIXMethodSchema(128, "GetInvokedKeys", null, 138, new InvokeHandler(CallGetInvokedKeys), false);
            UIXMethodSchema uixMethodSchema2 = new UIXMethodSchema(128, "GetInvokedKeys", new short[1]
            {
         138
            }, 240, new InvokeHandler(CallGetInvokedKeysList), false);
            UIXEventSchema uixEventSchema = new UIXEventSchema(128, "Invoked");
            Type.Initialize(new DefaultConstructHandler(Construct), null, new PropertySchema[12]
            {
         uixPropertySchema1,
         uixPropertySchema7,
         uixPropertySchema12,
         uixPropertySchema2,
         uixPropertySchema11,
         uixPropertySchema5,
         uixPropertySchema4,
         uixPropertySchema8,
         uixPropertySchema9,
         uixPropertySchema6,
         uixPropertySchema3,
         uixPropertySchema10
            }, new MethodSchema[2]
            {
         uixMethodSchema1,
         uixMethodSchema2
            }, new EventSchema[1] { uixEventSchema }, null, null, null, null, null, null, null);
        }
    }
}
