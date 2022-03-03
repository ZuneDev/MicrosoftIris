// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.EdgeDetectionInstanceSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Animations;
using Microsoft.Iris.Library;
using Microsoft.Iris.Session;
using Microsoft.Iris.UI;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class EdgeDetectionInstanceSchema
    {
        public static UIXTypeSchema Type;

        private static void SetEdgeLimit(ref object instanceObj, object valueObj)
        {
            EffectElementWrapper effectElementWrapper = (EffectElementWrapper)instanceObj;
            float num = (float)valueObj;
            Result result = SingleSchema.Validate0to1(valueObj);
            if (result.Failed)
                ErrorManager.ReportError(result.Error);
            else
                effectElementWrapper.SetProperty("EdgeLimit", num);
        }

        private static object CallPlayEdgeLimitAnimationEffectFloatAnimation(
          object instanceObj,
          object[] parameters)
        {
            ((EffectElementWrapper)instanceObj).PlayAnimation(EffectProperty.EdgeLimit, (EffectAnimation)parameters[0]);
            return null;
        }

        public static void Pass1Initialize() => Type = new UIXTypeSchema(67, "EdgeDetectionInstance", null, 74, typeof(EffectElementWrapper), UIXTypeFlags.None);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema = new UIXPropertySchema(67, "EdgeLimit", 194, -1, ExpressionRestriction.None, false, SingleSchema.Validate0to1, false, null, new SetValueHandler(SetEdgeLimit), false);
            UIXMethodSchema uixMethodSchema = new UIXMethodSchema(67, "PlayEdgeLimitAnimation", new short[1]
            {
         75
            }, 240, new InvokeHandler(CallPlayEdgeLimitAnimationEffectFloatAnimation), false);
            Type.Initialize(null, null, new PropertySchema[1]
            {
         uixPropertySchema
            }, new MethodSchema[1]
            {
         uixMethodSchema
            }, null, null, null, null, null, null, null, null);
        }
    }
}
