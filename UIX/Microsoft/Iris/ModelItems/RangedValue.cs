// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.ModelItems.RangedValue
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;
using Microsoft.Iris.Markup;
using System;

namespace Microsoft.Iris.ModelItems
{
    internal class RangedValue : NotifyObjectBase, IUIRangedValue, IUIValueRange, INotifyObject
    {
        private float _value;
        private float _min;
        private float _max;
        private float _step;

        public RangedValue(float min, float max, float step)
        {
            _min = min;
            _max = max;
            _step = step;
        }

        public RangedValue()
          : this(float.MinValue, float.MaxValue, 1f)
        {
        }

        public float Value
        {
            get => _value;
            set
            {
                value = Math.Max(value, _min);
                value = Math.Min(value, _max);
                if (_value == (double)value)
                    return;
                using (new RangedValue.PrevNextNotifier(this))
                {
                    _value = value;
                    FireNotification(NotificationID.Value);
                    FireNotification(NotificationID.ObjectValue);
                }
            }
        }

        object IUIValueRange.ObjectValue => _value;

        public float MinValue
        {
            get => _min;
            set
            {
                if (_min == (double)value)
                    return;
                using (new RangedValue.PrevNextNotifier(this))
                {
                    _min = value;
                    FireNotification(NotificationID.MinValue);
                    FireNotification(NotificationID.Range);
                    Value = Value;
                }
            }
        }

        public float MaxValue
        {
            get => _max;
            set
            {
                if (_max == (double)value)
                    return;
                using (new RangedValue.PrevNextNotifier(this))
                {
                    _max = value;
                    FireNotification(NotificationID.MaxValue);
                    FireNotification(NotificationID.Range);
                    Value = Value;
                }
            }
        }

        public float Range => _max - _min;

        public float Step
        {
            get => _step;
            set
            {
                if (_step == (double)value)
                    return;
                using (new RangedValue.PrevNextNotifier(this))
                {
                    _step = value;
                    FireNotification(NotificationID.Step);
                }
            }
        }

        public bool HasPreviousValue => _step < 0.0 ? _value < (double)_max : _value > (double)_min;

        public bool HasNextValue => _step < 0.0 ? _value > (double)_min : _value < (double)_max;

        public void PreviousValue() => Value -= Step;

        public void NextValue() => Value += Step;

        private struct PrevNextNotifier : IDisposable
        {
            private bool _hadPrev;
            private bool _hadNext;
            private RangedValue _range;

            public PrevNextNotifier(RangedValue range)
            {
                _range = range;
                _hadPrev = range.HasPreviousValue;
                _hadNext = range.HasNextValue;
            }

            public void Dispose()
            {
                if (_range.HasPreviousValue != _hadPrev)
                    _range.FireNotification(NotificationID.HasPreviousValue);
                if (_range.HasNextValue == _hadNext)
                    return;
                _range.FireNotification(NotificationID.HasNextValue);
            }
        }
    }
}
