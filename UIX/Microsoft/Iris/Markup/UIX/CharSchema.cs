// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.CharSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class CharSchema
    {
        public static UIXTypeSchema Type;

        private static object Construct() => char.MinValue;

        private static void EncodeBinary(ByteCodeWriter writer, object instanceObj)
        {
            char ch = (char)instanceObj;
            writer.WriteChar(ch);
        }

        private static object DecodeBinary(ByteCodeReader reader) => reader.ReadChar();

        private static Result ConvertFromString(object valueObj, out object instanceObj)
        {
            string str = (string)valueObj;
            instanceObj = null;
            if (str == null || str.Length != 1)
                return Result.Fail("Unable to convert \"{0}\" to type '{1}'", str, "Char");
            char ch = str[0];
            instanceObj = ch;
            return Result.Success;
        }

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
            char ch1 = (char)leftObj;
            char ch2 = (char)rightObj;
            switch (op)
            {
                case OperationType.RelationalEquals:
                    return BooleanBoxes.Box(ch1 == ch2);
                case OperationType.RelationalNotEquals:
                    return BooleanBoxes.Box(ch1 != ch2);
                default:
                    return null;
            }
        }

        private static object CallTryParseStringChar(object instanceObj, object[] parameters)
        {
            string parameter1 = (string)parameters[0];
            char parameter2 = (char)parameters[1];
            object instanceObj1;
            return ConvertFromString(parameter1, out instanceObj1).Failed ? parameter2 : instanceObj1;
        }

        public static void Pass1Initialize() => Type = new UIXTypeSchema(27, "Char", "char", 153, typeof(char), UIXTypeFlags.Immutable);

        public static void Pass2Initialize()
        {
            UIXMethodSchema uixMethodSchema = new UIXMethodSchema(27, "TryParse", new short[2]
            {
         208,
         27
            }, 27, new InvokeHandler(CallTryParseStringChar), true);
            Type.Initialize(new DefaultConstructHandler(Construct), null, null, new MethodSchema[1]
            {
         uixMethodSchema
            }, null, null, new TypeConverterHandler(TryConvertFrom), new SupportsTypeConversionHandler(IsConversionSupported), new EncodeBinaryHandler(EncodeBinary), new DecodeBinaryHandler(DecodeBinary), new PerformOperationHandler(ExecuteOperation), new SupportsOperationHandler(IsOperationSupported));
        }
    }
}
