// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Animations.AnimationProxy
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Drawing;
using Microsoft.Iris.Library;
using Microsoft.Iris.Render;
using Microsoft.Iris.Session;

namespace Microsoft.Iris.Animations
{
    public class AnimationProxy : DisposableObject
    {
        private ActiveSequence _activeSequence;
        private IKeyframeAnimation _animation;
        private int _keyframesValue;
        private bool _playingFlag;
        private AnimationType _type;
        private bool _dynamicFlag;
        private bool _doNotAutoReleaseFlag;
        private IAnimatable _animatableTarget;
        private RendererProperty _rendererProperty;
        private static DeferredHandler s_deferredCleanupWorker = new DeferredHandler(DeferredCleanupWorker);

        internal AnimationProxy(
          ActiveSequence activeSequence,
          IAnimatable animatableTarget,
          AnimationType createType,
          RendererProperty rendererProperty,
          int loopCount,
          StopCommand stopCmd)
        {
            UISession.Validate(activeSequence.Session);
            AnimationSystem.ValidateAnimationType(createType);
            _activeSequence = activeSequence;
            _type = createType;
            _animatableTarget = animatableTarget;
            _rendererProperty = rendererProperty;
            _animation = _activeSequence.Session.AnimationManager.BuildAnimation(this);
            CommonCreate(loopCount, stopCmd);
        }

        private void CommonCreate(int loopCount, StopCommand stopCmd)
        {
            _animation.RepeatCount = loopCount;
            _animation.AsyncNotifyEvent += new AsyncNotifyHandler(OnAsyncNotification);
            _animation.AddStageEvent(AnimationStage.Complete, new AnimationEvent(_animation, "AsyncNotify", 1U));
            _animation.AddStageEvent(AnimationStage.Reset, new AnimationEvent(_animation, "AsyncNotify", 2U));
            SetStopCommand(stopCmd);
            _activeSequence.OnAttachChildAnimation(this);
        }

        protected override void OnDispose()
        {
            CleanupWorker(0.0f, false);
            base.OnDispose();
        }

        public UISession Session => _activeSequence.Session;

        public AnimationType Type => _type;

        public bool HasDynamicKeyframes => _dynamicFlag;

        internal bool DoNotAutoRelease
        {
            get => _doNotAutoReleaseFlag;
            set => _doNotAutoReleaseFlag = value;
        }

        public void AddFloatKeyframe(BaseKeyframe keyframe, float value)
        {
            ++_keyframesValue;
            AnimationInput animationInput1;
            if (keyframe.IsRelativeToObject)
            {
                _dynamicFlag = true;
                animationInput1 = keyframe.RelativeTo.CreateAnimationInput(_animatableTarget, _rendererProperty.Property, _rendererProperty.SourceMask);
                if (keyframe.Multiply && value != 1.0)
                {
                    AnimationInput animationInput2 = new ConstantAnimationInput(value);
                    animationInput1 *= animationInput2;
                }
                else if (!keyframe.Multiply && value != 0.0)
                {
                    AnimationInput animationInput2 = new ConstantAnimationInput(value);
                    animationInput1 += animationInput2;
                }
            }
            else
                animationInput1 = new ConstantAnimationInput(value);
            _animation.AddKeyframe(new AnimationKeyframe(keyframe.Time, animationInput1, GenerateInterpolation(keyframe.Interpolation)));
        }

        public void AddVector2Keyframe(BaseKeyframe keyframe, Vector2 value)
        {
            ++_keyframesValue;
            AnimationInput animationInput1;
            if (keyframe.IsRelativeToObject)
            {
                _dynamicFlag = true;
                animationInput1 = keyframe.RelativeTo.CreateAnimationInput(_animatableTarget, _rendererProperty.Property, _rendererProperty.SourceMask);
                if (keyframe.Multiply && value != Vector2.UnitVector)
                {
                    AnimationInput animationInput2 = new ConstantAnimationInput(value);
                    animationInput1 *= animationInput2;
                }
                else if (!keyframe.Multiply && value != Vector2.Zero)
                {
                    AnimationInput animationInput2 = new ConstantAnimationInput(value);
                    animationInput1 += animationInput2;
                }
            }
            else
                animationInput1 = new ConstantAnimationInput(value);
            _animation.AddKeyframe(new AnimationKeyframe(keyframe.Time, animationInput1, GenerateInterpolation(keyframe.Interpolation)));
        }

        public void AddVector3Keyframe(BaseKeyframe keyframe, Vector3 value)
        {
            ++_keyframesValue;
            AnimationInput animationInput1;
            if (keyframe.IsRelativeToObject)
            {
                _dynamicFlag = true;
                animationInput1 = keyframe.RelativeTo.CreateAnimationInput(_animatableTarget, _rendererProperty.Property, _rendererProperty.SourceMask);
                if (keyframe.Multiply && value != Vector3.UnitVector)
                {
                    AnimationInput animationInput2 = new ConstantAnimationInput(value);
                    animationInput1 *= animationInput2;
                }
                else if (!keyframe.Multiply && value != Vector3.Zero)
                {
                    AnimationInput animationInput2 = new ConstantAnimationInput(value);
                    animationInput1 += animationInput2;
                }
            }
            else
                animationInput1 = new ConstantAnimationInput(value);
            _animation.AddKeyframe(new AnimationKeyframe(keyframe.Time, animationInput1, GenerateInterpolation(keyframe.Interpolation)));
        }

        public void AddVector4Keyframe(BaseKeyframe keyframe, Vector4 value)
        {
            ++_keyframesValue;
            AnimationInput animationInput1;
            if (keyframe.IsRelativeToObject)
            {
                _dynamicFlag = true;
                animationInput1 = keyframe.RelativeTo.CreateAnimationInput(_animatableTarget, _rendererProperty.Property, _rendererProperty.SourceMask);
                if (keyframe.Multiply && value != Vector4.UnitVector)
                {
                    AnimationInput animationInput2 = new ConstantAnimationInput(value);
                    animationInput1 *= animationInput2;
                }
                else if (!keyframe.Multiply && value != Vector4.Zero)
                {
                    AnimationInput animationInput2 = new ConstantAnimationInput(value);
                    animationInput1 += animationInput2;
                }
            }
            else
                animationInput1 = new ConstantAnimationInput(value);
            _animation.AddKeyframe(new AnimationKeyframe(keyframe.Time, animationInput1, GenerateInterpolation(keyframe.Interpolation)));
        }

        public void AddRotationKeyframe(BaseKeyframe keyframe, Rotation value)
        {
            if (keyframe.Type != AnimationType.Orientation)
            {
                AddVector4Keyframe(keyframe, new Vector4(value.Axis.X, value.Axis.Y, value.Axis.Z, value.AngleRadians));
            }
            else
            {
                ++_keyframesValue;
                AnimationInput animationInput1;
                if (keyframe.IsRelativeToObject)
                {
                    _dynamicFlag = true;
                    animationInput1 = keyframe.RelativeTo.CreateAnimationInput(_animatableTarget, _rendererProperty.Property, _rendererProperty.SourceMask);
                    if (value != Rotation.Default)
                    {
                        AnimationInput animationInput2 = new ConstantAnimationInput(new Quaternion(value.Axis, value.AngleRadians));
                        animationInput1 *= animationInput2;
                    }
                }
                else
                    animationInput1 = new ConstantAnimationInput(new Quaternion(value.Axis, value.AngleRadians));
                AnimationInterpolation interpolation = GenerateInterpolation(keyframe.Interpolation);
                interpolation.UseSphericalCombination = true;
                _animation.AddKeyframe(new AnimationKeyframe(keyframe.Time, animationInput1, interpolation));
            }
        }

        public void SetStopCommand(StopCommand stopCmd)
        {
            switch (stopCmd)
            {
                case StopCommand.LeaveCurrent:
                    _animation.ResetBehavior = AnimationResetBehavior.LeaveCurrent;
                    break;
                case StopCommand.MoveToBegin:
                    _animation.ResetBehavior = AnimationResetBehavior.SetInitialValue;
                    break;
                case StopCommand.MoveToEnd:
                    _animation.ResetBehavior = AnimationResetBehavior.SetFinalValue;
                    break;
            }
            _animation.AutoReset = true;
        }

        public bool ValidatePlayable()
        {
            if (_keyframesValue >= 2)
                return true;
            ErrorManager.ReportError("Animations must have at least 2 keyframes to play");
            return false;
        }

        public void Play()
        {
            if (!ValidatePlayable())
                return;
            if (!_activeSequence.Session.AnimationManager.CanPlayAnimationType(_type))
            {
                Cleanup(0.0f, true);
            }
            else
            {
                if (_playingFlag || _animation == null || !Session.IsValid)
                    return;
                _animation.AddTarget(_animatableTarget, _rendererProperty.Property, _rendererProperty.TargetMask);
                _animation.Play();
                _playingFlag = true;
            }
        }

        public void Stop() => StopWorker(false, StopCommand.MoveToEnd);

        public void Stop(StopCommand stopCommand) => StopWorker(true, stopCommand);

        private void StopWorker(bool stopCommandFlag, StopCommand stopCommand)
        {
            if (!_playingFlag)
                return;
            _playingFlag = false;
            if (_animation == null || !Session.IsValid)
                return;
            if (stopCommandFlag)
                SetStopCommand(stopCommand);
            _animation.Reset();
            _animation.RemoveAllTargets();
        }

        private void Cleanup(float progress, bool forceDeferFlag)
        {
            if (_animation == null)
                return;
            if (UIDispatcher.IsUIThread && !forceDeferFlag)
            {
                CleanupWorker(progress, true);
            }
            else
            {
                if (!Session.IsValid)
                    return;
                DeferredCall.Post(DispatchPriority.Housekeeping, s_deferredCleanupWorker, this);
            }
        }

        private void CleanupWorker(float progress, bool withNotifications)
        {
            _playingFlag = false;
            if (_animation != null)
            {
                _animation.AsyncNotifyEvent -= new AsyncNotifyHandler(OnAsyncNotification);
                _animation.UnregisterUsage(this);
                _animation = null;
            }
            _animatableTarget = null;
            if (!withNotifications)
                return;
            _activeSequence.OnDetachChildAnimation(this, progress);
        }

        private static void DeferredCleanupWorker(object args) => (args as AnimationProxy).CleanupWorker(0.0f, true);

        private static AnimationInterpolation GenerateInterpolation(
          Interpolation interpolation)
        {
            if (interpolation == null)
                return new LinearInterpolation();
            switch (interpolation.Type)
            {
                case InterpolationType.Linear:
                    return new LinearInterpolation();
                case InterpolationType.SCurve:
                    return new SCurveInterpolation(interpolation.Weight * 10f);
                case InterpolationType.Exp:
                    return interpolation.Weight > 0.0 ? new ExponentialInterpolation(interpolation.Weight * 10f) : (AnimationInterpolation)new LinearInterpolation();
                case InterpolationType.Log:
                    return interpolation.Weight > 0.0 ? new LogarithmicInterpolation(interpolation.Weight * 10f) : (AnimationInterpolation)new LinearInterpolation();
                case InterpolationType.Sine:
                    return new SineInterpolation();
                case InterpolationType.Cosine:
                    return new CosineInterpolation();
                case InterpolationType.Bezier:
                    return new BezierInterpolation(interpolation.BezierHandle1, interpolation.BezierHandle2);
                case InterpolationType.EaseIn:
                    return new EaseInInterpolation(interpolation.Weight * 10f, interpolation.EasePercent);
                case InterpolationType.EaseOut:
                    return new EaseOutInterpolation(interpolation.Weight * 10f, interpolation.EasePercent);
                default:
                    return new LinearInterpolation();
            }
        }

        private void OnAsyncNotification(int nCookie)
        {
            switch (nCookie)
            {
                case 1:
                    Cleanup(0.0f, false);
                    break;
                case 2:
                    Cleanup(0.0f, false);
                    break;
            }
        }

        private enum AsyncNotifications
        {
            OnComplete = 1,
            OnReset = 2,
        }
    }
}
