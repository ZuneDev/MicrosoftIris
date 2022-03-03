// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.AnimationKeyframe
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using System;

namespace Microsoft.Iris.Render
{
    public sealed class AnimationKeyframe
    {
        private float m_time;
        private AnimationInput m_value;
        private AnimationInterpolation m_interpolation;

        public AnimationKeyframe(
          float time,
          AnimationInput value,
          AnimationInterpolation interpolation)
        {
            Debug2.Validate(time >= 0.0, typeof(ArgumentException), "'time' must not be negative");
            Debug2.Validate(value != null, typeof(ArgumentNullException), nameof(value));
            Debug2.Validate(interpolation != null, typeof(ArgumentNullException), nameof(interpolation));
            this.m_time = time;
            this.m_value = value;
            this.m_interpolation = interpolation;
        }

        public float Time => this.m_time;

        public AnimationInput Value => this.m_value;

        public AnimationInterpolation Interpolation => this.m_interpolation;
    }
}
