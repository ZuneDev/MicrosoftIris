using System.Collections.Generic;
using System.Linq;
using Microsoft.Iris.Render.OpenGL.Engine;
using Microsoft.Iris.Render.OpenGL.Rendering;
using Silk.NET.Maths;

namespace Microsoft.Iris.Render.OpenGL.Scene
{
    /// <summary>
    /// A transform/grouping node in the visual tree. Renders its children in
    /// ascending <see cref="GLVisual.Layer"/> order after applying its own transform.
    /// </summary>
    public sealed class GLVisualContainer : GLVisual, IVisualContainer
    {
        private readonly List<GLVisual> m_children = new List<GLVisual>();

        public GLVisualContainer(GLRenderSession session, object ownerData, bool isRoot)
            : base(session, ownerData)
        {
            IsRoot = isRoot;
        }

        public bool IsRoot { get; }
        public int ChildCount => m_children.Count;
        public ICamera? Camera { get; set; }

        public void AddChild(IVisual vChild, IVisual vSibling, VisualOrder nOrder)
        {
            if (vChild is not GLVisual child)
                return;

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

        public void RemoveChild(IVisual vChild)
        {
            if (vChild is not GLVisual child || !m_children.Remove(child))
                return;
            child.ParentContainer = null;
            child.UnregisterUsage(this);
        }

        public void RemoveAllChildren()
        {
            // Snapshot: UnregisterUsage can trigger disposal which mutates state.
            foreach (GLVisual child in m_children.ToArray())
                RemoveChild(child);
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
            return m_children.Select((v, i) => (v, i))
                .OrderBy(t => t.v.Layer)
                .ThenByDescending(t => t.i)
                .Select(t => t.v)
                .ToList();
        }

        internal override void Render(SceneRenderer renderer, Matrix4X4<float> parentMatrix, float inheritedAlpha)
        {
            if (!Visible)
                return;

            Matrix4X4<float> matrix = LocalMatrix * parentMatrix;
            float alpha = inheritedAlpha * Alpha;

            foreach (var child in BackToFrontOrder())
                child.Render(renderer, matrix, alpha);
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
