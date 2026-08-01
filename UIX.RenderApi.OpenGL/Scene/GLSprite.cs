using Microsoft.Iris.Render.OpenGL.Engine;
using Microsoft.Iris.Render.OpenGL.Rendering;
using Silk.NET.Maths;

namespace Microsoft.Iris.Render.OpenGL.Scene
{
    /// <summary>
    /// A leaf visual that paints a textured/colored quad. Content comes from its
    /// <see cref="IEffect"/> (typically an image effect); absent an image it fills
    /// with the sprite's debug color, which keeps placeholder content visible.
    /// </summary>
    internal sealed class GLSprite : GLVisual, ISprite
    {
        private Inset? m_nineSlice;
        private IEffect? m_effect;
        private bool m_relativeSize;

        public GLSprite(GLRenderSession session, object ownerData)
            : base(session, ownerData)
        {
        }

        public IEffect? Effect
        {
            get { lock (Session.SyncRoot) return m_effect; }
            set { lock (Session.SyncRoot) m_effect = value; }
        }

        public bool RelativeSize
        {
            get { lock (Session.SyncRoot) return m_relativeSize; }
            set { lock (Session.SyncRoot) m_relativeSize = value; }
        }

        public void SetCoordMap(int idxLayer, CoordMap coordMap)
        {
            // TODO(stage 3): honor per-layer coordinate remaps. Not required for the
            // basic textured-quad path; stored intent is dropped for now.
        }

        public void SetNineGrid(int left, int top, int right, int bottom)
        {
            lock (Session.SyncRoot)
                m_nineSlice = new Inset(left, top, right, bottom);
        }

        // ISprite.RelativeSize (set widely -- ViewItem's background sprite, Graphic's
        // image content, TextRunRenderer's highlight sprite) means Size is a fraction of
        // ParentContainer.Size rather than absolute device pixels; RelativeSize=true with
        // Size=(1,1) (Vector2.UnitVector) means "100% of my parent container" -- the
        // usual "stretch to fill" case. Resolve it here rather than in Size's getter/
        // setter: the parent (and its Size) can change after this sprite's Size is set,
        // and ParentContainer isn't known until AddChild runs. Locked as one snapshot
        // (not RelativeSize/ParentContainer/Size read separately) so a concurrent
        // AddChild/property write can't be observed half-applied.
        private Vector2 EffectiveSize
        {
            get
            {
                lock (Session.SyncRoot)
                {
                    return m_relativeSize && ParentContainer != null
                        ? new Vector2(Size.X * ParentContainer.Size.X, Size.Y * ParentContainer.Size.Y)
                        : Size;
                }
            }
        }

        internal override void Render(SceneRenderer renderer, Matrix4X4<float> parentMatrix, float inheritedAlpha)
        {
            // One lock for the whole frame's worth of this sprite's state, so
            // Effect/size/transform/nine-slice are all read as one consistent
            // snapshot rather than tearing against a concurrent app-thread write.
            lock (Session.SyncRoot)
            {
                if (!Visible || Size.X <= 0f || Size.Y <= 0f)
                    return;

                Vector2 size = EffectiveSize;
                if (size.X <= 0f || size.Y <= 0f)
                    return;

                Matrix4X4<float> matrix = LocalMatrix * parentMatrix;
                float alpha = inheritedAlpha * Alpha;

                GLEffect? effect = Effect as GLEffect;
                GLImage? image = effect?.PrimaryImage;
                if (image != null)
                {
                    renderer.DrawTexturedQuad(matrix, size.X, size.Y, image, alpha, m_nineSlice);
                }
                else if (effect?.PrimaryColor is ColorF fill)
                {
                    renderer.DrawColoredQuad(matrix, size.X, size.Y, fill, alpha);
                }
                else
                {
                    ColorF c = DebugColor;
                    // A zeroed ColorF would be fully transparent black; only draw when the
                    // caller actually assigned a debug color.
                    if (c.A > 0f)
                        renderer.DrawColoredQuad(matrix, size.X, size.Y, c, alpha);
                }
            }
        }

        internal override GLVisual? HitTest(Vector2 screenPoint, Matrix4X4<float> parentMatrix)
        {
            // See Render: one lock for a consistent Visible/MouseOptions/transform/
            // size snapshot against a concurrent app-thread write.
            lock (Session.SyncRoot)
            {
                if (!Visible || (MouseOptions & MouseOptions.Hittable) == 0)
                    return null;
                Matrix4X4<float> world = LocalMatrix * parentMatrix;
                return ContainsPoint(screenPoint, world, EffectiveSize) ? this : null;
            }
        }
    }
}
