// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.ColorElementSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Drawing;
using Microsoft.Iris.Render;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class ColorElementSchema
    {
        public static UIXTypeSchema Type;

        private static void SetColor(ref object instanceObj, object valueObj) => ((ColorElement)instanceObj).Color = ((Color)valueObj).RenderConvert();

        private static object Construct() => new ColorElement();

        public static void Pass1Initialize() => Type = new UIXTypeSchema(36, "ColorElement", null, 77, typeof(ColorElement), UIXTypeFlags.None);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema = new UIXPropertySchema(36, "Color", 35, -1, ExpressionRestriction.None, false, null, false, null, new SetValueHandler(SetColor), false);
            Type.Initialize(new DefaultConstructHandler(Construct), null, new PropertySchema[1]
            {
         uixPropertySchema
            }, null, null, null, null, null, null, null, null, null);
        }
    }
}
