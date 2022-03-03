// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.BlendSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Render;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class BlendSchema
    {
        public static UIXTypeSchema Type;

        private static object GetInput1(object instanceObj) => ((BlendElement)instanceObj).Input1;

        private static void SetInput1(ref object instanceObj, object valueObj) => ((BlendElement)instanceObj).Input1 = (EffectInput)valueObj;

        private static object GetInput2(object instanceObj) => ((BlendElement)instanceObj).Input2;

        private static void SetInput2(ref object instanceObj, object valueObj) => ((BlendElement)instanceObj).Input2 = (EffectInput)valueObj;

        private static object GetColorOperation(object instanceObj) => ((BlendElement)instanceObj).ColorOperation;

        private static void SetColorOperation(ref object instanceObj, object valueObj) => ((BlendElement)instanceObj).ColorOperation = (ColorOperation)valueObj;

        private static object GetAlphaOperation(object instanceObj) => ((BlendElement)instanceObj).AlphaOperation;

        private static void SetAlphaOperation(ref object instanceObj, object valueObj) => ((BlendElement)instanceObj).AlphaOperation = (AlphaOperation)valueObj;

        private static object Construct() => new BlendElement();

        public static void Pass1Initialize() => Type = new UIXTypeSchema(13, "Blend", null, 77, typeof(BlendElement), UIXTypeFlags.None);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(13, "Input1", 77, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetInput1), new SetValueHandler(SetInput1), false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(13, "Input2", 77, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetInput2), new SetValueHandler(SetInput2), false);
            UIXPropertySchema uixPropertySchema3 = new UIXPropertySchema(13, "ColorOperation", 38, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetColorOperation), new SetValueHandler(SetColorOperation), false);
            UIXPropertySchema uixPropertySchema4 = new UIXPropertySchema(13, "AlphaOperation", 5, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetAlphaOperation), new SetValueHandler(SetAlphaOperation), false);
            Type.Initialize(new DefaultConstructHandler(Construct), null, new PropertySchema[4]
            {
         uixPropertySchema4,
         uixPropertySchema3,
         uixPropertySchema1,
         uixPropertySchema2
            }, null, null, null, null, null, null, null, null, null);
        }
    }
}
