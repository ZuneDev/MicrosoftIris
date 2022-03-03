// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.MajorMinorSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Data;
using Microsoft.Iris.Layouts;
using Microsoft.Iris.Library;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class MajorMinorSchema
    {
        public static UIXTypeSchema Type;

        private static object GetMajor(object instanceObj) => ((MajorMinor)instanceObj).Major;

        private static void SetMajor(ref object instanceObj, object valueObj)
        {
            MajorMinor majorMinor = (MajorMinor)instanceObj;
            int num = (int)valueObj;
            majorMinor.Major = num;
            instanceObj = majorMinor;
        }

        private static object GetMinor(object instanceObj) => ((MajorMinor)instanceObj).Minor;

        private static void SetMinor(ref object instanceObj, object valueObj)
        {
            MajorMinor majorMinor = (MajorMinor)instanceObj;
            int num = (int)valueObj;
            majorMinor.Minor = num;
            instanceObj = majorMinor;
        }

        private static object Construct() => MajorMinor.Zero;

        private static object ConstructMajorMinor(object[] parameters)
        {
            object instanceObj = Construct();
            SetMajor(ref instanceObj, parameters[0]);
            SetMinor(ref instanceObj, parameters[1]);
            return instanceObj;
        }

        private static Result ConvertFromStringMajorMinor(
          string[] splitString,
          out object instance)
        {
            instance = Construct();
            object valueObj1;
            Result result1 = UIXLoadResult.ValidateStringAsValue(splitString[0], Int32Schema.Type, null, out valueObj1);
            if (result1.Failed)
                return Result.Fail("Problem converting '{0}' ({1})", "MajorMinor", result1.Error);
            SetMajor(ref instance, valueObj1);
            object valueObj2;
            Result result2 = UIXLoadResult.ValidateStringAsValue(splitString[1], Int32Schema.Type, null, out valueObj2);
            if (result2.Failed)
                return Result.Fail("Problem converting '{0}' ({1})", "MajorMinor", result2.Error);
            SetMinor(ref instance, valueObj2);
            return result2;
        }

        private static void EncodeBinary(ByteCodeWriter writer, object instanceObj)
        {
            MajorMinor majorMinor = (MajorMinor)instanceObj;
            writer.WriteInt32(majorMinor.Major);
            writer.WriteInt32(majorMinor.Minor);
        }

        private static object DecodeBinary(ByteCodeReader reader) => new MajorMinor(reader.ReadInt32(), reader.ReadInt32());

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
                    result = ConvertFromStringMajorMinor(splitString, out instance);
                    if (!result.Failed)
                        return result;
                }
                else
                    result = Result.Fail("Unable to convert \"{0}\" to type '{1}'", from.ToString(), "MajorMinor");
            }
            return result;
        }

        public static void Pass1Initialize() => Type = new UIXTypeSchema(139, "MajorMinor", null, 153, typeof(MajorMinor), UIXTypeFlags.Immutable);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(139, "Major", 115, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetMajor), new SetValueHandler(SetMajor), false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(139, "Minor", 115, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetMinor), new SetValueHandler(SetMinor), false);
            UIXConstructorSchema constructorSchema = new UIXConstructorSchema(139, new short[2]
            {
         115,
         115
            }, new ConstructHandler(ConstructMajorMinor));
            Type.Initialize(new DefaultConstructHandler(Construct), new ConstructorSchema[1]
            {
         constructorSchema
            }, new PropertySchema[2]
            {
         uixPropertySchema1,
         uixPropertySchema2
            }, null, null, null, new TypeConverterHandler(TryConvertFrom), new SupportsTypeConversionHandler(IsConversionSupported), new EncodeBinaryHandler(EncodeBinary), new DecodeBinaryHandler(DecodeBinary), null, null);
        }
    }
}
