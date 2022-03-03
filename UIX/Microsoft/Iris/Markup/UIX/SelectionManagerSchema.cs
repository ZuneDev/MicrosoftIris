// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.SelectionManagerSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.ModelItems;
using Microsoft.Iris.Session;
using System.Collections;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class SelectionManagerSchema
    {
        public static UIXTypeSchema Type;

        private static object GetCount(object instanceObj) => ((SelectionManager)instanceObj).Count;

        private static object GetSourceList(object instanceObj) => ((SelectionManager)instanceObj).SourceList;

        private static void SetSourceList(ref object instanceObj, object valueObj) => ((SelectionManager)instanceObj).SourceList = (IList)valueObj;

        private static object GetAnchor(object instanceObj) => ((SelectionManager)instanceObj).Anchor;

        private static void SetAnchor(ref object instanceObj, object valueObj) => ((SelectionManager)instanceObj).Anchor = (int)valueObj;

        private static object GetSelectedIndices(object instanceObj) => ((SelectionManager)instanceObj).SelectedIndices;

        private static object GetSelectedItems(object instanceObj) => ((SelectionManager)instanceObj).SelectedItems;

        private static object GetSingleSelect(object instanceObj) => BooleanBoxes.Box(((SelectionManager)instanceObj).SingleSelect);

        private static void SetSingleSelect(ref object instanceObj, object valueObj) => ((SelectionManager)instanceObj).SingleSelect = (bool)valueObj;

        private static object GetSelectedIndex(object instanceObj) => ((SelectionManager)instanceObj).SelectedIndex;

        private static void SetSelectedIndex(ref object instanceObj, object valueObj) => ((SelectionManager)instanceObj).SelectedIndex = (int)valueObj;

        private static object GetSelectedItem(object instanceObj) => ((SelectionManager)instanceObj).SelectedItem;

        private static object Construct() => new SelectionManager();

        private static object CallIsSelectedInt32(object instanceObj, object[] parameters) => ((SelectionManager)instanceObj).IsSelected((int)parameters[0]);

        private static object CallIsRangeSelectedInt32Int32(object instanceObj, object[] parameters) => ((SelectionManager)instanceObj).IsRangeSelected((int)parameters[0], (int)parameters[1]);

        private static object CallClear(object instanceObj, object[] parameters)
        {
            ((SelectionManager)instanceObj).Clear();
            return null;
        }

        private static object CallSelectInt32Boolean(object instanceObj, object[] parameters) => ((SelectionManager)instanceObj).Select((int)parameters[0], (bool)parameters[1]);

        private static object CallSelectListBoolean(object instanceObj, object[] parameters)
        {
            SelectionManager selectionManager = (SelectionManager)instanceObj;
            IList parameter1 = (IList)parameters[0];
            bool parameter2 = (bool)parameters[1];
            if (parameter1 == null)
            {
                ErrorManager.ReportError("Script runtime failure: Invalid 'null' value for '{0}'", "indices");
                return null;
            }
            foreach (object obj in parameter1)
            {
                if (!(obj is int))
                {
                    ErrorManager.ReportError("Script runtime failure: Invalid value '{0}' within list '{1}'", obj, "indices");
                    return null;
                }
            }
            return selectionManager.Select(parameter1, parameter2);
        }

        private static object CallToggleSelectInt32(object instanceObj, object[] parameters) => ((SelectionManager)instanceObj).ToggleSelect((int)parameters[0]);

        private static object CallToggleSelectList(object instanceObj, object[] parameters)
        {
            SelectionManager selectionManager = (SelectionManager)instanceObj;
            IList parameter = (IList)parameters[0];
            if (parameter == null)
            {
                ErrorManager.ReportError("Script runtime failure: Invalid 'null' value for '{0}'", "items");
                return null;
            }
            foreach (object obj in parameter)
            {
                if (!(obj is int))
                {
                    ErrorManager.ReportError("Script runtime failure: Invalid value '{0}' within list '{1}'", obj, "items");
                    return null;
                }
            }
            return selectionManager.ToggleSelect(parameter);
        }

        private static object CallSelectRangeInt32Int32(object instanceObj, object[] parameters) => ((SelectionManager)instanceObj).SelectRange((int)parameters[0], (int)parameters[1]);

        private static object CallSelectRangeFromAnchorInt32(object instanceObj, object[] parameters) => ((SelectionManager)instanceObj).SelectRangeFromAnchor((int)parameters[0]);

        private static object CallSelectRangeFromAnchorInt32Int32(
          object instanceObj,
          object[] parameters)
        {
            return ((SelectionManager)instanceObj).SelectRangeFromAnchor((int)parameters[0], (int)parameters[1]);
        }

        private static object CallToggleSelectRangeInt32Int32(object instanceObj, object[] parameters) => ((SelectionManager)instanceObj).ToggleSelectRange((int)parameters[0], (int)parameters[1]);

        public static void Pass1Initialize() => Type = new UIXTypeSchema(186, "SelectionManager", null, 153, typeof(SelectionManager), UIXTypeFlags.Disposable);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(186, "Count", 115, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetCount), null, false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(186, "SourceList", 138, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetSourceList), new SetValueHandler(SetSourceList), false);
            UIXPropertySchema uixPropertySchema3 = new UIXPropertySchema(186, "Anchor", 115, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetAnchor), new SetValueHandler(SetAnchor), false);
            UIXPropertySchema uixPropertySchema4 = new UIXPropertySchema(186, "SelectedIndices", 138, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetSelectedIndices), null, false);
            UIXPropertySchema uixPropertySchema5 = new UIXPropertySchema(186, "SelectedItems", 138, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetSelectedItems), null, false);
            UIXPropertySchema uixPropertySchema6 = new UIXPropertySchema(186, "SingleSelect", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetSingleSelect), new SetValueHandler(SetSingleSelect), false);
            UIXPropertySchema uixPropertySchema7 = new UIXPropertySchema(186, "SelectedIndex", 115, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetSelectedIndex), new SetValueHandler(SetSelectedIndex), false);
            UIXPropertySchema uixPropertySchema8 = new UIXPropertySchema(186, "SelectedItem", 153, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetSelectedItem), null, false);
            UIXMethodSchema uixMethodSchema1 = new UIXMethodSchema(186, "IsSelected", new short[1]
            {
         115
            }, 15, new InvokeHandler(CallIsSelectedInt32), false);
            UIXMethodSchema uixMethodSchema2 = new UIXMethodSchema(186, "IsRangeSelected", new short[2]
            {
         115,
         115
            }, 15, new InvokeHandler(CallIsRangeSelectedInt32Int32), false);
            UIXMethodSchema uixMethodSchema3 = new UIXMethodSchema(186, "Clear", null, 240, new InvokeHandler(CallClear), false);
            UIXMethodSchema uixMethodSchema4 = new UIXMethodSchema(186, "Select", new short[2]
            {
         115,
         15
            }, 15, new InvokeHandler(CallSelectInt32Boolean), false);
            UIXMethodSchema uixMethodSchema5 = new UIXMethodSchema(186, "Select", new short[2]
            {
         138,
         15
            }, 15, new InvokeHandler(CallSelectListBoolean), false);
            UIXMethodSchema uixMethodSchema6 = new UIXMethodSchema(186, "ToggleSelect", new short[1]
            {
         115
            }, 15, new InvokeHandler(CallToggleSelectInt32), false);
            UIXMethodSchema uixMethodSchema7 = new UIXMethodSchema(186, "ToggleSelect", new short[1]
            {
         138
            }, 15, new InvokeHandler(CallToggleSelectList), false);
            UIXMethodSchema uixMethodSchema8 = new UIXMethodSchema(186, "SelectRange", new short[2]
            {
         115,
         115
            }, 15, new InvokeHandler(CallSelectRangeInt32Int32), false);
            UIXMethodSchema uixMethodSchema9 = new UIXMethodSchema(186, "SelectRangeFromAnchor", new short[1]
            {
         115
            }, 15, new InvokeHandler(CallSelectRangeFromAnchorInt32), false);
            UIXMethodSchema uixMethodSchema10 = new UIXMethodSchema(186, "SelectRangeFromAnchor", new short[2]
            {
         115,
         115
            }, 15, new InvokeHandler(CallSelectRangeFromAnchorInt32Int32), false);
            UIXMethodSchema uixMethodSchema11 = new UIXMethodSchema(186, "ToggleSelectRange", new short[2]
            {
         115,
         115
            }, 15, new InvokeHandler(CallToggleSelectRangeInt32Int32), false);
            Type.Initialize(new DefaultConstructHandler(Construct), null, new PropertySchema[8]
            {
         uixPropertySchema3,
         uixPropertySchema1,
         uixPropertySchema7,
         uixPropertySchema4,
         uixPropertySchema8,
         uixPropertySchema5,
         uixPropertySchema6,
         uixPropertySchema2
            }, new MethodSchema[11]
            {
         uixMethodSchema1,
         uixMethodSchema2,
         uixMethodSchema3,
         uixMethodSchema4,
         uixMethodSchema5,
         uixMethodSchema6,
         uixMethodSchema7,
         uixMethodSchema8,
         uixMethodSchema9,
         uixMethodSchema10,
         uixMethodSchema11
            }, null, null, null, null, null, null, null, null);
        }
    }
}
