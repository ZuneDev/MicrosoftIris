// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.SwitchAnimationSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Animations;
using Microsoft.Iris.ModelItems;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class SwitchAnimationSchema
    {
        public static UIXTypeSchema Type;

        private static object GetExpression(object instanceObj) => ((SwitchAnimation)instanceObj).Expression;

        private static void SetExpression(ref object instanceObj, object valueObj) => ((SwitchAnimation)instanceObj).Expression = (IUIValueRange)valueObj;

        private static object GetOptions(object instanceObj) => ((SwitchAnimation)instanceObj).Options;

        private static object GetType(object instanceObj) => ((SwitchAnimation)instanceObj).Type;

        private static void SetType(ref object instanceObj, object valueObj) => ((SwitchAnimation)instanceObj).Type = (AnimationEventType)valueObj;

        private static object Construct() => new SwitchAnimation();

        public static void Pass1Initialize() => Type = new UIXTypeSchema(210, "SwitchAnimation", null, 104, typeof(SwitchAnimation), UIXTypeFlags.None);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(210, "Expression", 231, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetExpression), new SetValueHandler(SetExpression), false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(210, "Options", 58, -1, ExpressionRestriction.NoAccess, false, null, true, new GetValueHandler(GetOptions), null, false);
            UIXPropertySchema uixPropertySchema3 = new UIXPropertySchema(210, "Type", 10, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetType), new SetValueHandler(SetType), false);
            Type.Initialize(new DefaultConstructHandler(Construct), null, new PropertySchema[3]
            {
         uixPropertySchema1,
         uixPropertySchema2,
         uixPropertySchema3
            }, null, null, null, null, null, null, null, null, null);
        }
    }
}
