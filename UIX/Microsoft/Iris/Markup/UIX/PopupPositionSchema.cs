// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.PopupPositionSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Layouts;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class PopupPositionSchema
    {
        public static UIXTypeSchema Type;

        private static object GetTarget(object instanceObj) => ((PopupPosition)instanceObj).Target;

        private static void SetTarget(ref object instanceObj, object valueObj)
        {
            PopupPosition popupPosition = (PopupPosition)instanceObj;
            InterestPoint interestPoint = (InterestPoint)valueObj;
            popupPosition.Target = interestPoint;
            instanceObj = popupPosition;
        }

        private static object GetPopup(object instanceObj) => ((PopupPosition)instanceObj).Popup;

        private static void SetPopup(ref object instanceObj, object valueObj)
        {
            PopupPosition popupPosition = (PopupPosition)instanceObj;
            InterestPoint interestPoint = (InterestPoint)valueObj;
            popupPosition.Popup = interestPoint;
            instanceObj = popupPosition;
        }

        private static object GetFlipped(object instanceObj) => ((PopupPosition)instanceObj).Flipped;

        private static void SetFlipped(ref object instanceObj, object valueObj)
        {
            PopupPosition popupPosition = (PopupPosition)instanceObj;
            FlipDirection flipDirection = (FlipDirection)valueObj;
            popupPosition.Flipped = flipDirection;
            instanceObj = popupPosition;
        }

        private static object Construct() => new PopupPosition();

        public static void Pass1Initialize() => Type = new UIXTypeSchema(163, "PopupPosition", null, 153, typeof(PopupPosition), UIXTypeFlags.Immutable);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(163, "Target", 118, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetTarget), new SetValueHandler(SetTarget), false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(163, "Popup", 118, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetPopup), new SetValueHandler(SetPopup), false);
            UIXPropertySchema uixPropertySchema3 = new UIXPropertySchema(163, "Flipped", 89, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetFlipped), new SetValueHandler(SetFlipped), false);
            Type.Initialize(new DefaultConstructHandler(Construct), null, new PropertySchema[3]
            {
         uixPropertySchema3,
         uixPropertySchema2,
         uixPropertySchema1
            }, null, null, null, null, null, null, null, null, null);
        }
    }
}
