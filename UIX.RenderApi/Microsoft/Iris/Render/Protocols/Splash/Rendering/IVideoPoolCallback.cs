// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocols.Splash.Rendering.IVideoPoolCallback
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Protocol;

namespace Microsoft.Iris.Render.Protocols.Splash.Rendering
{
    internal interface IVideoPoolCallback
    {
        void OnInvalidate(RENDERHANDLE target);

        void OnInputChanged(
          RENDERHANDLE target,
          Size sizeTargetPxl,
          Size sizeAspectRatioPxl,
          uint nFrameRateNumerator,
          uint nFrameRateDenominator,
          SurfaceFormat nFormat);
    }
}
