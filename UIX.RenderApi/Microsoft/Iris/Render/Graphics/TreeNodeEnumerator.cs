// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Graphics.TreeNodeEnumerator
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using System.Collections;

namespace Microsoft.Iris.Render.Graphics
{
    internal struct TreeNodeEnumerator : IEnumerator
    {
        private TreeNode m_nodeParent;
        private TreeNode m_nodeCurrent;
        private TreeNode m_nodeNext;

        internal TreeNodeEnumerator(TreeNode nodeParent)
        {
            this.m_nodeParent = nodeParent;
            this.m_nodeCurrent = null;
            this.m_nodeNext = nodeParent.FirstChild;
        }

        object IEnumerator.Current => m_nodeCurrent;

        public TreeNode Current => this.m_nodeCurrent;

        public void Reset()
        {
            this.m_nodeCurrent = null;
            this.m_nodeNext = this.m_nodeParent.FirstChild;
        }

        public bool MoveNext()
        {
            this.m_nodeCurrent = this.m_nodeNext;
            if (this.m_nodeNext == null)
                return false;
            this.m_nodeNext = this.m_nodeNext.NextSibling;
            return true;
        }
    }
}
