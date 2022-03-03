// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.DictionarySchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Session;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class DictionarySchema
    {
        public static UIXTypeSchema Type;

        private static object GetSource(object instanceObj) => (IDictionary)instanceObj;

        private static object Construct() => new Dictionary<object, object>();

        private static object Callget_ItemObject(object instanceObj, object[] parameters)
        {
            IDictionary dictionary = (IDictionary)instanceObj;
            object parameter = parameters[0];
            if (parameter != null)
                return dictionary[parameter];
            ErrorManager.ReportError("Script runtime failure: Invalid 'null' value for '{0}'", "key");
            return null;
        }

        private static object CallContainsObject(object instanceObj, object[] parameters)
        {
            IDictionary dictionary = (IDictionary)instanceObj;
            object parameter = parameters[0];
            if (parameter != null)
                return BooleanBoxes.Box(dictionary.Contains(parameter));
            ErrorManager.ReportError("Script runtime failure: Invalid 'null' value for '{0}'", "key");
            return false;
        }

        private static object Callset_ItemObjectObject(object instanceObj, object[] parameters)
        {
            IDictionary dictionary = (IDictionary)instanceObj;
            object parameter1 = parameters[0];
            object parameter2 = parameters[1];
            if (parameter1 == null)
            {
                ErrorManager.ReportError("Script runtime failure: Invalid 'null' value for '{0}'", "key");
                return null;
            }
            dictionary[parameter1] = parameter2;
            return null;
        }

        public static void Pass1Initialize() => Type = new UIXTypeSchema(58, "Dictionary", null, 153, typeof(IDictionary), UIXTypeFlags.None);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema = new UIXPropertySchema(58, "Source", 58, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetSource), null, false);
            UIXMethodSchema uixMethodSchema1 = new UIXMethodSchema(58, "get_Item", new short[1]
            {
         153
            }, 153, new InvokeHandler(Callget_ItemObject), false);
            UIXMethodSchema uixMethodSchema2 = new UIXMethodSchema(58, "Contains", new short[1]
            {
         153
            }, 15, new InvokeHandler(CallContainsObject), false);
            UIXMethodSchema uixMethodSchema3 = new UIXMethodSchema(58, "set_Item", new short[2]
            {
         153,
         153
            }, 240, new InvokeHandler(Callset_ItemObjectObject), false);
            Type.Initialize(new DefaultConstructHandler(Construct), null, new PropertySchema[1]
            {
         uixPropertySchema
            }, new MethodSchema[3]
            {
         uixMethodSchema1,
         uixMethodSchema2,
         uixMethodSchema3
            }, null, null, null, null, null, null, null, null);
        }
    }
}
