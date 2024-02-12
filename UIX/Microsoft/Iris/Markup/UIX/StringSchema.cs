// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.StringSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;
using Microsoft.Iris.Session;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class StringSchema
    {
        public static UIXTypeSchema Type;

        private static object GetLength(object instanceObj) => ((string)instanceObj).Length;

        private static object Construct() => string.Empty;

        private static void EncodeBinary(ByteCodeWriter writer, object instanceObj)
        {
            string str = (string)instanceObj;
            writer.WriteString(str);
        }

        private static object DecodeBinary(ByteCodeReader reader) => reader.ReadString();

        private static Result ConvertFromObject(object valueObj, out object instanceObj)
        {
            object obj = valueObj;
            instanceObj = null;
            string str = obj == null ? null : obj.ToString();
            instanceObj = str;
            return Result.Success;
        }

        private static object CallIsNullOrEmptyString(object instanceObj, object[] parameters) => BooleanBoxes.Box(string.IsNullOrEmpty((string)parameters[0]));

        private static object CallSubstringInt32(object instanceObj, object[] parameters)
        {
            string str = (string)instanceObj;
            int parameter = (int)parameters[0];
            if (parameter >= 0 && parameter <= str.Length)
                return str.Substring(parameter);
            ErrorManager.ReportError("Script runtime failure: Invalid '{0}' value is out of range for '{1}'", parameter, "startIndex");
            return null;
        }

        private static object CallSubstringInt32Int32(object instanceObj, object[] parameters)
        {
            string str = (string)instanceObj;
            int parameter1 = (int)parameters[0];
            int parameter2 = (int)parameters[1];
            if (parameter1 < 0 || parameter1 > str.Length)
            {
                ErrorManager.ReportError("Script runtime failure: Invalid '{0}' value is out of range for '{1}'", parameter1, "startIndex");
                return null;
            }
            if (parameter2 >= 0 && parameter1 + parameter2 >= 0 && parameter1 + parameter2 <= str.Length)
                return str.Substring(parameter1, parameter2);
            ErrorManager.ReportError("Script runtime failure: Invalid '{0}' value is out of range for '{1}'", parameter2, "length");
            return null;
        }

        private static object CallTrim(object instanceObj, object[] parameters) => ((string)instanceObj).Trim();

        private static object CallToLower(object instanceObj, object[] parameters) => ((string)instanceObj).ToLowerInvariant();

        private static object CallToUpper(object instanceObj, object[] parameters) => ((string)instanceObj).ToUpperInvariant();

        private static object CallFormatObject(object instanceObj, object[] parameters)
        {
            string format = (string)instanceObj;
            object parameter = parameters[0];
            return string.Format(format, parameters);
        }

        private static object CallFormatObjectObject(object instanceObj, object[] parameters)
        {
            string format = (string)instanceObj;
            object parameter1 = parameters[0];
            object parameter2 = parameters[1];
            return string.Format(format, parameters);
        }

        private static object CallFormatObjectObjectObject(object instanceObj, object[] parameters)
        {
            string format = (string)instanceObj;
            object parameter1 = parameters[0];
            object parameter2 = parameters[1];
            object parameter3 = parameters[2];
            return string.Format(format, parameters);
        }

        private static object CallFormatObjectObjectObjectObject(
          object instanceObj,
          object[] parameters)
        {
            string format = (string)instanceObj;
            object parameter1 = parameters[0];
            object parameter2 = parameters[1];
            object parameter3 = parameters[2];
            object parameter4 = parameters[3];
            return string.Format(format, parameters);
        }

        private static object CallFormatObjectObjectObjectObjectObject(
          object instanceObj,
          object[] parameters)
        {
            string format = (string)instanceObj;
            object parameter1 = parameters[0];
            object parameter2 = parameters[1];
            object parameter3 = parameters[2];
            object parameter4 = parameters[3];
            object parameter5 = parameters[4];
            return string.Format(format, parameters);
        }

        private static bool IsConversionSupported(TypeSchema fromType) => ObjectSchema.Type.IsAssignableFrom(fromType);

        private static Result TryConvertFrom(
          object from,
          TypeSchema fromType,
          out object instance)
        {
            Result result = Result.Fail("Unsupported");
            instance = null;
            if (ObjectSchema.Type.IsAssignableFrom(fromType))
            {
                result = ConvertFromObject(from, out instance);
                if (!result.Failed)
                    return result;
            }
            return result;
        }

        private static bool IsOperationSupported(OperationType op)
        {
            switch (op)
            {
                case OperationType.MathAdd:
                case OperationType.RelationalEquals:
                case OperationType.RelationalNotEquals:
                    return true;
                default:
                    return false;
            }
        }

        private static object ExecuteOperation(object leftObj, object rightObj, OperationType op)
        {
            string str1 = (string)leftObj;
            string str2 = (string)rightObj;
            switch (op)
            {
                case OperationType.MathAdd:
                    return str1 + str2;
                case OperationType.RelationalEquals:
                    return BooleanBoxes.Box(str1 == str2);
                case OperationType.RelationalNotEquals:
                    return BooleanBoxes.Box(str1 != str2);
                default:
                    return null;
            }
        }

        public static void Pass1Initialize() => Type = new UIXTypeSchema(UIXTypeID.String, "String", "string", UIXTypeID.Object, typeof(string), UIXTypeFlags.Immutable);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema = new(UIXTypeID.String, "Length", UIXTypeID.Int32, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetLength), null, false);
            UIXMethodSchema uixMethodSchema1 = new(UIXTypeID.String, "IsNullOrEmpty", [UIXTypeID.String], UIXTypeID.Boolean, CallIsNullOrEmptyString, true);
            UIXMethodSchema uixMethodSchema2 = new(UIXTypeID.String, "Substring", [UIXTypeID.Int32], UIXTypeID.String, CallSubstringInt32, false);
            UIXMethodSchema uixMethodSchema3 = new(UIXTypeID.String, "Substring", [UIXTypeID.Int32, UIXTypeID.Int32], UIXTypeID.String, CallSubstringInt32Int32, false);
            UIXMethodSchema uixMethodSchema4 = new(UIXTypeID.String, "Trim", null, UIXTypeID.String, CallTrim, false);
            UIXMethodSchema uixMethodSchema5 = new(UIXTypeID.String, "ToLower", null, UIXTypeID.String, CallToLower, false);
            UIXMethodSchema uixMethodSchema6 = new(UIXTypeID.String, "ToUpper", null, UIXTypeID.String, CallToUpper, false);
            UIXMethodSchema uixMethodSchema7 = new(UIXTypeID.String, "Format", [UIXTypeID.Object], UIXTypeID.String, CallFormatObject, false);
            UIXMethodSchema uixMethodSchema8 = new(UIXTypeID.String, "Format", [UIXTypeID.Object, UIXTypeID.Object], UIXTypeID.String, CallFormatObjectObject, false);
            UIXMethodSchema uixMethodSchema9 = new(UIXTypeID.String, "Format", [UIXTypeID.Object, UIXTypeID.Object, UIXTypeID.Object], UIXTypeID.String, CallFormatObjectObjectObject, false);
            UIXMethodSchema uixMethodSchema10 = new(UIXTypeID.String, "Format", [UIXTypeID.Object, UIXTypeID.Object, UIXTypeID.Object, UIXTypeID.Object], UIXTypeID.String, CallFormatObjectObjectObjectObject, false);
            UIXMethodSchema uixMethodSchema11 = new(UIXTypeID.String, "Format", [UIXTypeID.Object, UIXTypeID.Object, UIXTypeID.Object, UIXTypeID.Object, UIXTypeID.Object], UIXTypeID.String, CallFormatObjectObjectObjectObjectObject, false);
            Type.Initialize(Construct, null, [uixPropertySchema],
            [
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
            ], null, null, TryConvertFrom, IsConversionSupported, EncodeBinary, DecodeBinary, ExecuteOperation, IsOperationSupported);
        }
    }
}
