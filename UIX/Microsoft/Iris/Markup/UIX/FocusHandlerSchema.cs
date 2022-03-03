// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.FocusHandlerSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.InputHandlers;
using Microsoft.Iris.UI;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class FocusHandlerSchema
    {
        public static UIXTypeSchema Type;

        private static object GetReason(object instanceObj) => ((FocusHandler)instanceObj).Reason;

        private static void SetReason(ref object instanceObj, object valueObj) => ((FocusHandler)instanceObj).Reason = (FocusChangeReason)valueObj;

        private static object GetRequiredModifiers(object instanceObj) => ((ModifierInputHandler)instanceObj).RequiredModifiers;

        private static void SetRequiredModifiers(ref object instanceObj, object valueObj) => ((ModifierInputHandler)instanceObj).RequiredModifiers = (InputHandlerModifiers)valueObj;

        private static object GetDisallowedModifiers(object instanceObj) => ((ModifierInputHandler)instanceObj).DisallowedModifiers;

        private static void SetDisallowedModifiers(ref object instanceObj, object valueObj) => ((ModifierInputHandler)instanceObj).DisallowedModifiers = (InputHandlerModifiers)valueObj;

        private static object GetHandlerStage(object instanceObj) => ((InputHandler)instanceObj).HandlerStage;

        private static void SetHandlerStage(ref object instanceObj, object valueObj) => ((InputHandler)instanceObj).HandlerStage = (InputHandlerStage)valueObj;

        private static object GetGainedEventContext(object instanceObj) => ((FocusHandler)instanceObj).GainedEventContext;

        private static object GetLostEventContext(object instanceObj) => ((FocusHandler)instanceObj).LostEventContext;

        private static object Construct() => new FocusHandler();

        public static void Pass1Initialize() => Type = new UIXTypeSchema(92, "FocusHandler", null, 110, typeof(FocusHandler), UIXTypeFlags.Disposable);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(92, "Reason", 91, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetReason), new SetValueHandler(SetReason), false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(92, "RequiredModifiers", 111, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetRequiredModifiers), new SetValueHandler(SetRequiredModifiers), false);
            UIXPropertySchema uixPropertySchema3 = new UIXPropertySchema(92, "DisallowedModifiers", 111, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetDisallowedModifiers), new SetValueHandler(SetDisallowedModifiers), false);
            UIXPropertySchema uixPropertySchema4 = new UIXPropertySchema(92, "HandlerStage", 112, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetHandlerStage), new SetValueHandler(SetHandlerStage), false);
            UIXPropertySchema uixPropertySchema5 = new UIXPropertySchema(92, "GainedEventContext", 153, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetGainedEventContext), null, false);
            UIXPropertySchema uixPropertySchema6 = new UIXPropertySchema(92, "LostEventContext", 153, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetLostEventContext), null, false);
            UIXEventSchema uixEventSchema1 = new UIXEventSchema(92, "GainedFocus");
            UIXEventSchema uixEventSchema2 = new UIXEventSchema(92, "LostFocus");
            Type.Initialize(new DefaultConstructHandler(Construct), null, new PropertySchema[6]
            {
         uixPropertySchema3,
         uixPropertySchema5,
         uixPropertySchema4,
         uixPropertySchema6,
         uixPropertySchema1,
         uixPropertySchema2
            }, null, new EventSchema[2]
            {
         uixEventSchema1,
         uixEventSchema2
            }, null, null, null, null, null, null, null);
        }
    }
}
