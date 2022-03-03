// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.RelativeToSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Animations;
using Microsoft.Iris.Data;
using Microsoft.Iris.Library;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class RelativeToSchema
    {
        public static UIXTypeSchema Type;

        private static object GetSourceId(object instanceObj) => ((RelativeTo)instanceObj).SourceId;

        private static void SetSourceId(ref object instanceObj, object valueObj) => ((RelativeTo)instanceObj).SourceId = (int)valueObj;

        private static object GetProperty(object instanceObj) => ((RelativeTo)instanceObj).Property;

        private static void SetProperty(ref object instanceObj, object valueObj) => ((RelativeTo)instanceObj).Property = (string)valueObj;

        private static object GetSnapshot(object instanceObj) => ((RelativeTo)instanceObj).Snapshot;

        private static void SetSnapshot(ref object instanceObj, object valueObj) => ((RelativeTo)instanceObj).Snapshot = (SnapshotPolicy)valueObj;

        private static object GetPower(object instanceObj) => ((RelativeTo)instanceObj).Power;

        private static void SetPower(ref object instanceObj, object valueObj) => ((RelativeTo)instanceObj).Power = (int)valueObj;

        private static object GetMultiply(object instanceObj) => ((RelativeTo)instanceObj).Multiply;

        private static void SetMultiply(ref object instanceObj, object valueObj) => ((RelativeTo)instanceObj).Multiply = (float)valueObj;

        private static object GetAdd(object instanceObj) => ((RelativeTo)instanceObj).Add;

        private static void SetAdd(ref object instanceObj, object valueObj) => ((RelativeTo)instanceObj).Add = (float)valueObj;

        private static object Construct() => new RelativeTo();

        private static object ConstructSourceIdProperty(object[] parameters)
        {
            object instanceObj = Construct();
            SetSourceId(ref instanceObj, parameters[0]);
            SetProperty(ref instanceObj, parameters[1]);
            return instanceObj;
        }

        private static Result ConvertFromStringSourceIdProperty(
          string[] splitString,
          out object instance)
        {
            instance = Construct();
            object valueObj1;
            Result result1 = UIXLoadResult.ValidateStringAsValue(splitString[0], Int32Schema.Type, null, out valueObj1);
            if (result1.Failed)
                return Result.Fail("Problem converting '{0}' ({1})", "RelativeTo", result1.Error);
            SetSourceId(ref instance, valueObj1);
            object valueObj2;
            Result result2 = UIXLoadResult.ValidateStringAsValue(splitString[1], StringSchema.Type, null, out valueObj2);
            if (result2.Failed)
                return Result.Fail("Problem converting '{0}' ({1})", "RelativeTo", result2.Error);
            SetProperty(ref instance, valueObj2);
            return result2;
        }

        private static Result ConvertFromString(object valueObj, out object instanceObj)
        {
            string str = (string)valueObj;
            instanceObj = null;
            RelativeTo instance = StringToInstance(str);
            if (instance == null)
                return Result.Fail("Unable to convert \"{0}\" to type '{1}'", str, "RelativeTo");
            instanceObj = instance;
            return Result.Success;
        }

        private static object FindCanonicalInstance(string name) => StringToInstance(name);

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
            if (StringSchema.Type.IsAssignableFrom(fromType))
            {
                string[] splitString = StringUtility.SplitAndTrim(',', (string)from);
                if (splitString.Length == 2)
                {
                    result = ConvertFromStringSourceIdProperty(splitString, out instance);
                    if (!result.Failed)
                        return result;
                }
                else
                    result = Result.Fail("Unable to convert \"{0}\" to type '{1}'", from.ToString(), "RelativeTo");
            }
            return result;
        }

        private static object CallTryParseStringRelativeTo(object instanceObj, object[] parameters)
        {
            string parameter1 = (string)parameters[0];
            RelativeTo parameter2 = (RelativeTo)parameters[1];
            object instanceObj1;
            return ConvertFromString(parameter1, out instanceObj1).Failed ? parameter2 : instanceObj1;
        }

        private static RelativeTo StringToInstance(string value)
        {
            if (value == "Absolute")
                return RelativeTo.Absolute;
            if (value == "Current")
                return RelativeTo.Current;
            if (value == "CurrentSnapshotOnLoop")
                return RelativeTo.CurrentSnapshotOnLoop;
            return value == "Final" ? RelativeTo.Final : null;
        }

        public static void Pass1Initialize() => Type = new UIXTypeSchema(171, "RelativeTo", null, 153, typeof(RelativeTo), UIXTypeFlags.Immutable);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(171, "SourceId", 115, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetSourceId), new SetValueHandler(SetSourceId), false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(171, "Property", 208, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetProperty), new SetValueHandler(SetProperty), false);
            UIXPropertySchema uixPropertySchema3 = new UIXPropertySchema(171, "Snapshot", 200, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetSnapshot), new SetValueHandler(SetSnapshot), false);
            UIXPropertySchema uixPropertySchema4 = new UIXPropertySchema(171, "Power", 115, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetPower), new SetValueHandler(SetPower), false);
            UIXPropertySchema uixPropertySchema5 = new UIXPropertySchema(171, "Multiply", 194, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetMultiply), new SetValueHandler(SetMultiply), false);
            UIXPropertySchema uixPropertySchema6 = new UIXPropertySchema(171, "Add", 194, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetAdd), new SetValueHandler(SetAdd), false);
            UIXConstructorSchema constructorSchema = new UIXConstructorSchema(171, new short[2]
            {
         115,
         208
            }, new ConstructHandler(ConstructSourceIdProperty));
            UIXMethodSchema uixMethodSchema = new UIXMethodSchema(171, "TryParse", new short[2]
            {
         208,
         171
            }, 171, new InvokeHandler(CallTryParseStringRelativeTo), true);
            Type.Initialize(new DefaultConstructHandler(Construct), new ConstructorSchema[1]
            {
         constructorSchema
            }, new PropertySchema[6]
            {
         uixPropertySchema6,
         uixPropertySchema5,
         uixPropertySchema4,
         uixPropertySchema2,
         uixPropertySchema3,
         uixPropertySchema1
            }, new MethodSchema[1]
            {
         uixMethodSchema
            }, null, new FindCanonicalInstanceHandler(FindCanonicalInstance), new TypeConverterHandler(TryConvertFrom), new SupportsTypeConversionHandler(IsConversionSupported), null, null, null, null);
        }
    }
}
