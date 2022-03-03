// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Input.InputManager
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Debug;
using Microsoft.Iris.Library;
using Microsoft.Iris.OS;
using Microsoft.Iris.Queues;
using Microsoft.Iris.Render;
using Microsoft.Iris.Session;
using Microsoft.Iris.UI;
using System;

namespace Microsoft.Iris.Input
{
    internal class InputManager : IRawInputCallbacks
    {
        private InputModifiers _HACK_sysModifiers;
        private UISession _session;
        private InputQueue _inputQueue;
        private int _inputSuspendCount;
        private readonly SimpleCallback _resumeInput;
        private KeyboardDevice _keyboardDevice;
        private MouseDevice _mouseDevice;
        private HidDevice _remoteDevice;
        private DateTime _lastInputTime;
        private bool _keyFocusCanBeNull;
        private bool _ignoreHungKeyFocus;
        private ICookedInputSite _ignoreHungKeyFocusTarget;
        private bool _inputDisabled;
        private KeyCoalesceFilter _keyCoalescePolicy;
        private KeyStateInfo _currentCoalesceKeyEvent;
        private bool _currentCoalesceUndelivered;
        private Point _physicalMouseOffset;
        private bool _refreshHitTargetPending;
        private readonly SimpleCallback _refreshHitTargetHandler;
        private UIZone _mouseFocusZone;
        private UIZone _keyFocusZone;
        private static readonly DeferredHandler s_deliverCoalescedKey = new DeferredHandler(DeliverCoalescedKey);

        internal InputManager(UISession session)
        {
            _session = session;
            _keyFocusCanBeNull = true;
            _lastInputTime = DateTime.MinValue;
            _inputQueue = new InputQueue(this);
            _resumeInput = new SimpleCallback(ResumeInput);
            _refreshHitTargetHandler = new SimpleCallback(RefreshHitTargetHandler);
        }

        internal void ConnectToRenderer() => Session.RenderSession.InputSystem.RegisterRawInputCallbacks(this);

        internal void PrepareToShutDown()
        {
            KeyCoalescePolicy = null;
            EndKeyCoalesce();
            InvalidKeyFocus = null;
            _inputDisabled = true;
            Session.RenderSession.InputSystem.UnregisterRawInputCallbacks();
            _inputQueue.PrepareToShutDown();
        }

        internal UISession Session => _session;

        internal InputQueue Queue => _inputQueue;

        internal InputModifiers Modifiers => Keyboard.KeyboardModifiers | Mouse.MouseModifiers;

        internal InputModifiers DragModifiers => _inputQueue.DragModifiers;

        internal KeyboardDevice Keyboard
        {
            get
            {
                if (_keyboardDevice == null)
                    _keyboardDevice = new KeyboardDevice(this);
                return _keyboardDevice;
            }
        }

        internal MouseDevice Mouse
        {
            get
            {
                if (_mouseDevice == null)
                    _mouseDevice = new MouseDevice(this);
                return _mouseDevice;
            }
        }

        internal HidDevice Remote
        {
            get
            {
                if (_remoteDevice == null)
                    _remoteDevice = new HidDevice(this);
                return _remoteDevice;
            }
        }

        public DateTime LastInputTime => _lastInputTime;

        public bool InputEnabled => !_inputDisabled;

        public bool KeyFocusCanBeNull
        {
            get => _inputDisabled || _keyFocusCanBeNull;
            set
            {
                if (_keyFocusCanBeNull == value)
                    return;
                _keyFocusCanBeNull = value;
                if (value)
                    return;
                _inputQueue.RevalidateInputSiteUsage(null, false);
            }
        }

        public ICookedInputSite RawKeyFocus => _inputQueue.PendingKeyFocus;

        public ICookedInputSite RawInstantaneousKeyFocus => _inputQueue.InstantaneousKeyFocus;

        public bool RawKeyFocusIsDefault => _inputQueue.PendingKeyFocusIsDefault;

        public KeyCoalesceFilter KeyCoalescePolicy
        {
            get => _keyCoalescePolicy;
            set => _keyCoalescePolicy = value;
        }

        public Point MostRecentPhysicalMousePos
        {
            get => _physicalMouseOffset;
            set
            {
                if (!(_physicalMouseOffset != value))
                    return;
                _physicalMouseOffset = value;
                if (MousePositionChanged == null)
                    return;
                MousePositionChanged(this, EventArgs.Empty);
            }
        }

        public event InvalidKeyFocusHandler InvalidKeyFocus;

        public event EventHandler MousePositionChanged;

        public event InputNotificationHandler PreviewInput;

        public event InputNotificationHandler HandledInput;

        public event InputNotificationHandler UnhandledInput;

        public void SuspendInputUntil(DispatchPriority unlockPriority)
        {
            DeferredCall.Post(unlockPriority, _resumeInput);
            Session.Dispatcher.BlockInputQueue(true);
            ++_inputSuspendCount;
        }

        private void ResumeInput()
        {
            if (_inputSuspendCount <= 0)
                return;
            --_inputSuspendCount;
            if (_inputSuspendCount != 0)
                return;
            Session.Dispatcher.BlockInputQueue(false);
        }

        internal void UpdateLastInputTime() => _lastInputTime = DateTime.UtcNow;

        public InputHandlerFlags InputHandlerMask => InputHandlerFlags.All;

        public void HandleRawKeyboardInput(
          uint message,
          InputModifiers modifiers,
          ref RawKeyboardData args)
        {
            KeyInfo info = Keyboard.OnRawInput(message, modifiers, ref args);
            if (info != null)
                _inputQueue.RawKeyAction(info);
            UpdateLastInputTime();
        }

        public void HandleRawMouseInput(uint message, InputModifiers modifiers, ref RawMouseData args)
        {
            Mouse.OnRawInput(message, modifiers, ref args);
            UpdateLastInputTime();
        }

        public void HandleRawHidInput(ref RawHidData args)
        {
            KeyActionInfo keyActionInfo = Remote.OnRawInput(HIDCommandMapping.Find(args._commandCode, args._usagePage), ref args);
            if (keyActionInfo != null)
                _inputQueue.RawKeyAction(keyActionInfo);
            UpdateLastInputTime();
        }

        public void HandleAppCommand(ref RawHidData args)
        {
            KeyActionInfo keyActionInfo = Remote.OnRawInput(AppCommandMapping.Find(args._commandCode), ref args);
            if (keyActionInfo != null)
                _inputQueue.RawKeyAction(keyActionInfo);
            UpdateLastInputTime();
        }

        public void HandleRawDragInput(uint message, InputModifiers modifiers, ref RawDragData args)
        {
            using (DataObject dataObject = new DataObject(args._pDataStream))
            {
                object data = null;
                if (message == 0U)
                    data = dataObject.GetExternalData();
                Mouse.OnRawInput(message, modifiers, ref args, data);
            }
            UpdateLastInputTime();
        }

        internal void RevalidateInputSiteUsage(
          ICookedInputSite target,
          bool recursiveFlag,
          bool knownDisabledFlag)
        {
            if (_ignoreHungKeyFocus && !knownDisabledFlag)
            {
                StopIgnoringHungKeyFocus();
                target = null;
                recursiveFlag = false;
            }
            _inputQueue.RevalidateInputSiteUsage(target, recursiveFlag);
            if (_refreshHitTargetPending)
                return;
            _refreshHitTargetPending = true;
            DeferredCall.Post(DispatchPriority.RenderSync, _refreshHitTargetHandler);
        }

        private void RefreshHitTargetHandler()
        {
            _refreshHitTargetPending = false;
            Session.Form.RefreshHitTarget();
        }

        public bool IsPendingKeyFocusValid() => IsValidKeyFocusWorker(_inputQueue.PendingKeyFocus);

        public void SimulateDragEnter(
          ICookedInputSite dragSource,
          IRawInputSite rawTargetSite,
          object data,
          int x,
          int y,
          InputModifiers modifiers)
        {
            _inputQueue.SimulateDragEnter(dragSource, rawTargetSite, data, x, y, modifiers);
        }

        public void SimulateDragOver(InputModifiers modifiers) => _inputQueue.SimulateDragOver(modifiers);

        public void SimulateDragOver(
          IRawInputSite rawTargetSite,
          int x,
          int y,
          InputModifiers modifiers)
        {
            _inputQueue.SimulateDragOver(rawTargetSite, x, y, modifiers);
        }

        public void SimulateDragEnd(
          IRawInputSite rawTargetSite,
          InputModifiers modifiers,
          DragOperation formOperation)
        {
            _inputQueue.SimulateDragEnd(rawTargetSite, modifiers, formOperation);
        }

        public object GetDragDropValue() => _inputQueue.GetDragDropValue();

        internal bool FilterKeyboardEvent(KeyStateInfo info)
        {
            switch (info.Action)
            {
                case KeyAction.Up:
                    EndKeyCoalesce();
                    break;
                case KeyAction.Down:
                    if (info.RepeatCount == 2U)
                    {
                        BeginKeyCoalesce(info);
                        break;
                    }
                    if (info.RepeatCount > 2U && CoalesceRepeatedKey(info))
                        return false;
                    EndKeyCoalesce();
                    break;
            }
            return true;
        }

        private bool BeginKeyCoalesce(KeyStateInfo info)
        {
            EndKeyCoalesce();
            if (_keyCoalescePolicy == null || !_keyCoalescePolicy(info.Key))
                return false;
            SetCoalesceKeyEvent(info.MakeRepeatableCopy());
            return true;
        }

        private void SetCoalesceKeyEvent(KeyStateInfo info)
        {
            if (_currentCoalesceKeyEvent != null)
                _currentCoalesceKeyEvent.Unlock();
            _currentCoalesceKeyEvent = info;
            if (_currentCoalesceKeyEvent == null)
                return;
            _currentCoalesceKeyEvent.Lock();
        }

        private bool CoalesceRepeatedKey(KeyStateInfo info)
        {
            if (!info.IsRepeatOf(_currentCoalesceKeyEvent))
                return false;
            SetCoalesceKeyEvent(info);
            if (!_currentCoalesceUndelivered)
            {
                _currentCoalesceUndelivered = true;
                _inputQueue.RawInputIdleItem(DeferredCall.Create(s_deliverCoalescedKey, this));
            }
            return true;
        }

        private void EndKeyCoalesce()
        {
            SetCoalesceKeyEvent(null);
            _currentCoalesceUndelivered = false;
        }

        private static void DeliverCoalescedKey(object args)
        {
            InputManager inputManager = (InputManager)args;
            if (!inputManager._currentCoalesceUndelivered)
                return;
            inputManager._currentCoalesceUndelivered = false;
            ICookedInputSite instantaneousKeyFocus = inputManager._inputQueue.InstantaneousKeyFocus;
            inputManager.DeliverInputWorker(instantaneousKeyFocus, inputManager._currentCoalesceKeyEvent, EventRouteStages.All);
            inputManager.SuspendInputUntil(DispatchPriority.Idle);
        }

        public void MapMouseInput(MouseActionInfo info, ICookedInputSite captureSite)
        {
            ICookedInputSite target = HitTestInput(info.RawSource, captureSite);
            if (!IsValidCookedInputSite(target))
                target = null;
            IRawInputSite naturalHit = info.NaturalHit;
            ICookedInputSite naturalTarget = null;
            if (naturalHit != null)
                naturalTarget = naturalHit != info.RawSource ? (naturalHit is ITreeNode treeNode ? treeNode.Zone?.MapInput(naturalHit, null) : null) : target;
            info.SetMappedTargets(target, naturalTarget);
        }

        internal ICookedInputSite HitTestInput(
          IRawInputSite rawSource,
          ICookedInputSite targetRelative)
        {
            ICookedInputSite cookedInputSite = null;
            if (!_inputDisabled)
            {
                ITreeNode treeNode = null;
                if (targetRelative != null)
                    treeNode = targetRelative as ITreeNode;
                else if (rawSource != null)
                    treeNode = rawSource.OwnerData as ITreeNode;
                if (treeNode != null)
                {
                    UIZone zone = treeNode.Zone;
                    if (zone != null)
                        cookedInputSite = zone.MapInput(rawSource, targetRelative);
                }
            }
            return cookedInputSite;
        }

        internal bool IsValidCookedInputSite(ICookedInputSite target) => target == null || (target as ITreeNode).Zone != null;

        internal bool IsSiteInfluencedByPeer(ICookedInputSite target, ICookedInputSite peer)
        {
            if (!(peer is ITreeNode potentialParent))
                return false;
            UIZone zone = potentialParent.Zone;
            return zone != null && target is ITreeNode potentialChild && potentialChild.Zone == zone && zone.IsChildADescendant(potentialParent, potentialChild);
        }

        internal bool IsValidKeyFocusSite(ICookedInputSite target)
        {
            bool flag = IsValidKeyFocusWorker(target);
            if (_ignoreHungKeyFocus)
            {
                if (flag)
                    StopIgnoringHungKeyFocus();
                else if (target == _ignoreHungKeyFocusTarget)
                    flag = true;
                else
                    StopIgnoringHungKeyFocus();
            }
            return flag;
        }

        private bool IsValidKeyFocusWorker(ICookedInputSite candidate)
        {
            bool flag = KeyFocusCanBeNull;
            if (candidate is ITreeNode child)
            {
                UIZone zone = child.Zone;
                if (zone != null)
                    flag = zone.IsChildKeyFocusable(child);
            }
            return flag;
        }

        internal void RepairInvalidKeyFocus(uint attemptCount)
        {
            if (_inputQueue.PendingKeyFocus is ITreeNode pendingKeyFocus && pendingKeyFocus.Zone == null)
            {
                _inputQueue.RequestKeyFocus(null, KeyFocusReason.Default);
                if (_keyFocusCanBeNull)
                    return;
            }
            if (attemptCount <= 1U)
            {
                SuspendInputUntil(DispatchPriority.LayoutSync);
            }
            else
            {
                if (InvalidKeyFocus != null)
                    InvalidKeyFocus(_inputQueue.LastCompletedKeyFocus);
                ICookedInputSite pendingKeyFocusB = _inputQueue.PendingKeyFocus;
                if (IsValidKeyFocusWorker(pendingKeyFocusB))
                    return;
                if (_keyFocusCanBeNull)
                {
                    _inputQueue.RequestKeyFocus(null, KeyFocusReason.Default);
                }
                else
                {
                    _ignoreHungKeyFocus = true;
                    _ignoreHungKeyFocusTarget = pendingKeyFocusB;
                }
            }
        }

        private void StopIgnoringHungKeyFocus()
        {
            _ignoreHungKeyFocus = false;
            _ignoreHungKeyFocusTarget = null;
        }

        internal void RequestHostKeyFocus(ICookedInputSite target)
        {
            if (!_session.IsValid || target is IInputCustomFocus inputCustomFocus && inputCustomFocus.OverrideHostFocus() || _session.Form == null)
                return;
            _session.Form.TakeFocus();
        }

        internal void RequestHostMouseCapture(IRawInputSite rawSource, bool state)
        {
            if (_session.Form == null)
                return;
            _session.Form.SetCapture(rawSource, state);
        }

        internal void DeliverInput(ICookedInputSite target, InputInfo info)
        {
            if (info is KeyStateInfo info1 && !FilterKeyboardEvent(info1))
                return;
            DeliverInputWorker(target, info, EventRouteStages.All);
        }

        public void ForwardInput(ICookedInputSite target, InputInfo info) => DeliverInputWorker(target, info, EventRouteStages.Direct);

        private void DeliverInputWorker(
          ICookedInputSite target,
          InputInfo info,
          EventRouteStages stages)
        {
            if (_inputDisabled)
                return;
            InputManager.ZoneDeliveryInfo inputZoneRouting = ComputeInputZoneRouting(target, info);
            if (CheckFocus(target, ref inputZoneRouting, info) && inputZoneRouting.zone != null)
            {
                byte traceLevelForEvent = Trace.GetTraceLevelForEvent(info);
                EventRouteStages stage = EventRouteStages.None;
                if (PreviewInput != null && (stages & EventRouteStages.Preview) != EventRouteStages.None)
                {
                    PreviewInput(this, new InputNotificationEventArgs(info, target, stage));
                    if (info.Handled)
                        stage = EventRouteStages.Preview;
                }
                if (!info.Handled && (stages & EventRouteStages.Routed) != EventRouteStages.None)
                {
                    int num = (int)DeliverInputStageWorker(info, EventRouteStages.Routed, ref inputZoneRouting, traceLevelForEvent);
                    if (info.Handled)
                        stage = EventRouteStages.Routed;
                }
                if (!info.Handled && (stages & EventRouteStages.Direct) != EventRouteStages.None)
                {
                    int num = (int)DeliverInputStageWorker(info, EventRouteStages.Direct, ref inputZoneRouting, traceLevelForEvent);
                    if (info.Handled)
                        stage = EventRouteStages.Direct;
                }
                if (!info.Handled && (stages & EventRouteStages.Bubbled) != EventRouteStages.None)
                {
                    int num = (int)DeliverInputStageWorker(info, EventRouteStages.Bubbled, ref inputZoneRouting, traceLevelForEvent);
                    if (info.Handled)
                        stage = EventRouteStages.Bubbled;
                }
                if (!info.Handled && (stages & EventRouteStages.Unhandled) != EventRouteStages.None)
                {
                    int num = (int)DeliverInputStageWorker(info, EventRouteStages.Unhandled, ref inputZoneRouting, traceLevelForEvent);
                    if (info.Handled)
                        stage = EventRouteStages.Unhandled;
                }
                InputNotificationHandler notificationHandler = !info.Handled ? UnhandledInput : HandledInput;
                if (notificationHandler != null)
                    notificationHandler(this, new InputNotificationEventArgs(info, target, stage));
            }
            if (inputZoneRouting.zone == null)
                return;
            inputZoneRouting.zone.RecycleInputDeliveryData(inputZoneRouting.param);
        }

        private InputDeliveryStatus DeliverInputStageWorker(
          InputInfo input,
          EventRouteStages stage,
          ref InputManager.ZoneDeliveryInfo deliveryInfo,
          byte traceLevel)
        {
            InputDeliveryStatus inputDeliveryStatus = InputDeliveryStatus.Normal;
            if (deliveryInfo.param != null)
            {
                bool handled = input.Handled;
                inputDeliveryStatus = deliveryInfo.zone.DeliverInput(deliveryInfo.param, stage);
                int num1 = input.Handled ? 1 : 0;
                int num2 = handled ? 1 : 0;
            }
            deliveryInfo.stagesDelivered |= stage;
            return inputDeliveryStatus;
        }

        private InputManager.ZoneDeliveryInfo ComputeInputZoneRouting(
          ICookedInputSite finalTarget,
          InputInfo info)
        {
            ITreeNode endpoint = null;
            UIZone uiZone = null;
            if (finalTarget is ITreeNode)
            {
                endpoint = (ITreeNode)finalTarget;
                uiZone = endpoint.Zone;
            }
            if (uiZone == null)
                return new InputManager.ZoneDeliveryInfo();
            return new InputManager.ZoneDeliveryInfo()
            {
                zone = uiZone,
                param = uiZone.PrepareInputForDelivery(endpoint, finalTarget, info)
            };
        }

        private bool CheckFocus(
          ICookedInputSite target,
          ref InputManager.ZoneDeliveryInfo deliveryInfo,
          InputInfo info)
        {
            bool flag = true;
            switch (info)
            {
                case MouseFocusInfo mouseFocusInfo:
                    if (mouseFocusInfo.State || mouseFocusInfo.Other == null)
                    {
                        InputManager.ZoneDeliveryInfo newFocusInfo = new InputManager.ZoneDeliveryInfo();
                        if (mouseFocusInfo.State)
                            newFocusInfo = deliveryInfo;
                        ProcessFocusUpdates(InputDeviceType.Mouse, ref _mouseFocusZone, newFocusInfo, target as ITreeNode);
                        _session.RootZone.UpdateCursor(null);
                    }
                    if (mouseFocusInfo.State && target == mouseFocusInfo.Other)
                        flag = false;
                    return flag;
                case KeyFocusInfo keyFocusInfo:
                    if (keyFocusInfo.State || keyFocusInfo.Other == null)
                    {
                        InputManager.ZoneDeliveryInfo newFocusInfo = new InputManager.ZoneDeliveryInfo();
                        if (keyFocusInfo.State)
                            newFocusInfo = deliveryInfo;
                        ProcessFocusUpdates(InputDeviceType.Keyboard, ref _keyFocusZone, newFocusInfo, target as ITreeNode);
                    }
                    if (keyFocusInfo.State && target == keyFocusInfo.Other)
                        flag = false;
                    return flag;
                default:
                    return flag;
            }
        }

        private static void ProcessFocusUpdates(
          InputDeviceType focusType,
          ref UIZone refCurrentFocusZone,
          InputManager.ZoneDeliveryInfo newFocusInfo,
          ITreeNode actualFocus)
        {
            UIZone zone = refCurrentFocusZone;
            if (zone == null && newFocusInfo.zone == null)
                return;
            refCurrentFocusZone = newFocusInfo.zone;
            if (refCurrentFocusZone == null)
                UpdateZoneFocusStates(focusType, zone, null, false, null);
            if (newFocusInfo.zone == null)
                return;
            UpdateZoneFocusStates(focusType, newFocusInfo.zone, newFocusInfo.param, true, actualFocus);
        }

        private static void UpdateZoneFocusStates(
          InputDeviceType focusType,
          UIZone zone,
          object param,
          bool deepFocusFlag,
          ITreeNode actualFocus)
        {
            ITreeNode directFocusChild = null;
            object obj = null;
            if (deepFocusFlag)
            {
                if (actualFocus != null && actualFocus.Zone == zone)
                    directFocusChild = actualFocus;
                obj = param;
            }
            zone.UpdateInputFocusStates(focusType, deepFocusFlag, directFocusChild, obj);
        }

        internal InputModifiers HACK_SystemModifiers => _HACK_sysModifiers;

        internal void HACK_UpdateSystemModifiers(InputModifiers modifiers) => _HACK_sysModifiers = modifiers;

        private struct ZoneDeliveryInfo
        {
            public UIZone zone;
            public object param;
            public EventRouteStages stagesDelivered;
        }
    }
}
