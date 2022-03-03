// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.StackLayoutInputSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Layouts;
using Microsoft.Iris.Render;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class StackLayoutInputSchema
    {
        public static UIXTypeSchema Type;

        private static object GetPriority(object instanceObj) => ((StackLayoutInput)instanceObj).Priority;

        private static void SetPriority(ref object instanceObj, object valueObj) => ((StackLayoutInput)instanceObj).Priority = (StackPriority)valueObj;

        private static object GetMinimumSize(object instanceObj) => ((StackLayoutInput)instanceObj).MinimumSize;

        private static void SetMinimumSize(ref object instanceObj, object valueObj) => ((StackLayoutInput)instanceObj).MinimumSize = (Size)valueObj;

        private static object Construct() => new StackLayoutInput();

        public static void Pass1Initialize() => Type = new UIXTypeSchema(205, "StackLayoutInput", null, 133, typeof(StackLayoutInput), UIXTypeFlags.None);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(205, "Priority", 206, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetPriority), new SetValueHandler(SetPriority), false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(205, "MinimumSize", 195, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetMinimumSize), new SetValueHandler(SetMinimumSize), false);
            Type.Initialize(new DefaultConstructHandler(Construct), null, new PropertySchema[2]
            {
         uixPropertySchema2,
         uixPropertySchema1
            }, null, null, null, null, null, null, null, null, null);
        }
    }
}
