// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.InputHandlerSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.UI;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class InputHandlerSchema
    {
        public static UIXTypeSchema Type;

        private static object GetName(object instanceObj) => ((InputHandler)instanceObj).Name;

        private static void SetName(ref object instanceObj, object valueObj) => ((InputHandler)instanceObj).Name = (string)valueObj;

        private static object GetEnabled(object instanceObj) => BooleanBoxes.Box(((InputHandler)instanceObj).Enabled);

        private static void SetEnabled(ref object instanceObj, object valueObj) => ((InputHandler)instanceObj).Enabled = (bool)valueObj;

        public static void Pass1Initialize() => Type = new UIXTypeSchema(110, "InputHandler", null, -1, typeof(InputHandler), UIXTypeFlags.Disposable);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(110, "Name", 208, -1, ExpressionRestriction.NoAccess, false, null, true, new GetValueHandler(GetName), new SetValueHandler(SetName), false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(110, "Enabled", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetEnabled), new SetValueHandler(SetEnabled), false);
            Type.Initialize(null, null, new PropertySchema[2]
            {
         uixPropertySchema2,
         uixPropertySchema1
            }, null, null, null, null, null, null, null, null, null);
        }
    }
}
