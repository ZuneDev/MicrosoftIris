// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.CommandSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.ModelItems;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class CommandSchema
    {
        public static UIXTypeSchema Type;

        private static object GetAvailable(object instanceObj) => BooleanBoxes.Box(((IUICommand)instanceObj).Available);

        private static void SetAvailable(ref object instanceObj, object valueObj) => ((IUICommand)instanceObj).Available = (bool)valueObj;

        private static object GetPriority(object instanceObj) => ((IUICommand)instanceObj).Priority;

        private static void SetPriority(ref object instanceObj, object valueObj) => ((IUICommand)instanceObj).Priority = (InvokePriority)valueObj;

        private static object Construct() => new UICommand();

        private static object CallInvoke(object instanceObj, object[] parameters)
        {
            ((IUICommand)instanceObj).Invoke();
            return null;
        }

        public static void Pass1Initialize() => Type = new UIXTypeSchema(40, "Command", null, 153, typeof(IUICommand), UIXTypeFlags.None);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(40, "Available", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetAvailable), new SetValueHandler(SetAvailable), false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(40, "Priority", 126, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetPriority), new SetValueHandler(SetPriority), false);
            UIXEventSchema uixEventSchema = new UIXEventSchema(40, "Invoked");
            UIXMethodSchema uixMethodSchema = new UIXMethodSchema(40, "Invoke", null, 240, new InvokeHandler(CallInvoke), false);
            Type.Initialize(new DefaultConstructHandler(Construct), null, new PropertySchema[2]
            {
         uixPropertySchema1,
         uixPropertySchema2
            }, new MethodSchema[1]
            {
         uixMethodSchema
            }, new EventSchema[1] { uixEventSchema }, null, null, null, null, null, null, null);
        }
    }
}
