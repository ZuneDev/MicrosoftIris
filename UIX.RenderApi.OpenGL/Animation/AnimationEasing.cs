using System;

namespace Microsoft.Iris.Render.OpenGL
{
    /// <summary>
    /// Maps a keyframe's <see cref="AnimationInterpolation"/> to an eased parameter.
    /// The original curves are evaluated in native code, so these are standard easings
    /// matching each curve's name/family (see logs/UIX.RenderApi.OpenGL/Implementation.md).
    /// </summary>
    internal static class AnimationEasing
    {
        public static float Ease(AnimationInterpolation? interpolation, float t)
        {
            if (t <= 0f) return 0f;
            if (t >= 1f) return 1f;

            return interpolation switch
            {
                LinearInterpolation => t,
                EaseInInterpolation => t * t,
                EaseOutInterpolation => t * (2f - t),
                SCurveInterpolation => t * t * (3f - 2f * t),
                SineInterpolation => 0.5f * (1f - (float)Math.Cos(Math.PI * t)),
                CosineInterpolation => 1f - (float)Math.Cos(t * (Math.PI / 2.0)),
                ExponentialInterpolation => (float)Math.Pow(2.0, 10.0 * (t - 1.0)),
                LogarithmicInterpolation => 1f - (float)Math.Pow(2.0, -10.0 * t),
                // Bezier control points are internal to the curve; smoothstep is a
                // reasonable stand-in until they can be read.
                BezierInterpolation => t * t * (3f - 2f * t),
                _ => t,
            };
        }
    }
}
