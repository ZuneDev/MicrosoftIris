// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.BooleanChoiceSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.ModelItems;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class BooleanChoiceSchema
    {
        public static UIXTypeSchema Type;

        private static object GetValue(object instanceObj) => BooleanBoxes.Box(((IUIChoice)instanceObj).ChosenIndex == 1);

        private static void SetValue(ref object instanceObj, object valueObj)
        {
            IUIBooleanChoice uiBooleanChoice = (IUIBooleanChoice)instanceObj;
            if ((bool)valueObj)
                uiBooleanChoice.ChosenIndex = 1;
            else
                uiBooleanChoice.ChosenIndex = 0;
        }

        private static object Construct() => new Microsoft.Iris.ModelItems.BooleanChoice();

        public static void Pass1Initialize() => Type = new UIXTypeSchema(16, "BooleanChoice", null, 28, typeof(IUIBooleanChoice), UIXTypeFlags.Disposable);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema = new UIXPropertySchema(16, "Value", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetValue), new SetValueHandler(SetValue), false);
            Type.Initialize(new DefaultConstructHandler(Construct), null, new PropertySchema[1]
            {
         uixPropertySchema
            }, null, null, null, null, null, null, null, null, null);
        }
    }
}
