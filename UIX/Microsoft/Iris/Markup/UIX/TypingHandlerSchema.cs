// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.TypingHandlerSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.InputHandlers;
using Microsoft.Iris.ModelItems;
using Microsoft.Iris.UI;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class TypingHandlerSchema
    {
        public static UIXTypeSchema Type;

        private static object GetEditableTextData(object instanceObj) => ((TypingHandler)instanceObj).EditableTextData;

        private static void SetEditableTextData(ref object instanceObj, object valueObj) => ((TypingHandler)instanceObj).EditableTextData = (EditableTextData)valueObj;

        private static object GetHandlerStage(object instanceObj) => ((InputHandler)instanceObj).HandlerStage;

        private static void SetHandlerStage(ref object instanceObj, object valueObj) => ((InputHandler)instanceObj).HandlerStage = (InputHandlerStage)valueObj;

        private static object GetSubmitOnEnter(object instanceObj) => BooleanBoxes.Box(((TypingHandler)instanceObj).SubmitOnEnter);

        private static void SetSubmitOnEnter(ref object instanceObj, object valueObj) => ((TypingHandler)instanceObj).SubmitOnEnter = (bool)valueObj;

        private static object GetTreatEscapeAsBackspace(object instanceObj) => BooleanBoxes.Box(((TypingHandler)instanceObj).TreatEscapeAsBackspace);

        private static void SetTreatEscapeAsBackspace(ref object instanceObj, object valueObj) => ((TypingHandler)instanceObj).TreatEscapeAsBackspace = (bool)valueObj;

        private static object Construct() => new TypingHandler();

        public static void Pass1Initialize() => Type = new UIXTypeSchema(228, "TypingHandler", null, 110, typeof(TypingHandler), UIXTypeFlags.Disposable);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(228, "EditableTextData", 68, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetEditableTextData), new SetValueHandler(SetEditableTextData), false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(228, "HandlerStage", 112, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetHandlerStage), new SetValueHandler(SetHandlerStage), false);
            UIXPropertySchema uixPropertySchema3 = new UIXPropertySchema(228, "SubmitOnEnter", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetSubmitOnEnter), new SetValueHandler(SetSubmitOnEnter), false);
            UIXPropertySchema uixPropertySchema4 = new UIXPropertySchema(228, "TreatEscapeAsBackspace", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetTreatEscapeAsBackspace), new SetValueHandler(SetTreatEscapeAsBackspace), false);
            UIXEventSchema uixEventSchema = new UIXEventSchema(228, "TypingInputRejected");
            Type.Initialize(new DefaultConstructHandler(Construct), null, new PropertySchema[4]
            {
         uixPropertySchema1,
         uixPropertySchema2,
         uixPropertySchema3,
         uixPropertySchema4
            }, null, new EventSchema[1]
            {
         uixEventSchema
            }, null, null, null, null, null, null, null);
        }
    }
}
