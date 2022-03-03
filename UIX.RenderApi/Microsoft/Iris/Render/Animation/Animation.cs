// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Animation.Animation
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Protocol;
using Microsoft.Iris.Render.Protocols.Splash.Rendering;

namespace Microsoft.Iris.Render.Animation
{
    internal abstract class Animation :
      SharedRenderObject,
      IAnimation,
      ISharedRenderObject,
      IAnimationCallback
    {
        public static readonly string OutputProperty = "Output";
        public static readonly string PlayMethod = "Play";
        public static readonly string PauseMethod = "Pause";
        public static readonly string ResetMethod = "Reset";
        public static readonly string FinishMethod = "Finish";
        public static readonly string NotifyMethod = "AsyncNotify";

        internal Animation()
        {
        }

        public abstract int RepeatCount { get; set; }

        public abstract bool IsPlaying { get; }

        public abstract bool IsActive { get; }

        public abstract bool AutoReset { get; set; }

        public abstract AnimationResetBehavior ResetBehavior { get; set; }

        public abstract void Play();

        public abstract void Pause();

        public abstract void Reset();

        public abstract void InstantAdvance(float advanceTime);

        public abstract void InstantFinish();

        void IAnimationCallback.AsyncNotify(RENDERHANDLE target, int nCookie)
        {
            if (this.AsyncNotifyEvent == null)
                return;
            this.AsyncNotifyEvent(nCookie);
        }

        public event AsyncNotifyHandler AsyncNotifyEvent;

        protected enum AnimatableProperties : uint
        {
            Output = 1,
        }

        protected enum ActivatableMethods : uint
        {
            Play = 1,
            Pause = 2,
            Reset = 3,
            Finish = 4,
            Notify = 5,
        }
    }
}
