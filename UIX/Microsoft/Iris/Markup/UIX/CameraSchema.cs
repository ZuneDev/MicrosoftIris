// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.CameraSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Animations;
using Microsoft.Iris.Drawing;
using Microsoft.Iris.Render;
using Microsoft.Iris.Session;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class CameraSchema
    {
        public static UIXTypeSchema Type;

        private static object GetEye(object instanceObj) => ((Camera)instanceObj).Eye;

        private static void SetEye(ref object instanceObj, object valueObj) => ((Camera)instanceObj).Eye = (Vector3)valueObj;

        private static object GetAt(object instanceObj) => ((Camera)instanceObj).At;

        private static void SetAt(ref object instanceObj, object valueObj) => ((Camera)instanceObj).At = (Vector3)valueObj;

        private static object GetUp(object instanceObj) => ((Camera)instanceObj).Up;

        private static void SetUp(ref object instanceObj, object valueObj) => ((Camera)instanceObj).Up = (Vector3)valueObj;

        private static object GetZn(object instanceObj) => ((Camera)instanceObj).Zn;

        private static void SetZn(ref object instanceObj, object valueObj) => ((Camera)instanceObj).Zn = (float)valueObj;

        private static object GetEyeAnimation(object instanceObj) => ((Camera)instanceObj).EyeAnimation;

        private static void SetEyeAnimation(ref object instanceObj, object valueObj) => ((Camera)instanceObj).EyeAnimation = (IAnimationProvider)valueObj;

        private static object GetAtAnimation(object instanceObj) => ((Camera)instanceObj).AtAnimation;

        private static void SetAtAnimation(ref object instanceObj, object valueObj) => ((Camera)instanceObj).AtAnimation = (IAnimationProvider)valueObj;

        private static object GetUpAnimation(object instanceObj) => ((Camera)instanceObj).UpAnimation;

        private static void SetUpAnimation(ref object instanceObj, object valueObj) => ((Camera)instanceObj).UpAnimation = (IAnimationProvider)valueObj;

        private static object GetZnAnimation(object instanceObj) => ((Camera)instanceObj).ZnAnimation;

        private static void SetZnAnimation(ref object instanceObj, object valueObj) => ((Camera)instanceObj).ZnAnimation = (IAnimationProvider)valueObj;

        private static object GetPerspective(object instanceObj) => BooleanBoxes.Box(((Camera)instanceObj).Perspective);

        private static void SetPerspective(ref object instanceObj, object valueObj) => ((Camera)instanceObj).Perspective = (bool)valueObj;

        private static object Construct() => new Camera();

        private static object CallPlayAnimationIAnimation(object instanceObj, object[] parameters)
        {
            Camera camera = (Camera)instanceObj;
            IAnimationProvider parameter = (IAnimationProvider)parameters[0];
            if (parameter == null)
            {
                ErrorManager.ReportError("Script runtime failure: Invalid 'null' value for '{0}'", "animation");
                return null;
            }
            camera.PlayAnimation(parameter, null);
            return null;
        }

        private static object CallPlayAnimationIAnimationAnimationHandle(
          object instanceObj,
          object[] parameters)
        {
            Camera camera = (Camera)instanceObj;
            IAnimationProvider parameter1 = (IAnimationProvider)parameters[0];
            AnimationHandle parameter2 = (AnimationHandle)parameters[1];
            if (parameter1 == null)
            {
                ErrorManager.ReportError("Script runtime failure: Invalid 'null' value for '{0}'", "animation");
                return null;
            }
            if (parameter2 == null)
            {
                ErrorManager.ReportError("Script runtime failure: Invalid 'null' value for '{0}'", "handle");
                return null;
            }
            camera.PlayAnimation(parameter1, parameter2);
            return null;
        }

        public static void Pass1Initialize() => Type = new UIXTypeSchema(21, "Camera", null, 153, typeof(Camera), UIXTypeFlags.None);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(21, "Eye", 234, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetEye), new SetValueHandler(SetEye), false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(21, "At", 234, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetAt), new SetValueHandler(SetAt), false);
            UIXPropertySchema uixPropertySchema3 = new UIXPropertySchema(21, "Up", 234, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetUp), new SetValueHandler(SetUp), false);
            UIXPropertySchema uixPropertySchema4 = new UIXPropertySchema(21, "Zn", 194, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetZn), new SetValueHandler(SetZn), false);
            UIXPropertySchema uixPropertySchema5 = new UIXPropertySchema(21, "EyeAnimation", 104, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetEyeAnimation), new SetValueHandler(SetEyeAnimation), false);
            UIXPropertySchema uixPropertySchema6 = new UIXPropertySchema(21, "AtAnimation", 104, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetAtAnimation), new SetValueHandler(SetAtAnimation), false);
            UIXPropertySchema uixPropertySchema7 = new UIXPropertySchema(21, "UpAnimation", 104, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetUpAnimation), new SetValueHandler(SetUpAnimation), false);
            UIXPropertySchema uixPropertySchema8 = new UIXPropertySchema(21, "ZnAnimation", 104, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetZnAnimation), new SetValueHandler(SetZnAnimation), false);
            UIXPropertySchema uixPropertySchema9 = new UIXPropertySchema(21, "Perspective", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetPerspective), new SetValueHandler(SetPerspective), false);
            UIXMethodSchema uixMethodSchema1 = new UIXMethodSchema(21, "PlayAnimation", new short[1]
            {
         104
            }, 240, new InvokeHandler(CallPlayAnimationIAnimation), false);
            UIXMethodSchema uixMethodSchema2 = new UIXMethodSchema(21, "PlayAnimation", new short[2]
            {
         104,
         11
            }, 240, new InvokeHandler(CallPlayAnimationIAnimationAnimationHandle), false);
            Type.Initialize(new DefaultConstructHandler(Construct), null, new PropertySchema[9]
            {
         uixPropertySchema2,
         uixPropertySchema6,
         uixPropertySchema1,
         uixPropertySchema5,
         uixPropertySchema9,
         uixPropertySchema3,
         uixPropertySchema7,
         uixPropertySchema4,
         uixPropertySchema8
            }, new MethodSchema[2]
            {
         uixMethodSchema1,
         uixMethodSchema2
            }, null, null, null, null, null, null, null, null);
        }
    }
}
