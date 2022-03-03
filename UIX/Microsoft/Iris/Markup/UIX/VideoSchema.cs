// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.VideoSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Drawing;
using Microsoft.Iris.RenderAPI.VideoPlayback;
using Microsoft.Iris.UI;
using Microsoft.Iris.ViewItems;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class VideoSchema
    {
        public static UIXTypeSchema Type;

        private static object GetChildren(object instanceObj) => ViewItemSchema.ListProxy.GetChildren((ViewItem)instanceObj);

        private static object GetVideoStream(object instanceObj) => ((Video)instanceObj).VideoStream;

        private static void SetVideoStream(ref object instanceObj, object valueObj) => ((Video)instanceObj).VideoStream = (IUIVideoStream)valueObj;

        private static object GetLetterboxColor(object instanceObj) => ((Video)instanceObj).LetterboxColor;

        private static void SetLetterboxColor(ref object instanceObj, object valueObj) => ((Video)instanceObj).LetterboxColor = (Color)valueObj;

        private static object Construct() => new Video();

        public static void Pass1Initialize() => Type = new UIXTypeSchema(235, "Video", null, 239, typeof(Video), UIXTypeFlags.Disposable);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(235, "Children", 138, 239, ExpressionRestriction.NoAccess, false, null, true, new GetValueHandler(GetChildren), null, false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(235, "VideoStream", 238, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetVideoStream), new SetValueHandler(SetVideoStream), false);
            UIXPropertySchema uixPropertySchema3 = new UIXPropertySchema(235, "LetterboxColor", 35, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetLetterboxColor), new SetValueHandler(SetLetterboxColor), false);
            Type.Initialize(new DefaultConstructHandler(Construct), null, new PropertySchema[3]
            {
         uixPropertySchema1,
         uixPropertySchema3,
         uixPropertySchema2
            }, null, null, null, null, null, null, null, null, null);
        }
    }
}
