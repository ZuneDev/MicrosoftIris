// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.ListSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Data;
using Microsoft.Iris.ModelItems;
using Microsoft.Iris.Session;
using System.Collections;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class ListSchema
    {
        public static UIXTypeSchema Type;

        private static object GetCount(object instanceObj) => ((ICollection)instanceObj).Count;

        private static object GetSource(object instanceObj) => (IList)instanceObj;

        private static object GetCanSearch(object instanceObj) => (IList)instanceObj is IUIList uiList ? uiList.CanSearch : (object)false;

        private static object Construct() => new NotifyList();

        private static object CallIsNullOrEmptyList(object instanceObj, object[] parameters)
        {
            IList parameter = (IList)parameters[0];
            return BooleanBoxes.Box(parameter == null || parameter.Count == 0);
        }

        private static object Callget_ItemInt32(object instanceObj, object[] parameters)
        {
            IList list = (IList)instanceObj;
            int parameter = (int)parameters[0];
            if (parameter >= 0 && parameter < list.Count)
                return list[parameter];
            ErrorManager.ReportError("Script runtime failure: Invalid '{0}' value is out of range for '{1}'", parameter, "index");
            return null;
        }

        private static object Callset_ItemInt32Object(object instanceObj, object[] parameters)
        {
            IList list = (IList)instanceObj;
            int parameter1 = (int)parameters[0];
            object parameter2 = parameters[1];
            if (parameter1 < 0 || parameter1 >= list.Count)
            {
                ErrorManager.ReportError("Script runtime failure: Invalid '{0}' value is out of range for '{1}'", parameter1, "index");
                return null;
            }
            list[parameter1] = parameter2;
            return null;
        }

        private static object CallClear(object instanceObj, object[] parameters)
        {
            ((IList)instanceObj).Clear();
            return null;
        }

        private static object CallAddObject(object instanceObj, object[] parameters)
        {
            ((IList)instanceObj).Add(parameters[0]);
            return null;
        }

        private static object CallRemoveObject(object instanceObj, object[] parameters)
        {
            ((IList)instanceObj).Remove(parameters[0]);
            return null;
        }

        private static object CallContainsObject(object instanceObj, object[] parameters) => ((IList)instanceObj).Contains(parameters[0]);

        private static object CallIndexOfObject(object instanceObj, object[] parameters) => ((IList)instanceObj).IndexOf(parameters[0]);

        private static object CallInsertInt32Object(object instanceObj, object[] parameters)
        {
            IList list = (IList)instanceObj;
            int parameter1 = (int)parameters[0];
            object parameter2 = parameters[1];
            if (parameter1 < 0 || parameter1 > list.Count)
            {
                ErrorManager.ReportError("Script runtime failure: Invalid '{0}' value is out of range for '{1}'", parameter1, "index");
                return null;
            }
            list.Insert(parameter1, parameter2);
            return null;
        }

        private static object CallRemoveAtInt32(object instanceObj, object[] parameters)
        {
            IList list = (IList)instanceObj;
            int parameter = (int)parameters[0];
            if (parameter < 0 || parameter >= list.Count)
            {
                ErrorManager.ReportError("Script runtime failure: Invalid '{0}' value is out of range for '{1}'", parameter, "index");
                return null;
            }
            list.RemoveAt(parameter);
            return null;
        }

        private static object CallSearchForStringString(object instanceObj, object[] parameters)
        {
            IList list = (IList)instanceObj;
            string parameter = (string)parameters[0];
            return list is IUIList uiList ? uiList.SearchForString(parameter) : (object)-1;
        }

        private static object CallMoveInt32Int32(object instanceObj, object[] parameters)
        {
            IList list = (IList)instanceObj;
            int parameter1 = (int)parameters[0];
            int parameter2 = (int)parameters[1];
            if (parameter1 < 0 || parameter1 >= list.Count)
            {
                ErrorManager.ReportError("Script runtime failure: Invalid '{0}' value is out of range for '{1}'", parameter1, "oldIndex");
                return null;
            }
            if (parameter2 < 0 || parameter2 >= list.Count)
            {
                ErrorManager.ReportError("Script runtime failure: Invalid '{0}' value is out of range for '{1}'", parameter2, "newIndex");
                return null;
            }
            switch (list)
            {
                case IUIList uiList:
                    uiList.Move(parameter1, parameter2);
                    break;
                case INotifyList notifyList:
                    notifyList.Move(parameter1, parameter2);
                    break;
                default:
                    object obj = list[parameter1];
                    list.RemoveAt(parameter1);
                    list.Insert(parameter2, obj);
                    break;
            }
            return null;
        }

        private static object CallGetEnumerator(object instanceObj, object[] parameters) => ((IEnumerable)instanceObj).GetEnumerator();

        public static void Pass1Initialize() => Type = new UIXTypeSchema(138, "List", null, 153, typeof(IList), UIXTypeFlags.None);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(138, "Count", 115, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetCount), null, false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(138, "Source", 138, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetSource), null, false);
            UIXPropertySchema uixPropertySchema3 = new UIXPropertySchema(138, "CanSearch", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetCanSearch), null, false);
            UIXMethodSchema uixMethodSchema1 = new UIXMethodSchema(138, "IsNullOrEmpty", new short[1]
            {
         138
            }, 15, new InvokeHandler(CallIsNullOrEmptyList), true);
            UIXMethodSchema uixMethodSchema2 = new UIXMethodSchema(138, "get_Item", new short[1]
            {
         115
            }, 153, new InvokeHandler(Callget_ItemInt32), false);
            UIXMethodSchema uixMethodSchema3 = new UIXMethodSchema(138, "set_Item", new short[2]
            {
         115,
         153
            }, 240, new InvokeHandler(Callset_ItemInt32Object), false);
            UIXMethodSchema uixMethodSchema4 = new UIXMethodSchema(138, "Clear", null, 240, new InvokeHandler(CallClear), false);
            UIXMethodSchema uixMethodSchema5 = new UIXMethodSchema(138, "Add", new short[1]
            {
         153
            }, 240, new InvokeHandler(CallAddObject), false);
            UIXMethodSchema uixMethodSchema6 = new UIXMethodSchema(138, "Remove", new short[1]
            {
         153
            }, 240, new InvokeHandler(CallRemoveObject), false);
            UIXMethodSchema uixMethodSchema7 = new UIXMethodSchema(138, "Contains", new short[1]
            {
         153
            }, 15, new InvokeHandler(CallContainsObject), false);
            UIXMethodSchema uixMethodSchema8 = new UIXMethodSchema(138, "IndexOf", new short[1]
            {
         153
            }, 115, new InvokeHandler(CallIndexOfObject), false);
            UIXMethodSchema uixMethodSchema9 = new UIXMethodSchema(138, "Insert", new short[2]
            {
         115,
         153
            }, 240, new InvokeHandler(CallInsertInt32Object), false);
            UIXMethodSchema uixMethodSchema10 = new UIXMethodSchema(138, "RemoveAt", new short[1]
            {
         115
            }, 240, new InvokeHandler(CallRemoveAtInt32), false);
            UIXMethodSchema uixMethodSchema11 = new UIXMethodSchema(138, "SearchForString", new short[1]
            {
         208
            }, 115, new InvokeHandler(CallSearchForStringString), false);
            UIXMethodSchema uixMethodSchema12 = new UIXMethodSchema(138, "Move", new short[2]
            {
         115,
         115
            }, 240, new InvokeHandler(CallMoveInt32Int32), false);
            UIXMethodSchema uixMethodSchema13 = new UIXMethodSchema(138, "GetEnumerator", null, 86, new InvokeHandler(CallGetEnumerator), false);
            Type.Initialize(new DefaultConstructHandler(Construct), null, new PropertySchema[3]
            {
         uixPropertySchema3,
         uixPropertySchema1,
         uixPropertySchema2
            }, new MethodSchema[13]
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
         uixMethodSchema11,
         uixMethodSchema12,
         uixMethodSchema13
            }, null, null, null, null, null, null, null, null);
        }
    }
}
