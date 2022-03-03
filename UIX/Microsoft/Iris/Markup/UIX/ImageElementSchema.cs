// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.ImageElementSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Drawing;
using Microsoft.Iris.Render;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class ImageElementSchema
    {
        public static UIXTypeSchema Type;

        private static void SetImage(ref object instanceObj, object valueObj) => ((ImageElement)instanceObj).Image = ((UIImage)valueObj)?.RenderImage;

        private static object GetUVOffset(object instanceObj) => ((ImageElement)instanceObj).UVOffset;

        private static void SetUVOffset(ref object instanceObj, object valueObj) => ((ImageElement)instanceObj).UVOffset = (Vector2)valueObj;

        private static object Construct() => new ImageElement();

        public static void Pass1Initialize() => Type = new UIXTypeSchema(106, "ImageElement", null, 77, typeof(ImageElement), UIXTypeFlags.None);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(106, "Image", 105, -1, ExpressionRestriction.None, false, null, false, null, new SetValueHandler(SetImage), false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(106, "UVOffset", 233, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetUVOffset), new SetValueHandler(SetUVOffset), false);
            Type.Initialize(new DefaultConstructHandler(Construct), null, new PropertySchema[2]
            {
         uixPropertySchema1,
         uixPropertySchema2
            }, null, null, null, null, null, null, null, null, null);
        }
    }
}
