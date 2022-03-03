// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.EffectLayerSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Render;
using System.Collections;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class EffectLayerSchema
    {
        public static UIXTypeSchema Type;

        private static object GetInput(object instanceObj) => ((EffectLayer)instanceObj).Input;

        private static void SetInput(ref object instanceObj, object valueObj) => ((EffectLayer)instanceObj).Input = (EffectInput)valueObj;

        private static object GetOperations(object instanceObj) => ((EffectLayer)instanceObj).Operations;

        private static void SetOperations(ref object instanceObj, object valueObj) => ((EffectLayer)instanceObj).Operations = (IList)valueObj;

        private static object Construct() => new EffectLayer();

        public static void Pass1Initialize() => Type = new UIXTypeSchema(79, "EffectLayer", null, 77, typeof(EffectLayer), UIXTypeFlags.None);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(79, "Input", 77, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetInput), new SetValueHandler(SetInput), false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(79, "Operations", 138, 80, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetOperations), new SetValueHandler(SetOperations), false);
            Type.Initialize(new DefaultConstructHandler(Construct), null, new PropertySchema[2]
            {
         uixPropertySchema1,
         uixPropertySchema2
            }, null, null, null, null, null, null, null, null, null);
        }
    }
}
