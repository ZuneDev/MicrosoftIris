// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Graphics.ImageLoadInfo
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using System;

namespace Microsoft.Iris.Render.Graphics
{
    internal class ImageLoadInfo
    {
        public Image imageOwner;
        public IntPtr rgAppData;
        public int nStride;
        public int nLoadsInProgress;
        public int nSystemLoadRequests;
        public ContentNotifyHandler handlerNotify;
    }
}
