// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.AnchorEdgeSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Data;
using Microsoft.Iris.Layouts;
using Microsoft.Iris.Library;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class AnchorEdgeSchema
    {
        public static UIXTypeSchema Type;

        private static object GetId(object instanceObj) => ((AnchorEdge)instanceObj).Id;

        private static void SetId(ref object instanceObj, object valueObj) => ((AnchorEdge)instanceObj).Id = (string)valueObj;

        private static object GetPercent(object instanceObj) => ((AnchorEdge)instanceObj).Percent;

        private static void SetPercent(ref object instanceObj, object valueObj) => ((AnchorEdge)instanceObj).Percent = (float)valueObj;

        private static object GetOffset(object instanceObj) => ((AnchorEdge)instanceObj).Offset;

        private static void SetOffset(ref object instanceObj, object valueObj) => ((AnchorEdge)instanceObj).Offset = (int)valueObj;

        private static object GetMaximumPercent(object instanceObj) => ((AnchorEdge)instanceObj).MaximumPercent;

        private static void SetMaximumPercent(ref object instanceObj, object valueObj) => ((AnchorEdge)instanceObj).MaximumPercent = (float)valueObj;

        private static object GetMaximumOffset(object instanceObj) => ((AnchorEdge)instanceObj).MaximumOffset;

        private static void SetMaximumOffset(ref object instanceObj, object valueObj) => ((AnchorEdge)instanceObj).MaximumOffset = (int)valueObj;

        private static object GetMinimumPercent(object instanceObj) => ((AnchorEdge)instanceObj).MinimumPercent;

        private static void SetMinimumPercent(ref object instanceObj, object valueObj) => ((AnchorEdge)instanceObj).MinimumPercent = (float)valueObj;

        private static object GetMinimumOffset(object instanceObj) => ((AnchorEdge)instanceObj).MinimumOffset;

        private static void SetMinimumOffset(ref object instanceObj, object valueObj) => ((AnchorEdge)instanceObj).MinimumOffset = (int)valueObj;

        private static object Construct() => new AnchorEdge();

        private static object ConstructIdPercent(object[] parameters)
        {
            object instanceObj = Construct();
            SetId(ref instanceObj, parameters[0]);
            SetPercent(ref instanceObj, parameters[1]);
            return instanceObj;
        }

        private static Result ConvertFromStringIdPercent(
          string[] splitString,
          out object instance)
        {
            instance = Construct();
            object valueObj1;
            Result result1 = UIXLoadResult.ValidateStringAsValue(splitString[0], StringSchema.Type, null, out valueObj1);
            if (result1.Failed)
                return Result.Fail("Problem converting '{0}' ({1})", "AnchorEdge", result1.Error);
            SetId(ref instance, valueObj1);
            object valueObj2;
            Result result2 = UIXLoadResult.ValidateStringAsValue(splitString[1], SingleSchema.Type, null, out valueObj2);
            if (result2.Failed)
                return Result.Fail("Problem converting '{0}' ({1})", "AnchorEdge", result2.Error);
            SetPercent(ref instance, valueObj2);
            return result2;
        }

        private static object ConstructIdPercentOffset(object[] parameters)
        {
            object instanceObj = Construct();
            SetId(ref instanceObj, parameters[0]);
            SetPercent(ref instanceObj, parameters[1]);
            SetOffset(ref instanceObj, parameters[2]);
            return instanceObj;
        }

        private static Result ConvertFromStringIdPercentOffset(
          string[] splitString,
          out object instance)
        {
            instance = Construct();
            object valueObj1;
            Result result1 = UIXLoadResult.ValidateStringAsValue(splitString[0], StringSchema.Type, null, out valueObj1);
            if (result1.Failed)
                return Result.Fail("Problem converting '{0}' ({1})", "AnchorEdge", result1.Error);
            SetId(ref instance, valueObj1);
            object valueObj2;
            Result result2 = UIXLoadResult.ValidateStringAsValue(splitString[1], SingleSchema.Type, null, out valueObj2);
            if (result2.Failed)
                return Result.Fail("Problem converting '{0}' ({1})", "AnchorEdge", result2.Error);
            SetPercent(ref instance, valueObj2);
            object valueObj3;
            Result result3 = UIXLoadResult.ValidateStringAsValue(splitString[2], Int32Schema.Type, null, out valueObj3);
            if (result3.Failed)
                return Result.Fail("Problem converting '{0}' ({1})", "AnchorEdge", result3.Error);
            SetOffset(ref instance, valueObj3);
            return result3;
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
                string[] splitString = StringUtility.SplitAndTrim(',', (string)from);
                switch (splitString.Length)
                {
                    case 2:
                        result = ConvertFromStringIdPercent(splitString, out instance);
                        if (!result.Failed)
                            return result;
                        break;
                    case 3:
                        result = ConvertFromStringIdPercentOffset(splitString, out instance);
                        if (!result.Failed)
                            return result;
                        break;
                    default:
                        result = Result.Fail("Unable to convert \"{0}\" to type '{1}'", from.ToString(), "AnchorEdge");
                        break;
                }
            }
            return result;
        }

        public static void Pass1Initialize() => Type = new UIXTypeSchema(6, "AnchorEdge", null, 153, typeof(AnchorEdge), UIXTypeFlags.None);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(6, "Id", 208, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetId), new SetValueHandler(SetId), false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(6, "Percent", 194, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetPercent), new SetValueHandler(SetPercent), false);
            UIXPropertySchema uixPropertySchema3 = new UIXPropertySchema(6, "Offset", 115, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetOffset), new SetValueHandler(SetOffset), false);
            UIXPropertySchema uixPropertySchema4 = new UIXPropertySchema(6, "MaximumPercent", 194, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetMaximumPercent), new SetValueHandler(SetMaximumPercent), false);
            UIXPropertySchema uixPropertySchema5 = new UIXPropertySchema(6, "MaximumOffset", 115, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetMaximumOffset), new SetValueHandler(SetMaximumOffset), false);
            UIXPropertySchema uixPropertySchema6 = new UIXPropertySchema(6, "MinimumPercent", 194, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetMinimumPercent), new SetValueHandler(SetMinimumPercent), false);
            UIXPropertySchema uixPropertySchema7 = new UIXPropertySchema(6, "MinimumOffset", 115, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetMinimumOffset), new SetValueHandler(SetMinimumOffset), false);
            UIXConstructorSchema constructorSchema1 = new UIXConstructorSchema(6, new short[2]
            {
         208,
         194
            }, new ConstructHandler(ConstructIdPercent));
            UIXConstructorSchema constructorSchema2 = new UIXConstructorSchema(6, new short[3]
            {
         208,
         194,
         115
            }, new ConstructHandler(ConstructIdPercentOffset));
            Type.Initialize(new DefaultConstructHandler(Construct), new ConstructorSchema[2]
            {
         constructorSchema1,
         constructorSchema2
            }, new PropertySchema[7]
            {
         uixPropertySchema1,
         uixPropertySchema5,
         uixPropertySchema4,
         uixPropertySchema7,
         uixPropertySchema6,
         uixPropertySchema3,
         uixPropertySchema2
            }, null, null, null, new TypeConverterHandler(TryConvertFrom), new SupportsTypeConversionHandler(IsConversionSupported), null, null, null, null);
        }
    }
}
