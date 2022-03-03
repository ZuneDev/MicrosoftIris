// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.WindowSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;
using Microsoft.Iris.Render;
using Microsoft.Iris.Session;
using Microsoft.Iris.UI;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class WindowSchema
    {
        public static UIXTypeSchema Type;

        private static object GetMainWindow(object instanceObj) => UISession.Default.Form;

        private static object GetCaption(object instanceObj) => ((UIForm)instanceObj).Caption;

        private static void SetCaption(ref object instanceObj, object valueObj) => ((UIForm)instanceObj).Caption = (string)valueObj;

        private static object GetWindowState(object instanceObj) => ((Form)instanceObj).WindowState;

        private static void SetWindowState(ref object instanceObj, object valueObj) => ((Form)instanceObj).WindowState = (Microsoft.Iris.WindowState)valueObj;

        private static object GetActive(object instanceObj) => BooleanBoxes.Box(((Form)instanceObj).ActivationState);

        private static object GetMouseActive(object instanceObj) => BooleanBoxes.Box(!((UIForm)instanceObj).MouseIsIdle);

        private static object GetShowWindowFrame(object instanceObj) => BooleanBoxes.Box(((UIForm)instanceObj).ShowWindowFrame);

        private static void SetShowWindowFrame(ref object instanceObj, object valueObj) => ((UIForm)instanceObj).ShowWindowFrame = (bool)valueObj;

        private static object GetHideMouseOnIdle(object instanceObj) => BooleanBoxes.Box(((UIForm)instanceObj).HideMouseOnIdle);

        private static void SetHideMouseOnIdle(ref object instanceObj, object valueObj) => ((UIForm)instanceObj).HideMouseOnIdle = (bool)valueObj;

        private static object GetMouseIdleTimeout(object instanceObj) => ((UIForm)instanceObj).MouseIdleTimeout;

        private static void SetMouseIdleTimeout(ref object instanceObj, object valueObj)
        {
            UIForm uiForm = (UIForm)instanceObj;
            int num = (int)valueObj;
            Result result = Int32Schema.ValidateNotNegative(valueObj);
            if (result.Failed)
                ErrorManager.ReportError(result.Error);
            else
                uiForm.MouseIdleTimeout = num;
        }

        private static object GetAlwaysOnTop(object instanceObj) => BooleanBoxes.Box(((UIForm)instanceObj).AlwaysOnTop);

        private static void SetAlwaysOnTop(ref object instanceObj, object valueObj) => ((UIForm)instanceObj).AlwaysOnTop = (bool)valueObj;

        private static object GetShowInTaskbar(object instanceObj) => BooleanBoxes.Box(((UIForm)instanceObj).ShowInTaskbar);

        private static void SetShowInTaskbar(ref object instanceObj, object valueObj) => ((UIForm)instanceObj).ShowInTaskbar = (bool)valueObj;

        private static object GetPreventInterruption(object instanceObj) => BooleanBoxes.Box(((UIForm)instanceObj).PreventInterruption);

        private static void SetPreventInterruption(ref object instanceObj, object valueObj) => ((UIForm)instanceObj).PreventInterruption = (bool)valueObj;

        private static object GetMaximizeMode(object instanceObj) => ((UIForm)instanceObj).MaximizeMode;

        private static void SetMaximizeMode(ref object instanceObj, object valueObj) => ((UIForm)instanceObj).MaximizeMode = (MaximizeMode)valueObj;

        private static object GetClientSize(object instanceObj) => ((Form)instanceObj).ClientSize;

        private static void SetClientSize(ref object instanceObj, object valueObj) => ((Form)instanceObj).ClientSize = (Size)valueObj;

        private static object GetPosition(object instanceObj) => ((Form)instanceObj).Position;

        private static void SetPosition(ref object instanceObj, object valueObj) => ((Form)instanceObj).Position = (Point)valueObj;

        private static object GetVisible(object instanceObj) => BooleanBoxes.Box(((Form)instanceObj).Visible);

        private static void SetVisible(ref object instanceObj, object valueObj) => ((Form)instanceObj).Visible = (bool)valueObj;

        private static object Construct() => UISession.Default.Form;

        private static object CallClose(object instanceObj, object[] parameters)
        {
            ((UIForm)instanceObj).Close();
            return null;
        }

        private static object CallForceClose(object instanceObj, object[] parameters)
        {
            ((Form)instanceObj).ForceClose();
            return null;
        }

        private static object CallSaveKeyFocus(object instanceObj, object[] parameters) => ((UIForm)instanceObj).SaveKeyFocus();

        private static object CallRestoreKeyFocusSavedKeyFocus(object instanceObj, object[] parameters)
        {
            ((UIForm)instanceObj).RestoreKeyFocus((SavedKeyFocus)parameters[0]);
            return null;
        }

        public static void Pass1Initialize() => Type = new UIXTypeSchema(241, "Window", null, 153, typeof(UIForm), UIXTypeFlags.None);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(241, "MainWindow", 241, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetMainWindow), null, true);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(241, "Caption", 208, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetCaption), new SetValueHandler(SetCaption), false);
            UIXPropertySchema uixPropertySchema3 = new UIXPropertySchema(241, "WindowState", 242, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetWindowState), new SetValueHandler(SetWindowState), false);
            UIXPropertySchema uixPropertySchema4 = new UIXPropertySchema(241, "Active", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetActive), null, false);
            UIXPropertySchema uixPropertySchema5 = new UIXPropertySchema(241, "MouseActive", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetMouseActive), null, false);
            UIXPropertySchema uixPropertySchema6 = new UIXPropertySchema(241, "ShowWindowFrame", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetShowWindowFrame), new SetValueHandler(SetShowWindowFrame), false);
            UIXPropertySchema uixPropertySchema7 = new UIXPropertySchema(241, "HideMouseOnIdle", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetHideMouseOnIdle), new SetValueHandler(SetHideMouseOnIdle), false);
            UIXPropertySchema uixPropertySchema8 = new UIXPropertySchema(241, "MouseIdleTimeout", 115, -1, ExpressionRestriction.None, false, Int32Schema.ValidateNotNegative, true, new GetValueHandler(GetMouseIdleTimeout), new SetValueHandler(SetMouseIdleTimeout), false);
            UIXPropertySchema uixPropertySchema9 = new UIXPropertySchema(241, "AlwaysOnTop", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetAlwaysOnTop), new SetValueHandler(SetAlwaysOnTop), false);
            UIXPropertySchema uixPropertySchema10 = new UIXPropertySchema(241, "ShowInTaskbar", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetShowInTaskbar), new SetValueHandler(SetShowInTaskbar), false);
            UIXPropertySchema uixPropertySchema11 = new UIXPropertySchema(241, "PreventInterruption", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetPreventInterruption), new SetValueHandler(SetPreventInterruption), false);
            UIXPropertySchema uixPropertySchema12 = new UIXPropertySchema(241, "MaximizeMode", 146, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetMaximizeMode), new SetValueHandler(SetMaximizeMode), false);
            UIXPropertySchema uixPropertySchema13 = new UIXPropertySchema(241, "ClientSize", 195, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetClientSize), new SetValueHandler(SetClientSize), false);
            UIXPropertySchema uixPropertySchema14 = new UIXPropertySchema(241, "Position", 158, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetPosition), new SetValueHandler(SetPosition), false);
            UIXPropertySchema uixPropertySchema15 = new UIXPropertySchema(241, "Visible", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetVisible), new SetValueHandler(SetVisible), false);
            UIXMethodSchema uixMethodSchema1 = new UIXMethodSchema(241, "Close", null, 240, new InvokeHandler(CallClose), false);
            UIXMethodSchema uixMethodSchema2 = new UIXMethodSchema(241, "ForceClose", null, 240, new InvokeHandler(CallForceClose), false);
            UIXMethodSchema uixMethodSchema3 = new UIXMethodSchema(241, "SaveKeyFocus", null, 177, new InvokeHandler(CallSaveKeyFocus), false);
            UIXMethodSchema uixMethodSchema4 = new UIXMethodSchema(241, "RestoreKeyFocus", new short[1]
            {
         177
            }, 240, new InvokeHandler(CallRestoreKeyFocusSavedKeyFocus), false);
            Type.Initialize(new DefaultConstructHandler(Construct), null, new PropertySchema[15]
            {
         uixPropertySchema4,
         uixPropertySchema9,
         uixPropertySchema2,
         uixPropertySchema13,
         uixPropertySchema7,
         uixPropertySchema1,
         uixPropertySchema12,
         uixPropertySchema5,
         uixPropertySchema8,
         uixPropertySchema14,
         uixPropertySchema11,
         uixPropertySchema10,
         uixPropertySchema6,
         uixPropertySchema15,
         uixPropertySchema3
            }, new MethodSchema[4]
            {
         uixMethodSchema1,
         uixMethodSchema2,
         uixMethodSchema3,
         uixMethodSchema4
            }, null, null, null, null, null, null, null, null);
        }
    }
}
