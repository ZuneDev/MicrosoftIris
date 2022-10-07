// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Library.TreeNodeCollection
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System;
using System.Collections;

namespace Microsoft.Iris.Library
{
    public struct TreeNodeCollection : IList, ICollection, IEnumerable
    {
        private TreeNode _nodeSubject;

        internal TreeNodeCollection(TreeNode nodeSubject) => _nodeSubject = nodeSubject;

        public int Count => _nodeSubject.ChildCount;

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => (object)null;

        bool IList.IsFixedSize => false;

        bool IList.IsReadOnly => false;

        object IList.this[int index]
        {
            get => this[index];
            set
            {
            }
        }

        public TreeNode this[int childIndex]
        {
            get
            {
                int num = 0;
                foreach (TreeNode treeNode in this)
                {
                    if (childIndex == num)
                        return treeNode;
                    ++num;
                }
                return null;
            }
            set
            {
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public TreeNodeEnumerator GetEnumerator() => new TreeNodeEnumerator(_nodeSubject);

        void ICollection.CopyTo(Array destList, int destIndex)
        {
            foreach (TreeNode treeNode in this)
                destList.SetValue(treeNode, destIndex++);
        }

        public void CopyTo(TreeNode[] destList, int destIndex) => ((ICollection)this).CopyTo(destList, destIndex);

        int IList.Add(object value)
        {
            Add((TreeNode)value);
            return _nodeSubject.ChildCount - 1;
        }

        public void Add(TreeNode nodeChild) => nodeChild.ChangeParent(_nodeSubject, null, TreeNode.LinkType.Last);

        public void Clear() => _nodeSubject.RemoveAllChildren(true);

        bool IList.Contains(object nodeChild) => Contains((TreeNode)nodeChild);

        public bool Contains(TreeNode nodeChild) => nodeChild.Parent == _nodeSubject;

        int IList.IndexOf(object nodeChild) => IndexOf((TreeNode)nodeChild);

        public int IndexOf(TreeNode nodeChild)
        {
            if (nodeChild.Parent != _nodeSubject)
                return -1;
            int num = 0;
            foreach (TreeNode treeNode in this)
            {
                if (nodeChild == treeNode)
                    return num;
                ++num;
            }
            return -1;
        }

        void IList.Insert(int insertAtIndex, object nodeChild) => Insert(insertAtIndex, (TreeNode)nodeChild);

        public void Insert(int insertAtIndex, TreeNode nodeChild)
        {
            TreeNode nodeSibling = null;
            TreeNode.LinkType lt = TreeNode.LinkType.Last;
            if (insertAtIndex < Count)
            {
                nodeSibling = this[insertAtIndex];
                lt = TreeNode.LinkType.Before;
            }
            nodeChild.ChangeParent(_nodeSubject, nodeSibling, lt);
        }

        void IList.Remove(object nodeChild) => Remove((TreeNode)nodeChild);

        public void Remove(TreeNode nodeChild) => nodeChild.ChangeParent(null);

        public void RemoveAt(int removeAtIndex) => this[removeAtIndex].ChangeParent(null);
    }
}
