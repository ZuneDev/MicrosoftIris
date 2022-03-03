// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.PointSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Data;
using Microsoft.Iris.Library;
using Microsoft.Iris.Render;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class PointSchema
    {
        public static UIXTypeSchema Type;

        private static object GetX(object instanceObj) => ((Point)instanceObj).X;

        private static void SetX(ref object instanceObj, object valueObj)
        {
            Point point = (Point)instanceObj;
            int num = (int)valueObj;
            point.X = num;
            instanceObj = point;
        }

        private static object GetY(object instanceObj) => ((Point)instanceObj).Y;

        private static void SetY(ref object instanceObj, object valueObj)
        {
            Point point = (Point)instanceObj;
            int num = (int)valueObj;
            point.Y = num;
            instanceObj = point;
        }

        private static object Construct() => Point.Zero;

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
            Result result1 = UIXLoadResult.ValidateStringAsValue(splitString[0], Int32Schema.Type, null, out valueObj1);
            if (result1.Failed)
                return Result.Fail("Problem converting '{0}' ({1})", "Point", result1.Error);
            SetX(ref instance, valueObj1);
            object valueObj2;
            Result result2 = UIXLoadResult.ValidateStringAsValue(splitString[1], Int32Schema.Type, null, out valueObj2);
            if (result2.Failed)
                return Result.Fail("Problem converting '{0}' ({1})", "Point", result2.Error);
            SetY(ref instance, valueObj2);
            return result2;
        }

        private static void EncodeBinary(ByteCodeWriter writer, object instanceObj)
        {
            Point point = (Point)instanceObj;
            writer.WriteInt32(point.X);
            writer.WriteInt32(point.Y);
        }

        private static object DecodeBinary(ByteCodeReader reader) => new Point(reader.ReadInt32(), reader.ReadInt32());

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
                if (splitString.Length == 2)
                {
                    result = ConvertFromStringXY(splitString, out instance);
                    if (!result.Failed)
                        return result;
                }
                else
                    result = Result.Fail("Unable to convert \"{0}\" to type '{1}'", from.ToString(), "Point");
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
            Point point1 = (Point)leftObj;
            if (op == OperationType.MathNegate)
                return -point1;
            Point point2 = (Point)rightObj;
            switch (op)
            {
                case OperationType.MathAdd:
                    return point1 + point2;
                case OperationType.MathSubtract:
                    return point1 - point2;
                case OperationType.RelationalEquals:
                    return BooleanBoxes.Box(point1 == point2);
                case OperationType.RelationalNotEquals:
                    return BooleanBoxes.Box(point1 != point2);
                default:
                    return null;
            }
        }

        public static void Pass1Initialize() => Type = new UIXTypeSchema(158, "Point", null, 153, typeof(Point), UIXTypeFlags.Immutable);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(158, "X", 115, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetX), new SetValueHandler(SetX), false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(158, "Y", 115, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetY), new SetValueHandler(SetY), false);
            UIXConstructorSchema constructorSchema = new UIXConstructorSchema(158, new short[2]
            {
         115,
         115
            }, new ConstructHandler(ConstructXY));
            Type.Initialize(new DefaultConstructHandler(Construct), new ConstructorSchema[1]
            {
         constructorSchema
            }, new PropertySchema[2]
            {
         uixPropertySchema1,
         uixPropertySchema2
            }, null, null, null, new TypeConverterHandler(TryConvertFrom), new SupportsTypeConversionHandler(IsConversionSupported), new EncodeBinaryHandler(EncodeBinary), new DecodeBinaryHandler(DecodeBinary), new PerformOperationHandler(ExecuteOperation), new SupportsOperationHandler(IsOperationSupported));
        }
    }
}
