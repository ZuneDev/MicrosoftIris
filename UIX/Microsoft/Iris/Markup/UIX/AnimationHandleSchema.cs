// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.AnimationHandleSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Animations;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class AnimationHandleSchema
    {
        public static UIXTypeSchema Type;

        private static object GetPlaying(object instanceObj) => BooleanBoxes.Box(((AnimationHandle)instanceObj).Playing);

        private static object Construct() => new AnimationHandle();

        public static void Pass1Initialize() => Type = new UIXTypeSchema(11, "AnimationHandle", null, 153, typeof(AnimationHandle), UIXTypeFlags.None);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema = new UIXPropertySchema(11, "Playing", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetPlaying), null, false);
            UIXEventSchema uixEventSchema = new UIXEventSchema(11, "Completed");
            Type.Initialize(new DefaultConstructHandler(Construct), null, new PropertySchema[1]
            {
         uixPropertySchema
            }, null, new EventSchema[1]
            {
         uixEventSchema
            }, null, null, null, null, null, null, null);
        }
    }
}
