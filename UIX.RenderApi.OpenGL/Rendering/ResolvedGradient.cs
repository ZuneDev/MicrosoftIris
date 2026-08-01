using Microsoft.Iris.Render.OpenGL.Scene;
using Silk.NET.Maths;

namespace Microsoft.Iris.Render.OpenGL.Rendering
{
    /// <summary>
    /// One <see cref="GLGradient"/> attached somewhere in the visual tree, resolved into
    /// GPU-ready form for a specific draw call: stops already converted to absolute
    /// local-pixel coordinates (<see cref="GLGradient.ResolveStops"/>) in the *owning*
    /// visual's local space, plus the transform from the drawing visual's own local pixel
    /// space into that owning visual's local space. A sprite's draw call carries one of
    /// these per gradient attached anywhere from itself up to the root (its own
    /// <see cref="GLVisual.AddGradient"/> calls, plus every ancestor container's, per
    /// <see cref="IVisualContainer.AddGradient"/>'s "applies to the whole subtree"
    /// semantics -- see EdgeFade/Text's original usage).
    ///
    /// <see cref="Transform"/> composes as visuals are walked: a container's own
    /// gradients start with <see cref="Matrix4X4{T}.Identity"/> (evaluated in its own
    /// local space); when passed to a child, each inherited entry's transform is
    /// premultiplied by the child's own <see cref="GLVisual.LocalMatrix"/>
    /// (child-local -> parent-local -> ... -> owner-local), matching the row-vector
    /// convention used everywhere else in this backend (<c>LocalMatrix * parentMatrix</c>).
    /// </summary>
    internal readonly struct ResolvedGradient
    {
        public readonly Matrix4X4<float> Transform;
        public readonly Orientation Orientation;
        public readonly float[] StopPositions;
        public readonly float[] StopValues;

        public ResolvedGradient(Matrix4X4<float> transform, Orientation orientation, float[] stopPositions, float[] stopValues)
        {
            Transform = transform;
            Orientation = orientation;
            StopPositions = stopPositions;
            StopValues = stopValues;
        }

        public ResolvedGradient WithTransform(Matrix4X4<float> transform)
            => new ResolvedGradient(transform, Orientation, StopPositions, StopValues);
    }
}
