// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Graphics.TreeNodeCollection
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using System;
using System.Collections;

namespace Microsoft.Iris.Render.Graphics
{
    internal struct TreeNodeCollection : IList, ICollection, IEnumerable
    {
        private TreeNode m_nodeSubject;

        internal TreeNodeCollection(TreeNode nodeSubject) => this.m_nodeSubject = nodeSubject;

        public int Count => this.m_nodeSubject.ChildCount;

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => throw new InvalidOperationException("This collection may only be used on the correct thread");

        bool IList.IsFixedSize => false;

        bool IList.IsReadOnly => false;

        object IList.this[int index]
        {
            get => this[index];
            set => Debug2.Validate(false, typeof(InvalidOperationException), "Use Add() and Remove() to modify collection");
        }

        public TreeNode this[int idxChild]
        {
            get
            {
                Debug2.Validate(idxChild >= 0, typeof(ArgumentOutOfRangeException), "Must have a valid index");
                int num = 0;
                foreach (TreeNode treeNode in this)
                {
                    if (idxChild == num)
                        return treeNode;
                    ++num;
                }
                Debug2.Validate(false, typeof(ArgumentOutOfRangeException), "Must have a valid index");
                return null;
            }
            set => Debug2.Validate(false, typeof(InvalidOperationException), "Use Add() and Remove() to modify collection");
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public TreeNodeEnumerator GetEnumerator() => new TreeNodeEnumerator(this.m_nodeSubject);

        void ICollection.CopyTo(Array arDest, int idxDest)
        {
            PromptCompatibleArray(arDest, idxDest, typeof(TreeNode), this.m_nodeSubject.ChildCount);
            foreach (TreeNode treeNode in this)
                arDest.SetValue(treeNode, idxDest++);
        }

        public void CopyTo(TreeNode[] arDest, int idxDest) => ((ICollection)this).CopyTo(arDest, idxDest);

        int IList.Add(object value)
        {
            this.Add((TreeNode)value);
            return this.m_nodeSubject.ChildCount - 1;
        }

        public void Add(TreeNode nodeChild)
        {
            Debug2.Validate(nodeChild != null, typeof(ArgumentNullException), "Must have a valid child");
            Debug2.Validate(nodeChild.Parent == null, typeof(ArgumentException), "Child must not already be parented");
            nodeChild.ChangeParent(this.m_nodeSubject, null, TreeNode.LinkType.Last);
        }

        public void Clear() => this.m_nodeSubject.RemoveAllChildren();

        bool IList.Contains(object nodeChild) => this.Contains((TreeNode)nodeChild);

        public bool Contains(TreeNode nodeChild)
        {
            Debug2.Validate(nodeChild != null, typeof(ArgumentNullException), "Must have a valid child");
            return nodeChild.Parent == this.m_nodeSubject;
        }

        int IList.IndexOf(object nodeChild) => this.IndexOf((TreeNode)nodeChild);

        public int IndexOf(TreeNode nodeChild)
        {
            Debug2.Validate(nodeChild != null, typeof(ArgumentNullException), "Must have a valid child");
            if (nodeChild.Parent != this.m_nodeSubject)
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

        void IList.Insert(int idxInsertAt, object nodeChild) => this.Insert(idxInsertAt, (TreeNode)nodeChild);

        public void Insert(int idxInsertAt, TreeNode nodeChild)
        {
            Debug2.Validate(nodeChild != null, typeof(ArgumentNullException), "Must have a valid child");
            Debug2.Validate(nodeChild.Parent == null, typeof(ArgumentException), "Child must not already be parented");
            TreeNode nodeSibling = null;
            TreeNode.LinkType lt = TreeNode.LinkType.Last;
            if (idxInsertAt < this.Count)
            {
                nodeSibling = this[idxInsertAt];
                lt = TreeNode.LinkType.Before;
            }
            nodeChild.ChangeParent(this.m_nodeSubject, nodeSibling, lt);
        }

        void IList.Remove(object nodeChild) => this.Remove((TreeNode)nodeChild);

        public void Remove(TreeNode nodeChild)
        {
            Debug2.Validate(nodeChild != null, typeof(ArgumentNullException), "must pass a valid TreeNode");
            nodeChild.ChangeParent(null);
        }

        public void RemoveAt(int idxRemoveAt) => this[idxRemoveAt].ChangeParent(null);

        public static void PromptCompatibleArray(
          Array array,
          int indexStart,
          Type typeObject,
          int numItems)
        {
            Debug2.Validate(array != null, typeof(ArgumentNullException), "Must have valid array");
            Debug2.Validate(typeObject != null, typeof(ArgumentNullException), "Must have valid Type");
            Debug2.Validate(array.Rank == 1, typeof(RankException), "Array must have rank=1");
            Debug2.Validate(indexStart < array.Length || numItems == 0, typeof(ArgumentException), "Must have valid index");
            Debug2.Validate(indexStart >= 0, typeof(ArgumentOutOfRangeException), "Must have valid index");
            Debug2.Validate(indexStart + numItems <= array.Length, typeof(ArgumentOutOfRangeException), "Must have sufficient room in array");
            Type elementType = array.GetType().GetElementType();
            Debug2.Validate(typeObject == elementType || typeObject.IsSubclassOf(elementType), typeof(ArrayTypeMismatchException), "Must be a compatible array type");
        }
    }
}
