// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Library.TreeNode
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Session;
using Microsoft.Iris.UI;
using System;

namespace Microsoft.Iris.Library
{
    public abstract class TreeNode : DisposableObject, ITreeNode
    {
        private static readonly DataCookie s_instanceIDProperty = DataCookie.ReserveSlot();
        private static readonly EventCookie s_deepParentChangeEvent = EventCookie.ReserveSlot();
        private TreeNode _nodeParent;
        private TreeNode _nodeFirstChild;
        private TreeNode _nodeNext;
        private TreeNode _nodePrevious;
        private UIZone _zone;
        private DynamicData _dataMap;

        public TreeNode()
        {
            _dataMap = new DynamicData();
            _dataMap.Create();
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            ChangeParent(null);
            RemoveEventHandlers(s_deepParentChangeEvent);
        }

        public bool IsZoned => _zone != null;

        public void ChangeParent(TreeNode nodeNewParent) => ChangeParent(nodeNewParent, null, LinkType.First);

        public void ChangeParent(TreeNode nodeNewParent, TreeNode nodeSibling, TreeNode.LinkType lt)
        {
            if (_nodeParent == nodeNewParent)
                return;
            UIZone zone = null;
            TreeNode nodeParent = _nodeParent;
            if (_nodeParent != null)
            {
                DoUnlink(this);
                nodeParent.OnChildrenChanged();
            }
            if (nodeNewParent != null)
            {
                DoLink(nodeNewParent, this, nodeSibling, lt);
                zone = nodeNewParent.Zone;
                nodeNewParent.OnChildrenChanged();
            }
            PropagateZone(zone);
            FireTreeChangeWorker();
        }

        public void PropagateZone(UIZone zone)
        {
            if (_zone == zone)
                return;
            if (_zone != null)
                OnZoneDetached();
            _zone = zone;
            if (zone != null)
                OnZoneAttached();
            foreach (TreeNode child in Children)
                child.PropagateZone(zone);
        }

        public void MoveNode(TreeNode nodeSibling, TreeNode.LinkType lt)
        {
            TreeNode nodeParent = _nodeParent;
            DoUnlink(this);
            DoLink(nodeParent, this, nodeSibling, lt);
        }

        public void RemoveAllChildren(bool disposeChildrenFlag)
        {
            while (_nodeFirstChild != null)
                _nodeFirstChild.ChangeParent(null);
        }

        protected virtual void OnZoneAttached()
        {
        }

        protected virtual void OnZoneDetached()
        {
        }

        protected virtual void OnChildrenChanged()
        {
        }

        public event EventHandler DeepParentChange
        {
            add => AddEventHandler(s_deepParentChangeEvent, value);
            remove => RemoveEventHandler(s_deepParentChangeEvent, value);
        }

        public UIZone Zone => _zone;

        UIZone ITreeNode.Zone => _zone;

        public UISession UISession => _zone.Session;

        public bool HasChildren => _nodeFirstChild != null;

        public abstract bool IsRoot { get; }

        ITreeNode ITreeNode.Parent => _nodeParent;

        public TreeNode Parent => _nodeParent;

        public TreeNode NextSibling => _nodeNext;

        public TreeNode PreviousSibling => _nodePrevious;

        public TreeNode FirstSibling => _nodeParent == null ? this : _nodeParent._nodeFirstChild;

        public TreeNode LastSibling
        {
            get
            {
                TreeNode treeNode = this;
                while (treeNode._nodeNext != null)
                    treeNode = treeNode._nodeNext;
                return treeNode;
            }
        }

        public TreeNode FirstChild => _nodeFirstChild;

        public TreeNode LastChild => _nodeFirstChild != null ? _nodeFirstChild.LastSibling : null;

        public TreeNodeCollection Children => new TreeNodeCollection(this);

        public AncestorEnumerator Ancestors => new AncestorEnumerator(this);

        public bool HasDescendant(TreeNode nodeOther)
        {
            for (; nodeOther != null; nodeOther = nodeOther._nodeParent)
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
                for (TreeNode treeNode = _nodeFirstChild; treeNode != null; treeNode = treeNode._nodeNext)
                    ++num;
                return num;
            }
        }

        private static void DoLink(
          TreeNode nodeParent,
          TreeNode nodeChange,
          TreeNode nodeSibling,
          TreeNode.LinkType lt)
        {
            nodeChange._nodeParent = nodeParent;
            TreeNode nodeFirstChild = nodeParent._nodeFirstChild;
            if (nodeFirstChild == null)
            {
                nodeParent._nodeFirstChild = nodeChange;
            }
            else
            {
                switch (lt)
                {
                    case LinkType.Before:
                        nodeChange._nodeNext = nodeSibling;
                        nodeChange._nodePrevious = nodeSibling._nodePrevious;
                        nodeSibling._nodePrevious = nodeChange;
                        if (nodeChange._nodePrevious != null)
                        {
                            nodeChange._nodePrevious._nodeNext = nodeChange;
                            break;
                        }
                        nodeParent._nodeFirstChild = nodeChange;
                        break;
                    case LinkType.Behind:
                        nodeChange._nodePrevious = nodeSibling;
                        nodeChange._nodeNext = nodeSibling._nodeNext;
                        nodeSibling._nodeNext = nodeChange;
                        if (nodeChange._nodeNext == null)
                            break;
                        nodeChange._nodeNext._nodePrevious = nodeChange;
                        break;
                    case LinkType.First:
                        nodeParent._nodeFirstChild = nodeChange;
                        nodeChange._nodeNext = nodeFirstChild;
                        if (nodeFirstChild == null)
                            break;
                        nodeFirstChild._nodePrevious = nodeChange;
                        break;
                    case LinkType.Last:
                        TreeNode lastSibling = nodeFirstChild.LastSibling;
                        lastSibling._nodeNext = nodeChange;
                        nodeChange._nodePrevious = lastSibling;
                        break;
                }
            }
        }

        private static void DoUnlink(TreeNode nodeChange)
        {
            if (nodeChange._nodeParent._nodeFirstChild == nodeChange)
                nodeChange._nodeParent._nodeFirstChild = nodeChange._nodeNext;
            if (nodeChange._nodeNext != null)
                nodeChange._nodeNext._nodePrevious = nodeChange._nodePrevious;
            if (nodeChange._nodePrevious != null)
                nodeChange._nodePrevious._nodeNext = nodeChange._nodeNext;
            nodeChange._nodeParent = null;
            nodeChange._nodeNext = null;
            nodeChange._nodePrevious = null;
        }

        private void FireTreeChangeWorker()
        {
            foreach (TreeNode child in Children)
                child.FireTreeChangeWorker();
            if (!(GetEventHandler(s_deepParentChangeEvent) is EventHandler eventHandler))
                return;
            eventHandler(this, EventArgs.Empty);
        }

        protected object GetData(DataCookie cookie) => _dataMap.GetData(cookie);

        protected void SetData(DataCookie cookie, object value) => _dataMap.SetData(cookie, value);

        protected Delegate GetEventHandler(EventCookie cookie) => _dataMap.GetEventHandler(cookie);

        protected bool AddEventHandler(EventCookie cookie, Delegate handlerToAdd) => _dataMap.AddEventHandler(cookie, handlerToAdd);

        protected bool RemoveEventHandler(EventCookie cookie, Delegate handlerToRemove) => _dataMap.RemoveEventHandler(cookie, handlerToRemove);

        protected void RemoveEventHandlers(EventCookie cookie) => _dataMap.RemoveEventHandlers(cookie);

        private static uint GetKey(EventCookie cookie) => EventCookie.ToUInt32(cookie);

        public enum LinkType
        {
            Before = 1,
            Behind = 2,
            First = 3,
            Last = 4,
        }
    }
}
