using System;
using System.Collections.Generic;

namespace Microsoft.Iris.Render.OpenGL.Scene
{
    /// <summary>
    /// A piecewise-linear alpha ramp that can be attached to a visual (edge/scroll fades
    /// via <see cref="Microsoft.Iris.Render.IVisualContainer.AddGradient"/>, text clip
    /// fades via <see cref="Microsoft.Iris.Render.ISprite.AddGradient"/> -- see
    /// UIX/Microsoft/Iris/RenderAPI/Drawing/EdgeFade.cs and
    /// UIX/Microsoft/Iris/ViewItems/Text.cs for the original call sites). We record the
    /// stops and resolve them (<see cref="ResolveStops"/>) against the owning visual's
    /// extent into absolute local-pixel coordinates; the actual per-pixel ramp is
    /// evaluated on the GPU in FragmentShader.glsl, not sampled on the CPU (a prior
    /// CPU single-center-sample attempt caused widespread incorrect-alpha regressions --
    /// see logs/UIX.RenderApi.OpenGL/Implementation.md).
    ///
    /// Two assumptions, undecided by any decompiled call site (no source sets
    /// <see cref="ColorMask"/> or <see cref="Offset"/> to a non-default value anywhere in
    /// the outer repo or this submodule):
    /// - <see cref="ColorMask"/> is recorded but not applied. No decompiled call site
    ///   indicates whether it tints the faded region toward a color or is unused API
    ///   surface; TODO: cross-check against the native Splash engine before applying it.
    /// - <see cref="RelativeSpace.Global"/> is treated identically to
    ///   <see cref="RelativeSpace.Min"/> (no distinguishing call site found).
    /// </summary>
    public sealed class GLGradient : SharedRenderObject, IGradient
    {
        internal readonly struct Stop
        {
            public readonly float Position;
            public readonly float Value;
            public readonly RelativeSpace Space;
            public Stop(float position, float value, RelativeSpace space)
            {
                Position = position;
                Value = value;
                Space = space;
            }
        }

        private readonly List<Stop> m_stops = new List<Stop>();
        private Orientation m_orientation;
        private ColorF m_colorMask = new ColorF(1f, 0f, 0f, 0f);
        private float m_offset;

        // Now read from the render thread every frame (ResolveStops, via
        // GLVisual.ResolveOwnGradients), while AddValue/Clear/the property setters below
        // are called from the app thread -- so, unlike a gradient's identity/lifetime
        // (SharedRenderObject's own private lock, still fine for RegisterUsage/
        // UnregisterUsage), stop/property mutation now needs to share the owning
        // GLRenderSession's lock like the rest of the scene graph does.
        internal GLGradient(object syncRoot) : base(syncRoot)
        {
        }

        public Orientation Orientation
        {
            get { lock (SyncRoot) return m_orientation; }
            set { lock (SyncRoot) m_orientation = value; }
        }

        public ColorF ColorMask
        {
            get { lock (SyncRoot) return m_colorMask; }
            set { lock (SyncRoot) m_colorMask = value; }
        }

        public float Offset
        {
            get { lock (SyncRoot) return m_offset; }
            set { lock (SyncRoot) m_offset = value; }
        }

        public void AddValue(float flPosition, float flValue, RelativeSpace rsSpace)
        {
            lock (SyncRoot)
                m_stops.Add(new Stop(flPosition, flValue, rsSpace));
        }

        public void Clear()
        {
            lock (SyncRoot)
                m_stops.Clear();
        }

        internal IReadOnlyList<Stop> Stops { get { lock (SyncRoot) return m_stops.ToArray(); } }

        /// <summary>
        /// Resolves recorded stops into absolute local-pixel coordinates along this
        /// gradient's <see cref="Orientation"/> axis, given the owning visual's extent
        /// (<paramref name="extent"/> = Size.X for Horizontal, Size.Y for Vertical).
        /// <see cref="RelativeSpace.Max"/> stops add their (usually negative-or-zero)
        /// <c>Position</c> to <paramref name="extent"/> -- verified against
        /// EdgeFade.UpdateFades: with MinOffset/MaxOffset both 0 (the common case), its
        /// max-side stops are <c>AddValue(-FadeSize, 1f, Max)</c> /
        /// <c>AddValue(0, 1-FadeAmount, Max)</c>, which only reproduces "full alpha
        /// FadeSize pixels in from the far edge, fading to reduced alpha right at the far
        /// edge" (the mirror image of the min-side stops, which use
        /// <see cref="RelativeSpace.Min"/> positions unchanged) under
        /// <c>extent + Position</c>; <c>extent - Position</c> was tried first and puts
        /// both stops at/beyond the far edge, collapsing nearly the whole container to
        /// the reduced value -- exactly the "widespread invisible content" regression
        /// documented in logs/UIX.RenderApi.OpenGL/Implementation.md's earlier gradient
        /// attempts, reproduced and root-caused here rather than just reverted again.
        /// Sorted ascending so the shader's piecewise-linear lookup can walk stops in
        /// order.
        /// </summary>
        internal (float[] Positions, float[] Values) ResolveStops(float extent)
        {
            (float[] Positions, float[] Values) result;
            lock (SyncRoot)
            {
                float offset = m_offset;
                int n = m_stops.Count;
                var resolved = new (float Position, float Value)[n];
                for (int i = 0; i < n; i++)
                {
                    Stop s = m_stops[i];
                    float pos = s.Space == RelativeSpace.Max ? extent + s.Position : s.Position;
                    resolved[i] = (pos + offset, s.Value);
                }
                Array.Sort(resolved, static (a, b) => a.Position.CompareTo(b.Position));

                var positions = new float[n];
                var values = new float[n];
                for (int i = 0; i < n; i++)
                {
                    positions[i] = resolved[i].Position;
                    values[i] = resolved[i].Value;
                }
                result = (positions, values);
            }
            return result;
        }
    }
}
