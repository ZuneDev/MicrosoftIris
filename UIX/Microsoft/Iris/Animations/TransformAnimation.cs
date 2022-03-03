// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Animations.TransformAnimation
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System.Collections;

namespace Microsoft.Iris.Animations
{
    internal class TransformAnimation : ReferenceAnimation
    {
        private float _timeScaleValue;
        private float _timeOffsetValue;
        private float _magnitudeValue;
        private KeyframeFilter _filter;
        private AnimationTemplate _cacheAnimation;

        public TransformAnimation()
        {
            _timeScaleValue = 1f;
            _magnitudeValue = 1f;
            _filter = KeyframeFilter.All;
        }

        public float TimeScale
        {
            get => _timeScaleValue;
            set
            {
                if (_timeScaleValue == (double)value)
                    return;
                _timeScaleValue = value;
                ClearCache();
            }
        }

        public float Delay
        {
            get => _timeOffsetValue;
            set
            {
                if (_timeOffsetValue == (double)value)
                    return;
                _timeOffsetValue = value;
                ClearCache();
            }
        }

        public float Magnitude
        {
            get => _magnitudeValue;
            set
            {
                if (_magnitudeValue == (double)value)
                    return;
                _magnitudeValue = value;
                ClearCache();
            }
        }

        public KeyframeFilter Filter
        {
            get => _filter;
            set
            {
                if (_filter == value)
                    return;
                _filter = value;
                ClearCache();
            }
        }

        protected override AnimationTemplate BuildWorker(ref AnimationArgs args)
        {
            if (_cacheAnimation != null)
                return _cacheAnimation;
            float timeScale = GetTimeScale(ref args);
            float delayTime = GetDelayTime(ref args);
            float magnitude = GetMagnitude(ref args);
            bool flag1 = timeScale != 1.0;
            bool flag2 = delayTime != 0.0;
            bool flag3 = magnitude != 1.0;
            int filter = (int)_filter;
            AnimationTemplate anim1 = base.BuildWorker(ref args);
            DumpAnimation(anim1, "Source");
            if (!flag1 && !flag2 && !flag3)
                return anim1;
            AnimationTemplate anim2 = (AnimationTemplate)anim1.Clone();
            anim2.DebugID += "'";
            if (flag1)
                ApplyTimeScale(anim2, timeScale);
            if (flag2)
                ApplyTimeOffset(anim2, delayTime);
            if (flag3)
                ApplyMagnitude(anim2, magnitude);
            if (CanCache)
                _cacheAnimation = anim2;
            return anim2;
        }

        internal static void DumpAnimation(AnimationTemplate anim, string descriptionName)
        {
            foreach (BaseKeyframe keyframe in anim.Keyframes)
                ;
        }

        protected virtual float GetTimeScale(ref AnimationArgs args) => _timeScaleValue;

        protected virtual float GetDelayTime(ref AnimationArgs args) => _timeOffsetValue;

        protected virtual float GetMagnitude(ref AnimationArgs args) => _magnitudeValue;

        private void ApplyTimeScale(AnimationTemplate anim, float timeScaleValue)
        {
            foreach (BaseKeyframe keyframe in anim.Keyframes)
            {
                if (ShouldApplyTransform(keyframe))
                    keyframe.Time *= timeScaleValue;
            }
            DumpAnimation(anim, "Result");
        }

        private void ApplyTimeOffset(AnimationTemplate anim, float timeOffsetValue)
        {
            ArrayList arrayList = new ArrayList();
            foreach (BaseKeyframe keyframe in anim.Keyframes)
            {
                if (ShouldApplyTransform(keyframe))
                {
                    if (keyframe.Time == 0.0)
                        arrayList.Add(keyframe.Clone());
                    keyframe.Time += timeOffsetValue;
                }
            }
            foreach (BaseKeyframe key in arrayList)
                anim.AddKeyframe(key);
            DumpAnimation(anim, "Result");
        }

        private void ApplyMagnitude(AnimationTemplate anim, float magnitudeValue)
        {
            foreach (BaseKeyframe keyframe in anim.Keyframes)
            {
                if (ShouldApplyTransform(keyframe))
                    keyframe.MagnifyValue(magnitudeValue);
            }
            DumpAnimation(anim, "Result");
        }

        private KeyframeFilter GetKeyframeFilter(BaseKeyframe key)
        {
            switch (key.Type)
            {
                case AnimationType.Position:
                    return KeyframeFilter.Position;
                case AnimationType.Size:
                    return KeyframeFilter.Size;
                case AnimationType.Alpha:
                    return KeyframeFilter.Alpha;
                case AnimationType.Scale:
                    return KeyframeFilter.Scale;
                case AnimationType.Rotate:
                    return KeyframeFilter.Rotate;
                default:
                    return KeyframeFilter.All;
            }
        }

        private bool ShouldApplyTransform(BaseKeyframe key) => _filter == KeyframeFilter.All || GetKeyframeFilter(key) == _filter;

        protected override void OnSourceChanged() => ClearCache();

        protected void ClearCache() => _cacheAnimation = null;
    }
}
