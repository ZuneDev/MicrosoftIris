// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.ItemAlignmentSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Layout;
using Microsoft.Iris.Library;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class ItemAlignmentSchema
    {
        public static UIXTypeSchema Type;

        private static object GetHorizontal(object instanceObj) => ((ItemAlignment)instanceObj).Horizontal;

        private static void SetHorizontal(ref object instanceObj, object valueObj)
        {
            ItemAlignment itemAlignment = (ItemAlignment)instanceObj;
            Alignment alignment = (Alignment)valueObj;
            itemAlignment.Horizontal = alignment;
            instanceObj = itemAlignment;
        }

        private static object GetVertical(object instanceObj) => ((ItemAlignment)instanceObj).Vertical;

        private static void SetVertical(ref object instanceObj, object valueObj)
        {
            ItemAlignment itemAlignment = (ItemAlignment)instanceObj;
            Alignment alignment = (Alignment)valueObj;
            itemAlignment.Vertical = alignment;
            instanceObj = itemAlignment;
        }

        private static object Construct() => ItemAlignment.Default;

        private static object ConstructAlignment(object[] parameters)
        {
            Alignment parameter = (Alignment)parameters[0];
            return new ItemAlignment(parameter, parameter);
        }

        private static object ConstructAlignmentAlignment(object[] parameters) => new ItemAlignment((Alignment)parameters[0], (Alignment)parameters[1]);

        private static void EncodeBinary(ByteCodeWriter writer, object instanceObj)
        {
            ItemAlignment itemAlignment = (ItemAlignment)instanceObj;
            writer.WriteByte((byte)itemAlignment.Horizontal);
            writer.WriteByte((byte)itemAlignment.Vertical);
        }

        private static object DecodeBinary(ByteCodeReader reader) => new ItemAlignment((Alignment)reader.ReadByte(), (Alignment)reader.ReadByte());

        private static Result ConvertFromString(object valueObj, out object instanceObj)
        {
            string str = (string)valueObj;
            instanceObj = null;
            Alignment alignment1;
            Alignment alignment2;
            if (str.IndexOf(',') >= 0)
            {
                string[] strArray = str.Split(',');
                if (strArray.Length != 2)
                    return Result.Fail("Unable to convert \"{0}\" to type '{1}'", str, "ItemAlignment");
                Result alignment3 = ParseAlignment(strArray[0], out alignment1);
                if (alignment3.Failed)
                    return alignment3;
                alignment3 = ParseAlignment(strArray[1], out alignment2);
                if (alignment3.Failed)
                    return alignment3;
            }
            else
            {
                Result alignment3 = ParseAlignment(str, out alignment1);
                if (alignment3.Failed)
                    return alignment3;
                alignment2 = alignment1;
            }
            ItemAlignment itemAlignment = new ItemAlignment(alignment1, alignment2);
            instanceObj = itemAlignment;
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

        private static object CallTryParseStringItemAlignment(object instanceObj, object[] parameters)
        {
            string parameter1 = (string)parameters[0];
            ItemAlignment parameter2 = (ItemAlignment)parameters[1];
            object instanceObj1;
            return ConvertFromString(parameter1, out instanceObj1).Failed ? parameter2 : instanceObj1;
        }

        private static Result ParseAlignment(string value, out Alignment alignment)
        {
            value = value.Trim();
            alignment = Alignment.Unspecified;
            if (value != "-")
            {
                object instance;
                Result result = UIXLoadResultExports.AlignmentType.TypeConverter(value, StringSchema.Type, out instance);
                if (result.Failed)
                    return Result.Fail("Problem converting '{0}' ({1})", "ItemAlignment", result.Error);
                alignment = (Alignment)instance;
            }
            return Result.Success;
        }

        public static void Pass1Initialize() => Type = new UIXTypeSchema(sbyte.MaxValue, "ItemAlignment", null, 153, typeof(ItemAlignment), UIXTypeFlags.Immutable);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(sbyte.MaxValue, "Horizontal", 3, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetHorizontal), new SetValueHandler(SetHorizontal), false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(sbyte.MaxValue, "Vertical", 3, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetVertical), new SetValueHandler(SetVertical), false);
            UIXConstructorSchema constructorSchema1 = new UIXConstructorSchema(sbyte.MaxValue, new short[1]
            {
         3
            }, new ConstructHandler(ConstructAlignment));
            UIXConstructorSchema constructorSchema2 = new UIXConstructorSchema(sbyte.MaxValue, new short[2]
            {
         3,
         3
            }, new ConstructHandler(ConstructAlignmentAlignment));
            UIXMethodSchema uixMethodSchema = new UIXMethodSchema(sbyte.MaxValue, "TryParse", new short[2]
            {
         208,
         sbyte.MaxValue
            }, sbyte.MaxValue, new InvokeHandler(CallTryParseStringItemAlignment), true);
            Type.Initialize(new DefaultConstructHandler(Construct), new ConstructorSchema[2]
            {
         constructorSchema1,
         constructorSchema2
            }, new PropertySchema[2]
            {
         uixPropertySchema1,
         uixPropertySchema2
            }, new MethodSchema[1]
            {
         uixMethodSchema
            }, null, null, new TypeConverterHandler(TryConvertFrom), new SupportsTypeConversionHandler(IsConversionSupported), new EncodeBinaryHandler(EncodeBinary), new DecodeBinaryHandler(DecodeBinary), null, null);
        }
    }
}
