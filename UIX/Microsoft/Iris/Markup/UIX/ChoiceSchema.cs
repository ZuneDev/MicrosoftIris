// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.ChoiceSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.ModelItems;
using Microsoft.Iris.Session;
using System.Collections;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class ChoiceSchema
    {
        public static UIXTypeSchema Type;

        private static object GetChosenValue(object instanceObj) => ((IUIChoice)instanceObj).ChosenValue;

        private static void SetChosenValue(ref object instanceObj, object valueObj)
        {
            IUIChoice uiChoice = (IUIChoice)instanceObj;
            object obj = valueObj;
            int index;
            string error;
            if (uiChoice.ValidateOption(obj, out index, out error))
                uiChoice.ChosenIndex = index;
            else
                ErrorManager.ReportError(error);
        }

        private static object GetChosenIndex(object instanceObj) => ((IUIChoice)instanceObj).ChosenIndex;

        private static void SetChosenIndex(ref object instanceObj, object valueObj)
        {
            IUIChoice uiChoice = (IUIChoice)instanceObj;
            int index = (int)valueObj;
            string error;
            if (uiChoice.ValidateIndex(index, out error))
                uiChoice.ChosenIndex = index;
            else
                ErrorManager.ReportError(error);
        }

        private static object GetDefaultIndex(object instanceObj) => ((IUIChoice)instanceObj).DefaultIndex;

        private static void SetDefaultIndex(ref object instanceObj, object valueObj) => ((IUIChoice)instanceObj).DefaultIndex = (int)valueObj;

        private static object GetOptions(object instanceObj) => ((IUIChoice)instanceObj).Options;

        private static void SetOptions(ref object instanceObj, object valueObj)
        {
            IUIChoice uiChoice = (IUIChoice)instanceObj;
            IList list = (IList)valueObj;
            string error;
            if (uiChoice.ValidateOptionsList(list, out error))
                uiChoice.Options = list;
            else
                ErrorManager.ReportError(error);
        }

        private static object GetHasSelection(object instanceObj) => BooleanBoxes.Box(((IUIChoice)instanceObj).HasSelection);

        private static object GetWrap(object instanceObj) => BooleanBoxes.Box(((IUIChoice)instanceObj).Wrap);

        private static void SetWrap(ref object instanceObj, object valueObj) => ((IUIChoice)instanceObj).Wrap = (bool)valueObj;

        private static object GetHasPreviousValue(object instanceObj) => BooleanBoxes.Box(((IUIValueRange)instanceObj).HasPreviousValue);

        private static object GetHasNextValue(object instanceObj) => BooleanBoxes.Box(((IUIValueRange)instanceObj).HasNextValue);

        private static object Construct() => new Microsoft.Iris.ModelItems.Choice();

        private static object CallPreviousValue(object instanceObj, object[] parameters)
        {
            ((IUIValueRange)instanceObj).PreviousValue();
            return null;
        }

        private static object CallPreviousValueBoolean(object instanceObj, object[] parameters)
        {
            ((IUIChoice)instanceObj).PreviousValue((bool)parameters[0]);
            return null;
        }

        private static object CallNextValue(object instanceObj, object[] parameters)
        {
            ((IUIValueRange)instanceObj).NextValue();
            return null;
        }

        private static object CallNextValueBoolean(object instanceObj, object[] parameters)
        {
            ((IUIChoice)instanceObj).NextValue((bool)parameters[0]);
            return null;
        }

        private static object CallDefaultValue(object instanceObj, object[] parameters)
        {
            ((IUIChoice)instanceObj).DefaultValue();
            return null;
        }

        private static object CallClear(object instanceObj, object[] parameters)
        {
            ((IUIChoice)instanceObj).Clear();
            return null;
        }

        public static void Pass1Initialize() => Type = new UIXTypeSchema(28, "Choice", null, 231, typeof(IUIChoice), UIXTypeFlags.Disposable);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(28, "ChosenValue", 153, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetChosenValue), new SetValueHandler(SetChosenValue), false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(28, "ChosenIndex", 115, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetChosenIndex), new SetValueHandler(SetChosenIndex), false);
            UIXPropertySchema uixPropertySchema3 = new UIXPropertySchema(28, "DefaultIndex", 115, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetDefaultIndex), new SetValueHandler(SetDefaultIndex), false);
            UIXPropertySchema uixPropertySchema4 = new UIXPropertySchema(28, "Options", 138, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetOptions), new SetValueHandler(SetOptions), false);
            UIXPropertySchema uixPropertySchema5 = new UIXPropertySchema(28, "HasSelection", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetHasSelection), null, false);
            UIXPropertySchema uixPropertySchema6 = new UIXPropertySchema(28, "Wrap", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetWrap), new SetValueHandler(SetWrap), false);
            UIXPropertySchema uixPropertySchema7 = new UIXPropertySchema(28, "HasPreviousValue", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetHasPreviousValue), null, false);
            UIXPropertySchema uixPropertySchema8 = new UIXPropertySchema(28, "HasNextValue", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetHasNextValue), null, false);
            UIXMethodSchema uixMethodSchema1 = new UIXMethodSchema(28, "PreviousValue", null, 240, new InvokeHandler(CallPreviousValue), false);
            UIXMethodSchema uixMethodSchema2 = new UIXMethodSchema(28, "PreviousValue", new short[1]
            {
         15
            }, 240, new InvokeHandler(CallPreviousValueBoolean), false);
            UIXMethodSchema uixMethodSchema3 = new UIXMethodSchema(28, "NextValue", null, 240, new InvokeHandler(CallNextValue), false);
            UIXMethodSchema uixMethodSchema4 = new UIXMethodSchema(28, "NextValue", new short[1]
            {
         15
            }, 240, new InvokeHandler(CallNextValueBoolean), false);
            UIXMethodSchema uixMethodSchema5 = new UIXMethodSchema(28, "DefaultValue", null, 240, new InvokeHandler(CallDefaultValue), false);
            UIXMethodSchema uixMethodSchema6 = new UIXMethodSchema(28, "Clear", null, 240, new InvokeHandler(CallClear), false);
            Type.Initialize(new DefaultConstructHandler(Construct), null, new PropertySchema[8]
            {
         uixPropertySchema2,
         uixPropertySchema1,
         uixPropertySchema3,
         uixPropertySchema8,
         uixPropertySchema7,
         uixPropertySchema5,
         uixPropertySchema4,
         uixPropertySchema6
            }, new MethodSchema[6]
            {
         uixMethodSchema1,
         uixMethodSchema2,
         uixMethodSchema3,
         uixMethodSchema4,
         uixMethodSchema5,
         uixMethodSchema6
            }, null, null, null, null, null, null, null, null);
        }
    }
}
