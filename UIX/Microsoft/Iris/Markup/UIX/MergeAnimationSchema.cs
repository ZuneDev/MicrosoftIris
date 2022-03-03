// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.MergeAnimationSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Animations;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class MergeAnimationSchema
    {
        public static UIXTypeSchema Type;

        private static object GetSources(object instanceObj) => ((MergeAnimation)instanceObj).Sources;

        private static object GetType(object instanceObj) => ((MergeAnimation)instanceObj).Type;

        private static void SetType(ref object instanceObj, object valueObj) => ((MergeAnimation)instanceObj).Type = (AnimationEventType)valueObj;

        private static object Construct() => new MergeAnimation();

        public static void Pass1Initialize() => Type = new UIXTypeSchema(147, "MergeAnimation", null, 104, typeof(MergeAnimation), UIXTypeFlags.None);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(147, "Sources", 138, 104, ExpressionRestriction.NoAccess, false, null, false, new GetValueHandler(GetSources), null, false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(147, "Type", 10, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetType), new SetValueHandler(SetType), false);
            Type.Initialize(new DefaultConstructHandler(Construct), null, new PropertySchema[2]
            {
         uixPropertySchema1,
         uixPropertySchema2
            }, null, null, null, null, null, null, null, null, null);
        }
    }
}
