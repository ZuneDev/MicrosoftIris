// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.SingleSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;
using System;
using System.Globalization;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class SingleSchema
    {
        public static RangeValidator Validate0to1 = new RangeValidator(RangeValidate0to1);
        public static RangeValidator ValidateNotNegative = new RangeValidator(RangeValidateNotNegative);
        public static RangeValidator ValidateNotZero = new RangeValidator(RangeValidateNotZero);
        public static UIXTypeSchema Type;

        private static object Construct() => 0.0f;

        private static void EncodeBinary(ByteCodeWriter writer, object instanceObj)
        {
            float num = (float)instanceObj;
            writer.WriteSingle(num);
        }

        private static object DecodeBinary(ByteCodeReader reader) => reader.ReadSingle();

        private static Result ConvertFromString(object valueObj, out object instanceObj)
        {
            string s = (string)valueObj;
            instanceObj = null;
            float result;
            if (!float.TryParse(s, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out result))
                return Result.Fail("Unable to convert \"{0}\" to type '{1}'", s, "Single");
            instanceObj = result;
            return Result.Success;
        }

        private static Result ConvertFromBoolean(object valueObj, out object instanceObj)
        {
            bool flag = (bool)valueObj;
            instanceObj = null;
            float num = flag ? 1f : 0.0f;
            instanceObj = num;
            return Result.Success;
        }

        private static Result ConvertFromByte(object valueObj, out object instanceObj)
        {
            byte num1 = (byte)valueObj;
            instanceObj = null;
            float num2 = num1;
            instanceObj = num2;
            return Result.Success;
        }

        private static Result ConvertFromInt32(object valueObj, out object instanceObj)
        {
            int num1 = (int)valueObj;
            instanceObj = null;
            float num2 = num1;
            instanceObj = num2;
            return Result.Success;
        }

        private static Result ConvertFromInt64(object valueObj, out object instanceObj)
        {
            long num1 = (long)valueObj;
            instanceObj = null;
            float num2 = num1;
            instanceObj = num2;
            return Result.Success;
        }

        private static Result ConvertFromDouble(object valueObj, out object instanceObj)
        {
            double num1 = (double)valueObj;
            instanceObj = null;
            float num2 = (float)num1;
            instanceObj = num2;
            return Result.Success;
        }

        private static object CallToStringString(object instanceObj, object[] parameters) => ((float)instanceObj).ToString((string)parameters[0]);

        private static bool IsConversionSupported(TypeSchema fromType) => BooleanSchema.Type.IsAssignableFrom(fromType) || ByteSchema.Type.IsAssignableFrom(fromType) || (DoubleSchema.Type.IsAssignableFrom(fromType) || Int32Schema.Type.IsAssignableFrom(fromType)) || (Int64Schema.Type.IsAssignableFrom(fromType) || StringSchema.Type.IsAssignableFrom(fromType));

        private static Result TryConvertFrom(
          object from,
          TypeSchema fromType,
          out object instance)
        {
            Result result = Result.Fail("Unsupported");
            instance = null;
            if (BooleanSchema.Type.IsAssignableFrom(fromType))
            {
                result = ConvertFromBoolean(from, out instance);
                if (!result.Failed)
                    return result;
            }
            if (ByteSchema.Type.IsAssignableFrom(fromType))
            {
                result = ConvertFromByte(from, out instance);
                if (!result.Failed)
                    return result;
            }
            if (DoubleSchema.Type.IsAssignableFrom(fromType))
            {
                result = ConvertFromDouble(from, out instance);
                if (!result.Failed)
                    return result;
            }
            if (Int32Schema.Type.IsAssignableFrom(fromType))
            {
                result = ConvertFromInt32(from, out instance);
                if (!result.Failed)
                    return result;
            }
            if (Int64Schema.Type.IsAssignableFrom(fromType))
            {
                result = ConvertFromInt64(from, out instance);
                if (!result.Failed)
                    return result;
            }
            if (StringSchema.Type.IsAssignableFrom(fromType))
            {
                result = ConvertFromString(from, out instance);
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
                case OperationType.MathSubtract:
                case OperationType.MathMultiply:
                case OperationType.MathDivide:
                case OperationType.MathModulus:
                case OperationType.RelationalEquals:
                case OperationType.RelationalNotEquals:
                case OperationType.RelationalLessThan:
                case OperationType.RelationalGreaterThan:
                case OperationType.RelationalLessThanEquals:
                case OperationType.RelationalGreaterThanEquals:
                case OperationType.MathNegate:
                    return true;
                default:
                    return false;
            }
        }

        private static object ExecuteOperation(object leftObj, object rightObj, OperationType op)
        {
            float num1 = (float)leftObj;
            if (op == OperationType.MathNegate)
                return (float)-num1;
            float num2 = (float)rightObj;
            switch (op - 1)
            {
                case 0:
                    return (float)(num1 + (double)num2);
                case OperationType.MathAdd:
                    return (float)(num1 - (double)num2);
                case OperationType.MathSubtract:
                    return (float)(num1 * (double)num2);
                case OperationType.MathMultiply:
                    return (float)(num1 / (double)num2);
                case OperationType.MathDivide:
                    return (float)(num1 % (double)num2);
                case OperationType.LogicalOr:
                    return BooleanBoxes.Box(num1 == (double)num2);
                case OperationType.RelationalEquals:
                    return BooleanBoxes.Box(num1 != (double)num2);
                case OperationType.RelationalNotEquals:
                    return BooleanBoxes.Box(num1 < (double)num2);
                case OperationType.RelationalLessThan:
                    return BooleanBoxes.Box(num1 > (double)num2);
                case OperationType.RelationalGreaterThan:
                    return BooleanBoxes.Box(num1 <= (double)num2);
                case OperationType.RelationalLessThanEquals:
                    return BooleanBoxes.Box(num1 >= (double)num2);
                default:
                    return null;
            }
        }

        private static object CallTryParseStringSingle(object instanceObj, object[] parameters)
        {
            string parameter1 = (string)parameters[0];
            float parameter2 = (float)parameters[1];
            object instanceObj1;
            return ConvertFromString(parameter1, out instanceObj1).Failed ? parameter2 : instanceObj1;
        }

        private static Result RangeValidate0to1(object value)
        {
            float num = (float)value;
            return num < 0.0 || num > 1.0 ? Result.Fail("Expecting a value between {0} and {1}, but got {2}", "0.0", "1.0", num.ToString()) : Result.Success;
        }

        private static Result RangeValidateNotNegative(object value)
        {
            float num = (float)value;
            return num < 0.0 ? Result.Fail("Expecting a non-negative value, but got {0}", num.ToString()) : Result.Success;
        }

        private static Result RangeValidateNotZero(object value)
        {
            float num = (float)value;
            return num == 0.0 ? Result.Fail("Specified value '{0}' is not valid", num.ToString()) : Result.Success;
        }

        public static void Pass1Initialize() => Type = new UIXTypeSchema(194, "Single", "float", 153, typeof(float), UIXTypeFlags.Immutable);

        public static void Pass2Initialize()
        {
            UIXMethodSchema uixMethodSchema1 = new UIXMethodSchema(194, "ToString", new short[1]
            {
         208
            }, 208, new InvokeHandler(CallToStringString), false);
            UIXMethodSchema uixMethodSchema2 = new UIXMethodSchema(194, "TryParse", new short[2]
            {
         208,
         194
            }, 194, new InvokeHandler(CallTryParseStringSingle), true);
            Type.Initialize(new DefaultConstructHandler(Construct), null, null, new MethodSchema[2]
            {
         uixMethodSchema1,
         uixMethodSchema2
            }, null, null, new TypeConverterHandler(TryConvertFrom), new SupportsTypeConversionHandler(IsConversionSupported), new EncodeBinaryHandler(EncodeBinary), new DecodeBinaryHandler(DecodeBinary), new PerformOperationHandler(ExecuteOperation), new SupportsOperationHandler(IsOperationSupported));
        }
    }
}
