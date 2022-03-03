// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Animations.ActiveSequence
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;
using Microsoft.Iris.Render;
using Microsoft.Iris.Session;
using System;
using System.Text;

namespace Microsoft.Iris.Animations
{
    internal class ActiveSequence : DisposableObject
    {
        private UISession _session;
        private IAnimatable _animatableTarget;
        private Vector<AnimationProxy> _ready;
        private Vector<AnimationProxy> _playing;
        private float _lastProgress;
        private AnimationTemplate _template;
        private int _playingCount;

        internal ActiveSequence(
          AnimationTemplate template_,
          IAnimatable animatableTarget,
          UISession session)
        {
            _animatableTarget = animatableTarget;
            _session = session;
            _template = template_;
            _ready = new Vector<AnimationProxy>();
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            Vector<AnimationProxy> animationCollection = GetAnimationCollection();
            if (animationCollection != null)
            {
                foreach (DisposableObject disposableObject in animationCollection)
                    disposableObject.Dispose(this);
                FireComplete(false);
            }
            _session = null;
            _animatableTarget = null;
            _template = null;
            _ready = null;
        }

        public UISession Session => _session;

        internal IAnimatable Target => _animatableTarget;

        public AnimationTemplate Template => _template;

        public bool Playing => _playing != null;

        internal Vector<AnimationProxy> PendingProxies => _ready;

        public void Play()
        {
            Vector<AnimationProxy> ready = _ready;
            _ready = null;
            if (ready.Count > 0)
            {
                foreach (AnimationProxy animationProxy in ready)
                    animationProxy.Play();
                _playing = ready;
                FireStart();
            }
            else
            {
                FireStart();
                FireComplete(true);
            }
        }

        public void Stop() => Stop(null);

        public void Stop(StopCommandSet stopSetCommand)
        {
            if (UIDispatcher.IsUIThread)
            {
                StopWorker(stopSetCommand);
            }
            else
            {
                try
                {
                    if (!Session.IsValid)
                        return;
                    DeferredCall.Post(DispatchPriority.High, new DeferredHandler(DeferredStop), stopSetCommand);
                }
                catch (InvalidOperationException ex)
                {
                }
            }
        }

        private void StopWorker(StopCommandSet stopSetCommand)
        {
            if (_playing == null)
                return;
            foreach (AnimationProxy animationProxy in _playing)
            {
                if (stopSetCommand != null)
                    animationProxy.Stop(stopSetCommand[animationProxy.Type]);
                else
                    animationProxy.Stop();
            }
        }

        private void DeferredStop(object args) => StopWorker((StopCommandSet)args);

        public bool ValidatePlayable()
        {
            foreach (AnimationProxy animationProxy in _ready)
            {
                if (!animationProxy.ValidatePlayable())
                    return false;
            }
            return true;
        }

        internal void OnAttachChildAnimation(AnimationProxy child)
        {
            child.DeclareOwner(this);
            _ready.Add(child);
        }

        private Vector<AnimationProxy> GetAnimationCollection() => _ready ?? _playing;

        internal void OnDetachChildAnimation(AnimationProxy child, float progress)
        {
            Vector<AnimationProxy> animationCollection = GetAnimationCollection();
            if (animationCollection == null)
                return;
            animationCollection.Remove(child);
            child.Dispose(this);
            if (_lastProgress < (double)progress)
                _lastProgress = progress;
            if (animationCollection != _playing || animationCollection.Count != 0)
                return;
            FireComplete(true);
        }

        private void FireStart() => OnStart();

        private void FireComplete(bool notify)
        {
            if (_playing != null)
                _playing = null;
            OnStop(_lastProgress, notify);
        }

        public StopCommandSet GetStopCommandSet()
        {
            StopCommandSet stopCommandSet = null;
            foreach (AnimationProxy animation in GetAnimationCollection())
            {
                if (animation.HasDynamicKeyframes)
                {
                    if (stopCommandSet == null)
                        stopCommandSet = new StopCommandSet(StopCommand.MoveToEnd);
                    stopCommandSet[animation.Type] = StopCommand.LeaveCurrent;
                }
            }
            return stopCommandSet;
        }

        public ActiveTransitions GetActiveTransitions()
        {
            Vector<AnimationProxy> animationCollection = GetAnimationCollection();
            ActiveTransitions activeTransitions = ActiveTransitions.None;
            foreach (AnimationProxy animationProxy in animationCollection)
            {
                ActiveTransitions activeTransition = ConvertToActiveTransition(animationProxy.Type);
                activeTransitions |= activeTransition;
            }
            return activeTransitions;
        }

        public static ActiveTransitions ConvertToActiveTransition(AnimationType type)
        {
            switch (type)
            {
                case AnimationType.Position:
                    return ActiveTransitions.Move;
                case AnimationType.Size:
                    return ActiveTransitions.Size;
                case AnimationType.Alpha:
                    return ActiveTransitions.Alpha;
                case AnimationType.Scale:
                    return ActiveTransitions.Scale;
                case AnimationType.Rotate:
                    return ActiveTransitions.Rotate;
                case AnimationType.Orientation:
                    return ActiveTransitions.Orientation;
                case AnimationType.PositionX:
                    return ActiveTransitions.PositionX;
                case AnimationType.PositionY:
                    return ActiveTransitions.PositionY;
                case AnimationType.SizeX:
                    return ActiveTransitions.SizeX;
                case AnimationType.SizeY:
                    return ActiveTransitions.SizeY;
                case AnimationType.ScaleX:
                    return ActiveTransitions.ScaleX;
                case AnimationType.ScaleY:
                    return ActiveTransitions.ScaleY;
                case AnimationType.Float:
                case AnimationType.Vector2:
                case AnimationType.Vector3:
                    return ActiveTransitions.Effect;
                case AnimationType.CameraEye:
                    return ActiveTransitions.CameraEye;
                case AnimationType.CameraAt:
                    return ActiveTransitions.CameraAt;
                case AnimationType.CameraUp:
                    return ActiveTransitions.CameraUp;
                case AnimationType.CameraZn:
                    return ActiveTransitions.CameraZn;
                default:
                    return ActiveTransitions.None;
            }
        }

        public static ActiveTransitions ConvertToActiveTransition(
          AnimationEventType type)
        {
            switch (type)
            {
                case AnimationEventType.Move:
                    return ActiveTransitions.Move;
                case AnimationEventType.Size:
                    return ActiveTransitions.Size;
                case AnimationEventType.Scale:
                    return ActiveTransitions.Scale;
                case AnimationEventType.Rotate:
                    return ActiveTransitions.Rotate | ActiveTransitions.Orientation;
                case AnimationEventType.Alpha:
                    return ActiveTransitions.Alpha;
                default:
                    return ActiveTransitions.None;
            }
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("{ActiveSequence Template=\"");
            if (_template.DebugID != null)
            {
                stringBuilder.Append(_template.DebugID);
            }
            else
            {
                Animation template = _template as Animation;
                if (template != null)
                    stringBuilder.Append(template.Type);
                else
                    stringBuilder.Append("<Unknown>");
            }
            stringBuilder.Append("\", Target=");
            stringBuilder.Append(_animatableTarget.GetType().Name);
            if (_animatableTarget is IVisual)
            {
                stringBuilder.Append(", IVisual=");
                stringBuilder.Append(((IVisual)_animatableTarget).DebugID);
            }
            stringBuilder.Append("}");
            return stringBuilder.ToString();
        }

        public event EventHandler AnimationStarted;

        public event EventHandler AnimationCompleted;

        public event EventHandler AfterAnimationCompleted;

        internal void OnStart()
        {
            ++_playingCount;
            if (_playingCount != 1 || AnimationStarted == null)
                return;
            AnimationStarted(this, EventArgs.Empty);
        }

        internal void OnStop(float progress, bool notify)
        {
            --_playingCount;
            if (notify && _playingCount == 0)
            {
                EventArgs e = new AnimationCompleteArgs(progress);
                if (AnimationCompleted != null)
                    AnimationCompleted(this, e);
                if (AfterAnimationCompleted == null)
                    return;
                AfterAnimationCompleted(this, e);
            }
            else
            {
                int playingCount = _playingCount;
            }
        }
    }
}
