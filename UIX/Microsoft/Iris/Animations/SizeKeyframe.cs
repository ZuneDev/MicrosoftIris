// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Animations.SizeKeyframe
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Render;
using Microsoft.Iris.UI;

namespace Microsoft.Iris.Animations
{
    internal class SizeKeyframe : BaseVector2Keyframe
    {
        public override AnimationType Type => AnimationType.Size;

        public override Vector2 GetEffectiveValue(
          IAnimatable targetObject,
          Vector2 baseValueVector,
          ref AnimationArgs args)
        {
            if (RelativeTo == RelativeTo.Final)
                baseValueVector += args.NewSize;
            return baseValueVector;
        }

        public override void Apply(IAnimatableOwner animationTarget, Vector2 valueVector) => ((ViewItem)animationTarget).VisualSize = valueVector;
    }
}
