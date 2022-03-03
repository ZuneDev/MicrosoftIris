// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.UIStateSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Input;
using Microsoft.Iris.Library;
using Microsoft.Iris.Render;
using Microsoft.Iris.Session;
using Microsoft.Iris.UI;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class UIStateSchema
    {
        public static UIXTypeSchema Type;

        private static object GetCreateInterestOnFocus(object instanceObj) => BooleanBoxes.Box(((UIClass)instanceObj).CreateInterestOnFocus);

        private static void SetCreateInterestOnFocus(ref object instanceObj, object valueObj) => ((UIClass)instanceObj).CreateInterestOnFocus = (bool)valueObj;

        private static object GetCursor(object instanceObj) => ((UIClass)instanceObj).Cursor;

        private static void SetCursor(ref object instanceObj, object valueObj) => ((UIClass)instanceObj).Cursor = (CursorID)valueObj;

        private static object GetDirectKeyFocus(object instanceObj) => BooleanBoxes.Box(((UIClass)instanceObj).DirectKeyFocus);

        private static object GetDirectMouseFocus(object instanceObj) => BooleanBoxes.Box(((UIClass)instanceObj).DirectMouseFocus);

        private static object GetFocusInterestTarget(object instanceObj) => ((UIClass)instanceObj).FocusInterestTarget;

        private static void SetFocusInterestTarget(ref object instanceObj, object valueObj) => ((UIClass)instanceObj).FocusInterestTarget = (ViewItem)valueObj;

        private static object GetFocusInterestTargetMargins(object instanceObj) => ((UIClass)instanceObj).FocusInterestTargetMargins;

        private static void SetFocusInterestTargetMargins(ref object instanceObj, object valueObj) => ((UIClass)instanceObj).FocusInterestTargetMargins = (Inset)valueObj;

        private static object GetKeyFocus(object instanceObj) => BooleanBoxes.Box(((UIClass)instanceObj).KeyFocus);

        private static object GetKeyFocusOnMouseDown(object instanceObj) => BooleanBoxes.Box(((UIClass)instanceObj).KeyFocusOnMouseDown);

        private static void SetKeyFocusOnMouseDown(ref object instanceObj, object valueObj) => ((UIClass)instanceObj).KeyFocusOnMouseDown = (bool)valueObj;

        private static object GetKeyFocusOnMouseEnter(object instanceObj) => BooleanBoxes.Box(((UIClass)instanceObj).KeyFocusOnMouseEnter);

        private static void SetKeyFocusOnMouseEnter(ref object instanceObj, object valueObj) => ((UIClass)instanceObj).KeyFocusOnMouseEnter = (bool)valueObj;

        private static object GetKeyInteractive(object instanceObj) => BooleanBoxes.Box(((UIClass)instanceObj).KeyInteractive);

        private static void SetKeyInteractive(ref object instanceObj, object valueObj) => ((UIClass)instanceObj).KeyInteractive = (bool)valueObj;

        private static object GetMouseFocus(object instanceObj) => BooleanBoxes.Box(((UIClass)instanceObj).MouseFocus);

        private static object GetMouseInteractive(object instanceObj) => ((UIClass)instanceObj).MouseInteractive;

        private static void SetMouseInteractive(ref object instanceObj, object valueObj) => ((UIClass)instanceObj).SetMouseInteractive((bool)valueObj, true);

        private static object GetEnabled(object instanceObj) => BooleanBoxes.Box(((UIClass)instanceObj).Enabled);

        private static void SetEnabled(ref object instanceObj, object valueObj) => ((UIClass)instanceObj).Enabled = (bool)valueObj;

        private static object GetFullyEnabled(object instanceObj) => BooleanBoxes.Box(((UIClass)instanceObj).FullyEnabled);

        private static object GetAllowDoubleClicks(object instanceObj) => BooleanBoxes.Box(((UIClass)instanceObj).AllowDoubleClicks);

        private static void SetAllowDoubleClicks(ref object instanceObj, object valueObj) => ((UIClass)instanceObj).AllowDoubleClicks = (bool)valueObj;

        private static object GetPaintOrder(object instanceObj) => (int)((UIClass)instanceObj).PaintOrder;

        private static void SetPaintOrder(ref object instanceObj, object valueObj)
        {
            UIClass uiClass = (UIClass)instanceObj;
            int num = (int)valueObj;
            Result result = Int32Schema.ValidateNotNegative(valueObj);
            if (result.Failed)
                ErrorManager.ReportError(result.Error);
            else
                uiClass.PaintOrder = (uint)num;
        }

        private static object CallDisposeOwnedObjectObject(object instanceObj, object[] parameters)
        {
            UIClass uiClass = (UIClass)instanceObj;
            object parameter = parameters[0];
            if (parameter == null)
                return null;
            if (!(parameter is IDisposableObject disposable))
            {
                ErrorManager.ReportError("Attempt to dispose an object '{0}' that isn't disposable", TypeSchema.NameFromInstance(parameter));
                return null;
            }
            if (!uiClass.UnregisterDisposable(ref disposable))
            {
                ErrorManager.ReportError("Attempt to dispose an object '{0}' that '{1}' doesn't own", TypeSchema.NameFromInstance(disposable), uiClass.TypeSchema.Name);
                return null;
            }
            disposable.Dispose(uiClass);
            return null;
        }

        private static object CallNavigateInto(object instanceObj, object[] parameters)
        {
            ((UIClass)instanceObj).NavigateInto();
            return null;
        }

        private static object CallNavigateIntoBoolean(object instanceObj, object[] parameters)
        {
            ((UIClass)instanceObj).NavigateInto((bool)parameters[0]);
            return null;
        }

        public static void Pass1Initialize() => Type = new UIXTypeSchema(230, "UIState", null, -1, typeof(UIClass), UIXTypeFlags.None);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(230, "CreateInterestOnFocus", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetCreateInterestOnFocus), new SetValueHandler(SetCreateInterestOnFocus), false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(230, "Cursor", 44, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetCursor), new SetValueHandler(SetCursor), false);
            UIXPropertySchema uixPropertySchema3 = new UIXPropertySchema(230, "DirectKeyFocus", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetDirectKeyFocus), null, false);
            UIXPropertySchema uixPropertySchema4 = new UIXPropertySchema(230, "DirectMouseFocus", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetDirectMouseFocus), null, false);
            UIXPropertySchema uixPropertySchema5 = new UIXPropertySchema(230, "FocusInterestTarget", 239, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetFocusInterestTarget), new SetValueHandler(SetFocusInterestTarget), false);
            UIXPropertySchema uixPropertySchema6 = new UIXPropertySchema(230, "FocusInterestTargetMargins", 114, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetFocusInterestTargetMargins), new SetValueHandler(SetFocusInterestTargetMargins), false);
            UIXPropertySchema uixPropertySchema7 = new UIXPropertySchema(230, "KeyFocus", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetKeyFocus), null, false);
            UIXPropertySchema uixPropertySchema8 = new UIXPropertySchema(230, "KeyFocusOnMouseDown", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetKeyFocusOnMouseDown), new SetValueHandler(SetKeyFocusOnMouseDown), false);
            UIXPropertySchema uixPropertySchema9 = new UIXPropertySchema(230, "KeyFocusOnMouseEnter", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetKeyFocusOnMouseEnter), new SetValueHandler(SetKeyFocusOnMouseEnter), false);
            UIXPropertySchema uixPropertySchema10 = new UIXPropertySchema(230, "KeyInteractive", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetKeyInteractive), new SetValueHandler(SetKeyInteractive), false);
            UIXPropertySchema uixPropertySchema11 = new UIXPropertySchema(230, "MouseFocus", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetMouseFocus), null, false);
            UIXPropertySchema uixPropertySchema12 = new UIXPropertySchema(230, "MouseInteractive", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetMouseInteractive), new SetValueHandler(SetMouseInteractive), false);
            UIXPropertySchema uixPropertySchema13 = new UIXPropertySchema(230, "Enabled", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetEnabled), new SetValueHandler(SetEnabled), false);
            UIXPropertySchema uixPropertySchema14 = new UIXPropertySchema(230, "FullyEnabled", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetFullyEnabled), null, false);
            UIXPropertySchema uixPropertySchema15 = new UIXPropertySchema(230, "AllowDoubleClicks", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetAllowDoubleClicks), new SetValueHandler(SetAllowDoubleClicks), false);
            UIXPropertySchema uixPropertySchema16 = new UIXPropertySchema(230, "PaintOrder", 115, -1, ExpressionRestriction.None, false, Int32Schema.ValidateNotNegative, false, new GetValueHandler(GetPaintOrder), new SetValueHandler(SetPaintOrder), false);
            UIXMethodSchema uixMethodSchema1 = new UIXMethodSchema(230, "DisposeOwnedObject", new short[1]
            {
         153
            }, 240, new InvokeHandler(CallDisposeOwnedObjectObject), false);
            UIXMethodSchema uixMethodSchema2 = new UIXMethodSchema(230, "NavigateInto", null, 240, new InvokeHandler(CallNavigateInto), false);
            UIXMethodSchema uixMethodSchema3 = new UIXMethodSchema(230, "NavigateInto", new short[1]
            {
         15
            }, 240, new InvokeHandler(CallNavigateIntoBoolean), false);
            Type.Initialize(null, null, new PropertySchema[16]
            {
         uixPropertySchema15,
         uixPropertySchema1,
         uixPropertySchema2,
         uixPropertySchema3,
         uixPropertySchema4,
         uixPropertySchema13,
         uixPropertySchema5,
         uixPropertySchema6,
         uixPropertySchema14,
         uixPropertySchema7,
         uixPropertySchema8,
         uixPropertySchema9,
         uixPropertySchema10,
         uixPropertySchema11,
         uixPropertySchema12,
         uixPropertySchema16
            }, new MethodSchema[3]
            {
         uixMethodSchema1,
         uixMethodSchema2,
         uixMethodSchema3
            }, null, null, null, null, null, null, null, null);
        }
    }
}
