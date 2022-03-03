// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.IAnimation
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

namespace Microsoft.Iris.Render
{
    public interface IAnimation : ISharedRenderObject
    {
        int RepeatCount { get; set; }

        bool IsPlaying { get; }

        bool IsActive { get; }

        bool AutoReset { get; set; }

        AnimationResetBehavior ResetBehavior { get; set; }

        void Play();

        void Pause();

        void Reset();

        void InstantAdvance(float advanceTime);

        void InstantFinish();

        event AsyncNotifyHandler AsyncNotifyEvent;
    }
}
