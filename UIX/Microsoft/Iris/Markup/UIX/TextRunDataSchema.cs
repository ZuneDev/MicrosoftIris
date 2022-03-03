// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.TextRunDataSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.ViewItems;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class TextRunDataSchema
    {
        public static UIXTypeSchema Type;

        private static object GetPosition(object instanceObj) => ((TextRunData)instanceObj).Position;

        private static object GetSize(object instanceObj) => ((TextRunData)instanceObj).Size;

        private static object GetColor(object instanceObj) => ((TextRunData)instanceObj).Color;

        private static object GetLineNumber(object instanceObj) => ((TextRunData)instanceObj).LineNumber;

        public static void Pass1Initialize() => Type = new UIXTypeSchema(216, "TextRunData", null, 153, typeof(TextRunData), UIXTypeFlags.Immutable);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(216, "Position", 158, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetPosition), null, false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(216, "Size", 195, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetSize), null, false);
            UIXPropertySchema uixPropertySchema3 = new UIXPropertySchema(216, "Color", 35, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetColor), null, false);
            UIXPropertySchema uixPropertySchema4 = new UIXPropertySchema(216, "LineNumber", 115, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetLineNumber), null, false);
            Type.Initialize(null, null, new PropertySchema[4]
            {
         uixPropertySchema3,
         uixPropertySchema4,
         uixPropertySchema1,
         uixPropertySchema2
            }, null, null, null, null, null, null, null, null, null);
        }
    }
}
