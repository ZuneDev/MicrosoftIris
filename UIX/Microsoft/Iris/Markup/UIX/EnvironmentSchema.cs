// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.EnvironmentSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Render;
using Microsoft.Iris.Session;
using Microsoft.Iris.UI;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class EnvironmentSchema
    {
        public static UIXTypeSchema Type;

        private static object GetIsRightToLeft(object instanceObj) => BooleanBoxes.Box(((Environment)instanceObj).IsRightToLeft);

        private static object GetColorScheme(object instanceObj) => ((Environment)instanceObj).ColorScheme;

        private static object GetAnimationSpeed(object instanceObj) => ((Environment)instanceObj).AnimationSpeed;

        private static void SetAnimationSpeed(ref object instanceObj, object valueObj) => ((Environment)instanceObj).AnimationSpeed = (float)valueObj;

        private static object GetAnimationUpdatesPerSecond(object instanceObj) => ((Environment)instanceObj).AnimationUpdatesPerSecond;

        private static void SetAnimationUpdatesPerSecond(ref object instanceObj, object valueObj) => ((Environment)instanceObj).AnimationUpdatesPerSecond = (int)valueObj;

        private static object GetDpiScale(object instanceObj) => Environment.DpiScale;

        private static object GetGraphicsDeviceType(object instanceObj)
        {
            RenderingType renderingType;
            switch (UISession.Default.RenderSession.GraphicsDevice.DeviceType)
            {
                case GraphicsDeviceType.Gdi:
                    renderingType = RenderingType.GDI;
                    break;
                case GraphicsDeviceType.Direct3D9:
                case GraphicsDeviceType.XeDirectX9:
                    renderingType = RenderingType.DX9;
                    break;
                default:
                    renderingType = RenderingType.Default;
                    break;
            }
            return renderingType;
        }

        private static object Construct() => Environment.Instance;

        private static object CallAnimationAdvanceInt32(object instanceObj, object[] parameters)
        {
            ((Environment)instanceObj).AnimationAdvance((int)parameters[0]);
            return null;
        }

        public static void Pass1Initialize() => Type = new UIXTypeSchema(87, "Environment", null, 153, typeof(Environment), UIXTypeFlags.None);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(87, "IsRightToLeft", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetIsRightToLeft), null, false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(87, "ColorScheme", 39, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetColorScheme), null, false);
            UIXPropertySchema uixPropertySchema3 = new UIXPropertySchema(87, "AnimationSpeed", 194, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetAnimationSpeed), new SetValueHandler(SetAnimationSpeed), false);
            UIXPropertySchema uixPropertySchema4 = new UIXPropertySchema(87, "AnimationUpdatesPerSecond", 115, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetAnimationUpdatesPerSecond), new SetValueHandler(SetAnimationUpdatesPerSecond), false);
            UIXPropertySchema uixPropertySchema5 = new UIXPropertySchema(87, "DpiScale", 194, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetDpiScale), null, true);
            UIXPropertySchema uixPropertySchema6 = new UIXPropertySchema(87, "GraphicsDeviceType", 98, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetGraphicsDeviceType), null, true);
            UIXMethodSchema uixMethodSchema = new UIXMethodSchema(87, "AnimationAdvance", new short[1]
            {
         115
            }, 240, new InvokeHandler(CallAnimationAdvanceInt32), false);
            Type.Initialize(new DefaultConstructHandler(Construct), null, new PropertySchema[6]
            {
         uixPropertySchema3,
         uixPropertySchema4,
         uixPropertySchema2,
         uixPropertySchema5,
         uixPropertySchema6,
         uixPropertySchema1
            }, new MethodSchema[1]
            {
         uixMethodSchema
            }, null, null, null, null, null, null, null, null);
        }
    }
}
