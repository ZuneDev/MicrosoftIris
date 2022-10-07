// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Animations.AnimationManager
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;
using Microsoft.Iris.Render;
using System;

namespace Microsoft.Iris.Animations
{
    public class AnimationManager : IDisposable
    {
        private IRenderSession _session;
        private Vector<OrphanedVisualCollection> _orphans;
        private bool _shuttingDown;
        private static ConstantAnimationInput s_defaultPositionInput = new ConstantAnimationInput(Vector3.Zero);
        private static ConstantAnimationInput s_defaultSizeInput = new ConstantAnimationInput(Vector2.Zero);
        private static ConstantAnimationInput s_defaultAlphaInput = new ConstantAnimationInput(0.0f);
        private static ConstantAnimationInput s_defaultScaleInput = new ConstantAnimationInput(Vector3.UnitVector);
        private static ConstantAnimationInput s_defaultRotationInput = new ConstantAnimationInput(AxisAngle.Identity.ToVector4());
        private static ConstantAnimationInput s_defaultOrientationInput = new ConstantAnimationInput(AxisAngle.Identity.ToQuaternion());
        private static ConstantAnimationInput s_defaultFloatInput = new ConstantAnimationInput(0.0f);
        private static ConstantAnimationInput s_defaultVector2Input = new ConstantAnimationInput(Vector2.Zero);
        private static ConstantAnimationInput s_defaultVector3Input = new ConstantAnimationInput(Vector3.Zero);
        private static ConstantAnimationInput s_defaultVector4Input = new ConstantAnimationInput(Vector4.Zero);
        private static ConstantAnimationInput s_defaultCameraEyeInput = new ConstantAnimationInput(new Vector3(0.0f, 0.0f, -4f));
        private static ConstantAnimationInput s_defaultCameraAtInput = new ConstantAnimationInput(new Vector3(0.0f, 0.0f, 0.0f));
        private static ConstantAnimationInput s_defaultCameraUpInput = new ConstantAnimationInput(new Vector3(0.0f, 1f, 0.0f));
        private static ConstantAnimationInput s_defaultCameraZnInput = new ConstantAnimationInput(2f);

        public AnimationManager(IRenderSession session)
        {
            _session = session;
            _orphans = new Vector<OrphanedVisualCollection>();
            _session.AnimationSystem.UpdatesPerSecond = session.GraphicsDevice.DeviceType != GraphicsDeviceType.Gdi ? 0 : 3;
            _session.AnimationSystem.BackCompat = true;
        }

        ~AnimationManager() => Dispose(false);

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        protected virtual void Dispose(bool inDisposeFlag)
        {
            _shuttingDown = true;
            if (!inDisposeFlag)
                return;
            foreach (DisposableObject orphan in _orphans)
                orphan.Dispose(this);
            _orphans.Clear();
        }

        internal bool ShuttingDown => _shuttingDown;

        public void SetGlobalSpeedAdjustment(float value)
        {
            ValidateConnected();
            _session.AnimationSystem.SpeedAdjustment = value;
        }

        public void PulseTimeAdvance(int pulseSize)
        {
            ValidateConnected();
            _session.AnimationSystem.PulseTimeAdvance(pulseSize);
        }

        public void RegisterAnimatedOrphans(OrphanedVisualCollection orphan) => _orphans.Add(orphan);

        public void UnregisterAnimatedOrphans(OrphanedVisualCollection orphan) => _orphans.Remove(orphan);

        public bool CanPlayAnimationType(AnimationType type) => _session.GraphicsDevice.DeviceType != GraphicsDeviceType.Gdi || type == AnimationType.Position || (type == AnimationType.Size || type == AnimationType.Scale) || type == AnimationType.Alpha;

        private void ValidateConnected()
        {
        }

        internal IKeyframeAnimation BuildAnimation(AnimationProxy owner)
        {
            ValidateConnected();
            IKeyframeAnimation keyframeAnimation = null;
            switch (owner.Type)
            {
                case AnimationType.Position:
                    keyframeAnimation = _session.AnimationSystem.CreateKeyframeAnimation(owner, s_defaultPositionInput);
                    break;
                case AnimationType.Size:
                    keyframeAnimation = _session.AnimationSystem.CreateKeyframeAnimation(owner, s_defaultSizeInput);
                    break;
                case AnimationType.Alpha:
                    keyframeAnimation = _session.AnimationSystem.CreateKeyframeAnimation(owner, s_defaultAlphaInput);
                    break;
                case AnimationType.Scale:
                    keyframeAnimation = _session.AnimationSystem.CreateKeyframeAnimation(owner, s_defaultScaleInput);
                    break;
                case AnimationType.Rotate:
                    keyframeAnimation = _session.AnimationSystem.CreateKeyframeAnimation(owner, s_defaultRotationInput);
                    break;
                case AnimationType.Orientation:
                    keyframeAnimation = _session.AnimationSystem.CreateKeyframeAnimation(owner, s_defaultOrientationInput);
                    break;
                case AnimationType.PositionX:
                case AnimationType.PositionY:
                case AnimationType.SizeX:
                case AnimationType.SizeY:
                case AnimationType.ScaleX:
                case AnimationType.ScaleY:
                case AnimationType.Float:
                    keyframeAnimation = _session.AnimationSystem.CreateKeyframeAnimation(owner, s_defaultFloatInput);
                    break;
                case AnimationType.Vector2:
                    keyframeAnimation = _session.AnimationSystem.CreateKeyframeAnimation(owner, s_defaultVector2Input);
                    break;
                case AnimationType.Vector3:
                    keyframeAnimation = _session.AnimationSystem.CreateKeyframeAnimation(owner, s_defaultVector3Input);
                    break;
                case AnimationType.Vector4:
                    keyframeAnimation = _session.AnimationSystem.CreateKeyframeAnimation(owner, s_defaultVector4Input);
                    break;
                case AnimationType.CameraEye:
                    keyframeAnimation = _session.AnimationSystem.CreateKeyframeAnimation(owner, s_defaultCameraEyeInput);
                    break;
                case AnimationType.CameraAt:
                    keyframeAnimation = _session.AnimationSystem.CreateKeyframeAnimation(owner, s_defaultCameraAtInput);
                    break;
                case AnimationType.CameraUp:
                    keyframeAnimation = _session.AnimationSystem.CreateKeyframeAnimation(owner, s_defaultCameraUpInput);
                    break;
                case AnimationType.CameraZn:
                    keyframeAnimation = _session.AnimationSystem.CreateKeyframeAnimation(owner, s_defaultCameraZnInput);
                    break;
            }
            return keyframeAnimation;
        }
    }
}
