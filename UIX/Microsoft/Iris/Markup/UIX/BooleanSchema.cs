// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.BooleanSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class BooleanSchema
    {
        public static UIXTypeSchema Type;

        private static object Construct() => BooleanBoxes.FalseBox;

        private static void EncodeBinary(ByteCodeWriter writer, object instanceObj)
        {
            bool flag = (bool)instanceObj;
            writer.WriteBool(flag);
        }

        private static object DecodeBinary(ByteCodeReader reader) => BooleanBoxes.Box(reader.ReadBool());

        private static Result ConvertFromString(object valueObj, out object instanceObj)
        {
            string str = (string)valueObj;
            instanceObj = null;
            bool result;
            if (!bool.TryParse(str, out result))
                return Result.Fail("Unable to convert \"{0}\" to type '{1}'", str, "Boolean");
            instanceObj = BooleanBoxes.Box(result);
            return Result.Success;
        }

        private static Result ConvertFromInt32(object valueObj, out object instanceObj)
        {
            int num = (int)valueObj;
            instanceObj = null;
            bool flag = num != 0;
            instanceObj = BooleanBoxes.Box(flag);
            return Result.Success;
        }

        private static Result ConvertFromInt64(object valueObj, out object instanceObj)
        {
            long num = (long)valueObj;
            instanceObj = null;
            bool flag = num != 0L;
            instanceObj = BooleanBoxes.Box(flag);
            return Result.Success;
        }

        private static Result ConvertFromSingle(object valueObj, out object instanceObj)
        {
            float num = (float)valueObj;
            instanceObj = null;
            bool flag = num != 0.0;
            instanceObj = BooleanBoxes.Box(flag);
            return Result.Success;
        }

        private static Result ConvertFromDouble(object valueObj, out object instanceObj)
        {
            double num = (double)valueObj;
            instanceObj = null;
            bool flag = num != 0.0;
            instanceObj = BooleanBoxes.Box(flag);
            return Result.Success;
        }

        private static bool IsConversionSupported(TypeSchema fromType) => DoubleSchema.Type.IsAssignableFrom(fromType) || Int32Schema.Type.IsAssignableFrom(fromType) || (Int64Schema.Type.IsAssignableFrom(fromType) || SingleSchema.Type.IsAssignableFrom(fromType)) || StringSchema.Type.IsAssignableFrom(fromType);

        private static Result TryConvertFrom(
          object from,
          TypeSchema fromType,
          out object instance)
        {
            Result result = Result.Fail("Unsupported");
            instance = null;
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
                case OperationType.LogicalAnd:
                case OperationType.LogicalOr:
                case OperationType.RelationalEquals:
                case OperationType.RelationalNotEquals:
                case OperationType.LogicalNot:
                    return true;
                default:
                    return false;
            }
        }

        private static object ExecuteOperation(object leftObj, object rightObj, OperationType op)
        {
            bool flag1 = (bool)leftObj;
            if (op == OperationType.LogicalNot)
                return BooleanBoxes.Box(!flag1);
            bool flag2 = (bool)rightObj;
            switch (op - 6)
            {
                case 0:
                    return BooleanBoxes.Box(flag1 && flag2);
                case OperationType.MathAdd:
                    return BooleanBoxes.Box(flag1 || flag2);
                case OperationType.MathSubtract:
                    return BooleanBoxes.Box(flag1 == flag2);
                case OperationType.MathMultiply:
                    return BooleanBoxes.Box(flag1 != flag2);
                default:
                    return null;
            }
        }

        private static object CallTryParseStringBoolean(object instanceObj, object[] parameters)
        {
            string parameter1 = (string)parameters[0];
            bool parameter2 = (bool)parameters[1];
            object instanceObj1;
            return ConvertFromString(parameter1, out instanceObj1).Failed ? parameter2 : instanceObj1;
        }

        public static void Pass1Initialize() => Type = new UIXTypeSchema(15, "Boolean", "bool", 153, typeof(bool), UIXTypeFlags.Immutable);

        public static void Pass2Initialize()
        {
            UIXMethodSchema uixMethodSchema = new UIXMethodSchema(15, "TryParse", new short[2]
            {
         208,
         15
            }, 15, new InvokeHandler(CallTryParseStringBoolean), true);
            Type.Initialize(new DefaultConstructHandler(Construct), null, null, new MethodSchema[1]
            {
         uixMethodSchema
            }, null, null, new TypeConverterHandler(TryConvertFrom), new SupportsTypeConversionHandler(IsConversionSupported), new EncodeBinaryHandler(EncodeBinary), new DecodeBinaryHandler(DecodeBinary), new PerformOperationHandler(ExecuteOperation), new SupportsOperationHandler(IsOperationSupported));
        }
    }
}
