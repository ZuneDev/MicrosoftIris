// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Animations.ScaleKeyframe
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Render;
using Microsoft.Iris.RenderAPI.Drawing;
using Microsoft.Iris.UI;

namespace Microsoft.Iris.Animations
{
    internal class ScaleKeyframe : BaseVector3Keyframe
    {
        public ScaleKeyframe() => Value = Vector3.UnitVector;

        public override bool Multiply => true;

        public override AnimationType Type => AnimationType.Scale;

        public override Vector3 GetEffectiveValue(
          IAnimatable targetObject,
          Vector3 baseValueVector,
          ref AnimationArgs args)
        {
            return GetEffectiveScaleValue(RelativeTo, targetObject, baseValueVector, ref args);
        }

        public static Vector3 GetEffectiveScaleValue(
          RelativeTo relativeTo,
          IAnimatable targetObject,
          Vector3 baseValueVector,
          ref AnimationArgs args)
        {
            if (relativeTo == RelativeTo.Final)
                baseValueVector *= args.NewScale;
            else if (relativeTo is SnapshotRelativeTo snapshotRelativeTo)
            {
                RectangleF rectangleF = args.ViewItem.TransformFromAncestor(null, snapshotRelativeTo.Bounds);
                baseValueVector = baseValueVector * args.NewScale * new Vector3(rectangleF.Width / args.NewSize.X, rectangleF.Height / args.NewSize.Y, 0.0f);
            }
            return baseValueVector;
        }

        public override void Apply(IAnimatableOwner animationTarget, Vector3 valueVector) => ((ViewItem)animationTarget).VisualScale = valueVector;

        public override void MagnifyValue(float magnifyValue)
        {
            Vector3 vector3 = Value;
            Value = new Vector3(MagnifyDimension(vector3.X, magnifyValue), MagnifyDimension(vector3.Y, magnifyValue), MagnifyDimension(vector3.Z, magnifyValue));
        }

        private float MagnifyDimension(float dimensionValue, float magnifyValue)
        {
            if (dimensionValue != 1.0)
                dimensionValue *= magnifyValue;
            return dimensionValue;
        }
    }
}
