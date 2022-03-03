// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.RenderAPI.Drawing.EdgeFade
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Drawing;
using Microsoft.Iris.Render;
using Microsoft.Iris.Session;
using System;

namespace Microsoft.Iris.RenderAPI.Drawing
{
    internal class EdgeFade
    {
        private IGradient _minFadeGradient;
        private IGradient _maxFadeGradient;
        private float _fadeSizeValue;
        private float _fadeAmountValue;
        private float _minOffsetValue;
        private float _maxOffsetValue;
        private Orientation _orientation;
        private Color _maskColor;

        public EdgeFade()
        {
            _orientation = Orientation.Horizontal;
            _maskColor = Color.FromArgb(byte.MaxValue, 0, 0, 0);
            _fadeAmountValue = 1f;
        }

        public void Dispose() => DisposeGradients();

        private void DisposeGradients()
        {
            if (_minFadeGradient != null)
            {
                _minFadeGradient.UnregisterUsage(this);
                _minFadeGradient = null;
            }
            if (_maxFadeGradient == null)
                return;
            _maxFadeGradient.UnregisterUsage(this);
            _maxFadeGradient = null;
        }

        public float FadeSize
        {
            get => _fadeSizeValue;
            set
            {
                if (_fadeSizeValue == (double)value)
                    return;
                _fadeSizeValue = value;
                UpdateFades(true);
            }
        }

        public float FadeAmount
        {
            get => _fadeAmountValue;
            set
            {
                if (value < 0.0 || value > 1.0)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "FadeAmount must be between 0.0 and 1.0.");
                if (_fadeAmountValue == (double)value)
                    return;
                _fadeAmountValue = value;
                UpdateFades(true);
            }
        }

        public float MinOffset
        {
            get => _minOffsetValue;
            set
            {
                if (_minOffsetValue == (double)value)
                    return;
                _minOffsetValue = value;
                UpdateFades(true);
            }
        }

        public float MaxOffset
        {
            get => _maxOffsetValue;
            set
            {
                if (_maxOffsetValue == (double)value)
                    return;
                _maxOffsetValue = value;
                UpdateFades(true);
            }
        }

        public Orientation Orientation
        {
            get => _orientation;
            set
            {
                if (_orientation == value)
                    return;
                _orientation = value;
                UpdateFades(false);
            }
        }

        public Color ColorMask
        {
            get => _maskColor;
            set
            {
                if (!(_maskColor != value))
                    return;
                _maskColor = value;
                UpdateFades(false);
            }
        }

        internal void ApplyGradients(
          IVisualContainer visContainer,
          IRenderSession renderSession,
          bool minFlag,
          bool maxFlag)
        {
            visContainer.RemoveAllGradients();
            CreateFades(renderSession);
            UpdateFades(true);
            if (minFlag && _minFadeGradient != null)
                visContainer.AddGradient(_minFadeGradient);
            if (!maxFlag || _maxFadeGradient == null)
                return;
            visContainer.AddGradient(_maxFadeGradient);
        }

        internal bool NeedFades => _fadeSizeValue != 0.0 && _fadeAmountValue != 0.0;

        private void CreateFades(IRenderSession renderSession)
        {
            if (!NeedFades)
                return;
            if (_minFadeGradient == null)
            {
                _minFadeGradient = renderSession.CreateGradient(this);
                _minFadeGradient.Orientation = _orientation;
                _minFadeGradient.ColorMask = _maskColor.RenderConvert();
            }
            if (_maxFadeGradient != null)
                return;
            _maxFadeGradient = renderSession.CreateGradient(this);
            _maxFadeGradient.Orientation = _orientation;
            _maxFadeGradient.ColorMask = _maskColor.RenderConvert();
        }

        private void UpdateFades(bool isOffsetChange)
        {
            if (!NeedFades)
            {
                DisposeGradients();
            }
            else
            {
                if (_minFadeGradient == null)
                    return;
                _minFadeGradient.Orientation = _orientation;
                _maxFadeGradient.Orientation = _orientation;
                _minFadeGradient.ColorMask = _maskColor.RenderConvert();
                _maxFadeGradient.ColorMask = _maskColor.RenderConvert();
                if (!isOffsetChange)
                    return;
                _minFadeGradient.Clear();
                _maxFadeGradient.Clear();
                float flValue1 = 1f;
                float flValue2 = 1f - _fadeAmountValue;
                float flPosition1;
                float flPosition2;
                if (!UISession.Default.IsRtl || _orientation == Orientation.Vertical)
                {
                    flPosition1 = _minOffsetValue;
                    flPosition2 = _maxOffsetValue;
                }
                else
                {
                    flPosition1 = -_maxOffsetValue;
                    flPosition2 = -_minOffsetValue;
                }
                if (FadeSize > 0.0)
                {
                    _minFadeGradient.AddValue(flPosition1, flValue2, RelativeSpace.Min);
                    _minFadeGradient.AddValue(flPosition1 + FadeSize, flValue1, RelativeSpace.Min);
                    _maxFadeGradient.AddValue(flPosition2 - FadeSize, flValue1, RelativeSpace.Max);
                    _maxFadeGradient.AddValue(flPosition2, flValue2, RelativeSpace.Max);
                }
                else
                {
                    _minFadeGradient.AddValue(flPosition1 + FadeSize, flValue2, RelativeSpace.Min);
                    _minFadeGradient.AddValue(flPosition1, flValue1, RelativeSpace.Min);
                    _maxFadeGradient.AddValue(flPosition2, flValue1, RelativeSpace.Max);
                    _maxFadeGradient.AddValue(flPosition2 - FadeSize, flValue2, RelativeSpace.Max);
                }
            }
        }
    }
}
