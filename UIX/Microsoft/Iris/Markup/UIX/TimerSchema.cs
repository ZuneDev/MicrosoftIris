// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.TimerSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;
using Microsoft.Iris.ModelItems;
using Microsoft.Iris.Session;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class TimerSchema
    {
        public static UIXTypeSchema Type;

        private static object GetInterval(object instanceObj) => ((UITimer)instanceObj).Interval;

        private static void SetInterval(ref object instanceObj, object valueObj)
        {
            UITimer uiTimer = (UITimer)instanceObj;
            int num = (int)valueObj;
            Result result = Int32Schema.ValidateNotNegative(valueObj);
            if (result.Failed)
                ErrorManager.ReportError(result.Error);
            else
                uiTimer.Interval = num;
        }

        private static object GetEnabled(object instanceObj) => BooleanBoxes.Box(((UITimer)instanceObj).Enabled);

        private static void SetEnabled(ref object instanceObj, object valueObj) => ((UITimer)instanceObj).Enabled = (bool)valueObj;

        private static object GetAutoRepeat(object instanceObj) => BooleanBoxes.Box(((UITimer)instanceObj).AutoRepeat);

        private static void SetAutoRepeat(ref object instanceObj, object valueObj) => ((UITimer)instanceObj).AutoRepeat = (bool)valueObj;

        private static object Construct() => new UITimer();

        private static object CallStart(object instanceObj, object[] parameters)
        {
            ((UITimer)instanceObj).Start();
            return null;
        }

        private static object CallStop(object instanceObj, object[] parameters)
        {
            ((UITimer)instanceObj).Stop();
            return null;
        }

        public static void Pass1Initialize() => Type = new UIXTypeSchema(221, "Timer", null, 153, typeof(UITimer), UIXTypeFlags.Disposable);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(221, "Interval", 115, -1, ExpressionRestriction.None, false, Int32Schema.ValidateNotNegative, true, new GetValueHandler(GetInterval), new SetValueHandler(SetInterval), false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(221, "Enabled", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetEnabled), new SetValueHandler(SetEnabled), false);
            UIXPropertySchema uixPropertySchema3 = new UIXPropertySchema(221, "AutoRepeat", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetAutoRepeat), new SetValueHandler(SetAutoRepeat), false);
            UIXEventSchema uixEventSchema = new UIXEventSchema(221, "Tick");
            UIXMethodSchema uixMethodSchema1 = new UIXMethodSchema(221, "Start", null, 240, new InvokeHandler(CallStart), false);
            UIXMethodSchema uixMethodSchema2 = new UIXMethodSchema(221, "Stop", null, 240, new InvokeHandler(CallStop), false);
            Type.Initialize(new DefaultConstructHandler(Construct), null, new PropertySchema[3]
            {
         uixPropertySchema3,
         uixPropertySchema2,
         uixPropertySchema1
            }, new MethodSchema[2]
            {
         uixMethodSchema1,
         uixMethodSchema2
            }, new EventSchema[1] { uixEventSchema }, null, null, null, null, null, null, null);
        }
    }
}
