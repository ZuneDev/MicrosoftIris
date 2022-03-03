// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Graphics.IVideoPoolNotification
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

namespace Microsoft.Iris.Render.Graphics
{
    internal interface IVideoPoolNotification
    {
        void InvalidateClients();

        void NotifySourceChanged(Size sizeTargetPxl, Size sizeAspectRatio);
    }
}
