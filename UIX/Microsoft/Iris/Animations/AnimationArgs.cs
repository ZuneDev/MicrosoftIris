// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Animations.AnimationArgs
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Drawing;
using Microsoft.Iris.Render;
using Microsoft.Iris.UI;

namespace Microsoft.Iris.Animations
{
    internal struct AnimationArgs
    {
        public ViewItem ViewItem;
        public Vector3 OldPosition;
        public Vector2 OldSize;
        public Vector3 OldScale;
        public Rotation OldRotation;
        public float OldAlpha;
        public Vector3 NewPosition;
        public Vector2 NewSize;
        public Vector3 NewScale;
        public Rotation NewRotation;
        public float NewAlpha;
        public Vector3 OldEye;
        public Vector3 OldAt;
        public Vector3 OldUp;
        public float OldZn;
        public Vector3 NewEye;
        public Vector3 NewAt;
        public Vector3 NewUp;
        public float NewZn;

        public AnimationArgs(Camera cam)
        {
            ViewItem = null;
            OldPosition = Vector3.Zero;
            OldSize = Vector2.Zero;
            OldScale = Vector3.Zero;
            OldRotation = Rotation.Default;
            OldAlpha = 0.0f;
            NewPosition = Vector3.Zero;
            NewSize = Vector2.Zero;
            NewScale = Vector3.Zero;
            NewRotation = Rotation.Default;
            NewAlpha = 0.0f;
            OldEye = cam.Eye;
            NewEye = cam.Eye;
            OldAt = cam.At;
            NewAt = cam.At;
            OldUp = cam.Up;
            NewUp = cam.Up;
            OldZn = cam.Zn;
            NewZn = cam.Zn;
        }

        public AnimationArgs(
          ViewItem vi,
          Vector3 oldPosition,
          Vector2 oldSize,
          Vector3 oldScale,
          Rotation oldRotation,
          float oldAlpha,
          Vector3 newPosition,
          Vector2 newSize,
          Vector3 newScale,
          Rotation newRotation,
          float newAlpha)
        {
            ViewItem = vi;
            OldPosition = oldPosition;
            OldSize = oldSize;
            OldScale = oldScale;
            OldRotation = oldRotation;
            OldAlpha = oldAlpha;
            NewPosition = newPosition;
            NewSize = newSize;
            NewScale = newScale;
            NewRotation = newRotation;
            NewAlpha = newAlpha;
            OldEye = Vector3.Zero;
            NewEye = Vector3.Zero;
            OldAt = Vector3.Zero;
            NewAt = Vector3.Zero;
            OldUp = Vector3.Zero;
            NewUp = Vector3.Zero;
            OldZn = 0.0f;
            NewZn = 0.0f;
        }

        public AnimationArgs(
          ViewItem vi,
          Vector3 oldPosition,
          Vector2 oldSize,
          Vector3 oldScale,
          Rotation oldRotation,
          float oldAlpha)
          : this(vi, oldPosition, oldSize, oldScale, oldRotation, oldAlpha, oldPosition, oldSize, oldScale, oldRotation, oldAlpha)
        {
        }

        public AnimationArgs(ViewItem vi)
          : this(vi, vi.VisualPosition, vi.VisualSize, vi.VisualScale, vi.VisualRotation, vi.VisualAlpha, vi.VisualPosition, vi.VisualSize, vi.VisualScale, vi.VisualRotation, vi.VisualAlpha)
        {
        }
    }
}
