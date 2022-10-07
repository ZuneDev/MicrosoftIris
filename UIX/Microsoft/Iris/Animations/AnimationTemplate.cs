// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Animations.AnimationTemplate
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Render;
using Microsoft.Iris.Session;
using Microsoft.Iris.UI;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Iris.Animations
{
    public class AnimationTemplate : ICloneable
    {
        private const float k_epsilon = 0.0001f;
        private const StopCommand k_defaultStopCommand = StopCommand.MoveToEnd;
        private string _debugIDName;
        private List<BaseKeyframe> _keyframesList;
        private int _loopCount;
        private StopCommandSet _StopCommandSet;

        public AnimationTemplate()
          : this(null)
        {
        }

        public AnimationTemplate(string debugIDName)
        {
            _keyframesList = new List<BaseKeyframe>();
            DebugID = debugIDName;
        }

        public string DebugID
        {
            get => _debugIDName;
            set => _debugIDName = value;
        }

        public int Loop
        {
            get => _loopCount;
            set => _loopCount = value;
        }

        public List<BaseKeyframe> Keyframes => _keyframesList;

        public ActiveSequence Play(ViewItem vi)
        {
            AnimationArgs args = new AnimationArgs(vi);
            return Play(vi.RendererVisual, ref args, null);
        }

        public ActiveSequence Play(
          IVisual visualTarget,
          ref AnimationArgs args,
          EventHandler onCompleteHandler)
        {
            ActiveSequence instance = CreateInstance(visualTarget, ref args);
            if (onCompleteHandler != null)
                instance.AnimationCompleted += onCompleteHandler;
            instance.Play();
            return instance;
        }

        public ActiveSequence CreateInstance(
          IAnimatable animatableTarget,
          string property,
          ref AnimationArgs args)
        {
            if (_keyframesList.Count == 0)
            {
                ErrorManager.ReportError("Animations must have at least 2 keyframes to play");
                return null;
            }
            ActiveSequence aseq = new ActiveSequence(this, animatableTarget, UISession.Default);
            AnimationProxy[] animationProxyArray = new AnimationProxy[20];
            int[] numArray = new int[20];
            bool[] flagArray = new bool[20];
            foreach (BaseKeyframe keyframes in _keyframesList)
            {
                int type = (int)keyframes.Type;
                keyframes.AddtoAnimation(this, aseq, property, ref args, ref animationProxyArray[type]);
                ++numArray[type];
                flagArray[type] |= keyframes.Time == 0.0;
            }
            for (int index = 0; index <= 19; ++index)
            {
                if (animationProxyArray[index] != null)
                {
                    AnimationType animationType = (AnimationType)index;
                    if (numArray[index] < 2)
                    {
                        ErrorManager.ReportError("Animation must have at least 2 keyframes of each type. Attempted to play an animation that only has {0} keyframe of type '{1}'.", numArray[index], animationType);
                        return null;
                    }
                    if (!flagArray[index])
                    {
                        ErrorManager.ReportError("Animation must have a keyframe at time 0.0 for each type. Attempted to play an animation that has no start keyframe for type '{0}'", animationType);
                        return null;
                    }
                }
            }
            return !aseq.ValidatePlayable() ? null : aseq;
        }

        public ActiveSequence CreateInstance(
          IAnimatable animatableTarget,
          ref AnimationArgs args)
        {
            return CreateInstance(animatableTarget, null, ref args);
        }

        public void AddKeyframe(BaseKeyframe key) => InsertSorted(key);

        public BaseKeyframe GetKeyframe(float time)
        {
            int count = _keyframesList.Count;
            for (int index = 0; index < count; ++index)
            {
                BaseKeyframe keyframes = _keyframesList[index];
                if (IsSameTime(time, keyframes.Time))
                    return keyframes;
            }
            return null;
        }

        public void RemoveKeyframe(float time)
        {
            BaseKeyframe keyframe = GetKeyframe(time);
            if (keyframe == null)
                return;
            _keyframesList.Remove(keyframe);
        }

        public StopCommand GetStopCommand(AnimationType paramType)
        {
            AnimationSystem.ValidateAnimationType(paramType);
            StopCommand stopCommand = StopCommand.MoveToEnd;
            if (_StopCommandSet != null)
                stopCommand = _StopCommandSet[paramType];
            return stopCommand;
        }

        public void SetStopCommand(AnimationType paramType, StopCommand command)
        {
            AnimationSystem.ValidateAnimationType(paramType);
            if (_StopCommandSet == null)
                _StopCommandSet = new StopCommandSet(StopCommand.MoveToEnd);
            _StopCommandSet[paramType] = command;
        }

        object ICloneable.Clone() => Clone();

        public virtual object Clone()
        {
            AnimationTemplate anim = new AnimationTemplate(_debugIDName);
            CloneWorker(anim);
            return anim;
        }

        protected virtual void CloneWorker(AnimationTemplate anim)
        {
            anim._loopCount = _loopCount;
            int count = _keyframesList.Count;
            for (int index = 0; index < count; ++index)
                anim._keyframesList.Add(_keyframesList[index].Clone());
            anim._debugIDName = _debugIDName;
        }

        private void InsertSorted(BaseKeyframe key)
        {
            for (int index = _keyframesList.Count - 1; index >= 0; --index)
            {
                if (_keyframesList[index].Time < (double)key.Time)
                {
                    _keyframesList.Insert(index + 1, key);
                    return;
                }
            }
            _keyframesList.Insert(0, key);
        }

        private static bool IsSameTime(float t1, float t2)
        {
            float num = t2 - t1;
            if (num < 0.0)
                num *= -1f;
            return num < 9.99999974737875E-05;
        }

        internal class AnimationTemplateComparer : IComparer
        {
            public int Compare(object objx, object objy)
            {
                AnimationTemplate animationTemplate1 = objx as AnimationTemplate;
                AnimationTemplate animationTemplate2 = objy as AnimationTemplate;
                if (animationTemplate1 == null && animationTemplate2 == null)
                    return 0;
                if (animationTemplate1 == null)
                    return -1;
                return animationTemplate2 == null ? 1 : string.Compare(animationTemplate1.DebugID, animationTemplate2.DebugID, StringComparison.Ordinal);
            }
        }
    }
}
