// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.BlurSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Render;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class BlurSchema
    {
        public static UIXTypeSchema Type;

        private static object GetMode(object instanceObj) => ((GaussianBlurElement)instanceObj).Mode;

        private static void SetMode(ref object instanceObj, object valueObj) => ((GaussianBlurElement)instanceObj).Mode = (GaussianBlurMode)valueObj;

        private static object GetKernelRadius(object instanceObj) => ((GaussianBlurElement)instanceObj).KernelRadius;

        private static void SetKernelRadius(ref object instanceObj, object valueObj) => ((GaussianBlurElement)instanceObj).KernelRadius = (int)valueObj;

        private static object GetBluriness(object instanceObj) => ((GaussianBlurElement)instanceObj).Bluriness;

        private static void SetBluriness(ref object instanceObj, object valueObj) => ((GaussianBlurElement)instanceObj).Bluriness = (float)valueObj;

        private static object Construct() => new GaussianBlurElement();

        public static void Pass1Initialize() => Type = new UIXTypeSchema(14, "Blur", null, 80, typeof(GaussianBlurElement), UIXTypeFlags.None);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(14, "Mode", 96, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetMode), new SetValueHandler(SetMode), false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(14, "KernelRadius", 115, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetKernelRadius), new SetValueHandler(SetKernelRadius), false);
            UIXPropertySchema uixPropertySchema3 = new UIXPropertySchema(14, "Bluriness", 194, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetBluriness), new SetValueHandler(SetBluriness), false);
            Type.Initialize(new DefaultConstructHandler(Construct), null, new PropertySchema[3]
            {
         uixPropertySchema3,
         uixPropertySchema2,
         uixPropertySchema1
            }, null, null, null, null, null, null, null, null, null);
        }
    }
}
