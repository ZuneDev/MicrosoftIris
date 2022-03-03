// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.MarkupErrorSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

namespace Microsoft.Iris.Markup.UIX
{
    internal static class MarkupErrorSchema
    {
        public static UIXTypeSchema Type;

        private static object GetContext(object instanceObj) => ((MarkupError)instanceObj).Context;

        private static object GetMessage(object instanceObj) => ((MarkupError)instanceObj).Message;

        private static object GetUri(object instanceObj) => ((MarkupError)instanceObj).Uri;

        private static object GetLine(object instanceObj) => ((MarkupError)instanceObj).Line;

        private static object GetColumn(object instanceObj) => ((MarkupError)instanceObj).Column;

        private static object GetIsError(object instanceObj) => BooleanBoxes.Box(((MarkupError)instanceObj).IsError);

        public static void Pass1Initialize() => Type = new UIXTypeSchema(144, "MarkupError", null, 153, typeof(MarkupError), UIXTypeFlags.Immutable);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(144, "Context", 208, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetContext), null, false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(144, "Message", 208, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetMessage), null, false);
            UIXPropertySchema uixPropertySchema3 = new UIXPropertySchema(144, "Uri", 208, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetUri), null, false);
            UIXPropertySchema uixPropertySchema4 = new UIXPropertySchema(144, "Line", 115, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetLine), null, false);
            UIXPropertySchema uixPropertySchema5 = new UIXPropertySchema(144, "Column", 115, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetColumn), null, false);
            UIXPropertySchema uixPropertySchema6 = new UIXPropertySchema(144, "IsError", 15, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetIsError), null, false);
            Type.Initialize(null, null, new PropertySchema[6]
            {
         uixPropertySchema5,
         uixPropertySchema1,
         uixPropertySchema6,
         uixPropertySchema4,
         uixPropertySchema2,
         uixPropertySchema3
            }, null, null, null, null, null, null, null, null, null);
        }
    }
}
