// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Drawing.Camera
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Animations;
using Microsoft.Iris.Library;
using Microsoft.Iris.Markup;
using Microsoft.Iris.Render;
using Microsoft.Iris.Session;
using Microsoft.Iris.UI;
using System;
using System.Collections;
using System.Diagnostics;

namespace Microsoft.Iris.Drawing
{
    internal class Camera : SharedDisposableObject, INotifyObject, IAnimatableOwner
    {
        private ICamera _camera;
        private Vector3 _vEyeNoSend;
        private Vector3 _vAtNoSend;
        private Vector3 _vUpNoSend;
        private float _flZnNoSend;
        private bool _hasEyeNoSend;
        private bool _hasAtNoSend;
        private bool _hasUpNoSend;
        private bool _hasZnNoSend;
        private Vector<ActiveSequence> _activeAnimations;
        private IAnimationProvider _eyeAnimation;
        private IAnimationProvider _atAnimation;
        private IAnimationProvider _upAnimation;
        private IAnimationProvider _znAnimation;
        private NotifyService _notifier = new NotifyService();
        private IList _listeners = new ArrayList();

        public Camera() => _camera = UISession.Default.RenderSession.CreateCamera(this);

        protected override void OnDispose()
        {
            base.OnDispose();
            Vector<ActiveSequence> activeAnimations = GetActiveAnimations(false);
            if (activeAnimations != null)
            {
                foreach (DisposableObject disposableObject in activeAnimations)
                    disposableObject.Dispose(this);
            }
            _camera.UnregisterUsage(this);
            _camera = null;
        }

        public void AddListener(Listener listener) => _notifier.AddListener(listener);

        protected void FireNotification(string id)
        {
            _notifier.Fire(id);
            foreach (object listener in _listeners)
            {
                if (listener is ViewItem viewItem)
                    viewItem.MarkPaintInvalid();
            }
        }

        public override void RegisterUsage(object consumer)
        {
            base.RegisterUsage(consumer);
            _listeners.Add(consumer);
        }

        public override void UnregisterUsage(object consumer)
        {
            _listeners.Remove(consumer);
            base.UnregisterUsage(consumer);
        }

        public bool Perspective
        {
            get => _camera.Perspective;
            set
            {
                if (_camera.Perspective == value)
                    return;
                _camera.Perspective = value;
                FireNotification(NotificationID.Perspective);
            }
        }

        public Vector3 Eye
        {
            get => _hasEyeNoSend ? _vEyeNoSend : _camera.Eye;
            set
            {
                if (!(_camera.Eye != value))
                    return;
                CameraEyeNoSend = value;
                if (_eyeAnimation == null || !PlayAnimation(_eyeAnimation, null))
                    CameraEye = value;
                FireNotification(NotificationID.Eye);
            }
        }

        private Vector3 CameraEyeNoSend
        {
            set
            {
                _hasEyeNoSend = true;
                _vEyeNoSend = value;
            }
        }

        internal Vector3 CameraEye
        {
            set
            {
                _camera.Eye = value;
                _hasEyeNoSend = false;
            }
        }

        public Vector3 At
        {
            get => _hasAtNoSend ? _vAtNoSend : _camera.At;
            set
            {
                if (!(_camera.At != value))
                    return;
                CameraAtNoSend = value;
                if (_atAnimation == null || !PlayAnimation(_atAnimation, null))
                    CameraAt = value;
                FireNotification(NotificationID.At);
            }
        }

        private Vector3 CameraAtNoSend
        {
            set
            {
                _hasAtNoSend = true;
                _vAtNoSend = value;
            }
        }

        internal Vector3 CameraAt
        {
            set
            {
                _camera.At = value;
                _hasAtNoSend = false;
            }
        }

        public Vector3 Up
        {
            get => _hasUpNoSend ? _vUpNoSend : _camera.Up;
            set
            {
                if (!(_camera.Up != value))
                    return;
                CameraUpNoSend = value;
                if (_upAnimation == null || !PlayAnimation(_upAnimation, null))
                    CameraUp = value;
                FireNotification(NotificationID.Up);
            }
        }

        private Vector3 CameraUpNoSend
        {
            set
            {
                _hasUpNoSend = true;
                _vUpNoSend = value;
            }
        }

        internal Vector3 CameraUp
        {
            set
            {
                _camera.Up = value;
                _hasUpNoSend = false;
            }
        }

        public float Zn
        {
            get => _hasZnNoSend ? _flZnNoSend : _camera.Zn;
            set
            {
                if (_camera.Zn == (double)value)
                    return;
                CameraZnNoSend = value;
                if (_znAnimation == null || !PlayAnimation(_znAnimation, null))
                    CameraZn = value;
                FireNotification(NotificationID.Zn);
            }
        }

        private float CameraZnNoSend
        {
            set
            {
                _hasZnNoSend = true;
                _flZnNoSend = value;
            }
        }

        internal float CameraZn
        {
            set
            {
                _camera.Zn = value;
                _hasZnNoSend = false;
            }
        }

        internal ICamera APICamera => _camera;

        IAnimatable IAnimatableOwner.AnimationTarget => _camera;

        public IAnimationProvider EyeAnimation
        {
            get => _eyeAnimation;
            set
            {
                if (_eyeAnimation == value)
                    return;
                _eyeAnimation = value;
                FireNotification(NotificationID.EyeAnimation);
            }
        }

        public IAnimationProvider AtAnimation
        {
            get => _atAnimation;
            set
            {
                if (_atAnimation == value)
                    return;
                _atAnimation = value;
                FireNotification(NotificationID.AtAnimation);
            }
        }

        public IAnimationProvider UpAnimation
        {
            get => _upAnimation;
            set
            {
                if (_upAnimation == value)
                    return;
                _upAnimation = value;
                FireNotification(NotificationID.UpAnimation);
            }
        }

        public IAnimationProvider ZnAnimation
        {
            get => _znAnimation;
            set
            {
                if (_znAnimation == value)
                    return;
                _znAnimation = value;
                FireNotification(NotificationID.ZnAnimation);
            }
        }

        public bool PlayAnimation(IAnimationProvider ab, AnimationHandle animationHandle)
        {
            AnimationArgs args = new AnimationArgs(this);
            return PlayAnimation(ab, ref args, UIClass.ShouldPlayAnimation(ab), animationHandle);
        }

        private bool PlayAnimation(
          IAnimationProvider ab,
          ref AnimationArgs args,
          bool shouldPlayAnimation,
          AnimationHandle animationHandle)
        {
            AnimationTemplate anim = BuildAnimation(ab, ref args);
            if (anim == null)
                return false;
            if (shouldPlayAnimation)
            {
                PlayAnimation(anim, ref args, null, animationHandle);
            }
            else
            {
                ApplyFinalAnimationState(anim, ref args);
                animationHandle?.FireCompleted();
            }
            return true;
        }

        private void PlayAnimation(
          AnimationTemplate anim,
          ref AnimationArgs args,
          EventHandler onCompleteHandler,
          AnimationHandle animationHandle)
        {
            ActiveSequence instance = anim.CreateInstance(APICamera, ref args);
            if (instance == null)
                return;
            instance.DeclareOwner(this);
            if (onCompleteHandler != null)
                instance.AnimationCompleted += onCompleteHandler;
            animationHandle?.AssociateWithAnimationInstance(instance);
            instance.AnimationCompleted += new EventHandler(OnAnimationComplete);
            GetActiveAnimations(true).Add(instance);
            OnAnimationListChanged();
            PlayAnimationWorker(instance);
            if (!(anim is Animation animation))
                return;
            int num = animation.DisableMouseInput ? 1 : 0;
        }

        private void PlayAnimationWorker(ActiveSequence newSequence)
        {
            StopOverlappingAnimations(newSequence);
            newSequence.Play();
        }

        private AnimationTemplate BuildAnimation(
          IAnimationProvider ab,
          ref AnimationArgs args)
        {
            return ab.Build(ref args);
        }

        private void StopOverlappingAnimations(ActiveSequence newSequence)
        {
            Vector<ActiveSequence> activeAnimations = GetActiveAnimations(false);
            if (activeAnimations == null)
                return;
            ActiveTransitions activeTransitions = newSequence.GetActiveTransitions();
            StopCommandSet stopCommand = null;
            foreach (ActiveSequence playingSequence in activeAnimations)
                StopAnimationIfOverlapping(playingSequence, newSequence, activeTransitions, ref stopCommand);
        }

        private bool StopAnimationIfOverlapping(
          ActiveSequence playingSequence,
          ActiveSequence newSequence,
          ActiveTransitions newTransitions,
          ref StopCommandSet stopCommand)
        {
            if (playingSequence.Target == newSequence.Target)
            {
                ActiveTransitions activeTransitions = playingSequence.GetActiveTransitions();
                if ((newTransitions & activeTransitions) != ActiveTransitions.None)
                {
                    if (stopCommand == null)
                    {
                        stopCommand = newSequence.GetStopCommandSet();
                        StopCommandSet stopCommandSet = stopCommand;
                    }
                    playingSequence.Stop(stopCommand);
                    return true;
                }
            }
            return false;
        }

        private void OnAnimationComplete(object sender, EventArgs args)
        {
            ActiveSequence activeSequence = sender as ActiveSequence;
            activeSequence.AnimationCompleted -= new EventHandler(OnAnimationComplete);
            Vector<ActiveSequence> activeAnimations = GetActiveAnimations(false);
            activeAnimations.Remove(activeSequence);
            OnAnimationListChanged();
            if (activeAnimations.Count == 0)
                _activeAnimations = null;
            if (activeSequence.Template is Animation template)
            {
                int num = template.DisableMouseInput ? 1 : 0;
            }
            activeSequence.Dispose(this);
        }

        private void StopActiveAnimations()
        {
            if (_activeAnimations == null)
                return;
            foreach (ActiveSequence activeAnimation in GetActiveAnimations(true))
                activeAnimation.Stop();
        }

        internal void ApplyFinalAnimationState(AnimationTemplate anim, ref AnimationArgs args)
        {
            BaseKeyframe[] baseKeyframeArray = new BaseKeyframe[20];
            foreach (BaseKeyframe keyframe in anim.Keyframes)
            {
                BaseKeyframe baseKeyframe = baseKeyframeArray[(uint)keyframe.Type];
                if (baseKeyframe == null || baseKeyframe.Time <= (double)keyframe.Time)
                    baseKeyframeArray[(uint)keyframe.Type] = keyframe;
            }
            foreach (BaseKeyframe baseKeyframe in baseKeyframeArray)
                baseKeyframe?.Apply(this, ref args);
        }

        private Vector<ActiveSequence> GetActiveAnimations(bool createIfNone) => GetAnimationSequence(ref _activeAnimations, createIfNone);

        private Vector<ActiveSequence> GetAnimationSequence(
          ref Vector<ActiveSequence> currentAnimationsList,
          bool createIfNone)
        {
            Vector<ActiveSequence> vector = null;
            if (currentAnimationsList == null)
            {
                if (createIfNone)
                {
                    vector = new Vector<ActiveSequence>();
                    currentAnimationsList = vector;
                }
            }
            else
                vector = currentAnimationsList;
            return vector;
        }

        private void OnAnimationListChanged()
        {
        }

        private static bool DoesAnimationListContainAnimationType(
          Vector<ActiveSequence> animationList,
          ActiveTransitions type)
        {
            if (animationList != null)
            {
                foreach (ActiveSequence animation in animationList)
                {
                    if ((animation.GetActiveTransitions() & type) == type)
                        return true;
                }
            }
            return false;
        }

        [Conditional("DEBUG")]
        private void DEBUG_DumpAnimation(ActiveSequence aseq)
        {
            foreach (BaseKeyframe keyframe in aseq.Template.Keyframes)
                ;
        }
    }
}
