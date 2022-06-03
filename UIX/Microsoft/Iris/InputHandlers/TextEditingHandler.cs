// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.InputHandlers.TextEditingHandler
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Drawing;
using Microsoft.Iris.Input;
using Microsoft.Iris.Layout;
using Microsoft.Iris.Markup;
using Microsoft.Iris.ModelItems;
using Microsoft.Iris.OS;
using Microsoft.Iris.Render;
using Microsoft.Iris.RenderAPI;
using Microsoft.Iris.Session;
using Microsoft.Iris.UI;
using Microsoft.Iris.ViewItems;
using System;

#if NET6_0_OR_GREATER
using Range = Microsoft.Iris.ModelItems.Range;
#endif

namespace Microsoft.Iris.InputHandlers
{
    internal class TextEditingHandler :
      InputHandler,
      IRichTextCallbacks,
      ITextScrollModelCallback,
      IImeCallbacks
    {
        private const uint WM_LBUTTONDOWN = 513;
        private const uint WM_LBUTTONUP = 514;
        private TextEditingHandler.TextEditingCommand _copyCommand;
        private TextEditingHandler.TextEditingCommand _cutCommand;
        private TextEditingHandler.TextEditingCommand _deleteCommand;
        private TextEditingHandler.TextPasteCommand _pasteCommand;
        private TextEditingHandler.TextEditingCommand _selectAllCommand;
        private TextEditingHandler.TextEditingCommand _undoCommand;
        private uint _bits;
        private Range _selection;
        private Point _inputOffset;
        private int _savedMouseYPositionToWorkAroundRichEditBug;
        private EditableTextData _editData;
        private EventHandler _maxLengthChangedHandler;
        private EventHandler _readOnlyChangedHandler;
        private EventHandler _valueChangedHandler;
        private EventHandler _activationStateHandler;
        private Form _activationStateNotifier;
        private Text _textDisplay;
        private CaretInfo _caretInfo;
        private RichText _editControl;
        private CursorID _cursor;
        private TextScrollModel.State _pendingHorizontalScrollState;
        private TextScrollModel.State _pendingVerticalScrollState;
        private TextScrollModel _horizontalScrollModel;
        private TextScrollModel _verticalScrollModel;
        private SimpleCallback _updateScrollbars;
        private InputInfo _pendingPointerDown;
        private Color _linkColor;
        private string _linkClickedParameter;
        private bool _InImeMode;
        private uint _ImeCallbackToken;

        public TextEditingHandler()
        {
            _caretInfo = new CaretInfo();
            _editControl = new RichText(true, this);
            _maxLengthChangedHandler = new EventHandler(OnEditableTextMaxLengthChanged);
            _readOnlyChangedHandler = new EventHandler(OnEditableTextReadOnlyChanged);
            _valueChangedHandler = new EventHandler(OnEditableTextValueChanged);
            _activationStateHandler = new EventHandler(OnActivationChanged);
            SetBit(Bits.AcceptsEnter);
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            RemoveEditableTextEventHandlers();
            UnregisterImeMessageHandler();
            if (_activationStateNotifier != null)
                UpdateActivationStateHandler(false);
            HorizontalScrollModel = null;
            VerticalScrollModel = null;
            _editControl.Dispose();
        }

        protected override void ConfigureInteractivity()
        {
            base.ConfigureInteractivity();
            if (!HandleDirect)
                return;
            UI.MouseInteractive = true;
            UI.KeyInteractive = true;
        }

        public override void OnZoneAttached()
        {
            if (!UI.DirectKeyFocus)
                return;
            UpdateActivationStateHandler(true);
        }

        private void OnActivationChanged(object sender, EventArgs ea)
        {
            WindowIsActivated = _activationStateNotifier.ActivationState;
            UpdateCaretVisibility();
        }

        public EditableTextData EditableTextData
        {
            get => _editData;
            set
            {
                if (_editData == value)
                    return;
                RemoveEditableTextEventHandlers();
                _editData = value;
                if (_editData != null)
                {
                    AddEditableTextEventHandlers();
                    UpdateMaxLengthOnRichEdit();
                    UpdateReadOnlyOnRichEdit();
                    UpdateContentOnRichEdit();
                }
                FireThreadSafeNotification(NotificationID.EditableTextData);
            }
        }

        private void RemoveEditableTextEventHandlers()
        {
            if (_editData == null)
                return;
            _editData.MaxLengthChanged -= _maxLengthChangedHandler;
            _editData.ReadOnlyChanged -= _readOnlyChangedHandler;
            _editData.ValueChanged -= _valueChangedHandler;
        }

        private void AddEditableTextEventHandlers()
        {
            _editData.MaxLengthChanged += _maxLengthChangedHandler;
            _editData.ReadOnlyChanged += _readOnlyChangedHandler;
            _editData.ValueChanged += _valueChangedHandler;
        }

        private void UnregisterImeMessageHandler() => RendererApi.IFC(NativeApi.SpUnregisterImeCallbacks(_ImeCallbackToken));

        private void OnEditableTextMaxLengthChanged(object sender, EventArgs unused) => UpdateMaxLengthOnRichEdit();

        private void OnEditableTextReadOnlyChanged(object sender, EventArgs unused) => UpdateReadOnlyOnRichEdit();

        private void OnEditableTextValueChanged(object sender, EventArgs unused)
        {
            if (InsideValueChangeOnEditableTextData)
                return;
            UpdateContentOnRichEdit();
        }

        private void UpdateContentOnRichEdit()
        {
            if (_editData == null)
                return;
            InsideContentChangeOnRichEdit = true;
            _editControl.Content = _editData.Value;
            if (_textDisplay != null)
                _textDisplay.MarkScaleDirty();
            InsideContentChangeOnRichEdit = false;
        }

        private void UpdateReadOnlyOnRichEdit()
        {
            _editControl.ReadOnly = _editData.ReadOnly;
            UpdateSelectionAndReadOnlyCommands();
        }

        private void UpdateMaxLengthOnRichEdit() => _editControl.MaxLength = _editData.MaxLength;

        public Text TextDisplay
        {
            get => _textDisplay;
            set
            {
                if (_textDisplay == value)
                    return;
                LayoutCompleteEventHandler completeEventHandler = new LayoutCompleteEventHandler(TextLayoutComplete);
                if (_textDisplay != null)
                {
                    _textDisplay.ExternalRasterizer = null;
                    _textDisplay.ExternalEditingHandler = null;
                    _textDisplay.LayoutComplete -= completeEventHandler;
                }
                _textDisplay = value;
                if (_textDisplay != null)
                {
                    _textDisplay.ExternalRasterizer = _editControl;
                    _textDisplay.ExternalEditingHandler = this;
                    _textDisplay.OnDisplayedContentChange();
                    _textDisplay.LayoutComplete += completeEventHandler;
                    InputOffsetDirty = true;
                }
                FireThreadSafeNotification(NotificationID.TextDisplay);
            }
        }

        private void TextLayoutComplete(object sender) => InputOffsetDirty = true;

        private void FireTypingInputRejectedEvent() => FireThreadSafeNotification(NotificationID.TypingInputRejected);

        public bool DetectUrls
        {
            get => GetBit(Bits.DetectUrls);
            set
            {
                if (!ChangeBit(Bits.DetectUrls, value))
                    return;
                _editControl.DetectUrls = value;
                FireThreadSafeNotification(NotificationID.DetectUrls);
            }
        }

        public Color LinkColor
        {
            get => _linkColor;
            set
            {
                if (!(_linkColor != value))
                    return;
                _linkColor = value;
                FireThreadSafeNotification(NotificationID.LinkColor);
            }
        }

        private void FireLinkClicked() => FireThreadSafeNotification(NotificationID.LinkClicked);

        public string LinkClickedParameter
        {
            get => _linkClickedParameter;
            private set
            {
                if (!(_linkClickedParameter != value))
                    return;
                _linkClickedParameter = value;
                FireThreadSafeNotification(NotificationID.LinkClickedParameter);
            }
        }

        HRESULT IRichTextCallbacks.SetCursor(EditCursorType type)
        {
            CursorID cursorId = CursorID.NotSpecified;
            switch (type)
            {
                case EditCursorType.IBeam:
                    cursorId = CursorID.IBeam;
                    break;
            }
            if (cursorId != _cursor)
            {
                _cursor = cursorId;
                UpdateCursor();
            }
            return new HRESULT(0);
        }

        internal override CursorID GetCursor() => _cursor;

        private bool IgnoreReadOnlyChar(char character) => character == '\r' || character == '\t';

        protected override void OnKeyDown(UIClass ui, KeyStateInfo info)
        {
            bool flag = ForwardKeyStateChange(info);
            switch (info.Key)
            {
                case Keys.Back:
                    if (_editData.ReadOnly)
                        FireTypingInputRejectedEvent();
                    info.MarkHandled();
                    break;
                case Keys.Tab:
                    if (flag)
                        info.MarkHandled();
                    HandledTabKeyDown = info.Handled;
                    break;
                case Keys.Enter:
                    if (!info.Handled && AcceptsEnter && (info.Modifiers == InputModifiers.None && !_editData.ReadOnly))
                    {
                        _editData.Submit();
                        info.MarkHandled();
                    }
                    HandledEnterKeyDown = info.Handled;
                    break;
            }
        }

        protected override void OnKeyCharacter(UIClass ui, KeyCharacterInfo info)
        {
            if (ForwardKeyCharacter(info) && !info.Handled && (_editData.ReadOnly && !IgnoreReadOnlyChar(info.Character)))
                FireTypingInputRejectedEvent();
            switch (info.Character)
            {
                case '\b':
                    info.MarkHandled();
                    break;
                case '\t':
                    if (!HandledTabKeyDown)
                        break;
                    info.MarkHandled();
                    break;
                case '\r':
                    if (!HandledEnterKeyDown)
                        break;
                    info.MarkHandled();
                    break;
            }
        }

        protected override void OnKeyUp(UIClass ui, KeyStateInfo info)
        {
            ForwardKeyStateChange(info);
            switch (info.Key)
            {
                case Keys.Back:
                    info.MarkHandled();
                    break;
                case Keys.Tab:
                    if (!HandledTabKeyDown)
                        break;
                    info.MarkHandled();
                    HandledTabKeyDown = false;
                    break;
                case Keys.Enter:
                    if (!HandledEnterKeyDown)
                        break;
                    info.MarkHandled();
                    HandledEnterKeyDown = false;
                    break;
            }
        }

        private bool ForwardKeyToRichEdit(KeyStateInfo info)
        {
            bool flag = true;
            switch (info.Key)
            {
                case Keys.Tab:
                    flag = info.Action != KeyAction.Down ? HandledTabKeyDown : AcceptsTab;
                    break;
                case Keys.Enter:
                    flag = info.Action != KeyAction.Down ? HandledEnterKeyDown : AcceptsEnter;
                    break;
            }
            return flag;
        }

        private bool ForwardKeyCharacterToRichEdit(KeyCharacterInfo info)
        {
            bool flag = true;
            switch (info.Character)
            {
                case '\t':
                    flag = HandledTabKeyDown;
                    break;
                case '\r':
                    flag = HandledEnterKeyDown;
                    break;
            }
            return flag;
        }

        private bool ForwardKeyStateChange(KeyStateInfo info)
        {
            if (!ForwardKeyToRichEdit(info))
                return false;
            if (_editControl.ForwardKeyStateNotification(info.NativeMessageID, (int)info.Key, info.ScanCode, (int)info.RepeatCount, (uint)info.Modifiers, info.KeyboardFlags))
                info.MarkHandled();
            return true;
        }

        private bool ForwardKeyCharacter(KeyCharacterInfo info)
        {
            if (!ForwardKeyCharacterToRichEdit(info))
                return false;
            if (_editControl.ForwardKeyCharacterNotification(info.NativeMessageID, info.Character, info.ScanCode, (int)info.RepeatCount, (uint)info.Modifiers, info.KeyboardFlags))
                info.MarkHandled();
            return true;
        }

        protected override void OnGainKeyFocus(UIClass ui, KeyFocusInfo info)
        {
            RendererApi.IFC(NativeApi.SpRegisterImeCallbacks(this, out _ImeCallbackToken));
            _editControl.NotifyOfFocusChange(true);
            if (Overtype)
                SelectAll();
            DeliverPendingPointerDown();
            if (UI.IsZoned)
                UpdateActivationStateHandler(true);
            if (!_textDisplay.UsePasswordMask && !_textDisplay.DisableIme)
                return;
            RendererApi.IFC(NativeApi.SpPostDeferredImeMessage(1032U, new UIntPtr(_ImeCallbackToken), UIntPtr.Zero));
        }

        protected override void OnLoseKeyFocus(UIClass ui, KeyFocusInfo info)
        {
            _editControl.NotifyOfFocusChange(false);
            HandledEnterKeyDown = false;
            HandledTabKeyDown = false;
            if (UI.IsZoned)
                UpdateActivationStateHandler(false);
            ClearPendingPointerDown();
            RendererApi.IFC(NativeApi.SpUnregisterImeCallbacks(_ImeCallbackToken));
        }

        protected override void OnLoseMouseFocus(UIClass ui, MouseFocusInfo info)
        {
        }

        private void UpdateActivationStateHandler(bool add)
        {
            if (add)
            {
                _activationStateNotifier = UI.Zone.Form;
                _activationStateNotifier.ActivationChange += _activationStateHandler;
                OnActivationChanged(null, EventArgs.Empty);
            }
            else
            {
                _activationStateNotifier.ActivationChange -= _activationStateHandler;
                _activationStateNotifier = null;
            }
        }

        protected override void OnMouseDoubleClick(UIClass ui, MouseButtonInfo info) => ForwardMouseInput(info);

        protected override void OnMouseMove(UIClass ui, MouseMoveInfo info) => ForwardMouseInput(info);

        protected override void OnMousePrimaryDown(UIClass ui, MouseButtonInfo info)
        {
            _pendingPointerDown = info;
            info.Lock();
            if (!UI.DirectKeyFocus && HandlerStage == InputHandlerStage.Direct && UI.KeyFocusOnMouseDown)
                return;
            DeliverPendingPointerDown();
        }

        private void DeliverPendingPointerDown()
        {
            if (_pendingPointerDown == null)
                return;
            MousePrimaryDown = true;
            MouseButtonInfo pendingPointerDown = (MouseButtonInfo)_pendingPointerDown;
            _savedMouseYPositionToWorkAroundRichEditBug = pendingPointerDown.Y;
            ForwardMouseInput(pendingPointerDown);
            ClearPendingPointerDown();
        }

        private void ClearPendingPointerDown()
        {
            if (_pendingPointerDown == null)
                return;
            _pendingPointerDown.Unlock();
            _pendingPointerDown = null;
        }

        protected override void OnMousePrimaryUp(UIClass ui, MouseButtonInfo info)
        {
            MousePrimaryDown = false;
            ForwardMouseInput(info);
        }

        protected override void OnMouseSecondaryDown(UIClass ui, MouseButtonInfo info) => ForwardMouseInput(info);

        protected override void OnMouseSecondaryUp(UIClass ui, MouseButtonInfo info) => ForwardMouseInput(info);

        protected override void OnMouseWheel(UIClass ui, MouseWheelInfo info)
        {
            if (!_textDisplay.WordWrap)
                return;
            ForwardMouseInput(info);
        }

        private bool ForwardMouseInput(
          uint nativeMessageID,
          InputModifiers modifiers,
          MouseButtons button,
          int inputX,
          int inputY,
          int wheelDelta)
        {
            if (InputOffsetDirty)
            {
                Vector3 parentOffsetPxlVector;
                ViewItem.GetAccumulatedOffsetAndScale(_textDisplay, UI.RootItem, out parentOffsetPxlVector, out Vector3 _);
                _inputOffset = new Point((int)Math.Floor(parentOffsetPxlVector.X), (int)Math.Floor(parentOffsetPxlVector.Y));
                InputOffsetDirty = false;
            }
            int x = inputX - _inputOffset.X;
            int y = inputY - _inputOffset.Y;
            if (MousePrimaryDown && _textDisplay != null && !_textDisplay.WordWrap)
                y = _savedMouseYPositionToWorkAroundRichEditBug - _inputOffset.Y;
            return _editControl.ForwardMouseInput(nativeMessageID, (uint)modifiers, (int)button, x, y, wheelDelta);
        }

        private void ForwardMouseInput(MouseActionInfo info)
        {
            if (!ForwardMouseInput(info.NativeMessageID, info.Modifiers, info.Button, info.X, info.Y, info.WheelDelta))
                return;
            info.MarkHandled();
        }

        public bool Overtype
        {
            get => GetBit(Bits.Overtype);
            set
            {
                if (!ChangeBit(Bits.Overtype, value))
                    return;
                FireThreadSafeNotification(NotificationID.Overtype);
            }
        }

        public bool AcceptsTab
        {
            get => GetBit(Bits.AcceptsTab);
            set
            {
                if (!ChangeBit(Bits.AcceptsTab, value))
                    return;
                FireThreadSafeNotification(NotificationID.AcceptsTab);
            }
        }

        public bool AcceptsEnter
        {
            get => GetBit(Bits.AcceptsEnter);
            set
            {
                if (!ChangeBit(Bits.AcceptsEnter, value))
                    return;
                FireThreadSafeNotification(NotificationID.AcceptsEnter);
            }
        }

        public bool WordWrap
        {
            set
            {
                if (!ChangeBit(Bits.WordWrap, value))
                    return;
                _editControl.SetWordWrap(value);
                if (!value)
                    return;
                UpdateContentOnRichEdit();
            }
        }

        public CaretInfo CaretInfo => _caretInfo;

        private void CreateCommands()
        {
            if (GetBit(Bits.CommandsCreated))
                return;
            SetBit(Bits.CommandsCreated);
            _undoCommand = new TextEditingHandler.TextEditingCommand(new SimpleCallback(_editControl.Undo));
            _cutCommand = new TextEditingHandler.TextEditingCommand(new SimpleCallback(_editControl.Cut));
            _copyCommand = new TextEditingHandler.TextEditingCommand(new SimpleCallback(_editControl.Copy));
            _pasteCommand = new TextEditingHandler.TextPasteCommand(new SimpleCallback(_editControl.Paste));
            _deleteCommand = new TextEditingHandler.TextEditingCommand(new SimpleCallback(_editControl.Delete));
            _selectAllCommand = new TextEditingHandler.TextEditingCommand(new SimpleCallback(SelectAll));
            UpdateSelectionAndReadOnlyCommands();
            UpdateTextBasedCommandAvailability();
        }

        public IUICommand UndoCommand
        {
            get
            {
                CreateCommands();
                return _undoCommand;
            }
        }

        public void Undo() => _editControl.Undo();

        public IUICommand CutCommand
        {
            get
            {
                CreateCommands();
                return _cutCommand;
            }
        }

        public void Cut() => _editControl.Cut();

        public IUICommand CopyCommand
        {
            get
            {
                CreateCommands();
                return _copyCommand;
            }
        }

        public void Copy() => _editControl.Copy();

        public IUICommand PasteCommand
        {
            get
            {
                CreateCommands();
                return _pasteCommand;
            }
        }

        public void Paste() => _editControl.Paste();

        public IUICommand DeleteCommand
        {
            get
            {
                CreateCommands();
                return _deleteCommand;
            }
        }

        public void Delete() => _editControl.Delete();

        public IUICommand SelectAllCommand
        {
            get
            {
                CreateCommands();
                return _selectAllCommand;
            }
        }

        public void SelectAll()
        {
            string str = _editData != null ? _editData.Value : null;
            if (string.IsNullOrEmpty(str))
                return;
            SelectionRange = new Range(0, str.Length);
        }

        public Range SelectionRange
        {
            get => _selection;
            set
            {
                if (_selection.IsEqual(value))
                    return;
                _editControl.SetSelectionRange(value.Begin, value.End);
            }
        }

        private void UpdateTextBasedCommandAvailability()
        {
            _undoCommand.Available = _editControl.CanUndo;
            _selectAllCommand.Available = _editData != null && !string.IsNullOrEmpty(_editData.Value);
        }

        private void UpdateSelectionAndReadOnlyCommands()
        {
            if (!GetBit(Bits.CommandsCreated))
                return;
            bool flag1 = !_selection.IsEmpty;
            bool flag2 = _editData == null || _editData.ReadOnly;
            _cutCommand.Available = flag1 && !flag2;
            _copyCommand.Available = flag1;
            _deleteCommand.Available = flag1 && !flag2;
            _pasteCommand.TextIsReadOnly = flag2;
        }

        HRESULT IRichTextCallbacks.TextChanged()
        {
            if (!Application.IsApplicationThread)
            {
                Application.DeferredInvoke(args => ((IRichTextCallbacks)args).TextChanged(), this, DeferredInvokePriority.Normal);
                return new HRESULT(0);
            }
            if (GetBit(Bits.CommandsCreated))
                UpdateTextBasedCommandAvailability();
            if (_editData != null && !InsideContentChangeOnRichEdit)
            {
                InsideValueChangeOnEditableTextData = true;
                _editData.Value = _editControl.SimpleContent;
                InsideValueChangeOnEditableTextData = false;
            }
            return new HRESULT(0);
        }

        HRESULT IRichTextCallbacks.InvalidateContent()
        {
            if (!Application.IsApplicationThread)
            {
                Application.DeferredInvoke(args => ((IRichTextCallbacks)args).InvalidateContent(), this, DeferredInvokePriority.Normal);
                return new HRESULT(0);
            }
            if (_textDisplay != null)
                _textDisplay.OnDisplayedContentChange();
            RefreshCaretPosition();
            return new HRESULT(0);
        }

        HRESULT IRichTextCallbacks.SelectionChanged(
          int selectionStart,
          int selectionEnd)
        {
            _selection = new Range(selectionStart, selectionEnd);
            FireThreadSafeNotification(NotificationID.SelectionRange);
            UpdateSelectionAndReadOnlyCommands();
            return new HRESULT(0);
        }

        HRESULT IRichTextCallbacks.MaxLengthExceeded()
        {
            FireTypingInputRejectedEvent();
            return new HRESULT(0);
        }

        HRESULT IRichTextCallbacks.SetTimer(uint id, uint timeout)
        {
            _editControl.SetTimer(id, timeout);
            return new HRESULT(0);
        }

        HRESULT IRichTextCallbacks.KillTimer(uint id)
        {
            _editControl.KillTimer(id);
            return new HRESULT(0);
        }

        HRESULT IRichTextCallbacks.CreateCaret(int width, int height)
        {
            _caretInfo.CreateCaret(new Size(width, height));
            return new HRESULT(0);
        }

        HRESULT IRichTextCallbacks.SetCaretPos(int x, int y)
        {
            _caretInfo.SetCaretPosition(new Point(x, y));
            return new HRESULT(0);
        }

        HRESULT IRichTextCallbacks.ShowCaret(bool visible)
        {
            RichEditCaretVisible = visible;
            UpdateCaretVisibility();
            return new HRESULT(0);
        }

        HRESULT IRichTextCallbacks.ClientToWindow(Point pt, out Point ppt)
        {
            ppt = _textDisplay == null ? new Point(0, 0) : _textDisplay.ClientToWindow(pt);
            return new HRESULT(0);
        }

        HRESULT IRichTextCallbacks.ClientToScreen(Point pt, out Point ppt)
        {
            ppt = _textDisplay.ClientToScreen(pt);
            return new HRESULT(0);
        }

        HRESULT IRichTextCallbacks.LinkClicked(int start, int end)
        {
            string str = _editData != null ? _editData.Value : string.Empty;
            if (string.IsNullOrEmpty(str))
            {
                LinkClickedParameter = string.Empty;
            }
            else
            {
                end = end >= 0 ? Math.Min(str.Length, end) : str.Length;
                start = Math.Min(start, end);
                LinkClickedParameter = str.Substring(start, end - start);
            }
            FireLinkClicked();
            return new HRESULT(0);
        }

        HRESULT IImeCallbacks.OnImeMessageReceived(
          uint message,
          UIntPtr wParam,
          UIntPtr lParam)
        {
            switch (message)
            {
                case 269:
                    InImeCompositionMode = true;
                    break;
                case 270:
                    InImeCompositionMode = false;
                    break;
            }
            return _editControl.ForwardImeMessage(message, wParam, lParam);
        }

        public bool InImeCompositionMode
        {
            get => _InImeMode;
            set
            {
                if (_InImeMode == value)
                    return;
                CaretInfo.IgnoreIdealWidth = value;
                _InImeMode = value;
                FireThreadSafeNotification(NotificationID.InImeCompositionMode);
            }
        }

        HRESULT IRichTextCallbacks.SetScrollRange(
          int whichBarInt,
          int minPosition,
          int extent,
          int viewExtent,
          int scrollPosition)
        {
            if (whichBarInt == 1 && _verticalScrollModel != null)
            {
                SetScrollRange(ref _pendingVerticalScrollState, minPosition, extent, viewExtent, scrollPosition);
                ScheduleScrollbarUpdate(true);
            }
            else if (_horizontalScrollModel != null)
            {
                SetScrollRange(ref _pendingHorizontalScrollState, minPosition, extent, viewExtent, scrollPosition);
                ScheduleScrollbarUpdate(false);
            }
            return new HRESULT(0);
        }

        private void SetScrollRange(
          ref TextScrollModel.State state,
          int minPosition,
          int extent,
          int viewExtent,
          int scrollAmount)
        {
            state.Min = minPosition;
            state.Extent = extent;
            state.ViewExtent = viewExtent;
            state.ScrollAmount = scrollAmount;
        }

        private bool IsVerticalScrollbar(ScrollbarType whichBar) => whichBar == ScrollbarType.Vertical || whichBar == ScrollbarType.Both;

        private bool IsHorizontalScrollbar(ScrollbarType whichBar) => whichBar == ScrollbarType.Horizontal || whichBar == ScrollbarType.Both;

        HRESULT IRichTextCallbacks.EnableScrollbar(
          int whichBarInt,
          ScrollbarEnableFlags flags)
        {
            ScrollbarType whichBar = (ScrollbarType)whichBarInt;
            if (IsVerticalScrollbar(whichBar) && _verticalScrollModel != null)
            {
                EnableScrollbar(ref _pendingVerticalScrollState, flags);
                ScheduleScrollbarUpdate(true);
            }
            if (IsHorizontalScrollbar(whichBar) && _horizontalScrollModel != null)
            {
                EnableScrollbar(ref _pendingHorizontalScrollState, flags);
                ScheduleScrollbarUpdate(false);
            }
            return new HRESULT(0);
        }

        private void EnableScrollbar(ref TextScrollModel.State state, ScrollbarEnableFlags flags)
        {
            switch (flags)
            {
                case ScrollbarEnableFlags.EnableBoth:
                    state.CanScrollUp = true;
                    state.CanScrollDown = true;
                    break;
                case ScrollbarEnableFlags.DisableNear:
                    state.CanScrollUp = false;
                    state.CanScrollDown = true;
                    break;
                case ScrollbarEnableFlags.DisableFar:
                    state.CanScrollUp = true;
                    state.CanScrollDown = false;
                    break;
                case ScrollbarEnableFlags.DisableBoth:
                    state.CanScrollUp = false;
                    state.CanScrollDown = false;
                    break;
            }
        }

        private void ScheduleScrollbarUpdate(bool vertical)
        {
            bool flag = !GetBit(Bits.PendingVerticalScrollbarUpdate) && !GetBit(Bits.PendingHorizontalScrollbarUpdate);
            if (vertical)
                SetBit(Bits.PendingVerticalScrollbarUpdate);
            else
                SetBit(Bits.PendingHorizontalScrollbarUpdate);
            if (!flag)
                return;
            if (_updateScrollbars == null)
                _updateScrollbars = new SimpleCallback(UpdateScrollbars);
            DeferredCall.Post(DispatchPriority.Normal, _updateScrollbars);
        }

        private void UpdateScrollbars()
        {
            if (GetBit(Bits.PendingVerticalScrollbarUpdate))
            {
                ClearBit(Bits.PendingVerticalScrollbarUpdate);
                _verticalScrollModel.UpdateState(_pendingVerticalScrollState);
                _pendingVerticalScrollState = new TextScrollModel.State();
            }
            if (!GetBit(Bits.PendingHorizontalScrollbarUpdate))
                return;
            ClearBit(Bits.PendingHorizontalScrollbarUpdate);
            _horizontalScrollModel.UpdateState(_pendingHorizontalScrollState);
            _pendingHorizontalScrollState = new TextScrollModel.State();
        }

        public TextScrollModel HorizontalScrollModel
        {
            get
            {
                if (_horizontalScrollModel == null)
                    StoreScrollModel(ref _horizontalScrollModel, new TextScrollModel());
                return _horizontalScrollModel;
            }
            set
            {
                if (_horizontalScrollModel == value)
                    return;
                StoreScrollModel(ref _horizontalScrollModel, value);
                FireThreadSafeNotification(NotificationID.HorizontalScrollModel);
            }
        }

        public TextScrollModel VerticalScrollModel
        {
            get
            {
                if (_verticalScrollModel == null)
                    StoreScrollModel(ref _verticalScrollModel, new TextScrollModel());
                return _verticalScrollModel;
            }
            set
            {
                if (_verticalScrollModel == value)
                    return;
                StoreScrollModel(ref _verticalScrollModel, value);
                FireThreadSafeNotification(NotificationID.VerticalScrollModel);
            }
        }

        private void StoreScrollModel(ref TextScrollModel storage, TextScrollModel newDude)
        {
            if (storage != null)
                storage.DetachCallbacks();
            storage = newDude;
            if (storage != null)
                storage.AttachCallbacks(this);
            _editControl.SetScrollbars(_horizontalScrollModel != null, _verticalScrollModel != null);
        }

        private ScrollbarType WhichScrollbar(TextScrollModel who) => who != _verticalScrollModel ? ScrollbarType.Horizontal : ScrollbarType.Vertical;

        void ITextScrollModelCallback.ScrollUp(TextScrollModel who) => _editControl.ScrollUp(WhichScrollbar(who));

        void ITextScrollModelCallback.ScrollDown(TextScrollModel who) => _editControl.ScrollDown(WhichScrollbar(who));

        void ITextScrollModelCallback.PageUp(TextScrollModel who) => _editControl.PageUp(WhichScrollbar(who));

        void ITextScrollModelCallback.PageDown(TextScrollModel who) => _editControl.PageDown(WhichScrollbar(who));

        void ITextScrollModelCallback.ScrollToPosition(
          TextScrollModel who,
          int whereTo)
        {
            _editControl.ScrollToPosition(WhichScrollbar(who), whereTo);
        }

        private void UpdateCaretVisibility() => CaretInfo.SetVisible(RichEditCaretVisible && WindowIsActivated);

        private void RefreshCaretPosition()
        {
            if (!CaretInfo.Visible || MousePrimaryDown || _editControl == null)
                return;
            _editControl.NotifyOfFocusChange(true);
        }

        private bool HandledTabKeyDown
        {
            get => GetBit(Bits.HandledTabKeyDown);
            set => SetBit(Bits.HandledTabKeyDown, value);
        }

        private bool HandledEnterKeyDown
        {
            get => GetBit(Bits.HandledEnterKeyDown);
            set => SetBit(Bits.HandledEnterKeyDown, value);
        }

        private bool InputOffsetDirty
        {
            get => GetBit(Bits.InputOffsetDirty);
            set => SetBit(Bits.InputOffsetDirty, value);
        }

        private bool MousePrimaryDown
        {
            get => GetBit(Bits.MousePrimaryDown);
            set => SetBit(Bits.MousePrimaryDown, value);
        }

        private bool InsideValueChangeOnEditableTextData
        {
            get => GetBit(Bits.InsideValueChangeOnEditableTextData);
            set => SetBit(Bits.InsideValueChangeOnEditableTextData, value);
        }

        private bool InsideContentChangeOnRichEdit
        {
            get => GetBit(Bits.InsideContentChangeOnRichEdit);
            set => SetBit(Bits.InsideContentChangeOnRichEdit, value);
        }

        private bool RichEditCaretVisible
        {
            get => GetBit(Bits.RichEditCaretVisible);
            set => SetBit(Bits.RichEditCaretVisible, value);
        }

        private bool WindowIsActivated
        {
            get => GetBit(Bits.WindowIsActivated);
            set => SetBit(Bits.WindowIsActivated, value);
        }

        private bool GetBit(TextEditingHandler.Bits lookupBit) => ((TextEditingHandler.Bits)_bits & lookupBit) != 0;

        private void SetBit(TextEditingHandler.Bits changeBit)
        {
            TextEditingHandler textEditingHandler = this;
            textEditingHandler._bits = (uint)((TextEditingHandler.Bits)textEditingHandler._bits | changeBit);
        }

        private void SetBit(TextEditingHandler.Bits changeBit, bool value) => _bits = value ? (uint)((TextEditingHandler.Bits)_bits | changeBit) : (uint)((TextEditingHandler.Bits)_bits & ~changeBit);

        private void ClearBit(TextEditingHandler.Bits changeBit)
        {
            TextEditingHandler textEditingHandler = this;
            textEditingHandler._bits = (uint)((TextEditingHandler.Bits)textEditingHandler._bits & ~changeBit);
        }

        private bool ChangeBit(TextEditingHandler.Bits changeBit, bool value)
        {
            uint num = value ? (uint)((TextEditingHandler.Bits)_bits | changeBit) : (uint)((TextEditingHandler.Bits)_bits & ~changeBit);
            bool flag = (int)num != (int)_bits;
            if (flag)
                _bits = num;
            return flag;
        }

        private enum Bits
        {
            AcceptsTab = 1,
            AcceptsEnter = 2,
            HandledTabKeyDown = 4,
            HandledEnterKeyDown = 16, // 0x00000010
            InputOffsetDirty = 32, // 0x00000020
            MousePrimaryDown = 64, // 0x00000040
            Overtype = 128, // 0x00000080
            InsideValueChangeOnEditableTextData = 256, // 0x00000100
            InsideContentChangeOnRichEdit = 512, // 0x00000200
            RichEditCaretVisible = 1024, // 0x00000400
            WindowIsActivated = 2048, // 0x00000800
            WordWrap = 4096, // 0x00001000
            CommandsCreated = 8192, // 0x00002000
            PendingHorizontalScrollbarUpdate = 16384, // 0x00004000
            PendingVerticalScrollbarUpdate = 32768, // 0x00008000
            DetectUrls = 65536, // 0x00010000
        }

        private class TextEditingCommand : UICommand
        {
            private SimpleCallback _onInvoked;

            public TextEditingCommand(SimpleCallback onInvoked) => _onInvoked = onInvoked;

            protected override void OnInvoked()
            {
                base.OnInvoked();
                _onInvoked();
            }
        }

        private class TextPasteCommand : TextEditingHandler.TextEditingCommand, IUICommand
        {
            private bool _isReadOnly;

            public TextPasteCommand(SimpleCallback onInvoked)
              : base(onInvoked)
            {
            }

            bool IUICommand.Available
            {
                get => Clipboard.ContainsText() && !_isReadOnly;
                set
                {
                }
            }

            public bool TextIsReadOnly
            {
                get => _isReadOnly;
                set => _isReadOnly = value;
            }
        }
    }
}
