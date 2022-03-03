// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.AnimationSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Animations;
using Microsoft.Iris.Library;
using Microsoft.Iris.Render;
using Microsoft.Iris.Session;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class AnimationSchema
    {
        public static RangeValidator ValidateLoopValue = new RangeValidator(RangeValidateLoopValue);
        public static UIXTypeSchema Type;

        private static object GetCenterPointPercent(object instanceObj) => ((Animation)instanceObj).CenterPointPercent;

        private static void SetCenterPointPercent(ref object instanceObj, object valueObj) => ((Animation)instanceObj).CenterPointPercent = (Vector3)valueObj;

        private static object GetDisableMouseInput(object instanceObj) => BooleanBoxes.Box(((Animation)instanceObj).DisableMouseInput);

        private static void SetDisableMouseInput(ref object instanceObj, object valueObj) => ((Animation)instanceObj).DisableMouseInput = (bool)valueObj;

        private static object GetKeyframes(object instanceObj) => ((AnimationTemplate)instanceObj).Keyframes;

        private static object GetLoop(object instanceObj) => ((AnimationTemplate)instanceObj).Loop;

        private static void SetLoop(ref object instanceObj, object valueObj)
        {
            Animation animation = (Animation)instanceObj;
            int num = (int)valueObj;
            Result result = ValidateLoopValue(valueObj);
            if (result.Failed)
                ErrorManager.ReportError(result.Error);
            else
                animation.Loop = num;
        }

        private static object GetRotationAxis(object instanceObj) => ((Animation)instanceObj).RotationAxis;

        private static void SetRotationAxis(ref object instanceObj, object valueObj) => ((Animation)instanceObj).RotationAxis = (Vector3)valueObj;

        private static object GetType(object instanceObj) => ((Animation)instanceObj).Type;

        private static void SetType(ref object instanceObj, object valueObj) => ((Animation)instanceObj).Type = (AnimationEventType)valueObj;

        private static object Construct() => new Animation();

        private static Result RangeValidateLoopValue(object value)
        {
            int num = (int)value;
            return num < -1 ? Result.Fail("Expecting a value no smaller than {0}, but got {1}", "-1", num.ToString()) : Result.Success;
        }

        public static void Pass1Initialize() => Type = new UIXTypeSchema(9, "Animation", null, 104, typeof(Animation), UIXTypeFlags.None);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(9, "CenterPointPercent", 234, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetCenterPointPercent), new SetValueHandler(SetCenterPointPercent), false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(9, "DisableMouseInput", 15, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetDisableMouseInput), new SetValueHandler(SetDisableMouseInput), false);
            UIXPropertySchema uixPropertySchema3 = new UIXPropertySchema(9, "Keyframes", 138, 130, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetKeyframes), null, false);
            UIXPropertySchema uixPropertySchema4 = new UIXPropertySchema(9, "Loop", 115, -1, ExpressionRestriction.None, false, ValidateLoopValue, false, new GetValueHandler(GetLoop), new SetValueHandler(SetLoop), false);
            UIXPropertySchema uixPropertySchema5 = new UIXPropertySchema(9, "RotationAxis", 234, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetRotationAxis), new SetValueHandler(SetRotationAxis), false);
            UIXPropertySchema uixPropertySchema6 = new UIXPropertySchema(9, "Type", 10, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetType), new SetValueHandler(SetType), false);
            Type.Initialize(new DefaultConstructHandler(Construct), null, new PropertySchema[6]
            {
         uixPropertySchema1,
         uixPropertySchema2,
         uixPropertySchema3,
         uixPropertySchema4,
         uixPropertySchema5,
         uixPropertySchema6
            }, null, null, null, null, null, null, null, null, null);
        }
    }
}
