// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.ContinuousAnimationInput
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Animation;

namespace Microsoft.Iris.Render
{
    public sealed class ContinuousAnimationInput : ObjectAnimationInput
    {
        public ContinuousAnimationInput(IAnimatable sourceObject, string sourcePropertyName)
          : this(sourceObject, sourcePropertyName, null)
        {
        }

        public ContinuousAnimationInput(
          IAnimatable sourceObject,
          string sourcePropertyName,
          string sourceMaskSpec)
          : base(sourceObject, sourcePropertyName, sourceMaskSpec)
        {
        }

        public ContinuousAnimationInput(IAnimation sourceAnimation)
          : this(sourceAnimation, null)
        {
        }

        public ContinuousAnimationInput(IAnimation sourceAnimation, string sourceMaskSpec)
          : base(sourceAnimation, sourceMaskSpec)
        {
        }
    }
}
