// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Animations.RotateKeyframe
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Drawing;
using Microsoft.Iris.Render;
using Microsoft.Iris.Session;
using Microsoft.Iris.UI;

namespace Microsoft.Iris.Animations
{
    internal class RotateKeyframe : BaseRotationKeyframe
    {
        public override AnimationType Type => AnimationType.Rotate;

        public override Rotation GetEffectiveValue(
          IAnimatable targetObject,
          Rotation baseValueRotation,
          ref AnimationArgs args)
        {
            if (RelativeTo == RelativeTo.Final)
                baseValueRotation.AngleRadians += args.NewRotation.AngleRadians;
            if (UISession.Default.IsRtl && baseValueRotation.Axis.Y <= 0.0 && baseValueRotation.Axis.X <= 0.0)
                baseValueRotation.AngleRadians = -baseValueRotation.AngleRadians;
            return baseValueRotation;
        }

        public override void Apply(IAnimatableOwner animationTarget, Rotation valueRotation) => ((ViewItem)animationTarget).VisualRotation = valueRotation;

        public override void MagnifyValue(float magnifyValue) => Value = new Rotation(Value.AngleRadians * magnifyValue, Value.Axis);
    }
}
