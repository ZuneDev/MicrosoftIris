// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.UI.UIClass
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Accessibility;
using Microsoft.Iris.Animations;
using Microsoft.Iris.Debug;
using Microsoft.Iris.Drawing;
using Microsoft.Iris.Input;
using Microsoft.Iris.InputHandlers;
using Microsoft.Iris.Layout;
using Microsoft.Iris.Library;
using Microsoft.Iris.Markup;
using Microsoft.Iris.Navigation;
using Microsoft.Iris.Render;
using Microsoft.Iris.RenderAPI.Drawing;
using Microsoft.Iris.Session;
using Microsoft.Iris.ViewItems;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;

namespace Microsoft.Iris.UI
{
    public class UIClass :
      Microsoft.Iris.Library.TreeNode,
      ICookedInputSite,
      IMarkupTypeBase,
      IDisposableOwner,
      INotifyObject,
      ISchemaInfo
    {
        public const string UIStateReservedSymbolName = "UI";
        private BitVector32 _bits;
        private Host _ownerHost;
        private int _uiIDFactory;
        private ViewItem _rootItem;
        private InputHandlerList _inputHandlers;
        private UIClassTypeSchema _typeSchema;
        private Dictionary<object, object> _storage;
        private Vector<IDisposableObject> _disposables;
        private MarkupListeners _listeners;
        private NotifyService _notifier = new NotifyService();
        private ScriptRunScheduler _scriptRunScheduler = new ScriptRunScheduler();
        private static DeferredHandler s_executePendingScriptsHandler = new DeferredHandler(ExecutePendingScripts);
        private static readonly FocusStateHandler s_updateMouseFocusStates = new FocusStateHandler(UpdateMouseFocusStates);
        private static readonly FocusStateHandler s_updateKeyFocusStates = new FocusStateHandler(UpdateKeyFocusStates);
        private static readonly DataCookie s_cursorProperty = DataCookie.ReserveSlot();
        private static readonly DataCookie s_cursorOverrideProperty = DataCookie.ReserveSlot();
        private static readonly DataCookie s_accProxyProperty = DataCookie.ReserveSlot();
        private static readonly DataCookie s_pendingFocusRestoreProperty = DataCookie.ReserveSlot();
        private static readonly DataCookie s_focusInterestTargetProperty = DataCookie.ReserveSlot();
        private static readonly DataCookie s_focusInterestTargetMarginsProperty = DataCookie.ReserveSlot();
        private static readonly EventCookie s_descendantKeyFocusChangedEvent = EventCookie.ReserveSlot();
        private static readonly EventCookie s_descendantMouseFocusChangedEvent = EventCookie.ReserveSlot();
        private EventContext _eventContext;

        public UIClass(UIClassTypeSchema type)
        {
            _typeSchema = type;
            _storage = new Dictionary<object, object>(type.TotalPropertiesAndLocalsCount);
            _bits = new BitVector32();
            SetBit(Bits.Flippable, true);
            SetBit(Bits.CreateInterestOnFocus, true);
            SetBit(Bits.KeyFocusOnMouseDown, true);
            SetBit(Bits.AllowDoubleClicks, true);
            SetBit(Bits.Enabled, true);
            SetBit(Bits.ScriptEnabled, true);
        }

        public bool Initialized => GetBit(Bits.Initialized);

        public void RegisterDisposable(IDisposableObject disposable)
        {
            if (disposable is ViewItem || disposable is InputHandler)
                return;
            if (_disposables == null)
                _disposables = new Vector<IDisposableObject>();
            _disposables.Add(disposable);
        }

        public bool UnregisterDisposable(ref IDisposableObject disposable)
        {
            if (!(disposable is ViewItem) && !(disposable is InputHandler) && _disposables != null)
            {
                int index = _disposables.IndexOf(disposable);
                if (index != -1)
                {
                    disposable = _disposables[index];
                    _disposables.RemoveAt(index);
                    return true;
                }
            }
            return false;
        }

        protected override void OnDispose()
        {
            _typeSchema.RunFinalEvaluates(this);
            base.OnDispose();
            if (Initialized)
                AccessibleProxy.NotifyDestroyed(this);
            SetBit(Bits.ScriptEnabled, false);
            if (_listeners != null)
            {
                _listeners.Dispose(this);
                _listeners = null;
            }
            _notifier.ClearListeners();
            if (_rootItem != null)
            {
                DisposeViewItemTree(_rootItem);
                _rootItem = null;
            }
            DisposeInputHandlers();
            _storage.Clear();
            if (_disposables != null)
            {
                for (int index = 0; index < _disposables.Count; ++index)
                    _disposables[index].Dispose(this);
            }
            RemoveEventHandlers(s_descendantMouseFocusChangedEvent);
            RemoveEventHandlers(s_descendantKeyFocusChangedEvent);
        }

        private void DisposeInputHandlers()
        {
            if (_inputHandlers == null)
                return;
            foreach (DisposableObject inputHandler in _inputHandlers)
                inputHandler.Dispose(this);
        }

        internal void DisposeViewItemTree(ViewItem item)
        {
            if (item.UI != this)
                return;
            ViewItem nextSibling;
            for (ViewItem viewItem = (ViewItem)item.FirstChild; viewItem != null; viewItem = nextSibling)
            {
                nextSibling = (ViewItem)viewItem.NextSibling;
                DisposeViewItemTree(viewItem);
            }
            item.Dispose(this);
        }

        internal void DestroyVisualTree(ViewItem subjectItem) => DestroyVisualTree(subjectItem, false);

        internal void DestroyVisualTree(ViewItem subjectItem, bool markLayoutOutputDirty) => DestroyVisualTree(subjectItem, true, markLayoutOutputDirty);

        private void DestroyVisualTree(
          ViewItem subjectItem,
          bool allowAnimations,
          bool markLayoutOutputDirty)
        {
            OrphanedVisualCollection orphans = new OrphanedVisualCollection(UISession.AnimationManager);
            DestroyVisualTreeWorker(subjectItem, orphans, allowAnimations, markLayoutOutputDirty);
            orphans.OnLayoutApplyComplete();
        }

        private void DestroyVisualTreeWorker(
          ViewItem subjectItem,
          OrphanedVisualCollection orphans,
          bool allowAnimations,
          bool markLayoutOutputDirty)
        {
            subjectItem.RendererVisual.MouseOptions = MouseOptions.None;
            foreach (ViewItem child in subjectItem.Children)
            {
                if (child.HasVisual)
                    DestroyVisualTreeWorker(child, orphans, allowAnimations, markLayoutOutputDirty);
            }
            if (markLayoutOutputDirty)
                subjectItem.MarkLayoutOutputDirty(false);
            if (allowAnimations)
                AnimateDestroyedVisual(subjectItem, orphans);
            if (subjectItem == subjectItem.UI.RootItem && subjectItem.UI.RootItem.HasVisual)
                subjectItem.UI.RevalidateUsage(true, true);
            subjectItem.OrphanVisuals(orphans);
        }

        [Conditional("DEBUG")]
        private static void DEBUG_VerifyVisualTreeDestroyed(ViewItem vi)
        {
            foreach (ViewItem child in vi.Children)
                ;
        }

        public int AllocateUIID(ViewItem item) => _uiIDFactory++;

        public override bool IsRoot => Zone.RootUI == this;

        public UIClass Parent => (UIClass)base.Parent;

        protected override void OnZoneAttached()
        {
            base.OnZoneAttached();
            RevalidateUsage(true, false);
            bool enabledFlag = true;
            UIClass parent = Parent;
            if (parent != null)
                enabledFlag = parent.FullyEnabled;
            DeliverFullyEnabled(enabledFlag);
            if (_inputHandlers != null)
            {
                foreach (InputHandler inputHandler in _inputHandlers)
                    inputHandler.OnZoneAttached();
            }
            AccessibleProxy.NotifyTreeChanged(this);
        }

        protected override void OnZoneDetached()
        {
            base.OnZoneDetached();
            ChangeBit(Bits.AppFullyEnabled, false);
            RevalidateUsage(true, true);
            if (_inputHandlers != null)
            {
                foreach (InputHandler inputHandler in _inputHandlers)
                    inputHandler.OnZoneDetached();
            }
            if (_rootItem == null || !_rootItem.HasVisual)
                return;
            DestroyVisualTree(_rootItem);
        }

        protected override void OnChildrenChanged()
        {
            base.OnChildrenChanged();
            if (!IsZoned)
                return;
            AccessibleProxy.NotifyTreeChanged(this);
        }

        public bool HasAccessibleProxy => GetBit(Bits.HasAccProxy);

        protected virtual AccessibleProxy OnCreateAccessibleProxy(
          UIClass ui,
          Accessible data)
        {
            return new AccessibleProxy(ui, data);
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public AccessibleProxy AccessibleProxy
        {
            get
            {
                AccessibleProxy accessibleProxy;
                if (HasAccessibleProxy)
                {
                    accessibleProxy = (AccessibleProxy)GetData(s_accProxyProperty);
                }
                else
                {
                    Accessible data = null;
                    foreach (KeyValuePair<object, object> keyValuePair in _storage)
                    {
                        if (keyValuePair.Value is Accessible accessible)
                        {
                            data = accessible;
                            break;
                        }
                    }
                    if (data == null)
                        data = new Accessible();
                    accessibleProxy = OnCreateAccessibleProxy(this, data);
                    SetData(s_accProxyProperty, accessibleProxy);
                    SetBit(Bits.HasAccProxy, true);
                }
                return accessibleProxy;
            }
        }

        internal static bool ShouldPlayAnimation(IAnimationProvider provider)
        {
            if (provider == null)
                return false;
            bool flag = true;
            if (provider is NonIntensiveAnimation)
                flag = false;
            return AnimationSystem.Enabled || !flag;
        }

        private void AnimateNewVisual(
          ViewItem vi,
          Vector3 startPosVector,
          Vector2 startSizeVector,
          Vector3 startScaleVector,
          Rotation startRotation)
        {
            vi.VisualPosition = startPosVector;
            vi.VisualSize = startSizeVector;
            vi.VisualScale = startScaleVector;
            vi.VisualRotation = startRotation;
            AnimateNewVisual(vi);
        }

        private void AnimateNewVisual(ViewItem vi)
        {
            vi.PlayShowAnimation();
            if (!DirectKeyFocus)
                return;
            vi.PlayAnimation(AnimationEventType.GainFocus);
        }

        private void AnimateDestroyedVisual(ViewItem vi, OrphanedVisualCollection orphans) => vi.PlayHideAnimation(orphans);

        private void AnimateVisual(
          ViewItem vi,
          Vector3 newPosVector,
          Vector2 newSizeVector,
          Vector3 newScaleVector,
          Rotation newRotation)
        {
            Vector3 visualPosition = vi.VisualPosition;
            Vector2 visualSize = vi.VisualSize;
            Vector3 visualScale = vi.VisualScale;
            Rotation visualRotation = vi.VisualRotation;
            float visualAlpha = vi.VisualAlpha;
            AnimationArgs args = new AnimationArgs(vi, visualPosition, visualSize, visualScale, visualRotation, visualAlpha, newPosVector, newSizeVector, newScaleVector, newRotation, visualAlpha);
            if (visualPosition != newPosVector)
                vi.ApplyAnimatableValue(AnimationEventType.Move, ref args);
            if (visualSize != newSizeVector)
                vi.ApplyAnimatableValue(AnimationEventType.Size, ref args);
            if (visualScale != newScaleVector)
                vi.ApplyAnimatableValue(AnimationEventType.Scale, ref args);
            if (!(visualRotation != newRotation))
                return;
            vi.ApplyAnimatableValue(AnimationEventType.Rotate, ref args);
        }

        private void TryToPlayAnimationOnTree(ViewItem vi, AnimationEventType type)
        {
            if (!vi.HasVisual)
                return;
            vi.PlayAnimation(type);
            foreach (ViewItem child in vi.Children)
            {
                if (child.UI == this)
                    TryToPlayAnimationOnTree(child, type);
            }
        }

        internal object SaveKeyFocus()
        {
            if (KeyFocus)
            {
                UIClass keyFocusDescendant = KeyFocusDescendant;
                if (keyFocusDescendant != null && keyFocusDescendant.RootItem != null)
                    return new UIClass.SavedFocusState(keyFocusDescendant.RootItem);
            }
            return null;
        }

        internal void RestoreKeyFocus(object obj) => RestoreKeyFocus(obj, true);

        internal void RestoreKeyFocus(object obj, bool allowDeferralFlag)
        {
            if (!(obj is UIClass.SavedFocusState focusState))
                return;
            ViewItem resultItem;
            ViewItemID failedComponent;
            switch (RootItem.FindChildFromPath(focusState.Path, out resultItem, out failedComponent))
            {
                case FindChildResult.Success:
                    if (resultItem == null)
                        break;
                    UIClass ui = resultItem.UI;
                    if (!HasDescendant(ui) || !ui.IsKeyFocusable())
                        break;
                    resultItem.NavigateInto(focusState.FocusIsDefault);
                    break;
                case FindChildResult.PotentiallyFaultIn:
                    if (!allowDeferralFlag)
                        break;
                    new UIClass.DeferredKeyFocusRestoreHelper(this, focusState).FaultInChild(resultItem, failedComponent);
                    break;
            }
        }

        private UIClass.DeferredKeyFocusRestoreHelper PendingFocusRestore
        {
            get => GetData(s_pendingFocusRestoreProperty) as UIClass.DeferredKeyFocusRestoreHelper;
            set => SetData(s_pendingFocusRestoreProperty, value);
        }

        private static bool CheckHandled(InputInfo info, InputHandler inputHandler) => info.Handled;

        public void SetAreaOfInterest(AreaOfInterestID id) => SetAreaOfInterest(id, RootItem, Inset.Zero);

        public void SetAreaOfInterest(AreaOfInterestID id, ViewItem target, Inset margins) => target.SetAreaOfInterest(id, margins);

        public void ClearAreaOfInterest(AreaOfInterestID id) => ClearAreaOfInterest(id, RootItem);

        public void ClearAreaOfInterest(AreaOfInterestID id, ViewItem target) => target.ClearAreaOfInterest(id);

        internal void UpdateMouseHandling(ViewItem childItem)
        {
            if (childItem == null)
            {
                childItem = RootItem;
                if (childItem == null)
                    return;
            }
            if (!childItem.HasVisual)
                return;
            MouseOptions mouseOptions = MouseOptions.None;
            if (!childItem.IsOffscreen && childItem.AllowMouseInput && (childItem != RootItem || IsInputBranchEnabled() && InputEnabled))
                mouseOptions |= MouseOptions.Traversable;
            if (childItem.MouseInteractive && MouseInteractive)
                mouseOptions |= MouseOptions.Hittable;
            if (childItem == RootItem)
                mouseOptions |= MouseOptions.Returnable;
            if (childItem.ClipMouse)
                mouseOptions |= MouseOptions.ClipChildren;
            childItem.RendererVisual.MouseOptions = mouseOptions;
        }

        internal void OnHostVisibilityChanged()
        {
            OnInputEnabledChanged();
            AccessibleProxy.NotifyVisibilityChange(this, Visible);
        }

        public IList EnsureInputHandlerStorage()
        {
            if (_inputHandlers == null)
                _inputHandlers = new InputHandlerList();
            return _inputHandlers;
        }

        internal bool Visible => IsRoot || _ownerHost.HasVisual;

        internal bool InputEnabled
        {
            get
            {
                if (IsRoot)
                    return true;
                return Visible && _ownerHost.InputEnabled;
            }
        }

        public bool Enabled
        {
            get => GetBit(Bits.Enabled);
            set
            {
                if (!ChangeBit(Bits.Enabled, value))
                    return;
                UpdateMouseHandling(null);
                RevalidateUsage(true, !value);
                FireNotification(NotificationID.Enabled);
                if (!IsZoned || Parent != null && !Parent.FullyEnabled)
                    return;
                NotifyFullyEnabledChange();
            }
        }

        private bool HostEnabled => _ownerHost == null ? IsValid : _ownerHost.InputEnabled;

        public bool FullyEnabled => GetBit(Bits.AppFullyEnabled);

        public bool DirectMouseFocus => GetBit(Bits.DirectMouseFocus);

        public bool MouseFocus => GetBit(Bits.MouseFocus);

        public bool DirectKeyFocus => GetBit(Bits.DirectKeyFocus);

        public bool KeyFocus => GetBit(Bits.KeyFocus);

        public UIClass KeyFocusDescendant
        {
            get
            {
                //uiClass = (UIClass)null;
                if (KeyFocus && UISession.InputManager.RawInstantaneousKeyFocus is UIClass uiClass)
                {
                    if (uiClass.IsDisposed)
                        return null;
                    else
                        return uiClass;
                }
                else
                {
                    return null;
                }
            }
        }

        public uint PaintOrder
        {
            get => _ownerHost.Layer;
            set => _ownerHost.Layer = value;
        }

        public bool CreateInterestOnFocus
        {
            get => GetBit(Bits.CreateInterestOnFocus);
            set
            {
                if (!ChangeBit(Bits.CreateInterestOnFocus, value))
                    return;
                FireNotification(NotificationID.CreateInterestOnFocus);
            }
        }

        public ViewItem FocusInterestTarget
        {
            get => (ViewItem)GetData(s_focusInterestTargetProperty);
            set
            {
                if (FocusInterestTarget == value)
                    return;
                SetData(s_focusInterestTargetProperty, value);
                FireNotification(NotificationID.FocusInterestTarget);
            }
        }

        public Inset FocusInterestTargetMargins
        {
            get
            {
                object data = GetData(s_focusInterestTargetMarginsProperty);
                return data != null ? (Inset)data : Inset.Zero;
            }
            set
            {
                if (!(FocusInterestTargetMargins != value))
                    return;
                SetData(s_focusInterestTargetMarginsProperty, value);
                FireNotification(NotificationID.FocusInterestTargetMargins);
            }
        }

        public CursorID Cursor
        {
            get
            {
                CursorID cursorId = CursorID.NotSpecified;
                object data = GetData(s_cursorProperty);
                if (data != null)
                    cursorId = (CursorID)data;
                return cursorId;
            }
            set
            {
                if (Cursor == value)
                    return;
                SetData(s_cursorProperty, value);
                FireNotification(NotificationID.Cursor);
                if (!IsZoned || OverrideCursor != CursorID.NotSpecified)
                    return;
                Zone.UpdateCursor(this);
            }
        }

        private CursorID OverrideCursor
        {
            get
            {
                CursorID cursorId = CursorID.NotSpecified;
                object data = GetData(s_cursorOverrideProperty);
                if (data != null)
                    cursorId = (CursorID)data;
                return cursorId;
            }
            set
            {
                CursorID overrideCursor = OverrideCursor;
                if (overrideCursor == value)
                    return;
                SetData(s_cursorOverrideProperty, value);
                if (!IsZoned || overrideCursor == CursorID.NotSpecified && value == Cursor)
                    return;
                Zone.UpdateCursor(this);
            }
        }

        public CursorID EffectiveCursor
        {
            get
            {
                CursorID cursorId = OverrideCursor;
                if (cursorId == CursorID.NotSpecified)
                    cursorId = Cursor;
                return cursorId;
            }
        }

        public void UpdateCursor()
        {
            CursorID cursorId = CursorID.NotSpecified;
            if (_inputHandlers != null)
            {
                foreach (InputHandler inputHandler in _inputHandlers)
                {
                    if (inputHandler.Enabled)
                    {
                        cursorId = inputHandler.GetCursor();
                        if (cursorId != CursorID.NotSpecified)
                            break;
                    }
                }
            }
            OverrideCursor = cursorId;
        }

        public bool KeyFocusOnMouseEnter
        {
            get => GetBit(Bits.KeyFocusOnMouseEnter);
            set => SetBit(Bits.KeyFocusOnMouseEnter, value);
        }

        public bool KeyFocusOnMouseDown
        {
            get => GetBit(Bits.KeyFocusOnMouseDown);
            set => SetBit(Bits.KeyFocusOnMouseDown, value);
        }

        public bool AllowDoubleClicks
        {
            get => GetBit(Bits.AllowDoubleClicks);
            set => SetBit(Bits.AllowDoubleClicks, value);
        }

        internal void NotifyFullyEnabledChange() => Zone.ScheduleFullyEnabledChangeNotifications();

        internal void DeliverFullyEnabled(bool enabledFlag)
        {
            if (!Enabled || !HostEnabled)
                enabledFlag = false;
            if (ChangeBit(Bits.AppFullyEnabled, enabledFlag))
                FireNotification(NotificationID.FullyEnabled);
            foreach (UIClass child in Children)
                child.DeliverFullyEnabled(enabledFlag);
        }

        public void EnableRawInput(bool enableFlag)
        {
            if (!ChangeBit(Bits.RawInputDisabled, !enableFlag))
                return;
            UpdateMouseHandling(null);
            RevalidateUsage(true, !enableFlag);
        }

        public bool IsInputBranchEnabled() => GetBit(Bits.Enabled) && !GetBit(Bits.RawInputDisabled);

        public bool IsEligibleForInput() => IsEligibleForInput(out UIClass _);

        public bool IsEligibleForInput(out UIClass failurePoint)
        {
            failurePoint = null;
            UIClass uiClass = this;
            while (true)
            {
                failurePoint = uiClass;
                if (uiClass.IsInputBranchEnabled() && uiClass.InputEnabled)
                {
                    UIClass parent = uiClass.Parent;
                    if (parent != null)
                        uiClass = parent;
                    else
                        goto label_5;
                }
                else
                    break;
            }
            return false;
        label_5:
            failurePoint = null;
            return IsZoned;
        }

        public bool MouseInteractive
        {
            get => GetBit(Bits.MouseInteractive);
            set => SetMouseInteractive(value, false);
        }

        public void SetMouseInteractive(bool value, bool fromScript)
        {
            if (fromScript)
                SetBit(Bits.MouseInteractiveSet, true);
            if (!fromScript && GetBit(Bits.MouseInteractiveSet) || !ChangeBit(Bits.MouseInteractive, value))
                return;
            if (value && _rootItem != null && !HasMouseInteractiveContent())
                _rootItem.MouseInteractive = true;
            UpdateMouseHandling(null);
            RevalidateUsage(false, !value);
            FireNotification(NotificationID.MouseInteractive);
            if (!DebugOutlines.Enabled)
                return;
            DebugOutlines.NotifyInteractivityChange(Host);
        }

        public bool IsMouseFocusable() => MouseInteractive && IsEligibleForInput();

        public bool KeyInteractive
        {
            get => GetBit(Bits.KeyInteractive);
            set
            {
                if (!ChangeBit(Bits.KeyInteractive, value))
                    return;
                RevalidateUsage(false, !value);
                FireNotification(NotificationID.KeyInteractive);
                if (!DebugOutlines.Enabled)
                    return;
                DebugOutlines.NotifyInteractivityChange(Host);
            }
        }

        public event SessionInputHandler SessionInput
        {
            add => Zone.SessionInput += value;
            remove => Zone.SessionInput -= value;
        }

        public bool HostActivated => Zone.Form.ActivationState;

        public bool IsKeyFocusable() => KeyInteractive && IsEligibleForInput();

        public UIClass FindKeyFocusableAncestor()
        {
            if (!IsValid)
                return null;
            UIClass uiClass;
            for (uiClass = this; uiClass != null; uiClass = uiClass.Parent)
            {
                UIClass failurePoint = null;
                if (uiClass.KeyInteractive && uiClass.IsEligibleForInput(out failurePoint))
                    return uiClass;
                if (failurePoint != null)
                    uiClass = failurePoint;
            }
            return uiClass;
        }

        public void NavigateInto() => NavigateInto(false);

        public void NavigateInto(bool isDefault) => RequestKeyFocus(isDefault ? KeyFocusReason.Default : KeyFocusReason.Other);

        public void RequestKeyFocus() => RequestKeyFocus(KeyFocusReason.Other);

        private void RequestKeyFocus(KeyFocusReason keyfocusReason)
        {
            if (!IsKeyFocusable())
                return;
            UISession.InputManager.Queue.RequestKeyFocus(this, keyfocusReason);
        }

        public bool IsValid => !IsDisposed;

        public static FocusStateHandler GetFocusUpdateProc(InputDeviceType focusType)
        {
            switch (focusType)
            {
                case InputDeviceType.Keyboard:
                    return s_updateKeyFocusStates;
                case InputDeviceType.Mouse:
                    return s_updateMouseFocusStates;
                default:
                    return null;
            }
        }

        private void RevalidateUsage(bool recursiveFlag, bool knownDisabledFlag)
        {
            if (!IsZoned)
                return;
            UISession.InputManager.RevalidateInputSiteUsage(this, recursiveFlag, knownDisabledFlag);
        }

        internal void OnInputEnabledChanged()
        {
            if (!IsZoned)
                return;
            RevalidateUsage(true, !InputEnabled);
            UpdateMouseHandling(null);
            if (!Enabled || Parent != null && !Parent.FullyEnabled)
                return;
            NotifyFullyEnabledChange();
        }

        IRawInputSite ICookedInputSite.RawInputSource
        {
            get
            {
                IRawInputSite rawInputSite = null;
                if (_rootItem != null)
                    rawInputSite = _rootItem.RendererVisual;
                return rawInputSite;
            }
        }

        public void DeliverInput(InputInfo info, EventRouteStages stage)
        {
            if (stage != EventRouteStages.Unhandled)
            {
                switch (info.EventType)
                {
                    case InputEventType.CommandDown:
                    case InputEventType.CommandUp:
                    case InputEventType.KeyDown:
                    case InputEventType.KeyUp:
                    case InputEventType.KeyCharacter:
                    case InputEventType.MouseMove:
                    case InputEventType.MousePrimaryUp:
                    case InputEventType.MouseSecondaryUp:
                    case InputEventType.MouseWheel:
                    case InputEventType.DragEnter:
                    case InputEventType.DragOver:
                    case InputEventType.DragLeave:
                    case InputEventType.DragDropped:
                    case InputEventType.DragComplete:
                        DeliverInputEvent(info, stage);
                        break;
                    case InputEventType.GainKeyFocus:
                        DeliverGainKeyFocus(info, stage);
                        break;
                    case InputEventType.LoseKeyFocus:
                        DeliverLoseKeyFocus(info, stage);
                        break;
                    case InputEventType.GainMouseFocus:
                        DeliverGainMouseFocus(info, stage);
                        break;
                    case InputEventType.LoseMouseFocus:
                        DeliverFocusChange(info, stage);
                        break;
                    case InputEventType.MousePrimaryDown:
                    case InputEventType.MouseSecondaryDown:
                    case InputEventType.MouseDoubleClick:
                        DeliverMouseButtonDown(info, stage);
                        break;
                }
            }
            else
            {
                if (!(info is KeyStateInfo keyStateInfo) || keyStateInfo.Action != KeyAction.Down || (keyStateInfo.SystemKey || !KeyNavigate(keyStateInfo.Key, keyStateInfo.Modifiers)))
                    return;
                keyStateInfo.MarkHandled();
            }
        }

        private void DeliverInputEvent(InputInfo info, EventRouteStages stage)
        {
            if (info.Handled)
                return;
            ForwardEventToInputHandlers(info, stage);
        }

        private void ForwardEventToInputHandlers(InputInfo info, EventRouteStages stage)
        {
            if (_inputHandlers == null)
                return;
            foreach (InputHandler inputHandler in _inputHandlers)
            {
                inputHandler.DeliverInput(this, info, stage);
                if (CheckHandled(info, inputHandler))
                    break;
            }
        }

        private void DeliverFocusChange(InputInfo info, EventRouteStages stage)
        {
            if (stage == EventRouteStages.Routed)
            {
                int num = info.Handled ? 1 : 0;
                DeliverCodeNotifications(info);
            }
            ForwardEventToInputHandlers(info, stage);
        }

        private void DeliverGainKeyFocus(InputInfo info, EventRouteStages stage)
        {
            if (stage == EventRouteStages.Direct)
                OnGainKeyFocus();
            DeliverFocusChange(info, stage);
        }

        private void DeliverLoseKeyFocus(InputInfo info, EventRouteStages stage)
        {
            if (stage == EventRouteStages.Direct)
                OnLoseKeyFocus();
            DeliverFocusChange(info, stage);
        }

        private void DeliverGainMouseFocus(InputInfo info, EventRouteStages stage)
        {
            if (stage == EventRouteStages.Direct && GetBit(Bits.KeyFocusOnMouseEnter))
                RequestKeyFocus(KeyFocusReason.MouseEnter);
            DeliverFocusChange(info, stage);
        }

        private void DeliverMouseButtonDown(InputInfo args, EventRouteStages stage)
        {
            if (stage == EventRouteStages.Direct && GetBit(Bits.KeyFocusOnMouseDown))
                RequestKeyFocus(KeyFocusReason.MouseDown);
            DeliverInputEvent(args, stage);
        }

        private void OnGainKeyFocus()
        {
            NavigationServices.SeedDefaultFocus(RootItem);
            if (CreateInterestOnFocus)
            {
                SetAreaOfInterest(AreaOfInterestID.Focus);
                ViewItem focusInterestTarget = FocusInterestTarget;
                if (focusInterestTarget != null && focusInterestTarget != RootItem)
                    SetAreaOfInterest(AreaOfInterestID.FocusOverride, focusInterestTarget, FocusInterestTargetMargins);
            }
            TryToPlayAnimationOnTree(RootItem, AnimationEventType.GainFocus);
        }

        private void OnLoseKeyFocus()
        {
            if (RootItem == null)
                return;
            if (CreateInterestOnFocus)
            {
                ClearAreaOfInterest(AreaOfInterestID.Focus);
                ViewItem focusInterestTarget = FocusInterestTarget;
                if (focusInterestTarget != null && focusInterestTarget != RootItem)
                    ClearAreaOfInterest(AreaOfInterestID.FocusOverride, focusInterestTarget);
            }
            TryToPlayAnimationOnTree(RootItem, AnimationEventType.LoseFocus);
        }

        private void OnGainDeepKeyFocus()
        {
            if (_inputHandlers == null)
                return;
            foreach (InputHandler inputHandler in _inputHandlers)
                inputHandler.NotifyGainDeepKeyFocus();
        }

        private void OnLoseDeepKeyFocus()
        {
            if (_inputHandlers == null)
                return;
            foreach (InputHandler inputHandler in _inputHandlers)
                inputHandler.NotifyLoseDeepKeyFocus();
        }

        private static void UpdateKeyFocusStates(
          UIClass recipient,
          bool deepFocusFlag,
          bool directFocusFlag)
        {
            if (recipient.ChangeBit(Bits.KeyFocus, deepFocusFlag))
            {
                recipient.FireNotification(NotificationID.KeyFocus);
                if (deepFocusFlag)
                    recipient.OnGainDeepKeyFocus();
                else
                    recipient.OnLoseDeepKeyFocus();
            }
            if (!recipient.ChangeBit(Bits.DirectKeyFocus, directFocusFlag))
                return;
            recipient.FireNotification(NotificationID.DirectKeyFocus);
            if (DebugOutlines.Enabled)
                DebugOutlines.NotifyInteractivityChange(recipient.Host);
            if (!directFocusFlag)
                return;
            AccessibleProxy.NotifyFocus(recipient);
        }

        public event InputEventHandler DescendentKeyFocusChange
        {
            add => AddEventHandler(s_descendantKeyFocusChangedEvent, value);
            remove => RemoveEventHandler(s_descendantKeyFocusChangedEvent, value);
        }

        private static void UpdateMouseFocusStates(
          UIClass recipient,
          bool deepFocusFlag,
          bool directFocusFlag)
        {
            if (recipient.ChangeBit(Bits.MouseFocus, deepFocusFlag))
                recipient.FireNotification(NotificationID.MouseFocus);
            if (!recipient.ChangeBit(Bits.DirectMouseFocus, directFocusFlag))
                return;
            recipient.FireNotification(NotificationID.DirectMouseFocus);
            if (!DebugOutlines.Enabled)
                return;
            DebugOutlines.NotifyInteractivityChange(recipient.Host);
        }

        public event InputEventHandler DescendentMouseFocusChange
        {
            add => AddEventHandler(s_descendantMouseFocusChangedEvent, value);
            remove => RemoveEventHandler(s_descendantMouseFocusChangedEvent, value);
        }

        private void DeliverCodeNotifications(InputInfo info)
        {
            EventCookie focusChangedEvent = EventCookie.NULL;
            switch (info.EventType)
            {
                case InputEventType.GainKeyFocus:
                case InputEventType.LoseKeyFocus:
                    focusChangedEvent = s_descendantKeyFocusChangedEvent;
                    break;
                case InputEventType.GainMouseFocus:
                case InputEventType.LoseMouseFocus:
                    focusChangedEvent = s_descendantMouseFocusChangedEvent;
                    break;
            }
            if (!(focusChangedEvent != EventCookie.NULL) || !(GetEventHandler(focusChangedEvent) is InputEventHandler eventHandler))
                return;
            eventHandler(this, info);
        }

        public void ApplyLayoutOutput(
          ViewItem subjectItem,
          bool parentFullyVisible,
          bool parentOffscreen,
          bool allowAnimations,
          out bool visibilityChange,
          out bool offscreenChange)
        {
            bool flag1 = subjectItem.LayoutVisible && parentFullyVisible;
            bool hasVisual = subjectItem.HasVisual;
            if (!flag1 && !hasVisual)
            {
                visibilityChange = false;
                offscreenChange = false;
            }
            else
            {
                visibilityChange = flag1 != hasVisual;
                bool flag2 = visibilityChange && hasVisual;
                bool flag3 = visibilityChange && !hasVisual;
                if (flag3)
                    subjectItem.CreateVisual(Zone.Session.RenderSession);
                Rectangle layoutBounds = subjectItem.LayoutBounds;
                Vector2 vector2 = new Vector2(layoutBounds.Width, layoutBounds.Height);
                Vector3 vector3_1 = new Vector3(layoutBounds.X, layoutBounds.Y, 0.0f);
                Vector3 vector3_2 = subjectItem.LayoutScale * subjectItem.Scale;
                Rotation layoutRotation = subjectItem.LayoutRotation;
                Vector3 oldScaleVector;
                if (!hasVisual)
                {
                    AnimateNewVisual(subjectItem, vector3_1, vector2, vector3_2, layoutRotation);
                    oldScaleVector = vector3_2;
                }
                else
                {
                    Vector2 visualSize = subjectItem.VisualSize;
                    oldScaleVector = subjectItem.VisualScale;
                }
                bool flag4 = subjectItem.LayoutOffscreen || parentOffscreen;
                offscreenChange = flag4 != subjectItem.IsOffscreen;
                if (offscreenChange)
                    subjectItem.IsOffscreen = flag4;
                if (offscreenChange || flag3)
                    UpdateMouseHandling(subjectItem);
                bool flag5 = hasVisual;
                if (flag5 && flag2)
                    flag5 = false;
                if (flag5)
                    AnimateVisual(subjectItem, vector3_1, vector2, vector3_2, layoutRotation);
                if (flag2)
                {
                    if (subjectItem.IsRoot)
                        return;
                    DestroyVisualTree(subjectItem, allowAnimations, false);
                }
                else
                {
                    if (!(oldScaleVector != vector3_2) && hasVisual)
                        return;
                    subjectItem.OnScaleChange(oldScaleVector, vector3_2);
                }
            }
        }

        public bool FindNextFocusablePeer(
          Direction searchDirection,
          RectangleF startRectangleF,
          out UIClass resultUI)
        {
            if (_rootItem != null)
                return FindNextFocusablePeerWorker(_rootItem, searchDirection, startRectangleF, out resultUI);
            resultUI = null;
            return false;
        }

        private bool FindNextFocusablePeerWorker(
          ViewItem startItem,
          Direction searchDirection,
          RectangleF startRectangleF,
          out UIClass resultUI)
        {
            INavigationSite resultSite;
            bool nextPeer = NavigationServices.FindNextPeer(startItem, searchDirection, startRectangleF, out resultSite);
            resultUI = null;
            if (resultSite != null && resultSite is ViewItem viewItem)
                resultUI = viewItem.UI;
            return nextPeer;
        }

        internal bool FindNextFocusableWithin(
          Direction searchDirection,
          RectangleF startRectangleF,
          out UIClass resultUI)
        {
            INavigationSite resultSite;
            bool nextWithin = NavigationServices.FindNextWithin(_rootItem, searchDirection, startRectangleF, out resultSite);
            resultUI = null;
            if (resultSite != null && resultSite is ViewItem viewItem)
                resultUI = viewItem.UI;
            return nextWithin;
        }

        internal NavigationClass GetNavigability(ViewItem subjectItem)
        {
            NavigationClass navigationClass = NavigationClass.None;
            if (subjectItem == _rootItem && IsKeyFocusable())
                navigationClass = NavigationClass.Direct;
            return navigationClass;
        }

        public void NotifyNavigationDestination(KeyFocusReason keyFocusReason) => RequestKeyFocus(keyFocusReason);

        public bool InboundKeyNavigation(
          Direction searchDirection,
          RectangleF startRectangleF,
          bool defaultFlag)
        {
            UIClass resultUI;
            if (!FindNextFocusableWithin(searchDirection, startRectangleF, out resultUI) || resultUI == null)
                return false;
            resultUI.NotifyNavigationDestination(defaultFlag ? KeyFocusReason.Default : KeyFocusReason.Other);
            return true;
        }

        public bool NavigateDirection(Direction direction, KeyFocusReason reason)
        {
            UIClass resultUI;
            if (!FindNextFocusablePeer(direction, RectangleF.Zero, out resultUI) || resultUI == null)
                return false;
            UIClass uiClass = this;
            resultUI.NotifyNavigationDestination(reason);
            return true;
        }

        private bool KeyNavigate(Keys key, InputModifiers modifiers)
        {
            InputHandlerModifiers modifiers1 = InputHandler.GetModifiers(modifiers);
            KeyHandler.TranslateKey(ref key, ref modifiers1);
            KeyFocusReason reason = KeyFocusReason.Directional;
            Direction direction;
            switch (key)
            {
                case Keys.Tab:
                    direction = (modifiers1 & InputHandlerModifiers.Shift) != InputHandlerModifiers.Shift ? Direction.Next : Direction.Previous;
                    reason = KeyFocusReason.Tab;
                    break;
                case Keys.Left:
                    direction = Direction.West;
                    break;
                case Keys.Up:
                    direction = Direction.North;
                    break;
                case Keys.Right:
                    direction = Direction.East;
                    break;
                case Keys.Down:
                    direction = Direction.South;
                    break;
                default:
                    return false;
            }
            return NavigateDirection(direction, reason);
        }

        public TypeSchema TypeSchema => _typeSchema;

        public void NotifyInitialized()
        {
            if (_inputHandlers != null)
            {
                foreach (InputHandler inputHandler in _inputHandlers)
                    inputHandler.NotifyUIInitialized();
            }
            if (_rootItem != null)
            {
                if (MouseInteractive)
                {
                    if (!HasMouseInteractiveContent())
                        _rootItem.MouseInteractive = true;
                }
                else if (HasMouseInteractiveContent())
                    MouseInteractive = true;
            }
            SetBit(Bits.Initialized, true);
            AccessibleProxy.NotifyCreated(this);
        }

        public object ReadSymbol(SymbolReference symbolRef)
        {
            object obj = null;
            switch (symbolRef.Origin)
            {
                case SymbolOrigin.Properties:
                case SymbolOrigin.Locals:
                    obj = _storage[symbolRef.Symbol];
                    break;
                case SymbolOrigin.Input:
                    if (_inputHandlers != null)
                    {
                        foreach (InputHandler inputHandler in _inputHandlers)
                        {
                            if (inputHandler.Name != null && ReferenceEquals(inputHandler.Name, symbolRef.Symbol))
                            {
                                obj = inputHandler;
                                break;
                            }
                        }
                        break;
                    }
                    break;
                case SymbolOrigin.Content:
                    obj = FindViewItemByName(RootItem, symbolRef.Symbol);
                    break;
                case SymbolOrigin.Reserved:
                    if (symbolRef.Symbol == "UI")
                    {
                        obj = this;
                        break;
                    }
                    break;
            }
            return obj;
        }

        public ViewItem FindViewItemByName(ViewItem item, string name)
        {
            if (ReferenceEquals(item.Name, name))
                return item;
            if (item.HideNamedChildren)
                return null;
            foreach (ViewItem child in item.Children)
            {
                if (child.UI == this)
                {
                    ViewItem viewItemByName = FindViewItemByName(child, name);
                    if (viewItemByName != null)
                        return viewItemByName;
                }
            }
            return null;
        }

        public void WriteSymbol(SymbolReference symbolRef, object value)
        {
            string symbol = symbolRef.Symbol;
            if (_storage.ContainsKey(symbol) && Utility.IsEqual(_storage[symbol], value))
                return;
            _storage[symbol] = value;
            if (symbolRef.Origin != SymbolOrigin.Properties && symbolRef.Origin != SymbolOrigin.Locals)
                return;
            bool surfaceViaHost = symbolRef.Origin == SymbolOrigin.Properties;
            FireNotification(symbol, surfaceViaHost);
        }

        public object GetProperty(string name) => _storage[name];

        public void SetProperty(string name, object value)
        {
            if (_storage.ContainsKey(name) && Utility.IsEqual(_storage[name], value))
                return;
            _storage[name] = value;
            FireNotification(name, true);
        }

        public Dictionary<object, object> Storage => _storage;

        public MarkupListeners Listeners
        {
            get => _listeners;
            set => _listeners = value;
        }

        public void ScheduleScriptRun(uint scriptId, bool ignoreErrors)
        {
            if (!_scriptRunScheduler.Pending)
                DeferredCall.Post(DispatchPriority.Script, s_executePendingScriptsHandler, this);
            _scriptRunScheduler.ScheduleRun(scriptId, ignoreErrors);
        }

        private static void ExecutePendingScripts(object args)
        {
            UIClass uiClass = (UIClass)args;
            uiClass._scriptRunScheduler.Execute(uiClass);
        }

        public object RunScript(uint scriptId, bool ignoreErrors, ParameterContext parameterContext) => _typeSchema.Run(this, scriptId, ignoreErrors, parameterContext);

        public void NotifyScriptErrors()
        {
            SetBit(Bits.ScriptEnabled, false);
            _ownerHost.NotifyChildUIScriptErrors();
            ErrorManager.ReportWarning("Script runtime failure: Scripting has been disabled for '{0}' due to runtime scripting errors", _typeSchema.Name);
        }

        public bool ScriptEnabled => GetBit(Bits.ScriptEnabled);

        protected void FireNotification(string id) => FireNotification(id, false);

        private void FireNotification(string id, bool surfaceViaHost)
        {
            _notifier.Fire(id);
            if (!surfaceViaHost || _ownerHost == null)
                return;
            _ownerHost.FireChildUINotification(id);
        }

        void INotifyObject.AddListener(Listener listener) => _notifier.AddListener(listener);

        public ViewItem ConstructNamedContent(
          string contentName,
          ParameterContext parameterContext)
        {
            return _typeSchema.ConstructNamedContent(contentName, this, parameterContext);
        }

        private bool GetBit(UIClass.Bits lookupBit) => _bits[(int)lookupBit];

        private void SetBit(UIClass.Bits changeBit, bool value) => _bits[(int)changeBit] = value;

        private bool ChangeBit(UIClass.Bits bit, bool value)
        {
            if (_bits[(int)bit] == value)
                return false;
            _bits[(int)bit] = value;
            return true;
        }

        public ViewItem RootItem => _rootItem;

        public string ID => _ownerHost.Name;

        public Host Host => _ownerHost;

        public void DeclareHost(Host ownerHost) => _ownerHost = ownerHost;

        public bool Flippable
        {
            get => GetBit(Bits.Flippable);
            set => SetBit(Bits.Flippable, value);
        }

        internal void SetRootItem(ViewItem newRootItem) => _rootItem = newRootItem;

        internal bool HasMouseInteractiveContent() => _rootItem != null && HasMouseInteractiveContentWorker(_rootItem);

        private bool HasMouseInteractiveContentWorker(ViewItem checkItem)
        {
            if (checkItem.MouseInteractive)
                return true;
            foreach (ViewItem child in checkItem.Children)
            {
                if (child.UI == this && HasMouseInteractiveContentWorker(child))
                    return true;
            }
            return false;
        }

        public object GetEventContext() => _eventContext != null ? _eventContext.Value : null;

        internal void SetEventContext(EventContext eventContext) => _eventContext = eventContext;

        public override string ToString() => $"UI[{_typeSchema.Name}]";

        private class SavedFocusState
        {
            private ViewItemID[] _path;
            private bool _focusIsDefault;

            public SavedFocusState(ViewItem item)
            {
                _path = item.IDPath;
                _focusIsDefault = item.UISession.InputManager.RawKeyFocusIsDefault;
            }

            public ViewItemID[] Path => _path;

            public bool FocusIsDefault => _focusIsDefault;
        }

        private class DeferredKeyFocusRestoreHelper
        {
            private UIClass _ui;
            private UIClass.SavedFocusState _focusState;
            private ICookedInputSite _lastFocus;

            public DeferredKeyFocusRestoreHelper(UIClass ui, UIClass.SavedFocusState focusState)
            {
                _ui = ui;
                _focusState = focusState;
                _lastFocus = ui.UISession.InputManager.RawKeyFocus;
            }

            public void FaultInChild(ViewItem bottleneck, ViewItemID item)
            {
                _ui.PendingFocusRestore = this;
                bottleneck.FaultInChild(item, new ChildFaultedInDelegate(ChildFaultedIn));
            }

            private void ChildFaultedIn(ViewItem bottleneckItem, ViewItem faultedInItem)
            {
                if (!_ui.IsZoned || _ui.PendingFocusRestore != this)
                    return;
                _ui.PendingFocusRestore = null;
                InputManager inputManager = _ui.UISession.InputManager;
                if (!inputManager.RawKeyFocusIsDefault && inputManager.RawKeyFocus != _lastFocus || (faultedInItem == null || faultedInItem.IsDisposed))
                    return;
                _ui.RestoreKeyFocus(_focusState);
            }
        }

        private enum Bits
        {
            Initialized = 1,
            IsDisposed = 2,
            CreateInterestOnFocus = 4,
            Flippable = 8,
            KeyInteractive = 16, // 0x00000010
            MouseInteractive = 32, // 0x00000020
            MouseInteractiveSet = 64, // 0x00000040
            Enabled = 128, // 0x00000080
            AppFullyEnabled = 256, // 0x00000100
            RawInputDisabled = 512, // 0x00000200
            DirectKeyFocus = 1024, // 0x00000400
            DirectMouseFocus = 2048, // 0x00000800
            KeyFocusOnMouseEnter = 4096, // 0x00001000
            KeyFocusOnMouseDown = 8192, // 0x00002000
            AllowDoubleClicks = 16384, // 0x00004000
            KeyFocus = 32768, // 0x00008000
            MouseFocus = 65536, // 0x00010000
            HasAccProxy = 131072, // 0x00020000
            ScriptEnabled = 262144, // 0x00040000
            HasAutomationInstanceId = 524288, // 0x00080000
        }
    }
}
