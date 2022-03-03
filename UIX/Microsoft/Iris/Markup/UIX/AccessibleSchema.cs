// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.AccessibleSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Accessibility;
using Microsoft.Iris.ModelItems;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class AccessibleSchema
    {
        public static UIXTypeSchema Type;

        private static object GetEnabled(object instanceObj) => BooleanBoxes.Box(((Accessible)instanceObj).Enabled);

        private static object GetDefaultAction(object instanceObj) => ((Accessible)instanceObj).DefaultAction;

        private static void SetDefaultAction(ref object instanceObj, object valueObj) => ((Accessible)instanceObj).DefaultAction = (string)valueObj;

        private static object GetDefaultActionCommand(object instanceObj) => ((Accessible)instanceObj).DefaultActionCommand;

        private static void SetDefaultActionCommand(ref object instanceObj, object valueObj) => ((Accessible)instanceObj).DefaultActionCommand = (IUICommand)valueObj;

        private static object GetDescription(object instanceObj) => ((Accessible)instanceObj).Description;

        private static void SetDescription(ref object instanceObj, object valueObj) => ((Accessible)instanceObj).Description = (string)valueObj;

        private static object GetHasPopup(object instanceObj) => BooleanBoxes.Box(((Accessible)instanceObj).HasPopup);

        private static void SetHasPopup(ref object instanceObj, object valueObj) => ((Accessible)instanceObj).HasPopup = (bool)valueObj;

        private static object GetHelp(object instanceObj) => ((Accessible)instanceObj).Help;

        private static void SetHelp(ref object instanceObj, object valueObj) => ((Accessible)instanceObj).Help = (string)valueObj;

        private static object GetHelpTopic(object instanceObj) => ((Accessible)instanceObj).HelpTopic;

        private static void SetHelpTopic(ref object instanceObj, object valueObj) => ((Accessible)instanceObj).HelpTopic = (int)valueObj;

        private static object GetIsAnimated(object instanceObj) => BooleanBoxes.Box(((Accessible)instanceObj).IsAnimated);

        private static void SetIsAnimated(ref object instanceObj, object valueObj) => ((Accessible)instanceObj).IsAnimated = (bool)valueObj;

        private static object GetIsBusy(object instanceObj) => BooleanBoxes.Box(((Accessible)instanceObj).IsBusy);

        private static void SetIsBusy(ref object instanceObj, object valueObj) => ((Accessible)instanceObj).IsBusy = (bool)valueObj;

        private static object GetIsChecked(object instanceObj) => BooleanBoxes.Box(((Accessible)instanceObj).IsChecked);

        private static void SetIsChecked(ref object instanceObj, object valueObj) => ((Accessible)instanceObj).IsChecked = (bool)valueObj;

        private static object GetIsCollapsed(object instanceObj) => BooleanBoxes.Box(((Accessible)instanceObj).IsCollapsed);

        private static void SetIsCollapsed(ref object instanceObj, object valueObj) => ((Accessible)instanceObj).IsCollapsed = (bool)valueObj;

        private static object GetIsDefault(object instanceObj) => BooleanBoxes.Box(((Accessible)instanceObj).IsDefault);

        private static void SetIsDefault(ref object instanceObj, object valueObj) => ((Accessible)instanceObj).IsDefault = (bool)valueObj;

        private static object GetIsExpanded(object instanceObj) => BooleanBoxes.Box(((Accessible)instanceObj).IsExpanded);

        private static void SetIsExpanded(ref object instanceObj, object valueObj) => ((Accessible)instanceObj).IsExpanded = (bool)valueObj;

        private static object GetIsMarquee(object instanceObj) => BooleanBoxes.Box(((Accessible)instanceObj).IsMarquee);

        private static void SetIsMarquee(ref object instanceObj, object valueObj) => ((Accessible)instanceObj).IsMarquee = (bool)valueObj;

        private static object GetIsMixed(object instanceObj) => BooleanBoxes.Box(((Accessible)instanceObj).IsMixed);

        private static void SetIsMixed(ref object instanceObj, object valueObj) => ((Accessible)instanceObj).IsMixed = (bool)valueObj;

        private static object GetIsMultiSelectable(object instanceObj) => BooleanBoxes.Box(((Accessible)instanceObj).IsMultiSelectable);

        private static void SetIsMultiSelectable(ref object instanceObj, object valueObj) => ((Accessible)instanceObj).IsMultiSelectable = (bool)valueObj;

        private static object GetIsPressed(object instanceObj) => BooleanBoxes.Box(((Accessible)instanceObj).IsPressed);

        private static void SetIsPressed(ref object instanceObj, object valueObj) => ((Accessible)instanceObj).IsPressed = (bool)valueObj;

        private static object GetIsProtected(object instanceObj) => BooleanBoxes.Box(((Accessible)instanceObj).IsProtected);

        private static void SetIsProtected(ref object instanceObj, object valueObj) => ((Accessible)instanceObj).IsProtected = (bool)valueObj;

        private static object GetIsSelectable(object instanceObj) => BooleanBoxes.Box(((Accessible)instanceObj).IsSelectable);

        private static void SetIsSelectable(ref object instanceObj, object valueObj) => ((Accessible)instanceObj).IsSelectable = (bool)valueObj;

        private static object GetIsSelected(object instanceObj) => BooleanBoxes.Box(((Accessible)instanceObj).IsSelected);

        private static void SetIsSelected(ref object instanceObj, object valueObj) => ((Accessible)instanceObj).IsSelected = (bool)valueObj;

        private static object GetIsTraversed(object instanceObj) => BooleanBoxes.Box(((Accessible)instanceObj).IsTraversed);

        private static void SetIsTraversed(ref object instanceObj, object valueObj) => ((Accessible)instanceObj).IsTraversed = (bool)valueObj;

        private static object GetIsUnavailable(object instanceObj) => BooleanBoxes.Box(((Accessible)instanceObj).IsUnavailable);

        private static void SetIsUnavailable(ref object instanceObj, object valueObj) => ((Accessible)instanceObj).IsUnavailable = (bool)valueObj;

        private static object GetKeyboardShortcut(object instanceObj) => ((Accessible)instanceObj).KeyboardShortcut;

        private static void SetKeyboardShortcut(ref object instanceObj, object valueObj) => ((Accessible)instanceObj).KeyboardShortcut = (string)valueObj;

        private static object GetName(object instanceObj) => ((Accessible)instanceObj).Name;

        private static void SetName(ref object instanceObj, object valueObj) => ((Accessible)instanceObj).Name = (string)valueObj;

        private static object GetRole(object instanceObj) => ((Accessible)instanceObj).Role;

        private static void SetRole(ref object instanceObj, object valueObj) => ((Accessible)instanceObj).Role = (AccRole)valueObj;

        private static object GetValue(object instanceObj) => ((Accessible)instanceObj).Value;

        private static void SetValue(ref object instanceObj, object valueObj) => ((Accessible)instanceObj).Value = (string)valueObj;

        private static object Construct() => new Accessible();

        public static void Pass1Initialize() => Type = new UIXTypeSchema(0, "Accessible", null, 153, typeof(Accessible), UIXTypeFlags.None);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(0, "Enabled", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetEnabled), null, false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(0, "DefaultAction", 208, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetDefaultAction), new SetValueHandler(SetDefaultAction), false);
            UIXPropertySchema uixPropertySchema3 = new UIXPropertySchema(0, "DefaultActionCommand", 40, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetDefaultActionCommand), new SetValueHandler(SetDefaultActionCommand), false);
            UIXPropertySchema uixPropertySchema4 = new UIXPropertySchema(0, "Description", 208, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetDescription), new SetValueHandler(SetDescription), false);
            UIXPropertySchema uixPropertySchema5 = new UIXPropertySchema(0, "HasPopup", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetHasPopup), new SetValueHandler(SetHasPopup), false);
            UIXPropertySchema uixPropertySchema6 = new UIXPropertySchema(0, "Help", 208, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetHelp), new SetValueHandler(SetHelp), false);
            UIXPropertySchema uixPropertySchema7 = new UIXPropertySchema(0, "HelpTopic", 115, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetHelpTopic), new SetValueHandler(SetHelpTopic), false);
            UIXPropertySchema uixPropertySchema8 = new UIXPropertySchema(0, "IsAnimated", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetIsAnimated), new SetValueHandler(SetIsAnimated), false);
            UIXPropertySchema uixPropertySchema9 = new UIXPropertySchema(0, "IsBusy", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetIsBusy), new SetValueHandler(SetIsBusy), false);
            UIXPropertySchema uixPropertySchema10 = new UIXPropertySchema(0, "IsChecked", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetIsChecked), new SetValueHandler(SetIsChecked), false);
            UIXPropertySchema uixPropertySchema11 = new UIXPropertySchema(0, "IsCollapsed", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetIsCollapsed), new SetValueHandler(SetIsCollapsed), false);
            UIXPropertySchema uixPropertySchema12 = new UIXPropertySchema(0, "IsDefault", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetIsDefault), new SetValueHandler(SetIsDefault), false);
            UIXPropertySchema uixPropertySchema13 = new UIXPropertySchema(0, "IsExpanded", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetIsExpanded), new SetValueHandler(SetIsExpanded), false);
            UIXPropertySchema uixPropertySchema14 = new UIXPropertySchema(0, "IsMarquee", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetIsMarquee), new SetValueHandler(SetIsMarquee), false);
            UIXPropertySchema uixPropertySchema15 = new UIXPropertySchema(0, "IsMixed", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetIsMixed), new SetValueHandler(SetIsMixed), false);
            UIXPropertySchema uixPropertySchema16 = new UIXPropertySchema(0, "IsMultiSelectable", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetIsMultiSelectable), new SetValueHandler(SetIsMultiSelectable), false);
            UIXPropertySchema uixPropertySchema17 = new UIXPropertySchema(0, "IsPressed", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetIsPressed), new SetValueHandler(SetIsPressed), false);
            UIXPropertySchema uixPropertySchema18 = new UIXPropertySchema(0, "IsProtected", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetIsProtected), new SetValueHandler(SetIsProtected), false);
            UIXPropertySchema uixPropertySchema19 = new UIXPropertySchema(0, "IsSelectable", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetIsSelectable), new SetValueHandler(SetIsSelectable), false);
            UIXPropertySchema uixPropertySchema20 = new UIXPropertySchema(0, "IsSelected", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetIsSelected), new SetValueHandler(SetIsSelected), false);
            UIXPropertySchema uixPropertySchema21 = new UIXPropertySchema(0, "IsTraversed", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetIsTraversed), new SetValueHandler(SetIsTraversed), false);
            UIXPropertySchema uixPropertySchema22 = new UIXPropertySchema(0, "IsUnavailable", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetIsUnavailable), new SetValueHandler(SetIsUnavailable), false);
            UIXPropertySchema uixPropertySchema23 = new UIXPropertySchema(0, "KeyboardShortcut", 208, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetKeyboardShortcut), new SetValueHandler(SetKeyboardShortcut), false);
            UIXPropertySchema uixPropertySchema24 = new UIXPropertySchema(0, "Name", 208, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetName), new SetValueHandler(SetName), false);
            UIXPropertySchema uixPropertySchema25 = new UIXPropertySchema(0, "Role", 1, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetRole), new SetValueHandler(SetRole), false);
            UIXPropertySchema uixPropertySchema26 = new UIXPropertySchema(0, "Value", 208, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetValue), new SetValueHandler(SetValue), false);
            Type.Initialize(new DefaultConstructHandler(Construct), null, new PropertySchema[26]
            {
         uixPropertySchema2,
         uixPropertySchema3,
         uixPropertySchema4,
         uixPropertySchema1,
         uixPropertySchema5,
         uixPropertySchema6,
         uixPropertySchema7,
         uixPropertySchema8,
         uixPropertySchema9,
         uixPropertySchema10,
         uixPropertySchema11,
         uixPropertySchema12,
         uixPropertySchema13,
         uixPropertySchema14,
         uixPropertySchema15,
         uixPropertySchema16,
         uixPropertySchema17,
         uixPropertySchema18,
         uixPropertySchema19,
         uixPropertySchema20,
         uixPropertySchema21,
         uixPropertySchema22,
         uixPropertySchema23,
         uixPropertySchema24,
         uixPropertySchema25,
         uixPropertySchema26
            }, null, null, null, null, null, null, null, null, null);
        }
    }
}
