// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.SizeSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Data;
using Microsoft.Iris.Library;
using Microsoft.Iris.Render;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class SizeSchema
    {
        public static RangeValidator ValidateNotNegative = new RangeValidator(RangeValidateNotNegative);
        public static UIXTypeSchema Type;

        private static object GetWidth(object instanceObj) => ((Size)instanceObj).Width;

        private static void SetWidth(ref object instanceObj, object valueObj)
        {
            Size size = (Size)instanceObj;
            int num = (int)valueObj;
            size.Width = num;
            instanceObj = size;
        }

        private static object GetHeight(object instanceObj) => ((Size)instanceObj).Height;

        private static void SetHeight(ref object instanceObj, object valueObj)
        {
            Size size = (Size)instanceObj;
            int num = (int)valueObj;
            size.Height = num;
            instanceObj = size;
        }

        private static object Construct() => Size.Zero;

        private static object ConstructWidthHeight(object[] parameters)
        {
            object instanceObj = Construct();
            SetWidth(ref instanceObj, parameters[0]);
            SetHeight(ref instanceObj, parameters[1]);
            return instanceObj;
        }

        private static Result ConvertFromStringWidthHeight(
          string[] splitString,
          out object instance)
        {
            instance = Construct();
            object valueObj1;
            Result result1 = UIXLoadResult.ValidateStringAsValue(splitString[0], Int32Schema.Type, null, out valueObj1);
            if (result1.Failed)
                return Result.Fail("Problem converting '{0}' ({1})", "Size", result1.Error);
            SetWidth(ref instance, valueObj1);
            object valueObj2;
            Result result2 = UIXLoadResult.ValidateStringAsValue(splitString[1], Int32Schema.Type, null, out valueObj2);
            if (result2.Failed)
                return Result.Fail("Problem converting '{0}' ({1})", "Size", result2.Error);
            SetHeight(ref instance, valueObj2);
            return result2;
        }

        private static void EncodeBinary(ByteCodeWriter writer, object instanceObj)
        {
            Size size = (Size)instanceObj;
            writer.WriteInt32(size.Width);
            writer.WriteInt32(size.Height);
        }

        private static object DecodeBinary(ByteCodeReader reader) => new Size(reader.ReadInt32(), reader.ReadInt32());

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
                    result = ConvertFromStringWidthHeight(splitString, out instance);
                    if (!result.Failed)
                        return result;
                }
                else
                    result = Result.Fail("Unable to convert \"{0}\" to type '{1}'", from.ToString(), "Size");
            }
            return result;
        }

        private static Result RangeValidateNotNegative(object value)
        {
            Size size = (Size)value;
            return size.Width < 0 || size.Height < 0 ? Result.Fail("Expecting a non-negative value, but got {0}", size.ToString()) : Result.Success;
        }

        public static void Pass1Initialize() => Type = new UIXTypeSchema(195, "Size", null, 153, typeof(Size), UIXTypeFlags.Immutable);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(195, "Width", 115, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetWidth), new SetValueHandler(SetWidth), false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(195, "Height", 115, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetHeight), new SetValueHandler(SetHeight), false);
            UIXConstructorSchema constructorSchema = new UIXConstructorSchema(195, new short[2]
            {
         115,
         115
            }, new ConstructHandler(ConstructWidthHeight));
            Type.Initialize(new DefaultConstructHandler(Construct), new ConstructorSchema[1]
            {
         constructorSchema
            }, new PropertySchema[2]
            {
         uixPropertySchema2,
         uixPropertySchema1
            }, null, null, null, new TypeConverterHandler(TryConvertFrom), new SupportsTypeConversionHandler(IsConversionSupported), new EncodeBinaryHandler(EncodeBinary), new DecodeBinaryHandler(DecodeBinary), null, null);
        }
    }
}
