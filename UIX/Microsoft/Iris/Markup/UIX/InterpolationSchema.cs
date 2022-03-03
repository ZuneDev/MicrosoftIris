// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.InterpolationSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Animations;
using Microsoft.Iris.Library;
using Microsoft.Iris.Session;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class InterpolationSchema
    {
        public static RangeValidator ValidateEasePercent = new RangeValidator(RangeValidateEasePercent);
        public static UIXTypeSchema Type;

        private static object GetType(object instanceObj) => ((Interpolation)instanceObj).Type;

        private static void SetType(ref object instanceObj, object valueObj) => ((Interpolation)instanceObj).Type = (InterpolationType)valueObj;

        private static object GetWeight(object instanceObj) => ((Interpolation)instanceObj).Weight;

        private static void SetWeight(ref object instanceObj, object valueObj)
        {
            Interpolation interpolation = (Interpolation)instanceObj;
            float num = (float)valueObj;
            Result result = SingleSchema.ValidateNotNegative(valueObj);
            if (result.Failed)
                ErrorManager.ReportError(result.Error);
            else
                interpolation.Weight = num;
        }

        private static object GetBezierHandle1(object instanceObj) => ((Interpolation)instanceObj).BezierHandle1;

        private static void SetBezierHandle1(ref object instanceObj, object valueObj) => ((Interpolation)instanceObj).BezierHandle1 = (float)valueObj;

        private static object GetBezierHandle2(object instanceObj) => ((Interpolation)instanceObj).BezierHandle2;

        private static void SetBezierHandle2(ref object instanceObj, object valueObj) => ((Interpolation)instanceObj).BezierHandle2 = (float)valueObj;

        private static object GetEasePercent(object instanceObj) => ((Interpolation)instanceObj).EasePercent;

        private static void SetEasePercent(ref object instanceObj, object valueObj)
        {
            Interpolation interpolation = (Interpolation)instanceObj;
            float num = (float)valueObj;
            Result result = ValidateEasePercent(valueObj);
            if (result.Failed)
                ErrorManager.ReportError(result.Error);
            else
                interpolation.EasePercent = num;
        }

        private static object Construct() => new Interpolation();

        private static void EncodeBinary(ByteCodeWriter writer, object instanceObj)
        {
            Interpolation interpolation = (Interpolation)instanceObj;
            writer.WriteInt32((int)interpolation.Type);
            writer.WriteSingle(interpolation.Weight);
            writer.WriteSingle(interpolation.BezierHandle1);
            writer.WriteSingle(interpolation.BezierHandle2);
            writer.WriteSingle(interpolation.EasePercent);
        }

        private static object DecodeBinary(ByteCodeReader reader) => new Interpolation()
        {
            Type = (InterpolationType)reader.ReadInt32(),
            Weight = reader.ReadSingle(),
            BezierHandle1 = reader.ReadSingle(),
            BezierHandle2 = reader.ReadSingle(),
            EasePercent = reader.ReadSingle()
        };

        private static Result ConvertFromString(object valueObj, out object instanceObj)
        {
            string str = (string)valueObj;
            instanceObj = null;
            string[] strArray = str.Split(',');
            if (strArray.Length < 1)
                return Result.Fail("Unable to convert \"{0}\" to type '{1}'", str, "Interpolation");
            Interpolation interpolation = new Interpolation();
            instanceObj = interpolation;
            object valueObj1;
            Result result = UIXLoadResult.ValidateStringAsValue(strArray[0], UIXLoadResultExports.InterpolationTypeType, null, out valueObj1);
            if (result.Failed)
                return Result.Fail("Problem converting '{0}' ({1})", "Interpolation", result.Error);
            SetType(ref instanceObj, valueObj1);
            if (strArray.Length == 2)
            {
                object valueObj2;
                result = UIXLoadResult.ValidateStringAsValue(strArray[1], SingleSchema.Type, SingleSchema.ValidateNotNegative, out valueObj2);
                if (result.Failed)
                    return Result.Fail("Problem converting '{0}' ({1})", "Interpolation", result.Error);
                SetWeight(ref instanceObj, valueObj2);
            }
            else if (strArray.Length == 3)
            {
                if (interpolation.Type == InterpolationType.Bezier)
                {
                    object valueObj2;
                    result = UIXLoadResult.ValidateStringAsValue(strArray[1], SingleSchema.Type, null, out valueObj2);
                    if (result.Failed)
                        return Result.Fail("Problem converting '{0}' ({1})", "Interpolation", result.Error);
                    SetBezierHandle1(ref instanceObj, valueObj2);
                    object valueObj3;
                    result = UIXLoadResult.ValidateStringAsValue(strArray[2], SingleSchema.Type, null, out valueObj3);
                    if (result.Failed)
                        return Result.Fail("Problem converting '{0}' ({1})", "Interpolation", result.Error);
                    SetBezierHandle2(ref instanceObj, valueObj3);
                }
                else
                {
                    object valueObj2;
                    result = UIXLoadResult.ValidateStringAsValue(strArray[1], SingleSchema.Type, SingleSchema.ValidateNotNegative, out valueObj2);
                    if (result.Failed)
                        return Result.Fail("Problem converting '{0}' ({1})", "Interpolation", result.Error);
                    SetWeight(ref instanceObj, valueObj2);
                    object valueObj3;
                    result = UIXLoadResult.ValidateStringAsValue(strArray[2], SingleSchema.Type, ValidateEasePercent, out valueObj3);
                    if (result.Failed)
                        return Result.Fail("Problem converting '{0}' ({1})", "Interpolation", result.Error);
                    SetEasePercent(ref instanceObj, valueObj3);
                }
            }
            else if (strArray.Length >= 4)
                return Result.Fail("Unable to convert \"{0}\" to type '{1}'", str, "Interpolation");
            instanceObj = interpolation;
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

        private static object CallTryParseStringInterpolation(object instanceObj, object[] parameters)
        {
            string parameter1 = (string)parameters[0];
            Interpolation parameter2 = (Interpolation)parameters[1];
            object instanceObj1;
            return ConvertFromString(parameter1, out instanceObj1).Failed ? parameter2 : instanceObj1;
        }

        private static Result RangeValidateEasePercent(object value)
        {
            float num = (float)value;
            return num <= 0.0 || num >= 1.0 ? Result.Fail("Expecting a value between {0} and {1} (exclusive), but got {2}", "0.0", "1.0", num.ToString()) : Result.Success;
        }

        public static void Pass1Initialize() => Type = new UIXTypeSchema(121, "Interpolation", null, 153, typeof(Interpolation), UIXTypeFlags.Immutable);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(121, "Type", 122, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetType), new SetValueHandler(SetType), false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(121, "Weight", 194, -1, ExpressionRestriction.None, false, SingleSchema.ValidateNotNegative, false, new GetValueHandler(GetWeight), new SetValueHandler(SetWeight), false);
            UIXPropertySchema uixPropertySchema3 = new UIXPropertySchema(121, "BezierHandle1", 194, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetBezierHandle1), new SetValueHandler(SetBezierHandle1), false);
            UIXPropertySchema uixPropertySchema4 = new UIXPropertySchema(121, "BezierHandle2", 194, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetBezierHandle2), new SetValueHandler(SetBezierHandle2), false);
            UIXPropertySchema uixPropertySchema5 = new UIXPropertySchema(121, "EasePercent", 194, -1, ExpressionRestriction.None, false, ValidateEasePercent, false, new GetValueHandler(GetEasePercent), new SetValueHandler(SetEasePercent), false);
            UIXMethodSchema uixMethodSchema = new UIXMethodSchema(121, "TryParse", new short[2]
            {
         208,
         121
            }, 121, new InvokeHandler(CallTryParseStringInterpolation), true);
            Type.Initialize(new DefaultConstructHandler(Construct), null, new PropertySchema[5]
            {
         uixPropertySchema3,
         uixPropertySchema4,
         uixPropertySchema5,
         uixPropertySchema1,
         uixPropertySchema2
            }, new MethodSchema[1]
            {
         uixMethodSchema
            }, null, null, new TypeConverterHandler(TryConvertFrom), new SupportsTypeConversionHandler(IsConversionSupported), new EncodeBinaryHandler(EncodeBinary), new DecodeBinaryHandler(DecodeBinary), null, null);
        }
    }
}
