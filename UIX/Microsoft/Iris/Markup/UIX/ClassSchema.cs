// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.ClassSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.UI;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class ClassSchema
    {
        public static UIXTypeSchema Type;

        private static void SetShared(ref object instanceObj, object valueObj)
        {
            Class @class = (Class)instanceObj;
            int num = (bool)valueObj ? 1 : 0;
        }

        private static void SetBase(ref object instanceObj, object valueObj)
        {
            Class @class = (Class)instanceObj;
        }

        private static object GetProperties(object instanceObj) => ((Class)instanceObj).Storage;

        private static object GetLocals(object instanceObj) => ((Class)instanceObj).Storage;

        private static object GetScripts(object instanceObj) => (object)null;

        public static void Pass1Initialize() => Type = new UIXTypeSchema(29, "Class", null, -1, typeof(Class), UIXTypeFlags.Disposable);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(29, "Shared", 15, -1, ExpressionRestriction.NoAccess, false, null, true, null, new SetValueHandler(SetShared), false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(29, "Base", 208, -1, ExpressionRestriction.NoAccess, false, null, true, null, new SetValueHandler(SetBase), false);
            UIXPropertySchema uixPropertySchema3 = new UIXPropertySchema(29, "Properties", 58, -1, ExpressionRestriction.NoAccess, false, null, true, new GetValueHandler(GetProperties), null, false);
            UIXPropertySchema uixPropertySchema4 = new UIXPropertySchema(29, "Locals", 58, -1, ExpressionRestriction.NoAccess, false, null, true, new GetValueHandler(GetLocals), null, false);
            UIXPropertySchema uixPropertySchema5 = new UIXPropertySchema(29, "Scripts", 138, 240, ExpressionRestriction.NoAccess, false, null, true, new GetValueHandler(GetScripts), null, false);
            Type.Initialize(null, null, new PropertySchema[5]
            {
         uixPropertySchema2,
         uixPropertySchema4,
         uixPropertySchema3,
         uixPropertySchema5,
         uixPropertySchema1
            }, null, null, null, null, null, null, null, null, null);
        }
    }
}
