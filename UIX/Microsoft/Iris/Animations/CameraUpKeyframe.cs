// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Animations.CameraUpKeyframe
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Drawing;
using Microsoft.Iris.Render;

namespace Microsoft.Iris.Animations
{
    internal class CameraUpKeyframe : BaseMultiplyVector3Keyframe
    {
        public override AnimationType Type => AnimationType.CameraUp;

        public override void Apply(IAnimatableOwner animationTarget, Vector3 value) => ((Camera)animationTarget).CameraUp = value;

        public override Vector3 GetRelativeToFinalValue(
          IAnimatable targetObject,
          ref AnimationArgs args)
        {
            return args.NewUp;
        }
    }
}
