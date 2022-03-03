// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.IndexSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.ViewItems;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class IndexSchema
    {
        public static UIXTypeSchema Type;

        private static object GetValue(object instanceObj) => ((Index)instanceObj).Value;

        private static object GetSourceValue(object instanceObj) => ((Index)instanceObj).SourceValue;

        private static object CallGetContainerIndex(object instanceObj, object[] parameters) => ((Index)instanceObj).GetContainerIndex();

        public static void Pass1Initialize() => Type = new UIXTypeSchema(109, "Index", null, 153, typeof(Index), UIXTypeFlags.None);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(109, "Value", 115, -1, ExpressionRestriction.ReadOnly, false, null, true, new GetValueHandler(GetValue), null, false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(109, "SourceValue", 115, -1, ExpressionRestriction.ReadOnly, false, null, true, new GetValueHandler(GetSourceValue), null, false);
            UIXMethodSchema uixMethodSchema = new UIXMethodSchema(109, "GetContainerIndex", null, 109, new InvokeHandler(CallGetContainerIndex), false);
            Type.Initialize(null, null, new PropertySchema[2]
            {
         uixPropertySchema2,
         uixPropertySchema1
            }, new MethodSchema[1]
            {
         uixMethodSchema
            }, null, null, null, null, null, null, null, null);
        }
    }
}
