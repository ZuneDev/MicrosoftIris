// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.ClipSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Drawing;
using Microsoft.Iris.Library;
using Microsoft.Iris.Render;
using Microsoft.Iris.Session;
using Microsoft.Iris.UI;
using Microsoft.Iris.ViewItems;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class ClipSchema
    {
        public static UIXTypeSchema Type;

        private static object GetChildren(object instanceObj) => ViewItemSchema.ListProxy.GetChildren((ViewItem)instanceObj);

        private static object GetOrientation(object instanceObj) => ((Clip)instanceObj).Orientation;

        private static void SetOrientation(ref object instanceObj, object valueObj) => ((Clip)instanceObj).Orientation = (Orientation)valueObj;

        private static object GetFadeSize(object instanceObj) => ((Clip)instanceObj).FadeSize;

        private static void SetFadeSize(ref object instanceObj, object valueObj) => ((Clip)instanceObj).FadeSize = (float)valueObj;

        private static object GetNearOffset(object instanceObj) => ((Clip)instanceObj).NearOffset;

        private static void SetNearOffset(ref object instanceObj, object valueObj) => ((Clip)instanceObj).NearOffset = (float)valueObj;

        private static object GetFarOffset(object instanceObj) => ((Clip)instanceObj).FarOffset;

        private static void SetFarOffset(ref object instanceObj, object valueObj) => ((Clip)instanceObj).FarOffset = (float)valueObj;

        private static object GetNearPercent(object instanceObj) => ((Clip)instanceObj).NearPercent;

        private static void SetNearPercent(ref object instanceObj, object valueObj) => ((Clip)instanceObj).NearPercent = (float)valueObj;

        private static object GetFarPercent(object instanceObj) => ((Clip)instanceObj).FarPercent;

        private static void SetFarPercent(ref object instanceObj, object valueObj) => ((Clip)instanceObj).FarPercent = (float)valueObj;

        private static object GetShowNear(object instanceObj) => BooleanBoxes.Box(((Clip)instanceObj).ShowNear);

        private static void SetShowNear(ref object instanceObj, object valueObj) => ((Clip)instanceObj).ShowNear = (bool)valueObj;

        private static object GetShowFar(object instanceObj) => BooleanBoxes.Box(((Clip)instanceObj).ShowFar);

        private static void SetShowFar(ref object instanceObj, object valueObj) => ((Clip)instanceObj).ShowFar = (bool)valueObj;

        private static object GetColorMask(object instanceObj) => ((Clip)instanceObj).ColorMask;

        private static void SetColorMask(ref object instanceObj, object valueObj) => ((Clip)instanceObj).ColorMask = (Color)valueObj;

        private static object GetFadeAmount(object instanceObj) => ((Clip)instanceObj).FadeAmount;

        private static void SetFadeAmount(ref object instanceObj, object valueObj)
        {
            Clip clip = (Clip)instanceObj;
            float num = (float)valueObj;
            Result result = SingleSchema.Validate0to1(valueObj);
            if (result.Failed)
                ErrorManager.ReportError(result.Error);
            else
                clip.FadeAmount = num;
        }

        private static object Construct() => new Clip();

        public static void Pass1Initialize() => Type = new UIXTypeSchema(34, "Clip", null, 239, typeof(Clip), UIXTypeFlags.Disposable);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(34, "Children", 138, 239, ExpressionRestriction.NoAccess, false, null, true, new GetValueHandler(GetChildren), null, false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(34, "Orientation", 154, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetOrientation), new SetValueHandler(SetOrientation), false);
            UIXPropertySchema uixPropertySchema3 = new UIXPropertySchema(34, "FadeSize", 194, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetFadeSize), new SetValueHandler(SetFadeSize), false);
            UIXPropertySchema uixPropertySchema4 = new UIXPropertySchema(34, "NearOffset", 194, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetNearOffset), new SetValueHandler(SetNearOffset), false);
            UIXPropertySchema uixPropertySchema5 = new UIXPropertySchema(34, "FarOffset", 194, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetFarOffset), new SetValueHandler(SetFarOffset), false);
            UIXPropertySchema uixPropertySchema6 = new UIXPropertySchema(34, "NearPercent", 194, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetNearPercent), new SetValueHandler(SetNearPercent), false);
            UIXPropertySchema uixPropertySchema7 = new UIXPropertySchema(34, "FarPercent", 194, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetFarPercent), new SetValueHandler(SetFarPercent), false);
            UIXPropertySchema uixPropertySchema8 = new UIXPropertySchema(34, "ShowNear", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetShowNear), new SetValueHandler(SetShowNear), false);
            UIXPropertySchema uixPropertySchema9 = new UIXPropertySchema(34, "ShowFar", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetShowFar), new SetValueHandler(SetShowFar), false);
            UIXPropertySchema uixPropertySchema10 = new UIXPropertySchema(34, "ColorMask", 35, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetColorMask), new SetValueHandler(SetColorMask), false);
            UIXPropertySchema uixPropertySchema11 = new UIXPropertySchema(34, "FadeAmount", 194, -1, ExpressionRestriction.None, false, SingleSchema.Validate0to1, true, new GetValueHandler(GetFadeAmount), new SetValueHandler(SetFadeAmount), false);
            Type.Initialize(new DefaultConstructHandler(Construct), null, new PropertySchema[11]
            {
         uixPropertySchema1,
         uixPropertySchema10,
         uixPropertySchema11,
         uixPropertySchema3,
         uixPropertySchema5,
         uixPropertySchema7,
         uixPropertySchema4,
         uixPropertySchema6,
         uixPropertySchema2,
         uixPropertySchema9,
         uixPropertySchema8
            }, null, null, null, null, null, null, null, null, null);
        }
    }
}
