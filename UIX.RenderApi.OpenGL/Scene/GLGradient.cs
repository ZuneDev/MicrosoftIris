using System.Collections.Generic;

namespace Microsoft.Iris.Render.OpenGL.Scene
{
    /// <summary>
    /// An alpha/color gradient that can be attached to a visual. We record the stops and
    /// mask; applying them as an alpha ramp during compositing is left as a stage-3 TODO.
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

        public Orientation Orientation { get; set; }
        public ColorF ColorMask { get; set; } = new ColorF(1f, 1f, 1f, 1f);
        public float Offset { get; set; }

        public void AddValue(float flPosition, float flValue, RelativeSpace rsSpace)
            => m_stops.Add(new Stop(flPosition, flValue, rsSpace));

        public void Clear() => m_stops.Clear();

        internal IReadOnlyList<Stop> Stops => m_stops;
    }
}
