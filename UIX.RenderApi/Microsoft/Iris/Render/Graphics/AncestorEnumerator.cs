// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Graphics.AncestorEnumerator
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using System.Collections;

namespace Microsoft.Iris.Render.Graphics
{
    internal struct AncestorEnumerator : IEnumerator, IEnumerable
    {
        private TreeNode m_nodeStart;
        private TreeNode m_nodeCurrent;
        private TreeNode m_nodeNext;

        internal AncestorEnumerator(TreeNode nodeStart)
        {
            this.m_nodeStart = nodeStart;
            this.m_nodeCurrent = null;
            this.m_nodeNext = this.m_nodeStart;
        }

        object IEnumerator.Current => m_nodeCurrent;

        public TreeNode Current => this.m_nodeCurrent;

        IEnumerator IEnumerable.GetEnumerator() => this;

        public AncestorEnumerator GetEnumerator() => this;

        public void Reset()
        {
            this.m_nodeCurrent = null;
            this.m_nodeNext = this.m_nodeStart;
        }

        public bool MoveNext()
        {
            this.m_nodeCurrent = this.m_nodeNext;
            if (this.m_nodeNext == null)
                return false;
            this.m_nodeNext = this.m_nodeNext.Parent;
            return true;
        }
    }
}
