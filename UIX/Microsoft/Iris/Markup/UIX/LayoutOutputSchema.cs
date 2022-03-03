// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.LayoutOutputSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.ModelItems;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class LayoutOutputSchema
    {
        public static UIXTypeSchema Type;

        private static object GetSize(object instanceObj) => ((LayoutOutput)instanceObj).Size;

        public static void Pass1Initialize() => Type = new UIXTypeSchema(134, "LayoutOutput", null, 153, typeof(LayoutOutput), UIXTypeFlags.None);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema = new UIXPropertySchema(134, "Size", 195, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetSize), null, false);
            Type.Initialize(null, null, new PropertySchema[1]
            {
         uixPropertySchema
            }, null, null, null, null, null, null, null, null, null);
        }
    }
}
