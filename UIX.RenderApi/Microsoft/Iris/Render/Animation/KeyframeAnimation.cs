// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Animation.KeyframeAnimation
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Protocol;
using Microsoft.Iris.Render.Protocols.Splash.Rendering;
using System;

namespace Microsoft.Iris.Render.Animation
{
    internal class KeyframeAnimation :
      Microsoft.Iris.Render.Animation.Animation,
      IKeyframeAnimation,
      IAnimation,
      IRenderHandleOwner,
      IAnimatableObject,
      IAnimatable,
      ISharedRenderObject,
      IActivatableObject,
      IActivatable
    {
        private static LinearInterpolation s_defaultInterpolation = new LinearInterpolation();
        private AnimationSystem m_animationSystem;
        private RemoteAnimation m_remoteObject;
        private AnimationInput m_reference;
        private AnimationInput m_scale;
        private AnimationInputType m_animationType;
        private Vector m_keyframeList;
        private Vector m_targetList;
        private int m_repeatCount;
        private bool m_isActive;
        private bool m_isPlaying;
        private bool m_autoReset;
        private AnimationResetBehavior m_resetBehavior;

        internal KeyframeAnimation(AnimationSystem owner, AnimationInput initialValue)
        {
            this.m_animationSystem = owner;
            this.m_reference = null;
            this.m_scale = null;
            this.m_animationType = initialValue.InputType;
            this.m_keyframeList = new Vector();
            this.m_repeatCount = 0;
            this.m_autoReset = false;
            this.m_resetBehavior = AnimationResetBehavior.LeaveCurrent;
            this.m_animationSystem.RegisterUsage(this);
            this.m_remoteObject = owner.Session.BuildRemoteAnimation(this, this.m_animationType);
            if (this.m_animationSystem.BackCompat)
                return;
            this.AddKeyframe(new AnimationKeyframe(0.0f, initialValue, s_defaultInterpolation));
        }

        protected override void Dispose(bool inDispose)
        {
            try
            {
                if (inDispose)
                {
                    this.RemoveAllEvents();
                    this.RemoveAllTargets();
                    if (this.m_keyframeList != null)
                    {
                        foreach (AnimationKeyframe keyframe in this.m_keyframeList)
                            keyframe.Value.UnregisterUsage(this);
                        this.m_keyframeList.Clear();
                    }
                    if (this.m_remoteObject != null)
                        this.m_remoteObject.Dispose();
                    this.m_animationSystem.UnregisterUsage(this);
                }
                this.m_remoteObject = null;
                this.m_reference = null;
                this.m_keyframeList = null;
                this.m_targetList = null;
                this.m_animationSystem = null;
            }
            finally
            {
                base.Dispose(inDispose);
            }
        }

        public override int RepeatCount
        {
            get => this.m_repeatCount;
            set
            {
                if (this.m_repeatCount == value)
                    return;
                this.m_remoteObject.SendSetRepeatCount(value);
                this.m_repeatCount = value;
            }
        }

        public override bool IsPlaying => this.m_isPlaying;

        public override bool IsActive => this.m_isActive;

        public override bool AutoReset
        {
            get => this.m_autoReset;
            set
            {
                if (this.m_autoReset == value)
                    return;
                this.m_autoReset = value;
                this.m_remoteObject.SendSetAutoReset(value);
            }
        }

        public override AnimationResetBehavior ResetBehavior
        {
            get => this.m_resetBehavior;
            set
            {
                if (this.m_resetBehavior == value)
                    return;
                this.m_resetBehavior = value;
                this.m_remoteObject.SendSetResetBehavior(value);
            }
        }

        public override void Play()
        {
            this.m_isActive = true;
            this.m_isPlaying = true;
            this.m_remoteObject.SendPlay();
        }

        public override void Pause()
        {
            if (this.m_isActive)
                this.m_isPlaying = false;
            this.m_remoteObject.SendPause();
        }

        public override void Reset()
        {
            this.m_isActive = false;
            this.m_isPlaying = false;
            this.m_remoteObject.SendReset();
        }

        public override void InstantAdvance(float advanceTime)
        {
            Debug2.Validate(advanceTime >= 0.0, typeof(ArgumentOutOfRangeException), nameof(advanceTime));
            this.m_remoteObject.SendInstantAdvance(advanceTime);
        }

        public override void InstantFinish() => this.m_remoteObject.SendInstantFinish();

        public AnimationInputType Type => this.m_animationType;

        public AnimationInput InitialValue => this.GetKeyframe(0).Value;

        public int KeyframeCount => this.m_keyframeList.Count;

        public AnimationInput Reference
        {
            get => this.m_reference;
            set
            {
                this.SendInput(-1, value);
                this.m_reference = value;
            }
        }

        public AnimationInput Scale
        {
            get => this.m_scale;
            set
            {
                this.SendInput(-2, value);
                this.m_scale = value;
            }
        }

        void IKeyframeAnimation.AddKeyframe(AnimationKeyframe keyframe)
        {
            Debug2.Validate(keyframe != null, typeof(ArgumentNullException), nameof(keyframe));
            this.AddKeyframe(keyframe);
        }

        internal void AddKeyframe(AnimationKeyframe keyframe)
        {
            this.m_keyframeList.Add(keyframe);
            this.m_remoteObject.SendAddKeyframe(this.m_keyframeList.Count - 1, keyframe.Time);
            this.SetKeyframe(this.m_keyframeList.Count - 1, keyframe, false);
        }

        AnimationKeyframe IKeyframeAnimation.GetKeyframe(
          int keyframeIndex)
        {
            Debug2.Validate(keyframeIndex >= 0 || keyframeIndex < this.m_keyframeList.Count, typeof(ArgumentOutOfRangeException), nameof(keyframeIndex));
            return this.GetKeyframe(keyframeIndex);
        }

        internal AnimationKeyframe GetKeyframe(int keyframeIndex) => this.m_keyframeList[keyframeIndex] as AnimationKeyframe;

        void IKeyframeAnimation.SetKeyframe(
          int keyframeIndex,
          AnimationKeyframe keyframe)
        {
            Debug2.Validate(keyframe != null, typeof(ArgumentNullException), nameof(keyframe));
            Debug2.Validate(keyframeIndex >= 0 || keyframeIndex < this.m_keyframeList.Count, typeof(ArgumentOutOfRangeException), nameof(keyframeIndex));
            if (keyframeIndex == 0 && !this.m_animationSystem.BackCompat)
                Debug2.Validate(keyframe.Time == 0.0, typeof(ArgumentException), "Cannot change the time for keyframe 0");
            this.SetKeyframe(keyframeIndex, keyframe, true);
        }

        internal void SetKeyframe(int keyframeIndex, AnimationKeyframe keyframe, bool replaceKeyframe)
        {
            keyframe.Value.RegisterUsage(this);
            if (replaceKeyframe)
            {
                AnimationKeyframe keyframe1 = (AnimationKeyframe)this.m_keyframeList[keyframeIndex];
                this.m_keyframeList.RemoveAt(keyframeIndex);
                this.m_keyframeList.Insert(keyframeIndex, keyframe);
                keyframe1.Value.UnregisterUsage(this);
            }
            this.m_remoteObject.SendSetKeyframeTime(keyframeIndex, keyframe.Time);
            this.SendInput(keyframeIndex, keyframe.Value);
            this.SendInterpolation(keyframeIndex, keyframe.Interpolation);
        }

        void IKeyframeAnimation.AddTarget(
          IAnimatable targetObject,
          string targetPropertyName)
        {
            Debug2.Validate(targetObject is IAnimatableObject, typeof(ArgumentException), nameof(targetObject));
            this.AddTarget((IAnimatableObject)targetObject, targetPropertyName, null);
        }

        void IKeyframeAnimation.AddTarget(
          IAnimatable targetObject,
          string targetPropertyName,
          string targetMask)
        {
            Debug2.Validate(targetObject is IAnimatableObject, typeof(ArgumentException), nameof(targetObject));
            this.AddTarget((IAnimatableObject)targetObject, targetPropertyName, targetMask);
        }

        internal void AddTarget(
          IAnimatableObject targetObject,
          string targetPropertyName,
          string targetMaskSpec)
        {
            Debug2.Validate(targetObject != null, typeof(ArgumentNullException), nameof(targetObject));
            Debug2.Validate(targetPropertyName != null, typeof(ArgumentNullException), nameof(targetPropertyName));
            Debug2.Validate(!(targetObject is IAnimation), typeof(ArgumentException), "Animations can't be set as animation targets");
            AnimationTarget target = new AnimationTarget(targetObject, targetPropertyName);
            AnimationTypeMask sourceMask = AnimationTypeMask.FromString(targetMaskSpec);
            if (!sourceMask.CanMapFromType(this.m_animationType))
                throw new ArgumentException(string.Format("Mask cannot be applied to a {0} animation", m_animationType));
            if (!sourceMask.CanMapToType(target.TargetPropertyType))
                throw new ArgumentException(string.Format("Mask cannot be applied to a property of type {0}", target.TargetPropertyType));
            uint propertyInfo = this.GeneratePropertyInfo(target.TargetPropertyType, sourceMask);
            this.m_remoteObject.SendAddTarget(target.TargetId, target.TargetObject.GetObjectId(), target.TargetObject.GetPropertyId(target.TargetPropertyName), propertyInfo);
            if (this.m_targetList == null)
                this.m_targetList = new Vector();
            this.m_targetList.Add(new KeyframeAnimation.AnimationTargetInfo(target, targetMaskSpec));
        }

        void IKeyframeAnimation.RemoveTarget(
          IAnimatable targetObject,
          string targetPropertyName,
          string targetMaskSpec)
        {
            Debug2.Validate(targetObject != null, typeof(ArgumentNullException), nameof(targetObject));
            Debug2.Validate(targetPropertyName != null, typeof(ArgumentNullException), nameof(targetPropertyName));
            KeyframeAnimation.AnimationTargetInfo animationTargetInfo = null;
            if (this.m_targetList != null)
            {
                foreach (KeyframeAnimation.AnimationTargetInfo target in this.m_targetList)
                {
                    if (target.Target.TargetObject == targetObject && target.Target.TargetPropertyName == targetPropertyName && string.Compare(target.MaskSpec, targetMaskSpec, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        animationTargetInfo = target;
                        break;
                    }
                }
            }
            if (animationTargetInfo == null)
                return;
            this.m_remoteObject.SendRemoveTarget(animationTargetInfo.Target.TargetId);
            this.m_targetList.Remove(animationTargetInfo);
            animationTargetInfo.Target.Dispose();
        }

        void IKeyframeAnimation.RemoveAllTargets() => this.RemoveAllTargets();

        internal void RemoveAllTargets()
        {
            if (this.m_targetList == null)
                return;
            foreach (KeyframeAnimation.AnimationTargetInfo target in this.m_targetList)
            {
                this.m_remoteObject.SendRemoveTarget(target.Target.TargetId);
                target.Target.Dispose();
            }
            this.m_targetList.Clear();
        }

        void IKeyframeAnimation.AddStageEvent(
          AnimationStage animationStage,
          AnimationEvent animationEvent)
        {
            Debug2.Validate(animationEvent != null, typeof(ArgumentNullException), nameof(animationEvent));
            Debug2.Validate(animationStage < AnimationStage.Count, typeof(ArgumentException), "invalid animation stage");
            RENDERHANDLE targetObjectId = animationEvent.TargetObjectId;
            if (!(targetObjectId != RENDERHANDLE.NULL))
                return;
            this.m_remoteObject.SendAddStageEvent(animationEvent.EventId, animationStage, targetObjectId, animationEvent.TargetMethodId, animationEvent.TargetMethodArg, animationEvent.InitialActivation, animationEvent.AllowRepeat);
        }

        void IKeyframeAnimation.AddTimeEvent(
          float absoluteTime,
          AnimationEvent animationEvent)
        {
            Debug2.Validate(animationEvent != null, typeof(ArgumentNullException), nameof(animationEvent));
            Debug2.Validate(absoluteTime >= 0.0, typeof(ArgumentException), "absoluteTime value must be >= 0.0f");
            RENDERHANDLE targetObjectId = animationEvent.TargetObjectId;
            if (!(targetObjectId != RENDERHANDLE.NULL))
                return;
            this.m_remoteObject.SendAddTimeEvent(animationEvent.EventId, absoluteTime, targetObjectId, animationEvent.TargetMethodId, animationEvent.TargetMethodArg, animationEvent.InitialActivation, animationEvent.AllowRepeat);
        }

        void IKeyframeAnimation.AddProgressEvent(
          float progress,
          AnimationEvent animationEvent)
        {
            Debug2.Validate(animationEvent != null, typeof(ArgumentNullException), nameof(animationEvent));
            Debug2.Validate(progress >= 0.0 && progress <= 1.0, typeof(ArgumentException), "'progress' value must be between 0.0f and 1.0f");
            RENDERHANDLE targetObjectId = animationEvent.TargetObjectId;
            if (!(targetObjectId != RENDERHANDLE.NULL))
                return;
            this.m_remoteObject.SendAddProgressEvent(animationEvent.EventId, progress, targetObjectId, animationEvent.TargetMethodId, animationEvent.TargetMethodArg, animationEvent.InitialActivation, animationEvent.AllowRepeat);
        }

        void IKeyframeAnimation.AddValueEvent(
          ValueEventCondition condition,
          AnimationInput reference,
          AnimationEvent animationEvent)
        {
            Debug2.Validate(animationEvent != null, typeof(ArgumentNullException), nameof(animationEvent));
            Debug2.Validate(reference != null, typeof(ArgumentNullException), nameof(reference));
            Debug2.Validate(condition < ValueEventCondition.Count, typeof(ArgumentNullException), nameof(condition));
            RENDERHANDLE targetObjectId = animationEvent.TargetObjectId;
            if (!(targetObjectId != RENDERHANDLE.NULL))
                return;
            this.SendInput(-3, reference);
            this.m_remoteObject.SendAddValueEvent(animationEvent.EventId, condition, targetObjectId, animationEvent.TargetMethodId, animationEvent.TargetMethodArg, animationEvent.InitialActivation, animationEvent.AllowRepeat);
        }

        void IKeyframeAnimation.RemoveEvent(AnimationEvent animationEvent)
        {
            Debug2.Validate(animationEvent != null, typeof(ArgumentNullException), nameof(animationEvent));
            this.m_remoteObject.SendRemoveEvent(animationEvent.EventId);
        }

        void IKeyframeAnimation.RemoveAllEvents() => this.RemoveAllEvents();

        internal void RemoveAllEvents() => this.m_remoteObject.SendRemoveAllEvents();

        RENDERHANDLE IAnimatableObject.GetObjectId() => ((IRenderHandleOwner)this).RenderHandle;

        uint IAnimatableObject.GetPropertyId(string propertyName)
        {
            if (string.Compare(propertyName, OutputProperty, StringComparison.OrdinalIgnoreCase) == 0)
                return 1;
            Debug2.Validate(false, typeof(ArgumentException), "Unsupported property: {0}", propertyName);
            return 0;
        }

        AnimationInputType IAnimatableObject.GetPropertyType(
          string propertyName)
        {
            if (string.Compare(propertyName, OutputProperty, StringComparison.OrdinalIgnoreCase) == 0)
                return this.Type;
            Debug2.Validate(false, typeof(ArgumentException), "Unsupported property: {0}", propertyName);
            return AnimationInputType.Float;
        }

        RENDERHANDLE IActivatableObject.GetObjectId() => ((IRenderHandleOwner)this).RenderHandle;

        uint IActivatableObject.GetMethodId(string methodName)
        {
            if (string.Compare(methodName, PlayMethod, StringComparison.OrdinalIgnoreCase) == 0)
                return 1;
            if (string.Compare(methodName, PauseMethod, StringComparison.OrdinalIgnoreCase) == 0)
                return 2;
            if (string.Compare(methodName, ResetMethod, StringComparison.OrdinalIgnoreCase) == 0)
                return 3;
            if (string.Compare(methodName, FinishMethod, StringComparison.OrdinalIgnoreCase) == 0)
                return 4;
            if (string.Compare(methodName, NotifyMethod, StringComparison.OrdinalIgnoreCase) == 0)
                return 5;
            Debug2.Validate(false, typeof(ArgumentException), "Unsupported method: {0}", methodName);
            return 0;
        }

        RENDERHANDLE IRenderHandleOwner.RenderHandle => this.m_remoteObject.RenderHandle;

        void IRenderHandleOwner.OnDisconnect() => this.m_remoteObject = null;

        private void SendInput(int keyframeIndex, AnimationInput input)
        {
            Debug2.Validate(input != null, typeof(ArgumentNullException), "keyframe.Value.InputType");
            Debug2.Validate(input.InputType == this.m_animationType, typeof(ArgumentException), "keyframe.Value.InputType");
            Debug2.Validate(input.InputType == AnimationInputType.Float || input.InputType == AnimationInputType.Vector2 || (input.InputType == AnimationInputType.Vector3 || input.InputType == AnimationInputType.Vector4) || input.InputType == AnimationInputType.Quaternion, typeof(ArgumentException), "keyframe.Value.InputType");
            int num;
            switch (input)
            {
                case ConstantAnimationInput _:
                case ContinuousAnimationInput _:
                case CapturedAnimationInput _:
                    num = 1;
                    break;
                default:
                    num = input is BinaryOperation ? 1 : 0;
                    break;
            }
            System.Type exceptionType = typeof(ArgumentException);
            Debug2.Validate(num != 0, exceptionType, "Invalid animation input in keyframe");
            this.m_remoteObject.SendBeginAnimationInput(keyframeIndex);
            this.SendInputWorker(keyframeIndex, input);
            this.m_remoteObject.SendEndAnimationInput(keyframeIndex);
        }

        private void SendInputWorker(int keyframeIndex, AnimationInput input)
        {
            switch (input)
            {
                case ConstantAnimationInput _:
                    ConstantAnimationInput constantAnimationInput = (ConstantAnimationInput)input;
                    switch (constantAnimationInput.InputType)
                    {
                        case AnimationInputType.Float:
                            this.m_remoteObject.SendSetFloat(keyframeIndex, (float)constantAnimationInput.RawValue);
                            return;
                        case AnimationInputType.Vector2:
                            this.m_remoteObject.SendSetVector2(keyframeIndex, (Vector2)constantAnimationInput.RawValue);
                            return;
                        case AnimationInputType.Vector3:
                            this.m_remoteObject.SendSetVector3(keyframeIndex, (Vector3)constantAnimationInput.RawValue);
                            return;
                        case AnimationInputType.Vector4:
                            this.m_remoteObject.SendSetVector4(keyframeIndex, (Vector4)constantAnimationInput.RawValue);
                            return;
                        case AnimationInputType.Quaternion:
                            this.m_remoteObject.SendSetQuaternion(keyframeIndex, (Quaternion)constantAnimationInput.RawValue);
                            return;
                        default:
                            Debug2.Throw(false, "Unexpected constant input type");
                            return;
                    }
                case CapturedAnimationInput _:
                    uint propertyInfo1 = this.GeneratePropertyInfo(input.SourceType, input.SourceMask);
                    CapturedAnimationInput capturedAnimationInput = (CapturedAnimationInput)input;
                    this.m_remoteObject.SendSetCaptured(keyframeIndex, capturedAnimationInput.ObjectId, capturedAnimationInput.PropertyId, propertyInfo1, capturedAnimationInput.RefreshOnRepeat);
                    break;
                case ContinuousAnimationInput _:
                    uint propertyInfo2 = this.GeneratePropertyInfo(input.SourceType, input.SourceMask);
                    ContinuousAnimationInput continuousAnimationInput = (ContinuousAnimationInput)input;
                    this.m_remoteObject.SendSetContinuous(keyframeIndex, continuousAnimationInput.ObjectId, continuousAnimationInput.PropertyId, propertyInfo2);
                    break;
                case BinaryOperation _:
                    BinaryOperation binaryOperation = (BinaryOperation)input;
                    this.SendInputWorker(keyframeIndex, binaryOperation.LeftOperand);
                    this.SendInputWorker(keyframeIndex, binaryOperation.RightOperand);
                    this.m_remoteObject.SendSetBinaryOperation(keyframeIndex, binaryOperation.Operation);
                    break;
                default:
                    Debug2.Throw(false, "Unexpected animation input type!");
                    break;
            }
        }

        private void SendInterpolation(int keyframeIndex, AnimationInterpolation interpolation)
        {
            int num;
            switch (interpolation)
            {
                case LinearInterpolation _:
                case ExponentialInterpolation _:
                case LogarithmicInterpolation _:
                case SCurveInterpolation _:
                case SineInterpolation _:
                case CosineInterpolation _:
                case BezierInterpolation _:
                case EaseInInterpolation _:
                    num = 1;
                    break;
                default:
                    num = interpolation is EaseOutInterpolation ? 1 : 0;
                    break;
            }
            System.Type exceptionType = typeof(ArgumentException);
            Debug2.Validate(num != 0, exceptionType, "Invalid interpolation in keyframe");
            int idxKeyframe = !this.m_animationSystem.BackCompat ? (keyframeIndex > 0 ? keyframeIndex - 1 : 0) : keyframeIndex;
            switch (interpolation)
            {
                case LinearInterpolation _:
                    this.m_remoteObject.SendSetLinear(idxKeyframe, interpolation.UseSphericalCombination);
                    break;
                case ExponentialInterpolation _:
                    ExponentialInterpolation exponentialInterpolation = (ExponentialInterpolation)interpolation;
                    this.m_remoteObject.SendSetExponential(idxKeyframe, interpolation.UseSphericalCombination, exponentialInterpolation.Weight);
                    break;
                case LogarithmicInterpolation _:
                    LogarithmicInterpolation logarithmicInterpolation = (LogarithmicInterpolation)interpolation;
                    this.m_remoteObject.SendSetLogarithmic(idxKeyframe, interpolation.UseSphericalCombination, logarithmicInterpolation.Weight);
                    break;
                case SCurveInterpolation _:
                    SCurveInterpolation scurveInterpolation = (SCurveInterpolation)interpolation;
                    this.m_remoteObject.SendSetSCurve(idxKeyframe, interpolation.UseSphericalCombination, scurveInterpolation.Weight);
                    break;
                case SineInterpolation _:
                    this.m_remoteObject.SendSetSine(idxKeyframe, interpolation.UseSphericalCombination);
                    break;
                case CosineInterpolation _:
                    this.m_remoteObject.SendSetCosine(idxKeyframe, interpolation.UseSphericalCombination);
                    break;
                case BezierInterpolation _:
                    BezierInterpolation bezierInterpolation = (BezierInterpolation)interpolation;
                    this.m_remoteObject.SendSetBezier(idxKeyframe, interpolation.UseSphericalCombination, bezierInterpolation.ControlPoint1, bezierInterpolation.ControlPoint2);
                    break;
                case EaseInInterpolation _:
                    EaseInInterpolation easeInInterpolation = (EaseInInterpolation)interpolation;
                    this.m_remoteObject.SendSetEaseIn(idxKeyframe, interpolation.UseSphericalCombination, easeInInterpolation.Weight, easeInInterpolation.Handle);
                    break;
                case EaseOutInterpolation _:
                    EaseOutInterpolation outInterpolation = (EaseOutInterpolation)interpolation;
                    this.m_remoteObject.SendSetEaseOut(idxKeyframe, interpolation.UseSphericalCombination, outInterpolation.Weight, outInterpolation.Handle);
                    break;
                default:
                    Debug2.Throw(false, "Unexpected interpolation!");
                    break;
            }
        }

        private uint GeneratePropertyInfo(AnimationInputType sourceType, AnimationTypeMask sourceMask) => (uint)((int)(sourceType & (AnimationInputType.Vector4 | AnimationInputType.Quaternion)) << 19 | ((int)sourceMask.ChannelCount & 7) << 16 | sourceMask.MaskCode & ushort.MaxValue);

        private enum KeyframeSlots
        {
            ValueEvent = -3, // 0xFFFFFFFD
            Scale = -2, // 0xFFFFFFFE
            Reference = -1, // 0xFFFFFFFF
            Default = 0,
        }

        private class AnimationTargetInfo
        {
            internal AnimationTarget Target;
            internal string MaskSpec;

            internal AnimationTargetInfo(AnimationTarget target, string maskSpec)
            {
                this.Target = target;
                this.MaskSpec = maskSpec;
            }
        }
    }
}
