// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.TextFragmentSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.ViewItems;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class TextFragmentSchema
    {
        public static UIXTypeSchema Type;

        private static object GetRuns(object instanceObj) => ((TextFragment)instanceObj).Runs;

        private static object GetTagName(object instanceObj) => ((TextFragment)instanceObj).TagName;

        private static object GetContent(object instanceObj) => ((TextFragment)instanceObj).Content;

        private static object GetAttributes(object instanceObj) => ((TextFragment)instanceObj).Attributes;

        public static void Pass1Initialize() => Type = new UIXTypeSchema(215, "TextFragment", null, 153, typeof(TextFragment), UIXTypeFlags.Immutable);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(215, "Runs", 138, 216, ExpressionRestriction.ReadOnly, false, null, false, new GetValueHandler(GetRuns), null, false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(215, "TagName", 208, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetTagName), null, false);
            UIXPropertySchema uixPropertySchema3 = new UIXPropertySchema(215, "Content", 208, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetContent), null, false);
            UIXPropertySchema uixPropertySchema4 = new UIXPropertySchema(215, "Attributes", 58, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetAttributes), null, false);
            Type.Initialize(null, null, new PropertySchema[4]
            {
         uixPropertySchema4,
         uixPropertySchema3,
         uixPropertySchema1,
         uixPropertySchema2
            }, null, null, null, null, null, null, null, null, null);
        }
    }
}
