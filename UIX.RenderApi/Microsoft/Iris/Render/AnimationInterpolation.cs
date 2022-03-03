// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.AnimationInterpolation
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

namespace Microsoft.Iris.Render
{
    public abstract class AnimationInterpolation
    {
        private bool m_isSpherical;

        protected AnimationInterpolation() => this.m_isSpherical = false;

        public bool UseSphericalCombination
        {
            get => this.m_isSpherical;
            set => this.m_isSpherical = value;
        }
    }
}
