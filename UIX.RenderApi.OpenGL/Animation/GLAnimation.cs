using System.Collections.Generic;

namespace Microsoft.Iris.Render.OpenGL
{
    /// <summary>
    /// Shared state machine for animations: play/pause/reset, repeat and the async-notify
    /// event. Time evaluation lives in the concrete subclasses (see <see cref="GLKeyframeAnimation"/>).
    /// </summary>
    public abstract class GLAnimation : SharedRenderObject, IAnimation
    {
        public int RepeatCount { get; set; }
        public bool IsPlaying { get; protected set; }
        public bool IsActive { get; protected set; }
        public bool AutoReset { get; set; }
        public AnimationResetBehavior ResetBehavior { get; set; } = AnimationResetBehavior.LeaveCurrent;

        public event AsyncNotifyHandler? AsyncNotifyEvent;

        public virtual void Play()
        {
            IsPlaying = true;
            IsActive = true;
        }

        public virtual void Pause()
        {
            if (IsActive)
                IsPlaying = false;
        }

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

        /// <summary>Advance internal time by <paramref name="advanceMs"/>. Driven by the system's pulse.</summary>
        internal abstract void Advance(int advanceMs);
    }

    /// <summary>
    /// Aggregates child animations and drives them together. The public API exposes no way
    /// to add members (IAnimationGroup has no members beyond IAnimation), so membership is
    /// only available internally; kept for lifecycle parity.
    /// </summary>
    public sealed class GLAnimationGroup : GLAnimation, IAnimationGroup
    {
        private readonly List<GLAnimation> m_members = new List<GLAnimation>();

        public override void Play()
        {
            base.Play();
            foreach (GLAnimation a in m_members)
                a.Play();
        }

        public override void Pause()
        {
            base.Pause();
            foreach (GLAnimation a in m_members)
                a.Pause();
        }

        public override void Reset()
        {
            base.Reset();
            foreach (GLAnimation a in m_members)
                a.Reset();
        }

        internal void Add(GLAnimation animation) => m_members.Add(animation);

        internal override void Advance(int advanceMs)
        {
            foreach (GLAnimation a in m_members)
                if (a.IsPlaying)
                    a.Advance(advanceMs);
        }
    }
}
