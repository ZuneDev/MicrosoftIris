// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.TextEditingHandlerSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Drawing;
using Microsoft.Iris.InputHandlers;
using Microsoft.Iris.ModelItems;
using Microsoft.Iris.Session;
using Microsoft.Iris.UI;
using Microsoft.Iris.ViewItems;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class TextEditingHandlerSchema
    {
        public static UIXTypeSchema Type;

        private static object GetAcceptsEnter(object instanceObj) => BooleanBoxes.Box(((TextEditingHandler)instanceObj).AcceptsEnter);

        private static void SetAcceptsEnter(ref object instanceObj, object valueObj) => ((TextEditingHandler)instanceObj).AcceptsEnter = (bool)valueObj;

        private static object GetAcceptsTab(object instanceObj) => BooleanBoxes.Box(((TextEditingHandler)instanceObj).AcceptsTab);

        private static void SetAcceptsTab(ref object instanceObj, object valueObj) => ((TextEditingHandler)instanceObj).AcceptsTab = (bool)valueObj;

        private static object GetCaretInfo(object instanceObj) => ((TextEditingHandler)instanceObj).CaretInfo;

        private static object GetEditableTextData(object instanceObj) => ((TextEditingHandler)instanceObj).EditableTextData;

        private static void SetEditableTextData(ref object instanceObj, object valueObj) => ((TextEditingHandler)instanceObj).EditableTextData = (EditableTextData)valueObj;

        private static object GetOvertype(object instanceObj) => BooleanBoxes.Box(((TextEditingHandler)instanceObj).Overtype);

        private static void SetOvertype(ref object instanceObj, object valueObj) => ((TextEditingHandler)instanceObj).Overtype = (bool)valueObj;

        private static object GetTextDisplay(object instanceObj) => ((TextEditingHandler)instanceObj).TextDisplay;

        private static void SetTextDisplay(ref object instanceObj, object valueObj) => ((TextEditingHandler)instanceObj).TextDisplay = (Text)valueObj;

        private static object GetSelectionRange(object instanceObj) => ((TextEditingHandler)instanceObj).SelectionRange;

        private static void SetSelectionRange(ref object instanceObj, object valueObj)
        {
            TextEditingHandler textEditingHandler = (TextEditingHandler)instanceObj;
            Range range = (Range)valueObj;
            int num = 0;
            if (textEditingHandler.EditableTextData != null && textEditingHandler.EditableTextData.Value != null)
                num = textEditingHandler.EditableTextData.Value.Length;
            if (range.Begin < 0 || range.Begin > num)
                ErrorManager.ReportError("Script runtime failure: Invalid '{0}' value is out of range for '{1}'", range.Begin, "SelectionRange.Begin");
            if (range.End < 0 || range.End > num)
                ErrorManager.ReportError("Script runtime failure: Invalid '{0}' value is out of range for '{1}'", range.End, "SelectionRange.End");
            textEditingHandler.SelectionRange = range;
        }

        private static object GetCopyCommand(object instanceObj) => ((TextEditingHandler)instanceObj).CopyCommand;

        private static object GetCutCommand(object instanceObj) => ((TextEditingHandler)instanceObj).CutCommand;

        private static object GetDeleteCommand(object instanceObj) => ((TextEditingHandler)instanceObj).DeleteCommand;

        private static object GetPasteCommand(object instanceObj) => ((TextEditingHandler)instanceObj).PasteCommand;

        private static object GetSelectAllCommand(object instanceObj) => ((TextEditingHandler)instanceObj).SelectAllCommand;

        private static object GetUndoCommand(object instanceObj) => ((TextEditingHandler)instanceObj).UndoCommand;

        private static object GetHorizontalScrollModel(object instanceObj) => ((TextEditingHandler)instanceObj).HorizontalScrollModel;

        private static void SetHorizontalScrollModel(ref object instanceObj, object valueObj) => ((TextEditingHandler)instanceObj).HorizontalScrollModel = (TextScrollModel)valueObj;

        private static object GetVerticalScrollModel(object instanceObj) => ((TextEditingHandler)instanceObj).VerticalScrollModel;

        private static void SetVerticalScrollModel(ref object instanceObj, object valueObj) => ((TextEditingHandler)instanceObj).VerticalScrollModel = (TextScrollModel)valueObj;

        private static object GetDetectUrls(object instanceObj) => BooleanBoxes.Box(((TextEditingHandler)instanceObj).DetectUrls);

        private static void SetDetectUrls(ref object instanceObj, object valueObj) => ((TextEditingHandler)instanceObj).DetectUrls = (bool)valueObj;

        private static object GetLinkColor(object instanceObj) => ((TextEditingHandler)instanceObj).LinkColor;

        private static void SetLinkColor(ref object instanceObj, object valueObj) => ((TextEditingHandler)instanceObj).LinkColor = (Color)valueObj;

        private static object GetLinkClickedParameter(object instanceObj) => ((TextEditingHandler)instanceObj).LinkClickedParameter;

        private static object GetInImeCompositionMode(object instanceObj) => BooleanBoxes.Box(((TextEditingHandler)instanceObj).InImeCompositionMode);

        private static void SetInImeCompositionMode(ref object instanceObj, object valueObj) => ((TextEditingHandler)instanceObj).InImeCompositionMode = (bool)valueObj;

        private static object GetHandlerStage(object instanceObj) => ((InputHandler)instanceObj).HandlerStage;

        private static void SetHandlerStage(ref object instanceObj, object valueObj) => ((InputHandler)instanceObj).HandlerStage = (InputHandlerStage)valueObj;

        private static object Construct() => new TextEditingHandler();

        private static object CallCopy(object instanceObj, object[] parameters)
        {
            ((TextEditingHandler)instanceObj).Copy();
            return null;
        }

        private static object CallCut(object instanceObj, object[] parameters)
        {
            ((TextEditingHandler)instanceObj).Cut();
            return null;
        }

        private static object CallDelete(object instanceObj, object[] parameters)
        {
            ((TextEditingHandler)instanceObj).Delete();
            return null;
        }

        private static object CallPaste(object instanceObj, object[] parameters)
        {
            ((TextEditingHandler)instanceObj).Paste();
            return null;
        }

        private static object CallSelectAll(object instanceObj, object[] parameters)
        {
            ((TextEditingHandler)instanceObj).SelectAll();
            return null;
        }

        private static object CallUndo(object instanceObj, object[] parameters)
        {
            ((TextEditingHandler)instanceObj).Undo();
            return null;
        }

        public static void Pass1Initialize() => Type = new UIXTypeSchema(214, "TextEditingHandler", null, 110, typeof(TextEditingHandler), UIXTypeFlags.Disposable);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(214, "AcceptsEnter", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetAcceptsEnter), new SetValueHandler(SetAcceptsEnter), false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(214, "AcceptsTab", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetAcceptsTab), new SetValueHandler(SetAcceptsTab), false);
            UIXPropertySchema uixPropertySchema3 = new UIXPropertySchema(214, "CaretInfo", 26, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetCaretInfo), null, false);
            UIXPropertySchema uixPropertySchema4 = new UIXPropertySchema(214, "EditableTextData", 68, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetEditableTextData), new SetValueHandler(SetEditableTextData), false);
            UIXPropertySchema uixPropertySchema5 = new UIXPropertySchema(214, "Overtype", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetOvertype), new SetValueHandler(SetOvertype), false);
            UIXPropertySchema uixPropertySchema6 = new UIXPropertySchema(214, "TextDisplay", 212, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetTextDisplay), new SetValueHandler(SetTextDisplay), false);
            UIXPropertySchema uixPropertySchema7 = new UIXPropertySchema(214, "SelectionRange", 187, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetSelectionRange), new SetValueHandler(SetSelectionRange), false);
            UIXPropertySchema uixPropertySchema8 = new UIXPropertySchema(214, "CopyCommand", 40, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetCopyCommand), null, false);
            UIXPropertySchema uixPropertySchema9 = new UIXPropertySchema(214, "CutCommand", 40, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetCutCommand), null, false);
            UIXPropertySchema uixPropertySchema10 = new UIXPropertySchema(214, "DeleteCommand", 40, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetDeleteCommand), null, false);
            UIXPropertySchema uixPropertySchema11 = new UIXPropertySchema(214, "PasteCommand", 40, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetPasteCommand), null, false);
            UIXPropertySchema uixPropertySchema12 = new UIXPropertySchema(214, "SelectAllCommand", 40, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetSelectAllCommand), null, false);
            UIXPropertySchema uixPropertySchema13 = new UIXPropertySchema(214, "UndoCommand", 40, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetUndoCommand), null, false);
            UIXPropertySchema uixPropertySchema14 = new UIXPropertySchema(214, "HorizontalScrollModel", 218, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetHorizontalScrollModel), new SetValueHandler(SetHorizontalScrollModel), false);
            UIXPropertySchema uixPropertySchema15 = new UIXPropertySchema(214, "VerticalScrollModel", 218, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetVerticalScrollModel), new SetValueHandler(SetVerticalScrollModel), false);
            UIXPropertySchema uixPropertySchema16 = new UIXPropertySchema(214, "DetectUrls", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetDetectUrls), new SetValueHandler(SetDetectUrls), false);
            UIXPropertySchema uixPropertySchema17 = new UIXPropertySchema(214, "LinkColor", 35, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetLinkColor), new SetValueHandler(SetLinkColor), false);
            UIXPropertySchema uixPropertySchema18 = new UIXPropertySchema(214, "LinkClickedParameter", 208, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetLinkClickedParameter), null, false);
            UIXPropertySchema uixPropertySchema19 = new UIXPropertySchema(214, "InImeCompositionMode", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetInImeCompositionMode), new SetValueHandler(SetInImeCompositionMode), false);
            UIXPropertySchema uixPropertySchema20 = new UIXPropertySchema(214, "HandlerStage", 112, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetHandlerStage), new SetValueHandler(SetHandlerStage), false);
            UIXEventSchema uixEventSchema1 = new UIXEventSchema(214, "TypingInputRejected");
            UIXMethodSchema uixMethodSchema1 = new UIXMethodSchema(214, "Copy", null, 240, new InvokeHandler(CallCopy), false);
            UIXMethodSchema uixMethodSchema2 = new UIXMethodSchema(214, "Cut", null, 240, new InvokeHandler(CallCut), false);
            UIXMethodSchema uixMethodSchema3 = new UIXMethodSchema(214, "Delete", null, 240, new InvokeHandler(CallDelete), false);
            UIXMethodSchema uixMethodSchema4 = new UIXMethodSchema(214, "Paste", null, 240, new InvokeHandler(CallPaste), false);
            UIXMethodSchema uixMethodSchema5 = new UIXMethodSchema(214, "SelectAll", null, 240, new InvokeHandler(CallSelectAll), false);
            UIXMethodSchema uixMethodSchema6 = new UIXMethodSchema(214, "Undo", null, 240, new InvokeHandler(CallUndo), false);
            UIXEventSchema uixEventSchema2 = new UIXEventSchema(214, "LinkClicked");
            Type.Initialize(new DefaultConstructHandler(Construct), null, new PropertySchema[20]
            {
         uixPropertySchema1,
         uixPropertySchema2,
         uixPropertySchema3,
         uixPropertySchema8,
         uixPropertySchema9,
         uixPropertySchema10,
         uixPropertySchema16,
         uixPropertySchema4,
         uixPropertySchema20,
         uixPropertySchema14,
         uixPropertySchema19,
         uixPropertySchema18,
         uixPropertySchema17,
         uixPropertySchema5,
         uixPropertySchema11,
         uixPropertySchema12,
         uixPropertySchema7,
         uixPropertySchema6,
         uixPropertySchema13,
         uixPropertySchema15
            }, new MethodSchema[6]
            {
         uixMethodSchema1,
         uixMethodSchema2,
         uixMethodSchema3,
         uixMethodSchema4,
         uixMethodSchema5,
         uixMethodSchema6
            }, new EventSchema[2]
            {
         uixEventSchema1,
         uixEventSchema2
            }, null, null, null, null, null, null, null);
        }
    }
}
