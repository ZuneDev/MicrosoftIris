// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Animations.ScaleYKeyframe
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Render;
using Microsoft.Iris.UI;

namespace Microsoft.Iris.Animations
{
    internal class ScaleYKeyframe : BaseMultiplyFloatKeyframe
    {
        public override AnimationType Type => AnimationType.ScaleY;

        public override void Apply(IAnimatableOwner animationTarget, float value)
        {
            Vector3 visualScale = ((ViewItem)animationTarget).VisualScale;
            visualScale.Y = value;
            ((ViewItem)animationTarget).VisualScale = visualScale;
        }

        public override float GetEffectiveValue(
          IAnimatable targetObject,
          float baseValue,
          ref AnimationArgs args)
        {
            return ScaleKeyframe.GetEffectiveScaleValue(RelativeTo, targetObject, new Vector3(0.0f, baseValue, 0.0f), ref args).Y;
        }
    }
}
