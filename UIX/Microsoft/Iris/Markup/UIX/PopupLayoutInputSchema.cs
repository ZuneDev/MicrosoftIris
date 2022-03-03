// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.PopupLayoutInputSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Layouts;
using Microsoft.Iris.Render;
using Microsoft.Iris.UI;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class PopupLayoutInputSchema
    {
        public static UIXTypeSchema Type;

        private static object GetPlacementTarget(object instanceObj) => ((PopupLayoutInput)instanceObj).PlacementTarget;

        private static void SetPlacementTarget(ref object instanceObj, object valueObj) => ((PopupLayoutInput)instanceObj).PlacementTarget = (ViewItem)valueObj;

        private static object GetPlacement(object instanceObj) => ((PopupLayoutInput)instanceObj).Placement;

        private static void SetPlacement(ref object instanceObj, object valueObj) => ((PopupLayoutInput)instanceObj).Placement = (PlacementMode)valueObj;

        private static object GetOffset(object instanceObj) => ((PopupLayoutInput)instanceObj).Offset;

        private static void SetOffset(ref object instanceObj, object valueObj) => ((PopupLayoutInput)instanceObj).Offset = (Point)valueObj;

        private static object GetStayInBounds(object instanceObj) => BooleanBoxes.Box(((PopupLayoutInput)instanceObj).StayInBounds);

        private static void SetStayInBounds(ref object instanceObj, object valueObj) => ((PopupLayoutInput)instanceObj).StayInBounds = (bool)valueObj;

        private static object GetRespectMenuDropAlignment(object instanceObj) => BooleanBoxes.Box(((PopupLayoutInput)instanceObj).RespectMenuDropAlignment);

        private static void SetRespectMenuDropAlignment(ref object instanceObj, object valueObj) => ((PopupLayoutInput)instanceObj).RespectMenuDropAlignment = (bool)valueObj;

        private static object GetConstrainToTarget(object instanceObj) => BooleanBoxes.Box(((PopupLayoutInput)instanceObj).ConstrainToTarget);

        private static void SetConstrainToTarget(ref object instanceObj, object valueObj) => ((PopupLayoutInput)instanceObj).ConstrainToTarget = (bool)valueObj;

        private static object GetFlippedHorizontally(object instanceObj) => BooleanBoxes.Box(((PopupLayoutInput)instanceObj).FlippedHorizontally);

        private static object GetFlippedVertically(object instanceObj) => BooleanBoxes.Box(((PopupLayoutInput)instanceObj).FlippedVertically);

        private static object Construct() => new PopupLayoutInput();

        public static void Pass1Initialize() => Type = new UIXTypeSchema(162, "PopupLayoutInput", null, 133, typeof(PopupLayoutInput), UIXTypeFlags.None);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(162, "PlacementTarget", 239, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetPlacementTarget), new SetValueHandler(SetPlacementTarget), false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(162, "Placement", 157, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetPlacement), new SetValueHandler(SetPlacement), false);
            UIXPropertySchema uixPropertySchema3 = new UIXPropertySchema(162, "Offset", 158, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetOffset), new SetValueHandler(SetOffset), false);
            UIXPropertySchema uixPropertySchema4 = new UIXPropertySchema(162, "StayInBounds", 15, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetStayInBounds), new SetValueHandler(SetStayInBounds), false);
            UIXPropertySchema uixPropertySchema5 = new UIXPropertySchema(162, "RespectMenuDropAlignment", 15, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetRespectMenuDropAlignment), new SetValueHandler(SetRespectMenuDropAlignment), false);
            UIXPropertySchema uixPropertySchema6 = new UIXPropertySchema(162, "ConstrainToTarget", 15, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetConstrainToTarget), new SetValueHandler(SetConstrainToTarget), false);
            UIXPropertySchema uixPropertySchema7 = new UIXPropertySchema(162, "FlippedHorizontally", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetFlippedHorizontally), null, false);
            UIXPropertySchema uixPropertySchema8 = new UIXPropertySchema(162, "FlippedVertically", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetFlippedVertically), null, false);
            Type.Initialize(new DefaultConstructHandler(Construct), null, new PropertySchema[8]
            {
         uixPropertySchema6,
         uixPropertySchema7,
         uixPropertySchema8,
         uixPropertySchema3,
         uixPropertySchema2,
         uixPropertySchema1,
         uixPropertySchema5,
         uixPropertySchema4
            }, null, null, null, null, null, null, null, null, null);
        }
    }
}
