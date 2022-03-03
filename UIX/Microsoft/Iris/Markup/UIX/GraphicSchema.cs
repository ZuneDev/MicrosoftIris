// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.GraphicSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Drawing;
using Microsoft.Iris.Layouts;
using Microsoft.Iris.UI;
using Microsoft.Iris.ViewItems;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class GraphicSchema
    {
        public static UIXTypeSchema Type;

        private static object GetChildren(object instanceObj) => ViewItemSchema.ListProxy.GetChildren((ViewItem)instanceObj);

        private static object GetContent(object instanceObj) => ((Graphic)instanceObj).Content;

        private static void SetContent(ref object instanceObj, object valueObj) => ((Graphic)instanceObj).Content = (UIImage)valueObj;

        private static object GetPreloadContent(object instanceObj) => ((Graphic)instanceObj).PreloadContent;

        private static void SetPreloadContent(ref object instanceObj, object valueObj) => ((Graphic)instanceObj).PreloadContent = (UIImage)valueObj;

        private static object GetEffect(object instanceObj) => ((ViewItem)instanceObj).Effect;

        private static void SetEffect(ref object instanceObj, object valueObj) => ((ViewItem)instanceObj).Effect = (EffectClass)valueObj;

        private static object GetAcquiringImage(object instanceObj) => ((Graphic)instanceObj).AcquiringImage;

        private static void SetAcquiringImage(ref object instanceObj, object valueObj) => ((Graphic)instanceObj).AcquiringImage = (UIImage)valueObj;

        private static object GetErrorImage(object instanceObj) => ((Graphic)instanceObj).ErrorImage;

        private static void SetErrorImage(ref object instanceObj, object valueObj) => ((Graphic)instanceObj).ErrorImage = (UIImage)valueObj;

        private static object GetSizingPolicy(object instanceObj) => ((Graphic)instanceObj).SizingPolicy;

        private static void SetSizingPolicy(ref object instanceObj, object valueObj) => ((Graphic)instanceObj).SizingPolicy = (SizingPolicy)valueObj;

        private static object GetStretchingPolicy(object instanceObj) => ((Graphic)instanceObj).StretchingPolicy;

        private static void SetStretchingPolicy(ref object instanceObj, object valueObj) => ((Graphic)instanceObj).StretchingPolicy = (StretchingPolicy)valueObj;

        private static object GetHorizontalAlignment(object instanceObj) => ((Graphic)instanceObj).HorizontalAlignment;

        private static void SetHorizontalAlignment(ref object instanceObj, object valueObj) => ((Graphic)instanceObj).HorizontalAlignment = (StripAlignment)valueObj;

        private static object GetVerticalAlignment(object instanceObj) => ((Graphic)instanceObj).VerticalAlignment;

        private static void SetVerticalAlignment(ref object instanceObj, object valueObj) => ((Graphic)instanceObj).VerticalAlignment = (StripAlignment)valueObj;

        private static object Construct() => new Graphic();

        private static object CallCommitPreload(object instanceObj, object[] parameters)
        {
            ((Graphic)instanceObj).CommitPreload();
            return null;
        }

        public static void Pass1Initialize() => Type = new UIXTypeSchema(97, "Graphic", null, 239, typeof(Graphic), UIXTypeFlags.Disposable);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(97, "Children", 138, 239, ExpressionRestriction.NoAccess, false, null, true, new GetValueHandler(GetChildren), null, false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(97, "Content", 105, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetContent), new SetValueHandler(SetContent), false);
            UIXPropertySchema uixPropertySchema3 = new UIXPropertySchema(97, "PreloadContent", 105, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetPreloadContent), new SetValueHandler(SetPreloadContent), false);
            UIXPropertySchema uixPropertySchema4 = new UIXPropertySchema(97, "Effect", 78, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetEffect), new SetValueHandler(SetEffect), false);
            UIXPropertySchema uixPropertySchema5 = new UIXPropertySchema(97, "AcquiringImage", 105, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetAcquiringImage), new SetValueHandler(SetAcquiringImage), false);
            UIXPropertySchema uixPropertySchema6 = new UIXPropertySchema(97, "ErrorImage", 105, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetErrorImage), new SetValueHandler(SetErrorImage), false);
            UIXPropertySchema uixPropertySchema7 = new UIXPropertySchema(97, "SizingPolicy", 199, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetSizingPolicy), new SetValueHandler(SetSizingPolicy), false);
            UIXPropertySchema uixPropertySchema8 = new UIXPropertySchema(97, "StretchingPolicy", 207, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetStretchingPolicy), new SetValueHandler(SetStretchingPolicy), false);
            UIXPropertySchema uixPropertySchema9 = new UIXPropertySchema(97, "HorizontalAlignment", 209, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetHorizontalAlignment), new SetValueHandler(SetHorizontalAlignment), false);
            UIXPropertySchema uixPropertySchema10 = new UIXPropertySchema(97, "VerticalAlignment", 209, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetVerticalAlignment), new SetValueHandler(SetVerticalAlignment), false);
            UIXMethodSchema uixMethodSchema = new UIXMethodSchema(97, "CommitPreload", null, 240, new InvokeHandler(CallCommitPreload), false);
            Type.Initialize(new DefaultConstructHandler(Construct), null, new PropertySchema[10]
            {
         uixPropertySchema5,
         uixPropertySchema1,
         uixPropertySchema2,
         uixPropertySchema4,
         uixPropertySchema6,
         uixPropertySchema9,
         uixPropertySchema3,
         uixPropertySchema7,
         uixPropertySchema8,
         uixPropertySchema10
            }, new MethodSchema[1]
            {
         uixMethodSchema
            }, null, null, null, null, null, null, null, null);
        }
    }
}
