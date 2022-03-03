// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Animations.BaseRotationKeyframe
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Drawing;
using Microsoft.Iris.Render;

namespace Microsoft.Iris.Animations
{
    internal abstract class BaseRotationKeyframe : BaseKeyframe
    {
        private Rotation _valueRotation;

        public BaseRotationKeyframe() => _valueRotation = Rotation.Default;

        protected override void PopulateAnimationWorker(
          IAnimatable targetObject,
          AnimationProxy animation,
          ref AnimationArgs args)
        {
            Rotation effectiveValue = GetEffectiveValue(targetObject, _valueRotation, ref args);
            animation.AddRotationKeyframe(this, effectiveValue);
        }

        public Rotation Value
        {
            get => _valueRotation;
            set => _valueRotation = value;
        }

        public override object ObjectValue => Value;

        public virtual Rotation GetEffectiveValue(
          IAnimatable targetObject,
          Rotation baseValueRotation,
          ref AnimationArgs args)
        {
            return baseValueRotation;
        }

        public override void Apply(IAnimatableOwner animationTarget, ref AnimationArgs args)
        {
            Rotation effectiveValue = GetEffectiveValue(animationTarget.AnimationTarget, _valueRotation, ref args);
            Apply(animationTarget, effectiveValue);
        }

        public abstract void Apply(IAnimatableOwner animationTarget, Rotation valueRotation);
    }
}
