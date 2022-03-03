// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.UISchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.UI;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class UISchema
    {
        public static UIXTypeSchema Type;

        private static object GetProperties(object instanceObj) => ((UIClass)instanceObj).Storage;

        private static object GetLocals(object instanceObj) => ((UIClass)instanceObj).Storage;

        private static object GetInput(object instanceObj) => ((UIClass)instanceObj).EnsureInputHandlerStorage();

        private static object GetContent(object instanceObj) => ((UIClass)instanceObj).RootItem;

        private static void SetContent(ref object instanceObj, object valueObj) => ((UIClass)instanceObj).SetRootItem((ViewItem)valueObj);

        private static object GetFlippable(object instanceObj) => BooleanBoxes.Box(((UIClass)instanceObj).Flippable);

        private static void SetFlippable(ref object instanceObj, object valueObj) => ((UIClass)instanceObj).Flippable = (bool)valueObj;

        private static void SetBase(ref object instanceObj, object valueObj)
        {
            UIClass uiClass = (UIClass)instanceObj;
        }

        private static object GetScripts(object instanceObj) => (object)null;

        public static void Pass1Initialize() => Type = new UIXTypeSchema(229, "UI", null, -1, typeof(UIClass), UIXTypeFlags.Disposable);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(229, "Properties", 58, -1, ExpressionRestriction.NoAccess, false, null, true, new GetValueHandler(GetProperties), null, false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(229, "Locals", 58, -1, ExpressionRestriction.NoAccess, false, null, true, new GetValueHandler(GetLocals), null, false);
            UIXPropertySchema uixPropertySchema3 = new UIXPropertySchema(229, "Input", 138, 110, ExpressionRestriction.NoAccess, false, null, true, new GetValueHandler(GetInput), null, false);
            UIXPropertySchema uixPropertySchema4 = new UIXPropertySchema(229, "Content", 239, -1, ExpressionRestriction.NoAccess, false, null, true, new GetValueHandler(GetContent), new SetValueHandler(SetContent), false);
            UIXPropertySchema uixPropertySchema5 = new UIXPropertySchema(229, "Flippable", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetFlippable), new SetValueHandler(SetFlippable), false);
            UIXPropertySchema uixPropertySchema6 = new UIXPropertySchema(229, "Base", 208, -1, ExpressionRestriction.NoAccess, false, null, true, null, new SetValueHandler(SetBase), false);
            UIXPropertySchema uixPropertySchema7 = new UIXPropertySchema(229, "Scripts", 138, 240, ExpressionRestriction.NoAccess, false, null, true, new GetValueHandler(GetScripts), null, false);
            Type.Initialize(null, null, new PropertySchema[7]
            {
         uixPropertySchema6,
         uixPropertySchema4,
         uixPropertySchema5,
         uixPropertySchema3,
         uixPropertySchema2,
         uixPropertySchema1,
         uixPropertySchema7
            }, null, null, null, null, null, null, null, null, null);
        }
    }
}
