// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.Vector3Schema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Data;
using Microsoft.Iris.Library;
using Microsoft.Iris.Render;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class Vector3Schema
    {
        public static UIXTypeSchema Type;

        private static object GetX(object instanceObj) => ((Vector3)instanceObj).X;

        private static void SetX(ref object instanceObj, object valueObj)
        {
            Vector3 vector3 = (Vector3)instanceObj;
            float num = (float)valueObj;
            vector3.X = num;
            instanceObj = vector3;
        }

        private static object GetY(object instanceObj) => ((Vector3)instanceObj).Y;

        private static void SetY(ref object instanceObj, object valueObj)
        {
            Vector3 vector3 = (Vector3)instanceObj;
            float num = (float)valueObj;
            vector3.Y = num;
            instanceObj = vector3;
        }

        private static object GetZ(object instanceObj) => ((Vector3)instanceObj).Z;

        private static void SetZ(ref object instanceObj, object valueObj)
        {
            Vector3 vector3 = (Vector3)instanceObj;
            float num = (float)valueObj;
            vector3.Z = num;
            instanceObj = vector3;
        }

        private static object Construct() => Vector3.Zero;

        private static object ConstructXYZ(object[] parameters)
        {
            object instanceObj = Construct();
            SetX(ref instanceObj, parameters[0]);
            SetY(ref instanceObj, parameters[1]);
            SetZ(ref instanceObj, parameters[2]);
            return instanceObj;
        }

        private static Result ConvertFromStringXYZ(string[] splitString, out object instance)
        {
            instance = Construct();
            object valueObj1;
            Result result1 = UIXLoadResult.ValidateStringAsValue(splitString[0], SingleSchema.Type, null, out valueObj1);
            if (result1.Failed)
                return Result.Fail("Problem converting '{0}' ({1})", "Vector3", result1.Error);
            SetX(ref instance, valueObj1);
            object valueObj2;
            Result result2 = UIXLoadResult.ValidateStringAsValue(splitString[1], SingleSchema.Type, null, out valueObj2);
            if (result2.Failed)
                return Result.Fail("Problem converting '{0}' ({1})", "Vector3", result2.Error);
            SetY(ref instance, valueObj2);
            object valueObj3;
            Result result3 = UIXLoadResult.ValidateStringAsValue(splitString[2], SingleSchema.Type, null, out valueObj3);
            if (result3.Failed)
                return Result.Fail("Problem converting '{0}' ({1})", "Vector3", result3.Error);
            SetZ(ref instance, valueObj3);
            return result3;
        }

        private static void EncodeBinary(ByteCodeWriter writer, object instanceObj)
        {
            Vector3 vector3 = (Vector3)instanceObj;
            writer.WriteSingle(vector3.X);
            writer.WriteSingle(vector3.Y);
            writer.WriteSingle(vector3.Z);
        }

        private static object DecodeBinary(ByteCodeReader reader) => new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

        private static bool IsConversionSupported(TypeSchema fromType) => StringSchema.Type.IsAssignableFrom(fromType);

        private static Result TryConvertFrom(
          object from,
          TypeSchema fromType,
          out object instance)
        {
            Result result = Result.Fail("Unsupported");
            instance = null;
            if (StringSchema.Type.IsAssignableFrom(fromType))
            {
                string[] splitString = StringUtility.SplitAndTrim(',', (string)from);
                if (splitString.Length == 3)
                {
                    result = ConvertFromStringXYZ(splitString, out instance);
                    if (!result.Failed)
                        return result;
                }
                else
                    result = Result.Fail("Unable to convert \"{0}\" to type '{1}'", from.ToString(), "Vector3");
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
            Vector3 vector3_1 = (Vector3)leftObj;
            if (op == OperationType.MathNegate)
                return -vector3_1;
            Vector3 vector3_2 = (Vector3)rightObj;
            switch (op - 1)
            {
                case 0:
                    return vector3_1 + vector3_2;
                case OperationType.MathAdd:
                    return vector3_1 - vector3_2;
                case OperationType.MathSubtract:
                    return vector3_1 * vector3_2;
                case OperationType.MathMultiply:
                    return vector3_1 / vector3_2;
                case OperationType.LogicalOr:
                    return BooleanBoxes.Box(vector3_1 == vector3_2);
                case OperationType.RelationalEquals:
                    return BooleanBoxes.Box(vector3_1 != vector3_2);
                default:
                    return null;
            }
        }

        public static void Pass1Initialize() => Type = new UIXTypeSchema(234, "Vector3", null, 153, typeof(Vector3), UIXTypeFlags.Immutable);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(234, "X", 194, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetX), new SetValueHandler(SetX), false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(234, "Y", 194, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetY), new SetValueHandler(SetY), false);
            UIXPropertySchema uixPropertySchema3 = new UIXPropertySchema(234, "Z", 194, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetZ), new SetValueHandler(SetZ), false);
            UIXConstructorSchema constructorSchema = new UIXConstructorSchema(234, new short[3]
            {
         194,
         194,
         194
            }, new ConstructHandler(ConstructXYZ));
            Type.Initialize(new DefaultConstructHandler(Construct), new ConstructorSchema[1]
            {
         constructorSchema
            }, new PropertySchema[3]
            {
         uixPropertySchema1,
         uixPropertySchema2,
         uixPropertySchema3
            }, null, null, null, new TypeConverterHandler(TryConvertFrom), new SupportsTypeConversionHandler(IsConversionSupported), new EncodeBinaryHandler(EncodeBinary), new DecodeBinaryHandler(DecodeBinary), new PerformOperationHandler(ExecuteOperation), new SupportsOperationHandler(IsOperationSupported));
        }
    }
}
