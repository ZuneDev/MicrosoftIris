// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.MouseWheelHandlerSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.InputHandlers;
using Microsoft.Iris.UI;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class MouseWheelHandlerSchema
    {
        public static UIXTypeSchema Type;

        private static object GetHandle(object instanceObj) => BooleanBoxes.Box(((MouseWheelHandler)instanceObj).Handle);

        private static void SetHandle(ref object instanceObj, object valueObj) => ((MouseWheelHandler)instanceObj).Handle = (bool)valueObj;

        private static object GetHandlerStage(object instanceObj) => ((InputHandler)instanceObj).HandlerStage;

        private static void SetHandlerStage(ref object instanceObj, object valueObj) => ((InputHandler)instanceObj).HandlerStage = (InputHandlerStage)valueObj;

        private static object Construct() => new MouseWheelHandler();

        public static void Pass1Initialize() => Type = new UIXTypeSchema(150, "MouseWheelHandler", null, 110, typeof(MouseWheelHandler), UIXTypeFlags.Disposable);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(150, "Handle", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetHandle), new SetValueHandler(SetHandle), false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(150, "HandlerStage", 112, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetHandlerStage), new SetValueHandler(SetHandlerStage), false);
            UIXEventSchema uixEventSchema1 = new UIXEventSchema(150, "UpInvoked");
            UIXEventSchema uixEventSchema2 = new UIXEventSchema(150, "DownInvoked");
            Type.Initialize(new DefaultConstructHandler(Construct), null, new PropertySchema[2]
            {
         uixPropertySchema1,
         uixPropertySchema2
            }, null, new EventSchema[2]
            {
         uixEventSchema1,
         uixEventSchema2
            }, null, null, null, null, null, null, null);
        }
    }
}
