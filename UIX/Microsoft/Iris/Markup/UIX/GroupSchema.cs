// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.GroupSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.ModelItems;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class GroupSchema
    {
        public static UIXTypeSchema Type;

        private static object GetStartIndex(object instanceObj) => ((IUIGroup)instanceObj).StartIndex;

        private static object GetEndIndex(object instanceObj) => ((IUIGroup)instanceObj).EndIndex;

        public static void Pass1Initialize() => Type = new UIXTypeSchema(100, "Group", null, 138, typeof(IUIGroup), UIXTypeFlags.None);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(100, "StartIndex", 115, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetStartIndex), null, false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(100, "EndIndex", 115, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetEndIndex), null, false);
            Type.Initialize(null, null, new PropertySchema[2]
            {
         uixPropertySchema2,
         uixPropertySchema1
            }, null, null, null, null, null, null, null, null, null);
        }
    }
}
