// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.InsetSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Data;
using Microsoft.Iris.Library;
using Microsoft.Iris.Render;
using System;
using System.Globalization;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class InsetSchema
    {
        private static readonly object s_Default = Inset.Zero;
        public static UIXTypeSchema Type;

        private static object GetLeft(object instanceObj) => ((Inset)instanceObj).Left;

        private static void SetLeft(ref object instanceObj, object valueObj)
        {
            Inset inset = (Inset)instanceObj;
            int num = (int)valueObj;
            inset.Left = num;
            instanceObj = inset;
        }

        private static object GetTop(object instanceObj) => ((Inset)instanceObj).Top;

        private static void SetTop(ref object instanceObj, object valueObj)
        {
            Inset inset = (Inset)instanceObj;
            int num = (int)valueObj;
            inset.Top = num;
            instanceObj = inset;
        }

        private static object GetRight(object instanceObj) => ((Inset)instanceObj).Right;

        private static void SetRight(ref object instanceObj, object valueObj)
        {
            Inset inset = (Inset)instanceObj;
            int num = (int)valueObj;
            inset.Right = num;
            instanceObj = inset;
        }

        private static object GetBottom(object instanceObj) => ((Inset)instanceObj).Bottom;

        private static void SetBottom(ref object instanceObj, object valueObj)
        {
            Inset inset = (Inset)instanceObj;
            int num = (int)valueObj;
            inset.Bottom = num;
            instanceObj = inset;
        }

        private static object Construct() => s_Default;

        private static object ConstructInt32(object[] parameters)
        {
            int parameter = (int)parameters[0];
            return new Inset(parameter, parameter, parameter, parameter);
        }

        private static void EncodeBinary(ByteCodeWriter writer, object instanceObj)
        {
            Inset inset = (Inset)instanceObj;
            writer.WriteInt32(inset.Left);
            writer.WriteInt32(inset.Top);
            writer.WriteInt32(inset.Right);
            writer.WriteInt32(inset.Bottom);
        }

        private static object DecodeBinary(ByteCodeReader reader) => new Inset(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());

        private static Result ConvertFromString(object valueObj, out object instanceObj)
        {
            string s = (string)valueObj;
            instanceObj = null;
            int result;
            if (!int.TryParse(s, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out result))
                return Result.Fail("");
            instanceObj = new Inset()
            {
                Left = result,
                Top = result,
                Right = result,
                Bottom = result
            };
            return Result.Success;
        }

        private static Result ConvertFromInt32(object valueObj, out object instanceObj)
        {
            int num = (int)valueObj;
            instanceObj = null;
            instanceObj = new Inset()
            {
                Left = num,
                Top = num,
                Right = num,
                Bottom = num
            };
            return Result.Success;
        }

        private static Result ConvertFromSingle(object valueObj, out object instanceObj)
        {
            float num1 = (float)valueObj;
            instanceObj = null;
            Inset inset = new Inset();
            int num2 = (int)num1;
            inset.Left = num2;
            inset.Top = num2;
            inset.Right = num2;
            inset.Bottom = num2;
            instanceObj = inset;
            return Result.Success;
        }

        private static object ConstructLeftTopRightBottom(object[] parameters)
        {
            object instanceObj = Construct();
            SetLeft(ref instanceObj, parameters[0]);
            SetTop(ref instanceObj, parameters[1]);
            SetRight(ref instanceObj, parameters[2]);
            SetBottom(ref instanceObj, parameters[3]);
            return instanceObj;
        }

        private static Result ConvertFromStringLeftTopRightBottom(
          string[] splitString,
          out object instance)
        {
            instance = Construct();
            object valueObj1;
            Result result1 = UIXLoadResult.ValidateStringAsValue(splitString[0], Int32Schema.Type, null, out valueObj1);
            if (result1.Failed)
                return Result.Fail("Problem converting '{0}' ({1})", "Inset", result1.Error);
            SetLeft(ref instance, valueObj1);
            object valueObj2;
            Result result2 = UIXLoadResult.ValidateStringAsValue(splitString[1], Int32Schema.Type, null, out valueObj2);
            if (result2.Failed)
                return Result.Fail("Problem converting '{0}' ({1})", "Inset", result2.Error);
            SetTop(ref instance, valueObj2);
            object valueObj3;
            Result result3 = UIXLoadResult.ValidateStringAsValue(splitString[2], Int32Schema.Type, null, out valueObj3);
            if (result3.Failed)
                return Result.Fail("Problem converting '{0}' ({1})", "Inset", result3.Error);
            SetRight(ref instance, valueObj3);
            object valueObj4;
            Result result4 = UIXLoadResult.ValidateStringAsValue(splitString[3], Int32Schema.Type, null, out valueObj4);
            if (result4.Failed)
                return Result.Fail("Problem converting '{0}' ({1})", "Inset", result4.Error);
            SetBottom(ref instance, valueObj4);
            return result4;
        }

        private static bool IsConversionSupported(TypeSchema fromType) => Int32Schema.Type.IsAssignableFrom(fromType) || SingleSchema.Type.IsAssignableFrom(fromType) || StringSchema.Type.IsAssignableFrom(fromType);

        private static Result TryConvertFrom(
          object from,
          TypeSchema fromType,
          out object instance)
        {
            Result result = Result.Fail("Unsupported");
            instance = null;
            if (Int32Schema.Type.IsAssignableFrom(fromType))
            {
                result = ConvertFromInt32(from, out instance);
                if (!result.Failed)
                    return result;
            }
            if (SingleSchema.Type.IsAssignableFrom(fromType))
            {
                result = ConvertFromSingle(from, out instance);
                if (!result.Failed)
                    return result;
            }
            if (StringSchema.Type.IsAssignableFrom(fromType))
            {
                result = ConvertFromString(from, out instance);
                if (!result.Failed)
                    return result;
            }
            if (StringSchema.Type.IsAssignableFrom(fromType))
            {
                string[] splitString = StringUtility.SplitAndTrim(',', (string)from);
                if (splitString.Length == 4)
                {
                    result = ConvertFromStringLeftTopRightBottom(splitString, out instance);
                    if (!result.Failed)
                        return result;
                }
                else
                    result = Result.Fail("Unable to convert \"{0}\" to type '{1}'", from.ToString(), "Inset");
            }
            return result;
        }

        private static bool IsOperationSupported(OperationType op)
        {
            switch (op)
            {
                case OperationType.MathAdd:
                case OperationType.MathSubtract:
                case OperationType.RelationalEquals:
                case OperationType.RelationalNotEquals:
                case OperationType.MathNegate:
                    return true;
                default:
                    return false;
            }
        }

        private static object ExecuteOperation(object leftObj, object rightObj, OperationType op)
        {
            Inset inset1 = (Inset)leftObj;
            if (op == OperationType.MathNegate)
                return -inset1;
            Inset inset2 = (Inset)rightObj;
            switch (op)
            {
                case OperationType.MathAdd:
                    return inset1 + inset2;
                case OperationType.MathSubtract:
                    return inset1 - inset2;
                case OperationType.RelationalEquals:
                    return BooleanBoxes.Box(inset1 == inset2);
                case OperationType.RelationalNotEquals:
                    return BooleanBoxes.Box(inset1 != inset2);
                default:
                    return null;
            }
        }

        private static object CallTryParseStringInset(object instanceObj, object[] parameters)
        {
            string parameter1 = (string)parameters[0];
            Inset parameter2 = (Inset)parameters[1];
            object instanceObj1;
            return ConvertFromString(parameter1, out instanceObj1).Failed ? parameter2 : instanceObj1;
        }

        public static void Pass1Initialize() => Type = new UIXTypeSchema(114, "Inset", null, 153, typeof(Inset), UIXTypeFlags.Immutable);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(114, "Left", 115, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetLeft), new SetValueHandler(SetLeft), false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(114, "Top", 115, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetTop), new SetValueHandler(SetTop), false);
            UIXPropertySchema uixPropertySchema3 = new UIXPropertySchema(114, "Right", 115, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetRight), new SetValueHandler(SetRight), false);
            UIXPropertySchema uixPropertySchema4 = new UIXPropertySchema(114, "Bottom", 115, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetBottom), new SetValueHandler(SetBottom), false);
            UIXConstructorSchema constructorSchema1 = new UIXConstructorSchema(114, new short[1]
            {
         115
            }, new ConstructHandler(ConstructInt32));
            UIXConstructorSchema constructorSchema2 = new UIXConstructorSchema(114, new short[4]
            {
         115,
         115,
         115,
         115
            }, new ConstructHandler(ConstructLeftTopRightBottom));
            UIXMethodSchema uixMethodSchema = new UIXMethodSchema(114, "TryParse", new short[2]
            {
         208,
         114
            }, 114, new InvokeHandler(CallTryParseStringInset), true);
            Type.Initialize(new DefaultConstructHandler(Construct), new ConstructorSchema[2]
            {
         constructorSchema1,
         constructorSchema2
            }, new PropertySchema[4]
            {
         uixPropertySchema4,
         uixPropertySchema1,
         uixPropertySchema3,
         uixPropertySchema2
            }, new MethodSchema[1]
            {
         uixMethodSchema
            }, null, null, new TypeConverterHandler(TryConvertFrom), new SupportsTypeConversionHandler(IsConversionSupported), new EncodeBinaryHandler(EncodeBinary), new DecodeBinaryHandler(DecodeBinary), new PerformOperationHandler(ExecuteOperation), new SupportsOperationHandler(IsOperationSupported));
        }
    }
}
