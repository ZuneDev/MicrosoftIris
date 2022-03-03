// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.Int64Schema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;
using System;
using System.Globalization;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class Int64Schema
    {
        public static UIXTypeSchema Type;

        private static object GetMinValue(object instanceObj) => Int64Boxes.MinValueBox;

        private static object GetMaxValue(object instanceObj) => Int64Boxes.MaxValueBox;

        private static object Construct() => Int64Boxes.ZeroBox;

        private static void EncodeBinary(ByteCodeWriter writer, object instanceObj)
        {
            long num = (long)instanceObj;
            writer.WriteInt64(num);
        }

        private static object DecodeBinary(ByteCodeReader reader) => reader.ReadInt64();

        private static Result ConvertFromString(object valueObj, out object instanceObj)
        {
            string s = (string)valueObj;
            instanceObj = null;
            long result;
            if (!long.TryParse(s, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out result))
                return Result.Fail("Unable to convert \"{0}\" to type '{1}'", s, "Int64");
            instanceObj = result;
            return Result.Success;
        }

        private static Result ConvertFromBoolean(object valueObj, out object instanceObj)
        {
            bool flag = (bool)valueObj;
            instanceObj = null;
            long num = flag ? 1L : 0L;
            instanceObj = num;
            return Result.Success;
        }

        private static Result ConvertFromByte(object valueObj, out object instanceObj)
        {
            byte num1 = (byte)valueObj;
            instanceObj = null;
            long num2 = num1;
            instanceObj = num2;
            return Result.Success;
        }

        private static Result ConvertFromSingle(object valueObj, out object instanceObj)
        {
            float num1 = (float)valueObj;
            instanceObj = null;
            long num2 = (long)num1;
            instanceObj = num2;
            return Result.Success;
        }

        private static Result ConvertFromInt32(object valueObj, out object instanceObj)
        {
            int num1 = (int)valueObj;
            instanceObj = null;
            long num2 = num1;
            instanceObj = num2;
            return Result.Success;
        }

        private static Result ConvertFromDouble(object valueObj, out object instanceObj)
        {
            double num1 = (double)valueObj;
            instanceObj = null;
            long num2 = (long)num1;
            instanceObj = num2;
            return Result.Success;
        }

        private static object CallToStringString(object instanceObj, object[] parameters) => ((long)instanceObj).ToString((string)parameters[0]);

        private static bool IsConversionSupported(TypeSchema fromType) => BooleanSchema.Type.IsAssignableFrom(fromType) || ByteSchema.Type.IsAssignableFrom(fromType) || (DoubleSchema.Type.IsAssignableFrom(fromType) || Int32Schema.Type.IsAssignableFrom(fromType)) || (SingleSchema.Type.IsAssignableFrom(fromType) || StringSchema.Type.IsAssignableFrom(fromType));

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
            long num1 = (long)leftObj;
            if (op == OperationType.MathNegate)
                return -num1;
            long num2 = (long)rightObj;
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

        private static object CallTryParseStringInt64(object instanceObj, object[] parameters)
        {
            string parameter1 = (string)parameters[0];
            long parameter2 = (long)parameters[1];
            object instanceObj1;
            return ConvertFromString(parameter1, out instanceObj1).Failed ? parameter2 : instanceObj1;
        }

        public static void Pass1Initialize() => Type = new UIXTypeSchema(116, "Int64", "long", 153, typeof(long), UIXTypeFlags.Immutable);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(116, "MinValue", 116, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetMinValue), null, true);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(116, "MaxValue", 116, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetMaxValue), null, true);
            UIXMethodSchema uixMethodSchema1 = new UIXMethodSchema(116, "ToString", new short[1]
            {
         208
            }, 208, new InvokeHandler(CallToStringString), false);
            UIXMethodSchema uixMethodSchema2 = new UIXMethodSchema(116, "TryParse", new short[2]
            {
         208,
         116
            }, 116, new InvokeHandler(CallTryParseStringInt64), true);
            Type.Initialize(new DefaultConstructHandler(Construct), null, new PropertySchema[2]
            {
         uixPropertySchema2,
         uixPropertySchema1
            }, new MethodSchema[2]
            {
         uixMethodSchema1,
         uixMethodSchema2
            }, null, null, new TypeConverterHandler(TryConvertFrom), new SupportsTypeConversionHandler(IsConversionSupported), new EncodeBinaryHandler(EncodeBinary), new DecodeBinaryHandler(DecodeBinary), new PerformOperationHandler(ExecuteOperation), new SupportsOperationHandler(IsOperationSupported));
        }
    }
}
