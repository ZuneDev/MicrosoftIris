using System.Collections.Generic;
using Microsoft.Iris.Render.Animation;
using Microsoft.Iris.Render.OpenGL.Engine;
using Microsoft.Iris.Render.OpenGL.Rendering;
using Microsoft.Iris.Render.Protocol;
using Silk.NET.Maths;

namespace Microsoft.Iris.Render.OpenGL.Scene
{
    /// <summary>
    /// Base class shared by <see cref="GLVisualContainer"/> and <see cref="GLSprite"/>.
    /// Holds the common 2.5D transform state (position/size/scale/rotation/alpha) and
    /// produces the local model matrix used when walking the tree during rendering.
    /// </summary>
    internal abstract class GLVisual : SharedRenderObject, IVisual, IAnimatableObject
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

        protected GLVisual(GLRenderSession session, object ownerData) : base(session.SyncRoot)
        {
            Session = session;
            m_ownerData = ownerData;
        }

        // IRawInputSite
        public object OwnerData => m_ownerData;

        // IVisual
        // MouseOptions/DebugID are only ever touched from the main thread (set from
        // markup/session code, read by hit-testing -- also main-thread), so they're
        // left as plain auto-properties. ParentContainer and DebugColor are read
        // during Render (render thread) as well as written from the main thread
        // (AddChild/RemoveChild, debug tooling), so they're locked like the
        // transform properties above.
        public MouseOptions MouseOptions { get; set; } = MouseOptions.None;

        private GLVisualContainer? m_parentContainer;
        public GLVisualContainer? ParentContainer
        {
            get { lock (Session.SyncRoot) return m_parentContainer; }
            internal set { lock (Session.SyncRoot) m_parentContainer = value; }
        }

        public IVisualContainer Parent => ParentContainer!;
        public string DebugID { get; set; } = string.Empty;

        private ColorF m_debugColor;
        public ColorF DebugColor
        {
            get { lock (Session.SyncRoot) return m_debugColor; }
            set { lock (Session.SyncRoot) m_debugColor = value; }
        }

        public void Remove() => ParentContainer?.RemoveChild(this);

        public virtual void CopyFrom(IVisual visualSource)
        {
            if (visualSource is not GLVisual src)
                return;
            // src and this share the same render session, hence the same SyncRoot --
            // one lock covers both sides of the copy atomically.
            lock (Session.SyncRoot)
            {
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
        }

        // Shared transform surface (declared on both IVisualContainer and ISprite).
        // Locked on both sides (not just writes): the render thread's animation
        // pulse (GLKeyframeAnimation.Advance -> AnimationTargetApplier) writes these
        // same fields every frame, and hit-testing on the main thread
        // (GLInputTranslator -> GLVisual.HitTest) reads them outside the render
        // thread's per-frame lock, so an unguarded read here could tear against a
        // concurrent write.
        public Vector3 Position { get { lock (Session.SyncRoot) return m_position; } set { lock (Session.SyncRoot) m_position = value; } }
        public Vector2 Size { get { lock (Session.SyncRoot) return m_size; } set { lock (Session.SyncRoot) m_size = value; } }
        public Vector3 Scale { get { lock (Session.SyncRoot) return m_scale; } set { lock (Session.SyncRoot) m_scale = value; } }
        public AxisAngle Rotation { get { lock (Session.SyncRoot) return m_rotation; } set { lock (Session.SyncRoot) m_rotation = value; } }
        public Vector3 CenterPoint { get { lock (Session.SyncRoot) return m_centerPoint; } set { lock (Session.SyncRoot) m_centerPoint = value; } }
        public float Alpha { get { lock (Session.SyncRoot) return m_alpha; } set { lock (Session.SyncRoot) m_alpha = value; } }
        public bool Visible { get { lock (Session.SyncRoot) return m_visible; } set { lock (Session.SyncRoot) m_visible = value; } }
        public uint Layer { get { lock (Session.SyncRoot) return m_layer; } set { lock (Session.SyncRoot) m_layer = value; } }

        // The "force" overloads exist so callers can bypass change coalescing; our
        // implementation applies changes immediately, so force is a no-op distinction.
        public void SetPosition(Vector3 value, bool force) { lock (Session.SyncRoot) m_position = value; }
        public void SetSize(Vector2 value, bool force) { lock (Session.SyncRoot) m_size = value; }
        public void SetScale(Vector3 value, bool force) { lock (Session.SyncRoot) m_scale = value; }
        public void SetRotation(AxisAngle value, bool force) { lock (Session.SyncRoot) m_rotation = value; }
        public void SetAlpha(float value, bool force) { lock (Session.SyncRoot) m_alpha = value; }

        public void AddGradient(IGradient gradient)
        {
            if (gradient is not GLGradient g)
                return;
            lock (Session.SyncRoot)
            {
                g.RegisterUsage(this);
                m_gradients.Add(g);
            }
        }

        public void RemoveAllGradients()
        {
            lock (Session.SyncRoot)
            {
                foreach (var g in m_gradients)
                    g.UnregisterUsage(this);
                m_gradients.Clear();
            }
        }

        internal IReadOnlyList<GLGradient> Gradients { get { lock (Session.SyncRoot) return m_gradients.ToArray(); } }

        /// <summary>
        /// Resolves this visual's own directly-attached gradients (not ancestors') against
        /// <paramref name="size"/> -- the visual's own local extent, i.e. what
        /// <see cref="GLGradient.Orientation"/> measures stops along -- with
        /// <see cref="ResolvedGradient.Transform"/> left as identity, since a gradient's
        /// own attachment point is always evaluated in its own local space. Locks
        /// <see cref="GLRenderSession.SyncRoot"/> itself (reentrant-safe if the caller
        /// already holds it, as <see cref="GLSprite.Render"/> does) rather than trusting
        /// every call site to -- <see cref="GLVisualContainer.Render"/> in particular
        /// doesn't otherwise take this lock at all.
        /// </summary>
        private protected List<ResolvedGradient> ResolveOwnGradients(Vector2 size)
        {
            GLGradient[] snapshot;
            lock (Session.SyncRoot)
                snapshot = m_gradients.ToArray();

            var result = new List<ResolvedGradient>(snapshot.Length);
            foreach (GLGradient g in snapshot)
            {
                float extent = g.Orientation == Orientation.Horizontal ? size.X : size.Y;
                (float[] positions, float[] values) = g.ResolveStops(extent);
                result.Add(new ResolvedGradient(Matrix4X4<float>.Identity, g.Orientation, positions, values));
            }
            return result;
        }

        /// <summary>
        /// Local model transform: translate to position, rotate/scale about the center
        /// point. Matches the Iris convention where position/size are in device pixels
        /// with the y axis pointing down. Reads all four transform fields as one
        /// consistent snapshot under a single lock, rather than field-by-field, so the
        /// render thread never composes a matrix from a torn mix of old/new values.
        /// </summary>
        internal Matrix4X4<float> LocalMatrix
        {
            get
            {
                Vector3 c, position, scale;
                AxisAngle rotation;
                lock (Session.SyncRoot)
                {
                    c = m_centerPoint;
                    position = m_position;
                    scale = m_scale;
                    rotation = m_rotation;
                }
                Matrix4X4<float> toCenter = Matrix4X4.CreateTranslation(-c.X, -c.Y, -c.Z);
                Matrix4X4<float> scaleMatrix = Matrix4X4.CreateScale(scale.X, scale.Y, scale.Z);
                Matrix4X4<float> rot = Matrix4X4.CreateFromAxisAngle(
                    new Vector3D<float>(rotation.Axis.X, rotation.Axis.Y, rotation.Axis.Z),
                    rotation.Angle);
                Matrix4X4<float> fromCenter = Matrix4X4.CreateTranslation(c.X, c.Y, c.Z);
                Matrix4X4<float> translate = Matrix4X4.CreateTranslation(position.X, position.Y, position.Z);
                return toCenter * scaleMatrix * rot * fromCenter * translate;
            }
        }

        /// <summary>
        /// Draw this visual (and its subtree) with the accumulated parent transform.
        /// <paramref name="ambientGradients"/> are gradients attached to an ancestor
        /// container (or further up), already expressed as "this visual's own local pixel
        /// space -> the owning ancestor's local pixel space" -- see
        /// <see cref="ResolvedGradient"/>.
        /// </summary>
        internal abstract void Render(SceneRenderer renderer, Matrix4X4<float> parentMatrix, float inheritedAlpha, IReadOnlyList<ResolvedGradient> ambientGradients);

        /// <summary>
        /// Return the frontmost hittable visual under <paramref name="screenPoint"/>
        /// (client pixel space), or null. <paramref name="parentMatrix"/> is the
        /// accumulated transform of this visual's parent.
        /// </summary>
        internal abstract GLVisual? HitTest(Vector2 screenPoint, Matrix4X4<float> parentMatrix);

        /// <summary>
        /// Is the client-space point inside a local quad of the given <paramref name="size"/>?
        /// Takes size explicitly (rather than reading <see cref="Size"/> directly) because
        /// <see cref="GLSprite"/>'s effective size depends on its own ISprite.RelativeSize
        /// resolution against its parent container, not just the raw Size value.
        /// </summary>
        private protected bool ContainsPoint(Vector2 screenPoint, Matrix4X4<float> worldMatrix, Vector2 size)
        {
            if (size.X <= 0f || size.Y <= 0f)
                return false;
            if (!Matrix4X4.Invert(worldMatrix, out Matrix4X4<float> inverse))
                return false;
            // Our world matrix maps local -> screen as (local * world) under the
            // renderer's convention, so the inverse maps screen -> local the same way.
            Vector3D<float> local = Vector3D.Transform(new Vector3D<float>(screenPoint.X, screenPoint.Y, 0f), inverse);
            return local.X >= 0f && local.X <= size.X && local.Y >= 0f && local.Y <= size.Y;
        }

        public RENDERHANDLE GetObjectId() => default;

        public uint GetPropertyId(string propertyName)
        {
            throw new System.NotImplementedException();
        }

        public AnimationInputType GetPropertyType(string propertyName)
        {
            return propertyName switch
            {
                nameof(Position) or
                nameof(Size) => AnimationInputType.Vector2,
                nameof(Scale) => AnimationInputType.Vector3,
                nameof(Rotation) => AnimationInputType.Vector4,
                nameof(CenterPoint) => AnimationInputType.Vector3,
                nameof(Alpha) => AnimationInputType.Float,
                
                _ => throw new System.NotImplementedException(),
            };
        }
    }
}
