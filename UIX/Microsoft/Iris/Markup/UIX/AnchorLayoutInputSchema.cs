// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.AnchorLayoutInputSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Layouts;
using Microsoft.Iris.Library;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class AnchorLayoutInputSchema
    {
        public static UIXTypeSchema Type;

        private static object GetLeft(object instanceObj) => ((AnchorLayoutInput)instanceObj).Left;

        private static void SetLeft(ref object instanceObj, object valueObj) => ((AnchorLayoutInput)instanceObj).Left = (AnchorEdge)valueObj;

        private static object GetTop(object instanceObj) => ((AnchorLayoutInput)instanceObj).Top;

        private static void SetTop(ref object instanceObj, object valueObj) => ((AnchorLayoutInput)instanceObj).Top = (AnchorEdge)valueObj;

        private static object GetRight(object instanceObj) => ((AnchorLayoutInput)instanceObj).Right;

        private static void SetRight(ref object instanceObj, object valueObj) => ((AnchorLayoutInput)instanceObj).Right = (AnchorEdge)valueObj;

        private static object GetBottom(object instanceObj) => ((AnchorLayoutInput)instanceObj).Bottom;

        private static void SetBottom(ref object instanceObj, object valueObj) => ((AnchorLayoutInput)instanceObj).Bottom = (AnchorEdge)valueObj;

        private static object GetContributesToWidth(object instanceObj) => BooleanBoxes.Box(((AnchorLayoutInput)instanceObj).ContributesToWidth);

        private static void SetContributesToWidth(ref object instanceObj, object valueObj) => ((AnchorLayoutInput)instanceObj).ContributesToWidth = (bool)valueObj;

        private static object GetContributesToHeight(object instanceObj) => BooleanBoxes.Box(((AnchorLayoutInput)instanceObj).ContributesToHeight);

        private static void SetContributesToHeight(ref object instanceObj, object valueObj) => ((AnchorLayoutInput)instanceObj).ContributesToHeight = (bool)valueObj;

        private static object Construct() => new AnchorLayoutInput();

        private static Result ConvertFromString(object valueObj, out object instanceObj)
        {
            string id = (string)valueObj;
            instanceObj = null;
            instanceObj = new AnchorLayoutInput()
            {
                Left = new AnchorEdge(id, 0.0f),
                Top = new AnchorEdge(id, 0.0f),
                Right = new AnchorEdge(id, 1f),
                Bottom = new AnchorEdge(id, 1f)
            };
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

        private static object CallTryParseStringAnchorLayoutInput(
          object instanceObj,
          object[] parameters)
        {
            string parameter1 = (string)parameters[0];
            AnchorLayoutInput parameter2 = (AnchorLayoutInput)parameters[1];
            object instanceObj1;
            return ConvertFromString(parameter1, out instanceObj1).Failed ? parameter2 : instanceObj1;
        }

        public static void Pass1Initialize() => Type = new UIXTypeSchema(8, "AnchorLayoutInput", null, 133, typeof(AnchorLayoutInput), UIXTypeFlags.None);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(8, "Left", 6, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetLeft), new SetValueHandler(SetLeft), false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(8, "Top", 6, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetTop), new SetValueHandler(SetTop), false);
            UIXPropertySchema uixPropertySchema3 = new UIXPropertySchema(8, "Right", 6, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetRight), new SetValueHandler(SetRight), false);
            UIXPropertySchema uixPropertySchema4 = new UIXPropertySchema(8, "Bottom", 6, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetBottom), new SetValueHandler(SetBottom), false);
            UIXPropertySchema uixPropertySchema5 = new UIXPropertySchema(8, "ContributesToWidth", 15, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetContributesToWidth), new SetValueHandler(SetContributesToWidth), false);
            UIXPropertySchema uixPropertySchema6 = new UIXPropertySchema(8, "ContributesToHeight", 15, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetContributesToHeight), new SetValueHandler(SetContributesToHeight), false);
            UIXMethodSchema uixMethodSchema = new UIXMethodSchema(8, "TryParse", new short[2]
            {
         208,
         8
            }, 8, new InvokeHandler(CallTryParseStringAnchorLayoutInput), true);
            Type.Initialize(new DefaultConstructHandler(Construct), null, new PropertySchema[6]
            {
         uixPropertySchema4,
         uixPropertySchema6,
         uixPropertySchema5,
         uixPropertySchema1,
         uixPropertySchema3,
         uixPropertySchema2
            }, new MethodSchema[1]
            {
         uixMethodSchema
            }, null, null, new TypeConverterHandler(TryConvertFrom), new SupportsTypeConversionHandler(IsConversionSupported), null, null, null, null);
        }
    }
}
