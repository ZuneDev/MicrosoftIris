// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.PanelSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.UI;
using Microsoft.Iris.ViewItems;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class PanelSchema
    {
        public static UIXTypeSchema Type;

        private static object GetChildren(object instanceObj) => ViewItemSchema.ListProxy.GetChildren((ViewItem)instanceObj);

        private static object Construct() => new Panel();

        public static void Pass1Initialize() => Type = new UIXTypeSchema(156, "Panel", null, 239, typeof(Panel), UIXTypeFlags.Disposable);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema = new UIXPropertySchema(156, "Children", 138, 239, ExpressionRestriction.NoAccess, false, null, true, new GetValueHandler(GetChildren), null, false);
            Type.Initialize(new DefaultConstructHandler(Construct), null, new PropertySchema[1]
            {
         uixPropertySchema
            }, null, null, null, null, null, null, null, null, null);
        }
    }
}
