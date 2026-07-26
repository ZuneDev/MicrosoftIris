using System.Collections.Generic;
using Silk.NET.Maths;

namespace Microsoft.Iris.Render.OpenGL
{
    /// <summary>
    /// Base class shared by <see cref="GLVisualContainer"/> and <see cref="GLSprite"/>.
    /// Holds the common 2.5D transform state (position/size/scale/rotation/alpha) and
    /// produces the local model matrix used when walking the tree during rendering.
    /// </summary>
    public abstract class GLVisual : SharedRenderObject, IVisual
    {
        private readonly object m_ownerData;
        protected readonly GLRenderSession Session;

        private Vector3 m_position = Vector3.Zero;
        private Vector2 m_size = Vector2.Zero;
        private Vector3 m_scale = Vector3.UnitVector;
        private AxisAngle m_rotation = AxisAngle.Identity;
        private Vector3 m_centerPoint = Vector3.Zero;
        private float m_alpha = 1f;
        private bool m_visible = true;
        private uint m_layer;

        private readonly List<GLGradient> m_gradients = new List<GLGradient>();

        protected GLVisual(GLRenderSession session, object ownerData)
        {
            Session = session;
            m_ownerData = ownerData;
        }

        // IRawInputSite
        public object OwnerData => m_ownerData;

        // IVisual
        public MouseOptions MouseOptions { get; set; } = MouseOptions.None;
        public GLVisualContainer? ParentContainer { get; internal set; }
        public IVisualContainer Parent => ParentContainer!;
        public string DebugID { get; set; } = string.Empty;
        public ColorF DebugColor { get; set; }

        public void Remove() => ParentContainer?.RemoveChild(this);

        public virtual void CopyFrom(IVisual visualSource)
        {
            if (visualSource is not GLVisual src)
                return;
            m_position = src.m_position;
            m_size = src.m_size;
            m_scale = src.m_scale;
            m_rotation = src.m_rotation;
            m_centerPoint = src.m_centerPoint;
            m_alpha = src.m_alpha;
            m_visible = src.m_visible;
            m_layer = src.m_layer;
            MouseOptions = src.MouseOptions;
        }

        // Shared transform surface (declared on both IVisualContainer and ISprite).
        public Vector3 Position { get => m_position; set => m_position = value; }
        public Vector2 Size { get => m_size; set => m_size = value; }
        public Vector3 Scale { get => m_scale; set => m_scale = value; }
        public AxisAngle Rotation { get => m_rotation; set => m_rotation = value; }
        public Vector3 CenterPoint { get => m_centerPoint; set => m_centerPoint = value; }
        public float Alpha { get => m_alpha; set => m_alpha = value; }
        public bool Visible { get => m_visible; set => m_visible = value; }
        public uint Layer { get => m_layer; set => m_layer = value; }

        // The "force" overloads exist so callers can bypass change coalescing; our
        // implementation applies changes immediately, so force is a no-op distinction.
        public void SetPosition(Vector3 value, bool force) => m_position = value;
        public void SetSize(Vector2 value, bool force) => m_size = value;
        public void SetScale(Vector3 value, bool force) => m_scale = value;
        public void SetRotation(AxisAngle value, bool force) => m_rotation = value;
        public void SetAlpha(float value, bool force) => m_alpha = value;

        public void AddGradient(IGradient gradient)
        {
            if (gradient is GLGradient g)
            {
                g.RegisterUsage(this);
                m_gradients.Add(g);
            }
        }

        public void RemoveAllGradients()
        {
            foreach (GLGradient g in m_gradients)
                g.UnregisterUsage(this);
            m_gradients.Clear();
        }

        internal IReadOnlyList<GLGradient> Gradients => m_gradients;

        /// <summary>
        /// Local model transform: translate to position, rotate/scale about the center
        /// point. Matches the Iris convention where position/size are in device pixels
        /// with the y axis pointing down.
        /// </summary>
        internal Matrix4X4<float> LocalMatrix
        {
            get
            {
                Vector3 c = m_centerPoint;
                Matrix4X4<float> toCenter = Matrix4X4.CreateTranslation(-c.X, -c.Y, -c.Z);
                Matrix4X4<float> scale = Matrix4X4.CreateScale(m_scale.X, m_scale.Y, m_scale.Z);
                Matrix4X4<float> rot = Matrix4X4.CreateFromAxisAngle(
                    new Vector3D<float>(m_rotation.Axis.X, m_rotation.Axis.Y, m_rotation.Axis.Z),
                    m_rotation.Angle);
                Matrix4X4<float> fromCenter = Matrix4X4.CreateTranslation(c.X, c.Y, c.Z);
                Matrix4X4<float> translate = Matrix4X4.CreateTranslation(m_position.X, m_position.Y, m_position.Z);
                return toCenter * scale * rot * fromCenter * translate;
            }
        }

        /// <summary>Draw this visual (and its subtree) with the accumulated parent transform.</summary>
        internal abstract void Render(SceneRenderer renderer, Matrix4X4<float> parentMatrix, float inheritedAlpha);

        /// <summary>
        /// Return the frontmost hittable visual under <paramref name="screenPoint"/>
        /// (client pixel space), or null. <paramref name="parentMatrix"/> is the
        /// accumulated transform of this visual's parent.
        /// </summary>
        internal abstract GLVisual? HitTest(Vector2 screenPoint, Matrix4X4<float> parentMatrix);

        /// <summary>Is the client-space point inside this visual's local quad?</summary>
        private protected bool ContainsPoint(Vector2 screenPoint, Matrix4X4<float> worldMatrix)
        {
            if (Size.X <= 0f || Size.Y <= 0f)
                return false;
            if (!Matrix4X4.Invert(worldMatrix, out Matrix4X4<float> inverse))
                return false;
            // Our world matrix maps local -> screen as (local * world) under the
            // renderer's convention, so the inverse maps screen -> local the same way.
            Vector3D<float> local = Vector3D.Transform(new Vector3D<float>(screenPoint.X, screenPoint.Y, 0f), inverse);
            return local.X >= 0f && local.X <= Size.X && local.Y >= 0f && local.Y <= Size.Y;
        }
    }
}
