// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.RepeaterSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.UI;
using Microsoft.Iris.ViewItems;
using System.Collections;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class RepeaterSchema
    {
        public static UIXTypeSchema Type;

        private static object GetContentName(object instanceObj) => ((Repeater)instanceObj).ContentName;

        private static void SetContentName(ref object instanceObj, object valueObj) => ((Repeater)instanceObj).ContentName = (string)valueObj;

        private static object GetDividerName(object instanceObj) => ((Repeater)instanceObj).DividerName;

        private static void SetDividerName(ref object instanceObj, object valueObj) => ((Repeater)instanceObj).DividerName = (string)valueObj;

        private static object GetSource(object instanceObj) => ((Repeater)instanceObj).Source;

        private static void SetSource(ref object instanceObj, object valueObj) => ((Repeater)instanceObj).Source = (IList)valueObj;

        private static object GetDefaultFocusIndex(object instanceObj) => ((Repeater)instanceObj).DefaultFocusIndex;

        private static void SetDefaultFocusIndex(ref object instanceObj, object valueObj) => ((Repeater)instanceObj).DefaultFocusIndex = (int)valueObj;

        private static void SetContent(ref object instanceObj, object valueObj)
        {
            Repeater repeater = (Repeater)instanceObj;
        }

        private static void SetDivider(ref object instanceObj, object valueObj)
        {
            Repeater repeater = (Repeater)instanceObj;
        }

        private static object GetDiscardOffscreenVisuals(object instanceObj) => BooleanBoxes.Box(((ViewItem)instanceObj).DiscardOffscreenVisuals);

        private static void SetDiscardOffscreenVisuals(ref object instanceObj, object valueObj) => ((ViewItem)instanceObj).DiscardOffscreenVisuals = (bool)valueObj;

        private static object GetContentSelectors(object instanceObj) => ((Repeater)instanceObj).ContentSelectors;

        private static object GetMaintainFocusedItemOnSourceChanges(object instanceObj) => BooleanBoxes.Box(((Repeater)instanceObj).MaintainFocusedItemOnSourceChanges);

        private static void SetMaintainFocusedItemOnSourceChanges(
          ref object instanceObj,
          object valueObj)
        {
            ((Repeater)instanceObj).MaintainFocusedItemOnSourceChanges = (bool)valueObj;
        }

        private static object Construct() => new Repeater();

        private static object CallNavigateIntoIndexInt32(object instanceObj, object[] parameters)
        {
            ((Repeater)instanceObj).NavigateIntoIndex((int)parameters[0]);
            return null;
        }

        private static object CallScrollIndexIntoViewInt32(object instanceObj, object[] parameters)
        {
            ((Repeater)instanceObj).ScrollIndexIntoView((int)parameters[0]);
            return null;
        }

        public static void Pass1Initialize() => Type = new UIXTypeSchema(173, "Repeater", null, 239, typeof(Repeater), UIXTypeFlags.Disposable);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(173, "ContentName", 208, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetContentName), new SetValueHandler(SetContentName), false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(173, "DividerName", 208, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetDividerName), new SetValueHandler(SetDividerName), false);
            UIXPropertySchema uixPropertySchema3 = new UIXPropertySchema(173, "Source", 138, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetSource), new SetValueHandler(SetSource), false);
            UIXPropertySchema uixPropertySchema4 = new UIXPropertySchema(173, "DefaultFocusIndex", 115, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetDefaultFocusIndex), new SetValueHandler(SetDefaultFocusIndex), false);
            UIXPropertySchema uixPropertySchema5 = new UIXPropertySchema(173, "Content", 239, -1, ExpressionRestriction.NoAccess, false, null, true, null, new SetValueHandler(SetContent), false);
            UIXPropertySchema uixPropertySchema6 = new UIXPropertySchema(173, "Divider", 239, -1, ExpressionRestriction.NoAccess, false, null, true, null, new SetValueHandler(SetDivider), false);
            UIXPropertySchema uixPropertySchema7 = new UIXPropertySchema(173, "DiscardOffscreenVisuals", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetDiscardOffscreenVisuals), new SetValueHandler(SetDiscardOffscreenVisuals), false);
            UIXPropertySchema uixPropertySchema8 = new UIXPropertySchema(173, "ContentSelectors", 138, 227, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetContentSelectors), null, false);
            UIXPropertySchema uixPropertySchema9 = new UIXPropertySchema(173, "MaintainFocusedItemOnSourceChanges", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetMaintainFocusedItemOnSourceChanges), new SetValueHandler(SetMaintainFocusedItemOnSourceChanges), false);
            UIXMethodSchema uixMethodSchema1 = new UIXMethodSchema(173, "NavigateIntoIndex", new short[1]
            {
         115
            }, 240, new InvokeHandler(CallNavigateIntoIndexInt32), false);
            UIXMethodSchema uixMethodSchema2 = new UIXMethodSchema(173, "ScrollIndexIntoView", new short[1]
            {
         115
            }, 240, new InvokeHandler(CallScrollIndexIntoViewInt32), false);
            UIXEventSchema uixEventSchema = new UIXEventSchema(173, "FocusedItemDiscarded");
            Type.Initialize(new DefaultConstructHandler(Construct), null, new PropertySchema[9]
            {
         uixPropertySchema5,
         uixPropertySchema1,
         uixPropertySchema8,
         uixPropertySchema4,
         uixPropertySchema7,
         uixPropertySchema6,
         uixPropertySchema2,
         uixPropertySchema9,
         uixPropertySchema3
            }, new MethodSchema[2]
            {
         uixMethodSchema1,
         uixMethodSchema2
            }, new EventSchema[1] { uixEventSchema }, null, null, null, null, null, null, null);
        }
    }
}
