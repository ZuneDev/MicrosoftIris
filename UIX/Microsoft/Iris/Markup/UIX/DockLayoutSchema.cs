// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.DockLayoutSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Layout;
using Microsoft.Iris.Layouts;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class DockLayoutSchema
    {
        public static UIXTypeSchema Type;

        private static object GetDefaultLayoutInput(object instanceObj) => ((DockLayout)instanceObj).DefaultLayoutInput;

        private static void SetDefaultLayoutInput(ref object instanceObj, object valueObj) => ((DockLayout)instanceObj).DefaultLayoutInput = (DockLayoutInput)valueObj;

        private static object GetDefaultChildAlignment(object instanceObj) => ((DockLayout)instanceObj).DefaultChildAlignment;

        private static void SetDefaultChildAlignment(ref object instanceObj, object valueObj) => ((DockLayout)instanceObj).DefaultChildAlignment = (ItemAlignment)valueObj;

        private static object Construct() => new DockLayout();

        public static void Pass1Initialize() => Type = new UIXTypeSchema(59, "DockLayout", null, 132, typeof(DockLayout), UIXTypeFlags.Immutable);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(59, "DefaultLayoutInput", 60, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetDefaultLayoutInput), new SetValueHandler(SetDefaultLayoutInput), false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(59, "DefaultChildAlignment", sbyte.MaxValue, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetDefaultChildAlignment), new SetValueHandler(SetDefaultChildAlignment), false);
            Type.Initialize(new DefaultConstructHandler(Construct), null, new PropertySchema[2]
            {
         uixPropertySchema2,
         uixPropertySchema1
            }, null, null, null, null, null, null, null, null, null);
        }
    }
}
