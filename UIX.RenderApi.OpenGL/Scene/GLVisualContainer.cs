using System.Collections.Generic;
using Silk.NET.Maths;

namespace Microsoft.Iris.Render.OpenGL
{
    /// <summary>
    /// A transform/grouping node in the visual tree. Renders its children in
    /// ascending <see cref="GLVisual.Layer"/> order after applying its own transform.
    /// </summary>
    public sealed class GLVisualContainer : GLVisual, IVisualContainer
    {
        private readonly List<GLVisual> m_children = new List<GLVisual>();
        private readonly bool m_isRoot;

        public GLVisualContainer(GLRenderSession session, object ownerData, bool isRoot)
            : base(session, ownerData)
        {
            m_isRoot = isRoot;
        }

        public bool IsRoot => m_isRoot;
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
            if (vChild is GLVisual child && m_children.Remove(child))
            {
                child.ParentContainer = null;
                child.UnregisterUsage(this);
            }
        }

        public void RemoveAllChildren()
        {
            // Snapshot: UnregisterUsage can trigger disposal which mutates state.
            foreach (GLVisual child in m_children.ToArray())
                RemoveChild(child);
        }

        internal override void Render(SceneRenderer renderer, Matrix4X4<float> parentMatrix, float inheritedAlpha)
        {
            if (!Visible)
                return;

            Matrix4X4<float> matrix = LocalMatrix * parentMatrix;
            float alpha = inheritedAlpha * Alpha;

            // Draw children back-to-front by layer. OrderBy is stable, preserving
            // insertion order within a layer.
            m_children.Sort((a, b) => a.Layer.CompareTo(b.Layer));
            foreach (GLVisual child in m_children)
                child.Render(renderer, matrix, alpha);
        }

        internal override GLVisual? HitTest(Vector2 screenPoint, Matrix4X4<float> parentMatrix)
        {
            if (!Visible)
                return null;

            Matrix4X4<float> world = LocalMatrix * parentMatrix;

            // Children draw ascending by layer (back-to-front), so the frontmost hit is
            // found by testing in reverse order.
            m_children.Sort((a, b) => a.Layer.CompareTo(b.Layer));
            for (int i = m_children.Count - 1; i >= 0; i--)
            {
                GLVisual? hit = m_children[i].HitTest(screenPoint, world);
                if (hit != null)
                    return hit;
            }

            // Otherwise the container itself, if it is hittable and has extent.
            if ((MouseOptions & MouseOptions.Hittable) != 0 && ContainsPoint(screenPoint, world))
                return this;

            return null;
        }

        protected override void DisposeCore() => RemoveAllChildren();
    }
}
