// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.ByteSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;
using System;
using System.Globalization;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class ByteSchema
    {
        public static UIXTypeSchema Type;

        private static object Construct() => (byte)0;

        private static void EncodeBinary(ByteCodeWriter writer, object instanceObj)
        {
            byte num = (byte)instanceObj;
            writer.WriteByte(num);
        }

        private static object DecodeBinary(ByteCodeReader reader) => reader.ReadByte();

        private static object CallToStringString(object instanceObj, object[] parameters) => ((byte)instanceObj).ToString((string)parameters[0]);

        private static Result ConvertFromString(object valueObj, out object instanceObj)
        {
            string s = (string)valueObj;
            instanceObj = null;
            byte result;
            if (!byte.TryParse(s, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out result))
                return Result.Fail("Unable to convert \"{0}\" to type '{1}'", s, "Byte");
            instanceObj = result;
            return Result.Success;
        }

        private static Result ConvertFromBoolean(object valueObj, out object instanceObj)
        {
            bool flag = (bool)valueObj;
            instanceObj = null;
            byte num = 0;
            if (flag) num = 1;
            instanceObj = num;
            return Result.Success;
        }

        private static Result ConvertFromInt32(object valueObj, out object instanceObj)
        {
            int num1 = (int)valueObj;
            instanceObj = null;
            byte num2 = (byte)num1;
            instanceObj = num2;
            return Result.Success;
        }

        private static Result ConvertFromInt64(object valueObj, out object instanceObj)
        {
            long num1 = (long)valueObj;
            instanceObj = null;
            byte num2 = (byte)num1;
            instanceObj = num2;
            return Result.Success;
        }

        private static Result ConvertFromSingle(object valueObj, out object instanceObj)
        {
            float num1 = (float)valueObj;
            instanceObj = null;
            byte num2 = (byte)num1;
            instanceObj = num2;
            return Result.Success;
        }

        private static Result ConvertFromDouble(object valueObj, out object instanceObj)
        {
            double num1 = (double)valueObj;
            instanceObj = null;
            byte num2 = (byte)num1;
            instanceObj = num2;
            return Result.Success;
        }

        private static bool IsConversionSupported(TypeSchema fromType) => BooleanSchema.Type.IsAssignableFrom(fromType) || DoubleSchema.Type.IsAssignableFrom(fromType) || (Int32Schema.Type.IsAssignableFrom(fromType) || Int64Schema.Type.IsAssignableFrom(fromType)) || (SingleSchema.Type.IsAssignableFrom(fromType) || StringSchema.Type.IsAssignableFrom(fromType));

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
                    return true;
                default:
                    return false;
            }
        }

        private static object ExecuteOperation(object leftObj, object rightObj, OperationType op)
        {
            byte num1 = (byte)leftObj;
            byte num2 = (byte)rightObj;
            switch (op)
            {
                case OperationType.MathAdd:
                    return (byte)(num1 + (uint)num2);
                case OperationType.MathSubtract:
                    return (byte)(num1 - (uint)num2);
                case OperationType.MathMultiply:
                    return (byte)(num1 * (uint)num2);
                case OperationType.MathDivide:
                    return (byte)(num1 / (uint)num2);
                case OperationType.MathModulus:
                    return (byte)(num1 % (uint)num2);
                case OperationType.RelationalEquals:
                    return BooleanBoxes.Box(num1 == num2);
                case OperationType.RelationalNotEquals:
                    return BooleanBoxes.Box(num1 != num2);
                case OperationType.RelationalLessThan:
                    return BooleanBoxes.Box(num1 < num2);
                case OperationType.RelationalGreaterThan:
                    return BooleanBoxes.Box(num1 > num2);
                case OperationType.RelationalLessThanEquals:
                    return BooleanBoxes.Box(num1 <= num2);
                case OperationType.RelationalGreaterThanEquals:
                    return BooleanBoxes.Box(num1 >= num2);
                default:
                    return null;
            }
        }

        private static object CallTryParseStringByte(object instanceObj, object[] parameters)
        {
            string parameter1 = (string)parameters[0];
            byte parameter2 = (byte)parameters[1];
            object instanceObj1;
            return ConvertFromString(parameter1, out instanceObj1).Failed ? parameter2 : instanceObj1;
        }

        public static void Pass1Initialize() => Type = new UIXTypeSchema(19, "Byte", "byte", 153, typeof(byte), UIXTypeFlags.Immutable);

        public static void Pass2Initialize()
        {
            UIXMethodSchema uixMethodSchema1 = new UIXMethodSchema(19, "ToString", new short[1]
            {
         208
            }, 208, new InvokeHandler(CallToStringString), false);
            UIXMethodSchema uixMethodSchema2 = new UIXMethodSchema(19, "TryParse", new short[2]
            {
         208,
         19
            }, 19, new InvokeHandler(CallTryParseStringByte), true);
            Type.Initialize(new DefaultConstructHandler(Construct), null, null, new MethodSchema[2]
            {
         uixMethodSchema1,
         uixMethodSchema2
            }, null, null, new TypeConverterHandler(TryConvertFrom), new SupportsTypeConversionHandler(IsConversionSupported), new EncodeBinaryHandler(EncodeBinary), new DecodeBinaryHandler(DecodeBinary), new PerformOperationHandler(ExecuteOperation), new SupportsOperationHandler(IsOperationSupported));
        }
    }
}
