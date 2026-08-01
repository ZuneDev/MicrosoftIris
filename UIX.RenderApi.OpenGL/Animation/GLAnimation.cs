using System.Collections.Generic;
using Microsoft.Iris.Render.Animation;
using Microsoft.Iris.Render.OpenGL.Scene;
using Microsoft.Iris.Render.Protocol;

namespace Microsoft.Iris.Render.OpenGL.Animation
{
    /// <summary>
    /// Shared state machine for animations: play/pause/reset, repeat and the async-notify
    /// event. Time evaluation lives in the concrete subclasses (see <see cref="GLKeyframeAnimation"/>).
    /// </summary>
    internal abstract class GLAnimation : SharedRenderObject, IAnimation, IActivatableObject
    {
        protected GLAnimation(object syncRoot) : base(syncRoot)
        {
        }

        public int RepeatCount { get; set; }

        // Written by app-side Play/Pause/Reset *and* by the render thread's
        // Advance/Complete path (GLKeyframeAnimation sets these directly on
        // completion/auto-reset, called from GLAnimationSystem.StepAnimations) --
        // and read every frame by GLAnimationSystem.GetPlayingAnimations, so both
        // sides need the same lock rather than plain auto-properties.
        private bool m_isPlaying;
        private bool m_isActive;
        public bool IsPlaying { get { lock (SyncRoot) return m_isPlaying; } protected set { lock (SyncRoot) m_isPlaying = value; } }
        public bool IsActive { get { lock (SyncRoot) return m_isActive; } protected set { lock (SyncRoot) m_isActive = value; } }
        public bool AutoReset { get; set; }
        public AnimationResetBehavior ResetBehavior { get; set; } = AnimationResetBehavior.LeaveCurrent;

        public event AsyncNotifyHandler? AsyncNotifyEvent;

        public virtual void Play()
        {
            lock (SyncRoot)
            {
                IsPlaying = true;
                IsActive = true;
            }
        }

        public virtual void Pause()
        {
            lock (SyncRoot)
            {
                if (IsActive)
                    IsPlaying = false;
            }
        }

        public virtual void Reset()
        {
            lock (SyncRoot)
            {
                IsPlaying = false;
                IsActive = false;
            }
        }

        public virtual void InstantAdvance(float advanceTime) { }

        public virtual void InstantFinish()
        {
            lock (SyncRoot)
            {
                IsPlaying = false;
                IsActive = false;
            }
        }

        protected void RaiseAsyncNotify(int cookie) => AsyncNotifyEvent?.Invoke(cookie);

        /// <summary>Advance internal time by <paramref name="advanceMs"/>. Driven by the system's pulse.</summary>
        internal abstract void Advance(int advanceMs);

        public RENDERHANDLE GetObjectId() => default;

        public uint GetMethodId(string methodName)
        {
            throw new System.NotImplementedException();
        }
    }

    /// <summary>
    /// Aggregates child animations and drives them together. The public API exposes no way
    /// to add members (IAnimationGroup has no members beyond IAnimation), so membership is
    /// only available internally; kept for lifecycle parity.
    /// </summary>
    internal sealed class GLAnimationGroup : GLAnimation, IAnimationGroup
    {
        private readonly List<GLAnimation> m_members = new List<GLAnimation>();

        public GLAnimationGroup(object syncRoot) : base(syncRoot)
        {
        }

        public override void Play()
        {
            base.Play();
            lock (SyncRoot)
                foreach (GLAnimation a in m_members)
                    a.Play();
        }

        public override void Pause()
        {
            base.Pause();
            lock (SyncRoot)
                foreach (GLAnimation a in m_members)
                    a.Pause();
        }

        public override void Reset()
        {
            base.Reset();
            lock (SyncRoot)
                foreach (GLAnimation a in m_members)
                    a.Reset();
        }

        internal void Add(GLAnimation animation) { lock (SyncRoot) m_members.Add(animation); }

        internal override void Advance(int advanceMs)
        {
            lock (SyncRoot)
            {
                foreach (GLAnimation a in m_members)
                    if (a.IsPlaying)
                        a.Advance(advanceMs);
            }
        }
    }
}
