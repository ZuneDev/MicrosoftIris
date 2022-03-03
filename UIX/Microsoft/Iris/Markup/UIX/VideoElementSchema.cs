// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.VideoElementSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Render;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class VideoElementSchema
    {
        public static UIXTypeSchema Type;

        private static void SetVideoStream(ref object instanceObj, object valueObj)
        {
            VideoElement videoElement = (VideoElement)instanceObj;
            Microsoft.Iris.VideoStream videoStream = (Microsoft.Iris.VideoStream)valueObj;
            if (videoStream == null)
                return;
            videoElement.VideoStream = videoStream.RenderStream;
        }

        private static object Construct() => new VideoElement();

        public static void Pass1Initialize() => Type = new UIXTypeSchema(236, "VideoElement", null, 77, typeof(VideoElement), UIXTypeFlags.None);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema = new UIXPropertySchema(236, "VideoStream", 238, -1, ExpressionRestriction.None, false, null, false, null, new SetValueHandler(SetVideoStream), false);
            Type.Initialize(new DefaultConstructHandler(Construct), null, new PropertySchema[1]
            {
         uixPropertySchema
            }, null, null, null, null, null, null, null, null, null);
        }
    }
}
