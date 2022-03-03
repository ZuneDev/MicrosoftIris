// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Internal.ObjectTrackerGroup
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

namespace Microsoft.Iris.Render.Internal
{
    internal static class ObjectTrackerGroup
    {
        private static ObjectTracker s_objectTrackerGroup = new ObjectTracker(null, ObjectTracker.ThreadMode.Master, null);

        internal static void RegisterChildTracker(ObjectTracker trackerToAdd) => s_objectTrackerGroup.AddObject(trackerToAdd);
    }
}
