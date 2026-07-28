using System.Collections.Generic;

namespace Microsoft.Iris.Render.OpenGL.Animation
{
    /// <summary>
    /// Time-driven keyframe animation. On each pulse it advances its clock, finds the
    /// surrounding keyframes, eases and interpolates between their (constant) values and
    /// writes the result onto every target property. Supports repeat/auto-reset/reset
    /// behavior and additive/multiplicative Reference/Scale inputs.
    /// </summary>
    /// <remarks>
    /// Keyframe times are in seconds. Matching the original, keyframe 0 at t=0 holds the
    /// initial value (added here unless the system is in BackCompat mode). Stage/time/
    /// progress/value events are recorded but not yet dispatched — see the log for why
    /// (their targets require the render-internal IActivatableObject).
    /// </remarks>
    internal sealed class GLKeyframeAnimation : GLAnimation, IKeyframeAnimation
    {
        private static readonly LinearInterpolation s_defaultInterpolation = new LinearInterpolation();

        private readonly struct Target
        {
            public readonly IAnimatable Object;
            public readonly string Property;
            public readonly string? Mask;
            public Target(IAnimatable o, string property, string? mask)
            {
                Object = o;
                Property = property;
                Mask = mask;
            }
        }

        private readonly List<AnimationKeyframe> m_keyframes = new List<AnimationKeyframe>();
        private readonly List<Target> m_targets = new List<Target>();
        private readonly List<AnimationEvent> m_events = new List<AnimationEvent>();
        private readonly AnimationInput m_initialValue;

        private float m_timeSec;
        private int m_loopsCompleted;

        public GLKeyframeAnimation(AnimationInput initialValue)
        {
            m_initialValue = initialValue;
            Type = initialValue.InputType;
        }

        /// <summary>
        /// Seed keyframe 0 (t=0) with the initial value. The animation system calls this
        /// unless it is in BackCompat mode, matching the original renderer's constructor.
        /// </summary>
        internal void AddInitialKeyframe()
            => m_keyframes.Add(new AnimationKeyframe(0f, m_initialValue, s_defaultInterpolation));

        public int KeyframeCount => m_keyframes.Count;
        public AnimationInput InitialValue => m_keyframes.Count > 0 ? m_keyframes[0].Value : m_initialValue;
        public AnimationInput Reference { get; set; } = null!;
        public AnimationInput Scale { get; set; } = null!;
        public AnimationInputType Type { get; }

        public void AddKeyframe(AnimationKeyframe keyframe) => m_keyframes.Add(keyframe);
        public AnimationKeyframe GetKeyframe(int keyframeIndex) => m_keyframes[keyframeIndex];
        public void SetKeyframe(int keyframeIndex, AnimationKeyframe keyframe) => m_keyframes[keyframeIndex] = keyframe;

        public void AddTarget(IAnimatable targetObject, string targetProperty)
            => m_targets.Add(new Target(targetObject, targetProperty, null));

        public void AddTarget(IAnimatable targetObject, string targetProperty, string targetPropertyMask)
            => m_targets.Add(new Target(targetObject, targetProperty, targetPropertyMask));

        public void RemoveTarget(IAnimatable targetObject, string targetProperty, string targetPropertyMask)
            => m_targets.RemoveAll(t => ReferenceEquals(t.Object, targetObject)
                && t.Property == targetProperty && t.Mask == targetPropertyMask);

        public void RemoveAllTargets() => m_targets.Clear();

        public void AddStageEvent(AnimationStage animationStage, AnimationEvent animationEvent) => m_events.Add(animationEvent);
        public void AddTimeEvent(float absoluteTime, AnimationEvent animationEvent) => m_events.Add(animationEvent);
        public void AddProgressEvent(float progress, AnimationEvent animationEvent) => m_events.Add(animationEvent);
        public void AddValueEvent(ValueEventCondition condition, AnimationInput reference, AnimationEvent animationEvent) => m_events.Add(animationEvent);
        public void RemoveEvent(AnimationEvent animationEvent) => m_events.Remove(animationEvent);
        public void RemoveAllEvents() => m_events.Clear();

        // ---- Lifecycle -----------------------------------------------------------
        public override void Play()
        {
            // Replaying after a completed run restarts from the top.
            if (IsActive && !IsPlaying && m_timeSec >= Duration)
            {
                m_timeSec = 0f;
                m_loopsCompleted = 0;
            }
            base.Play();
        }

        public override void Reset()
        {
            base.Reset();
            m_timeSec = 0f;
            m_loopsCompleted = 0;
            ApplyResetBehavior();
        }

        public override void InstantAdvance(float advanceTime)
        {
            if (advanceTime > 0f)
                AdvanceBy(advanceTime);
        }

        public override void InstantFinish()
        {
            float duration = Duration;
            m_timeSec = duration;
            ApplyAt(duration);
            IsPlaying = false;
            m_loopsCompleted = RepeatCount < 0 ? 0 : RepeatCount;
            if (AutoReset)
                Reset();
        }

        internal override void Advance(int advanceMs)
        {
            if (IsPlaying && advanceMs > 0)
                AdvanceBy(advanceMs / 1000f);
        }

        // ---- Evaluation ----------------------------------------------------------
        /// <summary>Total animation length in seconds (largest keyframe time).</summary>
        private float Duration
        {
            get
            {
                float max = 0f;
                foreach (AnimationKeyframe k in m_keyframes)
                    if (k.Time > max)
                        max = k.Time;
                return max;
            }
        }

        private bool IsInfinite => RepeatCount < 0;

        private void AdvanceBy(float dt)
        {
            float duration = Duration;
            m_timeSec += dt;

            if (duration <= 0f)
            {
                ApplyAt(0f);
                Complete();
                return;
            }

            while (m_timeSec >= duration)
            {
                if (IsInfinite || m_loopsCompleted < RepeatCount)
                {
                    m_timeSec -= duration;
                    m_loopsCompleted++;
                }
                else
                {
                    m_timeSec = duration;
                    ApplyAt(duration);
                    Complete();
                    return;
                }
            }

            ApplyAt(m_timeSec);
        }

        private void Complete()
        {
            IsPlaying = false;
            if (AutoReset)
                Reset();
        }

        private void ApplyResetBehavior()
        {
            switch (ResetBehavior)
            {
                case AnimationResetBehavior.SetInitialValue:
                    ApplyAt(0f);
                    break;
                case AnimationResetBehavior.SetFinalValue:
                    ApplyAt(Duration);
                    break;
                // LeaveCurrent: nothing to do.
            }
        }

        private void ApplyAt(float time)
        {
            if (m_keyframes.Count == 0 || m_targets.Count == 0)
                return;
            if (!SampleValue(time, out AnimValue value))
                return;

            value = ApplyReferenceAndScale(value);

            foreach (Target target in m_targets)
                AnimationTargetApplier.Apply(target.Object, target.Property, target.Mask, value);
        }

        private bool SampleValue(float time, out AnimValue value)
        {
            value = default;

            // Keyframes are authored in time order; sort defensively so out-of-order
            // additions still evaluate correctly.
            m_keyframes.Sort((a, b) => a.Time.CompareTo(b.Time));

            if (time <= m_keyframes[0].Time)
                return AnimValue.TryRead(m_keyframes[0].Value, out value);

            AnimationKeyframe last = m_keyframes[m_keyframes.Count - 1];
            if (time >= last.Time)
                return AnimValue.TryRead(last.Value, out value);

            for (int i = 0; i < m_keyframes.Count - 1; i++)
            {
                AnimationKeyframe a = m_keyframes[i];
                AnimationKeyframe b = m_keyframes[i + 1];
                if (time < a.Time || time > b.Time)
                    continue;

                float span = b.Time - a.Time;
                float localT = span > 0f ? (time - a.Time) / span : 1f;
                float eased = AnimationEasing.Ease(b.Interpolation, localT);

                bool haveA = AnimValue.TryRead(a.Value, out AnimValue va);
                bool haveB = AnimValue.TryRead(b.Value, out AnimValue vb);
                if (haveA && haveB)
                {
                    bool spherical = b.Interpolation?.UseSphericalCombination ?? false;
                    value = AnimValue.Lerp(va, vb, eased, spherical);
                    return true;
                }
                // If only one endpoint is readable (e.g. the other is object-relative),
                // hold that endpoint rather than skipping the whole frame.
                if (haveA) { value = va; return true; }
                if (haveB) { value = vb; return true; }
                return false;
            }
            return false;
        }

        private AnimValue ApplyReferenceAndScale(AnimValue value)
        {
            // Effective = Reference + Scale * value (both optional). The exact native
            // combination is unverified; this is the conventional interpretation.
            if (Scale != null && AnimValue.TryRead(Scale, out AnimValue s))
                value = new AnimValue(value.Type, value.X * s.X, value.Y * s.Y, value.Z * s.Z, value.W * s.W);
            if (Reference != null && AnimValue.TryRead(Reference, out AnimValue r))
                value = new AnimValue(value.Type, value.X + r.X, value.Y + r.Y, value.Z + r.Z, value.W + r.W);
            return value;
        }
    }
}
