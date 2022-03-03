// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.SelectionRangeSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Data;
using Microsoft.Iris.Library;
using Microsoft.Iris.ModelItems;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class SelectionRangeSchema
    {
        public static UIXTypeSchema Type;

        private static object GetBegin(object instanceObj) => ((Range)instanceObj).Begin;

        private static void SetBegin(ref object instanceObj, object valueObj)
        {
            Range range = (Range)instanceObj;
            int num = (int)valueObj;
            range.Begin = num;
            instanceObj = range;
        }

        private static object GetEnd(object instanceObj) => ((Range)instanceObj).End;

        private static void SetEnd(ref object instanceObj, object valueObj)
        {
            Range range = (Range)instanceObj;
            int num = (int)valueObj;
            range.End = num;
            instanceObj = range;
        }

        private static object GetIsEmpty(object instanceObj) => BooleanBoxes.Box(((Range)instanceObj).IsEmpty);

        private static object Construct() => new Range(0, 0);

        private static object ConstructBeginEnd(object[] parameters)
        {
            object instanceObj = Construct();
            SetBegin(ref instanceObj, parameters[0]);
            SetEnd(ref instanceObj, parameters[1]);
            return instanceObj;
        }

        private static Result ConvertFromStringBeginEnd(string[] splitString, out object instance)
        {
            instance = Construct();
            object valueObj1;
            Result result1 = UIXLoadResult.ValidateStringAsValue(splitString[0], Int32Schema.Type, null, out valueObj1);
            if (result1.Failed)
                return Result.Fail("Problem converting '{0}' ({1})", "SelectionRange", result1.Error);
            SetBegin(ref instance, valueObj1);
            object valueObj2;
            Result result2 = UIXLoadResult.ValidateStringAsValue(splitString[1], Int32Schema.Type, null, out valueObj2);
            if (result2.Failed)
                return Result.Fail("Problem converting '{0}' ({1})", "SelectionRange", result2.Error);
            SetEnd(ref instance, valueObj2);
            return result2;
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
                if (splitString.Length == 2)
                {
                    result = ConvertFromStringBeginEnd(splitString, out instance);
                    if (!result.Failed)
                        return result;
                }
                else
                    result = Result.Fail("Unable to convert \"{0}\" to type '{1}'", from.ToString(), "SelectionRange");
            }
            return result;
        }

        public static void Pass1Initialize() => Type = new UIXTypeSchema(187, "SelectionRange", null, 153, typeof(Range), UIXTypeFlags.Immutable);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(187, "Begin", 115, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetBegin), new SetValueHandler(SetBegin), false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(187, "End", 115, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetEnd), new SetValueHandler(SetEnd), false);
            UIXPropertySchema uixPropertySchema3 = new UIXPropertySchema(187, "IsEmpty", 15, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetIsEmpty), null, false);
            UIXConstructorSchema constructorSchema = new UIXConstructorSchema(187, new short[2]
            {
         115,
         115
            }, new ConstructHandler(ConstructBeginEnd));
            Type.Initialize(new DefaultConstructHandler(Construct), new ConstructorSchema[1]
            {
         constructorSchema
            }, new PropertySchema[3]
            {
         uixPropertySchema1,
         uixPropertySchema2,
         uixPropertySchema3
            }, null, null, null, new TypeConverterHandler(TryConvertFrom), new SupportsTypeConversionHandler(IsConversionSupported), null, null, null, null);
        }
    }
}
