using System.Collections.Generic;

namespace Microsoft.Iris.Render.OpenGL.Animation
{
    /// <summary>
    /// Owns the set of live animations and advances them on each pulse. Pause/step/resume
    /// gate whether time flows. Objects are created here so the session stays a thin factory.
    /// </summary>
    public sealed class GLAnimationSystem : IAnimationSystem
    {
        private readonly object m_syncRoot;
        private readonly List<GLAnimation> m_animations = new List<GLAnimation>();
        private bool m_paused;

        private bool m_backCompat;

        public GLAnimationSystem(object syncRoot)
        {
            m_syncRoot = syncRoot;
        }

        public int UpdatesPerSecond { get; set; } = 60;
        public float SpeedAdjustment { get; set; } = 1f;

        // When set, keyframe 0 is not auto-populated with the initial value (matching the
        // original AnimationSystem.BackCompat behavior).
        public bool BackCompat { set => m_backCompat = value; }

        public IKeyframeAnimation CreateKeyframeAnimation(object objUser, AnimationInput initialValue)
        {
            var a = new GLKeyframeAnimation(m_syncRoot, initialValue);
            if (!m_backCompat)
                a.AddInitialKeyframe();
            lock (m_syncRoot)
                m_animations.Add(a);
            return a;
        }

        public IAnimationGroup CreateAnimationGroup(object objUser)
        {
            var g = new GLAnimationGroup(m_syncRoot);
            lock (m_syncRoot)
                m_animations.Add(g);
            return g;
        }

        public IExternalAnimationInput CreateExternalAnimationInput(object objUser, IAnimationPropertyMap propertyMap)
            => new GLExternalAnimationInput(propertyMap);

        public void PulseTimeAdvance(int nAdvanceMs)
        {
            if (m_paused)
                return;

            var scaled = (int)(nAdvanceMs * SpeedAdjustment);
            StepAnimations(scaled);
        }

        public void PauseAnimations() => m_paused = true;

        // Locked as one pass over a snapshot: m_animations itself only grows via
        // CreateKeyframeAnimation/CreateAnimationGroup (app thread), but each
        // animation's own IsPlaying/state is separately lock-protected (see
        // GLAnimation), so Advance still safely interleaves with app-side
        // Play/Pause/Reset even outside this method's own lock scope.
        public void StepAnimations(int nAdvanceMs)
        {
            List<GLAnimation> snapshot;
            lock (m_syncRoot)
                snapshot = new List<GLAnimation>(m_animations);

            foreach (var a in snapshot)
                if (a.IsPlaying)
                    a.Advance(nAdvanceMs);
        }

        public void ResumeAnimations() => m_paused = false;

        /// <summary>
        /// Whether any owned animation is currently playing. Used by the GL render
        /// engine to decide whether to keep pumping frames after this one (the render
        /// thread is otherwise idle/event-driven, not a continuous loop -- see
        /// GLRenderEngine.RenderThreadMain) instead of only rendering in response to
        /// an explicit FlushBatch/RenderNowIfPossible invalidation.
        /// </summary>
        internal bool HasPlayingAnimations
        {
            get
            {
                lock (m_syncRoot)
                {
                    foreach (var a in m_animations)
                        if (a.IsPlaying)
                            return true;
                    return false;
                }
            }
        }
    }
}
