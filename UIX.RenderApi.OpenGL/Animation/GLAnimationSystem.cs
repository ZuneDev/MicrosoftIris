using System.Collections.Generic;

namespace Microsoft.Iris.Render.OpenGL
{
    /// <summary>
    /// Owns the set of live animations and advances them on each pulse. Pause/step/resume
    /// gate whether time flows. Objects are created here so the session stays a thin factory.
    /// </summary>
    public sealed class GLAnimationSystem : IAnimationSystem
    {
        private readonly List<GLAnimation> m_animations = new List<GLAnimation>();
        private bool m_paused;

        public int UpdatesPerSecond { get; set; } = 60;
        public float SpeedAdjustment { get; set; } = 1f;
        public bool BackCompat { set { /* compatibility flag; no behavioral change */ } }

        public IKeyframeAnimation CreateKeyframeAnimation(object objUser, AnimationInput initialValue)
        {
            var a = new GLKeyframeAnimation(initialValue);
            m_animations.Add(a);
            return a;
        }

        public IAnimationGroup CreateAnimationGroup(object objUser)
        {
            var g = new GLAnimationGroup();
            m_animations.Add(g);
            return g;
        }

        public IExternalAnimationInput CreateExternalAnimationInput(object objUser, IAnimationPropertyMap propertyMap)
            => new GLExternalAnimationInput(propertyMap);

        public void PulseTimeAdvance(int nAdvanceMs)
        {
            if (m_paused)
                return;
            int scaled = (int)(nAdvanceMs * SpeedAdjustment);
            foreach (GLAnimation a in m_animations)
            {
                if (a.IsPlaying)
                    a.Advance(scaled);
            }
        }

        public void PauseAnimations() => m_paused = true;

        public void StepAnimations(int nAdvanceMs)
        {
            foreach (GLAnimation a in m_animations)
            {
                if (a.IsPlaying)
                    a.Advance(nAdvanceMs);
            }
        }

        public void ResumeAnimations() => m_paused = false;
    }
}
