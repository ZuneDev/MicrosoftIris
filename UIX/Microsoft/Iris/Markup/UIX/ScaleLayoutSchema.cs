// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.ScaleLayoutSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Layouts;
using Microsoft.Iris.Library;
using Microsoft.Iris.Render;
using Microsoft.Iris.Session;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class ScaleLayoutSchema
    {
        public static UIXTypeSchema Type;

        private static object GetMinimumScale(object instanceObj) => ((ScaleLayout)instanceObj).MinimumScale;

        private static void SetMinimumScale(ref object instanceObj, object valueObj)
        {
            ScaleLayout scaleLayout = (ScaleLayout)instanceObj;
            Vector2 vector2 = (Vector2)valueObj;
            Result result = Vector2Schema.ValidateNotNegative(valueObj);
            if (result.Failed)
                ErrorManager.ReportError(result.Error);
            else
                scaleLayout.MinimumScale = vector2;
        }

        private static object GetMaximumScale(object instanceObj) => ((ScaleLayout)instanceObj).MaximumScale;

        private static void SetMaximumScale(ref object instanceObj, object valueObj)
        {
            ScaleLayout scaleLayout = (ScaleLayout)instanceObj;
            Vector2 vector2 = (Vector2)valueObj;
            Result result = Vector2Schema.ValidateNotNegative(valueObj);
            if (result.Failed)
                ErrorManager.ReportError(result.Error);
            else
                scaleLayout.MaximumScale = vector2;
        }

        private static object GetMaintainAspectRatio(object instanceObj) => BooleanBoxes.Box(((ScaleLayout)instanceObj).MaintainAspectRatio);

        private static void SetMaintainAspectRatio(ref object instanceObj, object valueObj) => ((ScaleLayout)instanceObj).MaintainAspectRatio = (bool)valueObj;

        private static object Construct() => new ScaleLayout();

        public static void Pass1Initialize() => Type = new UIXTypeSchema(179, "ScaleLayout", null, 132, typeof(ScaleLayout), UIXTypeFlags.Immutable);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(179, "MinimumScale", 233, -1, ExpressionRestriction.None, false, Vector2Schema.ValidateNotNegative, false, new GetValueHandler(GetMinimumScale), new SetValueHandler(SetMinimumScale), false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(179, "MaximumScale", 233, -1, ExpressionRestriction.None, false, Vector2Schema.ValidateNotNegative, false, new GetValueHandler(GetMaximumScale), new SetValueHandler(SetMaximumScale), false);
            UIXPropertySchema uixPropertySchema3 = new UIXPropertySchema(179, "MaintainAspectRatio", 15, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetMaintainAspectRatio), new SetValueHandler(SetMaintainAspectRatio), false);
            Type.Initialize(new DefaultConstructHandler(Construct), null, new PropertySchema[3]
            {
         uixPropertySchema3,
         uixPropertySchema2,
         uixPropertySchema1
            }, null, null, null, null, null, null, null, null, null);
        }
    }
}
