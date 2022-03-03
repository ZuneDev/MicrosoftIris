// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.PointLight2DSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Drawing;
using Microsoft.Iris.Library;
using Microsoft.Iris.Render;
using Microsoft.Iris.Session;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class PointLight2DSchema
    {
        public static UIXTypeSchema Type;

        private static object GetPosition(object instanceObj) => ((PointLight2DElement)instanceObj).Position;

        private static void SetPosition(ref object instanceObj, object valueObj) => ((PointLight2DElement)instanceObj).Position = (Vector3)valueObj;

        private static object GetRadius(object instanceObj) => ((PointLight2DElement)instanceObj).Radius;

        private static void SetRadius(ref object instanceObj, object valueObj)
        {
            PointLight2DElement pointLight2Delement = (PointLight2DElement)instanceObj;
            float num = (float)valueObj;
            Result result = SingleSchema.ValidateNotNegative(valueObj);
            if (result.Failed)
                ErrorManager.ReportError(result.Error);
            else
                pointLight2Delement.Radius = num;
        }

        private static void SetLightColor(ref object instanceObj, object valueObj) => ((PointLight2DElement)instanceObj).LightColor = ((Color)valueObj).RenderConvert();

        private static void SetAmbientColor(ref object instanceObj, object valueObj) => ((PointLight2DElement)instanceObj).AmbientColor = ((Color)valueObj).RenderConvert();

        private static object GetAttenuation(object instanceObj) => ((PointLight2DElement)instanceObj).Attenuation;

        private static void SetAttenuation(ref object instanceObj, object valueObj) => ((PointLight2DElement)instanceObj).Attenuation = (Vector3)valueObj;

        private static object Construct() => new PointLight2DElement();

        public static void Pass1Initialize() => Type = new UIXTypeSchema(159, "PointLight2D", null, 77, typeof(PointLight2DElement), UIXTypeFlags.None);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(159, "Position", 234, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetPosition), new SetValueHandler(SetPosition), false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(159, "Radius", 194, -1, ExpressionRestriction.None, false, SingleSchema.ValidateNotNegative, false, new GetValueHandler(GetRadius), new SetValueHandler(SetRadius), false);
            UIXPropertySchema uixPropertySchema3 = new UIXPropertySchema(159, "LightColor", 35, -1, ExpressionRestriction.None, false, null, false, null, new SetValueHandler(SetLightColor), false);
            UIXPropertySchema uixPropertySchema4 = new UIXPropertySchema(159, "AmbientColor", 35, -1, ExpressionRestriction.None, false, null, false, null, new SetValueHandler(SetAmbientColor), false);
            UIXPropertySchema uixPropertySchema5 = new UIXPropertySchema(159, "Attenuation", 234, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetAttenuation), new SetValueHandler(SetAttenuation), false);
            Type.Initialize(new DefaultConstructHandler(Construct), null, new PropertySchema[5]
            {
         uixPropertySchema4,
         uixPropertySchema5,
         uixPropertySchema3,
         uixPropertySchema1,
         uixPropertySchema2
            }, null, null, null, null, null, null, null, null, null);
        }
    }
}
