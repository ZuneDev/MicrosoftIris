// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.ExponentialInterpolation
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using System;

namespace Microsoft.Iris.Render
{
    public sealed class ExponentialInterpolation : AnimationInterpolation
    {
        private float m_weight;

        public ExponentialInterpolation(float weight)
        {
            Debug2.Validate(weight > 0.0, typeof(ArgumentOutOfRangeException), "'weight' must be > 0");
            this.m_weight = weight;
        }

        internal float Weight => this.m_weight;
    }
}
