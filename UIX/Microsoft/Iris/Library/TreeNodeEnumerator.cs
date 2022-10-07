// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Library.TreeNodeEnumerator
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System.Collections;

namespace Microsoft.Iris.Library
{
    public struct TreeNodeEnumerator : IEnumerator
    {
        private TreeNode _nodeParent;
        private TreeNode _nodeCurrent;
        private TreeNode _nodeNext;

        internal TreeNodeEnumerator(TreeNode nodeParent)
        {
            _nodeParent = nodeParent;
            _nodeCurrent = null;
            _nodeNext = nodeParent.FirstChild;
        }

        object IEnumerator.Current => _nodeCurrent;

        public TreeNode Current => _nodeCurrent;

        public void Reset()
        {
            _nodeCurrent = null;
            _nodeNext = _nodeParent.FirstChild;
        }

        public bool MoveNext()
        {
            _nodeCurrent = _nodeNext;
            if (_nodeNext == null)
                return false;
            _nodeNext = _nodeNext.NextSibling;
            return true;
        }
    }
}
