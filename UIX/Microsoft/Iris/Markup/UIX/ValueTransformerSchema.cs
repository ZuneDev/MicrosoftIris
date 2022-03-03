// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.ValueTransformerSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Animations;
using Microsoft.Iris.Library;
using Microsoft.Iris.Session;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class ValueTransformerSchema
    {
        public static UIXTypeSchema Type;

        private static object GetAdd(object instanceObj) => ((ValueTransformer)instanceObj).Add;

        private static void SetAdd(ref object instanceObj, object valueObj) => ((ValueTransformer)instanceObj).Add = (float)valueObj;

        private static object GetSubtract(object instanceObj) => ((ValueTransformer)instanceObj).Subtract;

        private static void SetSubtract(ref object instanceObj, object valueObj) => ((ValueTransformer)instanceObj).Subtract = (float)valueObj;

        private static object GetMultiply(object instanceObj) => ((ValueTransformer)instanceObj).Multiply;

        private static void SetMultiply(ref object instanceObj, object valueObj) => ((ValueTransformer)instanceObj).Multiply = (float)valueObj;

        private static object GetDivide(object instanceObj) => ((ValueTransformer)instanceObj).Divide;

        private static void SetDivide(ref object instanceObj, object valueObj)
        {
            ValueTransformer valueTransformer = (ValueTransformer)instanceObj;
            float num = (float)valueObj;
            Result result = SingleSchema.ValidateNotZero(valueObj);
            if (result.Failed)
                ErrorManager.ReportError(result.Error);
            else
                valueTransformer.Divide = num;
        }

        private static object GetMod(object instanceObj) => ((ValueTransformer)instanceObj).Mod;

        private static void SetMod(ref object instanceObj, object valueObj)
        {
            ValueTransformer valueTransformer = (ValueTransformer)instanceObj;
            float num = (float)valueObj;
            Result result = SingleSchema.ValidateNotZero(valueObj);
            if (result.Failed)
                ErrorManager.ReportError(result.Error);
            else
                valueTransformer.Mod = num;
        }

        private static object GetAbsolute(object instanceObj) => BooleanBoxes.Box(((ValueTransformer)instanceObj).Absolute);

        private static void SetAbsolute(ref object instanceObj, object valueObj) => ((ValueTransformer)instanceObj).Absolute = (bool)valueObj;

        private static object Construct() => new ValueTransformer();

        public static void Pass1Initialize() => Type = new UIXTypeSchema(232, "ValueTransformer", null, 153, typeof(ValueTransformer), UIXTypeFlags.None);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(232, "Add", 194, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetAdd), new SetValueHandler(SetAdd), false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(232, "Subtract", 194, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetSubtract), new SetValueHandler(SetSubtract), false);
            UIXPropertySchema uixPropertySchema3 = new UIXPropertySchema(232, "Multiply", 194, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetMultiply), new SetValueHandler(SetMultiply), false);
            UIXPropertySchema uixPropertySchema4 = new UIXPropertySchema(232, "Divide", 194, -1, ExpressionRestriction.None, false, SingleSchema.ValidateNotZero, false, new GetValueHandler(GetDivide), new SetValueHandler(SetDivide), false);
            UIXPropertySchema uixPropertySchema5 = new UIXPropertySchema(232, "Mod", 194, -1, ExpressionRestriction.None, false, SingleSchema.ValidateNotZero, false, new GetValueHandler(GetMod), new SetValueHandler(SetMod), false);
            UIXPropertySchema uixPropertySchema6 = new UIXPropertySchema(232, "Absolute", 15, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetAbsolute), new SetValueHandler(SetAbsolute), false);
            Type.Initialize(new DefaultConstructHandler(Construct), null, new PropertySchema[6]
            {
         uixPropertySchema6,
         uixPropertySchema1,
         uixPropertySchema4,
         uixPropertySchema5,
         uixPropertySchema3,
         uixPropertySchema2
            }, null, null, null, null, null, null, null, null, null);
        }
    }
}
