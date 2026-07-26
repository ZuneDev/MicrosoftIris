using System;

namespace Microsoft.Iris.Render.OpenGL
{
    /// <summary>
    /// Maps a keyframe's <see cref="AnimationInterpolation"/> to an eased factor in
    /// [0,1] for a normalized segment position <c>t</c>. The eased factor is then used
    /// to combine the two keyframe endpoint values (linear lerp, or slerp when
    /// <see cref="AnimationInterpolation.UseSphericalCombination"/> is set).
    ///
    /// The formulas below were recovered from the original native Splash engine
    /// (UIXrender.dll) via Ghidra and are exact, not approximations — see
    /// logs/UIX.RenderApi.OpenGL/Implementation.md (2026-07-25 "Animation curve
    /// formulas RECOVERED from native").
    ///
    /// NOTE: <see cref="EaseInInterpolation"/> and <see cref="EaseOutInterpolation"/>
    /// are value-space curves in the original — they build a computed intermediate
    /// control value between the endpoints and split the segment at <c>Handle</c>,
    /// so they cannot be reproduced exactly by a scalar factor fed to a straight
    /// A→B lerp. They are approximated here; see the TODO on those cases.
    /// </summary>
    internal static class AnimationEasing
    {
        public static float Ease(AnimationInterpolation? interpolation, float t)
        {
            if (t <= 0f) return 0f;
            if (t >= 1f) return 1f;

            switch (interpolation)
            {
                case LinearInterpolation:
                    return t;

                // f = sin(t·π/2)  (ease-out shape)
                case SineInterpolation:
                    return (float)Math.Sin(t * (Math.PI / 2.0));

                // f = 1 − cos(t·π/2)  (native: sin((t−1)·π/2) + 1; ease-in shape)
                case CosineInterpolation:
                    return 1f - (float)Math.Cos(t * (Math.PI / 2.0));

                // f = ExpEase(t, Weight)
                case ExponentialInterpolation exp:
                    return (float)ExpEase(t, exp.Weight);

                // f = ExpEase(t, 1/Weight)  (reciprocal exponent of Exponential)
                case LogarithmicInterpolation log:
                    return (float)ExpEase(t, 1.0 / log.Weight);

                // Symmetric S built from the weighted-exponential ease.
                case SCurveInterpolation sc:
                    return t < 0.5f
                        ? (float)(0.5 * ExpEase(2.0 * t, sc.Weight))
                        : (float)(0.5 + 0.5 * ExpEase(2.0 * (t - 0.5), 1.0 / sc.Weight));

                // Quintic Bézier (Bernstein degree 5) with control values
                // P0=0, P1=0, P2=cp1, P3=cp2, P4=1, P5=1.
                case BezierInterpolation bez:
                {
                    double u = 1.0 - t;
                    double t2 = t * t, t3 = t2 * t, t4 = t3 * t, t5 = t4 * t;
                    double u2 = u * u, u3 = u2 * u;
                    return (float)(10.0 * bez.ControlPoint1 * u3 * t2
                                 + 10.0 * bez.ControlPoint2 * u2 * t3
                                 + 5.0 * u * t4
                                 + t5);
                }

                // TODO: EaseIn/EaseOut are value-space in the original (they insert a
                // computed intermediate control value and split the segment at Handle;
                // see the log). A scalar factor cannot reproduce them exactly. As a
                // reasonable stand-in, use the first/second half of the weighted-exp
                // ease. Wiring the true value-space behavior needs changes in
                // GLKeyframeAnimation (compute the control value, pick the sub-segment).
                case EaseInInterpolation ein:
                    return (float)ExpEase(t, ein.Weight);
                case EaseOutInterpolation eout:
                    return (float)ExpEase(t, 1.0 / eout.Weight);

                default:
                    return t;
            }
        }

        /// <summary>
        /// The native weighted-exponential ease (UIXrender.dll <c>FUN_310bbf90</c>):
        /// <c>(w^x − 1) / (w − 1)</c>, collapsing to the identity when <c>w == 1</c>.
        /// Underlies Exponential, Logarithmic and SCurve.
        /// </summary>
        private static double ExpEase(double x, double w)
        {
            if (w == 1.0)
                return x;
            return (Math.Pow(w, x) - 1.0) / (w - 1.0);
        }
    }
}
