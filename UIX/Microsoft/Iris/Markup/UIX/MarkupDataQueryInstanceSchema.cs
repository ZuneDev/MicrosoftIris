// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.MarkupDataQueryInstanceSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

namespace Microsoft.Iris.Markup.UIX
{
    internal static class MarkupDataQueryInstanceSchema
    {
        public static UIXTypeSchema Type;

        private static object GetStatus(object instanceObj) => ((MarkupDataQuery)instanceObj).Status;

        private static object GetResult(object instanceObj) => ((MarkupDataQuery)instanceObj).Result;

        private static object GetEnabled(object instanceObj) => BooleanBoxes.Box(((MarkupDataQuery)instanceObj).Enabled);

        private static void SetEnabled(ref object instanceObj, object valueObj) => ((MarkupDataQuery)instanceObj).Enabled = (bool)valueObj;

        private static object CallRefresh(object instanceObj, object[] parameters)
        {
            ((MarkupDataQuery)instanceObj).Refresh();
            return null;
        }

        public static void Pass1Initialize() => Type = new UIXTypeSchema(142, "MarkupDataQueryInstance", null, 153, typeof(MarkupDataQuery), UIXTypeFlags.Disposable);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(142, "Status", 47, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetStatus), null, false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(142, "Result", 143, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetResult), null, false);
            UIXPropertySchema uixPropertySchema3 = new UIXPropertySchema(142, "Enabled", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetEnabled), new SetValueHandler(SetEnabled), false);
            UIXMethodSchema uixMethodSchema = new UIXMethodSchema(142, "Refresh", null, 240, new InvokeHandler(CallRefresh), false);
            Type.Initialize(null, null, new PropertySchema[3]
            {
         uixPropertySchema3,
         uixPropertySchema2,
         uixPropertySchema1
            }, new MethodSchema[1]
            {
         uixMethodSchema
            }, null, null, null, null, null, null, null, null);
        }
    }
}
