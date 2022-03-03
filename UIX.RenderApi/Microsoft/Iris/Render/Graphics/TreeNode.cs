// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Graphics.TreeNode
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using System;
using System.Diagnostics;

namespace Microsoft.Iris.Render.Graphics
{
    internal abstract class TreeNode : CachedRenderObject
    {
        private static int s_idObject;
        private ITreeOwner m_treeOwner;
        private int m_idObject;
        private TreeNode m_nodeParent;
        private TreeNode m_nodeFirstChild;
        private TreeNode m_nodeNext;
        private TreeNode m_nodePrevious;
        public static int s_DEBUG_nTotalNodes;

        internal TreeNode(ITreeOwner iTreeOwner)
        {
            Debug2.Validate(iTreeOwner != null, typeof(ArgumentNullException), "Must have a valid ITreeOwner when creating a TreeNode");
            this.m_treeOwner = iTreeOwner;
        }

        protected override void Dispose(bool fInDispose)
        {
            try
            {
                if (fInDispose)
                {
                    this.ChangeParent(null);
                    this.RemoveAllChildren();
                }
                this.m_treeOwner = null;
            }
            finally
            {
                base.Dispose(fInDispose);
            }
        }

        private ITreeOwner Tree => this.m_treeOwner;

        internal bool HasChildren => this.m_nodeFirstChild != null;

        internal bool IsRoot => this.m_treeOwner.Root == this;

        internal TreeNode Parent => this.m_nodeParent;

        internal TreeNode NextSibling => this.m_nodeNext;

        internal TreeNode PreviousSibling => this.m_nodePrevious;

        internal TreeNode FirstSibling => this.m_nodeParent == null ? this : this.m_nodeParent.m_nodeFirstChild;

        internal TreeNode LastSibling
        {
            get
            {
                TreeNode treeNode = this;
                while (treeNode.m_nodeNext != null)
                    treeNode = treeNode.m_nodeNext;
                return treeNode;
            }
        }

        internal TreeNode FirstChild => this.m_nodeFirstChild;

        internal TreeNode LastChild => this.m_nodeFirstChild != null ? this.m_nodeFirstChild.LastSibling : null;

        internal TreeNodeCollection Children => new TreeNodeCollection(this);

        internal AncestorEnumerator Ancestors => new AncestorEnumerator(this);

        internal bool HasDescendant(TreeNode nodeOther)
        {
            for (; nodeOther != null; nodeOther = nodeOther.m_nodeParent)
            {
                if (nodeOther == this)
                    return true;
            }
            return false;
        }

        public int ChildCount
        {
            get
            {
                int num = 0;
                for (TreeNode treeNode = this.m_nodeFirstChild; treeNode != null; treeNode = treeNode.m_nodeNext)
                    ++num;
                return num;
            }
        }

        internal virtual void RemoveAllChildren()
        {
            while (this.m_nodeFirstChild != null)
                this.m_nodeFirstChild.ChangeParent(null);
        }

        internal void ChangeParent(TreeNode nodeNewParent) => this.ChangeParent(nodeNewParent, null, LinkType.First);

        internal void ChangeParent(TreeNode nodeNewParent, TreeNode nodeSibling, TreeNode.LinkType lt)
        {
            if (this.m_nodeParent == nodeNewParent)
                return;
            TreeNode nodeParent = this.m_nodeParent;
            if (this.m_nodeParent != null)
                DoUnlink(this);
            if (nodeNewParent != null)
                DoLink(nodeNewParent, this, nodeSibling, lt);
            if (nodeParent == null && nodeNewParent != null)
            {
                this.RegisterUsage(this);
            }
            else
            {
                if (nodeParent == null || nodeNewParent != null)
                    return;
                this.UnregisterUsage(this);
            }
        }

        private static void DoLink(
          TreeNode nodeParent,
          TreeNode nodeChange,
          TreeNode nodeSibling,
          TreeNode.LinkType lt)
        {
            nodeChange.m_nodeParent = nodeParent;
            TreeNode nodeFirstChild = nodeParent.m_nodeFirstChild;
            if (nodeFirstChild == null)
            {
                nodeParent.m_nodeFirstChild = nodeChange;
            }
            else
            {
                switch (lt)
                {
                    case LinkType.Before:
                        nodeChange.m_nodeNext = nodeSibling;
                        nodeChange.m_nodePrevious = nodeSibling.m_nodePrevious;
                        nodeSibling.m_nodePrevious = nodeChange;
                        if (nodeChange.m_nodePrevious != null)
                        {
                            nodeChange.m_nodePrevious.m_nodeNext = nodeChange;
                            break;
                        }
                        nodeParent.m_nodeFirstChild = nodeChange;
                        break;
                    case LinkType.Behind:
                        nodeChange.m_nodePrevious = nodeSibling;
                        nodeChange.m_nodeNext = nodeSibling.m_nodeNext;
                        nodeSibling.m_nodeNext = nodeChange;
                        if (nodeChange.m_nodeNext == null)
                            break;
                        nodeChange.m_nodeNext.m_nodePrevious = nodeChange;
                        break;
                    case LinkType.First:
                        nodeParent.m_nodeFirstChild = nodeChange;
                        nodeChange.m_nodeNext = nodeFirstChild;
                        if (nodeFirstChild == null)
                            break;
                        nodeFirstChild.m_nodePrevious = nodeChange;
                        break;
                    case LinkType.Last:
                        TreeNode lastSibling = nodeFirstChild.LastSibling;
                        lastSibling.m_nodeNext = nodeChange;
                        nodeChange.m_nodePrevious = lastSibling;
                        break;
                }
            }
        }

        private static void DoUnlink(TreeNode nodeChange)
        {
            if (nodeChange.m_nodeParent.m_nodeFirstChild == nodeChange)
                nodeChange.m_nodeParent.m_nodeFirstChild = nodeChange.m_nodeNext;
            if (nodeChange.m_nodeNext != null)
                nodeChange.m_nodeNext.m_nodePrevious = nodeChange.m_nodePrevious;
            if (nodeChange.m_nodePrevious != null)
                nodeChange.m_nodePrevious.m_nodeNext = nodeChange.m_nodeNext;
            nodeChange.m_nodeParent = null;
            nodeChange.m_nodeNext = null;
            nodeChange.m_nodePrevious = null;
        }

        internal int InstanceID
        {
            get
            {
                if (this.m_idObject == 0)
                    this.SetInstanceID(GenerateInstanceID());
                return this.m_idObject;
            }
        }

        private static int GenerateInstanceID() => ++s_idObject;

        private void SetInstanceID(int idObject) => this.m_idObject = idObject;

        public override int GetHashCode() => this.InstanceID;

        public override bool Equals(object oRHS) => ReferenceEquals(this, oRHS);

        public bool TestNodes(
          TreeNode.NodeTest test,
          TreeNode.NodeRelation nScope,
          bool fParentChainsMustGoToRoot)
        {
            if ((nScope & NodeRelation.FullChain) == NodeRelation.None || (nScope & NodeRelation.Self) != NodeRelation.None && !test(this))
                return false;
            if ((nScope & NodeRelation.Parents) != NodeRelation.None)
            {
                TreeNode node = this;
                do
                {
                    TreeNode parent = node.Parent;
                    if (parent != null)
                        node = parent;
                    else
                        goto label_7;
                }
                while (test(node));
                return false;
            label_7:
                if (fParentChainsMustGoToRoot && node != this.m_treeOwner.Root)
                    return false;
            }
            return true;
        }

        internal int DEBUG_CurrentGeneration => 0;

        [Conditional("DEBUG")]
        internal void DEBUG_CheckGeneration(int idxIntendedGeneration)
        {
        }

        [Conditional("DEBUG")]
        internal void DEBUG_IncTotalNodes() => ++s_DEBUG_nTotalNodes;

        [Conditional("DEBUG")]
        internal void DEBUG_DecTotalNodes() => --s_DEBUG_nTotalNodes;

        internal enum LinkType
        {
            Before = 1,
            Behind = 2,
            First = 3,
            Last = 4,
        }

        [Flags]
        public enum NodeRelation
        {
            None = 0,
            Self = 1,
            Parents = 2,
            FullChain = Parents | Self, // 0x00000003
        }

        public delegate bool NodeTest(TreeNode node);
    }
}
