// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Animations.TransformByAttributeAnimation
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.UI;
using System;

namespace Microsoft.Iris.Animations
{
    internal class TransformByAttributeAnimation : TransformAnimation
    {
        private TransformAttribute _attrib;
        private float _maxTimeScaleValue;
        private float _maxTimeOffsetValue;
        private float _maxMagnitudeValue;
        private float _overrideValue;
        private bool _haveOverrideFlag;
        private ValueTransformer _valueTransformer;

        public TransformByAttributeAnimation()
        {
            Delay = 0.0f;
            Magnitude = 0.0f;
            TimeScale = 0.0f;
            Attribute = TransformAttribute.Index;
        }

        public TransformAttribute Attribute
        {
            get => _attrib;
            set => _attrib = value;
        }

        public float MaxTimeScale
        {
            get => _maxTimeScaleValue;
            set
            {
                if (_maxTimeScaleValue == (double)value)
                    return;
                _maxTimeScaleValue = value;
                ClearCache();
            }
        }

        public float MaxDelay
        {
            get => _maxTimeOffsetValue;
            set
            {
                if (_maxTimeOffsetValue == (double)value)
                    return;
                _maxTimeOffsetValue = value;
                ClearCache();
            }
        }

        public float MaxMagnitude
        {
            get => _maxMagnitudeValue;
            set
            {
                if (_maxMagnitudeValue == (double)value)
                    return;
                _maxMagnitudeValue = value;
                ClearCache();
            }
        }

        public float Override
        {
            get => _overrideValue;
            set
            {
                if (_haveOverrideFlag && _overrideValue == (double)value)
                    return;
                _overrideValue = value;
                _haveOverrideFlag = true;
                ClearCache();
            }
        }

        public ValueTransformer ValueTransformer
        {
            get => _valueTransformer;
            set
            {
                if (_valueTransformer == value)
                    return;
                _valueTransformer = value;
                ClearCache();
            }
        }

        protected override float GetTimeScale(ref AnimationArgs args)
        {
            float timeScale = base.GetTimeScale(ref args);
            if (timeScale == 0.0)
                return 1f;
            float val1 = 1f + GetValue(ref args) * timeScale;
            if (_maxTimeScaleValue != 0.0)
                val1 = Math.Min(val1, _maxTimeScaleValue);
            if (val1 < 0.0)
                val1 = 0.0f;
            return val1;
        }

        protected override float GetDelayTime(ref AnimationArgs args)
        {
            float delayTime = base.GetDelayTime(ref args);
            if (delayTime == 0.0)
                return 0.0f;
            float val1 = GetValue(ref args) * delayTime;
            if (_maxTimeOffsetValue != 0.0)
                val1 = Math.Min(val1, _maxTimeOffsetValue);
            if (val1 < 0.0)
                val1 = 0.0f;
            return val1;
        }

        protected override float GetMagnitude(ref AnimationArgs args)
        {
            float magnitude = base.GetMagnitude(ref args);
            if (magnitude == 0.0)
                return 1f;
            float val1 = 1f + GetValue(ref args) * magnitude;
            if (_maxMagnitudeValue != 0.0)
                val1 = Math.Min(val1, _maxMagnitudeValue);
            return val1;
        }

        private float GetValue(ref AnimationArgs args)
        {
            float num = GetValueWorker(ref args);
            if (_valueTransformer != null)
                num = _valueTransformer.Transform(num);
            return num;
        }

        protected virtual float GetValueWorker(ref AnimationArgs args)
        {
            if (_haveOverrideFlag)
                return _overrideValue;
            switch (_attrib)
            {
                case TransformAttribute.Index:
                    return GetIndex(ref args);
                case TransformAttribute.Width:
                    return args.NewSize.X;
                case TransformAttribute.Height:
                    return args.NewSize.Y;
                case TransformAttribute.X:
                    return args.NewPosition.X;
                case TransformAttribute.Y:
                    return args.NewPosition.Y;
                default:
                    return 0.0f;
            }
        }

        private int GetIndex(ref AnimationArgs args)
        {
            int num = 0;
            ViewItem viewItem = args.ViewItem;
            if (viewItem != null && viewItem.Parent != null)
                num = viewItem.Parent.Children.IndexOf(viewItem);
            return num;
        }

        public override bool CanCache => false;
    }
}
