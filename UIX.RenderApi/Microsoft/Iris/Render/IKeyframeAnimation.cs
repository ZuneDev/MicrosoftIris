// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.IKeyframeAnimation
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

namespace Microsoft.Iris.Render
{
    public interface IKeyframeAnimation : IAnimation, ISharedRenderObject, IActivatable
    {
        int KeyframeCount { get; }

        AnimationInput InitialValue { get; }

        AnimationInput Reference { get; set; }

        AnimationInput Scale { get; set; }

        AnimationInputType Type { get; }

        void AddKeyframe(AnimationKeyframe keyframe);

        AnimationKeyframe GetKeyframe(int keyframeIndex);

        void SetKeyframe(int keyframeIndex, AnimationKeyframe keyframe);

        void AddTarget(IAnimatable targetObject, string targetProperty);

        void AddTarget(IAnimatable targetObject, string targetProperty, string targetPropertyMask);

        void RemoveTarget(IAnimatable targetObject, string targetProperty, string targetPropertyMask);

        void RemoveAllTargets();

        void AddStageEvent(AnimationStage animationStage, AnimationEvent animationEvent);

        void AddTimeEvent(float absoluteTime, AnimationEvent animationEvent);

        void AddProgressEvent(float progress, AnimationEvent animationEvent);

        void AddValueEvent(
          ValueEventCondition condition,
          AnimationInput reference,
          AnimationEvent animationEvent);

        void RemoveEvent(AnimationEvent animationEvent);

        void RemoveAllEvents();
    }
}
