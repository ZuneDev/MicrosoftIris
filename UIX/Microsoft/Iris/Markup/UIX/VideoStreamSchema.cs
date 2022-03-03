// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.VideoStreamSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

namespace Microsoft.Iris.Markup.UIX
{
    internal static class VideoStreamSchema
    {
        public static UIXTypeSchema Type;

        private static object GetStreamID(object instanceObj) => ((VideoStream)instanceObj).StreamID;

        private static object Construct() => new VideoStream();

        public static void Pass1Initialize() => Type = new UIXTypeSchema(238, "VideoStream", null, 153, typeof(VideoStream), UIXTypeFlags.Immutable);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema = new UIXPropertySchema(238, "StreamID", 115, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetStreamID), null, false);
            Type.Initialize(new DefaultConstructHandler(Construct), null, new PropertySchema[1]
            {
         uixPropertySchema
            }, null, null, null, null, null, null, null, null, null);
        }
    }
}
