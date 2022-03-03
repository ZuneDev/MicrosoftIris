// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.BezierInterpolation
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

namespace Microsoft.Iris.Render
{
    public sealed class BezierInterpolation : AnimationInterpolation
    {
        private float m_controlPoint1;
        private float m_controlPoint2;

        public BezierInterpolation(float controlPoint1, float controlPoint2)
        {
            this.m_controlPoint1 = controlPoint1;
            this.m_controlPoint2 = controlPoint2;
        }

        internal float ControlPoint1 => this.m_controlPoint1;

        internal float ControlPoint2 => this.m_controlPoint2;
    }
}
