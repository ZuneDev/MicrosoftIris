// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.Vector2Schema
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
    internal static class Vector2Schema
    {
        public static RangeValidator ValidateNotNegative = new RangeValidator(RangeValidateNotNegative);
        public static UIXTypeSchema Type;

        private static object GetX(object instanceObj) => ((Vector2)instanceObj).X;

        private static void SetX(ref object instanceObj, object valueObj)
        {
            Vector2 vector2 = (Vector2)instanceObj;
            float num = (float)valueObj;
            vector2.X = num;
            instanceObj = vector2;
        }

        private static object GetY(object instanceObj) => ((Vector2)instanceObj).Y;

        private static void SetY(ref object instanceObj, object valueObj)
        {
            Vector2 vector2 = (Vector2)instanceObj;
            float num = (float)valueObj;
            vector2.Y = num;
            instanceObj = vector2;
        }

        private static object Construct() => Vector2.Zero;

        private static object ConstructXY(object[] parameters)
        {
            object instanceObj = Construct();
            SetX(ref instanceObj, parameters[0]);
            SetY(ref instanceObj, parameters[1]);
            return instanceObj;
        }

        private static Result ConvertFromStringXY(string[] splitString, out object instance)
        {
            instance = Construct();
            object valueObj1;
            Result result1 = UIXLoadResult.ValidateStringAsValue(splitString[0], SingleSchema.Type, null, out valueObj1);
            if (result1.Failed)
                return Result.Fail("Problem converting '{0}' ({1})", "Vector2", result1.Error);
            SetX(ref instance, valueObj1);
            object valueObj2;
            Result result2 = UIXLoadResult.ValidateStringAsValue(splitString[1], SingleSchema.Type, null, out valueObj2);
            if (result2.Failed)
                return Result.Fail("Problem converting '{0}' ({1})", "Vector2", result2.Error);
            SetY(ref instance, valueObj2);
            return result2;
        }

        private static void EncodeBinary(ByteCodeWriter writer, object instanceObj)
        {
            Vector2 vector2 = (Vector2)instanceObj;
            writer.WriteSingle(vector2.X);
            writer.WriteSingle(vector2.Y);
        }

        private static object DecodeBinary(ByteCodeReader reader) => new Vector2(reader.ReadSingle(), reader.ReadSingle());

        private static Result ConvertFromSize(object valueObj, out object instanceObj)
        {
            Size size = (Size)valueObj;
            instanceObj = null;
            Vector2 vector2 = new Vector2(size.Width, size.Height);
            instanceObj = vector2;
            return Result.Success;
        }

        private static Result ConvertFromString(object valueObj, out object instanceObj)
        {
            string s = (string)valueObj;
            instanceObj = null;
            float result;
            if (!float.TryParse(s, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out result))
                return Result.Fail("");
            instanceObj = new Vector2()
            {
                X = result,
                Y = result
            };
            return Result.Success;
        }

        private static Result ConvertFromSingle(object valueObj, out object instanceObj)
        {
            float num = (float)valueObj;
            instanceObj = null;
            instanceObj = new Vector2()
            {
                X = num,
                Y = num
            };
            return Result.Success;
        }

        private static bool IsConversionSupported(TypeSchema fromType) => SingleSchema.Type.IsAssignableFrom(fromType) || SizeSchema.Type.IsAssignableFrom(fromType) || StringSchema.Type.IsAssignableFrom(fromType);

        private static Result TryConvertFrom(
          object from,
          TypeSchema fromType,
          out object instance)
        {
            Result result = Result.Fail("Unsupported");
            instance = null;
            if (SingleSchema.Type.IsAssignableFrom(fromType))
            {
                result = ConvertFromSingle(from, out instance);
                if (!result.Failed)
                    return result;
            }
            if (SizeSchema.Type.IsAssignableFrom(fromType))
            {
                result = ConvertFromSize(from, out instance);
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
                if (splitString.Length == 2)
                {
                    result = ConvertFromStringXY(splitString, out instance);
                    if (!result.Failed)
                        return result;
                }
                else
                    result = Result.Fail("Unable to convert \"{0}\" to type '{1}'", from.ToString(), "Vector2");
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
            Vector2 vector2_1 = (Vector2)leftObj;
            if (op == OperationType.MathNegate)
                return -vector2_1;
            Vector2 vector2_2 = (Vector2)rightObj;
            switch (op - 1)
            {
                case 0:
                    return vector2_1 + vector2_2;
                case OperationType.MathAdd:
                    return vector2_1 - vector2_2;
                case OperationType.MathSubtract:
                    return vector2_1 * vector2_2;
                case OperationType.MathMultiply:
                    return vector2_1 / vector2_2;
                case OperationType.LogicalOr:
                    return BooleanBoxes.Box(vector2_1 == vector2_2);
                case OperationType.RelationalEquals:
                    return BooleanBoxes.Box(vector2_1 != vector2_2);
                default:
                    return null;
            }
        }

        private static object CallTryParseStringVector2(object instanceObj, object[] parameters)
        {
            string parameter1 = (string)parameters[0];
            Vector2 parameter2 = (Vector2)parameters[1];
            object instanceObj1;
            return ConvertFromString(parameter1, out instanceObj1).Failed ? parameter2 : instanceObj1;
        }

        private static Result RangeValidateNotNegative(object value)
        {
            Vector2 vector2 = (Vector2)value;
            return vector2.X < 0.0 || vector2.Y < 0.0 ? Result.Fail("Expecting a non-negative value, but got {0}", vector2.ToString()) : Result.Success;
        }

        public static void Pass1Initialize() => Type = new UIXTypeSchema(233, "Vector2", null, 153, typeof(Vector2), UIXTypeFlags.Immutable);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(233, "X", 194, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetX), new SetValueHandler(SetX), false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(233, "Y", 194, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetY), new SetValueHandler(SetY), false);
            UIXConstructorSchema constructorSchema = new UIXConstructorSchema(233, new short[2]
            {
         194,
         194
            }, new ConstructHandler(ConstructXY));
            UIXMethodSchema uixMethodSchema = new UIXMethodSchema(233, "TryParse", new short[2]
            {
         208,
         233
            }, 233, new InvokeHandler(CallTryParseStringVector2), true);
            Type.Initialize(new DefaultConstructHandler(Construct), new ConstructorSchema[1]
            {
         constructorSchema
            }, new PropertySchema[2]
            {
         uixPropertySchema1,
         uixPropertySchema2
            }, new MethodSchema[1]
            {
         uixMethodSchema
            }, null, null, new TypeConverterHandler(TryConvertFrom), new SupportsTypeConversionHandler(IsConversionSupported), new EncodeBinaryHandler(EncodeBinary), new DecodeBinaryHandler(DecodeBinary), new PerformOperationHandler(ExecuteOperation), new SupportsOperationHandler(IsOperationSupported));
        }
    }
}
