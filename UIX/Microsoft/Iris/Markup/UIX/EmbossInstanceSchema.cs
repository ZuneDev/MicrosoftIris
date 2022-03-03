// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.EmbossInstanceSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.UI;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class EmbossInstanceSchema
    {
        public static UIXTypeSchema Type;

        private static void SetDirection(ref object instanceObj, object valueObj) => ((EffectElementWrapper)instanceObj).SetProperty("Direction", (int)valueObj);

        public static void Pass1Initialize() => Type = new UIXTypeSchema(85, "EmbossInstance", null, 74, typeof(EffectElementWrapper), UIXTypeFlags.None);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema = new UIXPropertySchema(85, "Direction", 84, -1, ExpressionRestriction.None, false, null, false, null, new SetValueHandler(SetDirection), false);
            Type.Initialize(null, null, new PropertySchema[1]
            {
         uixPropertySchema
            }, null, null, null, null, null, null, null, null, null);
        }
    }
}
