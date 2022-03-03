// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.TextRunRendererSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Drawing;
using Microsoft.Iris.UI;
using Microsoft.Iris.ViewItems;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class TextRunRendererSchema
    {
        public static UIXTypeSchema Type;

        private static object GetData(object instanceObj) => ((TextRunRenderer)instanceObj).Data;

        private static void SetData(ref object instanceObj, object valueObj) => ((TextRunRenderer)instanceObj).Data = (TextRunData)valueObj;

        private static object GetColor(object instanceObj) => ((TextRunRenderer)instanceObj).Color;

        private static void SetColor(ref object instanceObj, object valueObj) => ((TextRunRenderer)instanceObj).Color = (Color)valueObj;

        private static object GetEffect(object instanceObj) => ((ViewItem)instanceObj).Effect;

        private static void SetEffect(ref object instanceObj, object valueObj) => ((ViewItem)instanceObj).Effect = (EffectClass)valueObj;

        private static object Construct() => new TextRunRenderer();

        public static void Pass1Initialize() => Type = new UIXTypeSchema(217, "TextRunRenderer", null, 239, typeof(TextRunRenderer), UIXTypeFlags.Disposable);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(217, "Data", 216, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetData), new SetValueHandler(SetData), false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(217, "Color", 35, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetColor), new SetValueHandler(SetColor), false);
            UIXPropertySchema uixPropertySchema3 = new UIXPropertySchema(217, "Effect", 78, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetEffect), new SetValueHandler(SetEffect), false);
            Type.Initialize(new DefaultConstructHandler(Construct), null, new PropertySchema[3]
            {
         uixPropertySchema2,
         uixPropertySchema1,
         uixPropertySchema3
            }, null, null, null, null, null, null, null, null, null);
        }
    }
}
