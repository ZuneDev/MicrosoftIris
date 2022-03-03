// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Animations.PositionKeyframe
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Render;
using Microsoft.Iris.RenderAPI.Drawing;
using Microsoft.Iris.Session;
using Microsoft.Iris.UI;

namespace Microsoft.Iris.Animations
{
    internal class PositionKeyframe : BaseVector3Keyframe
    {
        public override AnimationType Type => AnimationType.Position;

        public override Vector3 GetEffectiveValue(
          IAnimatable targetObject,
          Vector3 baseValueVector,
          ref AnimationArgs args)
        {
            return GetEffectivePositionValue(RelativeTo, targetObject, baseValueVector, ref args);
        }

        public static Vector3 GetEffectivePositionValue(
          RelativeTo relativeTo,
          IAnimatable targetObject,
          Vector3 baseValueVector,
          ref AnimationArgs args)
        {
            if (UISession.Default.IsRtl)
                baseValueVector.X = -baseValueVector.X;
            if (relativeTo == RelativeTo.Final)
                baseValueVector += args.NewPosition;
            else if (relativeTo == RelativeTo.Absolute)
            {
                if (UISession.Default.IsRtl)
                {
                    Vector2 vector2 = new Vector2(0.0f, 0.0f);
                    IVisualContainer visualContainer1 = null;
                    if (targetObject is IVisualContainer visualContainer)
                    {
                        vector2 = visualContainer.Size;
                        visualContainer1 = visualContainer.Parent;
                    }
                    else if (targetObject is ISprite sprite)
                    {
                        vector2 = sprite.Size;
                        visualContainer1 = sprite.Parent;
                    }
                    if (visualContainer1 != null)
                        baseValueVector.X = visualContainer1.Size.X - vector2.X + baseValueVector.X;
                }
            }
            else if (relativeTo is SnapshotRelativeTo snapshotRelativeTo)
            {
                RectangleF rectangleF = args.ViewItem.TransformFromAncestor(null, snapshotRelativeTo.Bounds);
                baseValueVector = baseValueVector + args.NewPosition + new Vector3(rectangleF.X, rectangleF.Y, 0.0f) * args.NewScale;
            }
            return baseValueVector;
        }

        public override void Apply(IAnimatableOwner animationTarget, Vector3 valueVector) => ((ViewItem)animationTarget).VisualPosition = valueVector;
    }
}
