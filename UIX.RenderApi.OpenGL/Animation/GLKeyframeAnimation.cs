using System.Collections.Generic;

namespace Microsoft.Iris.Render.OpenGL
{
    /// <summary>
    /// Keyframe animation. Stores keyframes, targets and events and runs the play-state
    /// machine. Smooth per-frame evaluation and target property mutation are a stage-3
    /// TODO; today it drives lifecycle/events so higher layers sequence correctly.
    /// </summary>
    public sealed class GLKeyframeAnimation : GLAnimation, IKeyframeAnimation
    {
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

        public GLKeyframeAnimation(AnimationInput initialValue)
        {
            InitialValue = initialValue;
            Type = initialValue.InputType;
        }

        public int KeyframeCount => m_keyframes.Count;
        public AnimationInput InitialValue { get; }
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
    }
}
