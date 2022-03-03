// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.EaseInInterpolation
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using System;

namespace Microsoft.Iris.Render
{
    public sealed class EaseInInterpolation : AnimationInterpolation
    {
        private float m_weight;
        private float m_handle;

        public EaseInInterpolation(float weight, float handle)
        {
            Debug2.Validate(weight > 0.0, typeof(ArgumentOutOfRangeException), nameof(weight));
            Debug2.Validate(handle > 0.0 && handle < 1.0, typeof(ArgumentOutOfRangeException), nameof(handle));
            this.m_weight = weight;
            this.m_handle = handle;
        }

        internal float Weight => this.m_weight;

        internal float Handle => this.m_handle;
    }
}
