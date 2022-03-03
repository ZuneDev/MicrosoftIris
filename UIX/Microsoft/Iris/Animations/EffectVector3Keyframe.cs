// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Animations.EffectVector3Keyframe
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Render;

namespace Microsoft.Iris.Animations
{
    internal class EffectVector3Keyframe : BaseVector3Keyframe
    {
        public override void Apply(IAnimatableOwner animationTarget, Vector3 value)
        {
        }

        public override AnimationType Type => AnimationType.Vector3;
    }
}
