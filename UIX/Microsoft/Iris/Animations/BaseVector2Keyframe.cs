// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Animations.BaseVector2Keyframe
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Render;

namespace Microsoft.Iris.Animations
{
    internal abstract class BaseVector2Keyframe : BaseKeyframe
    {
        private Vector2 _valueVector;

        protected override void PopulateAnimationWorker(
          IAnimatable targetObject,
          AnimationProxy animation,
          ref AnimationArgs args)
        {
            Vector2 effectiveValue = GetEffectiveValue(targetObject, _valueVector, ref args);
            animation.AddVector2Keyframe(this, effectiveValue);
        }

        public Vector2 Value
        {
            get => _valueVector;
            set => _valueVector = value;
        }

        public override object ObjectValue => Value;

        public virtual Vector2 GetEffectiveValue(
          IAnimatable targetObject,
          Vector2 baseValueVector,
          ref AnimationArgs args)
        {
            return baseValueVector;
        }

        public override void Apply(IAnimatableOwner animationTarget, ref AnimationArgs args)
        {
            Vector2 effectiveValue = GetEffectiveValue(animationTarget.AnimationTarget, _valueVector, ref args);
            Apply(animationTarget, effectiveValue);
        }

        public abstract void Apply(IAnimatableOwner animationTarget, Vector2 valueVector);

        public override void MagnifyValue(float magnifyValue) => Value *= magnifyValue;
    }
}
