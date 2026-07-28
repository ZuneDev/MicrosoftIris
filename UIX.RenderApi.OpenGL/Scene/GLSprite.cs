using Silk.NET.Maths;

namespace Microsoft.Iris.Render.OpenGL
{
    /// <summary>
    /// A leaf visual that paints a textured/colored quad. Content comes from its
    /// <see cref="IEffect"/> (typically an image effect); absent an image it fills
    /// with the sprite's debug color, which keeps placeholder content visible.
    /// </summary>
    public sealed class GLSprite : GLVisual, ISprite
    {
        private int m_nineGridLeft, m_nineGridTop, m_nineGridRight, m_nineGridBottom;

        public GLSprite(GLRenderSession session, object ownerData)
            : base(session, ownerData)
        {
        }

        public IEffect? Effect { get; set; }
        public bool RelativeSize { get; set; }

        public void SetCoordMap(int idxLayer, CoordMap coordMap)
        {
            // TODO(stage 3): honor per-layer coordinate remaps. Not required for the
            // basic textured-quad path; stored intent is dropped for now.
        }

        public void SetNineGrid(int left, int top, int right, int bottom)
        {
            m_nineGridLeft = left;
            m_nineGridTop = top;
            m_nineGridRight = right;
            m_nineGridBottom = bottom;
            // TODO(stage 3): implement 9-slice stretching. Currently the sprite is
            // drawn as a single stretched quad regardless of these insets.
        }

        // ISprite.RelativeSize (set widely -- ViewItem's background sprite, Graphic's
        // image content, TextRunRenderer's highlight sprite) means Size is a fraction of
        // ParentContainer.Size rather than absolute device pixels; RelativeSize=true with
        // Size=(1,1) (Vector2.UnitVector) means "100% of my parent container" -- the
        // usual "stretch to fill" case. Resolve it here rather than in Size's getter/
        // setter: the parent (and its Size) can change after this sprite's Size is set,
        // and ParentContainer isn't known until AddChild runs.
        private Vector2 EffectiveSize => RelativeSize && ParentContainer != null
            ? new Vector2(Size.X * ParentContainer.Size.X, Size.Y * ParentContainer.Size.Y)
            : Size;

        internal override void Render(SceneRenderer renderer, Matrix4X4<float> parentMatrix, float inheritedAlpha)
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
                renderer.DrawTexturedQuad(matrix, size.X, size.Y, image, alpha);
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

        internal override GLVisual? HitTest(Vector2 screenPoint, Matrix4X4<float> parentMatrix)
        {
            if (!Visible || (MouseOptions & MouseOptions.Hittable) == 0)
                return null;
            Matrix4X4<float> world = LocalMatrix * parentMatrix;
            return ContainsPoint(screenPoint, world, EffectiveSize) ? this : null;
        }
    }
}
