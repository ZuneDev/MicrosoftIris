// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.ViewItems.Repeater
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Data;
using Microsoft.Iris.Debug;
using Microsoft.Iris.Input;
using Microsoft.Iris.Layout;
using Microsoft.Iris.Library;
using Microsoft.Iris.Markup;
using Microsoft.Iris.Navigation;
using Microsoft.Iris.RenderAPI.Drawing;
using Microsoft.Iris.Session;
using Microsoft.Iris.UI;
using System.Collections;
using System.Diagnostics;

namespace Microsoft.Iris.ViewItems
{
    internal class Repeater : ViewItem
    {
        internal const int c_defaultItemFocusOrderValue = 2147483646;
        private IList _source;
        private Vector<Repeater.RepeatedViewItemSet> _repeatedViewItems;
        private Vector<int> _outstandingDataIndexRequests;
        private int? _pendingIndexRequest;
        private Repeater.PendingIndexRequestType _pendingIndexRequestType;
        private int _sourceGeneration;
        private int _itemsCount;
        private string _contentName;
        private string _dividerName;
        private Repeater.ContentTypeHandler _ctproc;
        private bool _haveRequestedUpdate;
        private bool _ignoreCustomRepeaters;
        private bool _discardOffscreenVisuals;
        private bool _maintainFocusScreenLocation;
        private bool _focusNeedsRepairing;
        private int _defaultFocusIndex;
        private ViewItem _lastKeyFocusedItem;
        private ViewItem _lastMouseFocusedItem;
        private RepeaterContentSelector _contentSelector;
        private LayoutCompleteEventHandler _repeatedItemLayoutComplete;
        private static ChildFaultedInDelegate s_scrollIndexIntoViewHandler = new ChildFaultedInDelegate(ScrollIndexIntoViewItemFaultedIn);
        private static ChildFaultedInDelegate s_navigateIntoIndexHandler = new ChildFaultedInDelegate(NavigateIntoIndexItemFaultedIn);
        private static string[] s_repeatedItemParameters = new string[2]
        {
      "RepeatedItem",
      "RepeatedItemIndex"
        };
        private static string c_childIDSentinel = "Repeater child#";
        private static DeferredHandler s_listContentsChangedHandler = new DeferredHandler(AsyncListContentsChangedHandler);
        private static object s_unavailableObject = new object();

        protected override void OnDispose()
        {
            if (_source is INotifyList sourceA)
                sourceA.ContentsChanged -= new UIListContentsChangedHandler(QueueListContentsChanged);
            if (_source is IVirtualList sourceB)
                sourceB.RepeaterHost = null;
            _source = null;
            base.OnDispose();
        }

        protected override void OnZoneAttached()
        {
            base.OnZoneAttached();
            UI.DescendentKeyFocusChange += new InputEventHandler(OnDescendentKeyFocusChange);
            UI.DescendentMouseFocusChange += new InputEventHandler(OnDescendentMouseFocusChange);
        }

        protected override void OnZoneDetached()
        {
            base.OnZoneDetached();
            UI.DescendentKeyFocusChange -= new InputEventHandler(OnDescendentKeyFocusChange);
            UI.DescendentMouseFocusChange -= new InputEventHandler(OnDescendentMouseFocusChange);
        }

        public string ContentName
        {
            get => _contentName;
            set
            {
                if (!(_contentName != value))
                    return;
                _contentName = value;
                RebuildChildren();
                FireNotification(NotificationID.ContentName);
            }
        }

        public string DividerName
        {
            get => _dividerName;
            set
            {
                if (!(_dividerName != value))
                    return;
                _dividerName = value;
                RebuildChildren();
                FireNotification(NotificationID.DividerName);
            }
        }

        public IList Source
        {
            get => _source;
            set
            {
                if (_source == value)
                    return;
                if (_source != null)
                {
                    if (_source is INotifyList sourceA)
                        sourceA.ContentsChanged -= new UIListContentsChangedHandler(QueueListContentsChanged);
                    if (_source is IVirtualList sourceB)
                        sourceB.RepeaterHost = null;
                }
                _source = value;
                if (value != null)
                {
                    if (value is INotifyList notifyList)
                        notifyList.ContentsChanged += new UIListContentsChangedHandler(QueueListContentsChanged);
                    if (_source is IVirtualList source)
                        source.RepeaterHost = this;
                }
                RebuildChildren();
                FireNotification(NotificationID.Source);
            }
        }

        public IList ContentSelectors
        {
            get
            {
                if (_contentSelector == null)
                {
                    _contentSelector = new RepeaterContentSelector(this);
                    PreRepeatHandler = _contentSelector.ContentTypeHandler;
                }
                return _contentSelector.Selectors;
            }
        }

        public Repeater.ContentTypeHandler PreRepeatHandler
        {
            get => _ctproc;
            set => _ctproc = value;
        }

        public bool IgnoreCustomRepeaters
        {
            get => _ignoreCustomRepeaters;
            set => _ignoreCustomRepeaters = value;
        }

        public override bool DiscardOffscreenVisuals
        {
            get => _discardOffscreenVisuals;
            set
            {
                if (_discardOffscreenVisuals == value)
                    return;
                if (_repeatedItemLayoutComplete == null && HasChildren)
                    _repeatedItemLayoutComplete = new LayoutCompleteEventHandler(OnRepeatedItemLayoutComplete);
                if (_discardOffscreenVisuals)
                {
                    foreach (ViewItem child in Children)
                        child.LayoutComplete -= _repeatedItemLayoutComplete;
                }
                _discardOffscreenVisuals = value;
                if (_discardOffscreenVisuals)
                {
                    foreach (ViewItem child in Children)
                        child.LayoutComplete += _repeatedItemLayoutComplete;
                }
                FireNotification(NotificationID.DiscardOffscreenVisuals);
            }
        }

        public bool MaintainFocusedItemOnSourceChanges
        {
            get => !_maintainFocusScreenLocation;
            set
            {
                if (_maintainFocusScreenLocation == value)
                    return;
                _maintainFocusScreenLocation = value;
                FireNotification(NotificationID.MaintainFocusedItemOnSourceChanges);
            }
        }

        internal bool GetFocusedIndex(ref int index)
        {
            bool flag = false;
            if (GetExtendedLayoutOutput(VisibleIndexRangeLayoutOutput.DataCookie) is VisibleIndexRangeLayoutOutput extendedLayoutOutput && extendedLayoutOutput.FocusedItem.HasValue)
            {
                index = extendedLayoutOutput.FocusedItem.Value;
                flag = true;
            }
            return flag;
        }

        public override bool HideNamedChildren => true;

        public void ForceRefresh() => RebuildChildren();

        private int InternalCount
        {
            get => _itemsCount;
            set
            {
                if (_itemsCount == value)
                    return;
                _itemsCount = value;
                SetLayoutInput(new CountLayoutInput(_itemsCount));
            }
        }

        private void RebuildChildren()
        {
            if (HasVisual)
                UI.DestroyVisualTree(this, true);
            if (_repeatedViewItems != null)
            {
                UIClass keyFocusDescendant = UI.KeyFocusDescendant;
                foreach (Repeater.RepeatedViewItemSet repeatedViewItem in _repeatedViewItems)
                    DisposeRepeatedViewItemSet(repeatedViewItem, ref keyFocusDescendant);
            }
            _repeatedViewItems = new Vector<Repeater.RepeatedViewItemSet>();
            _outstandingDataIndexRequests = new Vector<int>();
            _pendingIndexRequest = new int?();
            _lastMouseFocusedItem = null;
            _lastKeyFocusedItem = null;
            int num = 0;
            if (_source != null)
                num = _source.Count;
            InternalCount = num;
            ++_sourceGeneration;
            RequestRepeatOfIndexUpdate();
        }

        public override void OnCommit()
        {
            if (LayoutRequestedCount <= 0 && LayoutRequestedIndices == null || LayoutRequestedCount > 0 && LayoutRequestedIndices == null && (_repeatedViewItems != null && InternalCount == _repeatedViewItems.Count))
                return;
            RequestRepeatOfIndexUpdate();
        }

        public void UpdateBinding()
        {
            if (_source == null)
                return;
            if (_itemsCount != _source.Count)
            {
                RebuildChildren();
            }
            else
            {
                if (Debug.Trace.IsCategoryEnabled(TraceCategory.Repeating, 5))
                {
                    int num = _pendingIndexRequest.HasValue ? 1 : 0;
                }
                Vector<int> vector = GetLayoutRepeatRequests();
                if (_pendingIndexRequest.HasValue)
                {
                    if (vector == null)
                        vector = new Vector<int>();
                    vector.Add(_pendingIndexRequest.Value);
                }
                if (vector == null)
                    return;
                foreach (int virtualIndex in vector)
                {
                    int dataIndex;
                    int generationValue;
                    object dataItemObject;
                    if (GetDataItem(virtualIndex, out dataIndex, out generationValue, out dataItemObject))
                    {
                        RepeatItem(virtualIndex, dataIndex, generationValue, dataItemObject);
                        if (_pendingIndexRequest.HasValue && _pendingIndexRequest.Value == virtualIndex)
                            _pendingIndexRequest = new int?();
                    }
                }
            }
        }

        private Vector<int> GetLayoutRepeatRequests()
        {
            if (LayoutRequestedCount == 0 && LayoutRequestedIndices == null)
                return null;
            int layoutRequestedCount = LayoutRequestedCount;
            Vector<int> requestedIndices = LayoutRequestedIndices;
            Vector<int> indicesToRequest = null;
            if (layoutRequestedCount > 0)
                indicesToRequest = GetMissingIndices(layoutRequestedCount);
            if (requestedIndices != null)
            {
                foreach (int requestIndex in requestedIndices)
                    RequestRepeatOfIndex(ref indicesToRequest, requestIndex);
            }
            return indicesToRequest;
        }

        private Vector<int> GetMissingIndices(int howManyMoreCount)
        {
            Vector<int> indicesToRequest = null;
            int dataStartIndex = 0;
            if (!ListUtility.IsNullOrEmpty(_repeatedViewItems))
            {
                foreach (Repeater.RepeatedViewItemSet repeatedViewItem in _repeatedViewItems)
                {
                    int dataIndex = repeatedViewItem.DataIndex;
                    if (dataIndex > dataStartIndex)
                        RequestRepeatOfIndexRange(ref indicesToRequest, dataStartIndex, dataIndex, ref howManyMoreCount);
                    else if (dataIndex < dataStartIndex)
                    {
                        RequestRepeatOfIndexRange(ref indicesToRequest, dataStartIndex, _itemsCount, ref howManyMoreCount);
                        RequestRepeatOfIndexRange(ref indicesToRequest, 0, dataIndex, ref howManyMoreCount);
                    }
                    dataStartIndex = dataIndex + 1;
                    if (howManyMoreCount <= 0 || dataStartIndex >= _itemsCount)
                        break;
                }
            }
            if (howManyMoreCount > 0 && dataStartIndex < _itemsCount)
                RequestRepeatOfIndexRange(ref indicesToRequest, dataStartIndex, _itemsCount, ref howManyMoreCount);
            return indicesToRequest;
        }

        private void RequestRepeatOfIndexRange(
          ref Vector<int> indicesToRequest,
          int dataStartIndex,
          int dataEndIndex,
          ref int howManyMoreCount)
        {
            if (howManyMoreCount <= 0)
                return;
            for (int requestIndex = dataStartIndex; requestIndex < dataEndIndex; ++requestIndex)
            {
                if (RequestRepeatOfIndex(ref indicesToRequest, requestIndex))
                    --howManyMoreCount;
                if (howManyMoreCount <= 0)
                    break;
            }
        }

        private bool RequestRepeatOfIndex(ref Vector<int> indicesToRequest, int requestIndex)
        {
            if (indicesToRequest != null && indicesToRequest.Contains(requestIndex) || HasVirtualIndexBeenRepeated(requestIndex))
                return false;
            if (indicesToRequest == null)
                indicesToRequest = new Vector<int>();
            indicesToRequest.Add(requestIndex);
            return true;
        }

        private bool GetDataItem(
          int virtualIndex,
          out int dataIndex,
          out int generationValue,
          out object dataItemObject)
        {
            bool flag = false;
            ListUtility.GetWrappedIndex(virtualIndex, _source.Count, out dataIndex, out generationValue);
            if (!ListUtility.IsValidIndex(_source, dataIndex))
                dataItemObject = null;
            else if (_source is IVirtualList source && !source.IsItemAvailable(dataIndex))
            {
                if (!_outstandingDataIndexRequests.Contains(dataIndex))
                {
                    _outstandingDataIndexRequests.Add(dataIndex);
                    source.RequestItem(dataIndex, QueryHandler);
                }
                dataItemObject = null;
            }
            else
            {
                dataItemObject = _source[dataIndex];
                flag = true;
            }
            return flag;
        }

        private void RepeatItem(
          int virtualIndex,
          int dataIndex,
          int generationValue,
          object dataItemObject)
        {
            if (dataItemObject == UnavailableItem)
                return;
            bool flag = false;
            IVirtualList source = null;
            if (_source is IVirtualList)
            {
                source = (IVirtualList)_source;
                flag = !HasDataIndexBeenRepeated(dataIndex);
            }
            Index index = new Index(virtualIndex, dataIndex, this);
            ErrorManager.EnterContext(UI.TypeSchema);
            try
            {
                ViewItem repeatedItem;
                ViewItem dividerItem;
                CreateRepeatedItem(index, dataItemObject, out repeatedItem, out dividerItem);
                int repeatedItemIndex;
                int viewItemIndex;
                GetInsertIndex(index.Value, out repeatedItemIndex, out viewItemIndex);
                if (repeatedItem != null)
                    Children.Insert(viewItemIndex, repeatedItem);
                if (dividerItem != null)
                    Children.Insert(viewItemIndex, dividerItem);
                if (repeatedItem == null)
                    return;
                _repeatedViewItems.Insert(repeatedItemIndex, new Repeater.RepeatedViewItemSet(index, generationValue, repeatedItem, dividerItem));
                if (flag)
                    source.NotifyVisualsCreated(dataIndex);
                if (virtualIndex == _defaultFocusIndex)
                    SetAsDefaultFocusRecipient(repeatedItem);
                if (!_discardOffscreenVisuals)
                    return;
                if (_repeatedItemLayoutComplete == null)
                    _repeatedItemLayoutComplete = new LayoutCompleteEventHandler(OnRepeatedItemLayoutComplete);
                repeatedItem.LayoutComplete += _repeatedItemLayoutComplete;
            }
            finally
            {
                ErrorManager.ExitContext();
            }
        }

        private void CreateRepeatedItem(
          Index index,
          object dataItemObject,
          out ViewItem repeatedItem,
          out ViewItem dividerItem)
        {
            ParameterContext parameterContext = new ParameterContext(s_repeatedItemParameters, new object[2]
            {
        dataItemObject,
         index
            });
            string contentTypeName = null;
            GetContentTypeForRepeatedItem(dataItemObject, out contentTypeName);
            repeatedItem = null;
            if (!string.IsNullOrEmpty(contentTypeName))
            {
                repeatedItem = UI.ConstructNamedContent(contentTypeName, parameterContext);
                if (repeatedItem == null)
                {
                    if (contentTypeName[0] == '#')
                        ErrorManager.ReportError("Repeater unable to create inline content");
                    else
                        ErrorManager.ReportError("Repeater failed to find content to repeat. ContentName was '{0}'", contentTypeName);
                }
            }
            else
                ErrorManager.ReportError("Repeater has no content to repeat");
            dividerItem = null;
            if (index.SourceValue != 0 && _dividerName != null)
            {
                dividerItem = UI.ConstructNamedContent(_dividerName, parameterContext);
                if (dividerItem == null)
                    ErrorManager.ReportError("Repeater failed to find divider content to repeat (DividerName was '{0}')", _dividerName);
            }
            if (repeatedItem != null)
                repeatedItem.SetLayoutInput(new IndexLayoutInput(index, IndexType.Content));
            if (dividerItem == null)
                return;
            dividerItem.SetLayoutInput(new IndexLayoutInput(index, IndexType.Divider));
        }

        private void RequestRepeatOfIndexUpdate()
        {
            if (_haveRequestedUpdate)
                return;
            DeferredCall.Post(DispatchPriority.RepeatItem, new SimpleCallback(UpdateBindingCallback));
            _haveRequestedUpdate = true;
        }

        private void UpdateBindingCallback()
        {
            _haveRequestedUpdate = false;
            UpdateBinding();
        }

        private void ItemRequestCallback(object senderObject, int dataIndex, object dataItemObject)
        {
            IVirtualList virtualList = (IVirtualList)senderObject;
            if (IsDisposed || virtualList != _source || !_outstandingDataIndexRequests.Contains(dataIndex))
                return;
            _outstandingDataIndexRequests.Remove(dataIndex);
            RequestRepeatOfIndexUpdate();
        }

        private void OnDescendentMouseFocusChange(UIClass sender, InputInfo inputInfo)
        {
            MouseFocusInfo mouseFocusInfo = (MouseFocusInfo)inputInfo;
            ViewItem childFromDescendant = GetDirectChildFromDescendant(mouseFocusInfo.State ? mouseFocusInfo.Target as UIClass : null);
            if (childFromDescendant == _lastMouseFocusedItem)
                return;
            UpdateKeepAliveState(ref _lastMouseFocusedItem, childFromDescendant, _lastKeyFocusedItem);
        }

        private void OnDescendentKeyFocusChange(UIClass sender, InputInfo inputInfo)
        {
            KeyFocusInfo keyFocusInfo = (KeyFocusInfo)inputInfo;
            if (!keyFocusInfo.State)
                return;
            ViewItem childFromDescendant = GetDirectChildFromDescendant(keyFocusInfo.Target as UIClass);
            if (childFromDescendant == null || childFromDescendant == _lastKeyFocusedItem)
                return;
            UpdateKeepAliveState(ref _lastKeyFocusedItem, childFromDescendant, _lastMouseFocusedItem);
        }

        private void UpdateKeepAliveState(
          ref ViewItem currentItem,
          ViewItem newItem,
          ViewItem otherItem)
        {
            if (currentItem != null && currentItem != otherItem && !currentItem.IsDisposed)
                currentItem.UnlockVisible();
            currentItem = newItem;
            if (currentItem == null)
                return;
            currentItem.LockVisible();
        }

        private ViewItem GetDirectChildFromDescendant(UIClass ui)
        {
            ViewItem viewItem = null;
            if (ui != null)
            {
                viewItem = ui.RootItem;
                while (viewItem != null && viewItem.Parent != this)
                    viewItem = viewItem.Parent;
            }
            return viewItem;
        }

        private void OnRepeatedItemLayoutComplete(object sender)
        {
            ViewItem viewItem = (ViewItem)sender;
            if (viewItem.HasVisual || viewItem.LayoutVisible)
                return;
            int virtualIndex = ((IndexLayoutInput)viewItem.GetLayoutInput(IndexLayoutInput.Data)).Index.Value;
            if (!viewItem.IsOffscreen)
                return;
            Repeater.RepeatedViewItemSet repeatedInstance = GetRepeatedInstance(virtualIndex);
            DeferredCall.Post(DispatchPriority.Housekeeping, new DeferredHandler(DeferredDisposeViewItem), repeatedInstance);
            _repeatedViewItems.Remove(repeatedInstance);
            viewItem.LayoutComplete -= _repeatedItemLayoutComplete;
            int dataIndex = repeatedInstance.DataIndex;
            if (HasDataIndexBeenRepeated(dataIndex) || !(_source is IVirtualList source))
                return;
            source.NotifyVisualsReleased(dataIndex);
        }

        private void DeferredDisposeViewItem(object arg) => DisposeRepeatedViewItemSet((Repeater.RepeatedViewItemSet)arg);

        private void DisposeRepeatedViewItemSet(Repeater.RepeatedViewItemSet viewItemSet)
        {
            UIClass keyFocusDescendant = UI.KeyFocusDescendant;
            DisposeRepeatedViewItemSet(viewItemSet, ref keyFocusDescendant);
        }

        private void DisposeRepeatedViewItemSet(
          Repeater.RepeatedViewItemSet viewItemSet,
          ref UIClass keyFocusDescendant)
        {
            if (viewItemSet.Repeated == _lastKeyFocusedItem)
                _lastKeyFocusedItem = null;
            if (viewItemSet.Repeated == _lastMouseFocusedItem)
                _lastMouseFocusedItem = null;
            viewItemSet.DisposeViewItems();
            if (keyFocusDescendant == null || keyFocusDescendant.IsValid)
                return;
            keyFocusDescendant = null;
            FireNotification(NotificationID.FocusedItemDiscarded);
        }

        private void GetRepeatedStats(out int minRepeatedIndex, out int maxRepeatedIndex)
        {
            if (_repeatedViewItems.Count == 0)
            {
                minRepeatedIndex = maxRepeatedIndex = -1;
            }
            else
            {
                Repeater.RepeatedViewItemSet repeatedViewItem1 = _repeatedViewItems[0];
                Repeater.RepeatedViewItemSet repeatedViewItem2 = _repeatedViewItems[_repeatedViewItems.Count - 1];
                minRepeatedIndex = repeatedViewItem1.VirtualIndex;
                maxRepeatedIndex = repeatedViewItem2.VirtualIndex;
            }
        }

        private bool HasDataIndexBeenRepeated(int dataIndex)
        {
            foreach (Repeater.RepeatedViewItemSet repeatedViewItem in _repeatedViewItems)
            {
                if (dataIndex == repeatedViewItem.DataIndex)
                    return true;
            }
            return false;
        }

        private bool HasVirtualIndexBeenRepeated(int virtualIndex)
        {
            foreach (Repeater.RepeatedViewItemSet repeatedViewItem in _repeatedViewItems)
            {
                if (virtualIndex == repeatedViewItem.VirtualIndex)
                    return true;
            }
            return false;
        }

        private void GetInsertIndex(int virtualIndex, out int repeatedItemIndex, out int viewItemIndex)
        {
            repeatedItemIndex = GetIndexOfClosestRepeatedItem(virtualIndex);
            viewItemIndex = GetViewItemIndex(repeatedItemIndex);
        }

        private int GetViewItemIndex(int repeatedItemIndex) => _dividerName == null ? repeatedItemIndex : (repeatedItemIndex != 0 ? repeatedItemIndex * 2 - 1 : 0);

        private void GetContentTypeForRepeatedItem(object repeatObject, out string contentTypeName)
        {
            contentTypeName = _contentName;
            if (!_ignoreCustomRepeaters && repeatObject is ICustomRepeatedItem customRepeatedItem && customRepeatedItem.UIName != null)
                contentTypeName = customRepeatedItem.UIName;
            if (_ctproc == null)
                return;
            _ctproc(repeatObject, ref contentTypeName);
        }

        private Microsoft.Iris.Data.ItemRequestCallback QueryHandler => new Microsoft.Iris.Data.ItemRequestCallback(ItemRequestCallback);

        protected override NavigationPolicies ForcedNavigationFlags => NavigationPolicies.Group;

        private void QueueListContentsChanged(IList senderList, UIListContentsChangedArgs args)
        {
            RepeaterListContentsChangedArgs contentsChangedArgs = new RepeaterListContentsChangedArgs(args, this, _sourceGeneration);
            DeferredCall.Post(DispatchPriority.Normal, s_listContentsChangedHandler, contentsChangedArgs);
        }

        private static void AsyncListContentsChangedHandler(object args)
        {
            RepeaterListContentsChangedArgs contentsChangedArgs = (RepeaterListContentsChangedArgs)args;
            Repeater target = contentsChangedArgs.Target;
            if (target.IsDisposed || target._sourceGeneration != contentsChangedArgs.Generation)
                return;
            target.OnListContentsChanged(contentsChangedArgs.ChangedArgs);
        }

        private void OnListContentsChanged(UIListContentsChangedArgs args)
        {
            int oldIndex = args.OldIndex;
            int newIndex = args.NewIndex;
            int count = args.Count;
            int? nullable = new int?();
            IndexLayoutInput indexLayoutInput = null;
            if (_lastKeyFocusedItem != null)
            {
                indexLayoutInput = _lastKeyFocusedItem.GetLayoutInput(IndexLayoutInput.Data) as IndexLayoutInput;
                nullable = new int?(indexLayoutInput.Index.Value);
            }
            bool flag = false;
            switch (args.Type)
            {
                case UIListContentsChangeType.Add:
                case UIListContentsChangeType.AddRange:
                    InternalCount += count;
                    RequestRepeatOfIndexUpdate();
                    ShiftRepeatedItemDataIndices(_itemsCount, count, InternalCount);
                    flag = true;
                    break;
                case UIListContentsChangeType.Remove:
                    Vector<Repeater.RepeatedViewItemSet> repeatedInstances1 = GetAllRepeatedInstances(oldIndex);
                    if (!ListUtility.IsNullOrEmpty(repeatedInstances1))
                    {
                        foreach (Repeater.RepeatedViewItemSet viewItemSet in repeatedInstances1)
                        {
                            DisposeRepeatedViewItemSet(viewItemSet);
                            _repeatedViewItems.Remove(viewItemSet);
                        }
                    }
                    ShiftPendingRequestIndices(oldIndex, -1);
                    --InternalCount;
                    ShiftRepeatedItemDataIndices(oldIndex, -1, InternalCount);
                    flag = true;
                    break;
                case UIListContentsChangeType.Move:
                    if (oldIndex == newIndex || MoveRepeatedViewItemSet(oldIndex, newIndex))
                        break;
                    goto default;
                case UIListContentsChangeType.Insert:
                case UIListContentsChangeType.InsertRange:
                    InternalCount += count;
                    ShiftRepeatedItemDataIndices(newIndex, count, InternalCount);
                    ShiftPendingRequestIndices(newIndex, count);
                    RequestRepeatOfIndexUpdate();
                    flag = true;
                    break;
                case UIListContentsChangeType.Clear:
                    RebuildChildren();
                    break;
                case UIListContentsChangeType.Modified:
                    Vector<Repeater.RepeatedViewItemSet> repeatedInstances2 = GetAllRepeatedInstances(oldIndex);
                    if (!ListUtility.IsNullOrEmpty(repeatedInstances2))
                    {
                        foreach (Repeater.RepeatedViewItemSet viewItemSet in repeatedInstances2)
                        {
                            DisposeRepeatedViewItemSet(viewItemSet);
                            _repeatedViewItems.Remove(viewItemSet);
                        }
                        break;
                    }
                    break;
                default:
                    RebuildChildren();
                    break;
            }
            if (nullable.HasValue && nullable.Value != indexLayoutInput.Index.Value && (UI.KeyFocus && UI.KeyFocusDescendant != null) && HasDescendant(UI.KeyFocusDescendant.RootItem))
                NavigationServices.SeedDefaultFocus(_lastKeyFocusedItem);
            if (!_maintainFocusScreenLocation || !flag || _focusNeedsRepairing)
                return;
            RectangleF descendantFocusRect = GetDescendantFocusRect();
            if (!(descendantFocusRect != RectangleF.Zero))
                return;
            _focusNeedsRepairing = true;
            bool keyFocusIsDefault = UISession.InputManager.Queue.PendingKeyFocusIsDefault;
            DeferredCall.Post(DispatchPriority.LayoutSync, new DeferredHandler(PatchUpFocus), new Repeater.FocusRepairArgs(descendantFocusRect, keyFocusIsDefault));
            UI.UISession.InputManager.SuspendInputUntil(DispatchPriority.LayoutSync);
        }

        private void PatchUpFocus(object focusArgs)
        {
            if (!_focusNeedsRepairing)
                return;
            _focusNeedsRepairing = false;
            Repeater.FocusRepairArgs focusRepairArgs = (Repeater.FocusRepairArgs)focusArgs;
            INavigationSite result;
            if (!NavigationServices.FindFromPoint(this, focusRepairArgs.focusBounds.Center, out result) || result == null || !(result is ViewItem viewItem))
                return;
            viewItem.UI.NotifyNavigationDestination(focusRepairArgs.focusIsDefault ? KeyFocusReason.Default : KeyFocusReason.Other);
        }

        private void ShiftRepeatedItemDataIndices(int baseIndex, int offsetValue, int updatedItemCount)
        {
            foreach (Repeater.RepeatedViewItemSet repeatedViewItem in _repeatedViewItems)
            {
                int dataIndex = repeatedViewItem.DataIndex;
                if (dataIndex >= baseIndex)
                    dataIndex += offsetValue;
                int unwrappedIndex = ListUtility.GetUnwrappedIndex(dataIndex, repeatedViewItem.Generation, updatedItemCount);
                repeatedViewItem.SetIndex(unwrappedIndex, dataIndex);
            }
        }

        private void ShiftPendingRequestIndices(int shiftBaseIndex, int adjustmentValue)
        {
            if (_pendingIndexRequest.HasValue)
            {
                if (adjustmentValue < 0 && _pendingIndexRequest.Value == shiftBaseIndex)
                    _pendingIndexRequest = new int?();
                else if (_pendingIndexRequest.Value >= shiftBaseIndex)
                    _pendingIndexRequest = new int?(_pendingIndexRequest.Value + adjustmentValue);
            }
            if (LayoutRequestedIndices == null)
                return;
            if (adjustmentValue < 0)
                LayoutRequestedIndices.Remove(shiftBaseIndex);
            ShiftIndices(LayoutRequestedIndices, shiftBaseIndex, adjustmentValue);
        }

        private void ShiftIndices(Vector<int> indices, int shiftBaseIndex, int adjustmentValue)
        {
            for (int index1 = 0; index1 < indices.Count; ++index1)
            {
                int index2 = indices[index1];
                if (index2 >= shiftBaseIndex)
                {
                    int num = index2 + adjustmentValue;
                    indices[index1] = num;
                }
            }
        }

        private Vector<Repeater.RepeatedViewItemSet> GetAllRepeatedInstances(int dataIndex)
        {
            Vector<Repeater.RepeatedViewItemSet> vector = null;
            foreach (Repeater.RepeatedViewItemSet repeatedViewItem in _repeatedViewItems)
            {
                if (repeatedViewItem.DataIndex == dataIndex)
                {
                    if (vector == null)
                        vector = new Vector<Repeater.RepeatedViewItemSet>();
                    vector.Add(repeatedViewItem);
                }
            }
            return vector;
        }

        private Repeater.RepeatedViewItemSet GetRepeatedInstance(int virtualIndex)
        {
            if (_repeatedViewItems != null)
            {
                foreach (Repeater.RepeatedViewItemSet repeatedViewItem in _repeatedViewItems)
                {
                    if (repeatedViewItem.VirtualIndex == virtualIndex)
                        return repeatedViewItem;
                }
            }
            return null;
        }

        private int GetIndexOfClosestRepeatedItem(int virtualIndex)
        {
            int num = 0;
            for (int index = _repeatedViewItems.Count - 1; index >= 0; --index)
            {
                if (_repeatedViewItems[index].VirtualIndex < virtualIndex)
                {
                    num = index + 1;
                    break;
                }
            }
            return num;
        }

        private bool MoveRepeatedViewItemSet(int dataOldIndex, int dataNewIndex)
        {
            Map<int, Repeater.RepeatedAndIndex> map1 = new Map<int, Repeater.RepeatedAndIndex>();
            Map<int, Repeater.RepeatedAndIndex> map2 = new Map<int, Repeater.RepeatedAndIndex>();
            for (int index = 0; index < _repeatedViewItems.Count; ++index)
            {
                Repeater.RepeatedViewItemSet repeatedViewItem = _repeatedViewItems[index];
                if (repeatedViewItem.DataIndex == dataOldIndex)
                    map1[repeatedViewItem.Generation] = new Repeater.RepeatedAndIndex(repeatedViewItem, index);
                else if (repeatedViewItem.DataIndex == dataNewIndex)
                    map2[repeatedViewItem.Generation] = new Repeater.RepeatedAndIndex(repeatedViewItem, index);
            }
            Vector<Repeater.RepeatedViewItemSet> vector = new Vector<Repeater.RepeatedViewItemSet>();
            bool flag = true;
            foreach (int key in map1.Keys)
            {
                Repeater.RepeatedAndIndex repeatedAndIndex1 = map1[key];
                if (!map2.ContainsKey(key))
                {
                    flag = false;
                }
                else
                {
                    Repeater.RepeatedAndIndex repeatedAndIndex2 = map2[key];
                    if (!MoveRepeatedViewItemSetWorker(repeatedAndIndex1.repeated, repeatedAndIndex2.repeated, dataOldIndex, dataNewIndex))
                    {
                        flag = false;
                    }
                    else
                    {
                        _repeatedViewItems.Remove(repeatedAndIndex1.repeated);
                        vector.Add(repeatedAndIndex1.repeated);
                        int unwrappedIndex = ListUtility.GetUnwrappedIndex(dataNewIndex, key, InternalCount);
                        repeatedAndIndex1.repeated.SetIndex(unwrappedIndex, dataNewIndex);
                    }
                }
            }
            foreach (int key in map2.Keys)
            {
                if (!map1.ContainsKey(key))
                {
                    Repeater.RepeatedAndIndex repeatedAndIndex = map2[key];
                    flag = false;
                }
            }
            ShiftRepeatedItemDataIndices(dataOldIndex + 1, -1, InternalCount);
            ShiftRepeatedItemDataIndices(dataNewIndex, 1, InternalCount);
            if (vector.Count > 0)
            {
                int index1 = 0;
                Repeater.RepeatedViewItemSet repeatedViewItemSet = vector[index1];
                for (int index2 = 0; index2 < _repeatedViewItems.Count; ++index2)
                {
                    if (_repeatedViewItems[index2].VirtualIndex > repeatedViewItemSet.VirtualIndex)
                    {
                        _repeatedViewItems.Insert(index2, repeatedViewItemSet);
                        ++index1;
                        if (index1 < vector.Count)
                            repeatedViewItemSet = vector[index1];
                        else
                            break;
                    }
                }
                for (; index1 < vector.Count; ++index1)
                    _repeatedViewItems.Add(vector[index1]);
            }
            if (!flag)
                return false;
            MarkLayoutInvalid();
            return true;
        }

        private bool MoveRepeatedViewItemSetWorker(
          Repeater.RepeatedViewItemSet item,
          Repeater.RepeatedViewItemSet itemFinal,
          int dataOldIndex,
          int dataNewIndex)
        {
            Microsoft.Iris.Library.TreeNode.LinkType lt = LinkType.Before;
            if (dataNewIndex > dataOldIndex)
                lt = LinkType.Behind;
            ViewItem viewItem = itemFinal.Repeated;
            if (itemFinal.Divider != null && lt == LinkType.Before)
                viewItem = itemFinal.Divider;
            item.Repeated.MoveNode(viewItem, lt);
            if (DividerName != null)
            {
                ViewItem divider = item.Divider;
                ViewItem repeated = item.Repeated;
                if (divider == null)
                {
                    Repeater.RepeatedViewItemSet repeatedViewItem = _repeatedViewItems[GetIndexOfClosestRepeatedItem(item.VirtualIndex)];
                    if (repeatedViewItem == null)
                        return false;
                    divider = repeatedViewItem.Divider;
                    item.Divider = repeatedViewItem.Divider;
                    repeatedViewItem.Divider = null;
                }
                if (itemFinal.Divider == null)
                {
                    repeated = itemFinal.Repeated;
                    itemFinal.Divider = item.Divider;
                    item.Divider = null;
                }
                divider?.MoveNode(repeated, LinkType.Before);
            }
            return true;
        }

        protected override ViewItemID IDForChild(ViewItem childItem) => new ViewItemID((childItem.GetLayoutInput(IndexLayoutInput.Data) as IndexLayoutInput).Index.Value, c_childIDSentinel);

        protected override FindChildResult ChildForID(
          ViewItemID part,
          out ViewItem resultItem)
        {
            resultItem = null;
            FindChildResult findChildResult = FindChildResult.Failure;
            if (part.IDValid && part.StringPartValid && part.StringPart == c_childIDSentinel)
            {
                resultItem = GetRepeatedItemForVirtualIndex(part.ID);
                findChildResult = resultItem == null || !resultItem.HasVisual ? FindChildResult.PotentiallyFaultIn : FindChildResult.Success;
            }
            return findChildResult;
        }

        internal int DefaultFocusIndex
        {
            get => _defaultFocusIndex;
            set
            {
                if (_defaultFocusIndex == value)
                    return;
                ViewItem itemForVirtualIndex1 = GetRepeatedItemForVirtualIndex(_defaultFocusIndex);
                if (itemForVirtualIndex1 != null && itemForVirtualIndex1.FocusOrder == 2147483646)
                    itemForVirtualIndex1.FocusOrder = int.MaxValue;
                _defaultFocusIndex = value;
                ViewItem itemForVirtualIndex2 = GetRepeatedItemForVirtualIndex(_defaultFocusIndex);
                if (itemForVirtualIndex2 != null)
                    SetAsDefaultFocusRecipient(itemForVirtualIndex2);
                FireNotification(NotificationID.DefaultFocusIndex);
            }
        }

        internal void ScrollIndexIntoView(int index)
        {
            ViewItem itemForVirtualIndex = GetRepeatedItemForVirtualIndex(index);
            if (itemForVirtualIndex != null)
                itemForVirtualIndex.ScrollIntoView();
            else
                FaultInChild(index, PendingIndexRequestType.ScrollIndexIntoView, s_scrollIndexIntoViewHandler);
        }

        private static void ScrollIndexIntoViewItemFaultedIn(ViewItem repeater, ViewItem faultedItem)
        {
            if (faultedItem == null || faultedItem.IsDisposed)
                return;
            faultedItem.ScrollIntoView();
        }

        private void SetAsDefaultFocusRecipient(ViewItem childItem) => childItem.FocusOrder = 2147483646;

        internal void NavigateIntoIndex(int index) => NavigateIntoIndex(index, true);

        internal void NavigateIntoIndex(int index, bool allowFaultIn)
        {
            ViewItem itemForVirtualIndex = GetRepeatedItemForVirtualIndex(index);
            if (itemForVirtualIndex != null && itemForVirtualIndex.HasVisual)
            {
                itemForVirtualIndex.NavigateInto();
            }
            else
            {
                if (!allowFaultIn)
                    return;
                FaultInChild(index, PendingIndexRequestType.NavigateIntoIndex, s_navigateIntoIndexHandler);
            }
        }

        private static void NavigateIntoIndexItemFaultedIn(ViewItem repeater, ViewItem faultedItem)
        {
            if (faultedItem == null || faultedItem.IsDisposed)
                return;
            faultedItem.NavigateInto();
        }

        internal override void FaultInChild(ViewItemID child, ChildFaultedInDelegate handler) => FaultInChild(child.ID, PendingIndexRequestType.FaultInChild, handler);

        private void FaultInChild(
          int virtualIndex,
          Repeater.PendingIndexRequestType type,
          ChildFaultedInDelegate handler)
        {
            if (_pendingIndexRequest.HasValue && type < _pendingIndexRequestType)
                return;
            _pendingIndexRequest = new int?(virtualIndex);
            _pendingIndexRequestType = type;
            RequestRepeatOfIndexUpdate();
            DeferredCall.Post(DispatchPriority.LayoutSync, new Repeater.FaultInChildThunk(this, virtualIndex, handler).Thunk);
        }

        internal ViewItem GetRepeatedItemForVirtualIndex(int index)
        {
            ViewItem viewItem = null;
            Repeater.RepeatedViewItemSet repeatedInstance = GetRepeatedInstance(index);
            if (repeatedInstance != null)
                viewItem = repeatedInstance.Repeated;
            return viewItem;
        }

        [Conditional("DEBUG")]
        private void DEBUG_DumpRepeatedItems(string st, byte level)
        {
            if (!Debug.Trace.IsCategoryEnabled(TraceCategory.Repeating, level))
                return;
            for (int index = 0; index < _repeatedViewItems.Count; ++index)
            {
                Repeater.RepeatedViewItemSet repeatedViewItem = _repeatedViewItems[index];
            }
            if (!Debug.Trace.IsCategoryEnabled(TraceCategory.Repeating, (byte)(level + 1U)))
                return;
            foreach (ViewItem child in Children)
                ;
        }

        [Conditional("DEBUG")]
        private void DEBUG_DumpRepeatedItem(Repeater.RepeatedViewItemSet item, byte level)
        {
            if (!Debug.Trace.IsCategoryEnabled(TraceCategory.Repeating, level) || !ListUtility.IsValidIndex(_source, item.DataIndex))
                return;
            _source[item.DataIndex]?.ToString();
        }

        [Conditional("DEBUG")]
        private void DEBUG_DumpRepeatedViewItem(ViewItem vi, byte level)
        {
        }

        [Conditional("DEBUG")]
        private void DEBUG_ValidateSortedByVirtualIndex(Vector<Repeater.RepeatedViewItemSet> list)
        {
            if (list == null)
                return;
            int num = int.MinValue;
            for (int index = 0; index < list.Count; ++index)
                num = list[index].VirtualIndex;
        }

        public static object UnavailableItem => s_unavailableObject;

        public delegate void ContentTypeHandler(object repeatObject, ref string contentName);

        internal class RepeatedViewItemSet
        {
            private Index _index;
            private int _generationValue;
            private ViewItem _repeatedItem;
            private ViewItem _dividerItem;

            public RepeatedViewItemSet(
              Index index,
              int generationValue,
              ViewItem repeatedItem,
              ViewItem dividerItem)
            {
                _index = index;
                _generationValue = generationValue;
                _repeatedItem = repeatedItem;
                _dividerItem = dividerItem;
            }

            public int DataIndex => _index.SourceValue;

            public int VirtualIndex => _index.Value;

            public int Generation => _generationValue;

            public ViewItem Repeated => _repeatedItem;

            public ViewItem Divider
            {
                get => _dividerItem;
                set => _dividerItem = value;
            }

            public void SetIndex(int virtualIndex, int dataIndex) => _index.SetValue(virtualIndex, dataIndex);

            public void DisposeViewItems()
            {
                DisposeViewItem(_repeatedItem);
                if (_dividerItem == null)
                    return;
                DisposeViewItem(_dividerItem);
            }

            private void DisposeViewItem(ViewItem vi)
            {
                UIClass ui = vi.UI;
                if (vi.HasVisual)
                    ui.DestroyVisualTree(vi);
                ui.DisposeViewItemTree(vi);
            }

            public override string ToString() => ToString(string.Empty);

            public string ToString(string dataName)
            {
                string str = null;
                if (_dividerItem != null)
                    str = InvariantString.Format(", Divider({0})", _dividerItem.GetType().Name);
                return InvariantString.Format("[{0},{1}] {2} ({3} {4})", VirtualIndex, DataIndex, dataName, _repeatedItem.GetType().Name, str);
            }
        }

        internal struct RepeatedAndIndex
        {
            public Repeater.RepeatedViewItemSet repeated;
            public int index;

            public RepeatedAndIndex(Repeater.RepeatedViewItemSet repeated, int index)
            {
                this.repeated = repeated;
                this.index = index;
            }
        }

        private class FaultInChildThunk
        {
            private Repeater _repeater;
            private int _virtualIndex;
            private ChildFaultedInDelegate _faultInHandler;

            public FaultInChildThunk(
              Repeater repeater,
              int virtualIndex,
              ChildFaultedInDelegate faultInHandler)
            {
                _repeater = repeater;
                _virtualIndex = virtualIndex;
                _faultInHandler = faultInHandler;
            }

            public SimpleCallback Thunk => new SimpleCallback(CallFaultInHandler);

            private void CallFaultInHandler()
            {
                ViewItem childItem = null;
                if (!_repeater.IsDisposed)
                    childItem = _repeater.GetRepeatedItemForVirtualIndex(_virtualIndex);
                _faultInHandler(_repeater, childItem);
            }
        }

        private struct FocusRepairArgs
        {
            public RectangleF focusBounds;
            public bool focusIsDefault;

            public FocusRepairArgs(RectangleF focusBounds, bool focusIsDefault)
            {
                this.focusBounds = focusBounds;
                this.focusIsDefault = focusIsDefault;
            }
        }

        private enum PendingIndexRequestType
        {
            ScrollIndexIntoView,
            FaultInChild,
            NavigateIntoIndex,
        }
    }
}
