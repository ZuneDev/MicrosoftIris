using System.Collections.Generic;
using Microsoft.Iris.Render.OpenGL.Engine;
using Microsoft.Iris.Render.OpenGL.Rendering;
using Silk.NET.Maths;

namespace Microsoft.Iris.Render.OpenGL.Scene
{
    /// <summary>
    /// A transform/grouping node in the visual tree. Renders its children in
    /// ascending <see cref="GLVisual.Layer"/> order after applying its own transform.
    /// </summary>
    internal sealed class GLVisualContainer : GLVisual, IVisualContainer
    {
        private readonly List<GLVisual> m_children = new List<GLVisual>();

        public GLVisualContainer(GLRenderSession session, object ownerData, bool isRoot)
            : base(session, ownerData)
        {
            IsRoot = isRoot;
        }

        public bool IsRoot { get; }
        public int ChildCount { get { lock (Session.SyncRoot) return m_children.Count; } }
        public ICamera? Camera { get; set; }

        public void AddChild(IVisual vChild, IVisual vSibling, VisualOrder nOrder)
        {
            if (vChild is not GLVisual child)
                return;

            lock (Session.SyncRoot)
            {
                child.ParentContainer?.RemoveChild(child);
                child.ParentContainer = this;

                int siblingIndex = vSibling is GLVisual s ? m_children.IndexOf(s) : -1;
                switch (nOrder)
                {
                    case VisualOrder.First:
                        m_children.Insert(0, child);
                        break;
                    case VisualOrder.Before when siblingIndex >= 0:
                        m_children.Insert(siblingIndex, child);
                        break;
                    case VisualOrder.After when siblingIndex >= 0:
                        m_children.Insert(siblingIndex + 1, child);
                        break;
                    default: // Any, Last, or unresolved sibling
                        m_children.Add(child);
                        break;
                }

                child.RegisterUsage(this);
            }
        }

        public void RemoveChild(IVisual vChild)
        {
            if (vChild is not GLVisual child)
                return;
            lock (Session.SyncRoot)
            {
                if (!m_children.Remove(child))
                    return;
                child.ParentContainer = null;
                child.UnregisterUsage(this);
            }
        }

        public void RemoveAllChildren()
        {
            lock (Session.SyncRoot)
            {
                // Snapshot: UnregisterUsage can trigger disposal which mutates state.
                foreach (GLVisual child in m_children.ToArray())
                    RemoveChild(child);
            }
        }

        /// <summary>
        /// Sorts children back-to-front for painting: ascending by <see cref="GLVisual.Layer"/>
        /// (higher layer paints later, i.e. on top -- the ordinary convention, kept as-is), with
        /// ties broken by *reverse* insertion/declaration order. The first child added to a
        /// container (e.g. markup's first-declared &lt;Children&gt; entry -- confirmed against
        /// PageStack.uix, whose "ForegroundUI"/"BackgroundUI" pair is declared in exactly that
        /// order, "Foreground" first) paints last, i.e. on top -- the opposite of the more common
        /// "later sibling wins" convention, but matches what PageStack.uix's declared naming and
        /// intent requires (Foreground must render over Background despite coming first). Uses a
        /// stable sort (List&lt;T&gt;.Sort is not stable) so same-layer ties resolve
        /// deterministically by original order, not sort-algorithm shuffling.
        /// </summary>
        private List<GLVisual> BackToFrontOrder()
        {
            // Snapshot *and* capture each child's Layer in the same lock, then sort
            // outside it using the captured values -- not m_children.Select(...).
            // OrderBy(t => t.v.Layer), which reads the (individually locked) Layer
            // property from *inside* the sort comparer, i.e. after this method's own
            // lock has already been released. That meant every single comparison
            // during the sort was its own separate lock acquisition -- O(n log n) of
            // them, on every hit-test (every mouse move) and every render frame, for
            // every container. GLRenderWindow.HitTest already wraps the whole
            // recursive hit-test walk in one lock for exactly this reason; this was
            // the same mistake hiding one level deeper, inside the sort itself.
            (GLVisual Visual, uint Layer, int Index)[] indexed;
            lock (Session.SyncRoot)
            {
                indexed = new (GLVisual, uint, int)[m_children.Count];
                for (int i = 0; i < m_children.Count; i++)
                    indexed[i] = (m_children[i], m_children[i].Layer, i);
            }

            // Ascending by Layer (higher layer paints later/on top), ties broken by
            // *reverse* index -- see this method's summary above for why. Array.Sort
            // isn't stable, but the explicit descending-Index tiebreak makes that
            // irrelevant (every pair compares unequal on Index alone).
            System.Array.Sort(indexed, static (a, b) =>
            {
                int cmp = a.Layer.CompareTo(b.Layer);
                return cmp != 0 ? cmp : b.Index.CompareTo(a.Index);
            });

            var result = new List<GLVisual>(indexed.Length);
            foreach (var entry in indexed)
                result.Add(entry.Visual);
            return result;
        }

        internal override void Render(SceneRenderer renderer, Matrix4X4<float> parentMatrix, float inheritedAlpha, IReadOnlyList<ResolvedGradient> ambientGradients)
        {
            if (!Visible)
                return;

            Matrix4X4<float> matrix = LocalMatrix * parentMatrix;
            float alpha = inheritedAlpha * Alpha;

            // Gradients attached to *this* container (the EdgeFade pattern: e.g. a
            // Scroller's viewport container carries an edge-fade gradient that applies to
            // everything scrolling through it, at any depth) apply in this container's
            // own local space -- identity transform, same as a sprite's own gradients.
            // Combined with whatever ambient gradients this container itself inherited
            // from further up, this is the full set descendants need to know about.
            List<ResolvedGradient> ownGradients = ResolveOwnGradients(Size);
            List<ResolvedGradient> combined;
            if (ambientGradients.Count == 0)
            {
                combined = ownGradients;
            }
            else
            {
                combined = new List<ResolvedGradient>(ambientGradients.Count + ownGradients.Count);
                combined.AddRange(ambientGradients);
                combined.AddRange(ownGradients);
            }

            foreach (var child in BackToFrontOrder())
            {
                // Re-express each entry in terms of the child's own local space: a point
                // in the child's local space maps into this container's local space via
                // child.LocalMatrix, then (for entries inherited from further up) on into
                // whatever ancestor originally owns the gradient via entry.Transform --
                // row-vector composition, same convention as `LocalMatrix * parentMatrix`.
                List<ResolvedGradient> childGradients = combined;
                if (combined.Count > 0)
                {
                    Matrix4X4<float> childLocalMatrix = child.LocalMatrix;
                    childGradients = new List<ResolvedGradient>(combined.Count);
                    foreach (var g in combined)
                        childGradients.Add(g.WithTransform(childLocalMatrix * g.Transform));
                }
                child.Render(renderer, matrix, alpha, childGradients);
            }
        }

        internal override GLVisual? HitTest(Vector2 screenPoint, Matrix4X4<float> parentMatrix)
        {
            if (!Visible)
                return null;

            Matrix4X4<float> world = LocalMatrix * parentMatrix;

            // Frontmost (last painted) first: same order BackToFrontOrder paints in,
            // reversed, since that method already lists back-to-front (painted first-to-last).
            var frontToBack = BackToFrontOrder();
            frontToBack.Reverse();
            foreach (var child in frontToBack)
            {
                var hit = child.HitTest(screenPoint, world);
                if (hit != null)
                    return hit;
            }

            // Otherwise the container itself, if it is hittable and has extent.
            if ((MouseOptions & MouseOptions.Hittable) != 0 && ContainsPoint(screenPoint, world, Size))
                return this;

            return null;
        }

        protected override void DisposeCore() => RemoveAllChildren();
    }
}
