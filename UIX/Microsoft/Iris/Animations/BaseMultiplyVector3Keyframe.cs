// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Animations.BaseMultiplyVector3Keyframe
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Render;

namespace Microsoft.Iris.Animations
{
    internal abstract class BaseMultiplyVector3Keyframe : BaseVector3Keyframe
    {
        public BaseMultiplyVector3Keyframe() => Value = Vector3.UnitVector;

        public override bool Multiply => true;

        public override Vector3 GetEffectiveValue(
          IAnimatable targetObject,
          Vector3 baseValueVector,
          ref AnimationArgs args)
        {
            if (RelativeTo == RelativeTo.Final)
                baseValueVector *= GetRelativeToFinalValue(targetObject, ref args);
            return baseValueVector;
        }

        public abstract Vector3 GetRelativeToFinalValue(
          IAnimatable targetObject,
          ref AnimationArgs args);
    }
}
