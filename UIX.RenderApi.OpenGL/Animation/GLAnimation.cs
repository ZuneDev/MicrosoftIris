using System.Collections.Generic;

namespace Microsoft.Iris.Render.OpenGL
{
    /// <summary>
    /// Shared state machine for animations: play/pause/reset, repeat counting and the
    /// async-notify event. Target property interpolation is intentionally minimal here
    /// (see <see cref="GLKeyframeAnimation"/>); full per-frame evaluation is a stage-3 TODO.
    /// </summary>
    public abstract class GLAnimation : SharedRenderObject, IAnimation
    {
        public int RepeatCount { get; set; }
        public bool IsPlaying { get; private set; }
        public bool IsActive { get; private set; }
        public bool AutoReset { get; set; }
        public AnimationResetBehavior ResetBehavior { get; set; } = AnimationResetBehavior.LeaveCurrent;

        public event AsyncNotifyHandler? AsyncNotifyEvent;

        public virtual void Play()
        {
            IsPlaying = true;
            IsActive = true;
        }

        public virtual void Pause() => IsPlaying = false;

        public virtual void Reset()
        {
            IsPlaying = false;
            IsActive = false;
        }

        public virtual void InstantAdvance(float advanceTime) { }

        public virtual void InstantFinish()
        {
            IsPlaying = false;
            IsActive = false;
        }

        protected void RaiseAsyncNotify(int cookie) => AsyncNotifyEvent?.Invoke(cookie);

        /// <summary>Advance internal time. Called by the animation system each pulse.</summary>
        internal virtual void Advance(int advanceMs) { }
    }

    public sealed class GLAnimationGroup : GLAnimation, IAnimationGroup
    {
        private readonly List<GLAnimation> m_members = new List<GLAnimation>();

        public override void Play()
        {
            base.Play();
            foreach (GLAnimation a in m_members)
                a.Play();
        }

        internal void Add(GLAnimation animation) => m_members.Add(animation);

        internal override void Advance(int advanceMs)
        {
            foreach (GLAnimation a in m_members)
                a.Advance(advanceMs);
        }
    }
}
