// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.UI.UIZone
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Debug;
using Microsoft.Iris.Input;
using Microsoft.Iris.Layout;
using Microsoft.Iris.Layouts;
using Microsoft.Iris.Library;
using Microsoft.Iris.Render;
using Microsoft.Iris.RenderAPI.Drawing;
using Microsoft.Iris.Session;
using Microsoft.Iris.ViewItems;
using System;

namespace Microsoft.Iris.UI
{
    internal class UIZone : DisposableObject
    {
        private SessionInputHandler sessionInputEvent;
        private UISession _parentSession;
        private UIForm _form;
        private uint _tasksRequestedValue;
        private bool _physicalVisible;
        private Size _containerSize;
        private Vector3 _scale;
        private Microsoft.Iris.UI.RootUI _rootUI;
        private RootViewItem _rootViewItem;
        private bool _needScaleNotificationsFlag;
        private bool _needFullyEnabledNotificationsFlag;
        private UIClass[] _mouseFocusRouteList;
        private UIClass[] _keyFocusRouteList;
        private UIZone.InputDeliveryData _cachedInputDeliveryData;
        private UIClass[][] _cachedUIClassStorage;
        private int _cachedUIClassStorageIndex;
        private bool _previousZonePhysicalVisibleFlag;
        private int _uncommittedLayouts;

        public UIZone(UIForm form)
        {
            _parentSession = form.Session;
            _form = form;
            _scale = Vector3.UnitVector;
            _rootUI = new Microsoft.Iris.UI.RootUI(this);
            _rootViewItem = new RootViewItem(this, _rootUI, form);
            _rootUI.SetRootItem(_rootViewItem);
            _cachedInputDeliveryData = new UIZone.InputDeliveryData();
            _cachedUIClassStorage = new UIClass[4][];
            _cachedUIClassStorageIndex = _cachedUIClassStorage.Length - 1;
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            _rootUI.Dispose(this);
            _rootUI = null;
        }

        public UISession Session => _parentSession;

        public UIForm Form => _form;

        public RootViewItem RootViewItem => _rootViewItem;

        public UIClass RootUI => _rootUI;

        public bool ZonePhysicalVisible => _physicalVisible;

        public Size RootContainerSize => _containerSize;

        public Vector3 HostDisplayScale => _scale;

        public bool InfiniteLayoutLoopDetected => _uncommittedLayouts > 153;

        public ICookedInputSite MapInput(
          IRawInputSite rawSource,
          ICookedInputSite targetRelative)
        {
            UIClass uiClass = null;
            if (targetRelative != null)
                uiClass = targetRelative as UIClass;
            else if (rawSource != null && rawSource.OwnerData is ViewItem ownerData)
            {
                UIClass ui = ownerData.UI;
                if (ui != null)
                    uiClass = ui;
            }
            if (uiClass != null && !uiClass.IsMouseFocusable())
                uiClass = null;
            return uiClass;
        }

        public object PrepareInputForDelivery(
          ITreeNode endpoint,
          ICookedInputSite finalTarget,
          InputInfo info)
        {
            UIZone.InputDeliveryData inputDeliveryData = GetInputDeliveryData();
            inputDeliveryData.target = finalTarget as UIClass;
            inputDeliveryData.sourceInputInfo = info;
            inputDeliveryData.eventRoute = ComputeEventRoute((UIClass)endpoint, out inputDeliveryData.routingLength);
            if (inputDeliveryData.eventRoute != null)
                --inputDeliveryData.routingLength;
            return inputDeliveryData;
        }

        public void UpdateInputFocusStates(
          InputDeviceType focusType,
          bool deepFocusFlag,
          ITreeNode directFocusChild,
          object param)
        {
            switch (focusType)
            {
                case InputDeviceType.Keyboard:
                    DoDeepFocusUpdates(ref _keyFocusRouteList, deepFocusFlag, directFocusChild, param, UIClass.GetFocusUpdateProc(InputDeviceType.Keyboard));
                    break;
                case InputDeviceType.Mouse:
                    DoDeepFocusUpdates(ref _mouseFocusRouteList, deepFocusFlag, directFocusChild, param, UIClass.GetFocusUpdateProc(InputDeviceType.Mouse));
                    break;
            }
        }

        public InputDeliveryStatus DeliverInput(object param, EventRouteStages stage)
        {
            UIZone.InputDeliveryData data = (UIZone.InputDeliveryData)param;
            bool routeTruncated = data.routeTruncated;
            switch (stage)
            {
                case EventRouteStages.Direct:
                    DeliverInputDirectWorker(data, false);
                    break;
                case EventRouteStages.Bubbled:
                    DeliverInputIndirectWorker(data, false);
                    break;
                case EventRouteStages.Routed:
                    DeliverInputIndirectWorker(data, true);
                    break;
                case EventRouteStages.Unhandled:
                    DeliverInputDirectWorker(data, true);
                    break;
            }
            return data.routeTruncated && !routeTruncated ? InputDeliveryStatus.Truncated : InputDeliveryStatus.Normal;
        }

        public void UpdateCursor(UIClass changedUI)
        {
            if (_form == null)
                return;
            UIClass uiClass = Session.InputManager.Queue.CurrentMouseFocus as UIClass;
            if (changedUI != null && !IsChildADescendant(changedUI, uiClass))
                return;
            CursorID cursorId = CursorID.NotSpecified;
            if (uiClass != null && uiClass.Zone == this)
            {
                do
                {
                    cursorId = uiClass.EffectiveCursor;
                    uiClass = uiClass.Parent;
                }
                while (cursorId == CursorID.NotSpecified && uiClass != null);
            }
            if (cursorId == CursorID.NotSpecified)
                cursorId = CursorID.Arrow;
            _form.Cursor = cursorId;
        }

        public bool IsChildADescendant(ITreeNode potentialParent, ITreeNode potentialChild) => potentialParent is Microsoft.Iris.Library.TreeNode treeNode && treeNode.HasDescendant(potentialChild as Microsoft.Iris.Library.TreeNode);

        public bool IsChildKeyFocusable(ITreeNode child) => ((UIClass)child).IsKeyFocusable();

        public bool IsChildRooted(ITreeNode child) => IsChildRooted((Microsoft.Iris.Library.TreeNode)child);

        public bool IsChildRooted(Microsoft.Iris.Library.TreeNode child) => child.Zone == this;

        public int RootAccessibilityID => 0;

        protected void ResendPaintedContent() => _rootViewItem?.ResendExistingContentTree();

        protected void OnContainerChange()
        {
            RootViewItem rootViewItem = _rootViewItem;
            if (rootViewItem == null || rootViewItem.IsDisposed)
                return;
            rootViewItem.MarkLayoutInvalid();
            bool zonePhysicalVisible = ZonePhysicalVisible;
            if (zonePhysicalVisible)
                return;
            rootViewItem.ApplyLayoutOutputs(true);
            _previousZonePhysicalVisibleFlag = zonePhysicalVisible;
        }

        protected void OnHostDisplayScaleChange() => _rootViewItem?.NotifyEffectiveScaleChange(true);

        internal void ScheduleScaleChangeNotifications()
        {
            if (_needScaleNotificationsFlag)
                return;
            ScheduleUiTask(UiTask.Initialization);
            _needScaleNotificationsFlag = true;
        }

        internal void ScheduleFullyEnabledChangeNotifications()
        {
            if (_needFullyEnabledNotificationsFlag)
                return;
            ScheduleUiTask(UiTask.Initialization);
            _needFullyEnabledNotificationsFlag = true;
        }

        public void OnRenderDeviceReset() => ResendPaintedContent();

        public void SetPhysicalVisible(bool visible)
        {
            if (_physicalVisible == visible)
                return;
            _physicalVisible = visible;
            OnContainerChange();
        }

        public void ResizeRootContainer(Size size)
        {
            if (!(_containerSize != size))
                return;
            _containerSize = size;
            OnContainerChange();
        }

        public void SetHostDisplayScale(Vector3 scale)
        {
            if (!(_scale != scale))
                return;
            _scale = scale;
            OnHostDisplayScaleChange();
        }

        public bool OnInboundKeyNavigation(
          Direction searchDirection,
          RectangleF startRectangleF,
          bool defaultFlag)
        {
            UIClass rootUi = _rootUI;
            return rootUi != null && rootUi.InboundKeyNavigation(searchDirection, startRectangleF, defaultFlag);
        }

        public event SessionInputHandler SessionInput
        {
            add
            {
                if (sessionInputEvent == null)
                    Session.InputManager.PreviewInput += new InputNotificationHandler(OnSessionInput);
                sessionInputEvent += value;
            }
            remove
            {
                sessionInputEvent -= value;
                if (sessionInputEvent != null)
                    return;
                Session.InputManager.PreviewInput -= new InputNotificationHandler(OnSessionInput);
            }
        }

        private void OnSessionInput(object sender, InputNotificationEventArgs args) => sessionInputEvent(args.InputInfo, args.HandledStage);

        protected void ImplementUiTask(UiTask task, object param)
        {
            switch (task)
            {
                case UiTask.Initialization:
                    DeliverInitializations();
                    break;
                case UiTask.LayoutComputation:
                    ILayoutNode rootViewItem1 = _rootViewItem;
                    if (rootViewItem1 == null)
                        break;
                    ScrollingLayout.ResetScrollFocusIntoView();
                    if (ZonePhysicalVisible)
                    {
                        Size rootContainerSize = RootContainerSize;
                        rootViewItem1.Measure(rootContainerSize);
                        rootViewItem1.Arrange(new LayoutSlot(rootContainerSize));
                        ++_uncommittedLayouts;
                        ScheduleUiTask(UiTask.Painting);
                    }
                    else
                        rootViewItem1.MarkHidden();
                    rootViewItem1.Commit();
                    _rootViewItem.ResetLayoutInvalid();
                    break;
                case UiTask.LayoutApplication:
                    ViewItem rootViewItem2 = _rootViewItem;
                    if (rootViewItem2 == null)
                        break;
                    bool zonePhysicalVisible = ZonePhysicalVisible;
                    rootViewItem2.ApplyLayoutOutputs(_previousZonePhysicalVisibleFlag != zonePhysicalVisible);
                    _previousZonePhysicalVisibleFlag = zonePhysicalVisible;
                    break;
                case UiTask.Painting:
                    if (_rootViewItem != null)
                    {
                        _rootViewItem.UISession.Dispatcher.RequestBatchFlush();
                        _rootViewItem.PaintTree(true);
                    }
                    _uncommittedLayouts = 0;
                    break;
            }
        }

        private void DeliverInputIndirectWorker(UIZone.InputDeliveryData data, bool downFlag)
        {
            UIClass[] eventRoute = data.eventRoute;
            InputInfo sourceInputInfo = data.sourceInputInfo;
            byte traceLevelForEvent = Trace.GetTraceLevelForEvent(sourceInputInfo);
            if (eventRoute == null)
                return;
            if (downFlag)
            {
                for (int index = 0; index < data.routingLength; ++index)
                {
                    UIClass target = eventRoute[index];
                    bool handled = sourceInputInfo.Handled;
                    target.DeliverInput(sourceInputInfo, EventRouteStages.Routed);
                    if (sourceInputInfo.Handled != handled)
                        TraceHandled(traceLevelForEvent, sourceInputInfo, target);
                    if (sourceInputInfo.RouteTruncated && !data.routeTruncated)
                    {
                        data.routeTruncated = true;
                        data.routingLength = index + 1;
                        break;
                    }
                }
            }
            else
            {
                for (int index = data.routingLength - 1; index >= 0; --index)
                {
                    UIClass target = eventRoute[index];
                    bool handled = sourceInputInfo.Handled;
                    target.DeliverInput(sourceInputInfo, EventRouteStages.Bubbled);
                    if (sourceInputInfo.Handled != handled)
                        TraceHandled(traceLevelForEvent, sourceInputInfo, target);
                    if (sourceInputInfo.RouteTruncated && !data.routeTruncated)
                    {
                        data.routeTruncated = true;
                        data.routingLength = index + 1;
                    }
                }
            }
        }

        private void DeliverInputDirectWorker(UIZone.InputDeliveryData data, bool lastChance)
        {
            if (data.routeTruncated)
                return;
            UIClass target = data.target;
            InputInfo sourceInputInfo = data.sourceInputInfo;
            byte traceLevelForEvent = Trace.GetTraceLevelForEvent(sourceInputInfo);
            EventRouteStages stage = EventRouteStages.Direct;
            if (lastChance)
                stage = EventRouteStages.Unhandled;
            bool handled = sourceInputInfo.Handled;
            target.DeliverInput(sourceInputInfo, stage);
            if (sourceInputInfo.Handled == handled)
                return;
            TraceHandled(traceLevelForEvent, sourceInputInfo, target);
        }

        private void TraceHandled(byte traceLevel, InputInfo info, UIClass target)
        {
        }

        private UIZone.InputDeliveryData GetInputDeliveryData()
        {
            UIZone.InputDeliveryData inputDeliveryData = _cachedInputDeliveryData;
            _cachedInputDeliveryData = null;
            if (inputDeliveryData == null)
                inputDeliveryData = new UIZone.InputDeliveryData();
            return inputDeliveryData;
        }

        public void RecycleInputDeliveryData(object param)
        {
            if (param == null)
                return;
            UIZone.InputDeliveryData inputDeliveryData = (UIZone.InputDeliveryData)param;
            inputDeliveryData.target = null;
            inputDeliveryData.sourceInputInfo = null;
            if (!inputDeliveryData.eventRouteCached)
                RecycleUIClassArray(inputDeliveryData.eventRoute);
            else
                inputDeliveryData.eventRouteCached = false;
            inputDeliveryData.eventRoute = null;
            inputDeliveryData.routingLength = 0;
            inputDeliveryData.routeTruncated = false;
            _cachedInputDeliveryData = inputDeliveryData;
        }

        private UIClass[] GetUIClassArray(int requiredLength)
        {
            UIClass[] uiClassArray = null;
            if (_cachedUIClassStorageIndex >= 0)
            {
                uiClassArray = _cachedUIClassStorage[_cachedUIClassStorageIndex];
                _cachedUIClassStorage[_cachedUIClassStorageIndex] = null;
                --_cachedUIClassStorageIndex;
            }
            if (uiClassArray == null || uiClassArray.Length < requiredLength)
                uiClassArray = new UIClass[requiredLength];
            return uiClassArray;
        }

        private void RecycleUIClassArray(UIClass[] storage)
        {
            if (storage == null || _cachedUIClassStorageIndex >= _cachedUIClassStorage.Length - 1)
                return;
            ++_cachedUIClassStorageIndex;
            Array.Clear(storage, 0, storage.Length);
            _cachedUIClassStorage[_cachedUIClassStorageIndex] = storage;
        }

        private UIClass[] ComputeEventRoute(UIClass endpoint, out int entriesCount)
        {
            entriesCount = 0;
            if (endpoint == null)
                return null;
            for (UIClass uiClass = endpoint; uiClass != null; uiClass = uiClass.Parent)
                ++entriesCount;
            UIClass[] uiClassArray = GetUIClassArray(entriesCount);
            UIClass uiClass1 = endpoint;
            int num = entriesCount;
            for (; uiClass1 != null; uiClass1 = uiClass1.Parent)
                uiClassArray[--num] = uiClass1;
            return uiClassArray;
        }

        private void DoDeepFocusUpdates(
          ref UIClass[] refCurrentFocusRouteList,
          bool deepFocusFlag,
          ITreeNode directFocusChild,
          object param,
          FocusStateHandler updateProc)
        {
            if (updateProc == null)
                return;
            UIClass[] uiClassArray1 = refCurrentFocusRouteList;
            UIClass[] uiClassArray2 = null;
            UIZone.InputDeliveryData inputDeliveryData = (UIZone.InputDeliveryData)param;
            if (inputDeliveryData != null)
                uiClassArray2 = inputDeliveryData.eventRoute;
            if (uiClassArray1 == null && uiClassArray2 == null)
                return;
            refCurrentFocusRouteList = uiClassArray2;
            if (inputDeliveryData != null)
                inputDeliveryData.eventRouteCached = true;
            UIClass[] removedFromRoute = FindControlsRemovedFromRoute(uiClassArray1, uiClassArray2);
            if (removedFromRoute != null)
                UpdateControlFocusStates(removedFromRoute, false, null, updateProc);
            RecycleUIClassArray(uiClassArray1);
            if (removedFromRoute != uiClassArray1)
                RecycleUIClassArray(removedFromRoute);
            if (uiClassArray2 == null)
                return;
            UpdateControlFocusStates(uiClassArray2, true, directFocusChild, updateProc);
        }

        private UIClass[] FindControlsRemovedFromRoute(
          UIClass[] oldRouteList,
          UIClass[] newRouteList)
        {
            UIClass[] uiClassArray = null;
            if (oldRouteList != null)
            {
                if (newRouteList != null && newRouteList.Length > 0)
                {
                    bool flag = false;
                    if (oldRouteList.Length == newRouteList.Length)
                    {
                        flag = true;
                        for (int index = 0; index < newRouteList.Length; ++index)
                        {
                            if (oldRouteList[index] != newRouteList[index])
                            {
                                flag = false;
                                break;
                            }
                        }
                    }
                    if (!flag)
                    {
                        int num = 0;
                        for (int index = 0; index < oldRouteList.Length; ++index)
                        {
                            UIClass oldRoute = oldRouteList[index];
                            if (Array.IndexOf<UIClass>(newRouteList, oldRoute) < 0)
                            {
                                if (uiClassArray == null)
                                    uiClassArray = GetUIClassArray(oldRouteList.Length - index);
                                uiClassArray[num++] = oldRoute;
                            }
                        }
                    }
                }
                else
                    uiClassArray = oldRouteList;
            }
            return uiClassArray;
        }

        private static void UpdateControlFocusStates(
          UIClass[] focusRouteList,
          bool deepFocusFlag,
          ITreeNode actualFocus,
          FocusStateHandler updateProc)
        {
            UIClass uiClass = actualFocus as UIClass;
            for (int index = 0; index < focusRouteList.Length; ++index)
            {
                UIClass focusRoute = focusRouteList[index];
                if (focusRoute == null)
                    break;
                if (focusRoute.IsValid)
                {
                    bool directFocusFlag = false;
                    if (deepFocusFlag)
                        directFocusFlag = focusRoute == uiClass;
                    updateProc(focusRoute, deepFocusFlag, directFocusFlag);
                }
            }
        }

        private void DeliverInitializations()
        {
            while (true)
            {
                while (_needFullyEnabledNotificationsFlag)
                {
                    _needFullyEnabledNotificationsFlag = false;
                    UIClass rootUi = _rootUI;
                    if (rootUi != null)
                        rootUi.DeliverFullyEnabled(true);
                    else
                        break;
                }
                if (_needScaleNotificationsFlag)
                {
                    _needScaleNotificationsFlag = false;
                    ViewItem rootViewItem = _rootViewItem;
                    if (rootViewItem != null)
                        rootViewItem.DeliverEffectiveScaleChange(false);
                    else
                        goto label_7;
                }
                else
                    break;
            }
            return;
        label_7:;
        }

        public void ScheduleUiTask(UiTask task)
        {
            uint num = (uint)(task & (UiTask)~(int)_tasksRequestedValue);
            if (num == 0U)
                return;
            _tasksRequestedValue |= num;
            _parentSession.ScheduleUiTask(task);
        }

        public void ProcessUiTask(UiTask task, object param)
        {
            uint num = (uint)task;
            if (((int)_tasksRequestedValue & (int)num) == 0)
                return;
            _tasksRequestedValue &= ~num;
            ImplementUiTask(task, param);
        }

        public override string ToString() => GetType().Name + "[" + _form + "]";

        private class InputDeliveryData
        {
            public UIClass target;
            public InputInfo sourceInputInfo;
            public UIClass[] eventRoute;
            public int routingLength;
            public bool routeTruncated;
            public bool eventRouteCached;
        }
    }
}
