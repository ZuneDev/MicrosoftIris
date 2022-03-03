// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.TypeSelectorSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.ViewItems;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class TypeSelectorSchema
    {
        public static UIXTypeSchema Type;

        private static object GetType(object instanceObj) => ((TypeSelector)instanceObj).Type;

        private static void SetType(ref object instanceObj, object valueObj) => ((TypeSelector)instanceObj).Type = (TypeSchema)valueObj;

        private static object GetContentName(object instanceObj) => ((TypeSelector)instanceObj).ContentName;

        private static void SetContentName(ref object instanceObj, object valueObj) => ((TypeSelector)instanceObj).ContentName = (string)valueObj;

        private static object Construct() => new TypeSelector();

        public static void Pass1Initialize() => Type = new UIXTypeSchema(227, "TypeSelector", null, 153, typeof(TypeSelector), UIXTypeFlags.None);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(227, "Type", 225, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetType), new SetValueHandler(SetType), false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(227, "ContentName", 208, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetContentName), new SetValueHandler(SetContentName), false);
            Type.Initialize(new DefaultConstructHandler(Construct), null, new PropertySchema[2]
            {
         uixPropertySchema2,
         uixPropertySchema1
            }, null, null, null, null, null, null, null, null, null);
        }
    }
}
