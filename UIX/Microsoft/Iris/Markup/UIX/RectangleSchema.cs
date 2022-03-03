// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.RectangleSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;
using Microsoft.Iris.Render;
using System;
using System.Globalization;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class RectangleSchema
    {
        public static UIXTypeSchema Type;

        private static object GetX(object instanceObj) => ((Rectangle)instanceObj).X;

        private static void SetX(ref object instanceObj, object valueObj)
        {
            Rectangle rectangle = (Rectangle)instanceObj;
            int num = (int)valueObj;
            rectangle.X = num;
            instanceObj = rectangle;
        }

        private static object GetY(object instanceObj) => ((Rectangle)instanceObj).Y;

        private static void SetY(ref object instanceObj, object valueObj)
        {
            Rectangle rectangle = (Rectangle)instanceObj;
            int num = (int)valueObj;
            rectangle.Y = num;
            instanceObj = rectangle;
        }

        private static object GetWidth(object instanceObj) => ((Rectangle)instanceObj).Width;

        private static void SetWidth(ref object instanceObj, object valueObj)
        {
            Rectangle rectangle = (Rectangle)instanceObj;
            int num = (int)valueObj;
            rectangle.Width = num;
            instanceObj = rectangle;
        }

        private static object GetHeight(object instanceObj) => ((Rectangle)instanceObj).Height;

        private static void SetHeight(ref object instanceObj, object valueObj)
        {
            Rectangle rectangle = (Rectangle)instanceObj;
            int num = (int)valueObj;
            rectangle.Height = num;
            instanceObj = rectangle;
        }

        private static object GetLeft(object instanceObj) => ((Rectangle)instanceObj).Left;

        private static object GetTop(object instanceObj) => ((Rectangle)instanceObj).Top;

        private static object GetRight(object instanceObj) => ((Rectangle)instanceObj).Right;

        private static object GetBottom(object instanceObj) => ((Rectangle)instanceObj).Bottom;

        private static object Construct() => Rectangle.Zero;

        private static Result ConvertFromString(object valueObj, out object instanceObj)
        {
            string s = (string)valueObj;
            instanceObj = null;
            int result;
            if (!int.TryParse(s, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out result))
                return Result.Fail("Unable to convert \"{0}\" to type '{1}'", s, "Int32");
            Rectangle rectangle1 = new Rectangle();
            Rectangle rectangle2 = Rectangle.FromLTRB(result, result, result, result);
            instanceObj = rectangle2;
            return Result.Success;
        }

        private static Result ConvertFromInt32(object valueObj, out object instanceObj)
        {
            int num1 = (int)valueObj;
            instanceObj = null;
            int num2 = num1;
            Rectangle rectangle = Rectangle.FromLTRB(num2, num2, num2, num2);
            instanceObj = rectangle;
            return Result.Success;
        }

        private static Result ConvertFromSingle(object valueObj, out object instanceObj)
        {
            float num1 = (float)valueObj;
            instanceObj = null;
            int num2 = (int)num1;
            Rectangle rectangle = Rectangle.FromLTRB(num2, num2, num2, num2);
            instanceObj = rectangle;
            return Result.Success;
        }

        private static void EncodeBinary(ByteCodeWriter writer, object instanceObj)
        {
            Rectangle rectangle = (Rectangle)instanceObj;
            writer.WriteInt32(rectangle.Left);
            writer.WriteInt32(rectangle.Top);
            writer.WriteInt32(rectangle.Right);
            writer.WriteInt32(rectangle.Bottom);
        }

        private static object DecodeBinary(ByteCodeReader reader) => Rectangle.FromLTRB(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());

        private static object CallContainsPoint(object instanceObj, object[] parameters) => BooleanBoxes.Box(((Rectangle)instanceObj).Contains((Point)parameters[0]));

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
            return result;
        }

        private static bool IsOperationSupported(OperationType op)
        {
            switch (op)
            {
                case OperationType.RelationalEquals:
                case OperationType.RelationalNotEquals:
                    return true;
                default:
                    return false;
            }
        }

        private static object ExecuteOperation(object leftObj, object rightObj, OperationType op)
        {
            Rectangle rectangle1 = (Rectangle)leftObj;
            Rectangle rectangle2 = (Rectangle)rightObj;
            switch (op)
            {
                case OperationType.RelationalEquals:
                    return BooleanBoxes.Box(rectangle1 == rectangle2);
                case OperationType.RelationalNotEquals:
                    return BooleanBoxes.Box(rectangle1 != rectangle2);
                default:
                    return null;
            }
        }

        private static object CallTryParseStringRectangle(object instanceObj, object[] parameters)
        {
            string parameter1 = (string)parameters[0];
            Rectangle parameter2 = (Rectangle)parameters[1];
            object instanceObj1;
            return ConvertFromString(parameter1, out instanceObj1).Failed ? parameter2 : instanceObj1;
        }

        public static void Pass1Initialize() => Type = new UIXTypeSchema(169, "Rectangle", null, 153, typeof(Rectangle), UIXTypeFlags.Immutable);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(169, "X", 115, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetX), new SetValueHandler(SetX), false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(169, "Y", 115, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetY), new SetValueHandler(SetY), false);
            UIXPropertySchema uixPropertySchema3 = new UIXPropertySchema(169, "Width", 115, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetWidth), new SetValueHandler(SetWidth), false);
            UIXPropertySchema uixPropertySchema4 = new UIXPropertySchema(169, "Height", 115, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetHeight), new SetValueHandler(SetHeight), false);
            UIXPropertySchema uixPropertySchema5 = new UIXPropertySchema(169, "Left", 115, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetLeft), null, false);
            UIXPropertySchema uixPropertySchema6 = new UIXPropertySchema(169, "Top", 115, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetTop), null, false);
            UIXPropertySchema uixPropertySchema7 = new UIXPropertySchema(169, "Right", 115, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetRight), null, false);
            UIXPropertySchema uixPropertySchema8 = new UIXPropertySchema(169, "Bottom", 115, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetBottom), null, false);
            UIXMethodSchema uixMethodSchema1 = new UIXMethodSchema(169, "Contains", new short[1]
            {
         158
            }, 15, new InvokeHandler(CallContainsPoint), false);
            UIXMethodSchema uixMethodSchema2 = new UIXMethodSchema(169, "TryParse", new short[2]
            {
         208,
         169
            }, 169, new InvokeHandler(CallTryParseStringRectangle), true);
            Type.Initialize(new DefaultConstructHandler(Construct), null, new PropertySchema[8]
            {
         uixPropertySchema8,
         uixPropertySchema4,
         uixPropertySchema5,
         uixPropertySchema7,
         uixPropertySchema6,
         uixPropertySchema3,
         uixPropertySchema1,
         uixPropertySchema2
            }, new MethodSchema[2]
            {
         uixMethodSchema1,
         uixMethodSchema2
            }, null, null, new TypeConverterHandler(TryConvertFrom), new SupportsTypeConversionHandler(IsConversionSupported), new EncodeBinaryHandler(EncodeBinary), new DecodeBinaryHandler(DecodeBinary), new PerformOperationHandler(ExecuteOperation), new SupportsOperationHandler(IsOperationSupported));
        }
    }
}
