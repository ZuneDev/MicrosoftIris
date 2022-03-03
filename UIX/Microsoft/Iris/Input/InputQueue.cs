// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Input.InputQueue
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Queues;
using Microsoft.Iris.Render;
using Microsoft.Iris.Session;
using System;

namespace Microsoft.Iris.Input
{
    internal class InputQueue : SimpleQueue
    {
        private QueueItem.Stack _filterStack;
        private InputManager _inputManager;
        private ICookedInputSite _currentKeyFocus;
        private ICookedInputSite _desiredKeyFocus;
        private KeyFocusReason _desiredKeyFocusReason;
        private ICookedInputSite _lastCompletedKeyFocus;
        private bool _revalidateKeyFocus;
        private uint _keyFocusRepairCount;
        private SimpleCallback _invalidKeyFocusCallback;
        private bool _mouseMoved;
        private IRawInputSite _rawMouseSite;
        private IRawInputSite _rawMouseNaturalSite;
        private Point _rawMousePos;
        private Point _rawScreenPos;
        private InputModifiers _rawMouseModifiers;
        private int _mouseWheelDelta;
        private bool _dragOver;
        private bool _dragging;
        private ICookedInputSite _dragSource;
        private IRawInputSite _rawDropTargetSite;
        private Point _rawDragPoint;
        private InputModifiers _rawDragModifiers;
        private object _dragData;
        private object _pendingDragData;
        private ICookedInputSite _appDropTarget;
        private IRawInputSite _appMouseFocusSite;
        private IRawInputSite _appNaturalTarget;
        private ICookedInputSite _appMouseFocusTarget;
        private ICookedInputSite _appMouseFocusCapture;
        private InputModifiers _appMouseFocusButtons;
        private ICookedInputSite _lastCompletedMouseFocus;
        private Point _appMousePos;
        private bool _revalidateMouseFocus;
        private SimpleQueue _inputIdleQueue;

        public InputQueue(InputManager manager)
        {
            _inputManager = manager;
            _filterStack = new QueueItem.Stack();
            _rawMousePos = new Point();
            _appMousePos = new Point();
            _desiredKeyFocusReason = KeyFocusReason.Default;
            _inputIdleQueue = new SimpleQueue();
            _inputIdleQueue.Wake += new EventHandler(OnChildQueueWake);
            _invalidKeyFocusCallback = new SimpleCallback(InvalidKeyFocusCallback);
        }

        public ICookedInputSite InstantaneousKeyFocus => _currentKeyFocus;

        public ICookedInputSite PendingKeyFocus => _desiredKeyFocus;

        public bool PendingKeyFocusIsDefault => _desiredKeyFocusReason == KeyFocusReason.Default;

        public ICookedInputSite LastCompletedKeyFocus => _lastCompletedKeyFocus;

        public ICookedInputSite CurrentMouseFocus => _appMouseFocusTarget;

        public InputModifiers DragModifiers => _rawDragModifiers;

        public void RequestKeyFocus(ICookedInputSite desired) => RequestKeyFocus(desired, KeyFocusReason.Other);

        public void RequestKeyFocus(ICookedInputSite desired, KeyFocusReason reason)
        {
            if (_desiredKeyFocus != desired)
            {
                _desiredKeyFocus = desired;
                _desiredKeyFocusReason = reason;
            }
            else if (reason != KeyFocusReason.Default)
                _desiredKeyFocusReason = reason;
            if (desired != null)
                _inputManager.RequestHostKeyFocus(desired);
            if (_desiredKeyFocus == _currentKeyFocus)
                return;
            OnWake();
        }

        public void PrepareToShutDown()
        {
            RequestKeyFocus(null);
            RawMouseLeave();
        }

        public void RawKeyAction(KeyInfo info) => PostItem(GenerateGenericInput(info));

        public void RawMouseMove(
          IRawInputSite site,
          IRawInputSite naturalSite,
          InputModifiers modifiers,
          ref RawMouseData rawEventData)
        {
            _rawMouseSite = site;
            _rawMouseNaturalSite = naturalSite;
            _rawMousePos.X = rawEventData._positionX;
            _rawMousePos.Y = rawEventData._positionY;
            _rawScreenPos.X = rawEventData._screenX;
            _rawScreenPos.Y = rawEventData._screenY;
            _rawMouseModifiers = modifiers;
            SetMouseMoved();
        }

        public void RawMouseLeave()
        {
            CancelMouseCapture(null);
            _mouseWheelDelta = 0;
            _rawMouseSite = null;
            _rawMouseNaturalSite = null;
            _rawMousePos.X = -1;
            _rawMousePos.Y = -1;
            _rawScreenPos.X = -1;
            _rawScreenPos.Y = -1;
            _rawMouseModifiers = InputModifiers.None;
            SetMouseMoved();
        }

        public void RawMouseButtonState(
          uint message,
          IRawInputSite site,
          IRawInputSite naturalSite,
          InputModifiers modifiers,
          MouseButtons button,
          bool state)
        {
            PostItem(GenerateMouseButton(site, naturalSite, _rawMousePos.X, _rawMousePos.Y, _rawScreenPos.X, _rawScreenPos.Y, modifiers, button, state, message));
        }

        public void RawMouseWheel(InputModifiers modifiers, ref RawMouseData rawEventData)
        {
            bool flag = _mouseWheelDelta != 0;
            _mouseWheelDelta += rawEventData._wheelDelta;
            _rawMouseModifiers = modifiers;
            if (flag || _mouseWheelDelta == 0)
                return;
            OnWake();
        }

        public void SimulateDragEnter(
          ICookedInputSite dragSource,
          IRawInputSite rawTargetSite,
          object data,
          int x,
          int y,
          InputModifiers modifiers)
        {
            SimulateDragDrop(dragSource, rawTargetSite, data, x, y, modifiers, DragOperation.Enter);
        }

        public void SimulateDragOver(InputModifiers modifiers) => SimulateDragDrop(_dragSource, _rawDropTargetSite, null, _rawDragPoint.X, _rawDragPoint.Y, modifiers, DragOperation.Over);

        public void SimulateDragOver(
          IRawInputSite rawTargetSite,
          int x,
          int y,
          InputModifiers modifiers)
        {
            SimulateDragDrop(_dragSource, rawTargetSite, null, x, y, modifiers, DragOperation.Over);
        }

        public void SimulateDragEnd(
          IRawInputSite rawTargetSite,
          InputModifiers modifiers,
          DragOperation formOperation)
        {
            PushFilterStack(GenerateDragDrop(_dragSource, null, _rawDragPoint.X, _rawDragPoint.Y, modifiers, DragOperation.DragComplete));
            SimulateDragDrop(_dragSource, rawTargetSite, null, _rawDragPoint.X, _rawDragPoint.Y, modifiers, formOperation);
            OnWake();
            _dragSource = null;
        }

        private void SimulateDragDrop(
          ICookedInputSite dragSource,
          IRawInputSite rawTargetSite,
          object data,
          int x,
          int y,
          InputModifiers modifiers,
          DragOperation formOperation)
        {
            _dragSource = dragSource;
            InputItem dragDropItem = GenerateDragDropItem(dragSource, rawTargetSite, data, x, y, modifiers, formOperation);
            if (dragDropItem == null)
                return;
            PushFilterStack(dragDropItem);
            OnWake();
        }

        public void RawDragDrop(
          IRawInputSite rawSite,
          object data,
          int x,
          int y,
          InputModifiers modifiers,
          DragOperation formOperation,
          RawDragData rawEventData)
        {
            if (_dragSource == null)
            {
                if (formOperation == DragOperation.Over && data == null && _pendingDragData != null)
                {
                    data = _pendingDragData;
                    formOperation = DragOperation.Enter;
                }
                _pendingDragData = null;
                InputItem dragDropItem = GenerateDragDropItem(null, rawSite, data, x, y, modifiers, formOperation);
                if (dragDropItem == null)
                    return;
                PostItem(dragDropItem);
            }
            else if (formOperation == DragOperation.Enter)
            {
                _pendingDragData = data;
            }
            else
            {
                if (formOperation != DragOperation.Leave)
                    return;
                _pendingDragData = null;
            }
        }

        private InputItem GenerateDragDropItem(
          ICookedInputSite dragSource,
          IRawInputSite rawSite,
          object data,
          int x,
          int y,
          InputModifiers modifiers,
          DragOperation formOperation)
        {
            InputItem inputItem = null;
            IRawInputSite rawDropTargetSite = _rawDropTargetSite;
            if (formOperation == DragOperation.Enter)
                _dragData = data;
            _rawDropTargetSite = rawSite;
            _rawDragPoint = new Point(x, y);
            _rawDragModifiers = modifiers;
            if (formOperation != DragOperation.Drop)
            {
                _dragging = true;
                if (rawDropTargetSite == rawSite)
                {
                    if (!_dragOver)
                    {
                        _dragOver = true;
                        OnWake();
                    }
                }
                else
                    inputItem = GenerateDragDrop(null, rawSite, x, y, modifiers, DragOperation.Over);
            }
            else
            {
                inputItem = GenerateDragDrop(_appDropTarget, rawSite, x, y, modifiers, DragOperation.Drop);
                _appDropTarget = null;
            }
            if (formOperation == DragOperation.Drop || formOperation == DragOperation.Leave)
            {
                _dragOver = false;
                _dragging = false;
                _rawDropTargetSite = null;
                _rawDragPoint = new Point();
                _rawDragModifiers = InputModifiers.None;
                if (formOperation == DragOperation.Leave)
                    _dragData = null;
            }
            return inputItem;
        }

        public object GetDragDropValue()
        {
            object dragData = _dragData;
            if (!_dragging)
                _dragData = null;
            return dragData;
        }

        public void RawInputIdleItem(QueueItem item) => _inputIdleQueue.PostItem(item);

        public void RevalidateInputSiteUsage(ICookedInputSite target, bool recursiveFlag)
        {
            bool flag1 = false;
            if (target == null)
            {
                if (!recursiveFlag)
                {
                    _revalidateKeyFocus = true;
                    _revalidateMouseFocus = true;
                    SetMouseMoved();
                    return;
                }
                flag1 = true;
            }
            CancelMouseCapture(target);
            bool flag2 = false;
            if (!_mouseMoved && _appMouseFocusTarget != null && (flag1 || _appMouseFocusTarget == target || recursiveFlag && _inputManager.IsSiteInfluencedByPeer(_appMouseFocusTarget, target)))
            {
                _revalidateMouseFocus = true;
                SetMouseMoved();
                if (_appMouseFocusTarget == _desiredKeyFocus)
                    flag2 = true;
            }
            if (_revalidateKeyFocus || !flag2 && (_desiredKeyFocus == null || !flag1 && _desiredKeyFocus != target && (!recursiveFlag || !_inputManager.IsSiteInfluencedByPeer(_desiredKeyFocus, target))))
                return;
            _revalidateKeyFocus = true;
            OnWake();
        }

        public override QueueItem GetNextItem()
        {
            InputItem inputItem;
            QueueItem queueItem1 = null;
            while (true)
            {
                QueueItem queueItem2;
                do
                {
                    queueItem2 = PeekFilterStack();
                    if (queueItem2 != null)
                    {
                        queueItem1 = FilterItem(queueItem2);
                        if (queueItem1 == null)
                            PopFilterStack();
                        else
                            goto label_5;
                    }
                    else
                        goto label_8;
                }
                while (!(queueItem2 is InputItem));
                inputItem = (InputItem)queueItem2;
                inputItem.ReturnToPool();
                continue;
            label_5:
                if (queueItem1 != queueItem2)
                {
                    PushFilterStack(queueItem1);
                    queueItem1 = null;
                }
                else
                    break;
            }
            PopFilterStack();
        label_8:
            return queueItem1;
        }

        private void PushFilterStack(QueueItem item) => _filterStack.Push(item);

        private QueueItem PopFilterStack() => _filterStack.Pop();

        private QueueItem PeekFilterStack()
        {
            QueueItem queueItem = _filterStack.Peek();
            if (queueItem == null)
            {
                queueItem = GetNextRawInputItem();
                if (queueItem != null)
                    PushFilterStack(queueItem);
            }
            return queueItem;
        }

        private QueueItem FilterItem(QueueItem item)
        {
            if (item is InputItem inputItem)
            {
                switch (inputItem.Info)
                {
                    case MouseActionInfo mouseActionInfo:
                        MapMouseInput(mouseActionInfo);
                        Point clientOffset = new Point(mouseActionInfo.X, mouseActionInfo.Y);
                        switch (mouseActionInfo)
                        {
                            case MouseMoveInfo _:
                                QueueItem queueItem1 = UpdateMouseFocus(mouseActionInfo.Target, mouseActionInfo.RawSource, clientOffset);
                                if (queueItem1 != null)
                                    return queueItem1;
                                if (IsAppMouseMove(mouseActionInfo))
                                {
                                    FinalizeMouseHit(inputItem, mouseActionInfo);
                                    break;
                                }
                                item = null;
                                break;
                            case MouseButtonInfo mouseButtonInfo:
                                if (IsAppMouseMove(mouseActionInfo))
                                {
                                    InputModifiers modifiers1 = mouseButtonInfo.Modifiers;
                                    InputModifiers modifiersForButtons = GetModifiersForButtons(mouseButtonInfo.Button);
                                    InputModifiers modifiers2 = !mouseButtonInfo.IsDown ? modifiers1 | modifiersForButtons : modifiers1 & ~modifiersForButtons;
                                    return GenerateMouseMove(mouseActionInfo.RawSource, mouseActionInfo.NaturalHit, mouseActionInfo.X, mouseActionInfo.Y, mouseButtonInfo.ScreenX, mouseButtonInfo.ScreenY, modifiers2);
                                }
                                FinalizeMouseHit(inputItem, mouseActionInfo);
                                UpdateMouseCapture(mouseActionInfo.RawSource, mouseActionInfo.Target, mouseButtonInfo.Modifiers);
                                _appMouseFocusButtons = mouseButtonInfo.Modifiers & InputModifiers.AllButtons;
                                break;
                            case MouseWheelInfo _:
                                inputItem.UpdateInputSite(_appMouseFocusTarget);
                                break;
                        }
                        break;
                    case MouseFocusInfo mouseFocusInfo:
                        if (!mouseFocusInfo.State && _appMouseFocusButtons != InputModifiers.None)
                        {
                            QueueItem queueItem2 = UpdateOrphanedButtons(mouseFocusInfo.RawSource, mouseFocusInfo.X, mouseFocusInfo.Y);
                            if (queueItem2 != null)
                                return queueItem2;
                            break;
                        }
                        break;
                    case KeyActionInfo _:
                        QueueItem queueItem3 = UpdateKeyFocus();
                        if (queueItem3 != null)
                            return queueItem3;
                        inputItem.UpdateInputSite(_currentKeyFocus);
                        break;
                    case DragDropInfo dragDropInfo:
                        if (dragDropInfo.Operation == DragOperation.Over)
                        {
                            InputItem inputItemB = UpdateDragOver(_inputManager.HitTestInput(_rawDropTargetSite, null));
                            if (inputItemB != null)
                                return inputItemB;
                            inputItemB.UpdateInputSite(_appDropTarget);
                            break;
                        }
                        break;
                }
            }
            return item;
        }

        private InputModifiers GetModifiersForButtons(MouseButtons button)
        {
            InputModifiers inputModifiers = InputModifiers.None;
            if ((button & MouseButtons.Left) != MouseButtons.None)
                inputModifiers |= InputModifiers.LeftMouse;
            if ((button & MouseButtons.Middle) != MouseButtons.None)
                inputModifiers |= InputModifiers.MiddleMouse;
            if ((button & MouseButtons.Right) != MouseButtons.None)
                inputModifiers |= InputModifiers.RightMouse;
            if ((button & MouseButtons.XButton1) != MouseButtons.None)
                inputModifiers |= InputModifiers.XMouse1;
            if ((button & MouseButtons.XButton2) != MouseButtons.None)
                inputModifiers |= InputModifiers.XMouse2;
            return inputModifiers;
        }

        private QueueItem GetNextRawInputItem()
        {
            QueueItem queueItem;
            if ((queueItem = InjectRawInput_Prty()) == null && (queueItem = InjectRawInput_Norm()) == null)
                queueItem = InjectRawInput_Idle();
            return queueItem;
        }

        private QueueItem InjectRawInput_Prty() => UpdateKeyFocus();

        private QueueItem InjectRawInput_Norm() => base.GetNextItem();

        private QueueItem InjectRawInput_Idle()
        {
            if (_mouseWheelDelta != 0)
            {
                QueueItem mouseWheel = GenerateMouseWheel(_rawMouseSite, _rawMouseNaturalSite, _rawMousePos.X, _rawMousePos.Y, _rawScreenPos.X, _rawScreenPos.Y, _rawMouseModifiers, _mouseWheelDelta);
                _mouseWheelDelta = 0;
                return mouseWheel;
            }
            if (_mouseMoved)
            {
                QueueItem mouseMove = GenerateMouseMove(_rawMouseSite, _rawMouseNaturalSite, _rawMousePos.X, _rawMousePos.Y, _rawScreenPos.X, _rawScreenPos.Y, _rawMouseModifiers);
                _mouseMoved = false;
                return mouseMove;
            }
            if (!_dragOver)
                return _inputIdleQueue.GetNextItem();
            InputItem dragDrop = GenerateDragDrop(null, _rawDropTargetSite, _rawDragPoint.X, _rawDragPoint.Y, _rawDragModifiers, DragOperation.Over);
            _dragOver = false;
            return dragDrop;
        }

        private void OnChildQueueWake(object sender, EventArgs args) => OnWake();

        private void MapMouseInput(MouseActionInfo info) => _inputManager.MapMouseInput(info, _appMouseFocusCapture);

        private void FinalizeMouseHit(InputItem item, MouseActionInfo mouse)
        {
            item.UpdateInputSite(mouse.Target);
            _appMousePos = new Point(mouse.X, mouse.Y);
            _appNaturalTarget = mouse.NaturalHit;
        }

        private QueueItem CheckForInvalidKeyFocus()
        {
            QueueItem queueItem = null;
            if (!IsValidInputSite(_desiredKeyFocus) || !_inputManager.IsValidKeyFocusSite(_desiredKeyFocus))
            {
                ++_keyFocusRepairCount;
                _revalidateKeyFocus = true;
                queueItem = GenerateInvalidKeyFocusCallback();
            }
            else
            {
                int focusRepairCount = (int)_keyFocusRepairCount;
                _keyFocusRepairCount = 0U;
                _revalidateKeyFocus = false;
            }
            return queueItem;
        }

        private void InvalidKeyFocusCallback() => _inputManager.RepairInvalidKeyFocus(_keyFocusRepairCount);

        private QueueItem UpdateKeyFocus()
        {
            QueueItem queueItem = null;
            if (_revalidateKeyFocus || _currentKeyFocus != _desiredKeyFocus)
            {
                queueItem = CheckForInvalidKeyFocus();
                if (queueItem == null && _currentKeyFocus != _desiredKeyFocus)
                {
                    if (_currentKeyFocus != null)
                    {
                        if (!IsValidInputSite(_currentKeyFocus))
                            _currentKeyFocus = null;
                        queueItem = GenerateKeyFocus(_currentKeyFocus, false, _desiredKeyFocus, _desiredKeyFocusReason);
                        _currentKeyFocus = null;
                    }
                    if (queueItem == null && _desiredKeyFocus != null)
                    {
                        _currentKeyFocus = _desiredKeyFocus;
                        if (!IsValidInputSite(_lastCompletedKeyFocus))
                            _lastCompletedKeyFocus = null;
                        queueItem = GenerateKeyFocus(_currentKeyFocus, true, _lastCompletedKeyFocus, _desiredKeyFocusReason);
                    }
                    if (_currentKeyFocus == _desiredKeyFocus)
                        _lastCompletedKeyFocus = _currentKeyFocus;
                }
                if (queueItem == null && _currentKeyFocus != null)
                    queueItem = GenerateKeyFocus(_currentKeyFocus, true, _lastCompletedKeyFocus, KeyFocusReason.Validation);
            }
            return queueItem;
        }

        private QueueItem UpdateMouseFocus(
          ICookedInputSite target,
          IRawInputSite site,
          Point clientOffset)
        {
            QueueItem queueItem = null;
            if (_revalidateMouseFocus || _appMouseFocusTarget != target)
            {
                _revalidateMouseFocus = false;
                if (_appMouseFocusTarget != target)
                {
                    if (_appMouseFocusTarget != null)
                    {
                        if (!IsValidInputSite(_appMouseFocusTarget))
                            _appMouseFocusTarget = null;
                        queueItem = GenerateMouseFocus(_appMouseFocusTarget, _appMouseFocusSite, _appMousePos.X, _appMousePos.Y, false, target);
                        _appMouseFocusSite = null;
                        _appMouseFocusTarget = null;
                        _appMousePos.X = -1;
                        _appMousePos.Y = -1;
                    }
                    if (queueItem == null && target != null)
                    {
                        _appMouseFocusSite = site;
                        _appMouseFocusTarget = target;
                        if (!IsValidInputSite(_lastCompletedMouseFocus))
                            _lastCompletedMouseFocus = null;
                        queueItem = GenerateMouseFocus(_appMouseFocusTarget, _appMouseFocusSite, clientOffset.X, clientOffset.Y, true, _lastCompletedMouseFocus);
                    }
                    if (_appMouseFocusTarget == target)
                        _lastCompletedMouseFocus = _appMouseFocusTarget;
                }
                if (queueItem == null && _appMouseFocusTarget != null)
                    queueItem = GenerateMouseFocus(_appMouseFocusTarget, _appMouseFocusSite, clientOffset.X, clientOffset.Y, true, _lastCompletedMouseFocus);
            }
            return queueItem;
        }

        private void UpdateMouseCapture(
          IRawInputSite site,
          ICookedInputSite target,
          InputModifiers modifiers)
        {
            bool flag = (modifiers & InputModifiers.AllButtons) != InputModifiers.None;
            ICookedInputSite cookedInputSite = null;
            if (flag)
                cookedInputSite = target;
            if (_appMouseFocusCapture == cookedInputSite)
                return;
            _inputManager.RequestHostMouseCapture(site, cookedInputSite != null);
            _appMouseFocusCapture = cookedInputSite;
            SetMouseMoved();
        }

        private QueueItem UpdateOrphanedButtons(IRawInputSite site, int x, int y)
        {
            MouseButtons button;
            uint message;
            if ((_appMouseFocusButtons & InputModifiers.LeftMouse) != InputModifiers.None)
            {
                _appMouseFocusButtons &= ~InputModifiers.LeftMouse;
                button = MouseButtons.Left;
                message = 514U;
            }
            else if ((_appMouseFocusButtons & InputModifiers.MiddleMouse) != InputModifiers.None)
            {
                _appMouseFocusButtons &= ~InputModifiers.MiddleMouse;
                button = MouseButtons.Middle;
                message = 520U;
            }
            else if ((_appMouseFocusButtons & InputModifiers.RightMouse) != InputModifiers.None)
            {
                _appMouseFocusButtons &= ~InputModifiers.RightMouse;
                button = MouseButtons.Right;
                message = 517U;
            }
            else if ((_appMouseFocusButtons & InputModifiers.XMouse1) != InputModifiers.None)
            {
                _appMouseFocusButtons &= ~InputModifiers.XMouse1;
                button = MouseButtons.XButton1;
                message = 524U;
            }
            else if ((_appMouseFocusButtons & InputModifiers.XMouse2) != InputModifiers.None)
            {
                _appMouseFocusButtons &= ~InputModifiers.XMouse2;
                button = MouseButtons.XButton2;
                message = 524U;
            }
            else
            {
                _appMouseFocusButtons = InputModifiers.None;
                return null;
            }
            return GenerateMouseButton(site, null, x, y, _rawScreenPos.X, _rawScreenPos.Y, _appMouseFocusButtons, button, false, message);
        }

        private InputItem UpdateDragOver(ICookedInputSite target)
        {
            if (target != _appDropTarget)
            {
                if (_appDropTarget != null)
                {
                    ICookedInputSite appDropTarget = _appDropTarget;
                    _appDropTarget = null;
                    return GenerateDragDrop(appDropTarget, null, _rawDragPoint.X, _rawDragPoint.Y, _rawMouseModifiers, DragOperation.Leave);
                }
                if (target != null)
                {
                    _appDropTarget = target;
                    return GenerateDragDrop(target, _rawDropTargetSite, _rawDragPoint.X, _rawDragPoint.Y, _rawDragModifiers, DragOperation.Enter);
                }
            }
            return null;
        }

        private bool IsAppMouseMove(MouseActionInfo mouseInfo) => mouseInfo.Target != _appMouseFocusTarget || mouseInfo.NaturalHit != _appNaturalTarget || mouseInfo.X != _appMousePos.X || mouseInfo.Y != _appMousePos.Y;

        private bool IsValidInputSite(ICookedInputSite target) => target == null || _inputManager.IsValidCookedInputSite(target);

        private void SetMouseMoved()
        {
            if (_mouseMoved)
                return;
            _mouseMoved = true;
            OnWake();
        }

        private void CancelMouseCapture(ICookedInputSite cancelSite)
        {
            if (_appMouseFocusCapture == null || cancelSite != _appMouseFocusCapture && cancelSite != null)
                return;
            UpdateMouseCapture(_appMouseFocusCapture.RawInputSource, null, InputModifiers.None);
        }

        private QueueItem GenerateGenericInput(InputInfo info) => InputItem.Create(_inputManager, null, info);

        private QueueItem GenerateInvalidKeyFocusCallback() => DeferredCall.Create(_invalidKeyFocusCallback);

        private QueueItem GenerateKeyFocus(
          ICookedInputSite target,
          bool focus,
          ICookedInputSite other,
          KeyFocusReason reason)
        {
            return InputItem.Create(_inputManager, target, KeyFocusInfo.Create(focus, other, reason));
        }

        private InputItem GenerateMouseFocus(
          ICookedInputSite target,
          IRawInputSite site,
          int x,
          int y,
          bool focus,
          ICookedInputSite other)
        {
            return InputItem.Create(_inputManager, target, MouseFocusInfo.Create(site, x, y, focus, other));
        }

        private InputItem GenerateMouseMove(
          IRawInputSite site,
          IRawInputSite naturalSite,
          int x,
          int y,
          int screenX,
          int screenY,
          InputModifiers modifiers)
        {
            return InputItem.Create(_inputManager, null, MouseMoveInfo.Create(site, naturalSite, x, y, screenX, screenY, modifiers));
        }

        private InputItem GenerateMouseButton(
          IRawInputSite site,
          IRawInputSite naturalSite,
          int x,
          int y,
          int screenX,
          int screenY,
          InputModifiers modifiers,
          MouseButtons button,
          bool state,
          uint message)
        {
            return InputItem.Create(_inputManager, null, MouseButtonInfo.Create(site, naturalSite, x, y, screenX, screenY, modifiers, button, state, message));
        }

        private InputItem GenerateMouseWheel(
          IRawInputSite site,
          IRawInputSite naturalSite,
          int x,
          int y,
          int screenX,
          int screenY,
          InputModifiers modifiers,
          int wheelDelta)
        {
            return InputItem.Create(_inputManager, null, MouseWheelInfo.Create(site, naturalSite, x, y, screenX, screenY, modifiers, wheelDelta));
        }

        private InputItem GenerateDragDrop(
          ICookedInputSite cookedSite,
          IRawInputSite rawSite,
          int x,
          int y,
          InputModifiers modifiers,
          DragOperation operation)
        {
            InputInfo info = DragDropInfo.Create(rawSite, x, y, modifiers, operation);
            return InputItem.Create(_inputManager, cookedSite, info);
        }
    }
}
