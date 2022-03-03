// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.DestinationElementSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;
using Microsoft.Iris.Render;
using Microsoft.Iris.Session;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class DestinationElementSchema
    {
        public static UIXTypeSchema Type;

        private static object GetDownsample(object instanceObj) => ((DestinationElement)instanceObj).Downsample;

        private static void SetDownsample(ref object instanceObj, object valueObj)
        {
            DestinationElement destinationElement = (DestinationElement)instanceObj;
            float num = (float)valueObj;
            Result result = SingleSchema.Validate0to1(valueObj);
            if (result.Failed)
                ErrorManager.ReportError(result.Error);
            else
                destinationElement.Downsample = num;
        }

        private static object GetUVOffset(object instanceObj) => ((DestinationElement)instanceObj).UVOffset;

        private static void SetUVOffset(ref object instanceObj, object valueObj) => ((DestinationElement)instanceObj).UVOffset = (Vector2)valueObj;

        private static object Construct() => new DestinationElement();

        public static void Pass1Initialize() => Type = new UIXTypeSchema(56, "DestinationElement", null, 77, typeof(DestinationElement), UIXTypeFlags.None);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(56, "Downsample", 194, -1, ExpressionRestriction.None, false, SingleSchema.Validate0to1, false, new GetValueHandler(GetDownsample), new SetValueHandler(SetDownsample), false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(56, "UVOffset", 233, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetUVOffset), new SetValueHandler(SetUVOffset), false);
            Type.Initialize(new DefaultConstructHandler(Construct), null, new PropertySchema[2]
            {
         uixPropertySchema1,
         uixPropertySchema2
            }, null, null, null, null, null, null, null, null, null);
        }
    }
}
