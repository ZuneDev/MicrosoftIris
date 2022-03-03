// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.EmbossSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Render;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class EmbossSchema
    {
        public static UIXTypeSchema Type;

        private static object GetDirection(object instanceObj) => ((EmbossElement)instanceObj).Direction;

        private static void SetDirection(ref object instanceObj, object valueObj) => ((EmbossElement)instanceObj).Direction = (EmbossDirection)valueObj;

        private static object Construct() => new EmbossElement();

        public static void Pass1Initialize() => Type = new UIXTypeSchema(83, "Emboss", null, 80, typeof(EmbossElement), UIXTypeFlags.None);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema = new UIXPropertySchema(83, "Direction", 84, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetDirection), new SetValueHandler(SetDirection), false);
            Type.Initialize(new DefaultConstructHandler(Construct), null, new PropertySchema[1]
            {
         uixPropertySchema
            }, null, null, null, null, null, null, null, null, null);
        }
    }
}
