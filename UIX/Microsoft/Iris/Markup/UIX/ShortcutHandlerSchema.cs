// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.ShortcutHandlerSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.InputHandlers;
using Microsoft.Iris.ModelItems;
using Microsoft.Iris.UI;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class ShortcutHandlerSchema
    {
        public static UIXTypeSchema Type;

        private static object GetShortcut(object instanceObj) => ((ShortcutHandler)instanceObj).Shortcut;

        private static void SetShortcut(ref object instanceObj, object valueObj) => ((ShortcutHandler)instanceObj).Shortcut = (ShortcutHandlerCommand)valueObj;

        private static object GetCommand(object instanceObj) => ((ShortcutHandler)instanceObj).Command;

        private static void SetCommand(ref object instanceObj, object valueObj) => ((ShortcutHandler)instanceObj).Command = (IUICommand)valueObj;

        private static object GetHandle(object instanceObj) => BooleanBoxes.Box(((ShortcutHandler)instanceObj).Handle);

        private static void SetHandle(ref object instanceObj, object valueObj) => ((ShortcutHandler)instanceObj).Handle = (bool)valueObj;

        private static object GetHandlerStage(object instanceObj) => ((InputHandler)instanceObj).HandlerStage;

        private static void SetHandlerStage(ref object instanceObj, object valueObj) => ((InputHandler)instanceObj).HandlerStage = (InputHandlerStage)valueObj;

        private static object Construct() => new ShortcutHandler();

        public static void Pass1Initialize() => Type = new UIXTypeSchema(192, "ShortcutHandler", null, 110, typeof(ShortcutHandler), UIXTypeFlags.Disposable);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(192, "Shortcut", 193, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetShortcut), new SetValueHandler(SetShortcut), false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(192, "Command", 40, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetCommand), new SetValueHandler(SetCommand), false);
            UIXPropertySchema uixPropertySchema3 = new UIXPropertySchema(192, "Handle", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetHandle), new SetValueHandler(SetHandle), false);
            UIXPropertySchema uixPropertySchema4 = new UIXPropertySchema(192, "HandlerStage", 112, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetHandlerStage), new SetValueHandler(SetHandlerStage), false);
            UIXEventSchema uixEventSchema = new UIXEventSchema(192, "Invoked");
            Type.Initialize(new DefaultConstructHandler(Construct), null, new PropertySchema[4]
            {
         uixPropertySchema2,
         uixPropertySchema3,
         uixPropertySchema4,
         uixPropertySchema1
            }, null, new EventSchema[1]
            {
         uixEventSchema
            }, null, null, null, null, null, null, null);
        }
    }
}
