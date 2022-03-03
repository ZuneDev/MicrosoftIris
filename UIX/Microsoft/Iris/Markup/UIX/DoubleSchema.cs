// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.DoubleSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;
using System;
using System.Globalization;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class DoubleSchema
    {
        public static UIXTypeSchema Type;

        private static object Construct() => 0.0;

        private static void EncodeBinary(ByteCodeWriter writer, object instanceObj)
        {
            double num = (double)instanceObj;
            writer.WriteDouble(num);
        }

        private static object DecodeBinary(ByteCodeReader reader) => reader.ReadDouble();

        private static Result ConvertFromString(object valueObj, out object instanceObj)
        {
            string s = (string)valueObj;
            instanceObj = null;
            double result;
            if (!double.TryParse(s, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out result))
                return Result.Fail("Unable to convert \"{0}\" to type '{1}'", s, "Double");
            instanceObj = result;
            return Result.Success;
        }

        private static Result ConvertFromBoolean(object valueObj, out object instanceObj)
        {
            bool flag = (bool)valueObj;
            instanceObj = null;
            double num = flag ? 1.0 : 0.0;
            instanceObj = num;
            return Result.Success;
        }

        private static Result ConvertFromByte(object valueObj, out object instanceObj)
        {
            byte num1 = (byte)valueObj;
            instanceObj = null;
            double num2 = num1;
            instanceObj = num2;
            return Result.Success;
        }

        private static Result ConvertFromInt32(object valueObj, out object instanceObj)
        {
            int num1 = (int)valueObj;
            instanceObj = null;
            double num2 = num1;
            instanceObj = num2;
            return Result.Success;
        }

        private static Result ConvertFromInt64(object valueObj, out object instanceObj)
        {
            long num1 = (long)valueObj;
            instanceObj = null;
            double num2 = num1;
            instanceObj = num2;
            return Result.Success;
        }

        private static Result ConvertFromSingle(object valueObj, out object instanceObj)
        {
            float num1 = (float)valueObj;
            instanceObj = null;
            double num2 = num1;
            instanceObj = num2;
            return Result.Success;
        }

        private static object CallToStringString(object instanceObj, object[] parameters) => ((double)instanceObj).ToString((string)parameters[0]);

        private static object CallIsNaNDouble(object instanceObj, object[] parameters) => BooleanBoxes.Box(double.IsNaN((double)parameters[0]));

        private static object CallIsNegativeInfinityDouble(object instanceObj, object[] parameters) => BooleanBoxes.Box(double.IsNegativeInfinity((double)parameters[0]));

        private static object CallIsPositiveInfinityDouble(object instanceObj, object[] parameters) => BooleanBoxes.Box(double.IsPositiveInfinity((double)parameters[0]));

        private static bool IsConversionSupported(TypeSchema fromType) => BooleanSchema.Type.IsAssignableFrom(fromType) || ByteSchema.Type.IsAssignableFrom(fromType) || (Int32Schema.Type.IsAssignableFrom(fromType) || Int64Schema.Type.IsAssignableFrom(fromType)) || (SingleSchema.Type.IsAssignableFrom(fromType) || StringSchema.Type.IsAssignableFrom(fromType));

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
            double num1 = (double)leftObj;
            if (op == OperationType.MathNegate)
                return -num1;
            double num2 = (double)rightObj;
            switch (op - 1)
            {
                case 0:
                    return num1 + num2;
                case OperationType.MathAdd:
                    return num1 - num2;
                case OperationType.MathSubtract:
                    return num1 * num2;
                case OperationType.MathMultiply:
                    return num1 / num2;
                case OperationType.MathDivide:
                    return num1 % num2;
                case OperationType.LogicalOr:
                    return BooleanBoxes.Box(num1 == num2);
                case OperationType.RelationalEquals:
                    return BooleanBoxes.Box(num1 != num2);
                case OperationType.RelationalNotEquals:
                    return BooleanBoxes.Box(num1 < num2);
                case OperationType.RelationalLessThan:
                    return BooleanBoxes.Box(num1 > num2);
                case OperationType.RelationalGreaterThan:
                    return BooleanBoxes.Box(num1 <= num2);
                case OperationType.RelationalLessThanEquals:
                    return BooleanBoxes.Box(num1 >= num2);
                default:
                    return null;
            }
        }

        private static object CallTryParseStringDouble(object instanceObj, object[] parameters)
        {
            string parameter1 = (string)parameters[0];
            double parameter2 = (double)parameters[1];
            object instanceObj1;
            return ConvertFromString(parameter1, out instanceObj1).Failed ? parameter2 : instanceObj1;
        }

        public static void Pass1Initialize() => Type = new UIXTypeSchema(61, "Double", "double", 153, typeof(double), UIXTypeFlags.Immutable);

        public static void Pass2Initialize()
        {
            UIXMethodSchema uixMethodSchema1 = new UIXMethodSchema(61, "ToString", new short[1]
            {
         208
            }, 208, new InvokeHandler(CallToStringString), false);
            UIXMethodSchema uixMethodSchema2 = new UIXMethodSchema(61, "IsNaN", new short[1]
            {
         61
            }, 15, new InvokeHandler(CallIsNaNDouble), true);
            UIXMethodSchema uixMethodSchema3 = new UIXMethodSchema(61, "IsNegativeInfinity", new short[1]
            {
         61
            }, 15, new InvokeHandler(CallIsNegativeInfinityDouble), true);
            UIXMethodSchema uixMethodSchema4 = new UIXMethodSchema(61, "IsPositiveInfinity", new short[1]
            {
         61
            }, 15, new InvokeHandler(CallIsPositiveInfinityDouble), true);
            UIXMethodSchema uixMethodSchema5 = new UIXMethodSchema(61, "TryParse", new short[2]
            {
         208,
         61
            }, 61, new InvokeHandler(CallTryParseStringDouble), true);
            Type.Initialize(new DefaultConstructHandler(Construct), null, null, new MethodSchema[5]
            {
         uixMethodSchema1,
         uixMethodSchema2,
         uixMethodSchema3,
         uixMethodSchema4,
         uixMethodSchema5
            }, null, null, new TypeConverterHandler(TryConvertFrom), new SupportsTypeConversionHandler(IsConversionSupported), new EncodeBinaryHandler(EncodeBinary), new DecodeBinaryHandler(DecodeBinary), new PerformOperationHandler(ExecuteOperation), new SupportsOperationHandler(IsOperationSupported));
        }
    }
}
