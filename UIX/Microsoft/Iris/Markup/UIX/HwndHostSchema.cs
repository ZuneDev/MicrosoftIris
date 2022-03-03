// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.HwndHostSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.ViewItems;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class HwndHostSchema
    {
        public static UIXTypeSchema Type;

        private static object GetHandle(object instanceObj) => ((HwndHost)instanceObj).Handle;

        private static object GetChildHandle(object instanceObj) => ((HwndHost)instanceObj).ChildHandle;

        private static void SetChildHandle(ref object instanceObj, object valueObj) => ((HwndHost)instanceObj).ChildHandle = (long)valueObj;

        private static object Construct() => new HwndHost();

        public static void Pass1Initialize() => Type = new UIXTypeSchema(103, "HwndHost", null, 239, typeof(HwndHost), UIXTypeFlags.Disposable);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(103, "Handle", 116, -1, ExpressionRestriction.ReadOnly, false, null, true, new GetValueHandler(GetHandle), null, false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(103, "ChildHandle", 116, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetChildHandle), new SetValueHandler(SetChildHandle), false);
            Type.Initialize(new DefaultConstructHandler(Construct), null, new PropertySchema[2]
            {
         uixPropertySchema2,
         uixPropertySchema1
            }, null, null, null, null, null, null, null, null, null);
        }
    }
}
