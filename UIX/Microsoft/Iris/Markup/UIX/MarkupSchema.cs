// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.MarkupSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class MarkupSchema
    {
        public static UIXTypeSchema Type;

        private static object GetErrors(object instanceObj) => ((MarkupServices)instanceObj).Errors;

        private static object GetWarningsOnly(object instanceObj) => BooleanBoxes.Box(((MarkupServices)instanceObj).WarningsOnly);

        private static object Construct() => MarkupServices.Instance;

        private static object CallClearErrors(object instanceObj, object[] parameters)
        {
            ((MarkupServices)instanceObj).ClearErrors();
            return null;
        }

        private static object CallIsDisposedObject(object instanceObj, object[] parameters)
        {
            object parameter = parameters[0];
            return parameter == null ? true : BooleanBoxes.Box(parameter is IDisposableObject disposableObject && disposableObject.IsDisposed);
        }

        public static void Pass1Initialize() => Type = new UIXTypeSchema(141, "Markup", null, 153, typeof(MarkupServices), UIXTypeFlags.None);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(141, "Errors", 138, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetErrors), null, false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(141, "WarningsOnly", 15, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetWarningsOnly), null, false);
            UIXMethodSchema uixMethodSchema1 = new UIXMethodSchema(141, "ClearErrors", null, 240, new InvokeHandler(CallClearErrors), false);
            UIXEventSchema uixEventSchema = new UIXEventSchema(141, "ErrorsDetected");
            UIXMethodSchema uixMethodSchema2 = new UIXMethodSchema(141, "IsDisposed", new short[1]
            {
         153
            }, 15, new InvokeHandler(CallIsDisposedObject), true);
            Type.Initialize(new DefaultConstructHandler(Construct), null, new PropertySchema[2]
            {
         uixPropertySchema1,
         uixPropertySchema2
            }, new MethodSchema[2]
            {
         uixMethodSchema1,
         uixMethodSchema2
            }, new EventSchema[1] { uixEventSchema }, null, null, null, null, null, null, null);
        }
    }
}
