// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.HostSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Session;
using Microsoft.Iris.ViewItems;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class HostSchema
    {
        private static object[] s_paramsList = new object[10];
        public static UIXTypeSchema Type;

        private static object GetNewContentOnTop(object instanceObj) => BooleanBoxes.Box(((Host)instanceObj).NewContentOnTop);

        private static void SetNewContentOnTop(ref object instanceObj, object valueObj) => ((Host)instanceObj).NewContentOnTop = (bool)valueObj;

        private static object GetSource(object instanceObj) => ((Host)instanceObj).Source;

        private static object GetSourceType(object instanceObj) => ((Host)instanceObj).SourceType;

        private static object GetStatus(object instanceObj) => ((Host)instanceObj).Status;

        private static object GetInputEnabled(object instanceObj) => BooleanBoxes.Box(((Host)instanceObj).InputEnabled);

        private static void SetInputEnabled(ref object instanceObj, object valueObj) => ((Host)instanceObj).InputEnabled = (bool)valueObj;

        private static object GetUnloadable(object instanceObj) => BooleanBoxes.Box(((Host)instanceObj).Unloadable);

        private static void SetUnloadable(ref object instanceObj, object valueObj) => ((Host)instanceObj).Unloadable = (bool)valueObj;

        private static object Construct() => new Host();

        private static object CallUnloadAll(object instanceObj, object[] parameters)
        {
            ((Host)instanceObj).UnloadAll();
            return null;
        }

        private static object CallForceRefresh(object instanceObj, object[] parameters)
        {
            ((Host)instanceObj).ForceRefresh();
            return null;
        }

        private static object CallForceRefreshBoolean(object instanceObj, object[] parameters)
        {
            ((Host)instanceObj).ForceRefresh((bool)parameters[0]);
            return null;
        }

        private static object CallRequestSourceString(object instanceObj, object[] parameters)
        {
            ((Host)instanceObj).RequestSource((string)parameters[0], null, null);
            return null;
        }

        private static object CallRequestSourceStringStringObject(
          object instanceObj,
          object[] parameters)
        {
            Host instance = (Host)instanceObj;
            string parameter1 = (string)parameters[0];
            string parameter2 = (string)parameters[1];
            object parameter3 = parameters[2];
            s_paramsList[0] = parameter2;
            s_paramsList[1] = parameter3;
            RequestSourceCallBuilder(instance, parameter1, null, 1);
            return null;
        }

        private static object CallRequestSourceStringStringObjectStringObject(
          object instanceObj,
          object[] parameters)
        {
            Host instance = (Host)instanceObj;
            string parameter1 = (string)parameters[0];
            string parameter2 = (string)parameters[1];
            object parameter3 = parameters[2];
            string parameter4 = (string)parameters[3];
            object parameter5 = parameters[4];
            s_paramsList[0] = parameter2;
            s_paramsList[1] = parameter3;
            s_paramsList[2] = parameter4;
            s_paramsList[3] = parameter5;
            RequestSourceCallBuilder(instance, parameter1, null, 2);
            return null;
        }

        private static object CallRequestSourceStringStringObjectStringObjectStringObject(
          object instanceObj,
          object[] parameters)
        {
            Host instance = (Host)instanceObj;
            string parameter1 = (string)parameters[0];
            string parameter2 = (string)parameters[1];
            object parameter3 = parameters[2];
            string parameter4 = (string)parameters[3];
            object parameter5 = parameters[4];
            string parameter6 = (string)parameters[5];
            object parameter7 = parameters[6];
            s_paramsList[0] = parameter2;
            s_paramsList[1] = parameter3;
            s_paramsList[2] = parameter4;
            s_paramsList[3] = parameter5;
            s_paramsList[4] = parameter6;
            s_paramsList[5] = parameter7;
            RequestSourceCallBuilder(instance, parameter1, null, 3);
            return null;
        }

        private static object CallRequestSourceStringStringObjectStringObjectStringObjectStringObject(
          object instanceObj,
          object[] parameters)
        {
            Host instance = (Host)instanceObj;
            string parameter1 = (string)parameters[0];
            string parameter2 = (string)parameters[1];
            object parameter3 = parameters[2];
            string parameter4 = (string)parameters[3];
            object parameter5 = parameters[4];
            string parameter6 = (string)parameters[5];
            object parameter7 = parameters[6];
            string parameter8 = (string)parameters[7];
            object parameter9 = parameters[8];
            s_paramsList[0] = parameter2;
            s_paramsList[1] = parameter3;
            s_paramsList[2] = parameter4;
            s_paramsList[3] = parameter5;
            s_paramsList[4] = parameter6;
            s_paramsList[5] = parameter7;
            s_paramsList[6] = parameter8;
            s_paramsList[7] = parameter9;
            RequestSourceCallBuilder(instance, parameter1, null, 4);
            return null;
        }

        private static object CallRequestSourceStringStringObjectStringObjectStringObjectStringObjectStringObject(
          object instanceObj,
          object[] parameters)
        {
            Host instance = (Host)instanceObj;
            string parameter1 = (string)parameters[0];
            string parameter2 = (string)parameters[1];
            object parameter3 = parameters[2];
            string parameter4 = (string)parameters[3];
            object parameter5 = parameters[4];
            string parameter6 = (string)parameters[5];
            object parameter7 = parameters[6];
            string parameter8 = (string)parameters[7];
            object parameter9 = parameters[8];
            string parameter10 = (string)parameters[9];
            object parameter11 = parameters[10];
            s_paramsList[0] = parameter2;
            s_paramsList[1] = parameter3;
            s_paramsList[2] = parameter4;
            s_paramsList[3] = parameter5;
            s_paramsList[4] = parameter6;
            s_paramsList[5] = parameter7;
            s_paramsList[6] = parameter8;
            s_paramsList[7] = parameter9;
            s_paramsList[8] = parameter10;
            s_paramsList[9] = parameter11;
            RequestSourceCallBuilder(instance, parameter1, null, 5);
            return null;
        }

        private static object CallRequestSourceType(object instanceObj, object[] parameters)
        {
            ((Host)instanceObj).RequestSource(null, (TypeSchema)parameters[0], null);
            return null;
        }

        private static object CallRequestSourceTypeStringObject(object instanceObj, object[] parameters)
        {
            Host instance = (Host)instanceObj;
            TypeSchema parameter1 = (TypeSchema)parameters[0];
            string parameter2 = (string)parameters[1];
            object parameter3 = parameters[2];
            s_paramsList[0] = parameter2;
            s_paramsList[1] = parameter3;
            RequestSourceCallBuilder(instance, null, parameter1, 1);
            return null;
        }

        private static object CallRequestSourceTypeStringObjectStringObject(
          object instanceObj,
          object[] parameters)
        {
            Host instance = (Host)instanceObj;
            TypeSchema parameter1 = (TypeSchema)parameters[0];
            string parameter2 = (string)parameters[1];
            object parameter3 = parameters[2];
            string parameter4 = (string)parameters[3];
            object parameter5 = parameters[4];
            s_paramsList[0] = parameter2;
            s_paramsList[1] = parameter3;
            s_paramsList[2] = parameter4;
            s_paramsList[3] = parameter5;
            RequestSourceCallBuilder(instance, null, parameter1, 2);
            return null;
        }

        private static object CallRequestSourceTypeStringObjectStringObjectStringObject(
          object instanceObj,
          object[] parameters)
        {
            Host instance = (Host)instanceObj;
            TypeSchema parameter1 = (TypeSchema)parameters[0];
            string parameter2 = (string)parameters[1];
            object parameter3 = parameters[2];
            string parameter4 = (string)parameters[3];
            object parameter5 = parameters[4];
            string parameter6 = (string)parameters[5];
            object parameter7 = parameters[6];
            s_paramsList[0] = parameter2;
            s_paramsList[1] = parameter3;
            s_paramsList[2] = parameter4;
            s_paramsList[3] = parameter5;
            s_paramsList[4] = parameter6;
            s_paramsList[5] = parameter7;
            RequestSourceCallBuilder(instance, null, parameter1, 3);
            return null;
        }

        private static object CallRequestSourceTypeStringObjectStringObjectStringObjectStringObject(
          object instanceObj,
          object[] parameters)
        {
            Host instance = (Host)instanceObj;
            TypeSchema parameter1 = (TypeSchema)parameters[0];
            string parameter2 = (string)parameters[1];
            object parameter3 = parameters[2];
            string parameter4 = (string)parameters[3];
            object parameter5 = parameters[4];
            string parameter6 = (string)parameters[5];
            object parameter7 = parameters[6];
            string parameter8 = (string)parameters[7];
            object parameter9 = parameters[8];
            s_paramsList[0] = parameter2;
            s_paramsList[1] = parameter3;
            s_paramsList[2] = parameter4;
            s_paramsList[3] = parameter5;
            s_paramsList[4] = parameter6;
            s_paramsList[5] = parameter7;
            s_paramsList[6] = parameter8;
            s_paramsList[7] = parameter9;
            RequestSourceCallBuilder(instance, null, parameter1, 4);
            return null;
        }

        private static object CallRequestSourceTypeStringObjectStringObjectStringObjectStringObjectStringObject(
          object instanceObj,
          object[] parameters)
        {
            Host instance = (Host)instanceObj;
            TypeSchema parameter1 = (TypeSchema)parameters[0];
            string parameter2 = (string)parameters[1];
            object parameter3 = parameters[2];
            string parameter4 = (string)parameters[3];
            object parameter5 = parameters[4];
            string parameter6 = (string)parameters[5];
            object parameter7 = parameters[6];
            string parameter8 = (string)parameters[7];
            object parameter9 = parameters[8];
            string parameter10 = (string)parameters[9];
            object parameter11 = parameters[10];
            s_paramsList[0] = parameter2;
            s_paramsList[1] = parameter3;
            s_paramsList[2] = parameter4;
            s_paramsList[3] = parameter5;
            s_paramsList[4] = parameter6;
            s_paramsList[5] = parameter7;
            s_paramsList[6] = parameter8;
            s_paramsList[7] = parameter9;
            s_paramsList[8] = parameter10;
            s_paramsList[9] = parameter11;
            RequestSourceCallBuilder(instance, null, parameter1, 5);
            return null;
        }

        private static void RequestSourceCallBuilder(
          Host instance,
          string source,
          TypeSchema type,
          int numPairs)
        {
            ErrorWatermark watermark = ErrorManager.Watermark;
            Vector<UIPropertyRecord> vector = new Vector<UIPropertyRecord>(numPairs);
            for (int index = 0; index < numPairs; ++index)
            {
                string name = (string)s_paramsList[index * 2];
                object obj = s_paramsList[index * 2 + 1];
                if (name == null)
                    ErrorManager.ReportError("Script runtime failure: Invalid 'null' value for '{0}'", "property" + index.ToString());
                else
                    UIPropertyRecord.AddToList(vector, name, obj);
            }
            for (int index = 0; index < numPairs * 2; ++index)
                s_paramsList[index] = null;
            if (watermark.ErrorsDetected)
                return;
            instance.RequestSource(source, type, vector);
        }

        public static void Pass1Initialize() => Type = new UIXTypeSchema(101, "Host", null, 239, typeof(Host), UIXTypeFlags.Disposable);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(101, "NewContentOnTop", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetNewContentOnTop), new SetValueHandler(SetNewContentOnTop), false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(101, "Source", 208, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetSource), null, false);
            UIXPropertySchema uixPropertySchema3 = new UIXPropertySchema(101, "SourceType", 225, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetSourceType), null, false);
            UIXPropertySchema uixPropertySchema4 = new UIXPropertySchema(101, "Status", 102, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetStatus), null, false);
            UIXPropertySchema uixPropertySchema5 = new UIXPropertySchema(101, "InputEnabled", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetInputEnabled), new SetValueHandler(SetInputEnabled), false);
            UIXPropertySchema uixPropertySchema6 = new UIXPropertySchema(101, "Unloadable", 15, -1, ExpressionRestriction.ReadOnly, false, null, true, new GetValueHandler(GetUnloadable), new SetValueHandler(SetUnloadable), false);
            UIXMethodSchema uixMethodSchema1 = new UIXMethodSchema(101, "UnloadAll", null, 240, new InvokeHandler(CallUnloadAll), false);
            UIXMethodSchema uixMethodSchema2 = new UIXMethodSchema(101, "ForceRefresh", null, 240, new InvokeHandler(CallForceRefresh), false);
            UIXMethodSchema uixMethodSchema3 = new UIXMethodSchema(101, "ForceRefresh", new short[1]
            {
         15
            }, 240, new InvokeHandler(CallForceRefreshBoolean), false);
            UIXMethodSchema uixMethodSchema4 = new UIXMethodSchema(101, "RequestSource", new short[1]
            {
         208
            }, 240, new InvokeHandler(CallRequestSourceString), false);
            UIXMethodSchema uixMethodSchema5 = new UIXMethodSchema(101, "RequestSource", new short[3]
            {
         208,
         208,
         153
            }, 240, new InvokeHandler(CallRequestSourceStringStringObject), false);
            UIXMethodSchema uixMethodSchema6 = new UIXMethodSchema(101, "RequestSource", new short[5]
            {
         208,
         208,
         153,
         208,
         153
            }, 240, new InvokeHandler(CallRequestSourceStringStringObjectStringObject), false);
            UIXMethodSchema uixMethodSchema7 = new UIXMethodSchema(101, "RequestSource", new short[7]
            {
         208,
         208,
         153,
         208,
         153,
         208,
         153
            }, 240, new InvokeHandler(CallRequestSourceStringStringObjectStringObjectStringObject), false);
            UIXMethodSchema uixMethodSchema8 = new UIXMethodSchema(101, "RequestSource", new short[9]
            {
         208,
         208,
         153,
         208,
         153,
         208,
         153,
         208,
         153
            }, 240, new InvokeHandler(CallRequestSourceStringStringObjectStringObjectStringObjectStringObject), false);
            UIXMethodSchema uixMethodSchema9 = new UIXMethodSchema(101, "RequestSource", new short[11]
            {
         208,
         208,
         153,
         208,
         153,
         208,
         153,
         208,
         153,
         208,
         153
            }, 240, new InvokeHandler(CallRequestSourceStringStringObjectStringObjectStringObjectStringObjectStringObject), false);
            UIXMethodSchema uixMethodSchema10 = new UIXMethodSchema(101, "RequestSource", new short[1]
            {
         225
            }, 240, new InvokeHandler(CallRequestSourceType), false);
            UIXMethodSchema uixMethodSchema11 = new UIXMethodSchema(101, "RequestSource", new short[3]
            {
         225,
         208,
         153
            }, 240, new InvokeHandler(CallRequestSourceTypeStringObject), false);
            UIXMethodSchema uixMethodSchema12 = new UIXMethodSchema(101, "RequestSource", new short[5]
            {
         225,
         208,
         153,
         208,
         153
            }, 240, new InvokeHandler(CallRequestSourceTypeStringObjectStringObject), false);
            UIXMethodSchema uixMethodSchema13 = new UIXMethodSchema(101, "RequestSource", new short[7]
            {
         225,
         208,
         153,
         208,
         153,
         208,
         153
            }, 240, new InvokeHandler(CallRequestSourceTypeStringObjectStringObjectStringObject), false);
            UIXMethodSchema uixMethodSchema14 = new UIXMethodSchema(101, "RequestSource", new short[9]
            {
         225,
         208,
         153,
         208,
         153,
         208,
         153,
         208,
         153
            }, 240, new InvokeHandler(CallRequestSourceTypeStringObjectStringObjectStringObjectStringObject), false);
            UIXMethodSchema uixMethodSchema15 = new UIXMethodSchema(101, "RequestSource", new short[11]
            {
         225,
         208,
         153,
         208,
         153,
         208,
         153,
         208,
         153,
         208,
         153
            }, 240, new InvokeHandler(CallRequestSourceTypeStringObjectStringObjectStringObjectStringObjectStringObject), false);
            Type.Initialize(new DefaultConstructHandler(Construct), null, new PropertySchema[6]
            {
         uixPropertySchema5,
         uixPropertySchema1,
         uixPropertySchema2,
         uixPropertySchema3,
         uixPropertySchema4,
         uixPropertySchema6
            }, new MethodSchema[15]
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
         uixMethodSchema13,
         uixMethodSchema14,
         uixMethodSchema15
            }, null, null, null, null, null, null, null, null);
        }
    }
}
