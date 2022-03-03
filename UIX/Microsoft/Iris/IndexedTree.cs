// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.IndexedTree
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System.Diagnostics;

namespace Microsoft.Iris
{
    internal class IndexedTree
    {
        private object _lockObj;
        private IndexedTree.TreeNode _root;
        private IndexedTree.TreeNode _poolHead;
        private int _lastSearchedIndex;

        public IndexedTree()
        {
            _lastSearchedIndex = -1;
            _lockObj = new object();
        }

        private void SetRoot(IndexedTree.TreeNode newValue)
        {
            if (_root == newValue)
                return;
            _root = newValue;
            _lastSearchedIndex = -1;
        }

        private IndexedTree.TreeNode AcquireTreeNode()
        {
            IndexedTree.TreeNode treeNode = _poolHead;
            if (_poolHead != null)
            {
                _poolHead = _poolHead.parent;
                treeNode.parent = null;
            }
            else
                treeNode = new IndexedTree.TreeNode();
            return treeNode;
        }

        private void ReclaimTreeNode(IndexedTree.TreeNode node)
        {
            node.parent = _poolHead;
            node.left = null;
            node.right = null;
            _poolHead = node;
        }

        public void Store(int index, object data)
        {
            lock (_lockObj)
            {
                IndexedTree.TreeNode newValue = Find(index);
                if (newValue == null)
                {
                    newValue = AcquireTreeNode();
                    newValue.delta = index;
                    newValue.parent = null;
                    if (_root != null)
                    {
                        _root.parent = newValue;
                        if (_root.delta < index)
                        {
                            newValue.left = _root;
                            if (_root.right != null && _root.right.delta + _root.delta > index)
                            {
                                _root.right.parent = newValue;
                                newValue.right = _root.right;
                                newValue.right.delta = _root.right.delta + _root.delta - index;
                                _root.right = null;
                            }
                        }
                        else
                        {
                            newValue.right = _root;
                            if (_root.left != null && _root.left.delta + _root.delta < index)
                            {
                                _root.left.parent = newValue;
                                newValue.left = _root.left;
                                newValue.left.delta = _root.left.delta + _root.delta - index;
                                _root.left = null;
                            }
                        }
                        _root.delta -= index;
                    }
                    SetRoot(newValue);
                }
                newValue.data = data;
            }
        }

        public void Remove(int index)
        {
            lock (_lockObj)
            {
                IndexedTree.TreeNode node = Find(index);
                if (node == null)
                    return;
                IndexedTree.TreeNode treeNode;
                for (; node.left != null; node = treeNode)
                {
                    if (node.right == null)
                    {
                        IndexedTree.TreeNode left = node.left;
                        if (node == _root)
                            SetRoot(left);
                        else if (node.IsRightBranch)
                            node.parent.right = left;
                        else
                            node.parent.left = left;
                        left.delta += node.delta;
                        left.parent = node.parent;
                        ReclaimTreeNode(node);
                        return;
                    }
                    treeNode = node.left;
                    int num1 = index + treeNode.delta;
                    while (treeNode.right != null)
                    {
                        treeNode = treeNode.right;
                        num1 += treeNode.delta;
                    }
                    node.data = treeNode.data;
                    node.delta = num1;
                    if (node == _root)
                        _lastSearchedIndex = -1;
                    int num2 = num1 - index;
                    node.left.delta -= num2;
                    node.right.delta -= num2;
                }
                IndexedTree.TreeNode right = node.right;
                if (node == _root)
                    SetRoot(right);
                else if (node.IsRightBranch)
                    node.parent.right = right;
                else
                    node.parent.left = right;
                if (right != null)
                {
                    right.delta += node.delta;
                    right.parent = node.parent;
                }
                ReclaimTreeNode(node);
            }
        }

        public bool TryGetData(int index, out object data)
        {
            lock (_lockObj)
            {
                IndexedTree.TreeNode treeNode = Find(index);
                data = treeNode == null ? null : treeNode.data;
                return treeNode != null;
            }
        }

        public void InsertRange(int index, int count)
        {
            lock (_lockObj)
            {
                Find(index);
                if (_root == null)
                    return;
                IndexedTree.TreeNode node = _root;
                if (_root.delta < index)
                    node = _root.right;
                ChangeIndex(node, count);
            }
        }

        private void ChangeIndex(IndexedTree.TreeNode node, int amount)
        {
            if (node == null)
                return;
            node.delta += amount;
            if (node.left == null)
                return;
            node.left.delta -= amount;
        }

        public void Insert(int index, bool setValue, object data)
        {
            lock (_lockObj)
            {
                InsertRange(index, 1);
                if (!setValue)
                    return;
                Store(index, data);
            }
        }

        public void RemoveIndex(int index)
        {
            lock (_lockObj)
            {
                IndexedTree.TreeNode treeNode = Find(index);
                if (treeNode != null)
                {
                    if (treeNode.right != null)
                        --treeNode.right.delta;
                    Remove(index);
                }
                else
                {
                    if (_root == null)
                        return;
                    IndexedTree.TreeNode node = _root;
                    if (_root.delta < index)
                        node = _root.right;
                    ChangeIndex(node, -1);
                }
            }
        }

        public void Clear()
        {
            lock (_lockObj)
                SetRoot(null);
        }

        public bool Contains(int index)
        {
            lock (_lockObj)
                return Find(index) != null;
        }

        public object this[int index]
        {
            get
            {
                lock (_lockObj)
                {
                    object data;
                    TryGetData(index, out data);
                    return data;
                }
            }
            set
            {
                lock (_lockObj)
                    Store(index, value);
            }
        }

        private IndexedTree.TreeNode Find(int index)
        {
            if (_lastSearchedIndex == index && _root != null)
                return _root.delta != index ? null : _root;
            _lastSearchedIndex = index;
            IndexedTree.TreeNode treeNode = _root;
            IndexedTree.TreeNode node = null;
            int num = 0;
            bool flag = false;
            while (!flag && treeNode != null)
            {
                num += treeNode.delta;
                node = treeNode;
                if (num == index)
                    flag = true;
                else
                    treeNode = num <= index ? treeNode.right : treeNode.left;
            }
            if (node != null)
                Splay(node);
            _lastSearchedIndex = index;
            return !flag ? null : treeNode;
        }

        private void Splay(IndexedTree.TreeNode node)
        {
            for (IndexedTree.TreeNode parent = node.parent; parent != null; parent = node.parent)
            {
                if (parent.parent == null)
                    Zig(node);
                else if (node.IsLeftBranch == parent.IsLeftBranch)
                    ZigZig(node, parent);
                else
                    ZigZag(node);
            }
            SetRoot(node);
        }

        private void Zig(IndexedTree.TreeNode item) => Rotate(item);

        private void ZigZig(IndexedTree.TreeNode item, IndexedTree.TreeNode parent)
        {
            Rotate(parent);
            Rotate(item);
        }

        private void ZigZag(IndexedTree.TreeNode item)
        {
            Rotate(item);
            Rotate(item);
        }

        private void Rotate(IndexedTree.TreeNode child)
        {
            IndexedTree.TreeNode parent = child.parent;
            int num1 = -child.delta;
            int num2 = child.delta + parent.delta;
            IndexedTree.TreeNode treeNode;
            if (child.IsLeftBranch)
            {
                treeNode = child.right;
                parent.left = treeNode;
                child.parent = parent.parent;
                child.right = parent;
            }
            else
            {
                treeNode = child.left;
                parent.right = treeNode;
                child.parent = parent.parent;
                child.left = parent;
            }
            if (treeNode != null)
            {
                treeNode.parent = parent;
                treeNode.delta += child.delta;
            }
            if (child.parent != null)
            {
                if (num2 < 0)
                    child.parent.left = child;
                else
                    child.parent.right = child;
            }
            child.delta = num2;
            parent.delta = num1;
            parent.parent = child;
        }

        public IndexedTree.TreeNode Root => _root;

        public IndexedTree.TreeEnumerator GetEnumerator() => new IndexedTree.TreeEnumerator(this);

        [Conditional("DEBUG")]
        private void DEBUG_BumpGeneration()
        {
        }

        public class TreeNode
        {
            public int delta;
            public IndexedTree.TreeNode left;
            public IndexedTree.TreeNode right;
            public IndexedTree.TreeNode parent;
            public object data;

            public bool IsLeftBranch => delta < 0;

            public bool IsRightBranch => delta > 0;
        }

        public struct TreeEntry
        {
            private int _index;
            private object _data;

            public TreeEntry(int index, object data)
            {
                _index = index;
                _data = data;
            }

            public int Index => _index;

            public object Value => _data;
        }

        public struct TreeEnumerator
        {
            private IndexedTree _tree;
            private bool _started;
            private IndexedTree.TreeNode _current;
            private int _index;

            public TreeEnumerator(IndexedTree tree)
            {
                _tree = tree;
                _current = _tree._root;
                _started = false;
                _index = _current != null ? _current.delta : -1;
            }

            [Conditional("DEBUG")]
            private void DEBUG_CheckGeneration()
            {
            }

            private void MoveToLeftmostChild()
            {
                if (_current == null)
                    return;
                while (_current.left != null)
                {
                    _current = _current.left;
                    _index += _current.delta;
                }
            }

            public bool MoveNext()
            {
                lock (_tree._lockObj)
                {
                    if (!_started)
                    {
                        _started = true;
                        MoveToLeftmostChild();
                    }
                    else if (_current.right != null)
                    {
                        _current = _current.right;
                        _index += _current.delta;
                        MoveToLeftmostChild();
                    }
                    else if (_current.IsLeftBranch)
                    {
                        _index -= _current.delta;
                        _current = _current.parent;
                    }
                    else
                    {
                        for (; _current != null && _current.IsRightBranch; _current = _current.parent)
                            _index -= _current.delta;
                        if (_current != null)
                        {
                            _index -= _current.delta;
                            _current = _current.parent;
                        }
                    }
                    return _current != null;
                }
            }

            public IndexedTree.TreeEntry Current => new IndexedTree.TreeEntry(_index, _current.data);
        }
    }
}
