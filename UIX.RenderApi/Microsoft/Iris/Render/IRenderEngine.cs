// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.IRenderEngine
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using System;

namespace Microsoft.Iris.Render
{
    public interface IRenderEngine : IRenderObject, IDisposable
    {
        void Initialize(
          GraphicsDeviceType typeGraphics,
          GraphicsRenderingQuality renderingQuality,
          SoundDeviceType typeSound);

        IRenderSession Session { get; }

        IRenderWindow Window { get; }

        IDisplayManager DisplayManager { get; }

        bool ProcessNativeEvents();

        void WaitForWork(uint nTimeoutInMsecs);

        void InterThreadWake();

        void FlushBatch();

        bool IsGraphicsDeviceAvailable(GraphicsDeviceType type, bool fFilterRecommended);

        bool IsSoundDeviceAvailable(SoundDeviceType type);
    }
}
